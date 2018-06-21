
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
 1. chinnari.prasad@siemens.com : PrepareView(CellContext context) : For disabled and readonly button style should be disabled. Enhancement : Adding button to cell
*/
#endregion Copyright

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SourceGrid.Cells.Views
{
	/// <summary>
	/// Summary description for a 3D themed Button.
	/// </summary>
	[Serializable]
	public class Button : Cell
	{
		/// <summary>
		/// Represents a Button with the ability to draw an Image. Disable also the selection border using the OwnerDrawSelectionBorder = true.
		/// </summary>
		public new readonly static Button Default;

		#region Constructors

		static Button()
		{
			Default = new Button();
		}

		/// <summary>
		/// Use default setting
		/// </summary>
		public Button()
		{
            Background = new DevAge.Drawing.VisualElements.ButtonThemed();
		}

		/// <summary>
		/// Copy constructor.  This method duplicate all the reference field (Image, Font, StringFormat) creating a new instance.
		/// </summary>
		/// <param name="p_Source"></param>
		public Button(Button p_Source):base(p_Source)
		{
            Background = (DevAge.Drawing.VisualElements.IButton)p_Source.Background.Clone();
		}
		#endregion

		#region Clone
		/// <summary>
		/// Clone this object. This method duplicate all the reference field (Image, Font, StringFormat) creating a new instance.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new Button(this);
		}
		#endregion

        #region Visual Elements

        public new DevAge.Drawing.VisualElements.IButton Background
        {
            get { return (DevAge.Drawing.VisualElements.IButton)base.Background; }
            set { base.Background = value; }
        }

        protected override void PrepareView(CellContext context)
        {
            
            base.PrepareView(context);

            //chinnari.prasad@siemens.com : For disabled and readonly button style should be disabled
            if (context.Cell.Editor != null && (!context.Cell.Editor.EnableEdit || context.Cell.Editor.ReadOnly))
                Background.Style = DevAge.Drawing.ButtonStyle.Disabled;
            else if (context.CellRange.Contains(context.Grid.MouseDownPosition))
                Background.Style = DevAge.Drawing.ButtonStyle.Pressed;
            else if (context.CellRange.Contains(context.Grid.MouseCellPosition))
                Background.Style = DevAge.Drawing.ButtonStyle.Hot;
            else if (context.CellRange.Contains(context.Grid.Selection.ActivePosition))
                Background.Style = DevAge.Drawing.ButtonStyle.Focus;
            else
                Background.Style = DevAge.Drawing.ButtonStyle.Normal;
        }
        #endregion
	}
}
