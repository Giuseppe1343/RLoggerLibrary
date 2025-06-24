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
    // Node structure
    // +-----------------------------------------------------------------+-----------------------------------------------------------------+
    // | IsReady (1 bit) | NextOffset (63 bits) | Data (variable length) | IsReady (1 bit) | NextOffset (63 bits) | Data (variable length) |
    // +-----------------------------------------------------------------+-----------------------------------------------------------------+
    //                   |----------------------|                        ^ 
    //                               |                                   |
    //                               |  Points next node start offset    |     
    //                               +-----------------------------------+
    public class MemoryMappedQueue : IDisposable
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

        private const long WrapHeader = 0L; // Marker for wrap-around in the queue

        private readonly Lock _headLock = new(); // Lock for head operations
        private readonly Lock _tailLock = new(); // Lock for tail operations
        private readonly ReaderWriterLockSlim _queueLock = new(); // Lock for resizing the queue

        private readonly SafeFileHandle fileHandle;
        private MemoryMappedFile mmf;
        private MemoryMappedViewAccessor accessor;
        public long fileLength;

        public bool IsEmpty => HeadOffset == TailOffset;

        private long HeadOffset
        {
            get => accessor.ReadInt64(0); // Read the head offset from the file header
            set => accessor.Write(0, value); // Write the head offset to the file header
        }

        private long TailOffset
        {
            get => accessor.ReadInt64(HeaderSize); // Read the tail offset from the file header
            set => accessor.Write(HeaderSize, value); // Write the tail offset to the file header
        }

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
        }

        public void Enqueue(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            int dataLength = data.Length;

            EnsureCapacity(dataLength);

            _queueLock.EnterReadLock();

            try
            {
                long currentTail;
                long newTail;
                // Enter the lock to safely update the tail position
                lock (_tailLock)
                {
                    currentTail = TailOffset;
                    newTail = currentTail + dataLength + HeaderSize;

                    // Overflow check: if the new tail position exceeds the file length, we need to wrap around
                    if (newTail > fileLength)
                    {
                        // Write the wrap marker at the current tail position
                        accessor.Write(TailOffset, WrapHeader);

                        // Reset the tail to the start of the queue
                        currentTail = TailOffset = FileHeaderSize;
                        newTail = currentTail + dataLength + HeaderSize;
                    }

                    accessor.Write(TailOffset, newTail); // Write the new global tail offset
                    accessor.Write(currentTail, newTail); // Write the next position at the current tail offset
                }

                // Now we can safely write the data to the queue
                // After finising write, we can set the IsReady flag to indicate that the record is ready for dequeueing
                accessor.WriteSpan(newTail + HeaderSize, data);
                accessor.Write(currentTail, newTail | IsReadyMask); // Set the IsReady flag and write the length of the data
            }
            finally
            {
                _queueLock.ExitReadLock();
            }
        }

        public bool TryDequeue(out byte[]? data)
        {
            _queueLock.EnterReadLock();

            try
            {
                if (IsEmpty)
                {
                    data = null;
                    return false; // Queue is empty
                }

                long head;
                long next;
                lock (_headLock)
                {
                    head = HeadOffset;
                    next = accessor.ReadInt64(head);
                    accessor.Write(HeadOffset, next); // Write the new global head offset
                }

                if((next & IsReadyMask) == 0) // If the next record is not ready, wait until it is
                    SpinWait.SpinUntil(() => ((next = accessor.ReadInt64(head)) & IsReadyMask) != 0);

                data = new byte[next - head - HeaderSize]; // Calculate the length of the data to read
                accessor.ReadSpan(head + HeaderSize, data.AsSpan()); // Read the data from the queue
                return true;
            }
            finally
            {
                _queueLock.ExitReadLock();
            }
        }

        private void EnsureCapacity(int requiredRecordSize)
        {
            if (requiredRecordSize <= FreeSpace)
                return;

            // If not enough space, acquire write lock to resize
            _queueLock.EnterWriteLock();
            try
            {
                if (requiredRecordSize <= FreeSpace)
                    return;

                accessor.Dispose();
                mmf.Dispose();

                RandomAccess.SetLength(fileHandle, fileLength * 2); // Double the file size

                mmf = MemoryMappedFile.CreateFromFile(fileHandle, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                accessor = mmf.CreateViewAccessor();
                fileLength = accessor.Capacity;

                if (HeadOffset > TailOffset)
                {
                    using var resizableBuffer = new ResizableBuffer(stackalloc byte[StackallocByteBufferSizeLimit]);
                    long tail = TailOffset;
                    long current = HeadOffset;
                    long movedCurrent = HeadOffset = fileLength / 2;

                    // Sort the data in the queue from head to tail and place it in the newly expanded area.
                    while (true)
                    {
                        long next = accessor.ReadInt64(current);
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
                _queueLock.ExitWriteLock();
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
                EnsureLength(requiredSize);
                return _arrayToReturnToPool.AsSpan()[..requiredSize];
            }

            private void EnsureLength(int requiredSize)
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

        public static void ReadSpan(this MemoryMappedViewAccessor accessor, long offset, Span<byte> destination)
        => accessor.SafeMemoryMappedViewHandle.ReadSpan(unchecked((ulong)offset), destination);

        public static void WriteSpan(this MemoryMappedViewAccessor accessor, long offset, ReadOnlySpan<byte> source)
            => accessor.SafeMemoryMappedViewHandle.WriteSpan(unchecked((ulong)offset), source);
    }
}
