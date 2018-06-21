#region Copyright

/*SourceGrid LICENSE (MIT style)

Copyright (c) 2005 - 2012 http://sourcegrid.codeplex.com/, Davide Icardi, Darius Damalakas

Permission is hereby granted, free of charge, to any person obtaining 
a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE. */

//------------------------------------------------------------------------ 
// Copyright (C) Siemens AG 2016    
//------------------------------------------------------------------------ 
// Project           : UIGrid
// Author            : Sandhra.Prakash@siemens.com
// In Charge for Code: Sandhra.Prakash@siemens.com
//------------------------------------------------------------------------ 

/*Changes : Methods were copied from GridVirtual, RowsBase, ColumnsBase to create CellByCellScrolling.cs
 * 1. GetScrollPostionToCell -ExtendedTopRowsHidden was commented as a fix for CHDOC00123837. y value is correct even without this assignement.
 * it works as expected without changing y value. the calculation in this method doesn't seem right
*/
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevAge.Drawing;

namespace SourceGrid
{
    internal class CellByCellScrolling : IScrollingStyle
    {
        #region Fields

        private readonly GridVirtual m_Grid;

        #endregion Fields

        #region Constructor

        public CellByCellScrolling(GridVirtual gridVirtual)
        {
            m_Grid = gridVirtual;
        }

        #endregion Constructor

        #region IScrollingStyle

        /// <summary>
        /// Returns the first visible scrollable row.
        /// Return null if there isn't a visible row.
        /// </summary>
        /// <returns></returns>
        public int? FirstVisibleScrollableRow
        {
            get
            {
                int firstVisible = m_Grid.Rows.HiddenRowsCoordinator.ConvertScrollbarValueToRowIndex(m_Grid.CustomScrollPosition.Y) + m_Grid.FixedRows;

                if (firstVisible >= m_Grid.Rows.Count)
                    return null;
                else
                    return firstVisible;
            }
        }

        /// <summary>
        /// Returns the last visible scrollable row.
        /// Return null if there isn't a visible row.
        /// </summary>
        /// <returns></returns>
        public int? LastVisibleScrollableRow
        {
            get
            {
                int? first = FirstVisibleScrollableRow;
                if (first == null)
                    return null;

                Rectangle scrollableArea = m_Grid.GetScrollableArea();

                int bottom = GetTop(first.Value);
                int r = first.Value;
                for (; r < m_Grid.Rows.Count; r++)
                {
                    bottom += m_Grid.Rows.GetHeight(r);

                    if (bottom >= scrollableArea.Bottom)
                        return r;
                }

                return r - 1;
            }
        }


        /// <summary>
        /// Returns the first visible scrollable column.
        /// Return null if there isn't a visible column.
        /// </summary>
        /// <returns></returns>
        public int? FirstVisibleScrollableColumn
        {
            get
            {
                int firstVisible = m_Grid.CustomScrollPosition.X + m_Grid.FixedColumns;

                if (firstVisible >= m_Grid.Columns.Count)
                    return null;
                else
                    return firstVisible;
            }
        }

        /// <summary>
        /// Returns the last visible scrollable column.
        /// Return null if there isn't a visible column.
        /// </summary>
        /// <returns></returns>
        public int? LastVisibleScrollableColumn
        {
            get
            {
                int? first = FirstVisibleScrollableColumn;
                if (first == null)
                    return null;

                Rectangle scrollableArea = m_Grid.GetScrollableArea();

                int right = GetLeft(first.Value);
                int c = first.Value;
                for (; c < m_Grid.Columns.Count; c++)
                {
                    right += m_Grid.Columns.GetWidth(c);

                    if (right >= scrollableArea.Right)
                        return c;
                }

                return c - 1;
            }
        }
		
        //MICK(11)
        /// <summary>
        /// Scroll the page down to line
        /// </summary>
        public void CustomScrollPageUp(int line)
        {
            m_Grid.CustomScrollPageToLine(line - m_Grid.ActualFixedRows);
        }
        //MICK(11)
        /// <summary>
        /// Scroll the page down to line
        /// </summary>
        public void CustomScrollPageDown(int line)
        {
            m_Grid.CustomScrollPageToLine(line - m_Grid.ActualFixedRows);
        }

