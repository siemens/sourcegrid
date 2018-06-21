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
 1. Validation for Control added in OnStartingEdit, SetEditValue, GetEditedValue
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
	/// Create an Editor that use a DateTimePicker as control for date editing.
	/// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public class DateTimePicker : EditorControlBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DateTimePicker():base(typeof(System.DateTime))
		{
		}

		#region Edit Control
		/// <summary>
		/// Create the editor control
		/// </summary>
		/// <returns></returns>
		protected override Control CreateControl()
		{
			System.Windows.Forms.DateTimePicker dtPicker = new System.Windows.Forms.DateTimePicker();
			dtPicker.Format = DateTimePickerFormat.Short;
            dtPicker.ShowCheckBox = AllowNull;
			return dtPicker;
		}

        protected override void OnChanged(EventArgs e)
        {
            base.OnChanged(e);

            //if the control is null the editor is not yet created
            if (Control != null)
            {
                Control.ShowCheckBox = AllowNull;
            }
        }

		/// <summary>
		/// Gets the control used for editing the cell.
		/// </summary>
		public new System.Windows.Forms.DateTimePicker Control
		{
			get
			{
				return (System.Windows.Forms.DateTimePicker)base.Control;
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
            
            //sandhra.prakash@siemens.com: Check to validate control
            System.Windows.Forms.DateTimePicker dtPicker = editorControl as System.Windows.Forms.DateTimePicker;
            if(dtPicker == null)
                return;
			dtPicker.Font = cellContext.Cell.View.Font;
		}
		/// <summary>
		/// Set the specified value in the current editor control.
		/// </summary>
		/// <param name="editValue"></param>
		public override void SetEditValue(object editValue)
		{
            //sandhra.prakash@siemens.com: Check to validate control
            if(Control == null)
                return;
			if (editValue is DateTime)
				Control.Value = (DateTime)editValue;
            else if (editValue == null)
                Control.Checked = false;
            else
                throw new SourceGridException("Invalid edit value, expected DateTime");
		}
		/// <summary>
		/// Returns the value inserted with the current editor control
		/// </summary>
		/// <returns></returns>
		public override object GetEditedValue()
		{
            //sandhra.prakash@siemens.com: Check to validate control
            if (Control != null && Control.Checked)
                return Control.Value;
            else
                return null;
		}

        protected override void OnSendCharToEditor(char key)
        {
            //No implementation
        }
	}
}

