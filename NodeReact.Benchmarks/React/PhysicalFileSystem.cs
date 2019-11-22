using System;
using System.IO;
using React;

namespace NodeReact.Benchmarks.React
{
    public class PhysicalFileSystem : FileSystemBase
    {
        private readonly string _baseDirPath;

        public PhysicalFileSystem()
            : this(AppDomain.CurrentDomain.BaseDirectory)
        { }

        public PhysicalFileSystem(string baseDirPath)
        {
            _baseDirPath = baseDirPath;
        }


        public override string MapPath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }

            relativePath = relativePath.TrimStart('~').TrimStart('/');

            return Path.GetFullPath(Path.Combine(_baseDirPath, relativePath));
        }
    }
}
