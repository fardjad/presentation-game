#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or 

using System;

namespace UniRx.Async
{
    public struct AsyncUnit : IEquatable<AsyncUnit>
    {
        public static readonly AsyncUnit Default = new AsyncUnit();

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(AsyncUnit other)
        {
            return true;
        }
    }
}
#endif