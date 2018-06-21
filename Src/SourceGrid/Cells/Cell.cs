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
 * 1. ToolTipText:sandhra.prakash@siemens.com :  Made Virtual so that it can be overridden in UIGridCell.
 * 2. SetSpan: In Mcpg we have situations were a cell with columnSpan >1 is split into multiple cells with columnSpan 1
 */
#endregion Copyright

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace SourceGrid.Cells
{
	/// <summary>
	/// Represents a Cell in a grid, with Cell.Value support and row/col span. Support also ToolTipText, ContextMenu and Cursor
	/// </summary>
	public class Cell : Virtual.CellVirtual, ICell
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public Cell():this(null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellValue"></param>
		public Cell(object cellValue)
		{
			Model = new SourceGrid.Cells.Models.ModelContainer();
			Model.ValueModel = new Models.ValueModel();

			Model.AddModel(new Models.ToolTip());
			Model.AddModel(new Models.Image());

			Value = cellValue;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellValue"></param>
		/// <param name="pType"></param>
		public Cell(object cellValue, Type pType):this(cellValue)
		{
			Editor = Editors.Factory.Create(pType);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellValue"></param>
		/// <param name="pEditor"></param>
		public Cell(object cellValue, Editors.EditorBase pEditor):this(cellValue)
		{
			Editor = pEditor;
		}
		#endregion

		#region LinkToGrid
		/// <summary>
		/// Link the cell at the specified grid.
		/// For internal use only.
		/// </summary>
		/// <param name="p_grid"></param>
		/// <param name="p_Position"></param>
		public virtual void BindToGrid(Grid p_grid, Position p_Position)
		{
			mGrid = p_grid;
			mRow = Grid.Rows[p_Position.Row];
			mColumn = Grid.Columns[p_Position.Column] as GridColumn;
		}

		/// <summary>
		/// Remove the link of the cell from the grid.
		/// For internal use only.
		/// </summary>
		public virtual void UnBindToGrid()
		{
			mGrid = null;
			mColumn = null;
			mRow = null;
		}

		private Grid mGrid;
		/// <summary>
		/// The Grid object
		/// </summary>
		public Grid Grid
		{
			get { return mGrid; }
		}

		private GridColumn mColumn;
		/// <summary>
		/// Gets the column of the specified cell
		/// </summary>
		public GridColumn Column
		{
			get { return mColumn; }
		}

		private GridRow mRow;
		/// <summary>
		/// Gets the row of the specified cell
		/// </summary>
		public GridRow Row
		{
			get { return mRow; }
		}

		/// <summary>
		/// Gets the range of the cell
		/// </summary>
		public Range Range
		{
			get
			{
				if (Grid == null)
					return Range.Empty;

				int col = Column.Index;
				int row = Row.Index;
				return new Range(row, col,
				                 row + RowSpan - 1, col + ColumnSpan - 1);
			}
		}

		protected CellContext GetContext()
		{
			return new CellContext(Grid, Range.Start, this);
		}

		#endregion

		#region Cell Data (Value, DisplayText, Tag)

		//ATTENTION: is reccomanded that all the actions fired by the user interface does not modify this property
		// instead call the CellEditor.ChangeCellValue to preserve data consistence

		/// <summary>
		/// The string representation of the Cell.Value property (default Value.ToString())
		/// </summary>
		public virtual string DisplayText
		{
			get
			{
				return GetContext().DisplayText;
			}
		}

		/// <summary>
		/// Value of the cell
		/// </summary>
		public virtual object Value
		{
			get{return Model.ValueModel.GetValue(GetContext());}
			set
			{
				Model.ValueModel.SetValue(GetContext(), value);
			}
		}

		/// <summary>
		/// Object to put additional info for this cell
		/// </summary>
		private object m_Tag = null;
		/// <summary>
		/// Object to put additional info for this cell
		/// </summary>
		public virtual object Tag
		{
			get{return m_Tag;}
			set{m_Tag = value;}
		}

		/// <summary>
		/// ToString method
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return DisplayText;
		}
		#endregion

		#region Row/Col Span
		
		public void SetSpan(int rowSpan, int colSpan)
		{
			int oldColSpan = ColumnSpan;
			int oldRowSpan = mRowSpan;
			
			try
			{
                //sandhra.prakash@siemens: In Mcpg we have situations were a cell with columnSpan >1 is split into multiple cells with columnSpan 1 and viceversa
				bool update = false;
				if ((mColumnSpan > colSpan) || (mRowSpan >rowSpan))
					update = true;
				mColumnSpan = colSpan;
				mRowSpan = rowSpan;
				
				if ( mGrid != null )
				{
                    //sandhra.prakash@siemens.com: In Mcpg we have situations were a cell with columnSpan >1 is split into multiple cells with columnSpan 1 and viceversa.
                    if (mColumnSpan != oldColSpan || mRowSpan != oldRowSpan)
					{
						if (update)
							mGrid.UpdateSpannedArea(this.Row.Index, this.Column.Index, this);
						else
							mGrid.OccupySpannedArea(this.Row.Index, this.Column.Index, this);
					}
				}
			}
			catch (OverlappingCellException e)
			{
				mColumnSpan = oldColSpan;
				mRowSpan = oldRowSpan;
				throw new OverlappingCellException("Can not change span", e);
			}
		}
		
		private int mColumnSpan = 1;
		/// <summary>
		/// ColSpan for merge operation
		/// </summary>
		public int ColumnSpan
		{
			get 
            {
                return mColumnSpan; 
            }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("ColumnSpan");

				SetSpan(this.RowSpan, value);
			}
		}

		private int mRowSpan = 1;
		/// <summary>
		/// RowSpan for merge operation
		/// </summary>
		public int RowSpan
		{
            get
            {
               return mRowSpan;
            }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("RowSpan");

				SetSpan(value, this.ColumnSpan);
			}
		}
		#endregion

		#region ToolTipText
		private Models.ToolTip ToolTipModel
		{
			get{return (Models.ToolTip)Model.FindModel(typeof(Models.ToolTip));}
		}

		/// <summary>
		/// Gets or sets the tool tip text of the cell. Internally use the Models.ToolTip class.
		/// sandhra.prakash@siemens.com: made virtual so that in UIGridCell it can be overridden.
		/// </summary>
		public virtual string ToolTipText
		{
			get{return ToolTipModel.ToolTipText;}
			set{ToolTipModel.ToolTipText = value;}
		}
		#endregion

		#region Image
		private Models.Image ImageModel
		{
			get{return (Models.Image)Model.FindModel(typeof(Models.Image));}
		}

		/// <summary>
		/// Gets or sets the Image associeted with the Cell. Internally use a Models.Image class.
		/// </summary>
		public System.Drawing.Image Image
		{
			get{return ImageModel.ImageValue;}
			set{ImageModel.ImageValue = value;}
		}
		#endregion
	}
}
