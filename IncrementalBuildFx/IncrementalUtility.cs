namespace IncrementalBuild.Fx
{
    using System;
    using System.IO;

    public static class IncrementalUtility
    {
        private const int MaxRetry = 3;

        public static string GetRandomEntry(string baseDir)
        {
            string name;
            do
            {
                name = Path.GetRandomFileName();
            } while (Directory.Exists(Path.Combine(baseDir, name)) || File.Exists(Path.Combine(baseDir, name)));
            return name;
        }

        public static string CreateRandomFileName(string baseDir) =>
            RetryIO(() =>
            {
                string fileName = GetRandomEntry(baseDir);
                using (File.Create(Path.Combine(baseDir, fileName)))
                {
                    // create new zero length file.
                }
                return fileName;
            });

        public static FileStream CreateRandomFileStream(string baseDir) =>
            RetryIO(() => File.Create(Path.Combine(baseDir, GetRandomEntry(baseDir))));

        public static string CreateRandomDirectory(string baseDir) =>
            RetryIO(() =>
            {
                var folderName = GetRandomEntry(baseDir);
                Directory.CreateDirectory(Path.Combine(baseDir, folderName));
                return folderName;
            });

        public static T RetryIO<T>(Func<T> func)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    return func();
                }
                catch (IOException)
                {
                    if (count++ >= MaxRetry)
                    {
                        throw;
                    }
                }
            }
        }

        public static void RetryIO(Action action)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (IOException)
                {
                    if (count++ >= MaxRetry)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
