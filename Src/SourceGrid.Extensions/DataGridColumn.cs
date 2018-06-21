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
 * 1. FilterCell Property added to support Filter.
*/
#endregion Copyright

using SourceGrid.Cells.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SourceGrid.Cells;

namespace SourceGrid
{
	public class DataGridColumns : ColumnInfoCollection
	{
		public DataGridColumns(DataGrid grid)
			: base(grid)
		{
		}

		public new DataGrid Grid
		{
			get { return (DataGrid)base.Grid; }
		}

		public new DataGridColumn this[int index]
		{
			get { return base[index] as DataGridColumn; }
		}

		/// <summary>
		/// Return the DataColumn object for a given grid column index. Return null if not applicable, for example if the column index requested is a FixedColumns of an unbound column
		/// </summary>
		/// <param name="gridColumnIndex"></param>
		/// <returns></returns>
		public System.ComponentModel.PropertyDescriptor IndexToPropertyColumn(int gridColumnIndex)
		{
			return Grid.Columns[gridColumnIndex].PropertyColumn;
		}
		/// <summary>
		/// Returns the index for a given DataColumn. -1 if not valid.
		/// </summary>
		/// <returns></returns>
		public int DataSourceColumnToIndex(System.ComponentModel.PropertyDescriptor propertyColumn)
		{
			for (int i = 0; i < Grid.Columns.Count; i++)
			{
				if (Grid.Columns[i].PropertyColumn == propertyColumn)
					return i;
			}

			return -1;
		}

		#region Add Helper methods
		public DataGridColumn Add(string property,
		                          string caption,
		                          Type propertyType)
		{
			SourceGrid.Cells.ICellVirtual cell = SourceGrid.Cells.DataGrid.Cell.Create(propertyType, true);

			return Add(property, caption, cell);
		}

		public DataGridColumn Add(string property,
		                          string caption,
		                          EditorBase editor)
		{
			SourceGrid.Cells.DataGrid.Cell cell = new SourceGrid.Cells.DataGrid.Cell();
			cell.Editor = editor;

			return Add(property, caption, cell);
		}

		public DataGridColumn Add(string property,
		                          string caption,
		                          ICellVirtual cell)
		{
			SourceGrid.DataGridColumn col = new DataGridColumn(Grid,
			                                                   new SourceGrid.Cells.DataGrid.ColumnHeader(caption),
			                                                   cell,
			                                                   property);
			Insert(Count, col);

			return col;
		}
		#endregion
	}

	/// <summary>
	/// A ColumnInfo derived class used to store column informations for a DataGrid control.
	/// Mantains the cell used on this grid and manage the binding to the DataSource using a DataGridValueModel class.
	/// </summary>
	public class DataGridColumn : ColumnInfo
	{
		/// <summary>
		/// Constructor. Create a DataGridColumn class.
		/// </summary>
		/// <param name="grid"></param>
		public DataGridColumn(DataGrid grid)
			: base(grid)
		{
			mHeaderCell = new Cells.DataGrid.ColumnHeader(string.Empty);
			mDataCell = new Cells.DataGrid.Cell();
		}

		/// <summary>
		/// Constructor. Create a DataGridColumn class.
		/// </summary>
		public DataGridColumn(DataGrid grid,
		                      Cells.ICellVirtual headerCell,
		                      Cells.ICellVirtual dataCell,
		                      string propertyName)
			: base(grid)
		{
			mPropertyName = propertyName;
			mHeaderCell = headerCell;
			
            mDataCell = dataCell;
		}

        public SourceGrid.Cells.ICellVirtual FilterCell
        {
            get
            {
                return mFilterCell;//
                //mFilterCell = new SourceGrid.Cells.Cell();
            }
            set
            {
                mFilterCell = value;
            }
        }
		/// <summary>
		/// Create a DataGridColumn with special cells used for RowHeader, usually used when FixedColumns is 1 for the first column.
		/// </summary>
		/// <param name="grid"></param>
		/// <returns></returns>
		public static DataGridColumn CreateRowHeader(DataGrid grid)
		{
			return new DataGridColumn(grid,
			                          new Cells.DataGrid.Header(),
			                          new Cells.DataGrid.RowHeader(),
			                          null);
		}

		public new DataGrid Grid
		{
			get { return (DataGrid)base.Grid; }
		}
        private Cells.ICellVirtual mFilterCell;
		private string mPropertyName;
		public string PropertyName
		{
			get { return mPropertyName; }
			set { mPropertyName = value; mPropertyColumn = null; }
		}
		
		/// <summary>
		/// Clears any associated data with DataGridView.
		/// <remarks>PropertyColumn binds to DataTable, calling invalidate will remove
		/// this link</remarks>
		/// </summary>
		public void Invalidate()
		{
			mPropertyColumn = null;
		}
		
		private PropertyDescriptor mPropertyColumn;
		/// <summary>
		/// Gets the property column. Can be null if not bound to a datasource Column.
		/// This field is used for example to support sorting.
		/// </summary>
		public PropertyDescriptor PropertyColumn
		{
			get
			{
				if (mPropertyColumn == null && Grid.DataSource != null)
					mPropertyColumn = Grid.DataSource.GetItemProperty(PropertyName, StringComparison.InvariantCultureIgnoreCase);

				return mPropertyColumn;
			}
		}

		private Cells.ICellVirtual mHeaderCell;
		/// <summary>
		/// Gets or sets the header cell for this column.
		/// Typically is an instance of SourceGrid.Cells.DataGrid.ColumnHeader
		/// </summary>
		public Cells.ICellVirtual HeaderCell
		{
			get { return mHeaderCell; }
			set { mHeaderCell = value; }
		}

		private Cells.ICellVirtual mDataCell;
		/// <summary>
		/// Gets or sets the cell used for this column for all the rows to disply the data
		/// Typically is an instance of SourceGrid.Cells.DataGrid.Cell or other classes of the same namespace
		/// </summary>
		public Cells.ICellVirtual DataCell
		{
			get { return mDataCell; }
			set { mDataCell = value; }
		}

		private List<Conditions.ICondition> mConditions = new List<Conditions.ICondition>();
		/// <summary>
		/// Gets the conditions used to returns different cell based on the data of the row.
		/// </summary>
		public List<Conditions.ICondition> Conditions
		{
			get { return mConditions; }
		}

		private Dictionary<Conditions.ICondition, Cells.ICellVirtual> mConditionalCells = new Dictionary<Conditions.ICondition, Cells.ICellVirtual>();


		/// <summary>
		/// Gets the ICellVirtual for the current column and the specified row.
		/// Override this method to provide custom cells, based on the row informations.
		/// </summary>
		/// <param name="gridRow"></param>
		/// <returns></returns>
		public virtual Cells.ICellVirtual GetDataCell(int gridRow)
		{
			object itemRow = Grid.Rows.IndexToDataSourceRow(gridRow);

			foreach (Conditions.ICondition con in Conditions)
			{
				if (con.Evaluate(this, gridRow, itemRow))
				{
					Cells.ICellVirtual cell;
					if (mConditionalCells.TryGetValue(con, out cell) == false)
					{
						cell = con.ApplyCondition(DataCell);
						mConditionalCells.Add(con, cell);
					}

					return cell;
				}
			}

			return DataCell;
		}
	}

}
