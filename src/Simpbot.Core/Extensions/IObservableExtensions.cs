using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Simpbot.Core.Extensions
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public static class IObservableExtensions
    {
        public static IDisposable SubscribeAsync<T>(this IObservable<T> source,
            Func<Task> asyncAction, Action<Exception> handler = null)
        {
            async Task<Unit> Wrapped(T t)
            {
                await asyncAction();
                return Unit.Default;
            }

            return handler == null ? source.SelectMany(Wrapped).Subscribe(_ => { }) : source.SelectMany(Wrapped).Subscribe(_ => { }, handler);
        }

        public static IDisposable SubscribeAsync<T>(this IObservable<T> source,
            Func<T, Task> asyncAction, Action<Exception> handler = null)
        {
            async Task<Unit> Wrapped(T t)
            {
                await asyncAction(t);
                return Unit.Default;
            }

            return handler == null ? source.SelectMany(Wrapped).Subscribe(_ => { }) : source.SelectMany(Wrapped).Subscribe(_ => { }, handler);
        }

        public static IObservable<T> SubscribeAsyncChain<T>(this IObservable<T> source,
            Func<Task> asyncAction, Action<Exception> handler = null)
        {
            async Task<Unit> Wrapped(T t)
            {
                await asyncAction();
                return Unit.Default;
            }

            var disp = handler == null ? source.SelectMany(Wrapped).Subscribe(_ => { }) : source.SelectMany(Wrapped).Subscribe(_ => { }, handler);
            return source;
        }

        public static IObservable<T> SubscribeAsyncChain<T>(this IObservable<T> source,
            Func<T, Task> asyncAction, Action<Exception> handler = null)
        {
            async Task<Unit> Wrapped(T t)
            {
                await asyncAction(t);
                return Unit.Default;
            }

            var disp = handler == null ? source.SelectMany(Wrapped).Subscribe(_ => { }) : source.SelectMany(Wrapped).Subscribe(_ => { }, handler);
            return source;
        }
    }
}