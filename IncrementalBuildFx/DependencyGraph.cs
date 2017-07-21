namespace IncrementalBuild.Fx
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using IncrementalBuild.Utility;

    public class DependencyGraph
    {
        // index key is dependencyTo, value is dependencyItem
        private ConcurrentDictionary<IDependencyProvider, List<DependencyItem>> _dependencies = new ConcurrentDictionary<IDependencyProvider, List<DependencyItem>>();
        // indexkey is reportedBy
        private ConcurrentDictionary<IDependencyProvider, List<DependencyItem>> _reportedBy = new ConcurrentDictionary<IDependencyProvider, List<DependencyItem>>();

        internal DependencyGraph(Dictionary<IDependencyProvider, List<DependencyItem>> deps)
        {
            _dependencies = new ConcurrentDictionary<IDependencyProvider, List<DependencyItem>>(deps);
            foreach (var d in (from deplist in deps.Values
                               from dep in deplist
                               select dep).Distinct())
            {
                if (d.ReportedBy == null)
                {
                    throw new InvalidDataException($"missing reportedby!");
                }
                UpdateIndex(_reportedBy, d, d.ReportedBy);
            }
        }

        internal DependencyGraph() : this(new Dictionary<IDependencyProvider, List<DependencyItem>>())
        {
        }

        internal DependencyGraph(DependencyGraph other) : this(other._dependencies.ToDictionary(p => p.Key, p => new List<DependencyItem>(p.Value)))
        {

        }

        public void ReportDependency(DependencyItem dependency)
        {
            if (dependency == null || dependency.From == null || dependency.To == null || dependency.ReportedBy == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            // ignore self-dependency
            if (dependency.From == dependency.To)
            {
                return;
            }
            UpdateIndex(_dependencies, dependency, dependency.To);
            UpdateIndex(_reportedBy, dependency, dependency.ReportedBy);
        }

        public void ClearDependency(IDependencyProvider reportedBy)
        {
            List<DependencyItem> items;
            if (!_reportedBy.TryRemove(reportedBy, out items))
            {
                return;
            }
            lock (_dependencies)
            {
                foreach (var item in items)
                {
                    _dependencies[item.To].Remove(item);
                }
            }
        }

        public List<IDependencyProvider> GetDependencyFrom(IDependencyProvider to)
        {
            List<DependencyItem> v;
            if (!_dependencies.TryGetValue(to, out v))
            {
                v = new List<DependencyItem>();
            }
            return v.Select(i => i.From).ToList();
        }

        public static DependencyGraph Load(string path)
        {
            return new DependencyGraph(JsonUtility.Deserialize<Dictionary<IDependencyProvider, List<DependencyItem>>>(path));
        }

        public void Save(string path)
        {
            JsonUtility.Serialize(path, _dependencies);
        }

        private static void UpdateIndex(ConcurrentDictionary<IDependencyProvider, List<DependencyItem>> indices, DependencyItem dependency, IDependencyProvider key)
        {
            indices.AddOrUpdate(
                key,
                new List<DependencyItem> { dependency },
                (k, old) =>
                {
                    var l = new List<DependencyItem> { dependency };
                    l.AddRange(old);
                    return l;
                });
        }
    }

    public class DependencyItem
    {
        public IDependencyProvider From { get; set; }

        public IDependencyProvider To { get; set; }

        public IDependencyProvider ReportedBy { get; set; }
    }
}
