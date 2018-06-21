
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
 1. chinnari.prasad@siemens.com : Setting ClipboardModes in RowHeader cell and cell virtual. enhancement : Enable clipboard modes
*/
#endregion Copyright

using System;
using System.Drawing;

namespace SourceGrid.Cells.Virtual
{
	/// <summary>
	/// A cell that rappresent a header of a table, with 3D effect. This cell override IsSelectable to false. Default use VisualModels.VisualModelHeader.Style1
	/// </summary>
	public class RowHeader : CellVirtual
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RowHeader()
		{
			View = Views.RowHeader.Default;
			AddController(Controllers.Unselectable.Default);
			AddController(Controllers.MouseInvalidate.Default);
			ResizeEnabled = true;
            //chinnari.prasad@siemens.com : Setting the clipboard mode
            ClipboardModes = ClipboardMode.Copy;
		}

		/// <summary>
		/// Gets or sets if enable the resize of the height, using a Resizable controller. Default is true.
		/// </summary>
		public bool ResizeEnabled
		{
			get{return FindController(typeof(Controllers.Resizable)) == Controllers.Resizable.ResizeHeight;}
			set
			{
                if (value == ResizeEnabled)
                    return;

				if (value)
					AddController(Controllers.Resizable.ResizeHeight);
				else
					RemoveController(Controllers.Resizable.ResizeHeight);
			}
		}
	}
}

namespace SourceGrid.Cells
{
	/// <summary>
	/// A cell that rappresent a header of a table, with 3D effect. This cell override IsSelectable to false. Default use VisualModels.VisualModelHeader.Style1
	/// </summary>
	public class RowHeader : Cell
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RowHeader():this(null)
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellValue"></param>
		public RowHeader(object cellValue):base(cellValue)
		{
			View = Views.RowHeader.Default;
			AddController(Controllers.Unselectable.Default);
			AddController(Controllers.MouseInvalidate.Default);
			ResizeEnabled = true;
            //chinnari.prasad@siemens.com : Setting the clipboard mode
            ClipboardModes = ClipboardMode.Copy;
		}

		/// <summary>
		/// Gets or sets if enable the resize of the height, using a Resizable controller. Default is true.
		/// </summary>
		public bool ResizeEnabled
		{
			get{return FindController(typeof(Controllers.Resizable)) == Controllers.Resizable.ResizeHeight;}
			set
			{
                if (value == ResizeEnabled)
                    return;

				if (value)
					AddController(Controllers.Resizable.ResizeHeight);
				else
					RemoveController(Controllers.Resizable.ResizeHeight);
			}
		}
	}

}