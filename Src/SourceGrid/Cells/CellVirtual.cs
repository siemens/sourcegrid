
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SourceGrid.Cells.Virtual
{
	/// <summary>
	/// Represents a CellVirtual in a grid.
	/// </summary>
	public class CellVirtual : ICellVirtual
	{
		#region Constructor

		/// <summary>
		/// Constructor. Create a CellVirtual using a default NullValueModel. You must provide your custom ValueModel to bind the cell to a value.
		/// </summary>
		public CellVirtual()
		{
			View = Views.Cell.Default;
			Editor = null;
			Model = new SourceGrid.Cells.Models.ModelContainer();
			Model.AddModel(Cells.Models.NullValueModel.Default);
            //chinnari.prasad@siemens.com : Setting the clipboard mode
            ClipboardModes = ClipboardMode.All;
		}

		/// <summary>
		/// Constructor. Create a CellVirtual using a default NullValueModel. You must provide your custom ValueModel to bind the cell to a value.
		/// </summary>
		public CellVirtual(Type type):this()
		{
			Editor = Editors.Factory.Create(type);
		}
		#endregion

		#region Model
		private Models.ModelContainer m_Model;

		/// <summary>
		/// Represents the model of the cell.
		/// </summary>
		public Models.ModelContainer Model
		{
			get{return m_Model;}
			set
			{
				if (value == null)
					throw new SourceGridException("Model cannot be null");
				m_Model = value;
			}
		}
		#endregion

		#region View

		private Views.IView m_View;

		/// <summary>
		/// Visual properties of this cell and other cell. You can share the VisualProperties between many cell to optimize memory size.
		/// Warning Changing this property can affect many cells
		/// </summary>
		public virtual Views.IView View
		{
			get{return m_View;}
			set
			{
				if (value == null)
					throw new ArgumentNullException("View");

				m_View = value;
			}
		}
		#endregion

		#region Controller
		private Controllers.ControllerContainer m_Controller;
		/// <summary>
		/// Controller of the cell.
		/// </summary>
		public Controllers.ControllerContainer Controller
		{
			get{return m_Controller;}
		}
		/// <summary>
		/// Add the specified controller
		/// </summary>
		/// <param name="controller"></param>
		public void AddController(Controllers.IController controller)
		{
			if (m_Controller == null)
				m_Controller = new SourceGrid.Cells.Controllers.ControllerContainer();
			m_Controller.AddController(controller);
		}
		/// <summary>
		/// Remove the specifed controller
		/// </summary>
		/// <param name="controller"></param>
		public void RemoveController(Controllers.IController controller)
		{
			if (m_Controller != null)
				m_Controller.RemoveController(controller);
		}
		/// <summary>
		/// Find the specified controller. Returns null if not found.
		/// </summary>
		/// <param name="pControllerType"></param>
		/// <returns></returns>
		public Controllers.IController FindController(Type pControllerType)
		{
			if (m_Controller != null)
				return m_Controller.FindController(pControllerType);
			else
				return null;
		}
		
		/// <summary>
		/// Find the specified controller. Returns null if not found.
		/// </summary>
		/// <returns></returns>
		public T FindController<T>() where T: class, IController
		{
			return this.FindController(typeof(T)) as T;
		}
		#endregion

		#region Editor

		private Editors.EditorBase m_Editor = null;
		/// <summary>
		/// Editor of this cell and others cells. If null no edit is supported. 
		///  You can share the same model between many cells to optimize memory size. Warning Changing this property can affect many cells
		/// </summary>
		public Editors.EditorBase Editor
		{
			get{return m_Editor;}
			set{m_Editor = value;}
		}
		#endregion

		#region Copy
		/// <summary>
		/// Create a shallow copy of the current object. Note that this is not a deep clone, all the reference are the same.
		/// Use internally MemberwiseClone method.
		/// </summary>
		/// <returns></returns>
		public ICellVirtual Copy()
		{
			return (ICellVirtual)MemberwiseClone();
		}
		#endregion

        #region Clipboard Modes
        /// <summary>
        /// Get or sets the clipboard mode
        /// </summary>
        /// chinnari.prasad@siemens.com : Property to set the clipboard mode
        public ClipboardMode ClipboardModes { get; set; }

        #endregion Clipboard Modes
	}
}
