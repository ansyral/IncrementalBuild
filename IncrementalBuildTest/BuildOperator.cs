namespace IncrementalBuild.Test
{
    using IncrementalBuild.Fx;
    using IncrementalBuild.Utility;

    public class BuildOperator : IOperator<BuildInput, string>
    {
        public string Operate(BuildInput input, BuildContext context)
        {
            // report dep
            input.ReportDependencyTo(BuildInput.ForDependencyTestInstance, context);
            return JsonUtility.ToJsonString(input);
        }
    }
}
