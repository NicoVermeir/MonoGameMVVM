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
using System.Collections.Generic;
using System.Linq;


namespace MonoGameMVVM
{
    /// <summary>
    ///     A Grid layout panel consisting of columns and rows.
    /// </summary>
    public class Grid : Panel
    {
        /// <summary>
        ///     Column attached property.
        /// </summary>
        public static readonly ReactiveProperty<int> ColumnProperty = ReactiveProperty<int>.Register(
            "Column", typeof (Grid));

        /// <summary>
        ///     Row attached property.
        /// </summary>
        public static readonly ReactiveProperty<int> RowProperty = ReactiveProperty<int>.Register("Row", typeof (Grid));

        private readonly LinkedList<Cell> allStars = new LinkedList<Cell>();

        private readonly LinkedList<Cell> autoPixelHeightStarWidth = new LinkedList<Cell>();

        private readonly IList<ColumnDefinition> columnDefinitions = new List<ColumnDefinition>();

        private readonly bool[] hasAuto = new bool[2];

        private readonly bool[] hasStar = new bool[2];

        private readonly LinkedList<Cell> noStars = new LinkedList<Cell>();

        private readonly IList<RowDefinition> rowDefinitions = new List<RowDefinition>();

        private readonly LinkedList<Cell> starHeightAutoPixelWidth = new LinkedList<Cell>();

        private Cell[] cells;

        private DefinitionBase[] columns;

        private DefinitionBase[] rows;

        /// <summary>
        ///     Gets the collection of column definitions.
        /// </summary>
        /// <value>The column definitions collection.</value>
        public IList<ColumnDefinition> ColumnDefinitions
        {
            get { return columnDefinitions; }
        }

        /// <summary>
        ///     Gets the collection of row definitions.
        /// </summary>
        /// <value>The row definitions collection.</value>
        public IList<RowDefinition> RowDefinitions
        {
            get { return rowDefinitions; }
        }

        /// <summary>
        ///     Gets the value of the Column attached property for the specified element.
        /// </summary>
        /// <param name="element">The element for which to read the proerty value.</param>
        /// <returns>The value of the Column attached property.</returns>
        public static int GetColumn(IElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.GetValue(ColumnProperty);
        }

        /// <summary>
        ///     Gets the value of the Row attached property for the specified element.
        /// </summary>
        /// <param name="element">The element for which to read the proerty value.</param>
        /// <returns>The value of the Row attached property.</returns>
        public static int GetRow(IElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.GetValue(RowProperty);
        }

        /// <summary>
        ///     Sets the value of the Column attached property for the specified element.
        /// </summary>
        /// <param name="element">The element for which to write the proerty value.</param>
        /// <param name="value">The value of the Column attached property.</param>
        public static void SetColumn(IElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ColumnProperty, value);
        }

