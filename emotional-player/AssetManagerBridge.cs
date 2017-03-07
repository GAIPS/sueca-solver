using System;
using System.IO;
using AssetPackage;

namespace EmotionalPlayer
{
    public interface IStorageProvider
    {
        Stream LoadFile(string absoluteFilePath, FileMode mode, FileAccess access);
        bool FileExists(string absoluteFilePath);
    }

    public class DefaultStreamingAssetsStorageProvider : IStorageProvider
    {
        public Stream LoadFile(string absoluteFilePath, FileMode mode, FileAccess access)
        {
            return File.Open(RootPath(absoluteFilePath), mode, access);
        }

        public bool FileExists(string absoluteFilePath)
        {
            var rootPath = RootPath(absoluteFilePath);
            return File.Exists(rootPath);
        }

        private static string RootPath(string path)
        {
            if (Path.IsPathRooted(path))
                return System.IO.Directory.GetCurrentDirectory() + path;
            return Path.Combine(System.IO.Directory.GetCurrentDirectory(), path);
        }
    }

    public class AssetManagerBridge : IBridge, ILog, IDataStorage
    {
        public readonly IStorageProvider _provider;
        private Object saveFileLock = new Object();

        public AssetManagerBridge()
        {
			_provider = new DefaultStreamingAssetsStorageProvider();
        }

        public void Log(Severity severity, string msg)
        {
            switch (severity)
            {
                case Severity.Critical:
                case Severity.Error:
                    Console.WriteLine(msg);
                    break;
                case Severity.Warning:
                    Console.WriteLine(msg);
                    break;
                case Severity.Information:
                case Severity.Verbose:
                    Console.WriteLine(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("severity", severity, null);
            }
        }

        public bool Delete(string fileId)
        {
            throw new InvalidOperationException();
        }

        public bool Exists(string fileId)
        {
            return _provider.FileExists(fileId);
        }

        public string[] Files()
        {
            throw new InvalidOperationException();
        }

        public string Load(string fileId)
        {
            using (var reader = new StreamReader(_provider.LoadFile(fileId, FileMode.Open, FileAccess.Read)))
            {
                return reader.ReadToEnd();
            }
        }

        public void Save(string fileId, string fileData)
        {
            lock (saveFileLock)
            {
                using (var writer = File.CreateText(fileId))
                {
                    writer.Write(fileData);
                }
            }
        }
    }
}