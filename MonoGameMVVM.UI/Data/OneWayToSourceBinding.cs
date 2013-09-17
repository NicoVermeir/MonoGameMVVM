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
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

namespace MonoGameMVVM.UI.Data
{
    internal class OneWayToSourceBinding<T> : IOneWayToSourceBinding<T>, IBinding, IDisposable
    {
        private readonly PropertyInfo propertyInfo;

        private readonly BindingResolutionMode resolutionMode;

        private bool isDisposed;

        public OneWayToSourceBinding(PropertyInfo propertyInfo)
            : this(BindingResolutionMode.Deferred)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            this.propertyInfo = propertyInfo;
        }

        public OneWayToSourceBinding(object source, PropertyInfo propertyInfo)
            : this(BindingResolutionMode.Immediate)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            TargetObserver = Observer.Create<T>(value => SetValue(source, propertyInfo, value));
        }

        protected OneWayToSourceBinding(BindingResolutionMode resolutionMode)
        {
            this.resolutionMode = resolutionMode;
        }

        protected OneWayToSourceBinding(IObserver<T> targetObserver)
            : this(BindingResolutionMode.Immediate)
        {
            TargetObserver = targetObserver;
        }

        protected T InitialValue { get; private set; }

        protected IDisposable Subscription { get; set; }

        protected IObserver<T> TargetObserver { get; set; }

        public BindingResolutionMode ResolutionMode
        {
            get { return resolutionMode; }
        }

        public virtual void Resolve(object dataContext)
        {
            TargetObserver = Observer.Create<T>(value => SetValue(dataContext, propertyInfo, value));
            OnNext(InitialValue);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void OnCompleted()
        {
            if (TargetObserver != null)
            {
                TargetObserver.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            if (TargetObserver != null)
            {
                TargetObserver.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            if (TargetObserver != null)
            {
                TargetObserver.OnNext(value);
            }
        }

        public virtual IDisposable Initialize(IObservable<T> observable)
        {
            Subscription = observable.Subscribe(this);
            InitialValue = observable.FirstOrDefault();
            return this;
        }

        ~OneWayToSourceBinding()
        {
            Dispose(false);
        }

        public void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (Subscription != null)
                    {
                        Subscription.Dispose();
                    }
                }
            }

            isDisposed = true;
        }

        private static void SetValue(object source, PropertyInfo propertyInfo, object value)
        {
            if (value != null)
            {
                Type sourceType = propertyInfo.PropertyType;
                Type targetType = typeof (T);
                if (sourceType != targetType && !targetType.GetTypeInfo().IsAssignableFrom(sourceType.GetTypeInfo()))
                {
                    value = Convert.ChangeType(value, sourceType, CultureInfo.InvariantCulture);
                }
            }

            propertyInfo.SetValue(source, value, null);
        }
    }
}