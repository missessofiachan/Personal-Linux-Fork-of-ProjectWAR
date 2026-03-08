using ClientDataMatrix.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientDataMatrix.UI
{
    public sealed class DefinitionExplorerForm : Form
    {
        private readonly DefinitionExplorerModel _model;
        private readonly List<DefinitionExplorerRow> _rows;
        private TextBox _summaryTextBox;
        private TextBox _filterTextBox;
        private Label _countLabel;
        private DataGridView _grid;

        public DefinitionExplorerForm(DefinitionExplorerModel model)
        {
            _model = model ?? new DefinitionExplorerModel { Rows = new List<DefinitionExplorerRow>() };
            _rows = _model.Rows ?? new List<DefinitionExplorerRow>();

            Text = "Definition Explorer - " + (_model.FieldPath ?? "Field");
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(980, 720);
            Size = new Size(1280, 820);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            Controls.Add(CreateLayout());
            Load += (sender, args) => ApplyFilter();
        }

        private Control CreateLayout()
        {
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(10) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _summaryTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true,
                Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point),
                Text = BuildSummary()
            };

            FlowLayoutPanel filterPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            filterPanel.Controls.Add(new Label { Text = "Filter", AutoSize = true, Margin = new Padding(0, 8, 8, 0) });
            _filterTextBox = new TextBox { Width = 320 };
            _filterTextBox.TextChanged += (sender, args) => ApplyFilter();
            filterPanel.Controls.Add(_filterTextBox);
            _countLabel = new Label { AutoSize = true, Margin = new Padding(12, 8, 0, 0) };
            filterPanel.Controls.Add(_countLabel);

            _grid = CreateGrid();

            FlowLayoutPanel buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            Button closeButton = new Button { Text = "Close", AutoSize = true, DialogResult = DialogResult.OK };
            buttons.Controls.Add(closeButton);
            AcceptButton = closeButton;
            CancelButton = closeButton;

            layout.Controls.Add(WrapInGroup("Selected Field", _summaryTextBox), 0, 0);
            layout.Controls.Add(filterPanel, 0, 1);
            layout.Controls.Add(WrapInGroup("Known Raw Values", _grid), 0, 2);
            layout.Controls.Add(buttons, 0, 3);
            return layout;
        }

        private DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false
            };

            grid.Columns.Add(CreateTextColumn("Current", "CurrentMarker", 70));
            grid.Columns.Add(CreateTextColumn("Raw", "RawValue", 90));
            grid.Columns.Add(CreateTextColumn("Meaning", "Meaning", 260));
            grid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            grid.Columns.Add(CreateTextColumn("Source", "Source", 140));
            grid.Columns.Add(CreateTextColumn("Location", "Location", 90));
            grid.Columns.Add(CreateTextColumn("Path", "SourcePath", 420));
            grid.Columns.Add(CreateTextColumn("Notes", "Notes", 280));
            return grid;
        }

        private void ApplyFilter()
        {
            string filter = (_filterTextBox.Text ?? string.Empty).Trim();
            IEnumerable<DefinitionExplorerRow> filteredRows = _rows;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                string lowered = filter.ToLowerInvariant();
                filteredRows = _rows.Where(row =>
                    Contains(row.RawValue, lowered)
                    || Contains(row.Meaning, lowered)
                    || Contains(row.Confidence, lowered)
                    || Contains(row.Source, lowered)
                    || Contains(row.Location, lowered)
                    || Contains(row.SourcePath, lowered)
                    || Contains(row.Notes, lowered));
            }

            List<DefinitionExplorerRow> boundRows = filteredRows.ToList();
            _grid.DataSource = boundRows;
            _countLabel.Text = boundRows.Count.ToString(CultureInfo.InvariantCulture) + " shown / " + _rows.Count.ToString(CultureInfo.InvariantCulture) + " total";
            if (_grid.Rows.Count > 0 && _grid.CurrentCell == null)
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
        }

        private string BuildSummary()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Field: " + NullToPlaceholder(_model.FieldPath));
            builder.AppendLine("Domain key: " + NullToPlaceholder(_model.DomainKey));
            builder.AppendLine("Domain: " + NullToPlaceholder(_model.DomainDisplayName));
            builder.AppendLine("Domain type: " + (_model.IsFiniteDomain ? "Finite option set" : "Scalar or open numeric field"));
            builder.AppendLine("Current raw value: " + NullToPlaceholder(_model.CurrentRawValue));
            builder.AppendLine("Current meaning: " + NullToPlaceholder(_model.CurrentMeaning));
            builder.AppendLine("Current confidence: " + NullToPlaceholder(_model.CurrentConfidence));
            builder.AppendLine("Meaning source: " + NullToPlaceholder(_model.CurrentDefinitionSource));
            builder.AppendLine("Value source: " + NullToPlaceholder(_model.CurrentValueSource));
            builder.AppendLine("Value location: " + NullToPlaceholder(_model.CurrentValueLocation));
            builder.AppendLine("Value path: " + NullToPlaceholder(_model.CurrentValueSourcePath));

            if (!string.IsNullOrWhiteSpace(_model.DomainDescription))
                builder.AppendLine("Domain description: " + _model.DomainDescription);
            if (!string.IsNullOrWhiteSpace(_model.DomainNotes))
                builder.AppendLine("Domain notes: " + _model.DomainNotes);
            if (!string.IsNullOrWhiteSpace(_model.CurrentNotes))
                builder.AppendLine("Current field notes: " + _model.CurrentNotes);

            builder.AppendLine("Known rows loaded for this domain: " + _rows.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Double-click the row in the main definitions grid to reopen this window for another field.");
            return builder.ToString().TrimEnd();
        }

        private static bool Contains(string value, string loweredFilter)
        {
            return !string.IsNullOrWhiteSpace(value) && value.ToLowerInvariant().Contains(loweredFilter);
        }

        private static string NullToPlaceholder(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(none)" : value;
        }

        private static GroupBox WrapInGroup(string title, Control content)
        {
            GroupBox group = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(10) };
            group.Controls.Add(content);
            return group;
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(string headerText, string propertyName, int width)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = headerText,
                DataPropertyName = propertyName,
                Width = width,
                MinimumWidth = Math.Min(width, 60),
                SortMode = DataGridViewColumnSortMode.Automatic
            };
        }
    }
}
