using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Deskband.Common
{
    public class ObservableObject<T> : INotifyPropertyChanged where T : class
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string GetPropertyName<TValue>(Expression<Func<T, TValue>> propertySelector)
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression != null)
                return memberExpression.Member.Name;
            else
                return "";
        }

        protected void RaisePropertyChangedEvent<TValue>(Expression<Func<T, TValue>> propertySelector)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var propertyName = GetPropertyName(propertySelector);
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}