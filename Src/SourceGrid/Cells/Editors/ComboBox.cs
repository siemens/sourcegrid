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
 1. Validation for Control added in SetEditValue, GetEditedValue,OnSendCharToEditor
 * 2. CreateControl() : To support Selectable readonly text enhancement, new custom control is used as the control of this editor
*/
#endregion Copyright
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevAge.Windows.Forms;


namespace SourceGrid.Cells.Editors
{
	/// <summary>
	/// Editor for a ComboBox (using DevAgeComboBox control)
	/// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public class ComboBox : EditorControlBase
	{
		#region Constructor
		/// <summary>
		/// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
		/// </summary>
		/// <param name="p_Type">The type of this model</param>
		public ComboBox(Type p_Type):base(p_Type)
		{
		}

		/// <summary>
		/// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
		/// </summary>
		/// <param name="p_Type">The type of this model</param>
		/// <param name="p_StandardValues"></param>
		/// <param name="p_StandardValueExclusive">True to not allow custom value, only the values specified in the standardvalues collection are allowed.</param>
		public ComboBox(Type p_Type, ICollection p_StandardValues, bool p_StandardValueExclusive):base(p_Type)
		{
			StandardValues = p_StandardValues;
			StandardValuesExclusive = p_StandardValueExclusive;
		}
		#endregion

		#region Edit Control
		/// <summary>
		/// Create the editor control
		/// </summary>
		/// <returns></returns>
		protected override Control CreateControl()
		{
            
            //sandhra.prakash@siemens.com: CreateControl() : To support Selectable readonly text enhancement, new custom control is used as the control of this editor
            UIComboBox editor = new UIComboBox();
			//editor.FlatStyle = FlatStyle.System;
            editor.BorderStyle = DevAge.Drawing.BorderStyle.None;
			editor.ComboBox.Validator = this;

            //NOTE: I have changed a little the ArrangeLinkedControls to support ComboBox control

			return editor;
		}

	    /// <summary>
		/// Gets the control used for editing the cell.
		/// </summary>
        public new UIComboBox Control
		{
			get
			{
                return (UIComboBox)base.Control;
			}
		}
		#endregion

		/// <summary>
		/// Set the specified value in the current editor control.
		/// </summary>
		/// <param name="editValue"></param>
		public override void SetEditValue(object editValue)
		{
            //sandhra.prakash@siemens.com: Check to validate control
            if(Control == null)
                return;

			if (editValue is string && IsStringConversionSupported() &&
                    Control.ComboBox.DropDownStyle == ComboBoxStyle.DropDown)
			{
                Control.ComboBox.SelectedIndex = -1;
                Control.ComboBox.Text = (string)editValue;
                Control.ComboBox.SelectionLength = 0;
                if (Control.ComboBox.Text != null)
                    Control.ComboBox.SelectionStart = Control.ComboBox.Text.Length;
				else
                    Control.ComboBox.SelectionStart = 0;
			}
			else
			{
                Control.ComboBox.SelectedIndex = -1;
                Control.ComboBox.Value = editValue;
                Control.ComboBox.SelectAll();
			}
		}

		/// <summary>
		/// Returns the value inserted with the current editor control
		/// </summary>
		/// <returns></returns>
		public override object GetEditedValue()
		{
            //sandhra.prakash@siemens.com: Check to validate control
            return Control != null ? Control.ComboBox.Value : null;
		}

        protected override void OnSendCharToEditor(char key)
        {
            //sandhra.prakash@siemens.com: Check to validate control
            if (Control != null && Control.ComboBox.DropDownStyle == ComboBoxStyle.DropDown)
            {
                Control.ComboBox.Text = key.ToString();
                if (Control.ComboBox.Text != null)
                    Control.ComboBox.SelectionStart = Control.ComboBox.Text.Length;
            }
        }
	}
}

