namespace IncrementalBuild.Fx
{
    using System;

    [Serializable]
    public class BuildCacheException : Exception
    {
        public BuildCacheException()
        {
        }

        public BuildCacheException(string message) : base(message)
        {
        }

        public BuildCacheException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
