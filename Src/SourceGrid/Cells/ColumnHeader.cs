
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
// Author            : chinnari.prasad@siemens.com 
// In Charge for Code: chinnari.prasad@siemens.com
//------------------------------------------------------------------------ 

/*Changes :
 1. chinnari.prasad@siemens.com : Setting ClipboardModes in ColumnHeader cell and cell virtual. enhancement : Enable clipboard modes
*/
#endregion Copyright

using System;
using System.Drawing;

namespace SourceGrid.Cells.Virtual
{
	/// <summary>
	/// A cell that rappresent a header of a table, with 3D effect. This cell override IsSelectable to false. Default use VisualModels.VisualModelHeader.Style1
	/// </summary>
	public class ColumnHeader : CellVirtual
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ColumnHeader()
		{
			View = Views.ColumnHeader.Default;
			Model.AddModel(new Models.SortableHeader());
			AddController(Controllers.Unselectable.Default);
			AddController(Controllers.MouseInvalidate.Default);
			ResizeEnabled = true;
			AutomaticSortEnabled = true;
            //chinnari.prasad@siemens.com : Setting the clipboard mode
            ClipboardModes = ClipboardMode.Copy;
		}

		/// <summary>
		/// Gets or sets if enable the resize of the width of the column. This property internally use the Controllers.Resizable.ResizeWidth.
		/// </summary>
		public bool ResizeEnabled
		{
			get{return FindController(typeof(Controllers.Resizable)) == Controllers.Resizable.ResizeWidth;}
			set
			{
                if (value == ResizeEnabled)
                    return;

				if (value)
					AddController(Controllers.Resizable.ResizeWidth);
				else
					RemoveController(Controllers.Resizable.ResizeWidth);
			}
		}

		/// <summary>
		/// Gets or sets if enable the automatic sort features of the column. This property internally use the Controllers.SortableHeader.Default.
		/// </summary>
		public bool AutomaticSortEnabled
		{
			get{return FindController(typeof(Controllers.SortableHeader)) == Controllers.SortableHeader.Default;}
			set
			{
                if (value == AutomaticSortEnabled)
                    return;

				if (value)
					AddController(Controllers.SortableHeader.Default);
				else
					RemoveController(Controllers.SortableHeader.Default);
			}
		}
	}
}

namespace SourceGrid.Cells
{
	/// <summary>
	/// A cell that rappresent a header of a table. 
	/// View: Views.ColumnHeader.Default 
	/// Model: Models.SortableHeader 
	/// Controllers: Controllers.Unselectable.Default, Controllers.MouseInvalidate.Default, Controllers.Resizable.ResizeWidth, Controllers.SortableHeader.Default 
	/// </summary>
	public class ColumnHeader : Cell
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ColumnHeader():this(null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellValue"></param>
		public ColumnHeader(object cellValue):base(cellValue)
		{
			View = Views.ColumnHeader.Default;
			Model.AddModel(new Models.SortableHeader());
			AddController(Controllers.Unselectable.Default);
			AddController(Controllers.MouseInvalidate.Default);
			ResizeEnabled = true;
			AutomaticSortEnabled = true;
            //chinnari.prasad@siemens.com : Setting the clipboard mode
            ClipboardModes = ClipboardMode.Copy;
		}

		/// <summary>
		/// Gets or sets if enable the resize of the width of the column. This property internally use the Controllers.Resizable.ResizeWidth.
		/// </summary>
		public bool ResizeEnabled
		{
			get{return FindController(typeof(Controllers.Resizable)) == Controllers.Resizable.ResizeWidth;}
			set
			{
                if (value == ResizeEnabled)
                    return;

				if (value)
					AddController(Controllers.Resizable.ResizeWidth);
				else
					RemoveController(Controllers.Resizable.ResizeWidth);
			}
		}

		/// <summary>
		/// Gets or sets if enable the automatic sort features of the column. This property internally use the Controllers.SortableHeader.Default.
		/// If you want to use a custom sort you can add a customized Controller or a customized instance of Controllers.SortableHeader.
		/// </summary>
		public bool AutomaticSortEnabled
		{
			get{return FindController(typeof(Controllers.SortableHeader)) == Controllers.SortableHeader.Default;}
			set
			{
                if (value == AutomaticSortEnabled)
                    return;

				if (value)
					AddController(Controllers.SortableHeader.Default);
				else
					RemoveController(Controllers.SortableHeader.Default);
			}
		}

		/// <summary>
		/// Gets the used SortableHeader model.
		/// </summary>
		private Models.SortableHeader SortableHeaderModel
		{
			get{return (Models.SortableHeader)Model.FindModel(typeof(Models.SortableHeader));}
		}

		/// <summary>
		/// Status of the sort.
		/// </summary>
		public Models.SortStatus SortStatus
		{
			get{return SortableHeaderModel.SortStatus;}
			set{SortableHeaderModel.SortStatus = value;}
		}

		/// <summary>
		/// Comparer used.
		/// </summary>
		public System.Collections.IComparer SortComparer
		{
			get{return SortStatus.Comparer;}
			set{SortStatus = new SourceGrid.Cells.Models.SortStatus(SortStatus.Style, value);}
		}

		/// <summary>
		/// Sort style.
		/// </summary>
		public DevAge.Drawing.HeaderSortStyle SortStyle
		{
			get{return SortStatus.Style;}
			set{SortStatus = new SourceGrid.Cells.Models.SortStatus(value, SortStatus.Comparer);}
		}

		/// <summary>
		/// Sort the column
		/// </summary>
		/// <param name="ascending"></param>
		public void Sort(bool ascending)
		{
			Controllers.SortableHeader sortableHeader = (Controllers.SortableHeader)FindController(typeof(Controllers.SortableHeader));
			if (sortableHeader == null)
				throw new SourceGridException("No SortableHeader controller found");
			sortableHeader.SortColumn(new CellContext(Grid, Range.Start, this), ascending, SortComparer);
		}
	}

}