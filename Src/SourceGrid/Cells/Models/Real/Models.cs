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
 * 1. class - CheckBox - sandhra.prakash@siemens.com:Readonly state should also be passed as a checkbox state - Selectable readonly enhancement 
*/

#endregion Copyright

using System;

namespace SourceGrid.Cells.Models
{
	public class NullValueModel : IValueModel
	{
		public readonly static NullValueModel Default = new NullValueModel();

		/// <summary>
		/// Constructor
		/// </summary>
		public NullValueModel()
		{
		}
		#region IModel Members
		public object GetValue(CellContext cellContext)
		{
			return null;
		}

		public void SetValue(CellContext cellContext, object p_Value)
		{
			throw new ApplicationException("This model doesn't support editing");
		}
		public string GetDisplayText(CellContext cellContext)
		{
			return null;
		}
		#endregion
	}

	/// <summary>
	/// A model that contains the value of cell. Usually used for a Real Cell or cells with a static text.
	/// </summary>
	public class ValueModel : IValueModel
	{
		public ValueModel()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val">value to set for this model</param>
		public ValueModel(object val)
		{
			m_Value = val;
		}

		private object m_Value;
		#region IModel Members

		public object GetValue(CellContext cellContext)
		{
			return m_Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cellContext"></param>
		/// <param name="newValue">new value of this model</param>
		public void SetValue(CellContext cellContext, object newValue)
		{
			if (IsNewValueEqual(newValue) == true)
				return;
			ValueChangeEventArgs valArgs = new ValueChangeEventArgs(m_Value, newValue);
			if (cellContext.Grid != null)
				cellContext.Grid.Controller.OnValueChanging(cellContext, valArgs);
			m_Value = valArgs.NewValue;
			if (cellContext.Grid != null)
				cellContext.Grid.Controller.OnValueChanged(cellContext, EventArgs.Empty);
		}
		
		public bool IsNewValueEqual(object newValue)
		{
			if (newValue == m_Value)
				return true;
			object valueModel = m_Value;
			if (valueModel == null)
				valueModel = string.Empty;
			if (newValue == null)
				newValue = string.Empty;
				
			return newValue.Equals(valueModel);
		}
		#endregion
	}

	/// <summary>
	/// CheckBox model.
	/// </summary>
	public class CheckBox : ICheckBox
	{
		#region ICheckBox Members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cellContext"></param>
		/// <returns></returns>
		public CheckBoxStatus GetCheckBoxStatus(CellContext cellContext)
		{
            //sandhra.prakash@siemens.com:Readonly state should also be passed as a checkbox state - Selectable readonly enhancement
			bool enableEdit = false;
		    bool readOnly = false;
		    if (cellContext.Cell.Editor != null)
		    {
                enableEdit = cellContext.Cell.Editor.EnableEdit;
		        readOnly = cellContext.Cell.Editor.ReadOnly;
		    }

			object val = cellContext.Cell.Model.ValueModel.GetValue(cellContext);
			if (val == null)
                return new CheckBoxStatus(enableEdit, DevAge.Drawing.CheckBoxState.Undefined, m_Caption, readOnly);
			else if (val is bool)
                return new CheckBoxStatus(enableEdit, (bool)val, m_Caption, readOnly);
			else
				throw new SourceGridException("Cell value not supported for this cell. Expected bool value or null.");
		}
		/// <summary>
		/// Set the checked value
		/// </summary>
		/// <param name="cellContext"></param>
		/// <param name="pChecked"></param>
		public void SetCheckedValue(CellContext cellContext, bool? pChecked)
		{
			if (cellContext.Cell.Editor != null && cellContext.Cell.Editor.EnableEdit)
				cellContext.Cell.Editor.SetCellValue(cellContext, pChecked);
		}
		#endregion

		private string m_Caption = null;
		public string Caption
		{
			get{return m_Caption;}
			set{m_Caption = value;}
		}
	}

	public class SortableHeader : ISortableHeader
	{
		#region ISortableHeader Members

		public SortStatus GetSortStatus(CellContext cellContext)
		{
			return m_SortStatus;
		}

        public void SetSortMode(CellContext cellContext, DevAge.Drawing.HeaderSortStyle pStyle)
		{
			m_SortStatus.Style = pStyle;
		}

		#endregion

        private SortStatus m_SortStatus = new SortStatus(DevAge.Drawing.HeaderSortStyle.None, null);
		public SortStatus SortStatus
		{
			get{return m_SortStatus;}
			set{m_SortStatus = value;}
		}
	}

	public class ToolTip : IToolTipText
	{
		#region IToolTipText Members

		public string GetToolTipText(CellContext cellContext)
		{
			if ( string.IsNullOrEmpty(m_ToolTipText) && ! cellContext.IsEmpty() )
				return cellContext.DisplayText;
			return m_ToolTipText;
		}

		#endregion

		private string m_ToolTipText;
		public string ToolTipText
		{
			get{return m_ToolTipText;}
			set{m_ToolTipText = value;}
		}
	}

	public class Image : IImage
	{
		public Image()
		{
		}

		public Image(System.Drawing.Image image)
		{
			mImage = image;
		}

		#region IImage Members
		/// <summary>
		/// Get the image of the specified cell. 
		/// </summary>
		/// <returns></returns>
		public System.Drawing.Image GetImage(CellContext cellContext)
		{
			return mImage;
		}
		#endregion

		private System.Drawing.Image mImage;
		public System.Drawing.Image ImageValue
		{
			get{return mImage;}
			set{mImage = value;}
		}
	}

	/// <summary>
	/// Model that implements the IImage interface, used to read the Image directly from the Value of the cell.
	/// </summary>
	public class ValueImage : IImage
	{
		public static readonly ValueImage Default = new ValueImage();

		private DevAge.ComponentModel.Validator.ValidatorTypeConverter imageConverter = new DevAge.ComponentModel.Validator.ValidatorTypeConverter(typeof(System.Drawing.Image));
		#region IImage Members

		public System.Drawing.Image GetImage(CellContext cellContext)
		{
			object val = cellContext.Cell.Model.ValueModel.GetValue(cellContext);
			return (System.Drawing.Image)imageConverter.ObjectToValue(val);
		}
		#endregion
	}
}
