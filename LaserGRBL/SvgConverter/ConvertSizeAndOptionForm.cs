//Copyright (c) 2016-2021 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using LaserGRBL.PSHelper;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Linq;
using System.Windows.Forms;

namespace LaserGRBL.SvgConverter
{
	/// <summary>
	/// Description of ConvertSizeAndOptionForm.
	/// </summary>
	public partial class SvgToGCodeForm : Form
	{
		GrblCore mCore;
		static List<SvgColorSetting> laserSettings = new List<SvgColorSetting>();
		Dictionary<string, Image> svgImages = new Dictionary<string, Image>();
		bool supportPWM = Settings.GetObject("Support Hardware PWM", true);

		public class ComboboxItem
		{
			public string Text { get; set; }
			public object Value { get; set; }

			public ComboboxItem(string text, object value)
			{ Text = text; Value = value; }

			public override string ToString()
			{
				return Text;
			}
		}

		internal static void CreateAndShowDialog(GrblCore core, string filename, Form parent, bool append)
        {
            using (SvgToGCodeForm f = new SvgToGCodeForm(core, filename))
            {
                f.ShowDialogForm(parent);
                if (f.DialogResult == DialogResult.OK)
                {
                    Settings.SetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", f.IIBorderTracing.CurrentValue);
                    Settings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", f.IIMaxPower.CurrentValue);
					Settings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", f.IIMinPower.CurrentValue);
					Settings.SetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", (f.CBLaserON.SelectedItem as LaserMode).GCode);

					core.LoadedFile.LoadImportedSVG(filename, append, core, laserSettings);
                }
            }
        }

        private SvgToGCodeForm(GrblCore core, string filename)
		{
			InitializeComponent();
			mCore = core;

			BackColor = ColorScheme.FormBackColor;
			GbLaser.ForeColor = GbSpeed.ForeColor = ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;

			LblSmin.Visible = LblSmax.Visible = IIMaxPower.Visible = IIMinPower.Visible = BtnModulationInfo.Visible = supportPWM;
			AssignMinMaxLimit();

			CBLaserON.DataSource = LaserMode.LaserModes;
			CBLaserON.ValueMember = "GCode";
			CBLaserON.DisplayMember = "DisplayName";

			cbLasermode.DataPropertyName = "LaserMode";
			cbLasermode.ValueMember = "Self";
			cbLasermode.DisplayMember = "DisplayName";
			cbLasermode.ValueType = typeof(LaserMode);
			cbLasermode.DataSource = new BindingSource(LaserMode.LaserModes, null);

			initDataGrid(filename);
		}

		int hoveredMaterialDbBtnRow = -1;
		int restoreMaterialDbBtnRow = -1;
		private bool dataGridMouseClicked = false;

		private void initDataGrid(string filename)
		{
			GCodeFromSVG converter = new SvgConverter.GCodeFromSVG();
			var colors = converter.getAllColorsInFile(filename).ToArray();

			// Convert SvgDocument to Image
			svgImages = converter.splitSvgByColor(filename, colors).Aggregate(new Dictionary<string, Image>(), (acc, svgByColor) =>
			{
				acc.Add(svgByColor.Key, svgByColor.Value.Draw());
				return acc;
			});

			laserSettings = colors.Select(c => new SvgColorSetting(c, mCore)).ToList();
			var boundList = new BindingList<SvgColorSetting>(laserSettings);
			dgvSvgColorSettings.DataSource = boundList;

			dgvSvgColorSettings.CellFormatting += DgvSvgColorSettings_CellFormatting;
			dgvSvgColorSettings.CellMouseEnter += DgvSvgColorSettings_CellMouseEnter;
			dgvSvgColorSettings.CellMouseLeave += DgvSvgColorSettings_CellMouseLeave;
			dgvSvgColorSettings.CellClick += DgvSvgColorSettings_CellClick;
			dgvSvgColorSettings.CellEnter += DgvSettings_CellEnter;
			dgvSvgColorSettings.CellValueChanged += DgvSettings_CellValueChanged;
			dgvSvgColorSettings.SelectionChanged += DgvSvgColorSettings_SelectionChanged;
			dgvSvgColorSettings.CellMouseDown += (sender, e) => { dataGridMouseClicked = true; };
			dgvSvgColorSettings.CellMouseUp += (sender, e) => { dataGridMouseClicked = false; };

			dgvSvgColorSettings.Rows[0].Selected = true;
		}

