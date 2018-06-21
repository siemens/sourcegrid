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
 1. Validation for Control added in SetEditValue, GetEditedValue
*/
#endregion Copyright

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace SourceGrid.Cells.Editors
{
	/// <summary>
	///  A model that use a TextBoxButton for Image editing, allowing to select a source image file. Returns null as DisplayString. Write and read byte[] values.
	/// </summary>
	[System.ComponentModel.ToolboxItem(false)]
	public class ImagePicker : EditorControlBase
	{
		public readonly static ImagePicker Default = new ImagePicker();
		
		#region Constructor
		/// <summary>
		/// Construct an Editor of type ImagePicker.
		/// </summary>
		public ImagePicker():base(typeof(byte[]))
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
			DevAge.Windows.Forms.TextBoxUITypeEditor editor = new DevAge.Windows.Forms.TextBoxUITypeEditor();
			editor.BorderStyle = DevAge.Drawing.BorderStyle.None;
			editor.Validator = new DevAge.ComponentModel.Validator.ValidatorTypeConverter(typeof(System.Drawing.Image));
			return editor;
		}

		/// <summary>
		/// Gets the control used for editing the cell.
		/// </summary>
		public new DevAge.Windows.Forms.TextBoxUITypeEditor Control
		{
			get
			{
				return (DevAge.Windows.Forms.TextBoxUITypeEditor)base.Control;
			}
		}
		#endregion

		public override object GetEditedValue()
		{
            //sandhra.prakash@siemens.com: check if Control exists
		    if (Control == null)
		    {
                return null;
		    }
		    object val = Control.Value;
		    if (val == null)
		        return null;
		    else if (val is System.Drawing.Image)
		    {
		        DevAge.ComponentModel.Validator.ValidatorTypeConverter imageValidator =
		            new DevAge.ComponentModel.Validator.ValidatorTypeConverter(typeof (System.Drawing.Image));
		        return imageValidator.ValueToObject(val, typeof (byte[]));

		        //Stranamente questo codice in caso di ico va in eccezione!
//				System.Drawing.Image img = (System.Drawing.Image)val;
//				using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
//				{
//					img.Save(memStream, System.Drawing.Imaging.ImageCodecInfo.);
//
//					return memStream.ToArray();
//				}
		    }
		    else if (val is byte[])
		        return val;
		    else
		        throw new SourceGridException("Invalid edited value, expected byte[] or Image");
		}

		public override void SetEditValue(object editValue)
		{
            //sandhra.prakash@siemens.com: check if control exists
            
		    if (Control != null)
		    {
		        Control.Value = editValue;
		        Control.TextBox.SelectAll();
		    }
		}

		/// <summary>
		/// Used to returns the display string for a given value. In this case return null.
		/// </summary>
		/// <param name="p_Value"></param>
		/// <returns></returns>
		public override string ValueToDisplayString(object p_Value)
		{
			return null;
		}

		protected override void OnSendCharToEditor(char key)
		{
			//No implementation
		}
	}
}
