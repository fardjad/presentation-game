using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Office.Tools.Ribbon;

namespace PresentationGame.ReactiveExtensions
{
    public static class RibbonButtonReactiveExtensions
    {
        public static IObservable<EventPattern<RibbonControlEventArgs>> GetClickObservable(this RibbonButton button)
        {
            return Observable.FromEventPattern<RibbonControlEventHandler, RibbonControlEventArgs>(
                h => button.Click += h, h => button.Click -= h);
        }
    }
}