#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;

namespace UniRx.Async.Internal
{
    internal static class Guard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowArgumentNullException<T>(T value, string paramName)
          where T : class
        {
            if (value == null) ThrowArgumentNullExceptionCore(paramName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowArgumentNullExceptionCore(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentException<T>(string message)
        {
            throw new ArgumentException(message);
        }
    }
}

#endif