        public Range GetScrollableRange()
        {
            int? firstRow = FirstVisibleScrollableRow;
            int? lastRow = LastVisibleScrollableRow;

            int? firstCol = FirstVisibleScrollableColumn;
            int? lastCol = LastVisibleScrollableColumn;

            if (firstRow == null || firstCol == null ||
                lastRow == null || lastCol == null)
                return Range.Empty;

            return new Range(firstRow.Value, firstCol.Value,
                lastRow.Value, lastCol.Value);
        }

        public bool IsColumnHiddenUnderFixedColumn(int column, bool includePartial)
        {
            int hScroll = 0;
            if (m_Grid.HScrollBarVisible)
                hScroll = m_Grid.HScrollBar.Value;

            if (column < 0)
                return true;

            if (column >= m_Grid.ActualFixedColumns)
            {
                if (column - m_Grid.ActualFixedColumns - hScroll < 0)
                    return true;
            }

            return false;
        }

        public bool IsRowHiddenUnderFixedRows(int rowIndex, bool includePartial)
        {
            int vScroll = 0;
            if (m_Grid.VScrollBarVisible)
                vScroll = m_Grid.VScrollBar.Value;

            if (rowIndex < 0)
                return true;

            if (rowIndex >= m_Grid.ActualFixedRows)
            {
                if (rowIndex - m_Grid.ActualFixedRows - vScroll < 0)
                    return true;
            }

            return false;
        }

        public void CustomScrollLineDown()
        {
            m_Grid.CellByCellScrollDown();
        }

        public void CustomScrollLineRight()
        {
            m_Grid.CellByCellScrollRight();
        }

        public bool GetScrollPositionToShowCell(Position position, List<int> visibleColumns, List<int> visibleRows, out Point newScrollPosition)
        {
            CellPositionType posType = m_Grid.GetPositionType(position);
            Rectangle displayRectangle = m_Grid.DisplayRectangle;

            bool isFixedLeft = posType == CellPositionType.FixedLeft || posType == CellPositionType.FixedTopLeft;
            int x;
            if (visibleColumns.Contains(position.Column)) //Is x visible
            {
                x = m_Grid.CustomScrollPosition.X;
            }
            else
            {
                if (isFixedLeft)
                    x = 0;
                else
                    x = position.Column - m_Grid.ActualFixedColumns;

                //Check if the scrollable positioin if not outside the valid area
                int maxX = m_Grid.GetScrollColumns(displayRectangle.Width);
                if (x > maxX)
                    x = maxX;
            }

            bool isFixedTop = posType == CellPositionType.FixedTop || posType == CellPositionType.FixedTopLeft;
            int y;
            if (visibleRows.Contains(position.Row)) //Is y visible
            {
                y = m_Grid.CustomScrollPosition.Y;
                //sandhra.prakash@siemens.com:CHDOC00123837: it works as expected without changing y value. the calculation in this method doesn't seem right
                //y = m_Grid.ExtendTopRowByHiddenRows(y);
            }
            else
            {
                //MICK(15): add direction dependent procedure of finding y (and x should be too).
                //It means that when you go from bottom to top, the selected item will be at top,
                //              (this already happens, so this part is not changed)
                //              when you go from top to bottom, the selected item will be at bottom,
                // y (and x) value is adjusted accordingly
                if (m_Grid.CustomScrollPosition.Y + m_Grid.ActualFixedRows > position.Row)
                {
                    //MICK(16): direction bottom to top
                    if (isFixedTop)
                        y = 0;
                    else
                        y = position.Row - m_Grid.FixedRows;

                    //Check if the scrollable positioin if not outside the valid area
                    int maxY = m_Grid.GetScrollRows(displayRectangle.Height);
                    if (y > maxY)
                        y = maxY;
                    //sandhra.prakash@siemens.com:CHDOC00123837: it works as expected without changing y value. the calculation in this method doesn't seem right 
                    //MICK(21)
                    //y = m_Grid.ExtendTopRowByHiddenRows(y);

                }
                else
                {
                    //MICK(17): direction from top to bottom
                    y = m_Grid.GetTopVisibleRowFromBottomRow(position.Row) - m_Grid.ActualFixedRows;
                    //sandhra.prakash@siemens.com: CHDOC00123837: it works as expected without changing y value. the calculation in this method doesn't seem right 
                    //y = m_Grid.ExtendTopRowByHiddenRows(y);
                }
            }

            newScrollPosition = new Point(x, y);

            return true;
        }

