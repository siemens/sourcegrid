using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace SourceGrid
{
	/// <summary>
	/// Abstract base class for manage rows informations.
	/// </summary>
	public abstract class RowsBase
	{
		private GridVirtual mGrid;

		public RowsBase(GridVirtual grid)
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
            return RowsInsideRegion(y, height, true);
        }

        /// <summary>
        /// Gets the rows index inside the specified display area.
        /// The list returned is ordered by the index.
        /// Note that this method returns also invisible rows.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="returnsPartial">True to returns also partial rows</param>
        public List<int> RowsInsideRegion(int y, int height, bool returnsPartial)
        {
            int bottom = y + height;

            List<int> list = new List<int>();

            //Add the fixed rows
            // Loop until the currentHeight is smaller then the requested displayRect
            for (int fr = 0; fr < Grid.FixedRows && fr < Count; fr++)
            {
                int topDisplay = GetTop(fr);
                int bottomDisplay = topDisplay + GetHeight(fr);

                //If the row is inside the view
                if (bottom >= topDisplay && y <= bottomDisplay &&
                    (returnsPartial || (bottomDisplay <= bottom && topDisplay >= y)))
                {
                    list.Add(fr);
                }

                if (bottomDisplay > bottom)
                    break;
            }

            int? relativeRow = FirstVisibleScrollableRow;

            if (relativeRow != null)
            {
                //Add the standard rows
                for (int r = relativeRow.Value; r < Count; r++)
                {
                    int topDisplay = GetTop(r);
                    int bottomDisplay = topDisplay + GetHeight(r);

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
                int firstVisible = Grid.CustomScrollPosition.Y + Grid.FixedRows;

                if (firstVisible >= Count)
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

                Rectangle scrollableArea = Grid.GetScrollableArea();

                int bottom = GetTop(first.Value);
                int r = first.Value;
                for (; r < Count; r++)
                {
                    bottom += GetHeight(r);

                    if (bottom >= scrollableArea.Bottom)
                        return r;
                }

                return r - 1;
            }
        }


		public void AutoSizeRow(int row)
		{
			AutoSizeRow(row, true, 0, Grid.Columns.Count - 1);
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

        /// <summary>
        /// Gets the row top position.
        /// The Top is relative to the specified start position.
        /// Calculate the top using also the FixedRows if present.
        /// </summary>
        public int GetTop(int row)
        {
            int actualFixedRows = Math.Min(Grid.FixedRows, Count);

            int top = 0;

            //Calculate fixed top cells
            for (int i = 0; i < actualFixedRows; i++)
            {
                if (i == row)
                    return top;

                top += GetHeight(i);
            }

            int? relativeRow = FirstVisibleScrollableRow;
            if (relativeRow == null)
                relativeRow = Count;

            if (relativeRow == row)
                return top;
            else if (relativeRow < row)
            {
                for (int i = relativeRow.Value; i < Count; i++)
                {
                    if (i == row)
                        return top;

                    top += GetHeight(i);
                }
            }
            else if (relativeRow > row)
            {
                for (int i = relativeRow.Value - 1; i >= 0; i--)
                {
                    top -= GetHeight(i);

                    if (i == row)
                        return top;
                }
            }

            throw new IndexOutOfRangeException();
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
            if (IsRowVisible(row) == false)
                SetHeight(row, Grid.DefaultHeight);
        }
        /// <summary>
        /// Hide the specified row (set the height to 0)
        /// </summary>
        /// <param name="row"></param>
        public void HideRow(int row)
        {
            SetHeight(row, 0);
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


	/// <summary>
	/// This class implements a RowsBase class using always the same Height for all rows. Using this class you must only implement the Count method.
	/// </summary>
	public abstract class RowsSimpleBase : RowsBase
	{
		public RowsSimpleBase(GridVirtual grid):base(grid)
		{
			mRowHeight = grid.DefaultHeight;
		}

		private int mRowHeight;
		public int RowHeight
		{
			get{return mRowHeight;}
			set
			{
				if (mRowHeight != value)
				{
					mRowHeight = value;
					PerformLayout();
				}
			}
		}

		public override int GetHeight(int row)
		{
			return RowHeight;
		}
		public override void SetHeight(int row, int height)
		{
			RowHeight = height;
		}
	}

	/// <summary>
	/// Row Information
	/// </summary>
	public class RowInfo
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p_Grid"></param>
		public RowInfo(GridVirtual p_Grid)
		{
			m_Grid = p_Grid;
			m_Height = Grid.DefaultHeight;
		}

		private int m_Height;
		/// <summary>
		/// Height of the current row
		/// </summary>
		public int Height
		{
			get{return m_Height;}
			set
			{
				if (value < 0)
					value = 0;

				if (m_Height != value)
				{
					m_Height = value;
					((RowInfoCollection)m_Grid.Rows).OnRowHeightChanged(new RowInfoEventArgs(this));
				}
			}
		}

		//private int m_Index;
		/// <summary>
		/// Index of the current row
		/// </summary>
		public int Index
		{
			get{return ((RowInfoCollection)Grid.Rows).IndexOf(this);}
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

				return new Range(Index, 0, Index, Grid.Columns.Count - 1);
			}
		}
		private object m_Tag;
		/// <summary>
		/// A property that the user can use to insert custom informations associated to a specific row
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
        /// Gets or sets if the row is visible.
        /// Internally set the height to 0 to hide a row.
        /// </summary>
        public bool Visible
        {
            get { return Height > 0; }
            set
            {
                if (value && Visible == false)
                    Height = Grid.DefaultHeight;
                else if (value == false && Visible)
                    Height = 0;
            }
        }
    }

	/// <summary>
	/// Collection of RowInfo
	/// </summary>
	public abstract class RowInfoCollection : RowsBase, IEnumerable<RowInfo>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="grid"></param>
		public RowInfoCollection(GridVirtual grid):base(grid)
		{
		}

        private List<RowInfo> m_List = new List<RowInfo>();

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
		/// Insert the specified number of rows at the specified position
		/// </summary>
		/// <param name="p_StartIndex"></param>
		/// <param name="rows"></param>
		protected void InsertRange(int p_StartIndex, RowInfo[] rows)
		{
			if (IsValidRangeForInsert(p_StartIndex, rows.Length) == false)
				throw new SourceGridException("Invalid index");

			for (int r = 0; r < rows.Length; r++)
			{
				m_List.Insert(p_StartIndex+r, rows[r]);
			}

			PerformLayout();

			OnRowsAdded(new IndexRangeEventArgs(p_StartIndex, rows.Length));
		}

		/// <summary>
		/// Remove a row at the speicifed position
		/// </summary>
		/// <param name="p_Index"></param>
		public void Remove(int p_Index)
		{
			RemoveRange(p_Index, 1);
		}
		/// <summary>
		/// Remove the RowInfo at the specified positions
		/// </summary>
		/// <param name="p_StartIndex"></param>
		/// <param name="p_Count"></param>
		public void RemoveRange(int p_StartIndex, int p_Count)
		{
			if (IsValidRange(p_StartIndex, p_Count)==false)
				throw new SourceGridException("Invalid index");

			IndexRangeEventArgs eventArgs = new IndexRangeEventArgs(p_StartIndex, p_Count);
			OnRowsRemoving(eventArgs);

			m_List.RemoveRange(p_StartIndex, p_Count);

			OnRowsRemoved(eventArgs);

			PerformLayout();
		}

		#endregion

		/// <summary>
		/// Move a row from one position to another position
		/// </summary>
		/// <param name="p_CurrentRowPosition"></param>
		/// <param name="p_NewRowPosition"></param>
		public void Move(int p_CurrentRowPosition, int p_NewRowPosition)
		{
			if (p_CurrentRowPosition == p_NewRowPosition)
				return;

			if (p_CurrentRowPosition < p_NewRowPosition)
			{
				for (int r = p_CurrentRowPosition; r < p_NewRowPosition; r++)
				{
					Swap(r, r + 1);
				}
			}
			else
			{
				for (int r = p_CurrentRowPosition; r > p_NewRowPosition; r--)
				{
					Swap(r, r - 1);
				}
			}
		}

		/// <summary>
		/// Change the position of row 1 with row 2.
		/// </summary>
		/// <param name="p_RowIndex1"></param>
		/// <param name="p_RowIndex2"></param>
		public void Swap(int p_RowIndex1, int p_RowIndex2)
		{
			if (p_RowIndex1 == p_RowIndex2)
				return;

			RowInfo row1 = this[p_RowIndex1];
			RowInfo row2 = this[p_RowIndex2];

			m_List[p_RowIndex1] = row2;
			m_List[p_RowIndex2] = row1;

			PerformLayout();
		}

		/// <summary>
		/// Fired when the number of rows change
		/// </summary>
		public event IndexRangeEventHandler RowsAdded;

		/// <summary>
		/// Fired when the number of rows change
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnRowsAdded(IndexRangeEventArgs e)
		{
			if (RowsAdded!=null)
				RowsAdded(this, e);

			RowsChanged();
		}

		/// <summary>
		/// Fired when some rows are removed
		/// </summary>
		public event IndexRangeEventHandler RowsRemoved;

		/// <summary>
		/// Fired when some rows are removed
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnRowsRemoved(IndexRangeEventArgs e)
		{
			if (RowsRemoved!=null)
				RowsRemoved(this, e);

			RowsChanged();
		}

		/// <summary>
		/// Fired before some rows are removed
		/// </summary>
		public event IndexRangeEventHandler RowsRemoving;

		/// <summary>
		/// Fired before some rows are removed
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnRowsRemoving(IndexRangeEventArgs e)
		{
			if (RowsRemoving!=null)
				RowsRemoving(this, e);

			//Grid.OnRowsRemoving(e);
		}

		/// <summary>
		/// Indexer. Returns a RowInfo at the specified position
		/// </summary>
		public RowInfo this[int p]
		{
			get{return m_List[p];}
		}

		protected override void OnLayout()
		{
			base.OnLayout ();
		}

		/// <summary>
		/// Fired when the user change the Height property of one of the Row
		/// </summary>
		public event RowInfoEventHandler RowHeightChanged;

		/// <summary>
		/// Execute the RowHeightChanged event
		/// </summary>
		/// <param name="e"></param>
		public void OnRowHeightChanged(RowInfoEventArgs e)
		{
			PerformLayout();

			if (RowHeightChanged!=null)
				RowHeightChanged(this, e);
		}


		public int IndexOf(RowInfo p_Info)
		{
			return m_List.IndexOf(p_Info);
		}

		/// <summary>
        /// Auto size the rows calculating the required size only on the columns currently visible
        /// </summary>
		public void AutoSizeView()
		{
            List<int> list = Grid.Columns.ColumnsInsideRegion(Grid.DisplayRectangle.X, Grid.DisplayRectangle.Width);
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

		#region RowsBase
		public override int GetHeight(int row)
		{
			return this[row].Height;
		}
		public override void SetHeight(int row, int height)
		{
			this[row].Height = height;
		}
        public override AutoSizeMode GetAutoSizeMode(int row)
        {
            return this[row].AutoSizeMode;
        }
		#endregion

		public override int Count
		{
			get{return m_List.Count;}
		}

        #region IEnumerable<RowInfo> Members

        public IEnumerator<RowInfo> GetEnumerator()
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
