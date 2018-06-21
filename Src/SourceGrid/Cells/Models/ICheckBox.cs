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
 * 1. sandhra.prakash@siemens.com:Constructors and new readonly field: CheckBox status should reflect whether it is readonly or not.
*/

#endregion Copyright
using System;

namespace SourceGrid.Cells.Models
{
	/// <summary>
	/// Interface for informations about a cechkbox
	/// </summary>
	public interface ICheckBox : IModel
	{
		/// <summary>
		/// Get the status of the checkbox at the current position
		/// </summary>
		/// <param name="cellContext"></param>
		/// <returns></returns>
		CheckBoxStatus GetCheckBoxStatus(CellContext cellContext);
		
		/// <summary>
		/// Set the checked value
		/// </summary>
		/// <param name="cellContext"></param>
		/// <param name="pChecked">True, False or Null</param>
		void SetCheckedValue(CellContext cellContext, bool? pChecked);
	}

    /// <summary>
    /// Status of the CheckBox
    /// </summary>
	public struct CheckBoxStatus
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="checkEnable"></param>
        /// <param name="bChecked"></param>
        /// <param name="caption"></param>
        /// <param name="readOnly"></param>
        public CheckBoxStatus(bool checkEnable, bool? bChecked, string caption, bool readOnly)
		{
            //sandhra.prakash@siemens.com:Constructors and new readonly field: CheckBox status should reflect whether it is readonly or not.
            ReadOnly = readOnly;
			CheckEnable = checkEnable;
            Caption = caption;

            mCheckState = DevAge.Drawing.CheckBoxState.Undefined;
            Checked = bChecked;

            
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="checkEnable"></param>
        /// <param name="checkState"></param>
        /// <param name="caption"></param>
        /// <param name="readOnly"></param>
        public CheckBoxStatus(bool checkEnable, DevAge.Drawing.CheckBoxState checkState, string caption, bool readOnly)
		{
            //sandhra.prakash@siemens.com:Constructors and new readonly field: CheckBox status should reflect whether it is readonly or not.
            ReadOnly = readOnly;
			CheckEnable = checkEnable;
			mCheckState = checkState;
			Caption = caption;
		}

        private DevAge.Drawing.CheckBoxState mCheckState;
		/// <summary>
		/// Gets or sets the state of the check box.
		/// </summary>
		public DevAge.Drawing.CheckBoxState CheckState
		{
			get{return mCheckState;}
			set{mCheckState = value;}
		}

		/// <summary>
		/// Enable or disable the checkbox
		/// </summary>
		public bool CheckEnable;

		/// <summary>
        /// Gets or set Checked status. Return true for Checked, false for Uncheck and null for Undefined
		/// </summary>
		public bool? Checked
		{
			get
			{
                if (CheckState == DevAge.Drawing.CheckBoxState.Checked)
                    return true;
                else if (CheckState == DevAge.Drawing.CheckBoxState.Unchecked)
                    return false;
                else
                    return null;
			}
			set
			{
                if (value == null)
                    CheckState = DevAge.Drawing.CheckBoxState.Undefined;
				else if (value.Value)
                    CheckState = DevAge.Drawing.CheckBoxState.Checked;
				else
                    CheckState = DevAge.Drawing.CheckBoxState.Unchecked;
			}
		}

        //sandhra.prakash@siemens.com:Constructors and new readonly field: CheckBox status should reflect whether it is readonly or not.
        public bool ReadOnly;

        /// <summary>
		/// Caption of the CheckBox
		/// </summary>
		public string Caption;
	}
}


