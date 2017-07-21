namespace IncrementalBuild.Test
{
    using System.Collections.Generic;

    using IncrementalBuild.Fx;

    using Newtonsoft.Json;

    public class BuildInput : IInput
    {
        public static BuildInput ForDependencyTestInstance = new BuildInput
        {
            Property1 = "heihei",
            Property2 = 2,
            Property3 = new List<string> { "hengheng" },
            Property4 = "haha",
        };

        public string Property1 { get; set; }

        public int Property2 { get; set; }

        public List<string> Property3 { get; set; }

        public string Property4 { get; set; }

        [JsonIgnore]
        public string Key
        {
            get
            {
                return Property1;
            }
            set
            {
                Property1 = value;
            }
        }

        public Dictionary<string, object> GetChangeProperties()
        {
            return new Dictionary<string, object>
            {
                { nameof(Property1), Property1 },
                { nameof(Property2), Property2 },
                { nameof(Property3), Property3 },
            };
        }

        public void ReportDependencyFrom(IDependencyProvider from, BuildContext context)
        {
            context.DG.ReportDependency(new DependencyItem
            {
                From = from,
                To = this,
                ReportedBy = this,
            });
        }

        public void ReportDependencyTo(IDependencyProvider to, BuildContext context)
        {
            context.DG.ReportDependency(new DependencyItem
            {
                From = this,
                To = to,
                ReportedBy = this,
            });
        }
    }
}
