using System;
using Microsoft.IO;

namespace NodeReact.Utils;

internal class PooledStream : IDisposable
{
    private readonly RecyclableMemoryStream _stream;

    private static readonly RecyclableMemoryStreamManager _manager;
    
    static PooledStream()
    {
        var blockSize = 1024 ;
        var largeBufferMultiple = 1024 * 1024;
        var maximumBufferSize = 128 * 1024 * 1024;
        
        _manager = new RecyclableMemoryStreamManager(blockSize, largeBufferMultiple, maximumBufferSize)
        {
            GenerateCallStacks = true,
            AggressiveBufferReturn = true,
            MaximumFreeLargePoolBytes = largeBufferMultiple * 4,
            MaximumFreeSmallPoolBytes = 250 * blockSize
        };
    }

    public PooledStream()
    {
        _stream = _manager.GetStream() as RecyclableMemoryStream;
    }
    
    public RecyclableMemoryStream Stream => _stream;

    public void Dispose()
    {
        _stream.Dispose();
    }
}