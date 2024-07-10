using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowsFormsSample.GridSamples
{
	/// <summary>
	/// Summary description for frmSample25.
	/// </summary>
	[Sample("SourceGrid - Generic Samples", 25, "Sample MDI Application - Log Grid")]
	public class frmSample25 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panelBottom;
		private SourceGrid.Grid grid;
		// TODO MainMenu is no longer supported. Use MenuStrip instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
		private System.Windows.Forms.MenuStrip mainMenu1;
		// TODO MenuItem is no longer supported. Use ToolStripMenuItem instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
		private System.Windows.Forms.ToolStripMenuItem mnWindow;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSample25()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelBottom = new System.Windows.Forms.Panel();
			this.grid = new SourceGrid.Grid();
			// TODO MainMenu is no longer supported. Use MenuStrip instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
			this.mainMenu1 = new System.Windows.Forms.MenuStrip();
			// TODO MenuItem is no longer supported. Use ToolStripMenuItem instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
			this.mnWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelBottom.Controls.Add(this.grid);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 342);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(772, 128);
			this.panelBottom.TabIndex = 1;
			// 
			// grid
			// 
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grid.AutoStretchColumnsToFitWidth = false;
			this.grid.AutoStretchRowsToFitHeight = false;
			this.grid.BackColor = System.Drawing.SystemColors.Window;
			this.grid.CustomSort = false;
			this.grid.Location = new System.Drawing.Point(0, 4);
			this.grid.Name = "grid";
			this.grid.OverrideCommonCmdKey = true;
			this.grid.Size = new System.Drawing.Size(768, 120);
			this.grid.SpecialKeys = SourceGrid.GridSpecialKeys.Default;
			this.grid.TabIndex = 0;
			// 
			// mainMenu1
			// 
			// TODO MenuItem is no longer supported. Use ToolStripMenuItem instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
												this.mainMenu1.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
																					  this.mnWindow});
			// 
			// mnWindow
			// 
			//this.mnWindow.Index = 0;
			this.mnWindow.Text = "Window";
			// 
			// frmSample25
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(772, 470);
			this.Controls.Add(this.panelBottom);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.mainMenu1;
			this.Name = "frmSample25";
			this.Text = "frmSample25";
			this.Load += new System.EventHandler(this.frmSample25_Load);
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void AddLog(string type, string description)
		{
			int row = grid.RowsCount;
			grid.Rows.Insert(row);
			grid[row, 0] = new SourceGrid.Cells.Cell(DateTime.Now);
			grid[row, 1] = new SourceGrid.Cells.Cell(type);
			grid[row, 2] = new SourceGrid.Cells.Cell(description);

			grid.Selection.ResetSelection(false);
			grid.Selection.SelectRow(row, true);
		}

		private void frmSample25_Load(object sender, System.EventArgs e)
		{
			grid.Redim(1, 3);

            grid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            grid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            grid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            grid.FixedRows = 1;

			grid[0, 0] = new SourceGrid.Cells.ColumnHeader("Date");
			grid[0, 1] = new SourceGrid.Cells.ColumnHeader("Type");
			grid[0, 2] = new SourceGrid.Cells.ColumnHeader("Description");

			grid.AutoStretchColumnsToFitWidth = true;
			grid.SelectionMode = SourceGrid.GridSelectionMode.Row;

			AddLog("Log", "Application Started");


			//Create Data Cells
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			Type[] assemblyTypes = assembly.GetTypes();
			for (int i = 0; i < assemblyTypes.Length; i++)
			{
				object[] attributes = assemblyTypes[i].GetCustomAttributes(typeof(SampleAttribute), true);
				if (attributes != null && attributes.Length > 0)
				{
					SampleAttribute sampleAttribute = (SampleAttribute)attributes[0];

					AddLog("Log Menu", "Add menu for " + sampleAttribute.Description + " " + assemblyTypes[i].Name);

					if (assemblyTypes[i] != this.GetType())
					{
                        // TODO MenuItem is no longer supported. Use ToolStripMenuItem instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
                        ToolStripMenuItem menu = new MenuForm(this, assemblyTypes[i], sampleAttribute.Description + " " + sampleAttribute.SampleNumber.ToString());
						mnWindow.DropDownItems.Add(menu);
					}
				}
			}

            for (int i = 0; i < 200; i++)
                AddLog("Log", "Application test " + i.ToString());

            grid.Columns.AutoSize(true);
            grid.Columns.StretchToFit();
        }


        private class MenuForm : // TODO MenuItem is no longer supported. Use ToolStripMenuItem instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
ToolStripMenuItem
		{
			private Type mFrm;
			private Form mParent;
			public MenuForm(Form parent, Type frm, string description):base("Add Form " + description)
			{
				mParent = parent;
				mFrm = frm;
			}

			protected override void OnClick(EventArgs e)
			{
				base.OnClick (e);
				Form frm = (Form)Activator.CreateInstance(mFrm);
				frm.MdiParent = mParent;
				frm.Show();
			}

		}
	}
}