        public Range GetFixedTopRange()
        {
            int actualFixed = m_Grid.FixedRows;
            if (actualFixed > m_Grid.Rows.Count)
                actualFixed = m_Grid.Rows.Count;

            if (actualFixed <= 0)
                return Range.Empty;

            int? firstCol = FirstVisibleScrollableColumn;
            int? lastCol = LastVisibleScrollableColumn;

            if (firstCol == null || lastCol == null)
                return Range.Empty;

            return new Range(0, firstCol.Value, actualFixed - 1, lastCol.Value);
        }

        public Range GetFixedLeftRange()
        {
            int actualFixed = m_Grid.FixedColumns;
            if (actualFixed > m_Grid.Columns.Count)
                actualFixed = m_Grid.Columns.Count;

            if (actualFixed <= 0)
                return Range.Empty;

            int? firstRow = FirstVisibleScrollableRow;
            int? lastRow = LastVisibleScrollableRow;

            if (firstRow == null || lastRow == null)
                return Range.Empty;

            return new Range(firstRow.Value, 0, lastRow.Value, actualFixed - 1);
        }

        /// <summary>
        /// recalculate the position of the horizontal scrollbar
        /// </summary>
        public void RecalcHScrollBar(int cols)
        {
            if (m_Grid.HScrollBarVisible == false)
                return;

            m_Grid.HScrollBar.Minimum = 0;
            m_Grid.HScrollBar.Maximum = cols + m_Grid.HorizontalPage - 1;

            if (m_Grid.HorizontalPage > 1)
                m_Grid.HScrollBar.LargeChange = m_Grid.HorizontalPage;
            else
                m_Grid.HScrollBar.LargeChange = 1;

            m_Grid.HScrollBar.SmallChange = 1;

            if (m_Grid.HScrollBar.Value > m_Grid.MaximumHScroll)
                m_Grid.HScrollBar.Value = m_Grid.MaximumHScroll;
        }

        /// <summary>
        /// Recalculate the position of the vertical scrollbar
        /// </summary>
        public void RecalcVScrollBar(int rows)
        {
            if (m_Grid.VScrollBarVisible == false)
                return;
            m_Grid.VScrollBar.Minimum = 0;
            //MICK(4): Now (rows + VerticalPage - 1) equals the count of all rows minus fixed rows...
            // But I do not change it here because I do not know how to do that properly
            m_Grid.VScrollBar.Maximum = rows + m_Grid.VerticalPage - 1;
            //MICK(5): this is changed too
            if (m_Grid.VerticalPage > 1)
                m_Grid.VScrollBar.LargeChange = m_Grid.VerticalPage;
            else
                m_Grid.VScrollBar.LargeChange = 1;

            m_Grid.VScrollBar.SmallChange = 1;

            if (m_Grid.VScrollBar.Value > m_Grid.MaximumVScroll)
                m_Grid.VScrollBar.Value = m_Grid.MaximumVScroll;
        }

        public void Paint(PaintEventArgs e)
        {
            //NOTE: For now I draw all the visible cells (not only the invalidated cells).
            using (GraphicsCache grCache = new GraphicsCache(e.Graphics, e.ClipRectangle))
            {
                foreach (Range rng in m_Grid.GetVisibleRegion())
                {
                    m_Grid.OnRangePaint(new RangePaintEventArgs(m_Grid, grCache, rng));
                }
            }
        }

