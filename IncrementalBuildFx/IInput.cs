namespace IncrementalBuild.Fx
{
    using System.Collections.Generic;

    public interface IInput : IChangePropertyProvider, IDependencyProvider
    {
    }

    public class InputKeyEqualityComparer<T> : EqualityComparer<T> where T : IInput
    {
        public override bool Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x != null && y != null)
            {
                return x.Key == y.Key;
            }
            return false;
        }

        public override int GetHashCode(T obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return obj.Key.GetHashCode();
        }
    }
}
