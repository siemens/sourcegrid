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

/*Changes : 
 * 
*/
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevAge.Drawing;

namespace SourceGrid
{
    class SmoothScrolling : IScrollingStyle
    {
        #region Fields

        private readonly GridVirtual m_Grid;

        private int m_FirstVisibleScrollableRow;
        private int m_FirstVisibleScrollableColumn;
        private int m_FirstVisibleScrollableRowTop;
        private int m_FirstVisibleScrollableColumnLeft;

        #endregion Fields

        #region Constructor

        public SmoothScrolling(GridVirtual grid)
        {
            m_Grid = grid;
        }

        #endregion Constructor

        #region IScrollingStyle members

        /// <summary>
        /// Returns the first visible scrollable row.
        /// Return null if there isn't a visible row.
        /// </summary>
        /// <returns></returns>
        public int? FirstVisibleScrollableRow
        {
            get
            {
                //sandhra.prakash@siemens.com: These checks ensure that m_FirstVisibleScrollableRow and m_FirstVisibleScrollableRowTop is having valid values at any point of time.
                if (m_FirstVisibleScrollableRow <= -1 || m_FirstVisibleScrollableRow >= m_Grid.Rows.Count
                    || m_FirstVisibleScrollableRowTop < (m_Grid.GetFixedAreaHeight() - m_Grid.VScrollBar.Value))
                {
                    Range range = GetScrollableRange();
                    m_FirstVisibleScrollableRow = FindFirstVisibleRowFromRange(range, out m_FirstVisibleScrollableRowTop);
                }
                
                return m_FirstVisibleScrollableRow;
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
                //sandhra.prakash@siemens.com: These checks ensure that m_FirstVisibleScrollableColumn and m_FirstVisibleScrollableColumnLeft is having valid values at any point of time.
                if (m_FirstVisibleScrollableColumn <= -1 || m_FirstVisibleScrollableColumn >= m_Grid.Columns.Count 
                    || m_FirstVisibleScrollableColumnLeft < (m_Grid.GetFixedAreaWidth() - m_Grid.HScrollBar.Value))
                {
                    Range range = GetScrollableRange();
                    m_FirstVisibleScrollableColumn = FindFirstVisibleColumnFromRange(range, out m_FirstVisibleScrollableColumnLeft);
                }

                return m_FirstVisibleScrollableColumn;
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

        /// <summary>
        /// Scroll the page down to line
        /// </summary>
        public void CustomScrollPageUp(int line)
        {
            m_Grid.CustomScrollPageToLine(m_Grid.Rows.GetAbsoluteTop(line));
        }

        /// <summary>
        /// Scroll the page down to line
        /// </summary>
        public void CustomScrollPageDown(int row)
        {
            if(row >= m_Grid.ActualFixedRows)
                m_Grid.CustomScrollPageToLine(m_Grid.Rows.GetAbsoluteTop(row - m_Grid.ActualFixedRows));
        }


        public Range GetScrollableRange()
        {
            if (m_Grid.Rows.Count <= 0 || m_Grid.Columns.Count <= 0 ||
                m_Grid.ActualFixedRows >= m_Grid.Rows.Count || m_Grid.ActualFixedColumns >= m_Grid.Columns.Count)
                return Range.Empty;

            Range range = new Range(m_Grid.ActualFixedRows, m_Grid.ActualFixedColumns,
                m_Grid.Rows.Count - 1, m_Grid.Columns.Count - 1);
            return range;
        }

        ///  <summary>
        /// sandhra.prakash@siemens.com: To find if the specified column is hidden under the fixedColumn
        ///  </summary>
        /// <param name="column"></param>
        /// <param name="includePartial">True - partially hidden col will also be treated as hidden</param>
        /// <returns></returns>
        public bool IsColumnHiddenUnderFixedColumn(int column, bool includePartial)
        {
            int actualFixedColumns = m_Grid.ActualFixedColumns;

            if (column < actualFixedColumns || column >= m_Grid.Columns.Count)
            {
                return false;
            }

            int fixedRight = m_Grid.GetFixedAreaWidth();
            int left = m_Grid.Columns.GetLeft(column);
            int right = left + m_Grid.Columns.GetWidth(column);
            return IsColumnHiddenUnderFixedColumn(fixedRight, left, right, column, includePartial);
        }

        ///  <summary>
        /// sandhra.prakash@siemens.com: To find if the specified row is hidden under the fixedRow
        ///  </summary>
        /// <param name="rowIndex"></param>
        /// <param name="includePartial">True - partially hidden row will also be treated as hidden</param>
        /// <returns></returns>
        public bool IsRowHiddenUnderFixedRows(int rowIndex, bool includePartial)
        {
            int actualFixedRows = m_Grid.ActualFixedRows;

            if (rowIndex < actualFixedRows || rowIndex >= m_Grid.Rows.Count)
            {
                return false;
            }

            int fixedBottom = m_Grid.GetFixedAreaHeight();
            int top = m_Grid.Rows.GetTop(rowIndex);
            int bottom = top + m_Grid.Rows.GetHeight(rowIndex);
            return IsRowHiddenUnderFixedRows(fixedBottom, top, bottom, rowIndex, includePartial);
        }

        public void CustomScrollLineDown()
        {
            if (m_Grid.VScrollBarVisible && (m_Grid.VScrollBar.Value + m_Grid.VScrollBar.LargeChange) <= m_Grid.VScrollBar.Maximum)
                m_Grid.VScrollBar.Value = Math.Min(m_Grid.VScrollBar.Value + m_Grid.VScrollBar.SmallChange, m_Grid.VScrollBar.Maximum);
        }

        public void CustomScrollLineRight()
        {
            if (m_Grid.HScrollBarVisible && m_Grid.HScrollBar.Value + m_Grid.HScrollBar.LargeChange <= m_Grid.HScrollBar.Maximum)
                m_Grid.HScrollBar.Value = Math.Min(m_Grid.HScrollBar.Value + m_Grid.HScrollBar.SmallChange, m_Grid.HScrollBar.Maximum);
        }

        public Range GetFixedTopRange()
        {
            Range fixedTop = Range.Empty;

            int actualFixedColumns = m_Grid.ActualFixedColumns;
            int actualFixedRows = m_Grid.ActualFixedRows;

            if (actualFixedRows > 0 && actualFixedColumns >= 0 && m_Grid.Columns.Count > 0 && 
                m_Grid.ActualFixedRows <= m_Grid.Rows.Count && m_Grid.ActualFixedColumns < m_Grid.Columns.Count)
                fixedTop = new Range(0, actualFixedColumns, actualFixedRows - 1, m_Grid.Columns.Count - 1);

            return fixedTop;
        }

        public Range GetFixedLeftRange()
        {
            Range fixedLeft = Range.Empty;

            int actualFixedColumns = m_Grid.ActualFixedColumns;
            int actualFixedRows = m_Grid.ActualFixedRows;

            if (actualFixedColumns > 0 && actualFixedRows >= 0 && m_Grid.Rows.Count > 0 &&
                m_Grid.ActualFixedRows < m_Grid.Rows.Count && m_Grid.ActualFixedColumns <= m_Grid.Columns.Count)
                fixedLeft = new Range(actualFixedRows, 0, m_Grid.Rows.Count - 1, actualFixedColumns - 1);
            return fixedLeft;
        }

        public bool GetScrollPositionToShowCell(Position position, List<int> columns, List<int> rows, out Point newScrollPosition)
        {
            newScrollPosition = m_Grid.CustomScrollPosition;
            if (!m_Grid.IsPositionValid(position))
                return false;

            CellPositionType posType = m_Grid.GetPositionType(position);
            Rectangle displayRectangle = m_Grid.DisplayRectangle;

            bool isFixedLeft = posType == CellPositionType.FixedLeft || posType == CellPositionType.FixedTopLeft;
            int x = m_Grid.CustomScrollPosition.X;
            if (!columns.Contains(position.Column))
            {
                if (isFixedLeft)
                    x = 0;
                else if (columns.Count > m_Grid.ActualFixedColumns)
                {
                    if (position.Column < columns[m_Grid.ActualFixedColumns])
                    {
                        x = m_Grid.Columns.GetAbsoluteLeft(position.Column) - m_Grid.GetFixedAreaWidth();
                    }
                    else
                    {
                        x = m_Grid.Columns.GetAbsoluteRight(position.Column) - displayRectangle.Width;
                    }
                }
                else
                {
                    x = m_Grid.Columns.GetAbsoluteLeft(position.Column) - m_Grid.GetFixedAreaWidth();
                }
            }

            bool isFixedTop = posType == CellPositionType.FixedTop || posType == CellPositionType.FixedTopLeft;
            int y = m_Grid.CustomScrollPosition.Y;

            if (!rows.Contains(position.Row))
            {
                if (isFixedTop)
                    y = 0;
                else if (rows.Count > m_Grid.ActualFixedRows)
                {
                    if (position.Row < rows[m_Grid.ActualFixedRows])
                    {
                        y = m_Grid.Rows.GetAbsoluteTop(position.Row) - m_Grid.GetFixedAreaHeight();
                    }
                    else
                    {
                        y = m_Grid.Rows.GetAbsoluteBottom(position.Row) - displayRectangle.Height;
                    }
                }
                else
                {
                    y = m_Grid.Rows.GetAbsoluteTop(position.Row) - m_Grid.GetFixedAreaHeight();
                }
            }

            newScrollPosition = new Point(x, y);

            return true;
        }

        public void RecalcHScrollBar(int cols)
        {
            if (m_Grid.Columns.Count <= 0 || m_Grid.Rows.Count <= 0)
            {
                m_Grid.HScrollBarVisible = false;
            }

            if (m_Grid.HScrollBarVisible == false)
                return;

            int widthOfScrollableArea = GetNonFixedAreaWidth();

            if (widthOfScrollableArea == 0)
            {
                m_Grid.HScrollBarVisible = false;
            }
            m_Grid.HScrollBar.Maximum = widthOfScrollableArea;
            m_Grid.HScrollBar.Minimum = 0;
            m_Grid.HScrollBar.SmallChange = 10;

            int onScreenScrollableAreaWidth = m_Grid.DisplayRectangle.Width - m_Grid.GetFixedAreaWidth();

            if (onScreenScrollableAreaWidth >= 0)
            {
                m_Grid.HScrollBar.LargeChange = onScreenScrollableAreaWidth;
            }

            if (m_Grid.HScrollBar.Value > m_Grid.MaximumHScroll)
                m_Grid.HScrollBar.Value = m_Grid.MaximumHScroll;
        }

        public void RecalcVScrollBar(int rows)
        {
            if (m_Grid.Columns.Count <= 0 || m_Grid.Rows.Count <= 0)
            {
                m_Grid.VScrollBarVisible = false;
            }

            if (m_Grid.VScrollBarVisible == false)
                return;

            int heightOfScrollableArea = GetNonFixedAreaHeight();

            if (heightOfScrollableArea == 0)
            {
                m_Grid.VScrollBarVisible = false;
            }

            m_Grid.VScrollBar.Maximum = heightOfScrollableArea;
            m_Grid.VScrollBar.Minimum = 0;
            m_Grid.VScrollBar.SmallChange = 10;

            int onScreenScrollableAreaHeight = m_Grid.DisplayRectangle.Height - m_Grid.GetFixedAreaHeight();
            if (onScreenScrollableAreaHeight >= 0)
            {
                m_Grid.VScrollBar.LargeChange = onScreenScrollableAreaHeight;
            }

            if (m_Grid.VScrollBar.Value > m_Grid.MaximumVScroll)
                m_Grid.VScrollBar.Value = m_Grid.MaximumVScroll;

        }

        public void Paint(PaintEventArgs e)
        {
            if (m_Grid.Columns.Count <= 0 || m_Grid.Rows.Count <= 0)
                return;

            int scrollableAreaX = m_Grid.GetFixedAreaWidth();
            int scrollableAreaY = m_Grid.GetFixedAreaHeight();

            Point scrollableArea = new Point(scrollableAreaX, scrollableAreaY);

            DrawFixedTopLeft(e);

            DrawFixedTop(e, scrollableArea);

            DrawFixedLeft(e, scrollableArea);

            DrawScrollableArea(e, scrollableArea);
        }

        public void PaintMergedCell(GraphicsCache graphics, Range cellRange, CellContext cellContext)
        {
            Grid grid = m_Grid as Grid;
            if (grid == null)
                return;

            GraphicsState state = graphics.Graphics.Save();

            graphics.Graphics.ResetClip();
            graphics.Graphics.ResetTransform();

            Rectangle spanRect = grid.RangeToRectangle(cellRange);

            Rectangle clientRectangle = grid.RangeToVisibleRectangle(cellRange);

            int actualFixedColumns = grid.ActualFixedColumns;
            int actualFixedRows = grid.ActualFixedRows;

            if (cellRange.Start.Column < actualFixedColumns && cellRange.End.Column >= actualFixedColumns)
            {
                spanRect.Width = clientRectangle.Width;
            }

            if (cellRange.Start.Row < actualFixedRows && cellRange.End.Row >= actualFixedRows)
            {
                spanRect.Height = clientRectangle.Height;
            }


            graphics.Graphics.SetClip(clientRectangle);

            graphics.Graphics.TranslateTransform(spanRect.X - clientRectangle.X, spanRect.Y - clientRectangle.Y);

            spanRect.Location = clientRectangle.Location;

            using (GraphicsCache graphicsCache = new GraphicsCache(graphics.Graphics, Rectangle.Round(graphics.Graphics.ClipBounds)))
            {
                grid.PaintMergedCell(graphicsCache, cellContext, spanRect);
            }

            graphics.Graphics.ResetTransform();
            graphics.Graphics.ResetClip();
            graphics.Graphics.Restore(state);

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
            int colCount = m_Grid.Columns.Count;

            List<int> list = new List<int>();
            int fixedRight = 0;
            int leftDisplay = 0;
            int actualFixedColumns = m_Grid.ActualFixedColumns;

            if (returnsFixedColumns)
            //Add the fixed columns
            // Loop until the currentHeight is smaller then the requested displayRect
            {
                for (int col = 0; col < actualFixedColumns && col < colCount; col++)
                {
                    fixedRight = leftDisplay + m_Grid.Columns.GetWidth(col);

                    //If the column is inside the view
                    if (right > leftDisplay && x < fixedRight &&
                        (returnsPartial || (fixedRight <= right && leftDisplay >= x)))
                    {
                        list.Add(col);
                    }

                    leftDisplay = fixedRight;

                    if (fixedRight > right)
                        break;
                }
            }
            int? firstRow = FirstVisibleScrollableColumn;

            if (firstRow.HasValue && firstRow > -1)
            {
                leftDisplay = m_FirstVisibleScrollableColumnLeft;

                //Add the standard columns
                for (int col = firstRow.Value; col < colCount; col++)
                {
                    int rightDisplay = leftDisplay + m_Grid.Columns.GetWidth(col);

                    //If the column is inside the view
                    if (right > leftDisplay && x < rightDisplay &&
                        ((returnsPartial)
                         || (rightDisplay <= right && leftDisplay >= x)))
                    {
                        if (!IsColumnHiddenUnderFixedColumn(fixedRight, leftDisplay, rightDisplay, col, !returnsPartial))
                            list.Add(col);
                    }

                    leftDisplay = rightDisplay;

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
            int rowsCount = m_Grid.Rows.Count;

            List<int> list = new List<int>();
            int fixedBottom = 0;
            int topDisplay = 0;
            int actualFixedRows = m_Grid.ActualFixedRows;

            if (returnsFixedRows)
            //Add the fixed rows
            // Loop until the currentHeight is smaller then the requested displayRect
            {
                for (int row = 0; row < actualFixedRows && row < rowsCount; row++)
                {
                    fixedBottom = topDisplay + m_Grid.Rows.GetHeight(row);

                    //If the row is inside the view
                    if (bottom > topDisplay && y < fixedBottom &&
                        (returnsPartial || (fixedBottom <= bottom && topDisplay >= y)))
                    {

                        list.Add(row);
                    }

                    topDisplay = fixedBottom;

                    if (fixedBottom > bottom)
                        break;
                }
            }

            int? firstRow = FirstVisibleScrollableRow;
            if (firstRow.HasValue && firstRow > -1)
            {
                topDisplay = m_FirstVisibleScrollableRowTop;

                //Add the standard rows
                for (int row = firstRow.Value; row < rowsCount; row++)
                {
                    int bottomDisplay = topDisplay + m_Grid.Rows.GetHeight(row);

                    //If the row is inside the view
                    if (bottom > topDisplay && y < bottomDisplay &&
                        ((returnsPartial)
                         || (bottomDisplay <= bottom && topDisplay >= y)))
                    {
                        if (!IsRowHiddenUnderFixedRows(fixedBottom, topDisplay, bottomDisplay, row, !returnsPartial))
                            list.Add(row);
                    }
                    topDisplay = bottomDisplay;

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
            if (m_FirstVisibleScrollableColumn < 0)
            {
                return GetLeft(0, 0, column);
            }

            int actualFixedColumns = Math.Min(m_Grid.FixedColumns, m_Grid.Columns.Count);

            int left = 0;
            if (column < actualFixedColumns)
            {
                //Calculate fixed left cells
                for (int i = 0; i < actualFixedColumns; i++)
                {
                    if (i == column)
                        return left;

                    left += m_Grid.Columns.GetWidth(i);
                }
            }
            int? relativeColumn = FirstVisibleScrollableColumn ?? m_Grid.Columns.Count;

            left = m_FirstVisibleScrollableColumnLeft;
            
            if (relativeColumn < column)
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
            return left;
            
        }

        public int GetTop(int row)
        {
            if (row < 0)
                throw new ArgumentNullException("Row is less than 0");

            if (m_FirstVisibleScrollableRow < 0)
            {
                return GetTop(0, 0, row);
            }

            int actualFixedRows = Math.Min(m_Grid.FixedRows, m_Grid.Rows.Count);

            int top = 0;
            if (row < actualFixedRows)
            {
                //Calculate fixed top cells
                for (int i = 0; i < actualFixedRows; i++)
                {
                    if (i == row)
                        return top;

                    top += m_Grid.Rows.GetHeight(i);
                }
            }

            int? relativeRow = FirstVisibleScrollableRow ?? m_Grid.Rows.Count;

            top = m_FirstVisibleScrollableRowTop;

            if (relativeRow < row)
            {
                top += m_Grid.Rows.GetTopPositive(relativeRow.Value, row);
            }
            else if (relativeRow > row)
            {
                top += m_Grid.Rows.GetTopNegative(relativeRow.Value, row);
            }
            return top;

        }

        private int GetTop(int startRow, int startRowAbsoluteTop, int row)
        {
            int top = startRowAbsoluteTop;

            int rowCount = m_Grid.Rows.Count;

            for (int i = startRow; i < rowCount; i++)
            {
                if (i == row)
                    break;

                top += m_Grid.Rows.GetHeight(i);
            }

            return row < m_Grid.ActualFixedRows ? top : top - m_Grid.CustomScrollPosition.Y;
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
            int fixedAreaHeight = m_Grid.GetFixedAreaHeight();
            int fixedAreaWidth = m_Grid.GetFixedAreaWidth();

            int firstRowAbsoluteTop = m_Grid.Rows.GetAbsoluteTop(firstRow);
            int y = firstRow < actualFixedRows ? firstRowAbsoluteTop : firstRowAbsoluteTop - m_Grid.CustomScrollPosition.Y;
            int bottom = y + m_Grid.Rows.GetHeight(firstRow);

            if (IsRowHiddenUnderFixedRows(fixedAreaHeight, y, bottom, firstRow, true))
            {
                y = fixedAreaHeight;
            }

            int firstColumn = FindFirstVisibleColumnFromRange(range);
            if (firstColumn < 0)
                return Rectangle.Empty;

            int firstColumnAbsoluteLeft = m_Grid.Columns.GetAbsoluteLeft(firstColumn);
            int x = firstColumn < actualFixedColumns ? firstColumnAbsoluteLeft : firstColumnAbsoluteLeft - m_Grid.CustomScrollPosition.X;
            int right = x + m_Grid.Columns.GetWidth(firstColumn);

            if (IsColumnHiddenUnderFixedColumn(fixedAreaWidth, x, right, firstColumn, true))
            {
                x = fixedAreaWidth;
            }

            int lastColumn = range.End.Column;
            int lastColumnLeft = GetLeft(firstColumn, firstColumnAbsoluteLeft, lastColumn);
            int lastColumnRight = lastColumnLeft + m_Grid.Columns.GetWidth(lastColumn);
            int width = lastColumnRight - x;

            if (lastColumn > actualFixedColumns - 1 && IsColumnHiddenUnderFixedColumn(fixedAreaWidth, lastColumnLeft, lastColumnRight, lastColumn, false))
            {
                //lastColumn = actualFixedColumns - 1;
                width = fixedAreaWidth - x;
            }


            int lastRow = range.End.Row;
            int lastRowTop = GetTop(firstRow, firstRowAbsoluteTop, lastRow);
            int lastRowBottom = lastRowTop + m_Grid.Rows.GetHeight(lastRow);
            int height = lastRowBottom - y;
            if (lastRow > actualFixedRows - 1 && IsRowHiddenUnderFixedRows(fixedAreaHeight, lastRowTop, lastRowBottom, lastRow, false))
            {
                //if last row is hidden under fixedRows 
                height = fixedAreaHeight - y;
            }

            Size size = new Size(width, height);

            if (size.IsEmpty)
                return Rectangle.Empty;

            return new Rectangle(new Point(x, y), size);
        }

        public BorderPartType GetBorderType(Range rng)
        {
            BorderPartType partType = BorderPartType.All;

            int lastFixedColumn = m_Grid.ActualFixedColumns - 1;
            int lastFixedRow = m_Grid.ActualFixedRows - 1;
            
            int? firstVisibleScrollableColumn = FirstVisibleScrollableColumn;
            int? firstVisibleScrollableRow = FirstVisibleScrollableRow;

            if (!firstVisibleScrollableColumn.HasValue || !firstVisibleScrollableRow.HasValue)
                return partType;

            #region firstColumn check

            int firstColumn = rng.Start.Column;

            if (firstColumn > lastFixedColumn && firstColumn < firstVisibleScrollableColumn)
            {
                partType &= ~(BorderPartType.LeftBorder);
            }
            else if (firstColumn == firstVisibleScrollableColumn)
            {
                if (IsColumnHiddenUnderFixedColumn(firstColumn, true))
                {
                    partType &= ~(BorderPartType.LeftBorder);
                }
            }
            #endregion firstColumn check

            #region lastColumn check
            int lastColumn = rng.End.Column;
            if (lastColumn != firstColumn && lastColumn > lastFixedColumn)
            {
                if (lastColumn < firstVisibleScrollableColumn)
                {
                    partType &= ~(BorderPartType.RightBorder | BorderPartType.DragRectangle);
                }
                else if (lastColumn == firstVisibleScrollableColumn)
                {
                    if (IsColumnHiddenUnderFixedColumn(lastColumn, false))
                    {
                        partType &= ~(BorderPartType.RightBorder | BorderPartType.DragRectangle);
                    }
                }
            }

            #endregion lastColumn check


            #region firstRow check
            int firstRow = rng.Start.Row;
            if (firstRow > lastFixedRow && firstRow < firstVisibleScrollableRow)
            {
                partType &= ~(BorderPartType.TopBorder);
            }
            else if (firstRow == firstVisibleScrollableRow)
            {
                if (IsRowHiddenUnderFixedRows(firstRow, true))
                {
                    partType &= ~(BorderPartType.TopBorder);
                }
            }

            #endregion firstRow check

            #region lastRow check
            int lastRow = rng.End.Row;
            if (lastRow != firstRow && lastRow > lastFixedRow)
            {
                if (lastRow < firstVisibleScrollableRow)
                {
                    partType &= ~(BorderPartType.BottomBorder | BorderPartType.DragRectangle);
                }
                else if (lastRow == firstVisibleScrollableRow)
                {
                    if (IsRowHiddenUnderFixedRows(lastRow, false))
                    {
                        partType &= ~(BorderPartType.BottomBorder | BorderPartType.DragRectangle);
                    }
                }
            }
            #endregion lastRow check

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

            if (mousePoint.X > scrollRect.Right)
                CustomScrollLineRight();

            if (mousePoint.Y > scrollRect.Bottom)
                CustomScrollLineDown();

            if (mousePoint.X < scrollRect.Left)
                m_Grid.CustomScrollLineLeft();

            if (mousePoint.Y < scrollRect.Top)
                m_Grid.CustomScrollLineUp();
        }
        #endregion IScrollingStyle members

        #region Private implementation

        private bool IsColumnHiddenUnderFixedColumn(int fixedRight, int left, int right, int column, bool includePartial)
        {
            int actualFixedColumns = m_Grid.ActualFixedColumns;

            if (column < actualFixedColumns)
            {
                return false;
            }


            if (fixedRight > left)
            {
                if (includePartial || fixedRight > right)
                    return true;
            }
            return false;
        }

        private bool IsRowHiddenUnderFixedRows(int fixedBottom, int top, int bottom, int rowIndex, bool includePartial)
        {
            int actualFixedRows = m_Grid.ActualFixedRows;

            if (rowIndex < actualFixedRows)
            {
                return false;
            }

            if (fixedBottom > top)
            {
                if (includePartial || fixedBottom > bottom)
                    return true;
            }
            return false;
        }

        private void DrawFixedLeft(PaintEventArgs e, Point scrollableArea)
        {
            Rectangle clipRectangle = new Rectangle(0, scrollableArea.Y,
                    scrollableArea.X, m_Grid.DisplayRectangle.Height - scrollableArea.Y);
            
            Range fixedLeft = m_Grid.RangeAtArea(CellPositionType.FixedLeft);

            if (fixedLeft.IsEmpty())
            {
                m_FirstVisibleScrollableRow = -1;
                return;
            }

            m_FirstVisibleScrollableRow = FindFirstVisibleRowFromRange(fixedLeft, out m_FirstVisibleScrollableRowTop);

            if (m_FirstVisibleScrollableRow == -1)
                return;

            const int firstCol = 0;
            int lastrow = FindLastVisibleRow(m_FirstVisibleScrollableRow, clipRectangle.Bottom);

            int lastColumn = m_Grid.ActualFixedColumns - 1;

            fixedLeft = new Range(m_FirstVisibleScrollableRow, firstCol, lastrow, lastColumn);

            if (fixedLeft.IsEmpty())
            {
                m_FirstVisibleScrollableRow = -1;
                return;
            }

            e.Graphics.SetClip(clipRectangle);

            e.Graphics.TranslateTransform(e.Graphics.ClipBounds.X, e.Graphics.ClipBounds.Y - (scrollableArea.Y - m_FirstVisibleScrollableRowTop),
                MatrixOrder.Append);

            Rectangle clientRectangle = Rectangle.Round(e.Graphics.ClipBounds);

            using (GraphicsCache grCache = new GraphicsCache(e.Graphics, clientRectangle))
            {
                m_Grid.OnRangePaint(new RangePaintEventArgs(m_Grid, grCache, fixedLeft),
                    new Rectangle(0, 0, clipRectangle.Width, clipRectangle.Height));
            }

            e.Graphics.ResetClip();
            e.Graphics.ResetTransform();
        }

        private void DrawFixedTop(PaintEventArgs e, Point scrollableArea)
        {
            Rectangle clipRectangle = new Rectangle(scrollableArea.X, 0,
                    m_Grid.DisplayRectangle.Width - scrollableArea.X, scrollableArea.Y);

            Range fixedTop = m_Grid.RangeAtArea(CellPositionType.FixedTop);

            if (fixedTop.IsEmpty())
            {
                m_FirstVisibleScrollableColumn = -1;
                return;
            }

            m_FirstVisibleScrollableColumn = FindFirstVisibleColumnFromRange(fixedTop, out m_FirstVisibleScrollableColumnLeft);

            if (m_FirstVisibleScrollableColumn == -1)
                return;

            const int firstRow = 0;

            int lastrow = m_Grid.ActualFixedRows - 1;

            int lastColumn = FindLastVisibleColumn(m_FirstVisibleScrollableColumn, clipRectangle.Right);

            fixedTop = new Range(firstRow, m_FirstVisibleScrollableColumn, lastrow, lastColumn);

            if (fixedTop.IsEmpty())
            {
                m_FirstVisibleScrollableColumn = -1;
                return;
            }

            e.Graphics.SetClip(clipRectangle);

            e.Graphics.TranslateTransform(e.Graphics.ClipBounds.X - (scrollableArea.X - m_FirstVisibleScrollableColumnLeft), e.Graphics.ClipBounds.Y,
                MatrixOrder.Append);

            Rectangle clientRectangle = Rectangle.Round(e.Graphics.ClipBounds);

            using (GraphicsCache grCache = new GraphicsCache(e.Graphics, clientRectangle))
            {
                m_Grid.OnRangePaint(new RangePaintEventArgs(m_Grid, grCache, fixedTop),
                    new Rectangle(0, 0, clipRectangle.Width, clipRectangle.Height));
            }
            e.Graphics.ResetClip();
            e.Graphics.ResetTransform();
        }

        private void DrawFixedTopLeft(PaintEventArgs e)
        {
            Range fixedTopLeft = m_Grid.RangeAtArea(CellPositionType.FixedTopLeft);

            if (!fixedTopLeft.IsEmpty())
            {
                Rectangle clientRectangle = Rectangle.Round(e.Graphics.ClipBounds);

                using (GraphicsCache grCache = new GraphicsCache(e.Graphics, clientRectangle))
                {
                    Rectangle drawRectangle = m_Grid.RangeToRectangle(fixedTopLeft);
                    m_Grid.OnRangePaint(new RangePaintEventArgs(m_Grid, grCache, fixedTopLeft),
                        drawRectangle);
                }
            }
        }
        private void DrawScrollableArea(PaintEventArgs e, Point scrollableArea)
        {
            Rectangle clipRectangle = new Rectangle(scrollableArea.X, scrollableArea.Y,
                m_Grid.DisplayRectangle.Width - scrollableArea.X, m_Grid.DisplayRectangle.Height - scrollableArea.Y);

            Range range = GetScrollableRange();

            if(range.IsEmpty())
                return;

            if (m_FirstVisibleScrollableRow == -1)
                m_FirstVisibleScrollableRow = FindFirstVisibleRowFromRange(range, out m_FirstVisibleScrollableRowTop);
            if (m_FirstVisibleScrollableColumn == -1)
                m_FirstVisibleScrollableColumn = FindFirstVisibleColumnFromRange(range, out m_FirstVisibleScrollableColumnLeft);

            int lastrow = FindLastVisibleRow(m_FirstVisibleScrollableRow, clipRectangle.Bottom);

            int lastColumn = FindLastVisibleColumn(m_FirstVisibleScrollableColumn, clipRectangle.Right);

            range = new Range(m_FirstVisibleScrollableRow, m_FirstVisibleScrollableColumn, lastrow, lastColumn);

            if (range.IsEmpty()) 
                return;

            e.Graphics.SetClip(clipRectangle);

            e.Graphics.TranslateTransform(e.Graphics.ClipBounds.X - (scrollableArea.X - m_FirstVisibleScrollableColumnLeft),
                e.Graphics.ClipBounds.Y - (scrollableArea.Y - m_FirstVisibleScrollableRowTop), MatrixOrder.Append);

            Rectangle clientRectangle = Rectangle.Round(e.Graphics.ClipBounds);


            using (GraphicsCache grCache = new GraphicsCache(e.Graphics, clientRectangle))
            {
                m_Grid.OnRangePaint(new RangePaintEventArgs(m_Grid, grCache, range),
                    new Rectangle(0, 0, clipRectangle.Width, clipRectangle.Height));
            }

            e.Graphics.ResetClip();
            e.Graphics.ResetTransform();
        }

        private int FindLastVisibleRow(int firstVisibleRow, int bottom)
        {
            if (firstVisibleRow == -1)
                return -1;

            int y = m_Grid.Rows.GetTop(firstVisibleRow);
            int row = firstVisibleRow;
            for (; row < m_Grid.Rows.Count; row++)
            {
                y += m_Grid.Rows.GetHeight(row);

                if (y >= bottom)
                    return row;
            }

            return row - 1;
        }

        private int FindLastVisibleColumn(int firstVisibleCol, int right)
        {
            if (firstVisibleCol == -1)
                return -1;

            int x = m_Grid.Columns.GetLeft(firstVisibleCol);
            int col = firstVisibleCol;
            for (; col < m_Grid.Columns.Count; col++)
            {
                x += m_Grid.Columns.GetWidth(col);

                if (x >= right)
                    return col;
            }

            return col - 1;
        }

        private int GetLeft(int startColumn, int startColumnAbsoluteLeft, int column)
        {
            int absoluteleft = startColumnAbsoluteLeft;

            int colCount = m_Grid.Columns.Count;

            for (int i = startColumn; i < colCount; i++)
            {
                if (i == column)
                    break;

                absoluteleft += m_Grid.Columns.GetWidth(i);
            }

            return column < m_Grid.ActualFixedColumns ? absoluteleft : absoluteleft - m_Grid.CustomScrollPosition.X;
        }


        /// <summary>
        ///sandhra.prakash@siemens.com: To find the first visible column from range.
        /// partially visible ones are also treated as visible hence it will be returned
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private int FindFirstVisibleRowFromRange(Range range)
        {
            int top;
            return FindFirstVisibleRowFromRange(range, out top);
        }

        ///  <summary>
        /// sandhra.prakash@siemens.com: To find the first visible Row from range.
        ///  partially visible ones are also treated as visible hence it will be returned
        ///  </summary>
        ///  <param name="range"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private int FindFirstVisibleRowFromRange(Range range, out int top)
        {

            top = GetTop(0, 0, range.Start.Row);

            if (range.Start.Row <= m_Grid.ActualFixedRows - 1)
                return range.Start.Row;

            int fixedBottom = m_Grid.GetFixedAreaHeight();

            for (int row = range.Start.Row; row <= range.End.Row; row++)
            {
                int bottom = top + m_Grid.Rows.GetHeight(row);

                if (!IsRowHiddenUnderFixedRows(fixedBottom, top, bottom, row, false) &&
                    row < m_Grid.Rows.Count)
                {
                    return row;
                }
                top = bottom;
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
            int left;
            return FindFirstVisibleColumnFromRange(range, out left);
        }

        ///  <summary>
        /// sandhra.prakash@siemens.com: To find the first visible column from range.
        ///  partially visible ones are also treated as visible hence it will be returned
        ///  </summary>
        ///  <param name="range"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        private int FindFirstVisibleColumnFromRange(Range range, out int left)
        {
            left = GetLeft(0, 0, range.Start.Column);
            if (range.Start.Column <= m_Grid.ActualFixedColumns - 1)
                return range.Start.Column;

            int fixedRight = m_Grid.GetFixedAreaWidth();

            for (int col = range.Start.Column; col <= range.End.Column; col++)
            {
                int right = left + m_Grid.Columns.GetWidth(col);
                if (!IsColumnHiddenUnderFixedColumn(fixedRight, left, right, col, false) &&
                    col < m_Grid.Columns.Count)
                {
                    return col;
                }
                left = right;
            }
            return -1;
        }
        private int GetNonFixedAreaHeight()
        {
            int heightOfScrollableArea = 0;
            for (int index = m_Grid.ActualFixedRows; index < m_Grid.Rows.Count; index++)
            {
                //IsRowVisible checks if height is greater than 0, in that case we dont need that check
                heightOfScrollableArea += m_Grid.Rows.GetHeight(index);
            }
            return heightOfScrollableArea;
        }

        internal int GetNonFixedAreaWidth()
        {
            int widthOfScrollableArea = 0;
            for (int index = m_Grid.ActualFixedColumns; index < m_Grid.Columns.Count; index++)
            {
                //IsColumnVisible checks if height is greater than 0, in that case we dont need that check
                widthOfScrollableArea += m_Grid.Columns.GetWidth(index);
            }
            return widthOfScrollableArea;
        }

        #endregion Private implementation


    }
}
