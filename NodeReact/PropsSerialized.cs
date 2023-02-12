using System;
using Microsoft.IO;
using NodeReact.Utils;

namespace NodeReact;

internal class PropsSerialized : IDisposable
{
    private PooledStream _pooledStream;

    public PropsSerialized(PooledStream pooledStream)
    {
        _pooledStream = pooledStream;
    }
    
    public RecyclableMemoryStream Stream => _pooledStream.Stream;

    public void Dispose()
    {
        _pooledStream?.Dispose();
    }
}