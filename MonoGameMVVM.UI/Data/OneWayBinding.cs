#region License

/* The MIT License
 *
 * Copyright (c) 2011 Red Badger Consulting
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

#endregion

using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace MonoGameMVVM.UI.Data
{
    internal class OneWayBinding<T> : IObservable<T>, IBinding, IDisposable
    {
        private readonly PropertyInfo deferredProperty;

        private readonly T initialValue;

        private readonly BindingResolutionMode resolutionMode;

        private readonly bool shouldPushInitialValueOnSubscribe;

        private INotifyPropertyChanged deferredSource;

        private bool isDisposed;

        private IObserver<T> observer;

        private IObservable<T> sourceObservable;

        private IDisposable subscription;

        /// <summary>
        ///     A one-way binding to the data context
        /// </summary>
        public OneWayBinding()
            : this(BindingResolutionMode.Deferred)
        {
        }

        /// <summary>
        ///     A one-way binding to a property on the data context
        /// </summary>
        /// <param name="propertyInfo"></param>
        public OneWayBinding(PropertyInfo propertyInfo)
            : this(BindingResolutionMode.Deferred)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            deferredProperty = propertyInfo;
            sourceObservable = Observable.Defer(GetDeferredObservable);
        }

        /// <summary>
        ///     A one-way binding to a specified source
        /// </summary>
        /// <param name="source"></param>
        public OneWayBinding(T source)
            : this(BindingResolutionMode.Immediate)
        {
            sourceObservable = new BehaviorSubject<T>(source);
        }

        /// <summary>
        ///     A one-way binding to a property on a specified source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyInfo"></param>
        public OneWayBinding(object source, PropertyInfo propertyInfo)
            : this(BindingResolutionMode.Immediate)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            initialValue = GetValue(source, propertyInfo);

            var notifyPropertyChanged = source as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                sourceObservable = GetObservable(notifyPropertyChanged, propertyInfo);
                shouldPushInitialValueOnSubscribe = true;
            }
            else
            {
                sourceObservable = new BehaviorSubject<T>(initialValue);
            }
        }

        protected OneWayBinding(BindingResolutionMode resolutionMode)
        {
            this.resolutionMode = resolutionMode;
        }

        protected IObserver<T> Observer
        {
            get { return observer; }
        }

        protected IObservable<T> SourceObservable
        {
            get { return sourceObservable; }

            set { sourceObservable = value; }
        }

        protected IDisposable Subscription
        {
            set { subscription = value; }
        }

        public BindingResolutionMode ResolutionMode
        {
            get { return resolutionMode; }
        }

        public virtual void Resolve(object dataContext)
        {
            if (sourceObservable == null)
            {
                observer.OnNext(Convert(dataContext));
            }
            else
            {
                observer.OnNext(GetValue(dataContext, deferredProperty));

                if (dataContext is INotifyPropertyChanged)
                {
                    deferredSource = (INotifyPropertyChanged) dataContext;
                    subscription = sourceObservable.Subscribe(observer);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            this.observer = observer;

            if (resolutionMode == BindingResolutionMode.Immediate)
            {
                if (shouldPushInitialValueOnSubscribe)
                {
                    this.observer.OnNext(initialValue);
                }

                subscription = sourceObservable.Subscribe(this.observer);
            }

            return this;
        }

        ~OneWayBinding()
        {
            Dispose(false);
        }

        public void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (subscription != null)
                    {
                        subscription.Dispose();
                    }
                }
            }

            isDisposed = true;
        }

        internal static T Convert(object value)
        {
            if (value != null)
            {
                Type sourceType = value.GetType();
                Type targetType = typeof (T);
                if (sourceType != targetType && !targetType.GetTypeInfo().IsAssignableFrom(sourceType.GetTypeInfo()))
                {
                    value = System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                }
            }

            return (T) value;
        }

        private static IObservable<T> GetObservable(INotifyPropertyChanged source, PropertyInfo propertyInfo)
        {
            return
                Observable.FromEventPattern<PropertyChangedEventArgs>(
                    handler => source.PropertyChanged += handler, handler => source.PropertyChanged -= handler).Where(
                        data => data.EventArgs.PropertyName == propertyInfo.Name).Select(
                            e => GetValue(source, propertyInfo));
        }

        private static T GetValue(object source, PropertyInfo propertyInfo)
        {
            return Convert(propertyInfo.GetValue(source, null));
        }

        private IObservable<T> GetDeferredObservable()
        {
            return GetObservable(deferredSource, deferredProperty);
        }
    }
}