        public void PaintMergedCell(GraphicsCache graphics, Range cellRange, CellContext cellContext)
        {
            Grid grid = m_Grid as Grid;
            if (grid == null)
                return;

            Rectangle spanRect = grid.RangeToRectangle(cellRange);

            grid.PaintMergedCell(graphics, cellContext, spanRect);

        }
        /// <summary>
        /// Gets the columns index inside the specified display area.
        /// The list returned is ordered by the index.
        /// Note that this method returns also invisible rows.
        /// </summary>
        /// <param name="returnsPartial">True to returns also partial columns</param>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <param name="returnsFixedColumns"></param>
        /// <returns></returns>
        public List<int> ColumnsInsideRegion(int x, int width, bool returnsPartial, bool returnsFixedColumns)
        {
            int right = x + width;

            List<int> list = new List<int>();

            //Add the fixed columns
            // Loop until the currentHeight is smaller then the requested displayRect
            for (int fr = 0; fr < m_Grid.FixedColumns && fr < m_Grid.Columns.Count; fr++)
            {
                int leftDisplay = m_Grid.Columns.GetLeft(fr);
                int rightDisplay = leftDisplay + m_Grid.Columns.GetWidth(fr);

                //If the column is inside the view
                if (right >= leftDisplay && x <= rightDisplay &&
                    (returnsPartial || (rightDisplay <= right && leftDisplay >= x)))
                {
                    if (returnsFixedColumns)
                        list.Add(fr);
                }

                if (rightDisplay > right)
                    break;
            }

            int? relativeCol = FirstVisibleScrollableColumn;

            if (relativeCol != null)
            {
                //Add the standard columns
                for (int r = relativeCol.Value; r < m_Grid.Columns.Count; r++)
                {
                    int leftDisplay = m_Grid.Columns.GetLeft(r);
                    int rightDisplay = leftDisplay + m_Grid.Columns.GetWidth(r);

                    //If the column is inside the view
                    if (right >= leftDisplay && x <= rightDisplay &&
                        (returnsPartial || (rightDisplay <= right && leftDisplay >= x)))
                    {
                        list.Add(r);
                    }

                    if (rightDisplay > right)
                        break;
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the rows index inside the specified display area.
        /// The list returned is ordered by the index.
        /// Note that this method returns also invisible rows.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="returnsPartial">True to returns also partial rows</param>
        /// <param name="returnsFixedRows"></param>
        public List<int> RowsInsideRegion(int y, int height, bool returnsPartial, bool returnsFixedRows)
        {
            int bottom = y + height;

            List<int> list = new List<int>();

            //Add the fixed rows
            // Loop until the currentHeight is smaller then the requested displayRect
            for (int fr = 0; fr < m_Grid.FixedRows && fr < m_Grid.Rows.Count; fr++)
            {
                int topDisplay = m_Grid.Rows.GetTop(fr);
                int bottomDisplay = topDisplay + m_Grid.Rows.GetHeight(fr);

                //If the row is inside the view
                if (bottom >= topDisplay && y <= bottomDisplay &&
                    (returnsPartial || (bottomDisplay <= bottom && topDisplay >= y)))
                {
                    if (returnsFixedRows)
                        list.Add(fr);
                }

                if (bottomDisplay > bottom)
                    break;
            }

            int? relativeRow = FirstVisibleScrollableRow;

            if (relativeRow != null)
            {
                //Add the standard rows
                for (int r = relativeRow.Value; r < m_Grid.Rows.Count; r++)
                {
                    int topDisplay = m_Grid.Rows.GetTop(r);
                    int bottomDisplay = topDisplay + m_Grid.Rows.GetHeight(r);

                    //If the row is inside the view
                    if (bottom >= topDisplay && y <= bottomDisplay &&
                        (returnsPartial || (bottomDisplay <= bottom && topDisplay >= y)))
                    {
                        list.Add(r);
                    }

                    if (bottomDisplay > bottom)
                        break;
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the column left position.
        /// The Left is relative to the specified start position.
        /// Calculate the left using also the FixedColumn if present.
        /// </summary>
        public int GetLeft(int column)
        {
            int actualFixedColumns = Math.Min(m_Grid.FixedColumns, m_Grid.Columns.Count);

            int left = 0;

            //Calculate fixed left cells
            for (int i = 0; i < actualFixedColumns; i++)
            {
                if (i == column)
                    return left;

                left += m_Grid.Columns.GetWidth(i);
            }

            int? relativeColumn = FirstVisibleScrollableColumn;
            if (relativeColumn == null)
                relativeColumn = m_Grid.Columns.Count;

            if (relativeColumn == column)
                return left;
            else if (relativeColumn < column)
            {
                for (int i = relativeColumn.Value; i < m_Grid.Columns.Count; i++)
                {
                    if (i == column)
                        return left;

                    left += m_Grid.Columns.GetWidth(i);
                }
            }
            else if (relativeColumn > column)
            {
                for (int i = relativeColumn.Value - 1; i >= 0; i--)
                {
                    left -= m_Grid.Columns.GetWidth(i);

                    if (i == column)
                        return left;
                }
            }

            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the row top position.
        /// The Top is relative to the specified start position.
        /// Calculate the top using also the FixedRows if present.
        /// </summary>
        public int GetTop(int row)
        {
            if (row < 0)
                throw new ArgumentNullException("Row is less than 0");
            int actualFixedRows = Math.Min(m_Grid.FixedRows, m_Grid.Rows.Count);

            int top = 0;

            //Calculate fixed top cells
            for (int i = 0; i < actualFixedRows; i++)
            {
                if (i == row)
                    return top;

                top += m_Grid.Rows.GetHeight(i);
            }

            int? relativeRow = FirstVisibleScrollableRow;
            if (relativeRow == null)
                relativeRow = m_Grid.Rows.Count;

            if (relativeRow == row)
                return top;
            else if (relativeRow < row)
            {
                top += m_Grid.Rows.GetTopPositive(relativeRow.Value, row);
                return top;
            }
            else if (relativeRow > row)
            {
                top += m_Grid.Rows.GetTopNegative(relativeRow.Value, row);
                return top;
            }

            throw new IndexOutOfRangeException(string.Format("row value is {0}", row));
        }

        public Rectangle RangeToVisibleRectangle(Range range)
        {
            if (range.IsEmpty())
                return Rectangle.Empty;
            if (range.Start.Column < 0)
                throw new ArgumentOutOfRangeException(string.Format("range.Start.Column was less than zero: {0}", range.Start.Column));
            if (range.Start.Row < 0)
                throw new ArgumentOutOfRangeException(string.Format("range.Start.Row was less than zero: {0}", range.Start.Row));

            int firstRow = FindFirstVisibleRowFromRange(range);
            if (firstRow < 0)
                return Rectangle.Empty;

            int actualFixedRows = m_Grid.ActualFixedRows;

            int actualFixedColumns = m_Grid.ActualFixedColumns;

            int y = m_Grid.Rows.GetTop(firstRow);

            if (m_Grid.IsRowHiddenUnderFixedRows(firstRow, true))
            {
                y = m_Grid.GetFixedAreaHeight();
            }

            int firstColumn = FindFirstVisibleColumnFromRange(range);
            if (firstColumn < 0)
                return Rectangle.Empty;

            int x = m_Grid.Columns.GetLeft(firstColumn);

            if (IsColumnHiddenUnderFixedColumn(firstColumn, true))
            {
                x = m_Grid.GetFixedAreaWidth();
            }

            int lastColumn = range.End.Column;
            if (range.End.Column != actualFixedColumns - 1 && m_Grid.IsColumnHiddenUnderFixedColumn(range.End.Column))
            {
                lastColumn = actualFixedColumns - 1;
            }
            int width = m_Grid.Columns.GetRight(lastColumn) - x;

            int lastRow = range.End.Row;
            if (range.End.Row != actualFixedRows - 1 && m_Grid.IsRowHiddenUnderFixedRows(range.End.Row))
            {
                lastRow = actualFixedRows - 1;
            }
            int height = m_Grid.Rows.GetBottom(lastRow) - y;

            Size size = new Size(width, height);

            if (size.IsEmpty)
                return Rectangle.Empty;

            return new Rectangle(new Point(x, y), size);
        }

        public BorderPartType GetBorderType(Range rng)
        {
            BorderPartType partType = BorderPartType.All;
            if (m_Grid.IsRowHiddenUnderFixedRows(rng.End.Row))
            {
                partType &= ~(BorderPartType.BottomBorder | BorderPartType.DragRectangle);
            }
            if (m_Grid.IsColumnHiddenUnderFixedColumn(rng.End.Column))
            {
                partType &= ~(BorderPartType.RightBorder | BorderPartType.DragRectangle);
            }
            if (m_Grid.IsRowHiddenUnderFixedRows(rng.Start.Row, true))
            {
                partType &= ~(BorderPartType.TopBorder);
            }
            if (m_Grid.IsColumnHiddenUnderFixedColumn(rng.Start.Column, true))
            {
                partType &= ~(BorderPartType.LeftBorder);
            }
            return partType;
        }


        /// <summary>
        /// Move the scrollbars to the direction specified by the point specified.
        /// Method used by the Mouse multi selection (MouseSelection.cs).
        /// Scroll the grid only if the specified location is outside the visible area.
        /// </summary>
        /// <param name="mousePoint"></param>
        public void ScrollOnPoint(Point mousePoint)
        {
            //Scroll if necesary
            Rectangle scrollRect = m_Grid.GetScrollableArea();

            //sandhra.prakash@siemens.com : In the below if condition, adding DragOffset value also 
            // to resolve a defect (when grid is the control which fits into the entire area of screen
            // and the form which holds the grid is maximised, Autoscrolling to one or more sides is not possible)
            //This reduces the scrollable area and when inside the grid at that time also autoscrolling will be triggered.
            if (m_Grid.IsCustomAreaAutoScrollEnabled && !m_Grid.DragSizeRectangle.Contains(mousePoint))
            {
                scrollRect = new Rectangle(scrollRect.X + m_Grid.DragOffset,
                                scrollRect.Y + m_Grid.DragOffset,
                                scrollRect.Width - 2 * m_Grid.DragOffset,
                                scrollRect.Height - 2 * m_Grid.DragOffset);
            }

            int? last = LastVisibleScrollableColumn;
            if (mousePoint.X > scrollRect.Right && (!last.HasValue ||
                                                    last.Value < m_Grid.Columns.Count - 1 || m_Grid.Columns.GetRight(last.Value) > scrollRect.Right))
                CustomScrollLineRight();
            last = LastVisibleScrollableRow;
            if (mousePoint.Y > scrollRect.Bottom && (!last.HasValue ||
                                                     last.Value < m_Grid.Rows.Count - 1 || m_Grid.Rows.GetBottom(last.Value) > scrollRect.Bottom))
                CustomScrollLineDown();
            if (mousePoint.X < scrollRect.Left)
                m_Grid.CustomScrollLineLeft();
            if (mousePoint.Y < scrollRect.Top)
                m_Grid.CustomScrollLineUp();
        }
        #endregion IScrollingStyle

        #region Private methods

        ///  <summary>
        /// sandhra.prakash@siemens.com: To find the first visible Row from range.
        ///  partially visible ones are also treated as visible hence it will be returned
        ///  </summary>
        ///  <param name="range"></param>
        /// <returns></returns>
        private int FindFirstVisibleRowFromRange(Range range)
        {
            if (range.Start.Row <= m_Grid.ActualFixedRows - 1)
                return range.Start.Row;
            for (int row = range.Start.Row; row <= range.End.Row; row++)
            {
                if (!m_Grid.IsRowHiddenUnderFixedRows(row))
                {
                    return row;
                }
            }
            return -1;
        }


        /// <summary>
        ///sandhra.prakash@siemens.com: To find the first visible column from range.
        /// partially visible ones are also treated as visible hence it will be returned
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private int FindFirstVisibleColumnFromRange(Range range)
        {
            if (range.Start.Column <= m_Grid.ActualFixedColumns - 1)
                return range.Start.Column;
            for (int col = range.Start.Column; col <= range.End.Column; col++)
            {
                if (!m_Grid.IsColumnHiddenUnderFixedColumn(col))
                {
                    return col;
                }
            }
            return -1;
        }

        #endregion Private methods

    }
}