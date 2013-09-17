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

namespace MonoGameMVVM.UI
{
    public class ElementCollection : IList<IElement>, ITemplatedList<IElement>
    {
        private readonly List<IElement> elements = new List<IElement>();

        private readonly IElement owner;

        public ElementCollection(IElement owner)
        {
            this.owner = owner;
        }

        public int Count
        {
            get { return elements.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IElement this[int index]
        {
            get { return elements[index]; }

            set
            {
                IElement oldItem = elements[index];
                IElement newItem = value;

                elements[index] = newItem;
                owner.InvalidateMeasure();
                SetParents(oldItem, newItem);
            }
        }

        public void Add(IElement item)
        {
            elements.Add(item);
            owner.InvalidateMeasure();
            SetParents(null, item);
        }

        public void Clear()
        {
            elements.Clear();
            owner.InvalidateMeasure();
        }

        public bool Contains(IElement item)
        {
            return elements.Contains(item);
        }

        public void CopyTo(IElement[] array, int arrayIndex)
        {
            elements.CopyTo(array, arrayIndex);
        }

        public bool Remove(IElement item)
        {
            bool wasRemoved = elements.Remove(item);
            if (wasRemoved)
            {
                owner.InvalidateMeasure();
                SetParents(item, null);
            }

            return wasRemoved;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        public int IndexOf(IElement item)
        {
            return elements.IndexOf(item);
        }

        public void Insert(int index, IElement item)
        {
            elements.Insert(index, item);
            owner.InvalidateMeasure();
            SetParents(null, item);
        }

        public void RemoveAt(int index)
        {
            IElement oldItem = elements[index];
            elements.RemoveAt(index);
            owner.InvalidateMeasure();
            SetParents(oldItem, null);
        }

        public void Add(object item, Func<object, IElement> template)
        {
            Add(Realize(item, template));
        }

        public void Insert(int index, object item, Func<object, IElement> template)
        {
            Insert(index, Realize(item, template));
        }

        public void Move(int oldIndex, int newIndex)
        {
            IElement element = this[oldIndex];
            RemoveAt(oldIndex);
            Insert(newIndex, element);
        }

        private static IElement Realize(object item, Func<object, IElement> template)
        {
            if (template == null)
            {
                throw new InvalidOperationException("An element cannot be created without a template");
            }

            IElement element = template(item);
            element.DataContext = item;
            return element;
        }

        private void SetParents(IElement oldItem, IElement newItem)
        {
            if (oldItem != null)
            {
                oldItem.VisualParent = null;
            }

            if (newItem != null)
            {
                newItem.VisualParent = owner;
            }
        }
    }
}