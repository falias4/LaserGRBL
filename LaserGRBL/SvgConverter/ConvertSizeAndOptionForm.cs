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
                f.ShowDialog(parent);
                if (f.DialogResult == DialogResult.OK)
                {
					// TODO FAL: Set default settings 
					/*
                    Settings.SetObject("GrayScaleConversion.VectorizeOptions.BorderSpeed", f.IIBorderTracing.CurrentValue);
                    Settings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMax", f.IIMaxPower.CurrentValue);
					Settings.SetObject("GrayScaleConversion.Gcode.LaserOptions.PowerMin", f.IIMinPower.CurrentValue);
					Settings.SetObject("GrayScaleConversion.Gcode.LaserOptions.LaserOn", (f.CBLaserON.SelectedItem as LaserMode).GCode);
					*/

					core.LoadedFile.LoadImportedSVG(filename, append, core, laserSettings);
                }
            }
        }

        private SvgToGCodeForm(GrblCore core, string filename)
		{
			InitializeComponent();
			mCore = core;

			// TODO FAL Theme
			BackColor = dgvSvgColorSettings.BackgroundColor = ColorScheme.FormBackColor;
			ForeColor = ColorScheme.FormForeColor;
			BtnCancel.BackColor = BtnCreate.BackColor = ColorScheme.FormButtonsColor;

			smin.Visible = sminPercentage.Visible = smax.Visible = smaxPercentage.Visible = supportPWM;

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
			loadImagesFromSvg(converter, colors, filename);

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

		private void loadImagesFromSvg(GCodeFromSVG converter, string[] colors, string filename)
        {
			svgImages = converter.splitSvgByColor(filename, colors).Aggregate(new Dictionary<string, Image>(), (acc, svgByColor) =>
			{
				var svg = svgByColor.Value;

				// create a white bitmap
				var svgDim = svg.GetDimensions();
				Bitmap bmp = new Bitmap((int)svgDim.Width, (int)svgDim.Height);
				using (Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.White);
				}

				svgByColor.Value.Draw(bmp);
				acc.Add(svgByColor.Key, bmp);

				return acc;
			});
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

        private void linkLaserModes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
			 Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/target-image-size-and-laser-options/#laser-modes"); 
		}

        private void linkPowerModulation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
			Tools.Utils.OpenLink(@"https://lasergrbl.com/usage/raster-image-import/target-image-size-and-laser-options/#power-modulation");
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
