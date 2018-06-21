
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
 1. chinnari.prasad@siemens.com : Added ClipboardModes to the cell property. enhancement : Enable clipboard modes
*/
#endregion Copyright

using SourceGrid.Cells.Controllers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SourceGrid.Cells
{
	/// <summary>
	/// Interface to represents a cell virtual (without position or value information).
	/// </summary>
	public interface ICellVirtual
	{
		#region Editor
		/// <summary>
		/// Editor of this cell and others cells. If null no edit is supported. 
		///  You can share the same model between many cells to optimize memory size. Warning Changing this property can affect many cells
		/// </summary>
		Editors.EditorBase Editor
		{
			get;
			set;
		}
		#endregion

		#region Controller
		/// <summary>
		/// Controller of the cell. Represents the actions of a cell.
		/// </summary>
		Controllers.ControllerContainer Controller
		{
			get;
		}

		/// <summary>
		/// Add the specified controller.
		/// </summary>
		/// <param name="controller"></param>
		void AddController(Controllers.IController controller);
		/// <summary>
		/// Remove the specifed controller
		/// </summary>
		/// <param name="controller"></param>
		void RemoveController(Controllers.IController controller);
		/// <summary>
		/// Find the specified controller. Returns null if not found.
		/// </summary>
		/// <param name="pControllerType"></param>
		/// <returns></returns>
		Controllers.IController FindController(Type pControllerType);

		/// <summary>
		/// Find the specified controller. Returns null if not found.
		/// </summary>
		/// <returns></returns>
		T FindController<T>() where T: class, IController;
		
		#endregion

		#region View
		/// <summary>
		/// Visual properties of this cell and other cell. You can share the VisualProperties between many cell to optimize memory size.
		/// Warning Changing this property can affect many cells
		/// </summary>
		Views.IView View
		{
			get;
			set;
		}
		#endregion

		#region Model
		/// <summary>
		/// Model that contains the data of the cells. Cannot be null.
		/// </summary>
		Models.ModelContainer Model
		{
			get;
			set;
		}
		#endregion

		#region Copy
		/// <summary>
		/// Create a shallow copy of the current object. Note that this is not a deep clone, all the reference are the same.
		/// Use internally MemberwiseClone method.
		/// </summary>
		/// <returns></returns>
		ICellVirtual Copy();
		#endregion

        #region Clipboard Modes
        /// <summary>
        /// Get or sets the clipboard mode
        /// </summary>
        /// chinnari.prasad@siemens.com : Property to set the clipboard mode
        ClipboardMode ClipboardModes { get; set; }

        #endregion Clipboard Modes
    }
}
