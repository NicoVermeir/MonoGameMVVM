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
using System.Reactive.Linq;

namespace MonoGameMVVM.UI.Data
{
    internal class OneWayReactivePropertyBinding<T, TSource> : OneWayBinding<T>
        where TSource : class, IReactiveObject
    {
        private readonly ReactiveProperty<T> deferredProperty;

        private ReactiveObject deferredSource;

        public OneWayReactivePropertyBinding(IReactiveObject source, ReactiveProperty<T> reactiveProperty)
            : base(BindingResolutionMode.Immediate)
        {
            SourceObservable = source.GetObservable<T, TSource>(reactiveProperty);
        }

        public OneWayReactivePropertyBinding(ReactiveProperty<T> reactiveProperty)
            : base(BindingResolutionMode.Deferred)
        {
            deferredProperty = reactiveProperty;
            SourceObservable = Observable.Defer(GetDeferredObservable);
        }

        public override void Resolve(object dataContext)
        {
            var reactiveObject = dataContext as ReactiveObject;
            if (reactiveObject != null)
            {
                deferredSource = reactiveObject;
                Subscription = SourceObservable.Subscribe(Observer);
            }
        }

        private IObservable<T> GetDeferredObservable()
        {
            return deferredSource.GetObservable<T, TSource>(deferredProperty);
        }
    }
}