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
 * 1.StretchToFit(): visible index code check commented because
                If there is horizontal scrollbar it will never take care of the Columns that are not visible right now on screen.
 * 2. ColumnsInsideRegion, FirstVisibleScrollableColumn, LastVisibleScrollableColumn, GetLeft - all these are dependent on scroll bar values, hence move to corresponding scrollstyle
 * 3. StretchToFit : When smoothscrolling is enabled, and horizontal scrollbar value is max, and stretch is performed. 
                        it doesnot really fit appropriately(reduce the width of already stretched column).
 * 4. StretchToFit :when there are more than 1 column with EnableStretch, there is a chance that, we get decimal numbers on division.
                            The precision part was discarded. When there are a lot of columns with enablestretch (as in FASE), this will amount to a visible number.
                            These extra precisions are added together and added with the width of the last column so that there is no unnecessary space is left.
*/
#endregion Copyright

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace SourceGrid
{
	/// <summary>
	/// Abstract base class for manage columns informations.
	/// </summary>
	public abstract class ColumnsBase
	{
		private GridVirtual mGrid;
		
		public ColumnsBase(GridVirtual grid)
		{
			mGrid = grid;
		}
		
		public GridVirtual Grid
		{
			get{return mGrid;}
		}
		
		#region Abstract methods
		public abstract int Count
		{
			get;
		}
		
		/// <summary>
		/// Gets the width of the specified column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public abstract int GetWidth(int column);
		/// <summary>
		/// Sets the width of the specified column.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="width"></param>
		public abstract void SetWidth(int column, int width);
		
		public abstract AutoSizeMode GetAutoSizeMode(int column);
		#endregion
		
		/// <summary>
		/// Autosize column using default auto size mode
		/// </summary>
		/// <param name="column"></param>
		public void AutoSizeColumn(int column)
		{
			AutoSizeColumn(column, true);
		}
		
		/// <summary>
		/// Autosize column using default auto size mode
		/// </summary>
		public void AutoSizeColumn(int column, bool useRowHeight)
		{
			int minRow = 0;
			int maxRow = Grid.Rows.Count - 1;
			
			if ((GetAutoSizeMode(column) & AutoSizeMode.EnableAutoSizeView) == AutoSizeMode.EnableAutoSizeView)
			{
				bool isColumnVisible = this.Grid.GetVisibleColumns(true).Contains(column);
				if (isColumnVisible == false)
					return;
				List<int> visibleRows = Grid.GetVisibleRows(true);
				visibleRows.Sort();
				if (visibleRows.Count == 0)
					return;
				minRow = visibleRows[0];
				maxRow = visibleRows[visibleRows.Count - 1];
			}
			AutoSizeColumn(column, useRowHeight, minRow, maxRow);
		}
		public void AutoSizeColumn(int column, bool useRowHeight, int StartRow, int EndRow)
		{
			if ((GetAutoSizeMode(column) & AutoSizeMode.EnableAutoSize) == AutoSizeMode.EnableAutoSize &&
			    IsColumnVisible(column) )
				SetWidth(column, MeasureColumnWidth(column, useRowHeight, StartRow, EndRow) );
		}
		/// <summary>
		/// Measures the current column when drawn with the specified cells.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="useRowHeight">True to fix the row height when measure the column width.</param>
		/// <param name="StartRow">Start row to measure</param>
		/// <param name="EndRow">End row to measure</param>
		/// <returns>Returns the required width</returns>
		public int MeasureColumnWidth(int column, bool useRowHeight, int StartRow, int EndRow)
		{
			int min = Grid.MinimumWidth;
			
			if ((GetAutoSizeMode(column) & AutoSizeMode.MinimumSize) == AutoSizeMode.MinimumSize)
				return min;
			
			for (int r = StartRow; r <= EndRow; r++)
			{
				Cells.ICellVirtual cell = Grid.GetCell(r, column);
				if (cell != null)
				{
					Position cellPosition = new Position(r, column);
					
					Size maxLayout = Size.Empty;
					//Use the width of the actual cell (considering spanned cells)
					if (useRowHeight)
						maxLayout.Height = Grid.RangeToSize(Grid.PositionToCellRange(cellPosition)).Height;
					
					CellContext cellContext = new CellContext(Grid, cellPosition, cell);
					Size cellSize = cellContext.Measure(maxLayout);
					if (cellSize.Width > min)
						min = cellSize.Width;
				}
			}
			return min;
		}
		
		public void AutoSize(bool useRowHeight)
		{
			SuspendLayout();
			for (int i = 0; i < Count; i++)
			{
				AutoSizeColumn(i, useRowHeight);
			}
			ResumeLayout();
		}
		
		/// <summary>
		/// Auto size all the columns with the max required width of all cells.
		/// </summary>
		/// <param name="useRowHeight">True to fix the row height when measure the column width.</param>
		/// <param name="StartRow">Start row to measure</param>
		/// <param name="EndRow">End row to measure</param>
		public void AutoSize(bool useRowHeight, int StartRow, int EndRow)
		{
			SuspendLayout();
			for (int i = 0; i < Count; i++)
			{
				AutoSizeColumn(i, useRowHeight, StartRow, EndRow);
			}
			ResumeLayout();
		}
		
		/// <summary>
		/// stretch the columns width to always fit the available space when the contents of the cell is smaller.
		/// </summary>
        public virtual void StretchToFit()
		{
			SuspendLayout();
			
			Rectangle displayRect = Grid.DisplayRectangle;
			
			if (Count > 0 && displayRect.Width > 0)
			{
                //sandhra.prakash@siemens.com: visible index code check commented because
                //If there is horizontal scrollbar it will never take care of the Columns that are not visible right now on screen.
				//List<int> visibleIndex = ColumnsInsideRegion(displayRect.X, displayRect.Width);
				
				//Continue only if the columns are all visible, otherwise this method cannot shirnk the columns
				//if (visibleIndex.Count >= Count)
				{
                    //sandhra.prakash@siemens.com: When smoothscrolling is enabled, and horizontal scrollbar value is max, and stretch is performed. 
                    //it doesnot really fit appropriately(reduce the width of already stretched column).
                    int? current = Grid.EnableSmoothScrolling ? GetAbsoluteRight(Count - 1) : GetRight(Count - 1);
					if (current != null && displayRect.Width > current.Value)
					{
						//Calculate the columns to stretch
						int countToStretch = 0;
						for (int i = 0; i < Count; i++)
						{
							if ((GetAutoSizeMode(i) & AutoSizeMode.EnableStretch) == AutoSizeMode.EnableStretch &&
							    IsColumnVisible(i) )
								countToStretch++;
						}
						
						if (countToStretch > 0)
						{
                            /*sandhra.prakash@siemens.com: when there are more than 1 column with EnableStretch, there is a chance that, we get decimal numbers on division.
                            //The precision part was discarded. When there are a lot of columns with enablestretch (as in FASE), this will amount to a visible number.
                             * These extra precisions are added together and added with the width of the last column so that there is no unnecessary space is left.
                             */
                            float deltaPerColumn = ((float)(displayRect.Width - current.Value)) / countToStretch;
                            float totalWastage = ((deltaPerColumn - (int)deltaPerColumn) * countToStretch);
						    bool lastStretchedColumn = true;
                            for (int i = Count -1; i >= 0; i--)
							{
							    if ((GetAutoSizeMode(i) & AutoSizeMode.EnableStretch) == AutoSizeMode.EnableStretch &&
							        IsColumnVisible(i))
							    {

                                    int widthOfColumn = GetWidth(i) + (int)deltaPerColumn + (lastStretchedColumn ? (int)totalWastage : 0);
                                    SetWidth(i, widthOfColumn);
							        lastStretchedColumn = false;

							    }
							}
						}
					}
				}
			}
			
			ResumeLayout();
		}
		
		/// <summary>
		/// Gets the columns index inside the specified display area.
		/// </summary>
		/// <returns></returns>
		public List<int> ColumnsInsideRegion(int x, int width)
		{
			return ColumnsInsideRegion(x, width, true, true);
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
            return Grid.ScrollingStyle.ColumnsInsideRegion(x, width, returnsPartial, returnsFixedColumns);
        }

		/// <summary>
		/// Calculate the Column that have the Left value smaller or equal than the point p_X, or -1 if not found found.
		/// </summary>
		/// <param name="x">X Coordinate to search for a column</param>
		/// <returns></returns>
		public int? ColumnAtPoint(int x)
		{
			List<int> list = ColumnsInsideRegion(x, 1);
			if (list.Count == 0)
				return null;
			else
				return list[0];
		}
		
		/// <summary>
		/// Returns the first visible scrollable column.
		/// Return null if there isn't a visible column.
		/// </summary>
		/// <returns></returns>
		public int? FirstVisibleScrollableColumn
		{
			get { return Grid.ScrollingStyle.FirstVisibleScrollableColumn; }
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
                return Grid.ScrollingStyle.LastVisibleScrollableColumn;
			}
		}
		
		public Range GetRange(int column)
		{
			return new Range(0, column, Grid.Rows.Count-1, column);
		}
		
		#region Layout
		private int mSuspendedCount = 0;
		public void SuspendLayout()
		{
			mSuspendedCount++;
		}
		public void ResumeLayout()
		{
			if (mSuspendedCount > 0)
				mSuspendedCount--;
			
			PerformLayout();
		}
		public void PerformLayout()
		{
			if (mSuspendedCount == 0)
				OnLayout();
		}
		protected virtual void OnLayout()
		{
			Grid.OnCellsAreaChanged();
		}
		#endregion
		
		/// <summary>
		/// Fired when the numbes of columns changed.
		/// </summary>
		public void ColumnsChanged()
		{
			PerformLayout();
		}
		
		#region Left/Right
		public int GetAbsoluteLeft(int column)
		{
			if (column < 0)
				throw new ArgumentException("Must be a valid index");
			
			int left = 0;
			
			int index = 0;
			while (index < column)
			{
				left += GetWidth(index);
				
				index++;
			}
			
			return left;
		}
		public int GetAbsoluteRight(int column)
		{
			int left = GetAbsoluteLeft(column);
			return left + GetWidth(column);
		}

        public int GetLeft(int column)
        {
            return Grid.ScrollingStyle.GetLeft(column);
        }


		/// <summary>
		/// Gets the column right position. GetLeft + GetWidth.
		/// </summary>
		public int GetRight(int column)
		{
			int left = GetLeft(column);
			return left + GetWidth(column);
		}
		#endregion
		
		/// <summary>
		/// Show a column (set the width to default width)
		/// </summary>
		/// <param name="column"></param>
		public abstract void ShowColumn(int column);

		/// <summary>
		/// Hide the specified column (set the width to 0)
		/// </summary>
		/// <param name="column"></param>
		public abstract void HideColumn(int column);

		/// <summary>
		/// Returns true if the specified column is visible
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public abstract bool IsColumnVisible(int column);
	}
}
