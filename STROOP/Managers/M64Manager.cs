﻿using STROOP.M64Editor;
using STROOP.Structs;
using STROOP.Structs.Gui;
using STROOP.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STROOP.Managers
{
    public class M64Manager
    {
        bool _displaySaveChangesOnOpen = false;
        M64File _m64;
        int _lastRow = -1;

        M64Gui _gui;

        public M64Manager(M64Gui gui)
        {
            _gui = gui;

            _gui.ButtonSave.Click += ButtonSave_Click;
            _gui.ButtonSaveAs.Click += ButtonSaveAs_Click;
            _gui.ButtonOpen.Click += ButtonOpen_Click;
            _gui.ButtonClose.Click += ButtonClose_Click;
            _gui.ButtonGoto.Click += ButtonGoto_Click;
            _gui.ButtonSetUsHeader.Click += (sender, e) => SetHeaderRomVersion(RomVersion.US);
            _gui.ButtonSetJpHeader.Click += (sender, e) => SetHeaderRomVersion(RomVersion.JP);

            _gui.ToolStripMenuItemInsertNewAfter.Click += ToolStripMenuItemInsertNewAfter_Click;
            _gui.ToolStripMenuItemInsertNewAfter.Click += ToolStripMenuItemInsertNewAfter_Click;
            _gui.ToolStripMenuItemCopy.Click += ToolStripMenuItemCopy_Click;
            _gui.ToolStripMenuItemPasteOnto.Click += ToolStripMenuItemPasteOnto_Click;
            _gui.ToolStripMenuItemPasteBefore.Click += ToolStripMenuItemPasteBefore_Click;
            _gui.ToolStripMenuItemPasteAfter.Click += ToolStripMenuItemPasteAfter_Click;

            _gui.DataGridViewInputs.MouseClick += DataGridViewEditor_MouseClick;
            _gui.DataGridViewInputs.DataError += (sender, e) => _gui.DataGridViewInputs.CancelEdit();
            _gui.DataGridViewInputs.SelectionChanged += DataGridViewEditor_SelectionChanged;

            _m64 = new M64File();
            _gui.DataGridViewInputs.DataSource = _m64.Inputs;
            UpdateTableSettings();
            _gui.PropertyGridHeader.SelectedObject = _m64.Header;
            _gui.PropertyGridHeader.Refresh();
            _gui.PropertyGridStats.SelectedObject = _m64.Stats;
            _gui.PropertyGridStats.Refresh();
            _gui.PropertyGridStats.ContextMenuStrip = _m64.Stats.CreateContextMenuStrip();
            _gui.TabControlDetails.SelectedIndexChanged += TabControlDetails_SelectedIndexChanged;

            _gui.ButtonClearCells.Click += ButtonClearCells_Click;
        }

        private void SetHeaderRomVersion(RomVersion romVersion)
        {
            switch (romVersion)
            {
                case RomVersion.US:
                    _m64.Header.CountryCode = M64Config.CountryCodeUS;
                    _m64.Header.Crc = M64Config.CrcUS;
                    break;
                case RomVersion.JP:
                    _m64.Header.CountryCode = M64Config.CountryCodeJP;
                    _m64.Header.Crc = M64Config.CrcJP;
                    break;
                case RomVersion.PAL:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _gui.PropertyGridHeader.Refresh();
        }

        private void TabControlDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_gui.TabControlDetails.SelectedTab == _gui.TabPageHeader)
            {
                ControlUtilities.SetPropertyGridLabelColumnWidth(_gui.PropertyGridHeader, 160);
                _gui.PropertyGridHeader.Refresh();
            }
            else if (_gui.TabControlDetails.SelectedTab == _gui.TabPageStats)
            {
                ControlUtilities.SetPropertyGridLabelColumnWidth(_gui.PropertyGridStats, 160);
                _gui.PropertyGridStats.Refresh();
            }
        }

        private void DataGridViewEditor_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _lastRow = _gui.DataGridViewInputs.HitTest(e.X, e.Y).RowIndex;

                if (_lastRow < 0)
                    return;

                _gui.ContextMenuStripEditor.Show(_gui.DataGridViewInputs, new Point(e.X, e.Y));

            }
        }

        private void DataGridViewEditor_SelectionChanged(object sender, EventArgs e)
        {
            List<M64InputCell> cells = M64Utilities.GetSelectedCells(_gui.DataGridViewInputs);
            int minRowIndex = cells.Min(cell => cell.RowIndex);
            int maxRowIndex = cells.Max(cell => cell.RowIndex);
            List<string> headerTexts = cells
                .FindAll(cell => cell.IsInput)
                .ConvertAll(cell => cell.HeaderText).Distinct().ToList();
            headerTexts.Sort(M64Utilities.InputStringComparison);

            _gui.TextBoxSelectionStartFrame.Text = minRowIndex.ToString();
            _gui.TextBoxSelectionEndFrame.Text = maxRowIndex.ToString();
            _gui.TextBoxSelectionInputs.Text = String.Join("", headerTexts);
        }

        private void ButtonClearCells_Click(object sender, EventArgs e)
        {
            List<M64InputCell> cells = M64Utilities.GetSelectedCells(_gui.DataGridViewInputs);
            cells.ForEach(cell => cell.Clear());
            _gui.DataGridViewInputs.Refresh();
        }

        private void ButtonGoto_Click(object sender, EventArgs e)
        {
            int value;
            if (!int.TryParse(_gui.TextBoxGoto.Text, out value) || value < 0
                || value >= _gui.DataGridViewInputs.Rows.Count)
                return;

            _gui.DataGridViewInputs.FirstDisplayedScrollingRowIndex = value;
        }

        private void ToolStripMenuItemPasteAfter_Click(object sender, EventArgs e)
        {
            _m64.PasteInsert(_lastRow + 1);
        }

        private void ToolStripMenuItemPasteBefore_Click(object sender, EventArgs e)
        {
            _m64.PasteInsert(_lastRow);
        }

        private void ToolStripMenuItemPasteOnto_Click(object sender, EventArgs e)
        {
            var rowIndices = new List<int>();
            foreach (DataGridViewRow row in _gui.DataGridViewInputs.SelectedRows)
                rowIndices.Add(row.Index);

            _m64.PasteOnto(rowIndices);
        }

        private void ToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            var rowIndices = new List<int>();
            foreach (DataGridViewRow row in _gui.DataGridViewInputs.SelectedRows)
                rowIndices.Add(row.Index);

            _m64.CopyRows(rowIndices);
        }

        private void ToolStripMenuItemInsertNewBefore_Click(object sender, EventArgs e)
        {
            if (_lastRow < 0)
                return;

            _m64.InsertNew(_lastRow);
            _gui.DataGridViewInputs.ClearSelection();
            _gui.DataGridViewInputs.Rows[_lastRow].Selected = true;
        }

        private void ToolStripMenuItemInsertNewAfter_Click(object sender, EventArgs e)
        {
            if (_lastRow < 0)
                return;

            _m64.InsertNew(_lastRow + 1);
            _gui.DataGridViewInputs.ClearSelection();
            _gui.DataGridViewInputs.Rows[_lastRow].Selected = true;
        }

        private void ButtonSaveAs_Click(object sender, EventArgs e)
        {
            var dialogResult = _gui.SaveFileDialogM64.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;

            bool success = _m64.Save(_gui.SaveFileDialogM64.FileName);
            if (!success)
            {
                MessageBox.Show(
                    "Could not save file.\n" +
                        "Perhaps Mupen is currently editing it.\n" +
                        "Try closing Mupen and trying again.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            bool success = _m64.Save();
            if (!success)
            {
                MessageBox.Show(
                    "Could not save file.\n" +
                        "Perhaps Mupen is currently editing it.\n" +
                        "Try closing Mupen and trying again.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ButtonOpen_Click(object sender, EventArgs e)
        {
            if (CheckSaveChanges() == DialogResult.Cancel)
                return;

            var dialogResult = _gui.OpenFileDialogM64.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;

            string filePath = _gui.OpenFileDialogM64.FileName;
            string fileName = _gui.OpenFileDialogM64.SafeFileName;

            _gui.DataGridViewInputs.DataSource = null;
            _gui.PropertyGridHeader.SelectedObject = null;
            bool success = _m64.OpenFile(filePath, fileName);
            if (!success)
            {
                MessageBox.Show(
                    "Could not open file " + filePath + ".\n" +
                        "Perhaps Mupen is currently editing it.\n" +
                        "Try closing Mupen and trying again.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            _gui.DataGridViewInputs.DataSource = _m64.Inputs;
            UpdateTableSettings();
            _gui.PropertyGridHeader.SelectedObject = _m64.Header;
            _gui.DataGridViewInputs.Refresh();
            _gui.PropertyGridHeader.Refresh();
            _gui.PropertyGridStats.Refresh();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            _m64.Close();
            _gui.DataGridViewInputs.Refresh();
            _gui.PropertyGridHeader.Refresh();
            _gui.PropertyGridStats.Refresh();
        }

        private DialogResult CheckSaveChanges()
        {
            if (!_displaySaveChangesOnOpen)
                return DialogResult.OK;

            return MessageBox.Show("Do you want to save changes?", "Save Changes",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        private void UpdateTableSettings()
        {
            DataGridView table = _gui.DataGridViewInputs;
            if (table.Columns.Count != M64InputFrame.ColumnParameters.Count)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                (string headerText, int fillWeight, Color? backColor) = M64InputFrame.ColumnParameters[i];
                table.Columns[i].HeaderText = headerText;
                table.Columns[i].FillWeight = fillWeight;
                if (backColor.HasValue) table.Columns[i].DefaultCellStyle.BackColor = backColor.Value;
                table.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public void Update(bool updateView)
        {
            if (!updateView) return;

            _gui.LabelFileName.Text = _m64.CurrentFileName ?? "(No File Opened)";
            _gui.LabelNumInputsValue.Text = _m64.Inputs.Count.ToString();
        }
    }
}
