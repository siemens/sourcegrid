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
 * 1. DropDownCancelled, ShowDialog, DialogClosed, OnDialogClosed - To notify the user if the dropdown was closed due to a
 * (pressing Escape key)cancel action or by changing focus or pressing Enter key which corresponds to an accept action.
*/
#endregion Copyright

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// A textBox with a button on the right.
	/// </summary>
	public class DevAgeTextBoxButton : EditableControlBase
	{
		private System.Windows.Forms.Button btDown;
		private System.Windows.Forms.TextBox txtBox;

        /// <summary>
        /// sandhra.prakash@siemens.com: added a field that will notify the user if the dropdown was closed due to a (pressing Escape key)cancel action
        /// or by changing focus or pressing Enter key which corresponds to an accept action
        /// </summary>
        protected bool DropDownCancelled;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public DevAgeTextBoxButton()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			btDown.BackColor = Color.FromKnownColor(KnownColor.Control);
            txtBox.TextChanged += new EventHandler(txtBox_TextChanged);

			SetContentAndButtonLocation(txtBox, btDown);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btDown = new System.Windows.Forms.Button();
			this.txtBox = new DevAge.Windows.Forms.DevAgeTextBox();
			this.SuspendLayout();
			// 
			// btDown
			// 
			this.btDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btDown.Location = new System.Drawing.Point(134, 2);
			this.btDown.Name = "btDown";
			this.btDown.Size = new System.Drawing.Size(24, 16);
			this.btDown.TabIndex = 1;
			this.btDown.Text = "...";
			this.btDown.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.btDown.Click += new System.EventHandler(this.btDown_Click);
			// 
			// txtBox
			// 
			this.txtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtBox.AutoSize = false;
			this.txtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtBox.HideSelection = false;
			this.txtBox.Location = new System.Drawing.Point(2, 2);
			this.txtBox.Name = "txtBox";
			this.txtBox.Size = new System.Drawing.Size(132, 16);
			this.txtBox.TabIndex = 0;
			this.txtBox.WordWrap = false;
			// 
			// TextBoxTypedButton
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.txtBox);
			this.Controls.Add(this.btDown);
			this.Name = "TextBoxTypedButton";
			this.Size = new System.Drawing.Size(160, 20);
			this.ResumeLayout(false);

		}
		#endregion


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

        /// <summary>
        /// Apply the current Validator rules. This method is automatically fired when the Validator change.
        /// </summary>
        protected virtual void ApplyValidatorRules()
        {
        }

		/// <summary>
		/// Show the dialog
		/// </summary>
		public virtual void ShowDialog()
		{
			OnDialogOpen(EventArgs.Empty);

            //sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a (pressing Escape key)cancel action
            //or by changing focus or pressing Enter key which corresponds to an accept action
            DropDownClosedEventArgs eventArgs = new DropDownClosedEventArgs(DropDownCancelled);
            OnDialogClosed(eventArgs);
		}

		private void btDown_Click(object sender, System.EventArgs e)
		{
			ShowDialog();
		}

		/// <summary>
		/// The button in the right of the editor
		/// </summary>
		public System.Windows.Forms.Button Button
		{
			get{return btDown;}
		}

		public System.Windows.Forms.TextBox TextBox
		{
			get{return txtBox;}
		}

		/// <summary>
		/// Fired when showing the drop down
		/// </summary>
		public event EventHandler DialogOpen;

		/// <summary>
		/// Fired when showing the drop down
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDialogOpen(EventArgs e)
		{
			if (DialogOpen!=null)
				DialogOpen(this,e);
		}

		/// <summary>
		/// Fired when closing the dropdown
		/// sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a (pressing Escape key)cancel action
        /// or by changing focus or pressing Enter key which corresponds to an accept action.
		/// </summary>
		public event EventHandler<DropDownClosedEventArgs> DialogClosed;

		/// <summary>
		/// Fired when closing the dropdown
        /// sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a (pressing Escape key)cancel action
        /// or by changing focus or pressing Enter key which corresponds to an accept action. 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDialogClosed(DropDownClosedEventArgs e)
		{
			if (DialogClosed!=null)
				DialogClosed(this,e);
		}

		protected override void OnBorderStyleChanged(EventArgs e)
		{
			base.OnBorderStyleChanged (e);
			SetContentAndButtonLocation(txtBox, btDown);
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged (e);

			if (txtBox != null)
			{
				if (BackColor == Color.Transparent)
					txtBox.BackColor = Color.FromKnownColor(KnownColor.Window);
				else
					txtBox.BackColor = BackColor;
			}
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged (e);

			if (txtBox != null)
				txtBox.ForeColor = ForeColor;
		}


        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            object val;
            if (IsValidValue(out val) == false)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Check if the selected value is valid based on the current validator and returns the value.
        /// </summary>
        /// <param name="convertedValue"></param>
        /// <returns></returns>
        public bool IsValidValue(out object convertedValue)
        {
            if (Validator != null)
            {
                if (mValue != null)
                {
                    convertedValue = mValue;
                    return true;
                }
                else
                {
                    if (Validator.IsValidObject(TextBox.Text, out mValue))
                    {
                        convertedValue = mValue;
                        return true;
                    }
                    else
                    {
                        convertedValue = null;
                        return false;
                    }
                }
            }
            else
            {
                convertedValue = txtBox.Text;
                return true;
            }
        }

        private object mValue = null;

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
                    if (Validator.IsStringConversionSupported())
                        TextBox.Text = Validator.ValueToString(value);
                    else
                        TextBox.Text = Validator.ValueToDisplayString(value);
                }
                else
                {
                    if (value == null)
                        TextBox.Text = "";
                    else
                        TextBox.Text = value.ToString();
                }

                mValue = value;
            }
        }

        void txtBox_TextChanged(object sender, EventArgs e)
        {
            mValue = null;
        }
	}
}
