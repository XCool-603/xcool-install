using System;
using System.IO;

namespace Installer.Core.Common
{
    /// <summary>
    /// 把一个可定位基础流的某个区间包装成"从 0 开始"的可写流。
    ///
    /// **为什么必须要有这个类**：
    /// .NET 的 <see cref="System.IO.Compression.ZipArchive"/> 在
    /// <see cref="System.IO.Compression.ZipArchiveMode.Create"/> 模式下，
    /// 会把**绝对流位置**写进 EOCD 的「中央目录起始偏移」字段。
    /// 因此直接把 ZipArchive 挂在"已经写了 stub 之后"的 FileStream 上，
    /// 打出来的 payload 是一个**只有从容器起点读才正确**的 zip ——
    /// 一旦按 payload 区间单独解压（或交给 SubStream）就会报
    /// "Int64 无法容纳中央目录偏移量"。
    ///
    /// 实测（偏移 100 处写一个 115 字节的 zip）：
    ///   EOCD 记录的中央目录偏移 = 142，而 payload 只有 115 字节 → 越界。
    /// 对照组（偏移 0）一切正常。
    ///
    /// 本类让 ZipArchive 始终看到 0 起始的逻辑位置，从而写出**自洽**的 zip，
    /// 同时底层仍然是顺序写到容器的 payload 区间，不需要中间文件、内存恒定。
    /// </summary>
    public sealed class OffsetWriteStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _offset;
        private long _position;
        private long _length;

        /// <summary>构造。</summary>
        /// <param name="baseStream">基础流，必须可定位可写。</param>
        /// <param name="offset">本区间在基础流中的起始偏移。</param>
        public OffsetWriteStream(Stream baseStream, long offset)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            if (!baseStream.CanSeek)
            {
                throw new ArgumentException("基础流必须可定位。", "baseStream");
            }

            if (!baseStream.CanWrite)
            {
                throw new ArgumentException("基础流必须可写。", "baseStream");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            _baseStream = baseStream;
            _offset = offset;
            _position = 0;
            _length = 0;
        }

        /// <summary>本区间已写入的字节数（即逻辑长度）。</summary>
        public long WrittenLength
        {
            get { return _length; }
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override long Length
        {
            get { return _length; }
        }

        /// <inheritdoc />
        public override long Position
        {
            get { return _position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            _baseStream.Seek(_offset + _position, SeekOrigin.Begin);
            _baseStream.Write(buffer, offset, count);
            _position += count;
            if (_position > _length)
            {
                _length = _position;
            }
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            long target;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    target = offset;
                    break;
                case SeekOrigin.Current:
                    target = _position + offset;
                    break;
                case SeekOrigin.End:
                    target = _length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }

            if (target < 0)
            {
                throw new IOException("定位超出了偏移写入流的范围。");
            }

            _position = target;
            return _position;
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            _length = value;
            if (_offset + value > _baseStream.Length)
            {
                _baseStream.SetLength(_offset + value);
            }
        }

        /// <inheritdoc />
        public override void Flush()
        {
            _baseStream.Flush();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            // 刻意不释放基础流：调用方还要继续往后写 manifest 与 footer。
            base.Dispose(disposing);
        }
    }
}
