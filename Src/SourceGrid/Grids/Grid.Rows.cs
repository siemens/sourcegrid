
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
// Copyright (C) Siemens AG 2017    
//------------------------------------------------------------------------ 
// Project           : UIGrid
// Author            : Sandhra.Prakash@siemens.com
// In Charge for Code: Sandhra.Prakash@siemens.com
//------------------------------------------------------------------------ 

/*Changes :
 * 1. sandhra.prakash@siemens.com : Block the painting when a row is deleted to avoid the count missmatch issue during painting
  */
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid
{
	public class GridRow : RowInfo
	{
		public GridRow(Grid grid)
			: base(grid)
		{
		}

		private Dictionary<GridColumn, Cells.ICell> mCells = new Dictionary<GridColumn, Cells.ICell>();

		public Cells.ICell this[GridColumn column]
		{
			get
			{
				Cells.ICell cell;
				if (mCells.TryGetValue(column, out cell))
					return cell;
				else
					return null;
			}
			set
			{
				mCells[column] = value;
			}
		}
	}

	public class GridRows : RowInfoCollection
	{
		public GridRows(Grid grid)
			: base(grid)
		{
		}
		
		public override void Swap(int p_RowIndex1, int p_RowIndex2)
		{
			base.Swap(p_RowIndex1, p_RowIndex2);
			this.Grid.SpannedCellReferences.Swap(p_RowIndex1, p_RowIndex2);
		}
		

		/// <summary>
		/// Insert a row at the specified position
		/// </summary>
		/// <param name="p_Index"></param>
		public void Insert(int p_Index)
		{
			InsertRange(p_Index, 1);
		}

		/// <summary>
		/// Insert the specified number of rows at the specified position
		/// </summary>
		public void InsertRange(int startIndex, int count)
		{
			RowInfo[] rows = new RowInfo[count];
			for (int i = 0; i < rows.Length; i++)
				rows[i] = CreateRow();

			base.InsertRange(startIndex, rows);
			// Ensure that grid grows when rows are inserted
			this.Grid.GrowGrid();
			
			this.Grid.SpannedCellReferences.MoveDownSpannedRanges(startIndex, count);
			this.Grid.SpannedCellReferences.ExpandSpannedRows(startIndex, count);
			
			
		}
		
		public new Grid Grid
		{
			get { return base.Grid as Grid;}
		}
		
		public override void RemoveRange(int startIndex, int count)
		{
			this.Grid.SpannedCellReferences.RemoveSpannedCellReferencesInRows(startIndex, count);
            //sandhra.prakash@siemens.com : Block the painting when a row is deleted to avoid the count missmatch issue during painting
            this.Grid.BlockPainting = true;
			base.RemoveRange(startIndex, count);
		    this.Grid.BlockPainting = false;
			this.Grid.SpannedCellReferences.ShrinkOrRemoveSpannedRows(startIndex, count);
			this.Grid.SpannedCellReferences.MoveUpSpannedRanges(startIndex, count);
		}

		protected GridRow CreateRow()
		{
			return new GridRow((Grid)Grid);
		}

		public new GridRow this[int index]
		{
			get { return (GridRow)base[index]; }
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
