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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameMVVM.UI
{
    public class VirtualizingElementCollection : IList<IElement>, ITemplatedList<IElement>
    {
        private readonly Cursor cursor;

        private readonly IList<Memento> items = new List<Memento>();

        private readonly IElement owner;

        public VirtualizingElementCollection(IElement owner)
        {
            this.owner = owner;
            cursor = new Cursor(items);
        }

        public IEnumerable<IElement> RealizedElements
        {
            get { return cursor.CurrentlyRealized; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IElement this[int index]
        {
            get
            {
                Memento memento = items[index];
                return memento.IsReal ? memento.Element : memento.Create();
            }

            set { throw new NotSupportedException("Please use the overload that takes a template"); }
        }

        public void Add(IElement element)
        {
            throw new NotSupportedException("Please use the overload that takes a template");
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(IElement item)
        {
            return false;
        }

        public void CopyTo(IElement[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IElement item)
        {
            throw new NotSupportedException("Please use RemoveAt(int index)");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IElement> GetEnumerator()
        {
            return items.Select(memento => memento.IsReal ? memento.Element : memento.Create()).GetEnumerator();
        }

        public int IndexOf(IElement item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IElement item)
        {
            throw new NotSupportedException(
                "Please use Insert(int index, object item, Func<object, IElement> template)");
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public virtual void Add(object item, Func<object, IElement> template)
        {
            items.Add(new Memento(item, template, owner));
        }

        public virtual void Insert(int index, object item, Func<object, IElement> template)
        {
            items.Insert(index, new Memento(item, template, owner));
        }

        public virtual void Move(int oldIndex, int newIndex)
        {
            Memento memento = items[oldIndex];
            items.RemoveAt(oldIndex);
            items.Insert(newIndex, memento);
        }

        public Cursor GetCursor(int startIndex)
        {
            return cursor.UnDispose(startIndex);
        }

        public bool IsReal(int index)
        {
            return items[index].IsReal;
        }

        public class Cursor : IDisposable, IEnumerable<IElement>
        {
            private readonly IList<Memento> mementoes;

            private LinkedList<Memento> currentRealizedMementoes = new LinkedList<Memento>();

            private int firstMemento;

            private bool isDisposed;

            private LinkedList<Memento> previousRealizedMementoes = new LinkedList<Memento>();

            public Cursor(IList<Memento> mementoes)
            {
                this.mementoes = mementoes;
            }

            public IEnumerable<IElement> CurrentlyRealized
            {
                get { return previousRealizedMementoes.Select(memento => memento.Element); }
            }

            public void Dispose()
            {
                if (!isDisposed)
                {
                    Dispose(true);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<IElement> GetEnumerator()
            {
                for (int i = firstMemento; i < mementoes.Count; i++)
                {
                    Memento memento = mementoes[i];
                    IElement element = memento.IsReal ? memento.Element : memento.Realize();

                    currentRealizedMementoes.AddLast(memento);
                    yield return element;
                }
            }

            public void Dispose(bool isDisposing)
            {
                isDisposed = true;
                foreach (Memento memento in previousRealizedMementoes.Except(currentRealizedMementoes))
                {
                    memento.Virtualize();
                }

                previousRealizedMementoes.Clear();
                LinkedList<Memento> newCurrent = previousRealizedMementoes;
                previousRealizedMementoes = currentRealizedMementoes;
                currentRealizedMementoes = newCurrent;
            }

            public Cursor UnDispose(int startIndex)
            {
                isDisposed = false;
                firstMemento = startIndex;
                return this;
            }
        }

        public class Memento
        {
            private readonly object item;

            private readonly IElement owner;

            private readonly Func<object, IElement> template;

            private IElement element;

            public Memento(object item, Func<object, IElement> template, IElement owner)
            {
                if (template == null)
                {
                    throw new InvalidOperationException("A Template for this Item has not been supplied");
                }

                this.item = item;
                this.template = template;
                this.owner = owner;

                this.owner.InvalidateMeasure();
            }

            public IElement Element
            {
                get { return element; }
            }

            public bool IsReal
            {
                get { return element != null; }
            }

            public IElement Create()
            {
                IElement newElement = template(item);
                newElement.DataContext = item;
                return newElement;
            }

            public IElement Realize()
            {
                element = Create();
                element.VisualParent = owner;
                owner.InvalidateMeasure();
                return element;
            }

            public void Virtualize()
            {
                element.DataContext = null;
                element.VisualParent = null;
                element = null;
            }
        }
    }
}