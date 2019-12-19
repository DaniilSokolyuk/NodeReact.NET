using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NodeReact.Allocator;

namespace NodeReact.Utils
{
    public sealed class ArrayPooledTextWriter : TextWriter
    {
        private static readonly ArrayPool<char> _pagePool = ArrayPool<char>.Shared;
        private static readonly ArrayPool<char[]> _rootPool = ArrayPool<char[]>.Shared;

        public ArrayPooledTextWriter(int minPageSize = 16384)
        {
            if (minPageSize != 4096 && minPageSize != 8192 && minPageSize != 16384)
            {
                throw new ArgumentOutOfRangeException(nameof(minPageSize));
            }

            _minPageSize = minPageSize;
        }

        private readonly int _minPageSize;

        private int _charIndex;
        private int _pageIndex = -1;
        private char[][] _pages = _rootPool.Rent(32);

        public int Length { get; private set; }

        public override Encoding Encoding { get; }

        public override void Write(char value)
        {
            var page = GetCurrentPage();
            page[_charIndex++] = value;

            Length++;
        }

        public override void Write(char[] buffer) => Write(new ReadOnlySpan<char>(buffer));

        public override void Write(string value) => Write(value.AsSpan());

        public override void Write(char[] buffer, int index, int count) => Write(new ReadOnlySpan<char>(buffer, index, count));

        public override void Write(ReadOnlySpan<char> buffer)
        {
            if (buffer.Length > 0)
            {
                var index = 0;
                var count = buffer.Length;

                while (count > 0)
                {
                    var page = GetCurrentPage();
                    var copyLength = Math.Min(count, page.Length - _charIndex);

                    buffer
                        .Slice(index, copyLength)
                        .CopyTo(new Span<char>(page, _charIndex, copyLength));

                    _charIndex += copyLength;
                    index += copyLength;
                    count -= copyLength;
                }

                Length += buffer.Length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char[] GetCurrentPage()
        {
            if (_pageIndex == -1 || _charIndex == _pages[_pageIndex].Length)
            {
                var page = NewPage();
                if (_pageIndex == _pages.Length - 1)
                {
                    char[][] rootPages = null;
                    try
                    {
                        rootPages = _rootPool.Rent(_pages.Length * 2);
                        Array.Copy(_pages, 0, rootPages, 0, _pageIndex + 1);
                        _rootPool.Return(_pages);
                        _pages = rootPages;
                    }
                    catch when (rootPages != null)
                    {
                        _rootPool.Return(rootPages);
                        throw;
                    }
                }

                _pages[++_pageIndex] = page;
                _charIndex = 0;

                return page;
            }

            return _pages[_pageIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char[] NewPage()
        {
            char[] page = null;
            try
            {
                page = _pagePool.Rent(_minPageSize);
            }
            catch when (page != null)
            {
                _pagePool.Return(page);
                throw;
            }

            return page;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            for (var i = 0; i < _pageIndex + 1; i++)
            {
                _pagePool.Return(_pages[i]);
            }

            _rootPool.Return(_pages);
            _pages = null;
        }

        public IMemoryOwner<char> GetMemoryOwner()
        {
            var length = Length;

            var charBuffer = BufferAllocator.Instance.AllocateChar(length);
            var spanBuffer = charBuffer.Memory.Span;

            int index = 0;

            for (var i = 0; i < _pageIndex + 1; i++)
            {
                var page = _pages[i];
                var pageLength = Math.Min(length, page.Length);

                page.AsSpan(0, pageLength).CopyTo(spanBuffer.Slice(index, pageLength));

                length -= pageLength;
                index += pageLength;
            }

            return charBuffer;
        }

        public override string ToString()
        {
            using (var buffer = GetMemoryOwner())
            {
                return new string(buffer.Memory.Span);
            }
        }
    }
}

