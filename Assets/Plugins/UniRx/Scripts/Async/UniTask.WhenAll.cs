#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using UniRx.Async.Internal;

namespace UniRx.Async
{
    public partial struct UniTask
    {
        // UniTask

        public static async UniTask<T[]> WhenAll<T>(params UniTask<T>[] tasks)
        {
            return await new WhenAllPromise<T>(tasks, tasks.Length);
        }

        public static async UniTask<T[]> WhenAll<T>(IEnumerable<UniTask<T>> tasks)
        {
            WhenAllPromise<T> promise;
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                promise = new WhenAllPromise<T>(span.Array, span.Length);
            }

            return await promise;
        }

        public static async UniTask WhenAll(params UniTask[] tasks)
        {
            await new WhenAllPromise(tasks, tasks.Length);
        }

        public static async UniTask WhenAll(IEnumerable<UniTask> tasks)
        {
            WhenAllPromise promise;
            using (var span = ArrayPoolUtil.Materialize(tasks))
            {
                promise = new WhenAllPromise(span.Array, span.Length);
            }

            await promise;
        }

        class WhenAllPromise<T>
        {
            readonly T[] result;
            int completeCount;
            Action whenComplete;
            ExceptionDispatchInfo exception;

            public WhenAllPromise(UniTask<T>[] tasks, int tasksLength)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.exception = null;
                this.result = new T[tasksLength];

                for (int i = 0; i < tasksLength; i++)
                {
                    RunTask(tasks[i], i).Forget();
                }
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask(UniTask<T> task, int index)
            {
                try
                {
                    var value = await task;
                    result[index] = value;
                    var count = Interlocked.Increment(ref completeCount);
                    if (count == result.Length)
                    {
                        TryCallContinuation();
                    }
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                }
            }

            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise<T> parent;

                public Awaiter(WhenAllPromise<T> parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.result.Length == parent.completeCount;
                    }
                }

                public T[] GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }

                    return parent.result;
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }

        class WhenAllPromise
        {
            int completeCount;
            int resultLength;
            Action whenComplete;
            ExceptionDispatchInfo exception;

            public WhenAllPromise(UniTask[] tasks, int tasksLength)
            {
                this.completeCount = 0;
                this.whenComplete = null;
                this.exception = null;
                this.resultLength = tasksLength;

                for (int i = 0; i < tasksLength; i++)
                {
                    RunTask(tasks[i], i).Forget();
                }
            }

            void TryCallContinuation()
            {
                var action = Interlocked.Exchange(ref whenComplete, null);
                if (action != null)
                {
                    action.Invoke();
                }
            }

            async UniTaskVoid RunTask(UniTask task, int index)
            {
                try
                {
                    await task;
                    var count = Interlocked.Increment(ref completeCount);
                    if (count == resultLength)
                    {
                        TryCallContinuation();
                    }
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                    TryCallContinuation();
                }
            }

            public Awaiter GetAwaiter()
            {
                return new Awaiter(this);
            }

            public struct Awaiter : ICriticalNotifyCompletion
            {
                WhenAllPromise parent;

                public Awaiter(WhenAllPromise parent)
                {
                    this.parent = parent;
                }

                public bool IsCompleted
                {
                    get
                    {
                        return parent.exception != null || parent.resultLength == parent.completeCount;
                    }
                }

                public void GetResult()
                {
                    if (parent.exception != null)
                    {
                        parent.exception.Throw();
                    }
                }

                public void OnCompleted(Action continuation)
                {
                    UnsafeOnCompleted(continuation);
                }

                public void UnsafeOnCompleted(Action continuation)
                {
                    parent.whenComplete = continuation;
                    if (IsCompleted)
                    {
                        var action = Interlocked.Exchange(ref parent.whenComplete, null);
                        if (action != null)
                        {
                            action();
                        }
                    }
                }
            }
        }
    }
}
#endif