        /// <summary>
        ///     Sets the value of the Row attached property for the specified element.
        /// </summary>
        /// <param name="element">The element for which to write the proerty value.</param>
        /// <param name="value">The value of the Row attached property.</param>
        public static void SetRow(IElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(RowProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            SetFinalLength(columns, finalSize.Width);
            SetFinalLength(rows, finalSize.Height);

            for (int i = 0; i < cells.Length; i++)
            {
                IElement child = Children[i];
                if (child != null)
                {
                    int columnIndex = cells[i].ColumnIndex;
                    int rowIndex = cells[i].RowIndex;

                    var finalRect = new Rect(
                        columns[columnIndex].FinalOffset,
                        rows[rowIndex].FinalOffset,
                        columns[columnIndex].FinalLength,
                        rows[rowIndex].FinalLength);

                    child.Arrange(finalRect);
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            columns = columnDefinitions.Count == 0
                ? new DefinitionBase[] {new ColumnDefinition()}
                : columnDefinitions.ToArray();

            rows = rowDefinitions.Count == 0
                ? new DefinitionBase[] {new RowDefinition()}
                : rowDefinitions.ToArray();

            bool treatStarAsAutoWidth = double.IsPositiveInfinity(availableSize.Width);
            bool treatStarAsAutoHeight = double.IsPositiveInfinity(availableSize.Height);

            InitializeMeasureData(columns, treatStarAsAutoWidth, Dimension.Width);
            InitializeMeasureData(rows, treatStarAsAutoHeight, Dimension.Height);

            CreateCells();
            MeasureCells(availableSize);

            return new Size(
                columns.Sum(definition => definition.MinLength), rows.Sum(definition => definition.MinLength));
        }

        private static void AllocateProportionalSpace(IEnumerable<DefinitionBase> definitions, double availableLength)
        {
            double occupiedLength = 0d;

            var stars = new LinkedList<DefinitionBase>();

            foreach (DefinitionBase definition in definitions)
            {
                switch (definition.LengthType)
                {
                    case GridUnitType.Auto:
                        occupiedLength += definition.MinLength;
                        break;
                    case GridUnitType.Pixel:
                        occupiedLength += definition.AvailableLength;
                        break;
                    case GridUnitType.Star:
                        double numerator = definition.UserLength.Value;
                        if (numerator.IsCloseTo(0d))
                        {
                            definition.Numerator = 0d;
                            definition.StarAllocationOrder = 0d;
                        }
                        else
                        {
                            definition.Numerator = numerator;
                            definition.StarAllocationOrder = Math.Max(definition.MinLength, definition.UserMaxLength)/
                                                             numerator;
                        }

                        stars.AddLast(definition);
                        break;
                }
            }

            if (stars.Count > 0)
            {
                DefinitionBase[] sortedStars = stars.OrderBy(o => o.StarAllocationOrder).ToArray();

                double denominator = 0d;
                foreach (DefinitionBase definition in sortedStars.Reverse())
                {
                    denominator += definition.Numerator;
                    definition.Denominator = denominator;
                }

                foreach (DefinitionBase definition in sortedStars)
                {
                    double length;
                    if (definition.Numerator.IsCloseTo(0d))
                    {
                        length = definition.MinLength;
                    }
                    else
                    {
                        double remainingLength = (availableLength - occupiedLength).EnsurePositive();
                        length = remainingLength*(definition.Numerator/definition.Denominator);
                        length = length.Coerce(definition.MinLength, definition.UserMaxLength);
                    }

                    occupiedLength += length;
                    definition.AvailableLength = length;
                }
            }
        }

        private static void SetFinalLength(DefinitionBase[] definitions, double gridFinalLength)
        {
            double occupiedLength = 0d;

            var stars = new LinkedList<DefinitionBase>();
            var nonStarDefinitions = new LinkedList<DefinitionBase>();

            foreach (DefinitionBase definition in definitions)
            {
                double minLength;

                switch (definition.UserLength.GridUnitType)
                {
                    case GridUnitType.Auto:
                        minLength = definition.MinLength;

                        definition.FinalLength = minLength.Coerce(definition.MinLength, definition.UserMaxLength);

                        occupiedLength += definition.FinalLength;
                        nonStarDefinitions.AddFirst(definition);
                        break;
                    case GridUnitType.Pixel:
                        minLength = definition.UserLength.Value;

                        definition.FinalLength = minLength.Coerce(definition.MinLength, definition.UserMaxLength);

                        occupiedLength += definition.FinalLength;
                        nonStarDefinitions.AddFirst(definition);
                        break;
                    case GridUnitType.Star:
                        double numerator = definition.UserLength.Value;
                        if (numerator.IsCloseTo(0d))
                        {
                            definition.Numerator = 0d;
                            definition.StarAllocationOrder = 0d;
                        }
                        else
                        {
                            definition.Numerator = numerator;
                            definition.StarAllocationOrder = Math.Max(definition.MinLength, definition.UserMaxLength)/
                                                             numerator;
                        }

                        stars.AddLast(definition);
                        break;
                }
            }

            if (stars.Count > 0)
            {
                DefinitionBase[] sortedStars = stars.OrderBy(o => o.StarAllocationOrder).ToArray();

                double denominator = 0d;
                foreach (DefinitionBase definitionBase in sortedStars.Reverse())
                {
                    denominator += definitionBase.Numerator;
                    definitionBase.Denominator = denominator;
                }

                foreach (DefinitionBase definition in sortedStars)
                {
                    double length;
                    if (definition.Numerator.IsCloseTo(0d))
                    {
                        length = definition.MinLength;
                    }
                    else
                    {
                        double remainingLength = (gridFinalLength - occupiedLength).EnsurePositive();
                        length = remainingLength*(definition.Numerator/definition.Denominator);
                        length = length.Coerce(definition.MinLength, definition.UserMaxLength);
                    }

                    occupiedLength += length;
                    definition.FinalLength = length;
                }
            }

            if (occupiedLength.IsGreaterThan(gridFinalLength))
            {
                IOrderedEnumerable<DefinitionBase> sortedDefinitions =
                    stars.Concat(nonStarDefinitions).OrderBy(o => o.FinalLength - o.MinLength);

                double excessLength = occupiedLength - gridFinalLength;
                int i = 0;
                foreach (DefinitionBase definitionBase in sortedDefinitions)
                {
                    double finalLength = definitionBase.FinalLength - (excessLength/(definitions.Length - i));
                    finalLength = finalLength.Coerce(definitionBase.MinLength, definitionBase.FinalLength);
                    excessLength -= definitionBase.FinalLength - finalLength;

                    definitionBase.FinalLength = finalLength;
                    i++;
                }
            }

            definitions[0].FinalOffset = 0d;
            for (int i = 1; i < definitions.Length; i++)
            {
                DefinitionBase previousDefinition = definitions[i - 1];
                definitions[i].FinalOffset = previousDefinition.FinalOffset + previousDefinition.FinalLength;
            }
        }

        private void CreateCells()
        {
            cells = new Cell[Children.Count];
            noStars.Clear();
            autoPixelHeightStarWidth.Clear();
            starHeightAutoPixelWidth.Clear();
            allStars.Clear();

            int i = 0;
            foreach (IElement child in Children)
            {
                if (child != null)
                {
                    int columnIndex = Math.Min(GetColumn(child), columns.Length - 1);
                    int rowIndex = Math.Min(GetRow(child), rows.Length - 1);
                    var cell = new Cell
                    {
                        ColumnIndex = columnIndex,
                        RowIndex = rowIndex,
                        Child = child,
                        WidthType = columns[columnIndex].LengthType,
                        HeightType = rows[rowIndex].LengthType,
                    };

                    if (cell.HeightType == GridUnitType.Star)
                    {
                        if (cell.WidthType == GridUnitType.Star)
                        {
                            allStars.AddLast(cell);
                        }
                        else
                        {
                            starHeightAutoPixelWidth.AddLast(cell);
                        }
                    }
                    else
                    {
                        if (cell.WidthType == GridUnitType.Star)
                        {
                            autoPixelHeightStarWidth.AddLast(cell);
                        }
                        else
                        {
                            noStars.AddLast(cell);
                        }
                    }

                    cells[i] = cell;
                }

                i++;
            }
        }

        private void InitializeMeasureData(
            IEnumerable<DefinitionBase> definitions, bool treatStarAsAuto, Dimension dimension)
        {
            foreach (DefinitionBase definition in definitions)
            {
                definition.MinLength = 0d;
                double availableLength = 0d;
                double userMinLength = definition.UserMinLength;
                double userMaxLength = definition.UserMaxLength;

                switch (definition.UserLength.GridUnitType)
                {
                    case GridUnitType.Auto:
                        definition.LengthType = GridUnitType.Auto;
                        availableLength = double.PositiveInfinity;
                        hasAuto[(int) dimension] = true;
                        break;
                    case GridUnitType.Pixel:
                        definition.LengthType = GridUnitType.Pixel;
                        availableLength = definition.UserLength.Value;
                        userMinLength = availableLength.Coerce(userMinLength, userMaxLength);
                        break;
                    case GridUnitType.Star:
                        definition.LengthType = treatStarAsAuto ? GridUnitType.Auto : GridUnitType.Star;
                        availableLength = double.PositiveInfinity;
                        hasStar[(int) dimension] = true;
                        break;
                }

                definition.UpdateMinLength(userMinLength);
                definition.AvailableLength = availableLength.Coerce(userMinLength, userMaxLength);
            }
        }

        private void MeasureCell(Cell cell, IElement child, bool shouldChildBeMeasuredWithInfiniteHeight)
        {
            if (child != null)
            {
                double x = cell.WidthType == GridUnitType.Auto
                    ? double.PositiveInfinity
                    : columns[cell.ColumnIndex].AvailableLength;

                double y = cell.HeightType == GridUnitType.Auto || shouldChildBeMeasuredWithInfiniteHeight
                    ? double.PositiveInfinity
                    : rows[cell.RowIndex].AvailableLength;

                child.Measure(new Size(x, y));
            }
        }

        private void MeasureCells(Size availableSize)
        {
            if (noStars.Count > 0)
            {
                MeasureCells(noStars, UpdateMinLengths.WidthsAndHeights);
            }

            if (!hasAuto[(int) Dimension.Height])
            {
                if (hasStar[(int) Dimension.Height])
                {
                    AllocateProportionalSpace(rows, availableSize.Height);
                }

                MeasureCells(starHeightAutoPixelWidth, UpdateMinLengths.WidthsAndHeights);

                if (hasStar[(int) Dimension.Width])
                {
                    AllocateProportionalSpace(columns, availableSize.Width);
                }

                MeasureCells(autoPixelHeightStarWidth, UpdateMinLengths.WidthsAndHeights);
            }
            else if (!hasAuto[(int) Dimension.Width])
            {
                if (hasStar[(int) Dimension.Width])
                {
                    AllocateProportionalSpace(columns, availableSize.Width);
                }

                MeasureCells(autoPixelHeightStarWidth, UpdateMinLengths.WidthsAndHeights);

                if (hasStar[(int) Dimension.Height])
                {
                    AllocateProportionalSpace(rows, availableSize.Height);
                }

                MeasureCells(starHeightAutoPixelWidth, UpdateMinLengths.WidthsAndHeights);
            }
            else
            {
                MeasureCells(starHeightAutoPixelWidth, UpdateMinLengths.SkipHeights);

                if (hasStar[(int) Dimension.Width])
                {
                    AllocateProportionalSpace(columns, availableSize.Width);
                }

                MeasureCells(autoPixelHeightStarWidth, UpdateMinLengths.WidthsAndHeights);

                if (hasStar[(int) Dimension.Height])
                {
                    AllocateProportionalSpace(rows, availableSize.Height);
                }

                MeasureCells(starHeightAutoPixelWidth, UpdateMinLengths.SkipWidths);
            }

            if (allStars.Count > 0)
            {
                MeasureCells(allStars, UpdateMinLengths.WidthsAndHeights);
            }
        }

        private void MeasureCells(IEnumerable<Cell> cells, UpdateMinLengths updateMinLengths)
        {
            foreach (Cell cell in cells)
            {
                bool shouldChildBeMeasuredWithInfiniteHeight = updateMinLengths == UpdateMinLengths.SkipHeights;

                MeasureCell(cell, cell.Child, shouldChildBeMeasuredWithInfiniteHeight);

                if (updateMinLengths != UpdateMinLengths.SkipWidths)
                {
                    DefinitionBase widthDefinition = columns[cell.ColumnIndex];
                    widthDefinition.UpdateMinLength(
                        Math.Min(cell.Child.DesiredSize.Width, widthDefinition.UserMaxLength));
                }

                if (updateMinLengths != UpdateMinLengths.SkipHeights)
                {
                    DefinitionBase heightDefinition = rows[cell.RowIndex];
                    heightDefinition.UpdateMinLength(
                        Math.Min(cell.Child.DesiredSize.Height, heightDefinition.UserMaxLength));
                }
            }
        }

        private struct Cell
        {
            public IElement Child;

            public int ColumnIndex;

            public GridUnitType HeightType;

            public int RowIndex;

            public GridUnitType WidthType;
        }

        private enum Dimension
        {
            Width,
            Height
        }

        private enum UpdateMinLengths
        {
            SkipHeights,
            SkipWidths,
            WidthsAndHeights
        }
    }
}