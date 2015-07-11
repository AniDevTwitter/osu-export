using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace osu_export.wpf
{
    public class AbstractViewModel<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string NameOf<TProp>(Expression<Func<T, TProp>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}