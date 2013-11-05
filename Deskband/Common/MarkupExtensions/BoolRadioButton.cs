using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Deskband.Common.MarkupExtensions
{
    public class BoolRadioButton : RadioButton
    {
        // http://stackoverflow.com/questions/1317891/simple-wpf-radiobutton-binding/15923466#15923466

        public bool RadioValue
        {
            get { return (bool)GetValue(RadioValueProperty); }
            set { SetValue(RadioValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RadioValue.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RadioValueProperty =
            DependencyProperty.Register(
                "RadioValue",
                typeof(object),
                typeof(BoolRadioButton),
                new UIPropertyMetadata(null));

        public object RadioBinding
        {
            get { return (object)GetValue(RadioBindingProperty); }
            set { SetValue(RadioBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RadioBinding.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RadioBindingProperty =
            DependencyProperty.Register(
                "RadioBinding",
                typeof(object),
                typeof(BoolRadioButton),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRadioBindingChanged));

        private static void OnRadioBindingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            BoolRadioButton rb = (BoolRadioButton)d;
            if (rb.RadioValue.Equals(e.NewValue))
                rb.SetCurrentValue(RadioButton.IsCheckedProperty, true);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            SetCurrentValue(RadioBindingProperty, RadioValue);
        }
    }
}