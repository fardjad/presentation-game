using System;
using System.Reactive.Linq;
using Microsoft.Office.Tools.Ribbon;

namespace PresentationGame.ReactiveExtensions
{
    public static class RibbonCheckBoxReactiveExtensions
    {
        public static IObservable<bool> GetCheckedObservable(this RibbonCheckBox checkBox)
        {
            return Observable
                .FromEventPattern<RibbonControlEventHandler, RibbonControlEventArgs>(h => checkBox.Click += h,
                    h => checkBox.Click -= h).Select(pattern => pattern.Sender as RibbonCheckBox)
                .Select(c => c.Checked);
        }
    }
}