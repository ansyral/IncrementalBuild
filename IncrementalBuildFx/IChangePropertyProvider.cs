namespace IncrementalBuild.Fx
{
    using System.Collections.Generic;

    public interface IChangePropertyProvider
    {
        Dictionary<string, object> GetChangeProperties();

        string Key { get; set; }
    }
}
