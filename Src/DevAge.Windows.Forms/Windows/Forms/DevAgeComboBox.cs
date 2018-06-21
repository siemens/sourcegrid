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
 * 1. OnCreateControl() : Restricted the MaxDropDownItems to 12.
 * 2. WndProc(ref Message m) : for selectable ReadOnly cell enhancement,
 *      Fires when drop down opens. Introduced events, so that user can define dropdown location.
*/
#endregion Copyright

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using SourceGrid.SourceGrid.Cells.Editors;

namespace DevAge.Windows.Forms
{
    /// <summary>
    /// DevAgeComboBox has a typed Value property and the validating features using the Validator property.
    /// Set the Validator property and then call the ApplyValidatorRules method.
    /// </summary>
    public class DevAgeComboBox : System.Windows.Forms.ComboBox //, SourceGrid.SourceGrid.Cells.Editors.IControlEdit
    {

        #region Generic validation methods
        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            object val;
            if (IsValidValue(out val) == false)
            {
                e.Cancel = true;
            }
            else
            {
                if (FormatValue && Validator != null)
                {
                    Text = Validator.ValueToDisplayString(val);
                }
            }
        }

        private bool mFormatValue = false;
        /// <summary>
        /// Gets or sets a property to enable or disable the automatic format of the Text when validating the control.
        /// Default false.
        /// </summary>
        [DefaultValue(false)]
        public bool FormatValue
        {
            get { return mFormatValue; }
            set { mFormatValue = value; }
        }

        private DevAge.ComponentModel.Validator.IValidator mValidator = null;
        /// <summary>
        /// Gets or sets the Validator class useded to validate the value and convert the text when using the Value property.
        /// You can use the ApplyValidatorRules method to apply the settings of the Validator directly to the ComboBox, for example the list of values.
        /// </summary>
        [DefaultValue(null)]
        public DevAge.ComponentModel.Validator.IValidator Validator
        {
            get { return mValidator; }
            set
            {
                if (mValidator != value)
                {
                    if (mValidator != null)
                        mValidator.Changed -= mValidator_Changed;

                    mValidator = value;
                    mValidator.Changed += mValidator_Changed;
                    ApplyValidatorRules();
                }
            }
        }

        void mValidator_Changed(object sender, EventArgs e)
        {
            ApplyValidatorRules();
        }

        ///// <summary>
        ///// Apply the current Validator rules. This method is automatically fired when the Validator change.
        ///// </summary>
        //protected virtual void ApplyValidatorRules()
        //{

        //}

        /// <summary>
        /// Check if the selected value is valid based on the current validator and returns the value.
        /// </summary>
        /// <param name="convertedValue"></param>
        /// <returns></returns>
        public bool IsValidValue(out object convertedValue)
        {
            //Note:
            // SelectedValue is only valid when data binding is active otherwise
            // you must use SelectedItem

            object valToCheck;
            if (this.SelectedValue != null)
                valToCheck = this.SelectedValue;
            else if (this.SelectedItem != null)
                valToCheck = this.SelectedItem;
            else
                valToCheck = this.Text;

            if (Validator != null)
            {
                if (Validator.IsValidObject(valToCheck, out convertedValue))
                    return true;
                else
                    return false;
            }
            else
            {
                convertedValue = valToCheck;
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the typed value for the control, using the Validator class.
        /// If the Validator is ull the Text property is used.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Value
        {
            get
            {
                object val;
                if (IsValidValue(out val))
                    return val;
                else
                    throw new ArgumentOutOfRangeException("Text");
            }
            set
            {
                if (Validator != null)
                {
                    Text = Validator.ValueToDisplayString(value);
                }
                else
                {
                    if (value == null)
                        Text = "";
                    else
                        Text = value.ToString();
                }
            }
        }

        #endregion

        /// <summary>
        /// Loads the Items from the StandardValues and the DropDownStyle based on the parameters of the validator.
        /// Apply the current Validator rules. This method is automatically fired when the Validator change.
        /// </summary>
        protected virtual void ApplyValidatorRules()
        {
            Items.Clear();
            if (Validator != null && Validator.StandardValues != null)
            {
                foreach (object val in Validator.StandardValues)
                    Items.Add(val);

                if (Validator.IsStringConversionSupported())
                    DropDownStyle = ComboBoxStyle.DropDown;
                else
                    DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        protected override void OnFormat(ListControlConvertEventArgs e)
        {
            base.OnFormat(e);

            // The method converts only to string type. 
            if (e.DesiredType != typeof(string) || Validator == null)
                return;

            e.Value = Validator.ValueToDisplayString(e.ListItem);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            IntegralHeight = false;
            MaxDropDownItems = 12;
        }

        private const UInt32 WM_CTLCOLORLISTBOX = 0x0134;
        /// <summary>
        /// sandhra.prakash@siemens.com:for selectable ReadOnly cell enhancement, Fires when drop down opens
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CTLCOLORLISTBOX)
            {
                if (DropDownOpened != null)
                    DropDownOpened(m);
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// sandhra.prakash@siemens.com: Introduced events for selectable ReadOnly cell enhancement. To define dropdown location
        /// </summary>
        internal event OnDropDownOpened DropDownOpened;

        internal delegate void OnDropDownOpened(Message message);

    }
}
