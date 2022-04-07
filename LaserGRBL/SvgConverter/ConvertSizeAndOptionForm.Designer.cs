/*
 * Created by SharpDevelop.
 * User: Diego
 * Date: 15/01/2017
 * Time: 12:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace LaserGRBL.SvgConverter
{
	partial class SvgToGCodeForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TableLayoutPanel tableMain;
		private System.Windows.Forms.Button BtnCreate;
		private System.Windows.Forms.Button BtnCancel;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvgToGCodeForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvSvgColorSettings = new System.Windows.Forms.DataGridView();
            this.colorSvgRef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colorCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColorVi = new System.Windows.Forms.DataGridViewImageColumn();
            this.speed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbLasermode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.smin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sminPercentage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.smax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.smaxPercentage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.passes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialDbCol = new System.Windows.Forms.DataGridViewImageColumn();
            this.tableBottomRow = new System.Windows.Forms.TableLayoutPanel();
            this.pbSvgPreview = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkPowerModulation = new System.Windows.Forms.LinkLabel();
            this.linkLaserModes = new System.Windows.Forms.LinkLabel();
            this.BtnCreate = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.TT = new System.Windows.Forms.ToolTip(this.components);
            this.tableMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSvgColorSettings)).BeginInit();
            this.tableBottomRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSvgPreview)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableMain
            // 
            resources.ApplyResources(this.tableMain, "tableMain");
            this.tableMain.Controls.Add(this.dgvSvgColorSettings, 0, 0);
            this.tableMain.Controls.Add(this.tableBottomRow, 0, 1);
            this.tableMain.Name = "tableMain";
            // 
            // dgvSvgColorSettings
            // 
            this.dgvSvgColorSettings.AllowUserToAddRows = false;
            this.dgvSvgColorSettings.AllowUserToDeleteRows = false;
            this.dgvSvgColorSettings.AllowUserToResizeRows = false;
            this.dgvSvgColorSettings.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvSvgColorSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSvgColorSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colorSvgRef,
            this.colorCode,
            this.ColorVi,
            this.speed,
            this.cbLasermode,
            this.smin,
            this.sminPercentage,
            this.smax,
            this.smaxPercentage,
            this.passes,
            this.materialDbCol});
            resources.ApplyResources(this.dgvSvgColorSettings, "dgvSvgColorSettings");
            this.dgvSvgColorSettings.MultiSelect = false;
            this.dgvSvgColorSettings.Name = "dgvSvgColorSettings";
            // 
            // colorSvgRef
            // 
            this.colorSvgRef.DataPropertyName = "ColorSvgRef";
            this.colorSvgRef.Frozen = true;
            resources.ApplyResources(this.colorSvgRef, "colorSvgRef");
            this.colorSvgRef.Name = "colorSvgRef";
            this.colorSvgRef.ReadOnly = true;
            // 
            // colorCode
            // 
            this.colorCode.DataPropertyName = "ColorName";
            this.colorCode.Frozen = true;
            resources.ApplyResources(this.colorCode, "colorCode");
            this.colorCode.Name = "colorCode";
            this.colorCode.ReadOnly = true;
            this.colorCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColorVi
            // 
            this.ColorVi.DataPropertyName = "ColorAsBitmap";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            this.ColorVi.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColorVi.Frozen = true;
            resources.ApplyResources(this.ColorVi, "ColorVi");
            this.ColorVi.Name = "ColorVi";
            this.ColorVi.ReadOnly = true;
            this.ColorVi.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // speed
            // 
            this.speed.DataPropertyName = "Speed";
            resources.ApplyResources(this.speed, "speed");
            this.speed.Name = "speed";
            // 
            // cbLasermode
            // 
            resources.ApplyResources(this.cbLasermode, "cbLasermode");
            this.cbLasermode.Name = "cbLasermode";
            // 
            // smin
            // 
            this.smin.DataPropertyName = "SMin";
            resources.ApplyResources(this.smin, "smin");
            this.smin.Name = "smin";
            // 
            // sminPercentage
            // 
            this.sminPercentage.DataPropertyName = "SMinPercentage";
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.DarkGray;
            this.sminPercentage.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.sminPercentage, "sminPercentage");
            this.sminPercentage.Name = "sminPercentage";
            this.sminPercentage.ReadOnly = true;
            // 
            // smax
            // 
            this.smax.DataPropertyName = "SMax";
            resources.ApplyResources(this.smax, "smax");
            this.smax.Name = "smax";
            // 
            // smaxPercentage
            // 
            this.smaxPercentage.DataPropertyName = "SMaxPercentage";
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.DimGray;
            this.smaxPercentage.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.smaxPercentage, "smaxPercentage");
            this.smaxPercentage.Name = "smaxPercentage";
            this.smaxPercentage.ReadOnly = true;
            // 
            // passes
            // 
            this.passes.DataPropertyName = "Passes";
            resources.ApplyResources(this.passes, "passes");
            this.passes.Name = "passes";
            // 
            // materialDbCol
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle4.NullValue")));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Transparent;
            this.materialDbCol.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.materialDbCol, "materialDbCol");
            this.materialDbCol.Image = ((System.Drawing.Image)(resources.GetObject("materialDbCol.Image")));
            this.materialDbCol.Name = "materialDbCol";
            this.materialDbCol.ReadOnly = true;
            // 
            // tableBottomRow
            // 
            resources.ApplyResources(this.tableBottomRow, "tableBottomRow");
            this.tableBottomRow.Controls.Add(this.pbSvgPreview, 0, 0);
            this.tableBottomRow.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableBottomRow.Name = "tableBottomRow";
            // 
            // pbSvgPreview
            // 
            this.pbSvgPreview.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pbSvgPreview, "pbSvgPreview");
            this.pbSvgPreview.Name = "pbSvgPreview";
            this.pbSvgPreview.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.BtnCreate, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.BtnCancel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.linkPowerModulation);
            this.groupBox1.Controls.Add(this.linkLaserModes);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // linkPowerModulation
            // 
            resources.ApplyResources(this.linkPowerModulation, "linkPowerModulation");
            this.linkPowerModulation.Name = "linkPowerModulation";
            this.linkPowerModulation.TabStop = true;
            this.TT.SetToolTip(this.linkPowerModulation, resources.GetString("linkPowerModulation.ToolTip"));
            this.linkPowerModulation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkPowerModulation_LinkClicked);
            // 
            // linkLaserModes
            // 
            resources.ApplyResources(this.linkLaserModes, "linkLaserModes");
            this.linkLaserModes.Name = "linkLaserModes";
            this.linkLaserModes.TabStop = true;
            this.TT.SetToolTip(this.linkLaserModes, resources.GetString("linkLaserModes.ToolTip"));
            this.linkLaserModes.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLaserModes_LinkClicked);
            // 
            // BtnCreate
            // 
            resources.ApplyResources(this.BtnCreate, "BtnCreate");
            this.BtnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnCreate.Name = "BtnCreate";
            this.BtnCreate.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // TT
            // 
            this.TT.AutoPopDelay = 10000;
            this.TT.InitialDelay = 500;
            this.TT.ReshowDelay = 100;
            // 
            // SvgToGCodeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.Controls.Add(this.tableMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SvgToGCodeForm";
            this.Load += new System.EventHandler(this.SvgToGCodeForm_Load);
            this.tableMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSvgColorSettings)).EndInit();
            this.tableBottomRow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSvgPreview)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ToolTip TT;
        private System.Windows.Forms.DataGridView dgvSvgColorSettings;
        private System.Windows.Forms.TableLayoutPanel tableBottomRow;
        private System.Windows.Forms.PictureBox pbSvgPreview;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel linkLaserModes;
        private System.Windows.Forms.LinkLabel linkPowerModulation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colorSvgRef;
        private System.Windows.Forms.DataGridViewTextBoxColumn colorCode;
        private System.Windows.Forms.DataGridViewImageColumn ColorVi;
        private System.Windows.Forms.DataGridViewTextBoxColumn speed;
        private System.Windows.Forms.DataGridViewComboBoxColumn cbLasermode;
        private System.Windows.Forms.DataGridViewTextBoxColumn smin;
        private System.Windows.Forms.DataGridViewTextBoxColumn sminPercentage;
        private System.Windows.Forms.DataGridViewTextBoxColumn smax;
        private System.Windows.Forms.DataGridViewTextBoxColumn smaxPercentage;
        private System.Windows.Forms.DataGridViewTextBoxColumn passes;
        private System.Windows.Forms.DataGridViewImageColumn materialDbCol;
    }
}
