using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace PresentationGame.ReactiveExtensions
{
    public static class FormReactiveExtensions
    {
        public static IObservable<EventPattern<object>> GetClosedObservable(this Form form)
        {
            return Observable.FromEventPattern(h => form.Closed += h, h => form.Closed -= h).Take(1);
        }
    }
}