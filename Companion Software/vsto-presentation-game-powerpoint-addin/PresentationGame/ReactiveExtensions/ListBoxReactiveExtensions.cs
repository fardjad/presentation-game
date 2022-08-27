using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace PresentationGame.ReactiveExtensions
{
    public static class ListBoxReactiveExtensions
    {
        public static IObservable<bool> GetIsItemSelectedObservable(this ListBox listBox)
        {
            return listBox.GetSelectedIndexObservable().Select(index => index != -1);
        }

        public static IObservable<int> GetSelectedIndexObservable(this ListBox listBox)
        {
            return Observable.FromEventPattern(h => listBox.SelectedIndexChanged += h,
                h => listBox.SelectedIndexChanged -= h).Select(_ => listBox.SelectedIndex);
        }

        public static IObservable<object> GetSelectedItemObservable(this ListBox listBox)
        {
            return Observable.FromEventPattern(h => listBox.SelectedIndexChanged += h,
                h => listBox.SelectedIndexChanged -= h).Select(_ =>
                listBox.SelectedIndex == -1 ? null : listBox.Items[listBox.SelectedIndex]);
        }

        public static IObservable<EventPattern<object>> GetDoubleClickObservable(this ListBox listBox)
        {
            return Observable.FromEventPattern(h => listBox.DoubleClick += h, h => listBox.DoubleClick -= h);
        }
    }
}