using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid
{
	public partial class Grid
	{
		public override bool EnableSort {get;set;}
		
	}
	
	public class GridColumn : ColumnInfo
	{
		public GridColumn(Grid grid)
			: base(grid)
		{
		}

		private Dictionary<GridRow, Cells.ICell> mCells = new Dictionary<GridRow, Cells.ICell>();

		public Cells.ICell this[GridRow row]
		{
			get
			{
				Cells.ICell cell;
				if (mCells.TryGetValue(row, out cell))
					return cell;
				else
					return null;
			}
			set
			{
				mCells[row] = value;
			}
		}
	}
    
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
     * 1.  InsertRange(int startIndex, int count): Added Grid.Grow as a fix to a defect.
     * Defect - If the total number of rows is less than 4 and the total column count is more 
     * than 4(say 9). And if the 5th column (columnIndex = 4) is having a columnSpan of 2, 
     * an arugmentOutOfRangeException will be thrown. Its because the grid didnot grow 
     * and update the bounds.
    */
#endregion Copyright

    public class GridColumns : ColumnInfoCollection
	{
		public new Grid Grid
		{
			get { return base.Grid as Grid;}
		}
		
		public GridColumns(Grid grid)
			: base(grid)
		{
		}

		/// <summary>
		/// Insert a column at the specified position
		/// </summary>
		/// <param name="p_Index"></param>
		public void Insert(int p_Index)
		{
			InsertRange(p_Index, 1);
		}

		/// <summary>
		/// Insert the specified number of Columns at the specified position
		/// </summary>
		public void InsertRange(int startIndex, int count)
		{
			GridColumn[] columns = new GridColumn[count];
			for (int i = 0; i < columns.Length; i++)
				columns[i] = CreateColumn();

			InsertRange(startIndex, columns);

            //sandhra.prakash@siemens.com : Ensures grid grows when columns are inserted
            this.Grid.GrowGrid();

			this.Grid.SpannedCellReferences.MoveRightSpannedRanges(startIndex, count);
			this.Grid.SpannedCellReferences.ExpandSpannedColumns(startIndex, count);
		}
		
		public override void RemoveRange(int startIndex, int count)
		{
			this.Grid.SpannedCellReferences.RemoveSpannedCellReferencesInColumns(startIndex, count);
			base.RemoveRange(startIndex, count);
			this.Grid.SpannedCellReferences.ShrinkOrRemoveSpannedColumns(startIndex, count);
			this.Grid.SpannedCellReferences.MoveLeftSpannedRanges(startIndex, count);
		}
		
		protected GridColumn CreateColumn()
		{
			return new GridColumn((Grid)Grid);
		}

		public void SetCount(int value)
		{
			this.Grid.GrowGrid();
			if (Count < value)
				InsertRange(Count, value - Count);
			else if (Count > value)
				RemoveRange(value, Count - value);
		}
	}
}
