using System.ComponentModel;
using Charon.IO;

namespace Charon.Core.Tests.IO
{
    public sealed class FileComparerTests
    {
        [Fact]
        public void DefaultCryptoName()
        {
            Assert.Equal("SHA256", FileComparer.CryptoName);
        }

        [Fact]
        public void Copy()
        {
            var sourcePath = "not existing.txt";
            var targetPath = "not existing copy.txt";

            Assert.False(FileComparer.Copy(sourcePath, targetPath, default));

            sourcePath = "Copy.Source.txt";
            targetPath = "Copy.Target.txt";

            File.WriteAllText(sourcePath, "content");
            File.Delete(targetPath);

            Assert.True(FileComparer.Copy(sourcePath, targetPath, default));
            File.WriteAllText(sourcePath, "content");
            Assert.False(FileComparer.Copy(sourcePath, targetPath, default));

            File.WriteAllText(sourcePath, "content2");
            File.Delete(targetPath);

            var count = 0;

            Assert.True(FileComparer.Copy(sourcePath, targetPath, default, 1, () =>
            {
                if (count++ < 3)
                    throw new UnauthorizedAccessException();
            }));
            Assert.False(FileComparer.Copy(sourcePath, targetPath, default));

            File.WriteAllText(sourcePath, "new content");

            var finishUtc = DateTime.UtcNow.AddSeconds(6);

            Assert.Throws<IOException>(() => FileComparer.Copy(sourcePath, targetPath, default, 1, () =>
            {
                if (finishUtc > DateTime.UtcNow)
                    throw new IOException();
            }));

            var cts = new CancellationTokenSource();
            count = 0;

            Assert.False(FileComparer.Copy(sourcePath, targetPath, cts.Token, 1, () =>
            {
                if (count++ == 2)
                    cts.Cancel();

                throw new IOException();
            }));

            Assert.True(FileComparer.Copy(sourcePath, targetPath, default));
            Assert.False(FileComparer.Copy(sourcePath, targetPath, default));
        }

        [Fact]
        public void EqualsFile()
        {
            var sourcePath = "not existing.txt";
            var targetPath = "not existing copy.txt";

            Assert.True(FileComparer.Equals(sourcePath, targetPath));

            sourcePath = "EqualsFile.Source.txt";
            targetPath = "EqualsFile.Target.txt";

            File.Delete(sourcePath);
            File.WriteAllText(targetPath, "t-content");
            Assert.False(FileComparer.Equals(sourcePath, targetPath));

            File.WriteAllText(sourcePath, "s-content");
            File.Delete(targetPath);

            Assert.False(FileComparer.Equals(sourcePath, targetPath));
            Assert.True(FileComparer.Copy(sourcePath, targetPath, default));
            Assert.True(FileComparer.Equals(sourcePath, targetPath));

            File.WriteAllText(sourcePath, "new content");

            Assert.True(FileComparer.Copy(sourcePath, targetPath, default));
            Assert.True(FileComparer.Equals(sourcePath, targetPath));
        }

        [Fact]
        public void GetHash()
        {
            var path = "not existing.txt";

            Assert.Null(FileComparer.GetHash(path));

            path = "GetHash.txt";

            File.WriteAllText(path, "content");

            var actual = FileComparer.GetHash(path);
            Assert.Equal("ED7002B439E9AC845F22357D822BAC1444730FBDB6016D3EC9432297B9EC9F73", actual);

            actual = FileComparer.GetHash(path, () => throw new Exception());
            Assert.Equal("ED7002B439E9AC845F22357D822BAC1444730FBDB6016D3EC9432297B9EC9F73", actual);
        }

        [Fact]
        public void Move()
        {
            var sourcePath = "not existing.txt";
            var targetPath = "not existing copy.txt";

            Assert.False(FileComparer.Move(sourcePath, targetPath, default));

            sourcePath = "Move.Source.txt";
            targetPath = "Move.Target.txt";

            File.WriteAllText(sourcePath, "content");
            File.Delete(targetPath);

            Assert.True(FileComparer.Move(sourcePath, targetPath, default));
            File.WriteAllText(sourcePath, "content");
            Assert.False(FileComparer.Move(sourcePath, targetPath, default));

            File.WriteAllText(sourcePath, "content2");
            File.Delete(targetPath);

            var count = 0;

            Assert.True(FileComparer.Move(sourcePath, targetPath, default, 1, () =>
            {
                if (count++ < 3)
                    throw new UnauthorizedAccessException();
            }));
            Assert.False(FileComparer.Move(sourcePath, targetPath, default));

            File.WriteAllText(sourcePath, "new content");

            var finishUtc = DateTime.UtcNow.AddSeconds(6);

            Assert.Throws<IOException>(() => FileComparer.Move(sourcePath, targetPath, default, 1, () =>
            {
                if (finishUtc > DateTime.UtcNow)
                    throw new IOException();
            }));

            var cts = new CancellationTokenSource();
            count = 0;

            Assert.False(FileComparer.Move(sourcePath, targetPath, cts.Token, 1, () =>
            {
                if (count++ == 2)
                    cts.Cancel();

                throw new IOException();
            }));

            Assert.True(FileComparer.Move(sourcePath, targetPath, default));
            Assert.False(FileComparer.Move(sourcePath, targetPath, default));
        }

        [Fact]
        public void Changed()
        {
            var path = "Changed.txt";
            File.Delete(path);
            File.Delete(string.Concat(path, '.', FileComparer.CryptoName));

            var fc = new FileComparer(path);
            Assert.True(fc.Changed);
            fc.Success();
            Assert.False(fc.Changed);

            File.WriteAllText(path, "content");
            Assert.True(fc.Changed);
            fc.Success();
            Assert.False(fc.Changed);

            File.WriteAllText(path, "content2");
            Assert.True(fc.Changed);
            fc.Success();
            Assert.False(fc.Changed);
        }

        [Fact]
        public void ChangedVersion()
        {
            var path = "ChangedVersion.txt";

            File.Delete(path);

            var version = "4711";
            var fc = new FileComparer(path, version);
        }
    }
}
