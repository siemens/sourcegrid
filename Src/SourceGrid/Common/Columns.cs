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

		public void AutoSizeColumn(int column)
		{
			AutoSizeColumn(column, true, 0, Grid.Rows.Count - 1);
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
			AutoSize(useRowHeight, 0, Grid.Rows.Count - 1);
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
                List<int> visibleIndex = ColumnsInsideRegion(displayRect.X, displayRect.Width);

                //Continue only if the columns are all visible, otherwise this method cannot shirnk the columns
                if (visibleIndex.Count >= Count)
                {
                    int? current = GetRight(Count - 1);
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
                            int deltaPerColumn = (displayRect.Width - current.Value) / countToStretch;
                            for (int i = 0; i < Count; i++)
                            {
                                if ((GetAutoSizeMode(i) & AutoSizeMode.EnableStretch) == AutoSizeMode.EnableStretch &&
                                    IsColumnVisible(i) )
                                    SetWidth(i, GetWidth(i) + deltaPerColumn);
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
        /// <param name="relativeCol"></param>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public List<int> ColumnsInsideRegion(int x, int width)
        {
            return ColumnsInsideRegion(x, width, true);
        }

        /// <summary>
        /// Gets the columns index inside the specified display area.
        /// The list returned is ordered by the index.
        /// Note that this method returns also invisible rows.
        /// </summary>
        /// <param name="returnsPartial">True to returns also partial columns</param>
        /// <returns></returns>
        public List<int> ColumnsInsideRegion(int x, int width, bool returnsPartial)
        {
            int right = x + width;

            List<int> list = new List<int>();

            //Add the fixed columns
            // Loop until the currentHeight is smaller then the requested displayRect
            for (int fr = 0; fr < Grid.FixedColumns && fr < Count; fr++)
            {
                int leftDisplay = GetLeft(fr);
                int rightDisplay = leftDisplay + GetWidth(fr);

                //If the column is inside the view
                if (right >= leftDisplay && x <= rightDisplay &&
                    (returnsPartial || (rightDisplay <= right && leftDisplay >= x)))
                {
                    list.Add(fr);
                }

                if (rightDisplay > right)
                    break;
            }

            int? relativeCol = FirstVisibleScrollableColumn;

            if (relativeCol != null)
            {
                //Add the standard columns
                for (int r = relativeCol.Value; r < Count; r++)
                {
                    int leftDisplay = GetLeft(r);
                    int rightDisplay = leftDisplay + GetWidth(r);

                    //If the column is inside the view
                    if (right >= leftDisplay && x <= rightDisplay &&
                        (returnsPartial || (rightDisplay <= right && leftDisplay >= x)) )
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
        /// Calculate the Column that have the Left value smaller or equal than the point p_X, or -1 if not found found.
        /// </summary>
        /// <param name="relativeCol"></param>
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
            get
            {
                int firstVisible = Grid.CustomScrollPosition.X + Grid.FixedColumns;

                if (firstVisible >= Count)
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

                Rectangle scrollableArea = Grid.GetScrollableArea();

                int right = GetLeft(first.Value);
                int c = first.Value;
                for (; c < Count; c++)
                {
                    right += GetWidth(c);

                    if (right >= scrollableArea.Right)
                        return c;
                }

                return c - 1;
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

        /// <summary>
        /// Gets the column left position.
        /// The Left is relative to the specified start position.
        /// Calculate the left using also the FixedColumn if present.
        /// </summary>
        public int GetLeft(int column)
        {
            int actualFixedColumns = Math.Min(Grid.FixedColumns, Count);

            int left = 0;

            //Calculate fixed left cells
            for (int i = 0; i < actualFixedColumns; i++)
            {
                if (i == column)
                    return left;

                left += GetWidth(i);
            }

            int? relativeColumn = FirstVisibleScrollableColumn;
            if (relativeColumn == null)
                relativeColumn = Count;

            if (relativeColumn == column)
                return left;
            else if (relativeColumn < column)
            {
                for (int i = relativeColumn.Value; i < Count; i++)
                {
                    if (i == column)
                        return left;

                    left += GetWidth(i);
                }
            }
            else if (relativeColumn > column)
            {
                for (int i = relativeColumn.Value - 1; i >= 0; i--)
                {
                    left -= GetWidth(i);

                    if (i == column)
                        return left;
                }
            }

            throw new IndexOutOfRangeException();
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
        public void ShowColumn(int column)
        {
            if (IsColumnVisible(column) == false)
                SetWidth(column, Grid.DefaultWidth);
        }
        /// <summary>
        /// Hide the specified column (set the width to 0)
        /// </summary>
        /// <param name="column"></param>
        public void HideColumn(int column)
        {
            SetWidth(column, 0);
        }
        /// <summary>
        /// Returns true if the specified column is visible
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool IsColumnVisible(int column)
        {
            return GetWidth(column) > 0;
        }
    }


	/// <summary>
	/// This class implements a RowsBase class using always the same Height for all rows. Using this class you must only implement the Count method.
	/// </summary>
	public abstract class ColumnsSimpleBase : ColumnsBase
	{
		public ColumnsSimpleBase(GridVirtual grid):base(grid)
		{
			mColumnWidth = grid.DefaultWidth;
		}

		private int mColumnWidth;
		public int ColumnWidth
		{
			get{return mColumnWidth;}
			set
			{
				if (mColumnWidth != value)
				{
					mColumnWidth = value;
					PerformLayout();
				}
			}
		}

		public override int GetWidth(int column)
		{
			return ColumnWidth;
		}
		public override void SetWidth(int column, int width)
		{
			ColumnWidth = width;
		}
	}

	/// <summary>
	/// Column Information
	/// </summary>
	public class ColumnInfo
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p_Grid"></param>
		public ColumnInfo(GridVirtual p_Grid)
		{
			m_Grid = p_Grid;
			m_Width = Grid.DefaultWidth;
		}

		private int m_Width;
		/// <summary>
		/// Width of the current Column
		/// </summary>
		public int Width
		{
			get{return m_Width;}
			set
			{
				if (value < 0)
					value=0;

				if (m_Width != value)
				{
					m_Width = value;
					((ColumnInfoCollection)Grid.Columns).OnColumnWidthChanged(new ColumnInfoEventArgs(this));
				}
			}
		}

		//private int m_Index;
		/// <summary>
		/// Index of the current Column
		/// </summary>
		public int Index
		{
			get{return ((ColumnInfoCollection)Grid.Columns).IndexOf(this);}
		}

		private GridVirtual m_Grid;
		/// <summary>
		/// Attached Grid
		/// </summary>
		[Browsable(false)]
		public GridVirtual Grid
		{
			get{return m_Grid;}
		}


		public Range Range
		{
			get
			{
				if (m_Grid == null)
					throw new SourceGridException("Invalid Grid object");

				return new Range(0, Index, Grid.Rows.Count - 1, Index);
			}
		}

		private object m_Tag;
		/// <summary>
		/// A property that the user can use to insert custom informations associated to a specific column
		/// </summary>
		[Browsable(false)]
		public object Tag
		{
			get{return m_Tag;}
			set{m_Tag = value;}
		}

		private AutoSizeMode m_AutoSizeMode = AutoSizeMode.Default;
		/// <summary>
		/// Flags for autosize and stretch
		/// </summary>
		public AutoSizeMode AutoSizeMode
		{
			get{return m_AutoSizeMode;}
			set{m_AutoSizeMode = value;}
		}

        /// <summary>
        /// Gets or sets if the column is visible.
        /// Internally set the width to 0 to hide a column.
        /// </summary>
        public bool Visible
        {
            get { return Width > 0; }
            set
            {
                if (value && Visible == false)
                    Width = Grid.DefaultWidth;
                else if (value == false && Visible)
                    Width = 0;
            }
        }
	}

	/// <summary>
	/// Collection of ColumnInfo
	/// </summary>
	public abstract class ColumnInfoCollection : ColumnsBase, IEnumerable<ColumnInfo>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="grid"></param>
		public ColumnInfoCollection(GridVirtual grid):base(grid)
		{
		}

		private List<ColumnInfo> m_List = new List<ColumnInfo>();

		/// <summary>
		/// Returns true if the range passed is valid
		/// </summary>
		/// <param name="p_StartIndex"></param>
		/// <param name="p_Count"></param>
		/// <returns></returns>
		public bool IsValidRange(int p_StartIndex, int p_Count)
		{
			if (p_StartIndex < Count && p_StartIndex >= 0 &&
				p_Count > 0 && (p_StartIndex+p_Count) <= Count)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns true if the range passed is valid for insert method
		/// </summary>
		/// <param name="p_StartIndex"></param>
		/// <param name="p_Count"></param>
		/// <returns></returns>
		public bool IsValidRangeForInsert(int p_StartIndex, int p_Count)
		{
			if (p_StartIndex <= Count && p_StartIndex >= 0 &&
				p_Count > 0)
				return true;
			else
				return false;
		}

		#region Insert/Remove Methods

		/// <summary>
		/// Insert the specified number of Columns at the specified position
		/// </summary>
		/// <param name="p_StartIndex"></param>
        /// <param name="columns"></param>
		protected void InsertRange(int p_StartIndex, ColumnInfo[] columns)
		{
			if (IsValidRangeForInsert(p_StartIndex, columns.Length)==false)
				throw new SourceGridException("Invalid index");

			for (int c = 0; c < columns.Length; c++)
			{
				m_List.Insert(p_StartIndex + c, columns[c]);
			}

			PerformLayout();

			OnColumnsAdded(new IndexRangeEventArgs(p_StartIndex, columns.Length));
		}

		/// <summary>
		/// Remove a column at the speicifed position
		/// </summary>
		/// <param name="p_Index"></param>
		public void Remove(int p_Index)
		{
			RemoveRange(p_Index, 1);
		}

		/// <summary>
		/// Remove the ColumnInfo at the specified positions
		/// </summary>
		/// <param name="p_StartIndex"></param>
		/// <param name="p_Count"></param>
		public void RemoveRange(int p_StartIndex, int p_Count)
		{
			if (IsValidRange(p_StartIndex, p_Count)==false)
				throw new SourceGridException("Invalid index");

			IndexRangeEventArgs eventArgs = new IndexRangeEventArgs(p_StartIndex, p_Count);
			OnColumnsRemoving(eventArgs);

			m_List.RemoveRange(p_StartIndex, p_Count);

			OnColumnsRemoved(eventArgs);

			PerformLayout();
		}


		#endregion

		/// <summary>
		/// Move a column from one position to another position
		/// </summary>
		/// <param name="p_CurrentColumnPosition"></param>
		/// <param name="p_NewColumnPosition"></param>
		public void Move(int p_CurrentColumnPosition, int p_NewColumnPosition)
		{
			if (p_CurrentColumnPosition == p_NewColumnPosition)
				return;

			if (p_CurrentColumnPosition < p_NewColumnPosition)
			{
				for (int r = p_CurrentColumnPosition; r < p_NewColumnPosition; r++)
				{
					Swap(r, r + 1);
				}				
			}
			else
			{
				for (int r = p_CurrentColumnPosition; r > p_NewColumnPosition; r--)
				{
					Swap(r, r - 1);
				}				
			}
		}

		/// <summary>
		/// Change the position of column 1 with column 2.
		/// </summary>
		/// <param name="p_ColumnIndex1"></param>
		/// <param name="p_ColumnIndex2"></param>
		public void Swap(int p_ColumnIndex1, int p_ColumnIndex2)
		{
			if (p_ColumnIndex1 == p_ColumnIndex2)
				return;

			ColumnInfo column1 = this[p_ColumnIndex1];
			ColumnInfo column2 = this[p_ColumnIndex2];

			m_List[p_ColumnIndex1] = column2;
			m_List[p_ColumnIndex2] = column1;

			PerformLayout();
		}

		/// <summary>
		/// Fired when the number of columns change
		/// </summary>
		public event IndexRangeEventHandler ColumnsAdded;

		/// <summary>
		/// Fired when the number of columns change
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnColumnsAdded(IndexRangeEventArgs e)
		{
			if (ColumnsAdded!=null)
				ColumnsAdded(this, e);

			ColumnsChanged();
		}

		/// <summary>
		/// Fired when some columns are removed
		/// </summary>
		public event IndexRangeEventHandler ColumnsRemoved;

		/// <summary>
		/// Fired when some columns are removed
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnColumnsRemoved(IndexRangeEventArgs e)
		{
			if (ColumnsRemoved!=null)
				ColumnsRemoved(this, e);

			ColumnsChanged();
		}

		/// <summary>
		/// Fired before some columns are removed
		/// </summary>
		public event IndexRangeEventHandler ColumnsRemoving;

		/// <summary>
		/// Fired before some columns are removed
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnColumnsRemoving(IndexRangeEventArgs e)
		{
			if (ColumnsRemoving!=null)
				ColumnsRemoving(this, e);

			//Grid.OnColumnsRemoving(e);
		}

		/// <summary>
		/// Indexer. Returns a ColumnInfo at the specified position
		/// </summary>
		public ColumnInfo this[int p]
		{
			get{return m_List[p];}
		}

		protected override void OnLayout()
		{
			base.OnLayout ();
		}

		/// <summary>
		/// Fired when the user change the Width property of one of the Column
		/// </summary>
		public event ColumnInfoEventHandler ColumnWidthChanged;

		/// <summary>
		/// Execute the RowHeightChanged event
		/// </summary>
		/// <param name="e"></param>
		public void OnColumnWidthChanged(ColumnInfoEventArgs e)
		{
			PerformLayout();

			if (ColumnWidthChanged!=null)
				ColumnWidthChanged(this, e);
		}


		public int IndexOf(ColumnInfo p_Info)
		{
			return m_List.IndexOf(p_Info);
		}

		/// <summary>
		/// Auto size the columns calculating the required size only on the rows currently visible
		/// </summary>
		public void AutoSizeView()
		{
            List<int> list = Grid.Rows.RowsInsideRegion(Grid.DisplayRectangle.Y, Grid.DisplayRectangle.Height);
            if (list.Count > 0)
            {
                AutoSize(false, list[0], list[list.Count - 1]);
            }
		}

		/// <summary>
		/// Remove all the columns
		/// </summary>
		public void Clear()
		{
			if (Count > 0)
				RemoveRange(0, Count);
		}

		#region ColumnsBase
        public override int GetWidth(int column)
		{
			return this[column].Width;
		}
		public override void SetWidth(int column, int width)
		{
			this[column].Width = width;
		}
        public override AutoSizeMode GetAutoSizeMode(int column)
        {
            return this[column].AutoSizeMode;
        }
		#endregion


		public override int Count
		{
			get{return m_List.Count;}
		}


        #region IEnumerable<ColumnInfo> Members

        public IEnumerator<ColumnInfo> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        #endregion
    }
}
