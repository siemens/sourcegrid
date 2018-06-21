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
 1. Validation for Control added in OnStartingEdit, SetEditValue, GetEditedValue,OnSendCharToEditor
*/
#endregion Copyright
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;


namespace SourceGrid.Cells.Editors
{
	/// <summary>
	/// An editor that use a TextBoxTypedButton for editing.
	/// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public class TextBoxButton : EditorControlBase
	{
		#region Constructor
		/// <summary>
		/// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
		/// </summary>
		/// <param name="p_Type">The type of this model</param>
		public TextBoxButton(Type p_Type):base(p_Type)
		{
		}
		#endregion

		#region Edit Control
		/// <summary>
		/// Create the editor control
		/// </summary>
		/// <returns></returns>
		protected override Control CreateControl()
		{
			DevAge.Windows.Forms.DevAgeTextBoxButton editor = new DevAge.Windows.Forms.DevAgeTextBoxButton();
			editor.BorderStyle = DevAge.Drawing.BorderStyle.None;
			editor.Validator = this;
			return editor;
		}

		/// <summary>
		/// Gets the control used for editing the cell.
		/// </summary>
		public new DevAge.Windows.Forms.DevAgeTextBoxButton Control
		{
			get
			{
				return (DevAge.Windows.Forms.DevAgeTextBoxButton)base.Control;
			}
		}
		#endregion

		/// <summary>
		/// This method is called just before the edit start. You can use this method to customize the editor with the cell informations.
		/// </summary>
		/// <param name="cellContext"></param>
		/// <param name="editorControl"></param>
		protected override void OnStartingEdit(CellContext cellContext, Control editorControl)
		{
			base.OnStartingEdit(cellContext, editorControl);

			DevAge.Windows.Forms.DevAgeTextBoxButton editor = editorControl as DevAge.Windows.Forms.DevAgeTextBoxButton;

            //sandhra.prakash@siemens.com: Check to validate control
            if(editor == null)
                return;
			//to set the scroll of the textbox to the initial position (otherwise the textbox use the previous scroll position)
			editor.TextBox.SelectionStart = 0;
			editor.TextBox.SelectionLength = 0;
		}

		/// <summary>
		/// Set the specified value in the current editor control.
		/// </summary>
		/// <param name="editValue"></param>
		public override void SetEditValue(object editValue)
		{
            //sandhra.prakash@siemens.com: Check to validate control
		    if (Control != null)
		    {
		        Control.Value = editValue;
		        Control.TextBox.SelectAll();
		    }
		}

		/// <summary>
		/// Returns the value inserted with the current editor control
		/// </summary>
		/// <returns></returns>
		public override object GetEditedValue()
		{
            //sandhra.prakash@siemens.com: Check to validate control
            return Control != null ? Control.Value : null;
		}

        protected override void OnSendCharToEditor(char key)
        {
            //sandhra.prakash@siemens.com: Check to validate control. Only is TextBox is editable 
            if (Control != null && !Control.TextBox.ReadOnly)
            {
                Control.TextBox.Text = key.ToString();
                if (Control.TextBox.Text != null)
                    Control.TextBox.SelectionStart = Control.TextBox.Text.Length;
            }
        }
    }
}

