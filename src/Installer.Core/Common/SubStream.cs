using System;
using System.IO;

namespace Installer.Core.Common
{
    /// <summary>
    /// 把一个可定位基础流的某个区间包装成一个独立的只读流。
    /// 用途：让 <see cref="System.IO.Compression.ZipArchive"/> 直接把容器内的 payload 区间当成一个完整的 zip 来读，
    /// 而无需先把 payload 解压到临时文件。
    /// </summary>
    public sealed class SubStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _start;
        private readonly long _length;
        private readonly bool _leaveOpen;
        private long _position;

        /// <summary>构造。</summary>
        /// <param name="baseStream">基础流，必须可定位。</param>
        /// <param name="start">区间起始偏移。</param>
        /// <param name="length">区间长度。</param>
        /// <param name="leaveOpen">释放本流时是否保留基础流。</param>
        public SubStream(Stream baseStream, long start, long length, bool leaveOpen = false)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            if (!baseStream.CanSeek)
            {
                throw new ArgumentException("基础流必须可定位。", "baseStream");
            }

            if (start < 0 || length < 0 || start + length > baseStream.Length)
            {
                throw new ArgumentOutOfRangeException("length", "区间超出了基础流的范围。");
            }

            _baseStream = baseStream;
            _start = start;
            _length = length;
            _leaveOpen = leaveOpen;
            _position = 0;
        }

        /// <summary>区间起始偏移。</summary>
        public long StartOffset
        {
            get { return _start; }
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool CanWrite
        {
            get { return false; }
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
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            var remaining = _length - _position;
            if (remaining <= 0)
            {
                return 0;
            }

            var want = (int)Math.Min(count, remaining);
            _baseStream.Seek(_start + _position, SeekOrigin.Begin);
            var read = _baseStream.Read(buffer, offset, want);
            _position += read;
            return read;
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

            if (target < 0 || target > _length)
            {
                throw new IOException("定位超出了子流范围。");
            }

            _position = target;
            return _position;
        }

        /// <inheritdoc />
        public override void Flush()
        {
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_leaveOpen)
            {
                _baseStream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
