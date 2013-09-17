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
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Linq;

namespace MonoGameMVVM
{
    /// <summary>
    ///     <see cref="ItemsControl">ItemsControl</see> allows you to represent a collection of items and provides scaffolding
    ///     to generate the UI for each item.
    /// </summary>
    public class ItemsControl : Control
    {
        /// <summary>
        ///     <see cref="ItemTemplate">ItemTemplate</see> Reactive Property.
        /// </summary>
        public static readonly ReactiveProperty<Func<object, IElement>> ItemTemplateProperty =
            ReactiveProperty<Func<object, IElement>>.Register("ItemTemplate", typeof (ItemsControl));

        /// <summary>
        ///     <see cref="ItemsPanel">ItemsPanel</see> Reactive Property.
        /// </summary>
        public static readonly ReactiveProperty<Panel> ItemsPanelProperty =
            ReactiveProperty<Panel>.Register("ItemsPanel", typeof (ItemsControl), ItemsPanelChanged);

        /// <summary>
        ///     <see cref="ItemsSource">ItemsSource</see> Reactive Property.
        /// </summary>
        public static readonly ReactiveProperty<IEnumerable> ItemsSourceProperty =
            ReactiveProperty<IEnumerable>.Register("ItemsSource", typeof (ItemsControl), ItemsSourceChanged);

        private IDisposable changingItems;

        private bool isItemsSourceNew;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemsControl">ItemsControl</see> class.
        /// </summary>
        public ItemsControl()
        {
            ItemsPanel = new StackPanel();
        }

        /// <summary>
        ///     Gets or sets a function that is used to generate the <see cref="IElement">IElement</see> for each item in the
        ///     <see cref="ItemsSource">ItemsSource</see>.
        ///     The function takes one argument of type <see cref="object">object</see> that represents the item's
        ///     <see cref="IElement.DataContext">DataContext</see>.
        /// </summary>
        public Func<object, IElement> ItemTemplate
        {
            get { return GetValue(ItemTemplateProperty); }

            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        ///     Gets of sets the <see cref="Panel">Panel</see> used to control the layout of items.
        /// </summary>
        public Panel ItemsPanel
        {
            get { return GetValue(ItemsPanelProperty); }

            set { SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the collection of items to be displayed.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }

            set { SetValue(ItemsSourceProperty, value); }
        }

        public override IEnumerable<IElement> GetVisualChildren()
        {
            Panel child = ItemsPanel;
            if (child != null)
            {
                yield return child;
            }
        }

        public override void OnApplyTemplate()
        {
            if (isItemsSourceNew)
            {
                PopulatePanelFromItemsSource();
                isItemsSourceNew = false;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Panel child = ItemsPanel;
            if (child != null)
            {
                child.Arrange(new Rect(new Point(), finalSize));
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Panel child = ItemsPanel;
            if (child == null)
            {
                return Size.Empty;
            }

            child.Measure(availableSize);
            return child.DesiredSize;
        }

        private static void ItemsPanelChanged(IReactiveObject source, ReactivePropertyChangeEventArgs<Panel> change)
        {
            var itemsControl = (ItemsControl) source;
            Panel newPanel = change.NewValue;
            Panel oldPanel = change.OldValue;

            if (oldPanel != null)
            {
                oldPanel.VisualParent = null;
            }

            if (newPanel != null)
            {
                if (!(newPanel.Children is ITemplatedList<IElement>))
                {
                    throw new NotSupportedException(
                        "ItemsControl requires a panel whose Children collection implements ITemplatedList<IElement>");
                }

                newPanel.VisualParent = itemsControl;
            }

            itemsControl.InvalidateMeasure();
        }

        private static void ItemsSourceChanged(
            IReactiveObject source, ReactivePropertyChangeEventArgs<IEnumerable> change)
        {
            var itemsControl = (ItemsControl) source;
            if (change.OldValue is INotifyCollectionChanged)
            {
                itemsControl.changingItems.Dispose();
            }

            var observableCollection = change.NewValue as INotifyCollectionChanged;
            if (observableCollection != null)
            {
                itemsControl.changingItems =
                    Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        handler => new NotifyCollectionChangedEventHandler(handler),
                        handler => observableCollection.CollectionChanged += handler,
                        handler => observableCollection.CollectionChanged -= handler).Subscribe(
                            itemsControl.OnNextItemChange);
            }

            itemsControl.isItemsSourceNew = true;
            itemsControl.InvalidateMeasure();
        }

        private void OnNextItemChange(EventPattern<NotifyCollectionChangedEventArgs> eventData)
        {
            var children = (ITemplatedList<IElement>) ItemsPanel.Children;
            switch (eventData.EventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object newItem in eventData.EventArgs.NewItems)
                    {
                        children.Add(newItem, ItemTemplate);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                {
                    int startingIndex = eventData.EventArgs.OldStartingIndex;
                    for (int index = startingIndex;
                        index < startingIndex + eventData.EventArgs.OldItems.Count;
                        index++)
                    {
                        children.RemoveAt(index);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Replace:
                {
                    int startingIndex = eventData.EventArgs.NewStartingIndex;

                    foreach (object newItem in eventData.EventArgs.NewItems)
                    {
                        ItemsPanel.Children.RemoveAt(startingIndex);
                        children.Insert(startingIndex, newItem, ItemTemplate);
                        startingIndex++;
                    }

                    break;
                }

#if !WINDOWS_PHONE
                case NotifyCollectionChangedAction.Move:
                    children.Move(eventData.EventArgs.OldStartingIndex, eventData.EventArgs.NewStartingIndex);

                    break;
#endif
                case NotifyCollectionChangedAction.Reset:
                    PopulatePanelFromItemsSource();
                    break;
            }
        }

        private void PopulatePanelFromItemsSource()
        {
            var children = (ITemplatedList<IElement>) ItemsPanel.Children;
            children.Clear();

            if (ItemsSource != null)
            {
                foreach (object item in ItemsSource)
                {
                    children.Add(item, ItemTemplate);
                }
            }
        }
    }
}