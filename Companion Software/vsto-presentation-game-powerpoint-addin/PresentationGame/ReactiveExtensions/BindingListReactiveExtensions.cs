using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

namespace PresentationGame.ReactiveExtensions
{
    public static class BindingListReactiveExtensions
    {
        public static IObservable<ICollection<T>> GetListObservable<T>(this BindingList<T> list)
        {
            return Observable.FromEventPattern<ListChangedEventHandler, ListChangedEventArgs>(h =>
                    list.ListChanged += h, h => list.ListChanged -= h)
                .Select(pattern => pattern.Sender as ICollection<T>);
        }
    }
}