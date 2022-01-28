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
 * 1. DropDownControl(System.Windows.Forms.Control control) : sandhra.prakash@siemens.com : 
        Added Property DropDownFlags which will allow the user to set the 
        DropDown.DropDownFlags property.
 * 2. ShowDialog, DropDownControl, DropDownClosed: To notify the user if the dropdown was closed due to a
 * (pressing Escape key)cancel action or by changing focus or pressing Enter key which corresponds to an accept action.
 * 3. DropDownControl(System.Windows.Forms.Control control) : In Syscon, on multiple open and close operations, 
      at times, due to Application.DoEvents in OnDropDownOpen of DropDown.cs, m_dropDown becomes null
*/
#endregion Copyright

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;
using System.Drawing.Design;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// A TextBoxTypedButton that uase the UITypeEditor associated with the type.
	/// </summary>
	public class TextBoxUITypeEditor : DevAgeTextBoxButton, IServiceProvider, System.Windows.Forms.Design.IWindowsFormsEditorService, ITypeDescriptorContext
	{
		private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// sandhra.prakash@siemens.com : The default Dropdown flag is CloseOnEscape.
        /// As per the previous version of this code,
        /// </summary>
        DropDownFlags m_DropDownFlags = DropDownFlags.CloseOnEscape;

		public TextBoxUITypeEditor()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

        /// <summary>
        /// Gets or sets the Dropdown Flags
        /// sandhra.prakash@siemens.com : This Property was introduced so that user will
        /// have the option to decide on which action the dropdown should close.
        /// </summary>
        public DropDownFlags DropDownFlags 
        {
            get { return m_DropDownFlags; }
            set { m_DropDownFlags = value; }
        }

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		public override void ShowDialog()
		{
			try
			{
				OnDialogOpen(EventArgs.Empty);
				if (m_UITypeEditor != null)
				{
					UITypeEditorEditStyle style = m_UITypeEditor.GetEditStyle();
                    if (style == UITypeEditorEditStyle.DropDown ||
                        style == UITypeEditorEditStyle.Modal)
					{
						object editObject;
                        //Try to read the actual value, if the function failed I edit the default value
                        if (IsValidValue(out editObject) == false)
                        {
                            if (Validator != null)
                                editObject = Validator.DefaultValue;
                            else
                                editObject = null;
                        }

                        object tmp = m_UITypeEditor.EditValue(this, this, editObject);
						Value = tmp;
					}
				}
                //sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a
                // (pressing Escape key)cancel action or by changing focus or pressing Enter key which corresponds to an accept action.
                DropDownClosedEventArgs eventArgs = new DropDownClosedEventArgs(DropDownCancelled);
                OnDialogClosed(eventArgs);
			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message,"Error");
			}
		}

		private UITypeEditor m_UITypeEditor;

        /// <summary>
        /// Gets or sets the UITypeEditor to use. If you have specified a validator the TypeDescriptor.GetEditor method is used based on the Validator.ValueType.
        /// </summary>
        [DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UITypeEditor UITypeEditor
		{
			get{return m_UITypeEditor;}
			set{m_UITypeEditor = value;}
		}

        //public bool ShouldSerializeUITypeEditor()
        //{
        //    return m_UITypeEditor != m_DefaultUITypeEditor;
        //}

        protected override void ApplyValidatorRules()
        {
            base.ApplyValidatorRules();

            if (m_UITypeEditor == null && Validator != null)
            {
                object tmp = System.ComponentModel.TypeDescriptor.GetEditor(Validator.ValueType, typeof(UITypeEditor));
                if (tmp is UITypeEditor)
                    m_UITypeEditor = (UITypeEditor)tmp;
            }
        }

		#region IServiceProvider Members
		System.Object IServiceProvider.GetService ( System.Type serviceType )
		{
			//modal
			if (serviceType == typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))
				return this;

			return null;
		}
		#endregion

		#region System.Windows.Forms.Design.IWindowsFormsEditorService
		private DevAge.Windows.Forms.DropDown m_dropDown = null;

	    public virtual void CloseDropDown ()
		{
			if (m_dropDown != null)
			{
				m_dropDown.CloseDropDown();
			}
		}

		public virtual void DropDownControl ( System.Windows.Forms.Control control )
		{
            using (m_dropDown = new DevAge.Windows.Forms.DropDown(control, this, this.ParentForm))
            {
                // sandhra.prakash@siemens.com : The default Dropdown flag is CloseOnEscape.
                // in this we give user the option to select the dropdown flag.
                m_dropDown.DropDownFlags = DropDownFlags;

                //sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a
                // (pressing Escape key)cancel action or by changing focus or pressing Enter key which corresponds to an accept action.
                m_dropDown.DropDownClosed += DropDownClosed;
                m_dropDown.ShowDropDown();

                //sandhra.prakash@siemens.com:In Syscon, on multiple open and close operations, 
                //at times, due to Application.DoEvents in OnDropDownOpen of DropDown.cs, m_dropDown becomes null
                if (m_dropDown != null)
                {
                    //sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a
                    // (pressing Escape key)cancel action or by changing focus or pressing Enter key which corresponds to an accept action.
                    m_dropDown.DropDownClosed -= DropDownClosed;
                    m_dropDown.Close();
                }
            }
            m_dropDown = null;
        }

        /// <summary>
        /// sandhra.prakash@siemens.com: To notify the user if the dropdown was closed due to a
        /// (pressing Escape key)cancel action or by changing focus or pressing Enter key which corresponds to an accept action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DropDownClosed(object sender, DropDownClosedEventArgs e)
        {
            DropDownCancelled = e.Cancelled;
        }

		public virtual System.Windows.Forms.DialogResult ShowDialog ( System.Windows.Forms.Form dialog )
		{
			return dialog.ShowDialog(this);
		}
		#endregion

		#region ITypeDescriptorContext Members

		void ITypeDescriptorContext.OnComponentChanged()
		{
			
		}

		IContainer ITypeDescriptorContext.Container
		{
			get
			{
				return base.Container;
			}
		}

		bool ITypeDescriptorContext.OnComponentChanging()
		{
			return true;
		}

		object ITypeDescriptorContext.Instance
		{
			get
			{
				return Value;
			}
		}

		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
		{
			get
			{
				return null;
			}
		}

		#endregion
	}
}

