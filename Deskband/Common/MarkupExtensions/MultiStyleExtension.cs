using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Deskband.Common.MarkupExtensions
{
    [MarkupExtensionReturnType(typeof(Style))]
    public class MultiStyleExtension : MarkupExtension
    {
        private string[] _resourceKeys;

        public MultiStyleExtension(string inputResourceKeys)
        {
            if (inputResourceKeys == null)
                throw new ArgumentNullException("inputResourceKeys");

            _resourceKeys = inputResourceKeys.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (_resourceKeys.Length == 0)
                throw new ArgumentException("No input resource keys specified.");
        }

        private void Merge(Style style1, Style style2)
        {
            if (style1 == null)
            {
                throw new ArgumentNullException("style1");
            }
            if (style2 == null)
            {
                throw new ArgumentNullException("style2");
            }

            if (style1.TargetType.IsAssignableFrom(style2.TargetType))
            {
                style1.TargetType = style2.TargetType;
            }

            if (style2.BasedOn != null)
            {
                Merge(style1, style2.BasedOn);
            }

            foreach (SetterBase currentSetter in style2.Setters)
            {
                style1.Setters.Add(currentSetter);
            }

            foreach (TriggerBase currentTrigger in style2.Triggers)
            {
                style1.Triggers.Add(currentTrigger);
            }

            // This code is only needed when using DynamicResources.
            foreach (object key in style2.Resources.Keys)
            {
                style1.Resources[key] = style2.Resources[key];
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Style resultStyle = new Style();

            foreach (string currentResourceKey in _resourceKeys)
            {
                Style currentStyle = new StaticResourceExtension(currentResourceKey).ProvideValue(serviceProvider) as Style;

                if (currentStyle == null)
                {
                    throw new InvalidOperationException("Could not find style with resource key " + currentResourceKey + ".");
                }

                Merge(resultStyle, currentStyle);
            }
            return resultStyle;
        }
    }
}