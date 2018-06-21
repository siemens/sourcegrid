using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowsFormsSample
{
	/// <summary>
	/// Summary description for frmSample14.
	/// </summary>
	[Sample("SourceGrid - Standard features", 21, "ColumnSpan and RowSpan")]
	public class frmSample21 : System.Windows.Forms.Form
	{
		private SourceGrid.Grid grid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSample21()
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
			this.grid1 = new SourceGrid.Grid();
			this.SuspendLayout();
			// 
			// grid1
			// 
			this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grid1.Location = new System.Drawing.Point(8, 8);
			this.grid1.Name = "grid1";
			this.grid1.Size = new System.Drawing.Size(612, 423);
			this.grid1.SpecialKeys = SourceGrid.GridSpecialKeys.Default;
			this.grid1.TabIndex = 0;
			// 
			// frmSample21
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(628, 438);
			this.Controls.Add(this.grid1);
			this.Name = "frmSample21";
			this.Text = "ColumnSpan and RowSpan";
			this.Load += new System.EventHandler(this.frmSample14_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmSample14_Load(object sender, System.EventArgs e)
		{
			grid1.Redim(20, 10);
			grid1.FixedRows = 2;
			
			//1 Header Row
			grid1[0, 0] = new MyHeader("3 Column Header");
			grid1[0, 0].ColumnSpan = 3;
			grid1[0, 3] = new MyHeader("5 Column Header");
			grid1[0, 3].ColumnSpan = 5;
			grid1[0, 8] = new MyHeader("1 Column Header");
			grid1[0, 9] = new MyHeader("1 Column Header");

			//2 Header Row
			grid1[1, 0] = new MyHeader("1");
			grid1[1, 1] = new MyHeader("2");
			grid1[1, 2] = new MyHeader("3");
			grid1[1, 3] = new MyHeader("4");
			grid1[1, 4] = new MyHeader("5");
			grid1[1, 5] = new MyHeader("6");
			grid1[1, 6] = new MyHeader("7");
			grid1[1, 7] = new MyHeader("8");
			grid1[1, 8] = new MyHeader("9");
			grid1[1, 9] = new MyHeader("10");

			SourceGrid.Cells.Views.Cell viewImage = new SourceGrid.Cells.Views.Cell();

			SourceGrid.Cells.Controllers.CustomEvents clickEvent = new SourceGrid.Cells.Controllers.CustomEvents();
			clickEvent.Click += new EventHandler(clickEvent_Click);

			for (int r = 2; r < grid1.RowsCount; r=r+2)
			{
				grid1[r, 0] = new SourceGrid.Cells.Cell(r.ToString(), typeof(string));
				grid1[r, 0].ColumnSpan = 2;

				grid1[r+1, 0] = new SourceGrid.Cells.Cell(DateTime.Today.AddDays(r), typeof(DateTime));
				grid1[r+1, 0].ColumnSpan = 2;

				grid1[r, 2] = new SourceGrid.Cells.CheckBox("CheckBox Column/Row Span", false);
				grid1[r, 2].ColumnSpan = 2;
				grid1[r, 2].RowSpan = 2;

				grid1[r, 4] = new SourceGrid.Cells.Link("Link Column/Row Span");
				grid1[r, 4].ColumnSpan = 2;
				grid1[r, 4].RowSpan = 2;
				grid1[r, 4].AddController(clickEvent);

				grid1[r, 6] = new SourceGrid.Cells.Button("Button Column/Row Span");
				grid1[r, 6].ColumnSpan = 2;
				grid1[r, 6].RowSpan = 2;
				grid1[r, 6].AddController(clickEvent);

				grid1[r, 8] = new SourceGrid.Cells.Cell("Image Column/Row Span");
				grid1[r, 8].View = viewImage;
				grid1[r, 8].Image = Properties.Resources.FACE02.ToBitmap();
				grid1[r, 8].ColumnSpan = 2;
				grid1[r, 8].RowSpan = 2;
			}

            grid1.AutoSizeCells();
		}

		private class MyHeader : SourceGrid.Cells.ColumnHeader
		{
			public MyHeader(object value):base(value)
			{
				//1 Header Row
				SourceGrid.Cells.Views.ColumnHeader view = new SourceGrid.Cells.Views.ColumnHeader();
				view.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
				view.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
				View = view;

				AutomaticSortEnabled = false;
			}
		}

		private void clickEvent_Click(object sender, EventArgs e)
		{
			SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
			MessageBox.Show(this, context.Position.ToString());
		}
	}
}
