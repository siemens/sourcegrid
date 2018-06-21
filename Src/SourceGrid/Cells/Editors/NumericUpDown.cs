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
 * 1. Constructor and CreateControl:sandhra.prakash@siemens.com : sandhra.prakash@siemens.com:As control creation is not the job of constructor
         * any more, temporary variables are required to store Increment, min and max values 
         * so that can be assigned to control once its created in the CreateControl method
         * Checks added in methods to make sure control exists. 
 * 2. CreateControl() : Custom numeric updown is used to support Selectable readonly text enhancement
*/
#endregion Copyright

using System;
using System.Windows.Forms;
using DevAge.Windows.Forms;

namespace SourceGrid.Cells.Editors
{
	/// <summary>
	/// EditorNumericUpDown editor class.
	/// </summary>
    [System.ComponentModel.ToolboxItem(false)]
    public class NumericUpDown : EditorControlBase
	{
        /*sandhra.prakash@siemens.com:As control creation is not the job of constructor
         * any more, temporary variables are required to store Increment, min and max values 
         * so that can be assigned to control once its created in the CreateControl method*/
        private readonly decimal m_Increment;
	    private readonly decimal m_Minimum;
	    private readonly decimal m_Maximum;

	    /// <summary>
		/// Create a model of type Decimal
		/// </summary>
		public NumericUpDown():base(typeof(decimal))
		{
		}

        public NumericUpDown(Type p_CellType, decimal p_Maximum, decimal p_Minimum, decimal p_Increment)
            : base(p_CellType)
        {
            if (p_CellType == null || p_CellType == typeof(int) ||
                p_CellType == typeof(long) || p_CellType == typeof(decimal))
            {
                m_Maximum = p_Maximum;
                m_Minimum = p_Minimum;
                m_Increment = p_Increment;
            }
            else
                throw new SourceGridException("Invalid CellType expected long, int or decimal");
        }


	    #region Edit Control
		/// <summary>
		/// Create the editor control
		/// </summary>
		/// <returns></returns>
		protected override Control CreateControl()
		{
            //sandhra.prakash@siemens.com: Custom numeric updown is used to support Selectable readonly text enhancement
            UINumericUpDown l_Control = new UINumericUpDown();
			l_Control.BorderStyle = System.Windows.Forms.BorderStyle.None;
            l_Control.Maximum = m_Maximum;
            l_Control.Minimum = m_Minimum;
            l_Control.Increment = m_Increment;
			return l_Control;
		}

		/// <summary>
		/// Gets the control used for editing the cell.
		/// </summary>
        public new UINumericUpDown Control
		{
			get
			{
                return (UINumericUpDown)base.Control;
			}
		}
		#endregion

		/// <summary>
		/// Set the specified value in the current editor control.
		/// </summary>
		/// <param name="editValue"></param>
		public override void SetEditValue(object editValue)
		{
            //sandhra.prakash@siemens.com: check if control exists
            if(Control == null)
                return;

			decimal dec;
			if (editValue is decimal)
				dec = (decimal)editValue;
			else if (editValue is long)
				dec = (decimal)((long)editValue);
			else if (editValue is int)
				dec = (decimal)((int)editValue);
			else if (editValue == null)
				dec = Control.Minimum;
			else
				throw new SourceGridException("Invalid value, expected Decimal, Int or Long");

			//.NET BUG:  First I must get the value, otherwise seems that the control don't work properly (when I hit the Escape so I never getthe value so the control use always the previous value also if I manually set the value)
			decimal oldValue = Control.Value;

			Control.Value = dec;
		}

		/// <summary>
		/// Returns the value inserted with the current editor control
		/// </summary>
		/// <returns></returns>
		public override object GetEditedValue()
		{
            //sandhra.prakash@siemens.com: check if control exists
		    if (Control == null)
		        return null;

			if (ValueType == null)
				return Control.Value;
			if (ValueType == typeof(decimal))
				return Control.Value;
			if (ValueType == typeof(int))
				return (int)Control.Value;
			if (ValueType == typeof(long))
				return (long)Control.Value;

			throw new SourceGridException("Invalid type of the cell expected decimal, long or int");
		}

        protected override void OnSendCharToEditor(char key)
        {
            //No implementation
        }
	}

}
