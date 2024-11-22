using System.Security.Cryptography;
using System.Text;

namespace Charon.IO
{
    public sealed class FileComparer(string fileName, string? version = null)
    {
        public const string CryptoName = "SHA256";

        private static HashAlgorithm? _crypto;

        private readonly string _fileName = fileName;
        private readonly string? _version = version;
        private string? _hash;

        public bool Changed
        {
            get
            {
                _hash = string.Concat(_version, GetHash(_fileName));

                return string.Compare(_hash, GetStoredHash(_fileName)) != 0;
            }
        }

        public void Success()
        {
            StoreHash(_fileName, _hash!);
        }

        public static bool Equals(string sourcePath, string targetPath)
        {
            var targetComputedHash = ComputeHash(targetPath);

            if (targetComputedHash == null)
                return !File.Exists(sourcePath);

            var sourceComputedHash = ComputeHash(sourcePath);

            if (sourceComputedHash == null ||
                sourceComputedHash.Length != targetComputedHash.Length)
                return false;

            for (int i = 0; i < sourceComputedHash.Length; i++)
            {
                if (sourceComputedHash[i] != targetComputedHash[i])
                    return false;
            }

            return true;
        }

        public static bool Move(string sourcePath, string targetPath, CancellationToken cancellationToken, int timeoutSeconds = 5, Action? beforeMove = null)
        {
            var moved = false;
            var startedUtc = DateTime.UtcNow;

            do
            {
                try
                {
                    try
                    {
                        if (!Equals(sourcePath, targetPath) &&
                            File.Exists(sourcePath))
                        {
                            beforeMove?.Invoke();

                            File.Copy(sourcePath, targetPath, true);

                            moved = true;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        File.Delete(targetPath);
                        File.Copy(sourcePath, targetPath, true);

                        moved = true;
                    }

                    break;
                }
                catch (IOException)
                {
                    if ((DateTime.UtcNow - startedUtc).TotalSeconds > timeoutSeconds)
                        throw;

                    Thread.Sleep(33);
                }

                if (cancellationToken.IsCancellationRequested)
                    return false;
            } while (true);

            File.Delete(sourcePath);

            return moved;
        }

        public static bool Copy(string sourcePath, string targetPath, CancellationToken cancellationToken, int timeoutSeconds = 5, Action? beforeCopy = null)
        {
            var copied = false;
            var startedUtc = DateTime.UtcNow;

            do
            {
                try
                {
                    try
                    {
                        if (!Equals(sourcePath, targetPath) &&
                            File.Exists(sourcePath))
                        {
                            beforeCopy?.Invoke();

                            File.Copy(sourcePath, targetPath, true);

                            copied = true;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        File.Delete(targetPath);
                        File.Copy(sourcePath, targetPath, true);

                        copied = true;
                    }

                    break;
                }
                catch (IOException)
                {
                    if ((DateTime.UtcNow - startedUtc).TotalSeconds > timeoutSeconds)
                        throw;

                    Thread.Sleep(33);
                }

                if (cancellationToken.IsCancellationRequested)
                    return false;
            } while (true);

            return copied;
        }

        public static string? GetHash(string path, Action? beforeCompute = null)
        {
            byte[]? computedHash;

            try
            {
                beforeCompute?.Invoke();

                computedHash = ComputeHash(path);
            }
            catch (Exception)
            {
                _crypto = null;

                computedHash = ComputeHash(path);
            }

            if (computedHash == null)
                return default;

            return Convert.ToHexString(computedHash);
        }

        private static byte[]? ComputeHash(string path)
        {
            if (!File.Exists(path))
                return default;

            _crypto ??= SHA256.Create();

            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var bufferedStream = new BufferedStream(fs, 65536);
                return _crypto.ComputeHash(bufferedStream);
            }
            catch (FileNotFoundException)
            {
                return default;
            }
        }

        private static string GetStoredHash(string fileName)
        {
            var storeFileName = string.Concat(fileName, '.', CryptoName);

            if (!File.Exists(storeFileName))
                return "<not yet>";

            string hash;

            using (var sr = new StreamReader(storeFileName, Encoding.UTF8, false))
            {
                hash = sr.ReadToEnd();
            }

            return hash;
        }

        private static void StoreHash(string fileName, string hash)
        {
            var storeFileName = string.Concat(fileName, '.', CryptoName);

            using var sw = new StreamWriter(storeFileName, false, Encoding.UTF8);
            sw.Write(hash);
        }
    }
}