        private void DgvSvgColorSettings_SelectionChanged(object sender, EventArgs e)
        {
			UpdateSvgBySelection();
        }

		private void UpdateSvgBySelection()
		{
			DataGridViewRow row = null;
			if (dgvSvgColorSettings.SelectedRows.Count == 1)
			{
				row = dgvSvgColorSettings.SelectedRows[0];
			}
			else if (dgvSvgColorSettings.SelectedCells.Count > 0)
			{
				var rowIdx = dgvSvgColorSettings.SelectedCells[0].RowIndex;
				row = dgvSvgColorSettings.Rows[rowIdx];
			}

			if (row != null)
			{
				var selectedSetting = row.DataBoundItem as SvgColorSetting;
				pbSvgPreview.Image = svgImages[selectedSetting.ColorSvgRef];
			}
		}

		private void DgvSvgColorSettings_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex > -1 && e.ColumnIndex == materialDbCol.Index)
			{
				hoveredMaterialDbBtnRow = e.RowIndex;
				dgvSvgColorSettings.UpdateCellValue(e.ColumnIndex, e.RowIndex);
			}
		}

		private void DgvSvgColorSettings_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
			if (e.RowIndex > -1 && e.ColumnIndex == materialDbCol.Index)
			{
				restoreMaterialDbBtnRow = hoveredMaterialDbBtnRow;
				hoveredMaterialDbBtnRow = -1;
				dgvSvgColorSettings.UpdateCellValue(e.ColumnIndex, e.RowIndex);
			}
		}

		private void DgvSvgColorSettings_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (hoveredMaterialDbBtnRow == e.RowIndex && e.ColumnIndex == materialDbCol.Index)
			{
				var highlightedImg = Base.Drawing.ImageTransform.Brightness(materialDbCol.Image, 0.11F);
				e.Value = highlightedImg;
			}
			else if (restoreMaterialDbBtnRow == e.RowIndex && e.ColumnIndex == materialDbCol.Index)
			{
				e.Value = materialDbCol.Image;
			}
		}

		private void DgvSvgColorSettings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0)
            {
				return;
            }

			if(e.ColumnIndex == materialDbCol.Index)
            {
				MaterialDB.MaterialsRow selectedMaterial = PSHelperForm.CreateAndShowDialog(this);
				if (selectedMaterial != null)
				{
					var currentRow = laserSettings[e.RowIndex];
					currentRow.Speed = selectedMaterial.Speed;
					currentRow.SMax = (int)mCore.Configuration.MaxPWM * selectedMaterial.Power / 100;
					currentRow.Passes = selectedMaterial.Cycles;
					updateDgvRow(e.RowIndex);
				}
			}
        }

		private void updateDgvRow(int rowIndex)
        {
			for(var i = 0; i < dgvSvgColorSettings.Columns.Count; i++)
            {
				dgvSvgColorSettings.UpdateCellValue(i, rowIndex);
            }
		}

        private void DgvSettings_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (dgvSvgColorSettings.Columns[e.ColumnIndex].ReadOnly && !dataGridMouseClicked)
			{
				SendKeys.Send("{TAB}");
				return;
			}

			dgvSvgColorSettings.BeginEdit(false);

			if (dgvSvgColorSettings.EditingControl is TextBoxBase)
			{
				TextBoxBase textBox = (TextBoxBase)dgvSvgColorSettings.EditingControl;

				var pos = dgvSvgColorSettings.EditingControl.PointToClient(Cursor.Position);
				var g = textBox.CreateGraphics();
				var textWidth = (int)g.MeasureString(textBox.Text, textBox.Font).Width;

				if (pos.X < textWidth)
				{
					textBox.SelectionStart = textBox.GetCharIndexFromPosition(dgvSvgColorSettings.EditingControl.PointToClient(Cursor.Position));
				} else
                {
					textBox.SelectionStart = textBox.TextLength;
				}
				
				
			}
			if (dgvSvgColorSettings.EditingControl is ComboBox)
            {
				ComboBox cb = (ComboBox)dgvSvgColorSettings.EditingControl;
				cb.DroppedDown = true;
            }
		}

		private void DgvSettings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			string colDataPropertyName = dgvSvgColorSettings.Columns[e.ColumnIndex].DataPropertyName;
			if (colDataPropertyName == "SMin" || colDataPropertyName == "SMax") { 
				dgvSvgColorSettings.UpdateCellValue(e.ColumnIndex + 1, e.RowIndex);
			}
        }

        private void AssignMinMaxLimit()
        { 
			IIBorderTracing.MaxValue = (int)mCore.Configuration.MaxRateX;
			IIMaxPower.MaxValue = (int)mCore.Configuration.MaxPWM;
		}

		public void ShowDialogForm(Form parent)
        {
			IIBorderTracing.CurrentValue = Settings.GetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", 1000);

			string LaserOn = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", "M3");

			if (LaserOn == "M3" || !mCore.Configuration.LaserMode)
				CBLaserON.SelectedItem = LaserMode.LaserModes.Single(m => m.GCode == "M3");
			else
				CBLaserON.SelectedItem = LaserMode.LaserModes.Single(m => m.GCode == "M4");

			string LaserOff = "M5"; //Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOff", "M5");

			IIMinPower.CurrentValue = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", 0);
			IIMaxPower.CurrentValue = Settings.GetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", (int)mCore.Configuration.MaxPWM);

			IIBorderTracing.Visible = LblBorderTracing.Visible = LblBorderTracingmm.Visible = true;

			RefreshPerc();

			ShowDialog(parent);
		}


		void IIBorderTracingCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			//IP.BorderSpeed = NewValue;
		}

	
		void IIMinPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMaxPower.CurrentValue <= NewValue)
				IIMaxPower.CurrentValue = NewValue + 1;

			RefreshPerc();
		}
		void IIMaxPowerCurrentValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		{
			if (ByUser && IIMinPower.CurrentValue >= NewValue)
				IIMinPower.CurrentValue = NewValue - 1;

			RefreshPerc();
		}

		private void RefreshPerc()
		{
			decimal maxpwm = mCore?.Configuration != null ? mCore.Configuration.MaxPWM : -1;

			if (maxpwm > 0)
			{
				LblMaxPerc.Text = (IIMaxPower.CurrentValue / mCore.Configuration.MaxPWM).ToString("P1");
				LblMinPerc.Text = (IIMinPower.CurrentValue / mCore.Configuration.MaxPWM).ToString("P1");
			}
			else
			{
				LblMaxPerc.Text = "";
				LblMinPerc.Text = "";
			}
		}

		private void BtnOnOffInfo_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/target-image-size-and-laser-options/#laser-modes");}

		private void BtnModulationInfo_Click(object sender, EventArgs e)
		{Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/target-image-size-and-laser-options/#power-modulation");}

		private void CBLaserON_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboboxItem mode = CBLaserON.SelectedItem as ComboboxItem;

			if (mode != null)
			{
				if (!mCore.Configuration.LaserMode && (mode.Value as string) == "M4")
					MessageBox.Show(Strings.WarnWrongLaserMode, Strings.WarnWrongLaserModeTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);//warning!!
			}

		}



		private void BtnPSHelper_Click(object sender, EventArgs e)
		{
			MaterialDB.MaterialsRow row = PSHelperForm.CreateAndShowDialog(this);
			if (row != null)
			{
				if (IIBorderTracing.Visible)
					IIBorderTracing.CurrentValue = row.Speed;
				//if (IILinearFilling.Visible)
				//	IILinearFilling.CurrentValue = row.Speed;

				IIMaxPower.CurrentValue = IIMaxPower.MaxValue * row.Power / 100;
			}
		}

		//private void IISizeW_OnTheFlyValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		//{
		//	if (ByUser)
		//		IISizeH.CurrentValue = IP.WidthToHeight(NewValue);
		//}

		//private void IISizeH_OnTheFlyValueChanged(object sender, int OldValue, int NewValue, bool ByUser)
		//{
		//	if (ByUser) IISizeW.CurrentValue = IP.HeightToWidht(NewValue);
		//}

	}
}
