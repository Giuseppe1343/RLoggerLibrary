using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace RLogger.Persistence
{
    /// <summary>
    /// Represents a single log entry in the Write-Ahead Log. This struct is used for in-memory representation and serialization/deserialization to/from the Memory Mapped File.
    /// 
    /// File layout for each entry:
    /// ├─────────────────────────────────────────────┤
    /// │ Status:         1 byte                      │
    /// │ LogId:         16 bytes (Guid)              │
    /// │ Timestamp:     10 bytes (long + short)      │
    /// │ Level:          1 byte                      │
    /// │ EventId:        4 bytes (int)               │
    /// │ CategoryLength: 4 bytes (int)               │
    /// │ MessageLength:  4 bytes (int)               │
    /// │ Category:       N bytes (UTF8)              │
    /// │ Message:        N bytes (UTF8)              │
    /// ├─────────────────────────────────────────────┤
    /// </summary>
    internal struct WalEntry
    {
        // ── Layout constants ─────────────────────────────────────────
        private const int StatusOffset = 0;   // 1  byte
        private const int LogIdOffset = 1;   // 16 bytes
        private const int TimestampOffset = 17;  // 10 bytes
        private const int TimestampOffset_Ticks = 17;  // 8 bytes
        private const int TimestampOffset_Offset = 25;  // 2 bytes (reserved for future use, e.g. timezone)
        private const int LevelOffset = 27;  // 1  byte
        private const int EventIdOffset = 28;  // 4  bytes
        private const int CategoryLengthOffset = 32;  // 4  bytes
        private const int MessageLengthOffset = 36;  // 4  bytes
        private const int PayloadOffset = 40;  // variable from here

        internal const byte StatusPending = 0;
        internal const byte StatusAcknowledged = 1;

        // ── Fields ───────────────────────────────────────────────────
        public byte Status;
        public Guid LogId;
        public DateTimeOffset Timestamp;
        public LogLevel Level;
        public int EventId;
        public string Category;
        public string Message;

        // ── Size ─────────────────────────────────────────────────────

        /// <summary>Total byte size of this entry as written to the MMF.</summary>
        public int ByteSize =>
            1 +  // Status
            16 + // LogId
            10 +  // Timestamp
            1 +  // Level
            4 +  // EventId
            4 +  // CategoryLength
            4 +  // MessageLength
            Encoding.UTF8.GetByteCount(Category) +
            Encoding.UTF8.GetByteCount(Message);

        // ── Write ────────────────────────────────────────────────────
        public void WriteTo(MemoryMappedViewAccessor accessor, long entryOffset)
        {
            var categoryBytes = Encoding.UTF8.GetBytes(Category);
            var messageBytes = Encoding.UTF8.GetBytes(Message);

            accessor.Write(entryOffset + StatusOffset, StatusPending);
            WriteGuid(accessor, entryOffset + LogIdOffset, LogId);
            accessor.Write(entryOffset + TimestampOffset_Ticks, Timestamp.UtcTicks);
            accessor.Write(entryOffset + TimestampOffset_Offset, (short)Timestamp.Offset.TotalMinutes);
            accessor.Write(entryOffset + LevelOffset, (byte)Level);
            accessor.Write(entryOffset + EventIdOffset, EventId);
            accessor.Write(entryOffset + CategoryLengthOffset, categoryBytes.Length);
            accessor.Write(entryOffset + MessageLengthOffset, messageBytes.Length);

            long payloadOffset = entryOffset + PayloadOffset;
            accessor.WriteArray(payloadOffset, categoryBytes, 0, categoryBytes.Length);
            accessor.WriteArray(payloadOffset + categoryBytes.Length, messageBytes, 0, messageBytes.Length);
        }

        // ── Read ─────────────────────────────────────────────────────
        public static WalEntry ReadFrom(MemoryMappedViewAccessor accessor, long entryOffset, out int totalBytesRead)
        {
            var status = accessor.ReadByte(entryOffset + StatusOffset);
            var logId = ReadGuid(accessor, entryOffset + LogIdOffset);
            long utcTicks = accessor.ReadInt64(entryOffset + TimestampOffset_Ticks);
            short offsetMinutes = accessor.ReadInt16(entryOffset + TimestampOffset_Offset);
            var level = (LogLevel)accessor.ReadByte(entryOffset + LevelOffset);
            int eventId = accessor.ReadInt32(entryOffset + EventIdOffset);

            int categoryLength = accessor.ReadInt32(entryOffset + CategoryLengthOffset);
            int messageLength = accessor.ReadInt32(entryOffset + MessageLengthOffset);

            long payloadOffset = entryOffset + PayloadOffset;
            var categoryBytes = new byte[categoryLength];
            var messageBytes = new byte[messageLength];
            accessor.ReadArray(payloadOffset, categoryBytes, 0, categoryLength);
            accessor.ReadArray(payloadOffset + categoryBytes.Length, messageBytes, 0, messageLength);
            
            totalBytesRead = PayloadOffset + categoryLength + messageLength;

            return new WalEntry
            {
                Status = status,
                LogId = logId,
                Timestamp = new DateTimeOffset(utcTicks, TimeSpan.FromMinutes(offsetMinutes)),
                Level = level,
                EventId = eventId,
                Category = string.Intern(Encoding.UTF8.GetString(categoryBytes)),
                Message = Encoding.UTF8.GetString(messageBytes)
            };
        }

        // ── Acknowledge ──────────────────────────────────────────────
        public static void Acknowledge(MemoryMappedViewAccessor accessor, long entryOffset)
        {
            accessor.Write(entryOffset + StatusOffset, StatusAcknowledged);
        }

        // ── Conversion ───────────────────────────────────────────────
        public LogMessage ToLogMessage() =>
            new(LogId, Timestamp, Level, Category, EventId, Message);

        public static WalEntry FromLogMessage(LogMessage message) => new()
        {
            Status = StatusPending,
            LogId = message.LogId,
            Timestamp = message.Timestamp,
            Level = message.Level,
            EventId = message.EventId,
            Category = message.Category,
            Message = message.Message
        };

        // ── Helpers ──────────────────────────────────────────────────
        private static void WriteGuid(MemoryMappedViewAccessor accessor, long offset, Guid guid)
        {
            var bytes = guid.ToByteArray();
            accessor.WriteArray(offset, bytes, 0, 16);
        }

        private static Guid ReadGuid(MemoryMappedViewAccessor accessor, long offset)
        {
            var bytes = new byte[16];
            accessor.ReadArray(offset, bytes, 0, 16);
            return new Guid(bytes);
        }
    }

    /// <summary>
    /// Write-Ahead Log persistence using a Memory Mapped File.
    /// 
    /// File layout:
    /// ┌─────────────────────────────────────────────┐
    /// │ Header (8 bytes)                            │
    /// │   WriteCursor: Int64                        │
    /// ├─────────────────────────────────────────────┤
    /// │ Entry 0 (WalEntry.ByteSize)                 │
    /// ├─────────────────────────────────────────────┤
    /// │ Entry 1 ...                                 │
    /// └─────────────────────────────────────────────┘
    /// </summary>
    internal sealed class MmfWalPersistence : ILogPersistence, IDisposable
    {
        // Header
        private const int HeaderSize = 8;

        private readonly Lock _writeLock = new();
        private readonly SafeFileHandle _handle;
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;

        private bool _disposed;
        private long _capacity;
        private long _dataEndOffset; // byte offset where next entry will be written — stored in header

        // In-memory index: LogId → file offset of entry start
        // Rebuilt from MMF on startup via GetPending()
        private readonly ConcurrentDictionary<Guid, long> _offsetIndex = new();

        public MmfWalPersistence(string path, long capacity)
        {
            _handle = File.OpenHandle(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            OpenOrCreate(capacity);

            _dataEndOffset = _accessor.ReadInt64(0); // Load write cursor from header
            if (_dataEndOffset == 0)
                _dataEndOffset = HeaderSize; // Initialize cursor if file was new/empty

            LoadPendingIndex(); // build in-memory index of pending entries for quick acknowledgment
        }

        public void Write(LogMessage message)
        {
            var entry = WalEntry.FromLogMessage(message);

            lock (_writeLock)
            {
                long newDataEndOffset = _dataEndOffset + entry.ByteSize;
                if (newDataEndOffset >= _capacity)
                {
                    Close(includeFile: false);
                    OpenOrCreate(_capacity * 2); // double capacity for next time
                }

                long entryOffset = _dataEndOffset;
                entry.WriteTo(_accessor, entryOffset);
                _offsetIndex[entry.LogId] = entryOffset;
                _dataEndOffset = newDataEndOffset;
                _accessor.Write(0, _dataEndOffset);
            }
        }

        public void Acknowledge(Guid logId)
        {
            if (!_offsetIndex.TryRemove(logId, out long entryOffset))
                return;

            WalEntry.Acknowledge(_accessor, entryOffset);
        }

        public IEnumerable<LogMessage> GetPending()
        {
            foreach (var (_, entryOffset) in _offsetIndex)
            {
                var entry = WalEntry.ReadFrom(_accessor, entryOffset, out _);
                yield return entry.ToLogMessage();
            }
        }

        public void Dispose()
        {
            if (_disposed) 
                return;

            lock (_writeLock)
            {
                if (_disposed) 
                    return;
                
                _disposed = true;

                if (_offsetIndex.IsEmpty)
                {
                    _dataEndOffset = HeaderSize;
                    _accessor.Write(0, _dataEndOffset);
                }
                Close(includeFile: true);
            }
        }

        private void LoadPendingIndex()
        {
            long currentOffset = HeaderSize;
            while (currentOffset < _dataEndOffset)
            {
                var entry = WalEntry.ReadFrom(_accessor, currentOffset, out int totalBytesRead);
                if (entry.Status == WalEntry.StatusPending)
                {
                    _offsetIndex[entry.LogId] = currentOffset;
                }
                currentOffset += totalBytesRead; // hop to next entry
            }
        }

        // Helper for MMF initialization and resizing. Not part of ILogPersistence interface.

        [MemberNotNull(nameof(_mmf), nameof(_accessor))]
        private void OpenOrCreate(long capacity)
        {
            long fileLength = RandomAccess.GetLength(_handle);

            if (fileLength == 0 || fileLength < capacity)
            {
                RandomAccess.SetLength(_handle, capacity);
                fileLength = capacity;
            }
            else if (fileLength > capacity)
            {
                Span<byte> buffer = stackalloc byte[8];
                RandomAccess.Read(_handle, buffer, 0);
                var dataEndOffset = BitConverter.ToInt64(buffer);
                if (capacity > dataEndOffset)
                {
                    RandomAccess.SetLength(_handle, capacity);
                    fileLength = capacity;
                }
            }

            _capacity = fileLength;
            _mmf = MemoryMappedFile.CreateFromFile(_handle, null, 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
            _accessor = _mmf.CreateViewAccessor();
            Debug.Assert(_capacity == _accessor.Capacity, "MMF capacity does not match expected capacity after initialization.");
        }

        private void Close(bool includeFile = false)
        {
            _accessor.Flush();
            _accessor.Dispose();
            _mmf.Dispose();
            if (includeFile)
                _handle.Dispose();
        }
    }
}
