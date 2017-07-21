namespace IncrementalBuild.Fx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Builder<Input, Output> where Input : IInput
    {
        public IOperator<Input, Output> BuildOperation { get; private set; }

        public Builder(IOperator<Input, Output> operation)
        {
            BuildOperation = operation;
        }

        public List<Output> BuildWithIncremental(IEnumerable<Input> inputCollection, BuildContext context)
        {
            if (inputCollection == null)
            {
                throw new ArgumentNullException(nameof(inputCollection));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.UpdateInputCache(inputCollection);
            List<Output> outputs = new List<Output>();
            if (!context.CanIncremental)
            {
                outputs.AddRange(inputCollection.Select(input => GetOutputAndUpdateCache(input, context)));
                context.SaveCache();
                return outputs;
            }
            var changes = context.GetChangesWithDependencies(inputCollection).ToList();
            if (changes.Count > 0)
            {
                outputs.AddRange(changes.Select(change => GetOutputAndUpdateCache(change, context)));
            }
            var unChanged = inputCollection.Except(changes, new InputKeyEqualityComparer<Input>());
            if (unChanged.Any())
            {
                outputs.AddRange(context.GetCachedOutput<Input, Output>(unChanged));
            }
            context.SaveCache();
            return outputs;
        }

        private Output GetOutputAndUpdateCache(Input input, BuildContext context)
        {
            var output = BuildOperation.Operate(input, context);
            context.DG.ClearDependency(input);
            context.UpdateOutputCache(input, output);
            return output;
        }
    }
}
