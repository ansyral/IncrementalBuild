namespace IncrementalBuild.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using IncrementalBuild.Fx;

    using Xunit;

    public class IncrementalBuildTest
    {
        [Fact]
        public void TestBasic()
        {
            var buildOperator = new BuildOperator();
            var builder = new Builder<BuildInput, string>(buildOperator);
            var inputs = new[]
            {
                new BuildInput{ Property1 = "miaomiao", Property2 = 2, Property3 = BuildInput.ForDependencyTestInstance.Property3, Property4 = "wangwang" },
                new BuildInput{ Property1 = "eee", Property2 = 3, Property3 = BuildInput.ForDependencyTestInstance.Property3, Property4 = "houhou" },
                BuildInput.ForDependencyTestInstance,
            };
            string cacheFolder = Path.Combine(Directory.GetCurrentDirectory(), "intermediateFolder");
            Directory.CreateDirectory(cacheFolder);

            var firstOutput = builder.BuildWithIncremental(inputs, new BuildContext(cacheFolder));
            var secondOutput = builder.BuildWithIncremental(inputs, new BuildContext(cacheFolder));
            var forceOutput = builder.BuildWithIncremental(inputs, new BuildContext(cacheFolder, true));
            Assert.True(Equals(secondOutput, forceOutput));

            // update BuildInput.ForDependencyTestInstance, should trigger other inputs to rebuild
            BuildInput.ForDependencyTestInstance.Property3.Add("nenene");
            firstOutput = builder.BuildWithIncremental(inputs, new BuildContext(cacheFolder));
            forceOutput = builder.BuildWithIncremental(inputs, new BuildContext(cacheFolder, true));
            Assert.True(Equals(firstOutput, forceOutput));
        }

        private static bool Equals<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            return !(first.Except(second).Any() || second.Except(first).Any());
        }
    }
}
