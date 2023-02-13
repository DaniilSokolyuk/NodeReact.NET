using System;
using System.Buffers;
using System.IO;

namespace NodeReact.Utils;

internal class TextWriterBufferWriter: IBufferWriter<char>
{
    private static readonly MemoryPool<char> _memoryPool = MemoryPool<char>.Shared;

    private readonly TextWriter _textWriter;

    public TextWriterBufferWriter(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    private IMemoryOwner<char> _memoryOwner;
    public void Advance(int count)
    {
        _textWriter.Write(_memoryOwner.Memory.Span.Slice(0, count));
            
        _memoryOwner.Dispose();
        _memoryOwner = null;
    }

    public Memory<char> GetMemory(int sizeHint = 0)
    {
        _memoryOwner = _memoryPool.Rent(sizeHint);
        return _memoryOwner.Memory;
    }

    public Span<char> GetSpan(int sizeHint = 0)
    {
        _memoryOwner = _memoryPool.Rent(sizeHint);
        return _memoryOwner.Memory.Span;
    }
}