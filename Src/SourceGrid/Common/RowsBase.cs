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
 * 1. RowsInsideRegion, FirstVisibleScrollableRow, LastVisibleScrollableRow, GetTop: Modified for SmoothScrolling requirement. Thus had to make it independent of scrollbar,
 * original implementation is there in CellByCellScrolling.cs
*/
#endregion Copyright
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SourceGrid
{
	public delegate void RowVisibilityChangedHandler(int rowIndex, bool becameVisible);
	
	/// <summary>
	/// Abstract base class for manage rows informations.
	/// </summary>
	public abstract class RowsBase : IRows
	{
		private GridVirtual mGrid;
		protected IHiddenRowCoordinator m_HiddenRowsCoordinator = null;
		public event RowVisibilityChangedHandler RowVisibilityChanged;
		
		
		
		protected virtual void OnRowVisibilityChanged(int rowIndex, bool becameVisible)
		{
			if (RowVisibilityChanged != null) {
				RowVisibilityChanged(rowIndex, becameVisible);
			}
		}
		
		public IHiddenRowCoordinator HiddenRowsCoordinator {
			get { return m_HiddenRowsCoordinator; }
		}
		
		public RowsBase(GridVirtual grid)
		{
			mGrid = grid;
			
			m_HiddenRowsCoordinator = new StandardHiddenRowCoordinator(this);
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
		/// Gets the height of the specified row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public abstract int GetHeight(int row);
		/// <summary>
		/// Sets the height of the specified row.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="height"></param>
		public abstract void SetHeight(int row, int height);
		
		public abstract AutoSizeMode GetAutoSizeMode(int row);
		#endregion
		
		/// <summary>
		/// Gets the rows index inside the specified display area.
		/// </summary>
		public List<int> RowsInsideRegion(int y, int height)
		{
			return RowsInsideRegion(y, height, true, true);
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
            return Grid.ScrollingStyle.RowsInsideRegion(y, height, returnsPartial, returnsFixedRows);
        }

        /// <summary>
		/// Calculate the Row that have the Top value smaller or equal than the point p_Y, or -1 if not found found.
		/// </summary>
		/// <param name="y">Y Coordinate to search for a row</param>
		/// <returns></returns>
		public int? RowAtPoint(int y)
		{
			List<int> list = RowsInsideRegion(y, 1);
			if (list.Count == 0)
				return null;
			else
				return list[0];
		}
		
		/// <summary>
		/// Returns the first visible scrollable row.
		/// Return null if there isn't a visible row.
		/// </summary>
		/// <returns></returns>
		public int? FirstVisibleScrollableRow
		{
			get
			{
				return Grid.ScrollingStyle.FirstVisibleScrollableRow;
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
                return Grid.ScrollingStyle.LastVisibleScrollableRow;
			}
		}
		
		
		public void AutoSizeRow(int row)
		{
			int minColumn = 0;
			int maxColumn = Grid.Columns.Count - 1;
			
			if ((GetAutoSizeMode(row) & AutoSizeMode.EnableAutoSizeView) == AutoSizeMode.EnableAutoSizeView)
			{
				bool isRowVisible = this.Grid.GetVisibleRows(true).Contains(row);
				if (isRowVisible == false)
					return;
				List<int> visibleColumns = Grid.GetVisibleColumns(true);
				visibleColumns.Sort();
				if (visibleColumns.Count == 0)
					return;
				minColumn = visibleColumns[0];
				maxColumn = visibleColumns[visibleColumns.Count - 1];
			}
			AutoSizeRow(row, true, minColumn, maxColumn);
		}
		public void AutoSizeRow(int row, bool useColumnWidth, int StartCol, int EndCol)
		{
			if (( GetAutoSizeMode(row) & AutoSizeMode.EnableAutoSize) == AutoSizeMode.EnableAutoSize &&
			    IsRowVisible(row) )
				SetHeight(row, MeasureRowHeight(row, useColumnWidth, StartCol, EndCol) );
		}
		
		/// <summary>
		/// Measures the current row when drawn with the specified cells.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="useColumnWidth">True to fix the column width when calculating the required height of the row.</param>
		/// <param name="StartCol">Start column to measure</param>
		/// <param name="EndCol">End column to measure</param>
		/// <returns>Returns the required height</returns>
		public int MeasureRowHeight(int row, bool useColumnWidth, int StartCol, int EndCol)
		{
			int min = Grid.MinimumHeight;
			
			if ((GetAutoSizeMode(row) & AutoSizeMode.MinimumSize) == AutoSizeMode.MinimumSize)
				return min;
			
			for (int c = StartCol; c <= EndCol; c++)
			{
				Cells.ICellVirtual cell = Grid.GetCell(row, c);
				if (cell != null)
				{
					Position cellPosition = new Position(row, c);
					
					Size maxLayout = Size.Empty;
					//Use the width of the actual cell (considering spanned cells)
					if (useColumnWidth)
						maxLayout.Width = Grid.RangeToSize(Grid.PositionToCellRange(cellPosition)).Width;
					
					CellContext cellContext = new CellContext(Grid, cellPosition, cell);
					Size cellSize = cellContext.Measure(maxLayout);
					if (cellSize.Height > min)
						min = cellSize.Height;
				}
			}
			return min;
		}
		
		
		public void AutoSize(bool useColumnWidth)
		{
			AutoSize(useColumnWidth, 0, Grid.Columns.Count - 1);
		}
		
		/// <summary>
		/// Auto size all the rows with the max required height of all cells.
		/// </summary>
		/// <param name="useColumnWidth">True to fix the column width when calculating the required height of the row.</param>
		/// <param name="StartCol">Start column to measure</param>
		/// <param name="EndCol">End column to measure</param>
		public void AutoSize(bool useColumnWidth, int StartCol, int EndCol)
		{
			SuspendLayout();
			for (int i = 0; i < Count; i++)
				AutoSizeRow(i, useColumnWidth, StartCol, EndCol);
			ResumeLayout();
		}
		
		/// <summary>
		/// stretch the rows height to always fit the available space when the contents of the cell is smaller.
		/// </summary>
		public virtual void StretchToFit()
		{
			SuspendLayout();
			
			Rectangle displayRect = Grid.DisplayRectangle;
			
			if (Count > 0 && displayRect.Height > 0)
			{
				List<int> visibleIndex = RowsInsideRegion(displayRect.Y, displayRect.Height);
				
				//Continue only if the rows are all visible, otherwise this method cannot shirnk the rows
				if (visibleIndex.Count >= Count)
				{
					int? current = GetBottom(Count - 1);
					if (current != null && displayRect.Height > current.Value)
					{
						//Calculate the columns to stretch
						int countToStretch = 0;
						for (int i = 0; i < Count; i++)
						{
							if ((GetAutoSizeMode(i) & AutoSizeMode.EnableStretch) == AutoSizeMode.EnableStretch &&
							    IsRowVisible(i) )
								countToStretch++;
						}
						
						if (countToStretch > 0)
						{
							int deltaPerRow = (displayRect.Height - current.Value) / countToStretch;
							for (int i = 0; i < Count; i++)
							{
								if ((GetAutoSizeMode(i) & AutoSizeMode.EnableStretch) == AutoSizeMode.EnableStretch &&
								    IsRowVisible(i) )
									SetHeight(i, GetHeight(i) + deltaPerRow);
							}
						}
					}
				}
			}
			
			ResumeLayout();
		}
		
		public Range GetRange(int row)
		{
			return new Range(row, 0, row, Grid.Columns.Count-1);
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
		/// Fired when the numbers of rows changed.
		/// </summary>
		public void RowsChanged()
		{
			PerformLayout();
		}
		
		#region Top/Bottom
		public int GetAbsoluteTop(int row)
		{
			if (row < 0)
				throw new ArgumentException("Must be a valid index");
			
			int top = 0;
			
			int index = 0;
			while (index < row)
			{
				top += GetHeight(index);
				
				index++;
			}
			
			return top;
		}
		public int GetAbsoluteBottom(int row)
		{
			int top = GetAbsoluteTop(row);
			return top + GetHeight(row);
		}

        internal int GetTopPositive(int relativeRow, int row)
		{
			int top = 0;
			for (int i = relativeRow; i < row; i++)
			{
				/*if (i == row)
						return top;*/
				
				top += GetHeight(i);
			}
			return top;
		}
		
		internal int GetTopNegative(int relativeRow, int row)
		{
			int top = 0;
			for (int i = relativeRow - 1; i >= row; i--)
			{
				top -= GetHeight(i);
				
				/*if (i == row)
						return top;*/
			}
			return top;
		}
        
        public int GetTop(int row)
        {
           return Grid.ScrollingStyle.GetTop(row);
        }
	
        /// <summary>
		/// Gets the row bottom position. GetTop + GetHeight.
		/// </summary>
		public int GetBottom(int row)
		{
			int top = GetTop(row);
			return top + GetHeight(row);
		}
		#endregion
		
		/// <summary>
		/// Show a row (set the height to default height)
		/// </summary>
		/// <param name="row"></param>
		public void ShowRow(int row)
		{
			ShowRow(row, true);
		}
		
		/// <summary>
		/// Makes row visible or hidden.
		/// Fires OnRowVisibilityChanged event only if row visibility is changed
		/// </summary>
		/// <param name="row"></param>
		/// <param name="isVisible"></param>
		public void ShowRow(int row, bool isVisible)
		{
			if (isVisible == true && IsRowVisible(row) == false)
			{
				SetHeight(row, Grid.DefaultHeight);
				OnRowVisibilityChanged(row, isVisible);
			}
			else 
				if (isVisible == false && IsRowVisible(row))
			{
				SetHeight(row, 0);
				OnRowVisibilityChanged(row, isVisible);
			}
			
		}
		
		
		/// <summary>
		/// Hide the specified row (set the height to 0)
		/// </summary>
		/// <param name="row"></param>
		public void HideRow(int row)
		{
			ShowRow(row, false);
		}
		
		/// <summary>
		/// Returns true if the specified row is visible
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public bool IsRowVisible(int row)
		{
			return GetHeight(row) > 0;
		}
	}
}
