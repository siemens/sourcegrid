#region Copyright

//------------------------------------------------------------------------ 
// Copyright (C) Siemens AG 2017    
//------------------------------------------------------------------------ 
// Project           : UIGrid
// Author            : Sandhra.Prakash@siemens.com
// In Charge for Code: Sandhra.Prakash@siemens.com
//------------------------------------------------------------------------ 

#endregion Copyright

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
    public class UIComboBox : EditableControlBase
    {
        #region Constants

        private const int SwpNoSize = 0x1;

        #endregion Constants
        
        #region Fields

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint uFlags);

        readonly TextBox m_ReadOnlyTextBox = new TextBox();
        readonly DevAgeComboBox m_ComboBox = new DevAgeComboBox();
        private bool m_ReadOnly;
        
        #endregion Fields

        #region Constructor\dispose

        public UIComboBox()
        {
            Controls.Add(m_ComboBox);
            Controls.Add(m_ReadOnlyTextBox);
            m_ReadOnlyTextBox.Visible = false;
            m_ReadOnlyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            m_ReadOnlyTextBox.AutoSize = false;
            //sandhra.prakash@siemens.com: To properly align the text and to hide the border of combobox location is set beyond the visible area
            m_ComboBox.Location = new Point(-1, -2);
            m_ComboBox.TextChanged += ComboBoxTextChanged;
            ClientSizeChanged += OnClientSizeChanged;
            m_ComboBox.DropDownOpened += ComboBoxOnDropDownOpened;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_ComboBox.TextChanged -= ComboBoxTextChanged;
                ClientSizeChanged -= OnClientSizeChanged;
                m_ComboBox.DropDownOpened -= ComboBoxOnDropDownOpened;
            }
            base.Dispose(disposing);
        }

        #endregion Constructor

        #region Overridden Methods

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (m_ReadOnlyTextBox != null)
            {
                if (BackColor == Color.Transparent)
                {
                    m_ReadOnlyTextBox.BackColor = Color.FromKnownColor(KnownColor.Window);
                    m_ComboBox.BackColor = Color.FromKnownColor(KnownColor.Window);
                }
                else
                {
                    m_ReadOnlyTextBox.BackColor = BackColor;
                    m_ComboBox.BackColor = BackColor;
                }
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            if (m_ReadOnlyTextBox != null)
            {
                m_ReadOnlyTextBox.ForeColor = ForeColor;
                m_ComboBox.ForeColor = ForeColor;
            }
        }

        #endregion Overridden Methods

        #region Properties

        public DevAgeComboBox ComboBox
        {
            get
            {
                return m_ComboBox;

            }
        }


        public HorizontalAlignment TextAlignment { get; set; }

        public bool ReadOnly
        {
            get { return m_ReadOnly; }
            set
            {
                m_ReadOnly = value;
                if (value)
                {
                    m_ReadOnlyTextBox.ReadOnly = true;
                    m_ReadOnlyTextBox.Visible = true;
                    m_ReadOnlyTextBox.ContextMenuStrip = m_ComboBox.ContextMenuStrip;
                    m_ComboBox.SendToBack();
                    m_ComboBox.Enabled = false;
                }
                else
                {
                    m_ReadOnlyTextBox.ReadOnly = false;
                    m_ReadOnlyTextBox.Visible = false;
                    m_ComboBox.Enabled = true;
                    m_ComboBox.BringToFront();
                }
                SetChildControlProperites();
            }
        }

        public bool IsButtonVisibleOnReadOnly { get; set; }

        #endregion Properties

        #region Private Methods

        private void ComboBoxTextChanged(object sender, EventArgs e)
        {
            SetReadOnlyText();
        }

        private void SetReadOnlyText()
        {
            m_ReadOnlyTextBox.Text = m_ComboBox.Text;
            m_ReadOnlyTextBox.SelectAll();
        }
        private void SetChildControlProperites()
        {
            m_ComboBox.Width = Width;

            m_ReadOnlyTextBox.Size = Size;
            const int dropDownWidth = 18;
            if (IsButtonVisibleOnReadOnly && (m_ComboBox.DropDownStyle == ComboBoxStyle.DropDown || m_ComboBox.DropDownStyle == ComboBoxStyle.DropDownList))
                m_ReadOnlyTextBox.Width -= dropDownWidth + m_ReadOnlyTextBox.Left;
            if(ReadOnly)
            {
                //This is to avoid the condition: set the column size less than length of text.
                //now click on the cell and go into editmode.. Now the cursor is in the last position.
                //now resize the column to make the complete text visible. Again click on the cell and go into editmode.
                //Verify if the complete text is visible.
                m_ReadOnlyTextBox.Text = string.Empty;
                SetReadOnlyText();
                m_ReadOnlyTextBox.TextAlign = TextAlignment;
            }
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            SetChildControlProperites();
        }
        
        private void ComboBoxOnDropDownOpened(Message message)
        {
            var bottomLeft = PointToScreen(new Point(0, Height));
            var x = bottomLeft.X;
            var y = bottomLeft.Y + 1;
            SetWindowPos(message.LParam, IntPtr.Zero, x, y, 0, 0, SwpNoSize);
        }

        #endregion Private Methods

    }
}