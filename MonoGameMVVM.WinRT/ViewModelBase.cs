using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGameMVVM
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
       
        public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);

            RaisePropertyChanged(propertyName);
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            EventHandler<PropertyChangedEventArgs> handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = propertyExpression.Body as MemberExpression;

            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }

            var property = body.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }

            return property.Name;
        }

    }
}
