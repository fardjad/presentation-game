using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace PresentationGame.ReactiveExtensions
{
    public static class ButtonReactiveExtensions
    {
        public static IObservable<EventPattern<object>> GetClickObservable(this Button button)
        {
            return Observable.FromEventPattern(h => button.Click += h, h => button.Click -= h);
        }
    }
}