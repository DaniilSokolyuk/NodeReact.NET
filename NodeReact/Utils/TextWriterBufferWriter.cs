using System;
using System.Buffers;
using System.IO;

namespace NodeReact.Utils;

internal class TextWriterBufferWriter: IBufferWriter<char>
{
    private static readonly ArrayPool<char> _pagePool = ArrayPool<char>.Shared;

    private readonly TextWriter _textWriter;

    public TextWriterBufferWriter(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    private char[] _bufferWritePage;
    public void Advance(int count)
    {
        _textWriter.Write(_bufferWritePage, 0, count);
            
        _pagePool.Return(_bufferWritePage);
        _bufferWritePage = null;
    }

    public Memory<char> GetMemory(int sizeHint = 0)
    {
        try
        {
            _bufferWritePage = _pagePool.Rent(sizeHint);
        }
        catch when (_bufferWritePage != null)
        {
            _pagePool.Return(_bufferWritePage);
            _bufferWritePage = null;
            throw;
        }
            
        return _bufferWritePage.AsMemory();
    }

    public Span<char> GetSpan(int sizeHint = 0)
    {
        try
        {
            _bufferWritePage = _pagePool.Rent(sizeHint);
        }
        catch when (_bufferWritePage != null)
        {
            _pagePool.Return(_bufferWritePage);
            _bufferWritePage = null;
            throw;
        }
            
        return _bufferWritePage.AsSpan();
    }
}