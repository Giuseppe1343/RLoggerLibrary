using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RLogger.Channel
{
    
    public unsafe class MemoryMappedQueue
    {
        private const int QueueSize = 1024 * 1024; // 1 MB
        private const string QueueNamePrefix = "RLoggerQueue_";
        private const string DefaultQueueName = "Default";

        // File Header structure
        // | HeadOffset (8 bytes) | TailOffset (8 bytes) |
        private const int FileHeaderSize = HeaderSize * 2; // 8 + 8 (head offset, tail offset) also specifies start of the queue

        // Header structure of any record in the queue
        // | IsReady (1 byte) | NextOffset (7 bytes) |
        private const int HeaderSize = 8; // Size of long in bytes

        private const long IsReadyMask = -0x8000000000000000L; // Mask to check if the record is ready (the most significant bit is set)

        private const long WrapMarker = 0L; // Marker for wrap-around in the queue

        private readonly ReaderWriterLockSlim _rwLock = new();
        private readonly SafeFileHandle fileHandle;
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor accessor;
        public long fileLength;

        public bool IsEmpty => HeadOffset == TailOffset;

        private byte* headPtr;
        private byte* tailPtr;

        private ref long HeadOffset => ref *(long*)headPtr; // Reference to head (dequeue start) offset in the queue

        private ref long TailOffset => ref *(long*)tailPtr; // Reference to tail (enqueue start) offset in the queue

        // Gets the next position in the queue based on provided offset
        private ref long Next(long offset) => ref Unsafe.AsRef<long>(headPtr + offset);

        private long FreeSpace
        {
            get
            {
                if (HeadOffset > TailOffset)
                    return HeadOffset - TailOffset - HeaderSize; // Space available when head is ahead of tail
                else
                    return Math.Max(fileLength - TailOffset - HeaderSize, HeadOffset - FileHeaderSize - HeaderSize); // Space available when tail is ahead of head, or they are equal (empty queue)
            }
        }

        public MemoryMappedQueue() : this(DefaultQueueName) { }
        public MemoryMappedQueue(string fileName)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + QueueNamePrefix + fileName;

            fileHandle = File.OpenHandle(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FileOptions.None);

            long filelength = RandomAccess.GetLength(fileHandle);

            bool initialize = filelength == 0;

            if (filelength > QueueSize)
            {
                Span<byte> buffer = stackalloc byte[FileHeaderSize];
                RandomAccess.Read(fileHandle, buffer, 0);
                long headOffset = BitConverter.ToInt64(buffer[..HeaderSize]);
                long tailOffset = BitConverter.ToInt64(buffer[HeaderSize..]);

                initialize = headOffset == tailOffset; // If head and tail offsets are equal, the queue is empty
            }

            if (initialize)
            {
                RandomAccess.SetLength(fileHandle, QueueSize);

                // Initialize the file header with head and tail offsets set to the start of the queue
                Span<byte> buffer = stackalloc byte[FileHeaderSize];
                buffer[..HeaderSize].WriteInt64(FileHeaderSize);
                buffer[HeaderSize..].WriteInt64(FileHeaderSize);
                RandomAccess.Write(fileHandle, buffer, 0);
            }

            mmf = MemoryMappedFile.CreateFromFile(fileHandle, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true); // The mapName is null because it will not be shared with other processes at the current implementation.
            accessor = mmf.CreateViewAccessor();
            fileLength = accessor.Capacity;
            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref headPtr);
            tailPtr = headPtr + HeaderSize; // Point to the tail offset after the head offset
        }

        public void Enqueue(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            int dataLength = data.Length;

            // Critical section start: acquire an upgradeable read lock to check the queue state and potentially write
            _rwLock.EnterUpgradeableReadLock();

            EnsureCapacity(dataLength);

            long currentTail = TailOffset;
            long newTail = currentTail + dataLength + HeaderSize;

            // Overflow check: if the new tail position exceeds the file length, we need to wrap around
            if (newTail > fileLength)
            {
                // Write the wrap marker at the current tail position
                accessor.Write(TailOffset, WrapMarker);

                // Reset the tail to the start of the queue
                currentTail = TailOffset = FileHeaderSize;
                newTail = currentTail + dataLength + HeaderSize;
            }

            accessor.Write(currentTail, newTail); // Write the next position at the current tail offset
            accessor.Write(TailOffset, newTail); // Write the new global tail offset

            _rwLock.ExitUpgradeableReadLock();
            // Critical section end

            // Now we can safely write the data to the queue
            // After finising write, we can set the IsReady flag to indicate that the record is ready for dequeueing
            accessor.WriteSpan(newTail + HeaderSize, data);
            accessor.Write(currentTail, newTail | IsReadyMask); // Set the IsReady flag and write the length of the data

        }

        public bool TryDequeue(out byte[]? data)
        {
            _rwLock.EnterReadLock();

            try
            {
                if (IsEmpty)
                {
                    data = null;
                    return false; // Queue is empty
                }

                long head;
                long next;
                do
                {
                    head = HeadOffset;
                    next = Next(HeadOffset);
                    data = new byte[next - head - HeaderSize]; // Calculate the length of the data to read
                    accessor.ReadSpan(head + HeaderSize, data.AsSpan()); // Read the data from the queue
                } while (Interlocked.CompareExchange(ref HeadOffset, next, head) != head); // Attempt to move head to the next position atomically
                
                return true;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        private void EnsureCapacity(int requiredRecordSize)
        {
            if (requiredRecordSize <= FreeSpace)
                return;

            // If not enough space, acquire write lock to resize
            _rwLock.EnterWriteLock();
            try
            {
                accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                accessor.Dispose();
                mmf.Dispose();

                RandomAccess.SetLength(fileHandle, fileLength * 2); // Double the file size

                mmf = MemoryMappedFile.CreateFromFile(fileHandle, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                accessor = mmf.CreateViewAccessor();
                fileLength = accessor.Capacity;
                accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref headPtr);
                tailPtr = headPtr + HeaderSize; // Point to the tail offset after the head offset

                if (HeadOffset > TailOffset)
                {
                    using var resizableBuffer = new ResizableBuffer(stackalloc byte[StackallocByteBufferSizeLimit]);
                    long tail = TailOffset;
                    long current = HeadOffset;
                    long movedCurrent = HeadOffset = fileLength / 2;

                    // Sort the data in the queue from head to tail and place it in the newly expanded area.
                    while (true)
                    {
                        long next = Next(current);
                        if (next == tail)
                            break;

                        long length = next - current;
                        Debug.Assert(length >= 0, "Length should be non-negative.");

                        long movedNext = movedCurrent + length;

                        var buffer = resizableBuffer.GetBuffer((int)length);
                        buffer.WriteInt64(movedNext); // Write the next position at the start of the buffer
                        accessor.ReadSpan(current + HeaderSize, buffer[HeaderSize..]); // Read the data into the buffer, skipping the first 8 bytes (the next position)
                        accessor.WriteSpan(movedCurrent, buffer); // Write the buffer to the new position

                        current = next;
                        movedCurrent = movedNext;
                    }
                }

            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            accessor?.SafeMemoryMappedViewHandle.ReleasePointer();
            accessor?.Dispose();
            mmf?.Dispose();
            fileHandle?.Dispose();
        }

        const int StackallocByteBufferSizeLimit = 512;
        private ref struct ResizableBuffer
        {

            private Span<byte> _bytes;
            private byte[]? _arrayToReturnToPool;

            public ResizableBuffer(Span<byte> initialBuffer)
            {
                _bytes = initialBuffer;
                _arrayToReturnToPool = null;
            }

            public Span<byte> GetBuffer(int requiredSize)
            {
                // Best case: if the required size is small enough, use stack allocation
                if (_bytes.Length >= requiredSize)
                {
                    return _bytes[..requiredSize];
                }

                // If the required size exceeds the stack allocation limit, use heap allocation
                EnsureCapacity(requiredSize);
                return _arrayToReturnToPool.AsSpan()[..requiredSize];
            }

            private void EnsureCapacity(int requiredSize)
            {
                byte[]? buffer = _arrayToReturnToPool;
                if (buffer == null)
                {
                    _arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(requiredSize);
                }
                else if (buffer.Length < requiredSize)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    _arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(requiredSize);
                }
            }

            public void Dispose()
            {
                byte[]? toReturn = _arrayToReturnToPool;
                this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
                if (toReturn != null)
                {
                    ArrayPool<byte>.Shared.Return(toReturn);
                }
            }
        }

    }

    public static class MemoryMappedViewAccessorExtensions
    {
        public static void WriteInt64(this Span<byte> span, long value)
            => Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);

        public static void ReadArray(this MemoryMappedViewAccessor accessor, long offset, byte[] destination, int destinationOffset, int count)
            => ReadSpan(accessor, offset, destination.AsSpan(destinationOffset, count));
        public static void ReadSpan(this MemoryMappedViewAccessor accessor, long offset, Span<byte> destination)
        => accessor.SafeMemoryMappedViewHandle.ReadSpan(unchecked((ulong)offset), destination);

        public static void WriteSpan(this MemoryMappedViewAccessor accessor, long offset, ReadOnlySpan<byte> source)
            => accessor.SafeMemoryMappedViewHandle.WriteSpan(unchecked((ulong)offset), source);

        public static unsafe ref T As<T>(this MemoryMappedViewAccessor accessor) where T : struct =>
          ref Unsafe.AsRef<T>(accessor.SafeMemoryMappedViewHandle.DangerousGetHandle().ToPointer());

        public static unsafe ref T As<T>(this MemoryMappedViewAccessor accessor, long offset) where T : struct
        {
            if (offset < 0 || offset + Marshal.SizeOf<T>() > accessor.Capacity)
                throw new ArgumentOutOfRangeException(nameof(offset));

            return ref Unsafe.AsRef<T>((accessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + (nint)offset).ToPointer());
        }
    }
}
