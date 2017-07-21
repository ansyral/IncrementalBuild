namespace IncrementalBuild.Fx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using IncrementalBuild.Utility;

    // to-do: GC style
    public class BuildCache
    {
        private const string DependencyGraphFile = "dependencygraph.cache";
        private const string InputFile = "input.cache";
        private const string OutputIndexFile = "output.index.cache";

        internal string BaseDir { get; private set; }

        internal DependencyGraph DG { get; private set; } = new DependencyGraph();

        internal Dictionary<string, string> InputCache { get; private set; } = new Dictionary<string, string>();

        internal Dictionary<string, string> OutputCacheIndex { get; private set; } = new Dictionary<string, string>();

        internal BuildCache(BuildCache other)
        {
            DG = new DependencyGraph(other.DG);
            InputCache = other.InputCache.ToDictionary(p => p.Key, p => p.Value);
            OutputCacheIndex = other.OutputCacheIndex.ToDictionary(p => p.Key, p => p.Value);
        }

        internal BuildCache()
        {
        }

        public static BuildCache Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (!Directory.Exists(path))
            {
                throw new InvalidDataException($"path {path} should be a directory!");
            }

            try
            {
                return new BuildCache
                {
                    BaseDir = path,
                    DG = DependencyGraph.Load(Path.Combine(path, DependencyGraphFile)),
                    InputCache = JsonUtility.Deserialize<Dictionary<string, string>>(Path.Combine(path, InputFile)),
                    OutputCacheIndex = JsonUtility.Deserialize<Dictionary<string, string>>(Path.Combine(path, OutputIndexFile)),
                };
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Fail to load cache file from folder {path}. Details: {ex.Message}.", ex);
                return null;
            }
        }

        public void Save(string path)
        {
            JsonUtility.Serialize(Path.Combine(path, DependencyGraphFile), DG);
            JsonUtility.Serialize(Path.Combine(path, InputFile), InputCache);
            JsonUtility.Serialize(Path.Combine(path, OutputIndexFile), OutputCacheIndex);
        }
    }
}
