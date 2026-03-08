using ClientDataMatrix.Model;
using ClientDataMatrix.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientDataMatrix.UI
{
    public sealed class MainForm : Form
    {
        private TextBox _rootPathTextBox;
        private TextBox _outputPathTextBox;
        private Button _reloadButton;
        private Button _generateAllButton;
        private TextBox _abilitySearchTextBox;
        private Label _abilityCatalogStatusLabel;
        private DataGridView _abilityGrid;
        private TextBox _abilityIdTextBox;
        private Button _generateAbilityButton;
        private Button _openAbilityMarkdownButton;
        private Button _openAbilityFolderButton;
        private TextBox _abilitySummaryTextBox;
        private TreeView _abilityTreeView;
        private TextBox _abilityNarrativeTextBox;
        private DataGridView _definitionGrid;
        private Label _definitionHintLabel;
        private Button _generateTokenDictionaryButton;
        private Button _openTokenDictionaryMarkdownButton;
        private Button _openTokenDictionaryFolderButton;
        private TextBox _tokenSummaryTextBox;
        private DataGridView _tokenGrid;
        private Button _generateCoverageButton;
        private Button _openCoverageMarkdownButton;
        private Button _openCoverageFolderButton;
        private TextBox _coverageSummaryTextBox;
        private DataGridView _coverageGrid;
        private Button _generateDomainsButton;
        private Button _openDomainsMarkdownButton;
        private Button _openDomainsFolderButton;
        private TextBox _domainsSummaryTextBox;
        private DataGridView _domainsGrid;
        private DataGridView _domainValuesGrid;
        private Button _generateRequirementsButton;
        private Button _openRequirementsMarkdownButton;
        private Button _openRequirementsFolderButton;
        private TextBox _requirementsSummaryTextBox;
        private DataGridView _requirementsGrid;
        private DataGridView _requirementRowGrid;
        private DataGridView _requirementFieldGrid;
        private DataGridView _requirementReferenceGrid;
        private Button _generateOperationSchemasButton;
        private Button _openOperationSchemasMarkdownButton;
        private Button _openOperationSchemasFolderButton;
        private TextBox _operationSummaryTextBox;
        private DataGridView _operationGrid;
        private DataGridView _operationFieldGrid;
        private DataGridView _operationAbilityGrid;
        private Button _generateConflictsButton;
        private Button _openConflictMarkdownButton;
        private Button _openConflictFolderButton;
        private TextBox _conflictSummaryTextBox;
        private DataGridView _conflictDomainGrid;
        private DataGridView _conflictGrid;
        private DataGridView _statusGrid;
        private TextBox _logTextBox;
        private ToolStripStatusLabel _statusLabel;

        private MatrixAnalysisSession _session;
        private DefinitionCatalog _definitions;
        private AbilityNarrativeBuilder _narrativeBuilder;
        private List<AbilityCatalogEntry> _abilityCatalog = new List<AbilityCatalogEntry>();
        private AbilityAnalysisResult _lastAbilityReport;
        private TokenDictionaryDocument _lastTokenDictionary;
        private CoverageReportDocument _lastCoverageReport;
        private DomainLedgerDocument _lastDomainLedger;
        private RequirementLedgerDocument _lastRequirementLedger;
        private OperationSchemaDocument _lastOperationSchema;
        private ConflictReportDocument _lastConflictReport;
        private string _lastAbilityMarkdownPath;
        private string _lastTokenDictionaryMarkdownPath;
        private string _lastCoverageMarkdownPath;
        private string _lastDomainLedgerMarkdownPath;
        private string _lastRequirementMarkdownPath;
        private string _lastOperationSchemaMarkdownPath;
        private string _lastConflictMarkdownPath;
        private bool _isBusy;

        public MainForm(string extractedRootPath, string outputRoot)
        {
            Text = "ClientDataMatrix";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1360, 860);
            Size = new Size(1500, 940);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            TableLayoutPanel shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.Controls.Add(CreatePathPanel(extractedRootPath, outputRoot), 0, 0);
            shell.Controls.Add(CreateMainTabs(), 0, 1);

            StatusStrip strip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Text = "Waiting to load extracted dataset..." };
            strip.Items.Add(_statusLabel);
            shell.Controls.Add(strip, 0, 2);

            Controls.Add(shell);
            Shown += async (sender, args) => await LoadSessionAsync();
        }

        private Control CreatePathPanel(string extractedRootPath, string outputRoot)
        {
            TableLayoutPanel panel = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 5, RowCount = 2, Padding = new Padding(10, 10, 10, 4) };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            panel.Controls.Add(new Label { Text = "Extracted root", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 8, 0) }, 0, 0);
            _rootPathTextBox = new TextBox { Dock = DockStyle.Fill, Text = extractedRootPath ?? string.Empty, Margin = new Padding(0, 4, 8, 4) };
            panel.Controls.Add(_rootPathTextBox, 1, 0);
            Button browseRoot = new Button { Text = "Browse...", AutoSize = true, Margin = new Padding(0, 3, 8, 3) };
            browseRoot.Click += (sender, args) => ChooseFolder(_rootPathTextBox, "Select the extracted client root folder.");
            panel.Controls.Add(browseRoot, 2, 0);
            _reloadButton = new Button { Text = "Reload Data", AutoSize = true };
            _reloadButton.Click += async (sender, args) => await LoadSessionAsync();
            panel.Controls.Add(_reloadButton, 3, 0);
            _generateAllButton = new Button { Text = "Generate All", AutoSize = true, Enabled = false };
            _generateAllButton.Click += async (sender, args) => await GenerateAllReportsAsync();
            panel.Controls.Add(_generateAllButton, 4, 0);

            panel.Controls.Add(new Label { Text = "Output root", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 8, 0) }, 0, 1);
            _outputPathTextBox = new TextBox { Dock = DockStyle.Fill, Text = outputRoot ?? string.Empty, Margin = new Padding(0, 4, 8, 4) };
            panel.Controls.Add(_outputPathTextBox, 1, 1);
            Button browseOutput = new Button { Text = "Browse...", AutoSize = true, Margin = new Padding(0, 3, 8, 3) };
            browseOutput.Click += (sender, args) => ChooseFolder(_outputPathTextBox, "Select the report output folder.");
            panel.Controls.Add(browseOutput, 2, 1);
            Label hint = new Label { Text = "Double-click the exe to launch this GUI. Analysis remains read-only against extracted files.", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 0) };
            panel.Controls.Add(hint, 3, 1);
            panel.SetColumnSpan(hint, 2);

            return panel;
        }

        private Control CreateMainTabs()
        {
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(CreateAbilityTab());
            tabs.TabPages.Add(CreateTokenTab());
            tabs.TabPages.Add(CreateDomainTab());
            tabs.TabPages.Add(CreateRequirementTab());
            tabs.TabPages.Add(CreateCoverageTab());
            tabs.TabPages.Add(CreateOperationTab());
            tabs.TabPages.Add(CreateConflictTab());
            tabs.TabPages.Add(CreateStatusTab());
            tabs.TabPages.Add(CreateLogTab());
            return tabs;
        }

        private TabPage CreateAbilityTab()
        {
            TabPage tab = new TabPage("Ability Doctor");
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 430 };
            split.Panel1.Controls.Add(CreateAbilityCatalogGroup());
            split.Panel2.Controls.Add(CreateAbilityReportGroup());
            tab.Controls.Add(split);
            return tab;
        }

        private Control CreateAbilityCatalogGroup()
        {
            GroupBox left = new GroupBox { Text = "Ability Catalog", Dock = DockStyle.Fill, Padding = new Padding(10) };
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel search = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            search.Controls.Add(new Label { Text = "Filter", AutoSize = true, Margin = new Padding(0, 8, 8, 0) });
            _abilitySearchTextBox = new TextBox { Width = 320 };
            _abilitySearchTextBox.TextChanged += (sender, args) => ApplyAbilityFilter();
            search.Controls.Add(_abilitySearchTextBox);

            _abilityCatalogStatusLabel = new Label { AutoSize = true, Margin = new Padding(0, 0, 0, 8), Text = "Load the extracted dataset to populate the catalog." };
            _abilityGrid = CreateReadOnlyGrid();
            _abilityGrid.AutoGenerateColumns = false;
            _abilityGrid.Columns.Add(CreateTextColumn("AbilityId", "AbilityId", 90));
            _abilityGrid.Columns.Add(CreateTextColumn("Name", "Name", 230));
            _abilityGrid.Columns.Add(CreateTextColumn("Effect", "EffectIdText", 80));
            _abilityGrid.Columns.Add(CreateTextColumn("Sources", "Sources", 100));
            _abilityGrid.SelectionChanged += (sender, args) => { AbilityCatalogEntry selected = SelectedAbility(); if (selected != null) _abilityIdTextBox.Text = selected.AbilityId.ToString(CultureInfo.InvariantCulture); };
            _abilityGrid.CellDoubleClick += async (sender, args) => { if (args.RowIndex >= 0) await GenerateAbilityReportAsync(); };

            layout.Controls.Add(search, 0, 0);
            layout.Controls.Add(_abilityCatalogStatusLabel, 0, 1);
            layout.Controls.Add(_abilityGrid, 0, 2);
            left.Controls.Add(layout);
            return left;
        }

        private Control CreateAbilityReportGroup()
        {
            GroupBox right = new GroupBox { Text = "Ability Report", Dock = DockStyle.Fill, Padding = new Padding(10) };
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            actions.Controls.Add(new Label { Text = "Ability ID", AutoSize = true, Margin = new Padding(0, 8, 8, 0) });
            _abilityIdTextBox = new TextBox { Width = 110 };
            actions.Controls.Add(_abilityIdTextBox);
            _generateAbilityButton = new Button { Text = "Generate Ability Report", AutoSize = true, Enabled = false };
            _generateAbilityButton.Click += async (sender, args) => await GenerateAbilityReportAsync();
            actions.Controls.Add(_generateAbilityButton);
            _openAbilityMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openAbilityMarkdownButton.Click += (sender, args) => OpenPath(_lastAbilityMarkdownPath);
            actions.Controls.Add(_openAbilityMarkdownButton);
            _openAbilityFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openAbilityFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastAbilityMarkdownPath) ? null : Path.GetDirectoryName(_lastAbilityMarkdownPath));
            actions.Controls.Add(_openAbilityFolderButton);

            _abilitySummaryTextBox = CreateReadOnlyTextBox();
            _abilitySummaryTextBox.Text = "Generate an ability report to populate the tree, the narrative, and decoded field definitions.";

            TabControl reportTabs = new TabControl { Dock = DockStyle.Fill };
            _abilityTreeView = new TreeView { Dock = DockStyle.Fill, HideSelection = false };
            TabPage treeTab = new TabPage("Tree");
            treeTab.Controls.Add(_abilityTreeView);
            reportTabs.TabPages.Add(treeTab);

            _abilityNarrativeTextBox = CreateReadOnlyTextBox();
            _abilityNarrativeTextBox.Text = "Generate an ability report to see what we think happens when the ability is used.";
            TabPage narrativeTab = new TabPage("What Happens");
            narrativeTab.Controls.Add(_abilityNarrativeTextBox);
            reportTabs.TabPages.Add(narrativeTab);

            TabPage defTab = new TabPage("Definitions");
            defTab.Controls.Add(CreateDefinitionPanel());
            reportTabs.TabPages.Add(defTab);

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_abilitySummaryTextBox, 0, 1);
            layout.Controls.Add(reportTabs, 0, 2);
            right.Controls.Add(layout);
            return right;
        }

        private TabPage CreateConflictTab()
        {
            TabPage tab = new TabPage("Conflict Ledger");
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _generateConflictsButton = new Button { Text = "Generate Conflict Report", AutoSize = true, Enabled = false };
            _generateConflictsButton.Click += async (sender, args) => await GenerateConflictReportAsync();
            actions.Controls.Add(_generateConflictsButton);
            _openConflictMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openConflictMarkdownButton.Click += (sender, args) => OpenPath(_lastConflictMarkdownPath);
            actions.Controls.Add(_openConflictMarkdownButton);
            _openConflictFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openConflictFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastConflictMarkdownPath) ? null : Path.GetDirectoryName(_lastConflictMarkdownPath));
            actions.Controls.Add(_openConflictFolderButton);

            _conflictSummaryTextBox = CreateReadOnlyTextBox();
            _conflictSummaryTextBox.Text = "Generate the conflict ledger to browse contradiction domains.";

            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 240 };
            _conflictDomainGrid = CreateReadOnlyGrid();
            _conflictDomainGrid.AutoGenerateColumns = false;
            _conflictDomainGrid.Columns.Add(CreateTextColumn("Domain", "Domain", 180));
            _conflictDomainGrid.Columns.Add(CreateTextColumn("Conflicts", "ConflictCount", 90));
            _conflictDomainGrid.Columns.Add(CreateTextColumn("Subjects", "SubjectCount", 90));
            _conflictDomainGrid.Columns.Add(CreateTextColumn("Facts", "FactCount", 90));
            _conflictDomainGrid.SelectionChanged += (sender, args) => RefreshConflictRows();

            _conflictGrid = CreateReadOnlyGrid();
            _conflictGrid.AutoGenerateColumns = false;
            _conflictGrid.Columns.Add(CreateTextColumn("Subject", "SubjectKey", 190));
            _conflictGrid.Columns.Add(CreateTextColumn("Fact", "FactName", 140));
            _conflictGrid.Columns.Add(CreateTextColumn("Domain", "Domain", 110));
            _conflictGrid.Columns.Add(CreateTextColumn("Distinct Values", "DistinctValues", 420));
            _conflictGrid.Columns.Add(CreateTextColumn("Evidence", "ClaimCount", 80));

            split.Panel1.Controls.Add(WrapInGroup("Conflict Domains", _conflictDomainGrid));
            split.Panel2.Controls.Add(WrapInGroup("Conflict Details", _conflictGrid));

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_conflictSummaryTextBox, 0, 1);
            layout.Controls.Add(split, 0, 2);
            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage CreateTokenTab()
        {
            TabPage tab = new TabPage("Token Dictionary");
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _generateTokenDictionaryButton = new Button { Text = "Generate Token Dictionary", AutoSize = true, Enabled = false };
            _generateTokenDictionaryButton.Click += async (sender, args) => await GenerateTokenDictionaryAsync();
            actions.Controls.Add(_generateTokenDictionaryButton);
            _openTokenDictionaryMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openTokenDictionaryMarkdownButton.Click += (sender, args) => OpenPath(_lastTokenDictionaryMarkdownPath);
            actions.Controls.Add(_openTokenDictionaryMarkdownButton);
            _openTokenDictionaryFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openTokenDictionaryFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastTokenDictionaryMarkdownPath) ? null : Path.GetDirectoryName(_lastTokenDictionaryMarkdownPath));
            actions.Controls.Add(_openTokenDictionaryFolderButton);

            _tokenSummaryTextBox = CreateReadOnlyTextBox();
            _tokenSummaryTextBox.Text = "Generate the COM token dictionary to write plain-English token definitions from extracted client evidence.";

            _tokenGrid = CreateReadOnlyGrid();
            _tokenGrid.AutoGenerateColumns = false;
            _tokenGrid.Columns.Add(CreateTextColumn("Token", "ExampleToken", 150));
            _tokenGrid.Columns.Add(CreateTextColumn("Field", "FieldKey", 130));
            _tokenGrid.Columns.Add(CreateTextColumn("Meaning", "PlainEnglishMeaning", 420));
            _tokenGrid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            _tokenGrid.Columns.Add(CreateTextColumn("Tags", "ContextTagsText", 150));
            _tokenGrid.Columns.Add(CreateTextColumn("Abilities", "ExampleAbilityIdsText", 200));
            _tokenGrid.Columns.Add(CreateTextColumn("Notes", "Notes", 260));

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_tokenSummaryTextBox, 0, 1);
            layout.Controls.Add(_tokenGrid, 0, 2);
            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage CreateDomainTab()
        {
            TabPage tab = new TabPage("Domains");
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _generateDomainsButton = new Button { Text = "Generate Domain Ledger", AutoSize = true, Enabled = false };
            _generateDomainsButton.Click += async (sender, args) => await GenerateDomainLedgerAsync();
            actions.Controls.Add(_generateDomainsButton);
            _openDomainsMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openDomainsMarkdownButton.Click += (sender, args) => OpenPath(_lastDomainLedgerMarkdownPath);
            actions.Controls.Add(_openDomainsMarkdownButton);
            _openDomainsFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openDomainsFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastDomainLedgerMarkdownPath) ? null : Path.GetDirectoryName(_lastDomainLedgerMarkdownPath));
            actions.Controls.Add(_openDomainsFolderButton);

            _domainsSummaryTextBox = CreateReadOnlyTextBox();
            _domainsSummaryTextBox.Text = "Generate the core identity domain ledger to separate proven extracted-client domains such as Race.EntryId, CareerLine.EntryId, and repeated CareerName entry ids.";

            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 250 };
            _domainsGrid = CreateReadOnlyGrid();
            _domainsGrid.AutoGenerateColumns = false;
            _domainsGrid.Columns.Add(CreateTextColumn("Domain", "DisplayName", 210));
            _domainsGrid.Columns.Add(CreateTextColumn("Key", "DomainKey", 190));
            _domainsGrid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            _domainsGrid.Columns.Add(CreateTextColumn("Canonicality", "Canonicality", 260));
            _domainsGrid.Columns.Add(CreateTextColumn("Values", "ValueCount", 70));
            _domainsGrid.Columns.Add(CreateTextColumn("Distinct Meanings", "DistinctMeaningCount", 95));
            _domainsGrid.Columns.Add(CreateTextColumn("Duplicate Groups", "DuplicateMeaningCount", 95));
            _domainsGrid.Columns.Add(CreateTextColumn("Sources", "SourceFilesText", 160));
            _domainsGrid.SelectionChanged += (sender, args) => RefreshDomainValues();

            _domainValuesGrid = CreateReadOnlyGrid();
            _domainValuesGrid.AutoGenerateColumns = false;
            _domainValuesGrid.Columns.Add(CreateTextColumn("Raw", "RawValue", 80));
            _domainValuesGrid.Columns.Add(CreateTextColumn("Meaning", "Meaning", 220));
            _domainValuesGrid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            _domainValuesGrid.Columns.Add(CreateTextColumn("Source", "Source", 150));
            _domainValuesGrid.Columns.Add(CreateTextColumn("Path", "SourcePath", 360));
            _domainValuesGrid.Columns.Add(CreateTextColumn("Location", "SourceLocation", 90));
            _domainValuesGrid.Columns.Add(CreateTextColumn("Notes", "Notes", 320));

            split.Panel1.Controls.Add(WrapInGroup("Domain Summary", _domainsGrid));
            split.Panel2.Controls.Add(WrapInGroup("Domain Values", _domainValuesGrid));

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_domainsSummaryTextBox, 0, 1);
            layout.Controls.Add(split, 0, 2);
            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage CreateRequirementTab()
        {
            TabPage tab = new TabPage("Requirements");
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _generateRequirementsButton = new Button { Text = "Generate Requirement Ledger", AutoSize = true, Enabled = false };
            _generateRequirementsButton.Click += async (sender, args) => await GenerateRequirementLedgerAsync();
            actions.Controls.Add(_generateRequirementsButton);
            _openRequirementsMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openRequirementsMarkdownButton.Click += (sender, args) => OpenPath(_lastRequirementMarkdownPath);
            actions.Controls.Add(_openRequirementsMarkdownButton);
            _openRequirementsFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openRequirementsFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastRequirementMarkdownPath) ? null : Path.GetDirectoryName(_lastRequirementMarkdownPath));
            actions.Controls.Add(_openRequirementsFolderButton);

            _requirementsSummaryTextBox = CreateReadOnlyTextBox();
            _requirementsSummaryTextBox.Text = "Generate the requirement ledger to inspect inbound links, child requirement chains, and active ext-data fields.";

            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 520 };
            _requirementsGrid = CreateReadOnlyGrid();
            _requirementsGrid.AutoGenerateColumns = false;
            _requirementsGrid.Columns.Add(CreateTextColumn("RequirementId", "RequirementId", 95));
            _requirementsGrid.Columns.Add(CreateTextColumn("Rows", "RecordCount", 60));
            _requirementsGrid.Columns.Add(CreateTextColumn("Fields", "FieldCount", 60));
            _requirementsGrid.Columns.Add(CreateTextColumn("Abilities", "DirectAbilityCount", 70));
            _requirementsGrid.Columns.Add(CreateTextColumn("Components", "DirectComponentCount", 80));
            _requirementsGrid.Columns.Add(CreateTextColumn("Parents", "ParentRequirementCount", 70));
            _requirementsGrid.Columns.Add(CreateTextColumn("Children", "ChildRequirementCount", 70));
            _requirementsGrid.Columns.Add(CreateTextColumn("Tags", "ContextTagsText", 160));
            _requirementsGrid.Columns.Add(CreateTextColumn("Summary", "SemanticSummary", 420));
            _requirementsGrid.SelectionChanged += (sender, args) => RefreshRequirementDetails();
            split.Panel1.Controls.Add(WrapInGroup("Requirement Summary", _requirementsGrid));

            TabControl details = new TabControl { Dock = DockStyle.Fill };
            _requirementRowGrid = CreateReadOnlyGrid();
            _requirementRowGrid.AutoGenerateColumns = false;
            _requirementRowGrid.Columns.Add(CreateTextColumn("RecordIndex", "RecordIndex", 90));
            _requirementRowGrid.Columns.Add(CreateTextColumn("ExtData", "ExtDataText", 520));
            _requirementRowGrid.Columns.Add(CreateTextColumn("Path", "SourcePath", 300));
            _requirementRowGrid.Columns.Add(CreateTextColumn("Location", "SourceLocation", 90));
            TabPage rowsTab = new TabPage("Rows");
            rowsTab.Controls.Add(_requirementRowGrid);
            details.TabPages.Add(rowsTab);

            _requirementFieldGrid = CreateReadOnlyGrid();
            _requirementFieldGrid.AutoGenerateColumns = false;
            _requirementFieldGrid.Columns.Add(CreateTextColumn("Field", "FieldKey", 160));
            _requirementFieldGrid.Columns.Add(CreateTextColumn("NonZero", "NonZeroCount", 70));
            _requirementFieldGrid.Columns.Add(CreateTextColumn("Distinct", "DistinctValueCount", 70));
            _requirementFieldGrid.Columns.Add(CreateTextColumn("SampleValues", "SampleValuesText", 220));
            _requirementFieldGrid.Columns.Add(CreateTextColumn("Meaning", "SemanticSummary", 260));
            _requirementFieldGrid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            _requirementFieldGrid.Columns.Add(CreateTextColumn("Notes", "Notes", 260));
            TabPage fieldsTab = new TabPage("Fields");
            fieldsTab.Controls.Add(_requirementFieldGrid);
            details.TabPages.Add(fieldsTab);

            _requirementReferenceGrid = CreateReadOnlyGrid();
            _requirementReferenceGrid.AutoGenerateColumns = false;
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Direction", "Direction", 80));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("SourceKind", "SourceKind", 90));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("SourceId", "SourceId", 85));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("SourceLabel", "SourceLabel", 220));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Field", "SourceField", 130));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("LinkedRequirement", "LinkedRequirementId", 95));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Abilities", "RelatedAbilitiesText", 220));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Tags", "ContextTagsText", 160));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Path", "SourcePath", 260));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Location", "SourceLocation", 90));
            _requirementReferenceGrid.Columns.Add(CreateTextColumn("Notes", "Notes", 260));
            TabPage linksTab = new TabPage("Links");
            linksTab.Controls.Add(_requirementReferenceGrid);
            details.TabPages.Add(linksTab);

            split.Panel2.Controls.Add(details);

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_requirementsSummaryTextBox, 0, 1);
            layout.Controls.Add(split, 0, 2);
            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage CreateOperationTab()
        {
            TabPage tab = new TabPage("Operation Schemas");
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _generateOperationSchemasButton = new Button { Text = "Generate Operation Schemas", AutoSize = true, Enabled = false };
            _generateOperationSchemasButton.Click += async (sender, args) => await GenerateOperationSchemaReportAsync();
            actions.Controls.Add(_generateOperationSchemasButton);
            _openOperationSchemasMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openOperationSchemasMarkdownButton.Click += (sender, args) => OpenPath(_lastOperationSchemaMarkdownPath);
            actions.Controls.Add(_openOperationSchemasMarkdownButton);
            _openOperationSchemasFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openOperationSchemasFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastOperationSchemaMarkdownPath) ? null : Path.GetDirectoryName(_lastOperationSchemaMarkdownPath));
            actions.Controls.Add(_openOperationSchemasFolderButton);

            _operationSummaryTextBox = CreateReadOnlyTextBox();
            _operationSummaryTextBox.Text = "Generate operation schemas to inspect component operations, their non-zero fields, semantic hints, and sample abilities.";

            SplitContainer outerSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 460 };
            _operationGrid = CreateReadOnlyGrid();
            _operationGrid.AutoGenerateColumns = false;
            _operationGrid.Columns.Add(CreateTextColumn("OperationId", "OperationId", 90));
            _operationGrid.Columns.Add(CreateTextColumn("OperationName", "OperationName", 180));
            _operationGrid.Columns.Add(CreateTextColumn("Priority", "PriorityText", 80));
            _operationGrid.Columns.Add(CreateTextColumn("Components", "ComponentCount", 90));
            _operationGrid.Columns.Add(CreateTextColumn("Abilities", "AbilityCount", 90));
            _operationGrid.Columns.Add(CreateTextColumn("Tags", "ContextTagsText", 180));
            _operationGrid.SelectionChanged += (sender, args) => RefreshOperationDetails();
            outerSplit.Panel1.Controls.Add(WrapInGroup("Operations", _operationGrid));

            SplitContainer innerSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 290 };
            _operationFieldGrid = CreateReadOnlyGrid();
            _operationFieldGrid.AutoGenerateColumns = false;
            _operationFieldGrid.Columns.Add(CreateTextColumn("Field", "FieldKey", 170));
            _operationFieldGrid.Columns.Add(CreateTextColumn("NonZero", "NonZeroCount", 70));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Distinct", "DistinctValueCount", 70));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Sample Values", "SampleValuesText", 220));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Tokens", "TokenRenderingsText", 220));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Meaning", "SemanticSummary", 360));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Tags", "ContextTagsText", 160));
            _operationFieldGrid.Columns.Add(CreateTextColumn("Notes", "Notes", 260));
            innerSplit.Panel1.Controls.Add(WrapInGroup("Field Schema", _operationFieldGrid));

            _operationAbilityGrid = CreateReadOnlyGrid();
            _operationAbilityGrid.AutoGenerateColumns = false;
            _operationAbilityGrid.Columns.Add(CreateTextColumn("AbilityId", "AbilityId", 90));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("AbilityName", "AbilityName", 170));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("ComponentId", "ComponentId", 95));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("Slot", "ComponentSlotIndex", 60));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("Trigger", "TriggerText", 170));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("Tags", "ContextTagsText", 150));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("TextExcerpt", "TextExcerpt", 420));
            _operationAbilityGrid.Columns.Add(CreateTextColumn("Source", "SourceLocation", 90));
            innerSplit.Panel2.Controls.Add(WrapInGroup("Sample Abilities", _operationAbilityGrid));

            outerSplit.Panel2.Controls.Add(innerSplit);

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_operationSummaryTextBox, 0, 1);
            layout.Controls.Add(outerSplit, 0, 2);
            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage CreateCoverageTab()
        {
            TabPage tab = new TabPage("Coverage");
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _generateCoverageButton = new Button { Text = "Generate Coverage Report", AutoSize = true, Enabled = false };
            _generateCoverageButton.Click += async (sender, args) => await GenerateCoverageReportAsync();
            actions.Controls.Add(_generateCoverageButton);
            _openCoverageMarkdownButton = new Button { Text = "Open Markdown", AutoSize = true, Enabled = false };
            _openCoverageMarkdownButton.Click += (sender, args) => OpenPath(_lastCoverageMarkdownPath);
            actions.Controls.Add(_openCoverageMarkdownButton);
            _openCoverageFolderButton = new Button { Text = "Open Output Folder", AutoSize = true, Enabled = false };
            _openCoverageFolderButton.Click += (sender, args) => OpenPath(string.IsNullOrWhiteSpace(_lastCoverageMarkdownPath) ? null : Path.GetDirectoryName(_lastCoverageMarkdownPath));
            actions.Controls.Add(_openCoverageFolderButton);

            _coverageSummaryTextBox = CreateReadOnlyTextBox();
            _coverageSummaryTextBox.Text = "Generate the coverage report to see which abilities are fully mapped and which still have extracted-client gaps.";

            _coverageGrid = CreateReadOnlyGrid();
            _coverageGrid.AutoGenerateColumns = false;
            _coverageGrid.Columns.Add(CreateTextColumn("AbilityId", "AbilityId", 90));
            _coverageGrid.Columns.Add(CreateTextColumn("Name", "Name", 210));
            _coverageGrid.Columns.Add(CreateTextColumn("Status", "CoverageStatus", 140));
            _coverageGrid.Columns.Add(CreateTextColumn("EffectId", "EffectIdText", 80));
            _coverageGrid.Columns.Add(CreateTextColumn("Csv", "HasClientCsvText", 55));
            _coverageGrid.Columns.Add(CreateTextColumn("Bin", "HasClientBinText", 55));
            _coverageGrid.Columns.Add(CreateTextColumn("NameText", "HasLocalizedNameText", 75));
            _coverageGrid.Columns.Add(CreateTextColumn("DescText", "HasDescriptionTextText", 75));
            _coverageGrid.Columns.Add(CreateTextColumn("EffectText", "HasEffectTextText", 75));
            _coverageGrid.Columns.Add(CreateTextColumn("EffectRow", "HasRootEffectRowText", 75));
            _coverageGrid.Columns.Add(CreateTextColumn("Components", "ComponentCount", 85));
            _coverageGrid.Columns.Add(CreateTextColumn("ReqLinks", "RequirementLinkCount", 75));
            _coverageGrid.Columns.Add(CreateTextColumn("ReqRows", "RequirementRowCount", 70));
            _coverageGrid.Columns.Add(CreateTextColumn("Sources", "SourcesText", 170));
            _coverageGrid.Columns.Add(CreateTextColumn("Missing", "MissingText", 220));

            layout.Controls.Add(actions, 0, 0);
            layout.Controls.Add(_coverageSummaryTextBox, 0, 1);
            layout.Controls.Add(_coverageGrid, 0, 2);
            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage CreateStatusTab()
        {
            TabPage tab = new TabPage("Source Status");
            _statusGrid = CreateReadOnlyGrid();
            _statusGrid.AutoGenerateColumns = false;
            _statusGrid.Columns.Add(CreateTextColumn("Source", "SourceFamily", 110));
            _statusGrid.Columns.Add(CreateTextColumn("File", "TableName", 180));
            _statusGrid.Columns.Add(CreateTextColumn("Loaded", "Loaded", 70));
            _statusGrid.Columns.Add(CreateTextColumn("Rows", "RowCount", 90));
            _statusGrid.Columns.Add(CreateTextColumn("Path", "SourcePath", 500));
            _statusGrid.Columns.Add(CreateTextColumn("Error", "ErrorMessage", 320));
            tab.Controls.Add(_statusGrid);
            return tab;
        }

        private TabPage CreateLogTab()
        {
            TabPage tab = new TabPage("Log");
            _logTextBox = CreateReadOnlyTextBox();
            _logTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            _logTextBox.Text = "Session log will appear here." + Environment.NewLine;
            tab.Controls.Add(_logTextBox);
            return tab;
        }

        private async Task LoadSessionAsync()
        {
            if (_isBusy)
                return;

            try
            {
                SetBusy(true, "Loading extracted client dataset...");
                _session = await Task.Run(() => MatrixAnalysisSession.Load(_rootPathTextBox.Text.Trim()));
                _definitions = new DefinitionCatalog(_session.Dataset);
                _narrativeBuilder = new AbilityNarrativeBuilder(_definitions);
                _abilityCatalog = _session.BuildAbilityCatalog();
                _lastAbilityReport = null;
                _lastTokenDictionary = null;
                _lastCoverageReport = null;
                _lastDomainLedger = null;
                _lastRequirementLedger = null;
                _lastOperationSchema = null;
                _lastConflictReport = null;
                _lastAbilityMarkdownPath = null;
                _lastTokenDictionaryMarkdownPath = null;
                _lastCoverageMarkdownPath = null;
                _lastDomainLedgerMarkdownPath = null;
                _lastRequirementMarkdownPath = null;
                _lastOperationSchemaMarkdownPath = null;
                _lastConflictMarkdownPath = null;
                _openAbilityMarkdownButton.Enabled = false;
                _openAbilityFolderButton.Enabled = false;
                _openTokenDictionaryMarkdownButton.Enabled = false;
                _openTokenDictionaryFolderButton.Enabled = false;
                _openCoverageMarkdownButton.Enabled = false;
                _openCoverageFolderButton.Enabled = false;
                _openDomainsMarkdownButton.Enabled = false;
                _openDomainsFolderButton.Enabled = false;
                _openRequirementsMarkdownButton.Enabled = false;
                _openRequirementsFolderButton.Enabled = false;
                _openOperationSchemasMarkdownButton.Enabled = false;
                _openOperationSchemasFolderButton.Enabled = false;
                _openConflictMarkdownButton.Enabled = false;
                _openConflictFolderButton.Enabled = false;

                _statusGrid.DataSource = _session.Dataset.TableStatuses.OrderBy(x => x.SourceFamily).ThenBy(x => x.TableName).ToList();
                ApplyAbilityFilter();
                ClearAbilityPresentation(BuildDatasetSummary());
                _tokenSummaryTextBox.Text = "Generate the COM token dictionary to write plain-English token definitions from extracted client evidence.";
                _tokenGrid.DataSource = null;
                _domainsSummaryTextBox.Text = "Generate the core identity domain ledger to separate proven extracted-client domains such as Race.EntryId, CareerLine.EntryId, and repeated CareerName entry ids.";
                _domainsGrid.DataSource = null;
                _domainValuesGrid.DataSource = null;
                _requirementsSummaryTextBox.Text = "Generate the requirement ledger to inspect inbound links, child requirement chains, and active ext-data fields.";
                _requirementsGrid.DataSource = null;
                _requirementRowGrid.DataSource = null;
                _requirementFieldGrid.DataSource = null;
                _requirementReferenceGrid.DataSource = null;
                _coverageSummaryTextBox.Text = "Generate the coverage report to see which abilities are fully mapped and which still have extracted-client gaps.";
                _coverageGrid.DataSource = null;
                _operationSummaryTextBox.Text = "Generate operation schemas to inspect component operations, their non-zero fields, semantic hints, and sample abilities.";
                _operationGrid.DataSource = null;
                _operationFieldGrid.DataSource = null;
                _operationAbilityGrid.DataSource = null;
                _conflictSummaryTextBox.Text = "Generate the conflict ledger to browse contradiction domains.";
                _conflictDomainGrid.DataSource = null;
                _conflictGrid.DataSource = null;

                AppendLog("Loaded dataset from " + _session.ExtractedRootPath + ".");
                AppendLog("Catalog contains " + _abilityCatalog.Count.ToString(CultureInfo.InvariantCulture) + " unique ability ids.");
                SetBusy(false, "Dataset loaded.");
            }
            catch (Exception ex)
            {
                _session = null;
                _definitions = null;
                _narrativeBuilder = null;
                _abilityCatalog = new List<AbilityCatalogEntry>();
                _lastDomainLedger = null;
                _lastDomainLedgerMarkdownPath = null;
                _lastRequirementLedger = null;
                _lastRequirementMarkdownPath = null;
                _abilityGrid.DataSource = null;
                _statusGrid.DataSource = null;
                _abilityCatalogStatusLabel.Text = "Dataset load failed.";
                ClearAbilityPresentation(ex.ToString());
                _tokenGrid.DataSource = null;
                _domainsGrid.DataSource = null;
                _domainValuesGrid.DataSource = null;
                _domainsSummaryTextBox.Text = "Dataset load failed. Fix the extracted root path and reload.";
                _openDomainsMarkdownButton.Enabled = false;
                _openDomainsFolderButton.Enabled = false;
                _requirementsGrid.DataSource = null;
                _requirementRowGrid.DataSource = null;
                _requirementFieldGrid.DataSource = null;
                _requirementReferenceGrid.DataSource = null;
                _requirementsSummaryTextBox.Text = "Dataset load failed. Fix the extracted root path and reload.";
                _openRequirementsMarkdownButton.Enabled = false;
                _openRequirementsFolderButton.Enabled = false;
                _coverageGrid.DataSource = null;
                _operationGrid.DataSource = null;
                _operationFieldGrid.DataSource = null;
                _operationAbilityGrid.DataSource = null;
                _conflictSummaryTextBox.Text = "Dataset load failed. Fix the extracted root path and reload.";
                AppendLog("Dataset load failed: " + ex.Message);
                SetBusy(false, "Dataset load failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> GenerateAbilityReportAsync()
        {
            if (_isBusy || _session == null)
                return false;

            ushort abilityId;
            if (!TryGetSelectedAbilityId(out abilityId))
            {
                MessageBox.Show(this, "Ability ID must be a valid unsigned 16-bit integer.", "ClientDataMatrix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                SetBusy(true, "Generating ability report for " + abilityId.ToString(CultureInfo.InvariantCulture) + "...");
                string outputRoot = ResolveOutputRoot();
                AbilityAnalysisResult report = await Task.Run(() => _session.BuildAbilityAnalysis(abilityId));
                _lastAbilityMarkdownPath = _session.WriteAbilityReport(outputRoot, report);
                _lastAbilityReport = report;
                _openAbilityMarkdownButton.Enabled = File.Exists(_lastAbilityMarkdownPath);
                _openAbilityFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastAbilityMarkdownPath));
                PopulateAbilityPresentation(report, _lastAbilityMarkdownPath);
                AppendLog("Ability " + abilityId.ToString(CultureInfo.InvariantCulture) + " report written to " + _lastAbilityMarkdownPath + ".");
                SetBusy(false, "Ability report generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Ability report failed: " + ex.Message);
                SetBusy(false, "Ability report failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Ability Report Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> GenerateConflictReportAsync()
        {
            if (_isBusy || _session == null)
                return false;

            try
            {
                SetBusy(true, "Generating conflict report...");
                string outputRoot = ResolveOutputRoot();
                ConflictReportDocument report = await Task.Run(() => _session.BuildConflictReport());
                _lastConflictMarkdownPath = _session.WriteConflictReport(outputRoot, report);
                _lastConflictReport = report;
                _openConflictMarkdownButton.Enabled = File.Exists(_lastConflictMarkdownPath);
                _openConflictFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastConflictMarkdownPath));
                _conflictSummaryTextBox.Text = BuildConflictSummary(report, _lastConflictMarkdownPath);
                _conflictDomainGrid.DataSource = BuildDomainRows(report);
                SelectFirstRow(_conflictDomainGrid);
                RefreshConflictRows();
                AppendLog("Conflict report written to " + _lastConflictMarkdownPath + ".");
                SetBusy(false, "Conflict report generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Conflict report failed: " + ex.Message);
                SetBusy(false, "Conflict report failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Conflict Report Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> GenerateTokenDictionaryAsync()
        {
            if (_isBusy || _session == null)
                return false;

            try
            {
                SetBusy(true, "Generating token dictionary...");
                string outputRoot = ResolveOutputRoot();
                TokenDictionaryDocument report = await Task.Run(() => _session.BuildTokenDictionary());
                _lastTokenDictionaryMarkdownPath = await Task.Run(() => _session.WriteTokenDictionaryReport(outputRoot, report));
                _lastTokenDictionary = report;
                _openTokenDictionaryMarkdownButton.Enabled = File.Exists(_lastTokenDictionaryMarkdownPath);
                _openTokenDictionaryFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastTokenDictionaryMarkdownPath));
                _tokenSummaryTextBox.Text = BuildTokenDictionarySummary(report, _lastTokenDictionaryMarkdownPath);
                _tokenGrid.DataSource = report.Definitions == null
                    ? new List<TokenDefinitionListRow>()
                    : report.Definitions.Select(row => new TokenDefinitionListRow
                    {
                        ExampleToken = row.ExampleToken,
                        FieldKey = row.FieldKey,
                        PlainEnglishMeaning = row.PlainEnglishMeaning,
                        Confidence = row.Confidence,
                        ContextTagsText = row.ContextTags == null ? string.Empty : string.Join(", ", row.ContextTags),
                        ExampleAbilityIdsText = row.ExampleAbilityIds == null ? string.Empty : string.Join(", ", row.ExampleAbilityIds.Select(value => value.ToString(CultureInfo.InvariantCulture))),
                        Notes = row.Notes
                    }).ToList();
                AppendLog("Token dictionary written to " + _lastTokenDictionaryMarkdownPath + ".");
                SetBusy(false, "Token dictionary generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Token dictionary failed: " + ex.Message);
                SetBusy(false, "Token dictionary failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Token Dictionary Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> GenerateOperationSchemaReportAsync()
        {
            if (_isBusy || _session == null)
                return false;

            try
            {
                SetBusy(true, "Generating operation schemas...");
                string outputRoot = ResolveOutputRoot();
                OperationSchemaDocument report = await Task.Run(() => _session.BuildOperationSchemas());
                _lastOperationSchemaMarkdownPath = await Task.Run(() => _session.WriteOperationSchemaReport(outputRoot, report));
                _lastOperationSchema = report;
                _openOperationSchemasMarkdownButton.Enabled = File.Exists(_lastOperationSchemaMarkdownPath);
                _openOperationSchemasFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastOperationSchemaMarkdownPath));
                _operationSummaryTextBox.Text = BuildOperationSummary(report, _lastOperationSchemaMarkdownPath);
                _operationGrid.DataSource = report.Operations == null
                    ? new List<OperationSchemaListRow>()
                    : report.Operations.Select(row => new OperationSchemaListRow
                    {
                        OperationId = row.OperationId,
                        OperationName = row.OperationName,
                        PriorityText = row.IsPriority ? "Yes" : "No",
                        ComponentCount = row.ComponentCount,
                        AbilityCount = row.AbilityCount,
                        ContextTagsText = row.ContextTags == null ? string.Empty : string.Join(", ", row.ContextTags),
                        LayoutVariantsText = row.LayoutVariantsText
                    }).ToList();
                SelectFirstRow(_operationGrid);
                RefreshOperationDetails();
                AppendLog("Operation schema report written to " + _lastOperationSchemaMarkdownPath + ".");
                SetBusy(false, "Operation schemas generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Operation schema report failed: " + ex.Message);
                SetBusy(false, "Operation schema report failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Operation Schemas Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> GenerateDomainLedgerAsync()
        {
            if (_isBusy || _session == null)
                return false;

            try
            {
                SetBusy(true, "Generating core identity domains...");
                string outputRoot = ResolveOutputRoot();
                DomainLedgerDocument report = await Task.Run(() => _session.BuildDomainLedger());
                _lastDomainLedgerMarkdownPath = await Task.Run(() => _session.WriteDomainLedgerReport(outputRoot, report));
                _lastDomainLedger = report;
                _openDomainsMarkdownButton.Enabled = File.Exists(_lastDomainLedgerMarkdownPath);
                _openDomainsFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastDomainLedgerMarkdownPath));
                _domainsSummaryTextBox.Text = BuildDomainSummary(report, _lastDomainLedgerMarkdownPath);
                _domainsGrid.DataSource = report.Domains == null
                    ? new List<DomainListRow>()
                    : report.Domains.Select(row => new DomainListRow
                    {
                        DomainKey = row.DomainKey,
                        DisplayName = row.DisplayName,
                        Confidence = row.Confidence,
                        Canonicality = row.Canonicality,
                        ValueCount = row.ValueCount,
                        DistinctMeaningCount = row.DistinctMeaningCount,
                        DuplicateMeaningCount = row.DuplicateMeaningCount,
                        SourceFilesText = row.SourceFilesText,
                        RecommendedUsage = row.RecommendedUsage,
                        Notes = row.Notes
                    }).ToList();
                SelectFirstRow(_domainsGrid);
                RefreshDomainValues();
                AppendLog("Domain ledger written to " + _lastDomainLedgerMarkdownPath + ".");
                SetBusy(false, "Core identity domains generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Domain ledger failed: " + ex.Message);
                SetBusy(false, "Core identity domains failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Domain Ledger Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> GenerateRequirementLedgerAsync()
        {
            if (_isBusy || _session == null)
                return false;

            try
            {
                SetBusy(true, "Generating requirement ledger...");
                string outputRoot = ResolveOutputRoot();
                RequirementLedgerDocument report = await Task.Run(() => _session.BuildRequirementLedger());
                _lastRequirementMarkdownPath = await Task.Run(() => _session.WriteRequirementLedgerReport(outputRoot, report));
                _lastRequirementLedger = report;
                _openRequirementsMarkdownButton.Enabled = File.Exists(_lastRequirementMarkdownPath);
                _openRequirementsFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastRequirementMarkdownPath));
                _requirementsSummaryTextBox.Text = BuildRequirementSummary(report, _lastRequirementMarkdownPath);
                _requirementsGrid.DataSource = report.Requirements == null
                    ? new List<RequirementListRow>()
                    : report.Requirements.Select(row => new RequirementListRow
                    {
                        RequirementId = row.RequirementId,
                        RecordCount = row.RecordCount,
                        FieldCount = row.FieldCount,
                        DirectAbilityCount = row.DirectAbilityCount,
                        DirectComponentCount = row.DirectComponentCount,
                        ParentRequirementCount = row.ParentRequirementCount,
                        ChildRequirementCount = row.ChildRequirementCount,
                        ContextTagsText = row.ContextTagsText,
                        SampleAbilitiesText = row.SampleAbilitiesText,
                        SemanticSummary = row.SemanticSummary,
                        Notes = row.Notes
                    }).ToList();
                SelectFirstRow(_requirementsGrid);
                RefreshRequirementDetails();
                AppendLog("Requirement ledger written to " + _lastRequirementMarkdownPath + ".");
                SetBusy(false, "Requirement ledger generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Requirement ledger failed: " + ex.Message);
                SetBusy(false, "Requirement ledger failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Requirement Ledger Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> GenerateCoverageReportAsync()
        {
            if (_isBusy || _session == null)
                return false;

            try
            {
                SetBusy(true, "Generating coverage report...");
                string outputRoot = ResolveOutputRoot();
                CoverageReportDocument report = await Task.Run(() => _session.BuildCoverageReport());
                _lastCoverageMarkdownPath = await Task.Run(() => _session.WriteCoverageReport(outputRoot, report));
                _lastCoverageReport = report;
                _openCoverageMarkdownButton.Enabled = File.Exists(_lastCoverageMarkdownPath);
                _openCoverageFolderButton.Enabled = Directory.Exists(Path.GetDirectoryName(_lastCoverageMarkdownPath));
                _coverageSummaryTextBox.Text = BuildCoverageSummary(report, _lastCoverageMarkdownPath);
                _coverageGrid.DataSource = report.Abilities == null
                    ? new List<CoverageListRow>()
                    : report.Abilities.Select(row => new CoverageListRow
                    {
                        AbilityId = row.AbilityId,
                        Name = row.Name,
                        CoverageStatus = row.CoverageStatus,
                        EffectIdText = row.EffectId.HasValue ? row.EffectId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                        HasClientCsvText = row.HasClientCsv ? "Yes" : "No",
                        HasClientBinText = row.HasClientBin ? "Yes" : "No",
                        HasLocalizedNameText = row.HasLocalizedName ? "Yes" : "No",
                        HasDescriptionTextText = row.HasDescriptionText ? "Yes" : "No",
                        HasEffectTextText = row.HasEffectText ? "Yes" : "No",
                        HasRootEffectRowText = row.HasRootEffectRow ? "Yes" : "No",
                        ComponentCount = row.ComponentCount,
                        RequirementLinkCount = row.RequirementLinkCount,
                        RequirementRowCount = row.RequirementRowCount,
                        SourcesText = row.SourcesText,
                        MissingText = row.MissingText
                    }).ToList();
                AppendLog("Coverage report written to " + _lastCoverageMarkdownPath + ".");
                SetBusy(false, "Coverage report generated.");
                return true;
            }
            catch (Exception ex)
            {
                AppendLog("Coverage report failed: " + ex.Message);
                SetBusy(false, "Coverage report failed.");
                MessageBox.Show(this, ex.ToString(), "ClientDataMatrix Coverage Report Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task GenerateAllReportsAsync()
        {
            if (_isBusy || _session == null)
                return;

            AppendLog("Generate All started.");

            if (!await GenerateDomainLedgerAsync())
                return;
            if (!await GenerateRequirementLedgerAsync())
                return;
            if (!await GenerateTokenDictionaryAsync())
                return;
            if (!await GenerateCoverageReportAsync())
                return;
            if (!await GenerateOperationSchemaReportAsync())
                return;
            if (!await GenerateConflictReportAsync())
                return;

            ushort abilityId;
            if (TryGetSelectedAbilityId(out abilityId))
            {
                if (!await GenerateAbilityReportAsync())
                    return;
            }
            else
                AppendLog("Generate All skipped ability report because no valid ability id is currently selected.");

            _statusLabel.Text = "Generate All finished.";
            AppendLog("Generate All completed.");
        }

        private bool TryGetSelectedAbilityId(out ushort abilityId)
        {
            return ushort.TryParse(_abilityIdTextBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out abilityId);
        }

        private void PopulateAbilityPresentation(AbilityAnalysisResult report, string markdownPath)
        {
            _abilitySummaryTextBox.Text = BuildAbilitySummary(report, markdownPath);
            _abilityNarrativeTextBox.Text = _narrativeBuilder == null ? "Narrative builder is not initialized." : _narrativeBuilder.BuildNarrative(report);
            _definitionGrid.DataSource = _definitions == null ? new List<DefinitionEntry>() : _definitions.BuildAbilityDefinitions(report);
            UpdateDefinitionHint();
            BuildAbilityTree(report);
        }

        private void ClearAbilityPresentation(string summary)
        {
            _abilitySummaryTextBox.Text = summary;
            _abilityNarrativeTextBox.Text = "Generate an ability report to see what we think happens when the ability is used.";
            _definitionGrid.DataSource = null;
            UpdateDefinitionHint();
            _abilityTreeView.Nodes.Clear();
            _abilityTreeView.Nodes.Add(new TreeNode("Generate an ability report to populate this tree."));
        }

        private void BuildAbilityTree(AbilityAnalysisResult report)
        {
            _abilityTreeView.BeginUpdate();
            _abilityTreeView.Nodes.Clear();

            TreeNode root = new TreeNode(GetAbilityName(report) + " [" + report.AbilityId.ToString(CultureInfo.InvariantCulture) + "]");
            TreeNode summary = root.Nodes.Add("Summary");
            summary.Nodes.Add("Client rows: " + report.ClientAbilityRows.Count.ToString(CultureInfo.InvariantCulture));
            summary.Nodes.Add("BIN rows: " + report.BinaryAbilityRows.Count.ToString(CultureInfo.InvariantCulture));
            summary.Nodes.Add("Effect rows: " + report.ClientEffectRows.Count.ToString(CultureInfo.InvariantCulture));
            summary.Nodes.Add("Component rows: " + report.BinaryComponentRows.Count.ToString(CultureInfo.InvariantCulture));
            summary.Nodes.Add("Requirement links: " + (report.RequirementReferences == null ? 0 : report.RequirementReferences.Count).ToString(CultureInfo.InvariantCulture));
            summary.Nodes.Add("Requirement rows: " + report.BinaryRequirementRows.Count.ToString(CultureInfo.InvariantCulture));

            TreeNode warnings = root.Nodes.Add("Warnings");
            if (report.Warnings.Count == 0)
                warnings.Nodes.Add("No warnings.");
            else
                foreach (string warning in report.Warnings)
                    warnings.Nodes.Add(warning);

            TreeNode csv = root.Nodes.Add("abilities.csv");
            if (report.ClientAbilityRows.Count == 0)
                csv.Nodes.Add("No client CSV row found.");
            foreach (ClientAbilityRecord row in report.ClientAbilityRows)
            {
                TreeNode node = csv.Nodes.Add("Line " + row.LineNumber.ToString(CultureInfo.InvariantCulture));
                node.Nodes.Add("Name: " + NullToPlaceholder(row.Name));
                node.Nodes.Add("Description: " + NullToPlaceholder(row.Description));
                node.Nodes.Add("EffectId: " + row.EffectId.ToString(CultureInfo.InvariantCulture));
            }

            TreeNode bin = root.Nodes.Add("abilityexport.bin");
            if (report.BinaryAbilityRows.Count == 0)
                bin.Nodes.Add("No BIN row found.");
            foreach (BinaryAbilityRecord row in report.BinaryAbilityRows)
            {
                TreeNode node = bin.Nodes.Add("Record " + row.RecordIndex.ToString(CultureInfo.InvariantCulture) + " @ byte " + row.ByteOffset.ToString(CultureInfo.InvariantCulture));
                node.Nodes.Add("EffectId: " + row.EffectId.ToString(CultureInfo.InvariantCulture) + " -> " + EffectName(report, row.EffectId));
                node.Nodes.Add("CareerLine: " + row.CareerLine.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeCareerLine(row.CareerLine));
                node.Nodes.Add("TargetType: " + row.TargetType.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeTargetType(row.TargetType));
                node.Nodes.Add("AbilityType: " + row.AbilityType.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeAbilityType(row.AbilityType));
                node.Nodes.Add("AttackType: " + row.AttackType.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeAttackType(row.AttackType));
                node.Nodes.Add("TacticType: " + row.TacticType.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeTacticType(row.TacticType));
                node.Nodes.Add("CastTime: " + row.CastTime.ToString(CultureInfo.InvariantCulture) + " ms");
                node.Nodes.Add("Cooldown: " + row.Cooldown.ToString(CultureInfo.InvariantCulture) + " ms");
                node.Nodes.Add("Range: " + row.Range.ToString(CultureInfo.InvariantCulture) + " feet");
                node.Nodes.Add("AP Cost: " + row.ApCost.ToString(CultureInfo.InvariantCulture));
                TreeNode comp = node.Nodes.Add("Component Slots");
                for (int index = 0; index < row.ComponentIds.Count; ++index)
                {
                    ushort componentId = row.ComponentIds[index];
                    uint triggerValue = index < row.Triggers.Count ? row.Triggers[index] : 0;
                    if (componentId <= 0 && triggerValue == 0)
                        continue;
                    string text = "Slot " + index.ToString(CultureInfo.InvariantCulture) + ": Component " + componentId.ToString(CultureInfo.InvariantCulture);
                    BinaryComponentRecord componentRow = report.BinaryComponentRows.FirstOrDefault(x => x.ComponentId == componentId);
                    if (componentRow != null)
                        text += " -> " + _definitions.DescribeComponentOperation(componentRow.Operation);
                    if (triggerValue > 0)
                        text += " | Trigger " + triggerValue.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeTrigger(triggerValue);
                    comp.Nodes.Add(text);
                }
            }

            TreeNode effects = root.Nodes.Add("effects.csv");
            if (report.ClientEffectRows.Count == 0)
                effects.Nodes.Add("No effect rows found.");
            foreach (ClientEffectRecord row in report.ClientEffectRows.OrderBy(x => x.EffectId))
            {
                TreeNode node = effects.Nodes.Add("Effect " + row.EffectId.ToString(CultureInfo.InvariantCulture) + " - " + NullToPlaceholder(row.Name));
                node.Nodes.Add("BuildUp: " + TextOrNone(row.BuildUpId));
                node.Nodes.Add("Cast: " + TextOrNone(row.CastId));
                node.Nodes.Add("Activate: " + TextOrNone(row.ActivateId));
                node.Nodes.Add("Projectile: " + TextOrNone(row.ProjectileId));
                node.Nodes.Add("Impact: " + TextOrNone(row.ImpactId));
                node.Nodes.Add("AOE: " + TextOrNone(row.AoeId));
                node.Nodes.Add("Channel: " + TextOrNone(row.ChannelingId));
            }

            TreeNode components = root.Nodes.Add("Components");
            if (report.BinaryComponentRows.Count == 0)
                components.Nodes.Add("No component rows found.");
            foreach (BinaryComponentRecord row in report.BinaryComponentRows.OrderBy(x => x.ComponentId))
            {
                TreeNode node = components.Nodes.Add("Component " + row.ComponentId.ToString(CultureInfo.InvariantCulture) + " -> " + _definitions.DescribeComponentOperation(row.Operation));
                node.Nodes.Add("Duration: " + row.Duration.ToString(CultureInfo.InvariantCulture) + " ms");
                node.Nodes.Add("Interval: " + row.Interval.ToString(CultureInfo.InvariantCulture) + " ms");
                node.Nodes.Add("Radius: " + row.Radius.ToString(CultureInfo.InvariantCulture) + " feet");
                node.Nodes.Add("Description: " + NullToPlaceholder(row.Description));
                node.Nodes.Add("Values: " + string.Join(", ", row.Values.Select(x => x.ToString(CultureInfo.InvariantCulture))));
            }

            TreeNode requirementLinks = root.Nodes.Add("Requirement Links");
            if (report.RequirementReferences == null || report.RequirementReferences.Count == 0)
                requirementLinks.Nodes.Add("No inferred requirement links found.");
            else
            {
                foreach (RequirementReferenceRecord reference in report.RequirementReferences.OrderBy(x => x.SourceKind).ThenBy(x => x.SourceId).ThenBy(x => x.ExtSlotIndex).ThenBy(x => x.RequirementId))
                {
                    TreeNode node = requirementLinks.Nodes.Add(reference.SourceLabel + " -> Requirement " + reference.RequirementId.ToString(CultureInfo.InvariantCulture));
                    node.Nodes.Add("Field: " + reference.SourceField);
                    node.Nodes.Add("Confidence: " + reference.Confidence);
                    node.Nodes.Add("Location: " + reference.SourceLocation);
                    node.Nodes.Add("Notes: " + reference.Notes);
                }
            }

            TreeNode requirements = root.Nodes.Add("Requirements");
            if (report.BinaryRequirementRows == null || report.BinaryRequirementRows.Count == 0)
                requirements.Nodes.Add("No linked requirement rows found.");
            else
            {
                foreach (BinaryRequirementRecord row in report.BinaryRequirementRows.OrderBy(x => x.RequirementId).ThenBy(x => x.RecordIndex))
                {
                    TreeNode node = requirements.Nodes.Add("Requirement " + row.RequirementId.ToString(CultureInfo.InvariantCulture) + " @ byte " + row.ByteOffset.ToString(CultureInfo.InvariantCulture));
                    foreach (BinaryExtDataRecord ext in row.ExtData)
                    {
                        node.Nodes.Add("ExtData[" + ext.SlotIndex.ToString(CultureInfo.InvariantCulture) + "]: [" + ext.Val1.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val2.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val3.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val4.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val5.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val6.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val7.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val8.ToString(CultureInfo.InvariantCulture) + ", " + ext.Val9.ToString(CultureInfo.InvariantCulture) + "]");
                    }
                }
            }

            _abilityTreeView.Nodes.Add(root);
            root.Expand();
            foreach (TreeNode child in root.Nodes)
                child.Expand();
            _abilityTreeView.EndUpdate();
        }

        private void ApplyAbilityFilter()
        {
            string filter = (_abilitySearchTextBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            List<AbilityCatalogEntry> filtered = string.IsNullOrWhiteSpace(filter)
                ? _abilityCatalog.OrderBy(x => x.AbilityId).ToList()
                : _abilityCatalog.Where(x => x.SearchText.Contains(filter)).OrderBy(x => x.AbilityId).ToList();
            _abilityGrid.DataSource = filtered;
            _abilityCatalogStatusLabel.Text = filtered.Count.ToString(CultureInfo.InvariantCulture) + " abilities shown out of " + _abilityCatalog.Count.ToString(CultureInfo.InvariantCulture) + ".";
            SelectFirstRow(_abilityGrid);
        }

        private AbilityCatalogEntry SelectedAbility()
        {
            return _abilityGrid.CurrentRow == null ? null : _abilityGrid.CurrentRow.DataBoundItem as AbilityCatalogEntry;
        }

        private void RefreshConflictRows()
        {
            if (_lastConflictReport == null)
            {
                _conflictGrid.DataSource = null;
                return;
            }

            ConflictDomainRow selected = _conflictDomainGrid.CurrentRow == null ? null : _conflictDomainGrid.CurrentRow.DataBoundItem as ConflictDomainRow;
            IEnumerable<ConflictRecord> conflicts = _lastConflictReport.Conflicts;
            if (selected != null && !string.IsNullOrWhiteSpace(selected.Domain))
                conflicts = conflicts.Where(x => string.Equals(x.Domain, selected.Domain, StringComparison.OrdinalIgnoreCase));

            _conflictGrid.DataSource = conflicts.OrderBy(x => x.SubjectKey).ThenBy(x => x.FactName).Take(500).Select(x => new ConflictListRow
            {
                SubjectKey = x.SubjectKey,
                FactName = x.FactName,
                Domain = x.Domain,
                DistinctValues = string.Join(" | ", x.DistinctValues ?? new List<string>()),
                ClaimCount = x.Claims == null ? 0 : x.Claims.Count
            }).ToList();
        }

        private void RefreshDomainValues()
        {
            if (_lastDomainLedger == null)
            {
                _domainValuesGrid.DataSource = null;
                return;
            }

            DomainListRow selected = _domainsGrid.CurrentRow == null ? null : _domainsGrid.CurrentRow.DataBoundItem as DomainListRow;
            IdentityDomainRecord domain = selected == null
                ? _lastDomainLedger.Domains.FirstOrDefault()
                : _lastDomainLedger.Domains.FirstOrDefault(row => string.Equals(row.DomainKey, selected.DomainKey, StringComparison.OrdinalIgnoreCase));
            if (domain == null)
            {
                _domainValuesGrid.DataSource = null;
                return;
            }

            _domainValuesGrid.DataSource = (domain.Values ?? new List<IdentityDomainValueRecord>()).Select(row => new DomainValueListRow
            {
                RawValue = row.RawValue,
                Meaning = row.Meaning,
                Confidence = row.Confidence,
                Source = row.Source,
                SourcePath = row.SourcePath,
                SourceLocation = row.SourceLocation,
                Notes = row.Notes
            }).ToList();
        }

        private void RefreshRequirementDetails()
        {
            if (_lastRequirementLedger == null)
            {
                _requirementRowGrid.DataSource = null;
                _requirementFieldGrid.DataSource = null;
                _requirementReferenceGrid.DataSource = null;
                return;
            }

            RequirementListRow selected = _requirementsGrid.CurrentRow == null ? null : _requirementsGrid.CurrentRow.DataBoundItem as RequirementListRow;
            RequirementLedgerRecord requirement = selected == null
                ? _lastRequirementLedger.Requirements.FirstOrDefault()
                : _lastRequirementLedger.Requirements.FirstOrDefault(row => row.RequirementId == selected.RequirementId);
            if (requirement == null)
            {
                _requirementRowGrid.DataSource = null;
                _requirementFieldGrid.DataSource = null;
                _requirementReferenceGrid.DataSource = null;
                return;
            }

            _requirementRowGrid.DataSource = (requirement.Rows ?? new List<RequirementRowRecord>()).Select(row => new RequirementRowListRow
            {
                RecordIndex = row.RecordIndex,
                ExtDataText = row.ExtDataText,
                SourcePath = row.SourcePath,
                SourceLocation = row.SourceLocation
            }).ToList();
            _requirementFieldGrid.DataSource = (requirement.Fields ?? new List<RequirementFieldRecord>()).Select(row => new RequirementFieldListRow
            {
                FieldKey = row.FieldKey,
                NonZeroCount = row.NonZeroCount,
                DistinctValueCount = row.DistinctValueCount,
                SampleValuesText = row.SampleValuesText,
                SemanticSummary = row.SemanticSummary,
                Confidence = row.Confidence,
                Notes = row.Notes
            }).ToList();

            List<RequirementReferenceListRow> referenceRows = new List<RequirementReferenceListRow>();
            foreach (RequirementLinkEvidenceRecord reference in requirement.InboundReferences ?? new List<RequirementLinkEvidenceRecord>())
            {
                referenceRows.Add(new RequirementReferenceListRow
                {
                    Direction = "Inbound",
                    SourceKind = reference.SourceKind,
                    SourceId = reference.SourceId,
                    SourceLabel = reference.SourceLabel,
                    SourceField = reference.SourceField,
                    LinkedRequirementId = reference.LinkedRequirementId,
                    RelatedAbilitiesText = reference.RelatedAbilitiesText,
                    ContextTagsText = reference.ContextTagsText,
                    SourcePath = reference.SourcePath,
                    SourceLocation = reference.SourceLocation,
                    Notes = reference.Notes
                });
            }

            foreach (RequirementLinkEvidenceRecord reference in requirement.OutboundReferences ?? new List<RequirementLinkEvidenceRecord>())
            {
                referenceRows.Add(new RequirementReferenceListRow
                {
                    Direction = "Outbound",
                    SourceKind = reference.SourceKind,
                    SourceId = reference.SourceId,
                    SourceLabel = reference.SourceLabel,
                    SourceField = reference.SourceField,
                    LinkedRequirementId = reference.LinkedRequirementId,
                    RelatedAbilitiesText = reference.RelatedAbilitiesText,
                    ContextTagsText = reference.ContextTagsText,
                    SourcePath = reference.SourcePath,
                    SourceLocation = reference.SourceLocation,
                    Notes = reference.Notes
                });
            }

            _requirementReferenceGrid.DataSource = referenceRows
                .OrderBy(row => row.Direction)
                .ThenBy(row => row.SourceKind)
                .ThenBy(row => row.SourceId)
                .ThenBy(row => row.LinkedRequirementId)
                .ToList();
        }

        private void RefreshOperationDetails()
        {
            if (_lastOperationSchema == null)
            {
                _operationFieldGrid.DataSource = null;
                _operationAbilityGrid.DataSource = null;
                return;
            }

            OperationSchemaListRow selected = _operationGrid.CurrentRow == null ? null : _operationGrid.CurrentRow.DataBoundItem as OperationSchemaListRow;
            ComponentOperationSchemaRecord operation = selected == null
                ? _lastOperationSchema.Operations.FirstOrDefault()
                : _lastOperationSchema.Operations.FirstOrDefault(row => row.OperationId == selected.OperationId);
            if (operation == null)
            {
                _operationFieldGrid.DataSource = null;
                _operationAbilityGrid.DataSource = null;
                return;
            }

            _operationFieldGrid.DataSource = (operation.Fields ?? new List<ComponentOperationFieldRecord>()).Select(row => new OperationFieldListRow
            {
                FieldKey = row.FieldKey,
                NonZeroCount = row.NonZeroCount,
                DistinctValueCount = row.DistinctValueCount,
                SampleValuesText = row.SampleValuesText,
                TokenRenderingsText = row.TokenRenderingsText,
                SemanticSummary = row.SemanticSummary,
                Confidence = row.Confidence,
                ContextTagsText = row.ContextTagsText,
                Notes = row.Notes
            }).ToList();
            _operationAbilityGrid.DataSource = (operation.SampleAbilities ?? new List<ComponentOperationAbilityRecord>()).Select(row => new OperationAbilityListRow
            {
                AbilityId = row.AbilityId,
                AbilityName = row.AbilityName,
                ComponentId = row.ComponentId,
                ComponentSlotIndex = row.ComponentSlotIndex,
                TriggerText = row.TriggerText,
                ContextTagsText = row.ContextTagsText,
                TextExcerpt = row.TextExcerpt,
                SourcePath = row.SourcePath,
                SourceLocation = row.SourceLocation
            }).ToList();
        }

        private string BuildDatasetSummary()
        {
            if (_session == null)
                return "Dataset is not loaded.";

            int loaded = _session.Dataset.TableStatuses.Count(x => x.Loaded);
            int failed = _session.Dataset.TableStatuses.Count(x => !x.Loaded);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Extracted root: " + _session.ExtractedRootPath);
            builder.AppendLine("Loaded sources: " + loaded.ToString(CultureInfo.InvariantCulture) + " / " + _session.Dataset.TableStatuses.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Failed sources: " + failed.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Client abilities: " + _session.Dataset.ClientAbilities.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("BIN abilities: " + _session.Dataset.BinaryAbilities.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("BIN components: " + _session.Dataset.BinaryComponents.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("BIN requirements: " + _session.Dataset.BinaryRequirements.Count.ToString(CultureInfo.InvariantCulture));
            return builder.ToString().TrimEnd();
        }

        private string BuildAbilitySummary(AbilityAnalysisResult report, string markdownPath)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(GetAbilityName(report) + " [" + report.AbilityId.ToString(CultureInfo.InvariantCulture) + "]");
            builder.AppendLine("Markdown: " + markdownPath);
            builder.AppendLine("Client rows: " + report.ClientAbilityRows.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("BIN rows: " + report.BinaryAbilityRows.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Effect rows: " + report.ClientEffectRows.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Component rows: " + report.BinaryComponentRows.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Requirement links: " + (report.RequirementReferences == null ? 0 : report.RequirementReferences.Count).ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Requirement rows: " + report.BinaryRequirementRows.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Conflicts: " + report.Graph.GetConflicts().Count.ToString(CultureInfo.InvariantCulture));
            if (report.Warnings.Count > 0)
            {
                builder.AppendLine("Warnings:");
                foreach (string warning in report.Warnings)
                    builder.AppendLine("- " + warning);
            }

            return builder.ToString().TrimEnd();
        }

        private string BuildConflictSummary(ConflictReportDocument report, string markdownPath)
        {
            int domainCount = report.Conflicts.Select(x => x.Domain ?? string.Empty).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            return "Markdown: " + markdownPath + Environment.NewLine
                + "Claims: " + report.Claims.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Conflicts: " + report.Conflicts.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Domains: " + domainCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "The detail grid is filtered by the selected domain and capped at 500 rows.";
        }

        private string BuildTokenDictionarySummary(TokenDictionaryDocument report, string markdownPath)
        {
            int count = report == null || report.Definitions == null ? 0 : report.Definitions.Count;
            return "Markdown: " + markdownPath + Environment.NewLine
                + "Token grammar: " + (report == null ? string.Empty : report.TokenGrammar) + Environment.NewLine
                + "Definitions: " + count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "The dictionary explains COM tokens in plain English using extracted client text evidence first, with Londo reserved for last-resort overrides.";
        }

        private string BuildDomainSummary(DomainLedgerDocument report, string markdownPath)
        {
            List<IdentityDomainRecord> domains = report == null || report.Domains == null ? new List<IdentityDomainRecord>() : report.Domains;
            return "Markdown: " + markdownPath + Environment.NewLine
                + "Domains: " + domains.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Confirmed: " + domains.Count(row => string.Equals(row.Confidence, SemanticConfidence.Confirmed, StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Inferred: " + domains.Count(row => string.Equals(row.Confidence, SemanticConfidence.Inferred, StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Londo: " + domains.Count(row => string.Equals(row.Confidence, SemanticConfidence.Londo, StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "This ledger keeps `Race.EntryId`, `CareerLine.EntryId`, and repeated `CareerName.EntryId` rows separate so numeric domains do not get renamed into facts the extracted client files never proved.";
        }

        private string BuildRequirementSummary(RequirementLedgerDocument report, string markdownPath)
        {
            List<RequirementLedgerRecord> rows = report == null || report.Requirements == null ? new List<RequirementLedgerRecord>() : report.Requirements;
            return "Markdown: " + markdownPath + Environment.NewLine
                + "Requirements: " + rows.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Direct ability usage: " + rows.Count(row => row.DirectAbilityCount > 0).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Component-only usage: " + rows.Count(row => row.DirectAbilityCount <= 0 && row.DirectComponentCount > 0).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Nested child links: " + rows.Count(row => row.ChildRequirementCount > 0).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "This ledger keeps requirement decoding evidence-based by showing exactly which abilities, components, and other requirement rows point at each RequirementId.";
        }

        private string BuildCoverageSummary(CoverageReportDocument report, string markdownPath)
        {
            List<CoverageAbilityRecord> rows = report == null || report.Abilities == null ? new List<CoverageAbilityRecord>() : report.Abilities;
            return "Markdown: " + markdownPath + Environment.NewLine
                + "Abilities: " + rows.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "MappedWithRequirements: " + rows.Count(row => string.Equals(row.CoverageStatus, "MappedWithRequirements", StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Mapped: " + rows.Count(row => string.Equals(row.CoverageStatus, "Mapped", StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "PlayableSurface: " + rows.Count(row => string.Equals(row.CoverageStatus, "PlayableSurface", StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "PartialOrWorse: " + rows.Count(row => !string.Equals(row.CoverageStatus, "MappedWithRequirements", StringComparison.OrdinalIgnoreCase) && !string.Equals(row.CoverageStatus, "Mapped", StringComparison.OrdinalIgnoreCase) && !string.Equals(row.CoverageStatus, "PlayableSurface", StringComparison.OrdinalIgnoreCase)).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "This ledger shows which extracted-client abilities have the minimum pieces needed for a strong analysis and which ones are still missing strings, effects, components, or requirement evidence.";
        }

        private string BuildOperationSummary(OperationSchemaDocument report, string markdownPath)
        {
            int operationCount = report == null || report.Operations == null ? 0 : report.Operations.Count;
            int fieldCount = report == null || report.Operations == null ? 0 : report.Operations.Sum(row => row.Fields == null ? 0 : row.Fields.Count);
            int priorityCount = report == null || report.Operations == null ? 0 : report.Operations.Count(row => row.IsPriority);
            return "Markdown: " + markdownPath + Environment.NewLine
                + "Operations: " + operationCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Field schemas: " + fieldCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "Priority operations: " + priorityCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "This ledger groups component rows by operation, shows recurring non-zero fields, and explains them from extracted client evidence before any Londo override.";
        }

        private List<ConflictDomainRow> BuildDomainRows(ConflictReportDocument report)
        {
            return report.Conflicts.GroupBy(x => x.Domain ?? string.Empty, StringComparer.OrdinalIgnoreCase).Select(group => new ConflictDomainRow
            {
                Domain = group.Key,
                ConflictCount = group.Count(),
                SubjectCount = group.Select(x => x.SubjectKey).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                FactCount = group.Select(x => x.FactName).Distinct(StringComparer.OrdinalIgnoreCase).Count()
            }).OrderByDescending(x => x.ConflictCount).ThenBy(x => x.Domain).ToList();
        }

        private string ResolveOutputRoot()
        {
            return _session == null ? Path.GetFullPath(_outputPathTextBox.Text.Trim()) : _session.ResolveOutputRoot(_outputPathTextBox.Text.Trim());
        }

        private void SetBusy(bool isBusy, string statusText)
        {
            _isBusy = isBusy;
            UseWaitCursor = isBusy;
            Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
            _reloadButton.Enabled = !isBusy;
            _generateAllButton.Enabled = !isBusy && _session != null;
            _generateAbilityButton.Enabled = !isBusy && _session != null;
            _generateTokenDictionaryButton.Enabled = !isBusy && _session != null;
            _generateDomainsButton.Enabled = !isBusy && _session != null;
            _generateRequirementsButton.Enabled = !isBusy && _session != null;
            _generateCoverageButton.Enabled = !isBusy && _session != null;
            _generateOperationSchemasButton.Enabled = !isBusy && _session != null;
            _generateConflictsButton.Enabled = !isBusy && _session != null;
            _statusLabel.Text = statusText;
        }

        private void ChooseFolder(TextBox target, string description)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                dialog.SelectedPath = Directory.Exists(target.Text) ? target.Text : Environment.CurrentDirectory;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    target.Text = dialog.SelectedPath;
            }
        }

        private void OpenPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show(this, "No output path is available yet.", "ClientDataMatrix", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                MessageBox.Show(this, "The target path does not exist:" + Environment.NewLine + path, "ClientDataMatrix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(path);
        }

        private void AppendLog(string message)
        {
            _logTextBox.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "] " + message + Environment.NewLine);
        }

        private Control CreateDefinitionPanel()
        {
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _definitionHintLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(8),
                Text = "Generate an ability report, then double-click a definition row to inspect every known raw value, meaning, and source provenance for that field."
            };

            _definitionGrid = CreateReadOnlyGrid();
            _definitionGrid.AutoGenerateColumns = false;
            _definitionGrid.Columns.Add(CreateTextColumn("Field", "FieldPath", 220));
            _definitionGrid.Columns.Add(CreateTextColumn("Raw", "RawValue", 80));
            _definitionGrid.Columns.Add(CreateTextColumn("Meaning", "Meaning", 320));
            _definitionGrid.Columns.Add(CreateTextColumn("Confidence", "Confidence", 90));
            _definitionGrid.Columns.Add(CreateTextColumn("Source", "DefinitionSource", 180));
            _definitionGrid.Columns.Add(CreateTextColumn("Value From", "ValueSource", 140));
            _definitionGrid.Columns.Add(CreateTextColumn("Location", "ValueSourceLocation", 90));
            _definitionGrid.Columns.Add(CreateTextColumn("Notes", "Notes", 220));
            _definitionGrid.CellDoubleClick += (sender, args) => { if (args.RowIndex >= 0) ShowDefinitionExplorer(); };

            layout.Controls.Add(_definitionHintLabel, 0, 0);
            layout.Controls.Add(_definitionGrid, 0, 1);
            return layout;
        }

        private void ShowDefinitionExplorer()
        {
            if (_definitions == null)
                return;

            DefinitionEntry entry = _definitionGrid.CurrentRow == null ? null : _definitionGrid.CurrentRow.DataBoundItem as DefinitionEntry;
            if (entry == null)
                return;

            DefinitionExplorerModel model = _definitions.BuildExplorer(entry);
            using (DefinitionExplorerForm dialog = new DefinitionExplorerForm(model))
                dialog.ShowDialog(this);

            AppendLog("Opened definition explorer for " + entry.FieldPath + ".");
        }

        private void UpdateDefinitionHint()
        {
            List<DefinitionEntry> rows = _definitionGrid == null ? null : _definitionGrid.DataSource as List<DefinitionEntry>;
            int count = rows == null ? 0 : rows.Count;
            if (_definitionHintLabel != null)
                _definitionHintLabel.Text = count <= 0
                    ? "Generate an ability report, then double-click a definition row to inspect every known raw value, meaning, and source provenance for that field."
                    : count.ToString(CultureInfo.InvariantCulture) + " decoded rows loaded. Double-click a row to open its full client-domain ledger.";
        }

        private static TextBox CreateReadOnlyTextBox() { return new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Both, ReadOnly = true, WordWrap = true }; }
        private static DataGridView CreateReadOnlyGrid() { return new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false, ReadOnly = true, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells, ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false }; }
        private static DataGridViewTextBoxColumn CreateTextColumn(string headerText, string propertyName, int width) { return new DataGridViewTextBoxColumn { HeaderText = headerText, DataPropertyName = propertyName, Width = width, MinimumWidth = Math.Min(width, 60), SortMode = DataGridViewColumnSortMode.Automatic }; }
        private static GroupBox WrapInGroup(string title, Control content) { GroupBox group = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(10) }; group.Controls.Add(content); return group; }
        private static void SelectFirstRow(DataGridView grid) { if (grid != null && grid.Rows.Count > 0 && grid.CurrentCell == null) grid.CurrentCell = grid.Rows[0].Cells[0]; }
        private static string GetAbilityName(AbilityAnalysisResult report) { ClientAbilityRecord client = report.ClientAbilityRows.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Name)); if (client != null) return client.Name; IndexedStringRecord name = report.AbilityNameRows.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.NormalizedValue)); return name == null ? "Ability" : name.NormalizedValue; }
        private static string EffectName(AbilityAnalysisResult report, int effectId) { ClientEffectRecord row = report.ClientEffectRows.FirstOrDefault(x => x.EffectId == effectId); return row == null ? "Unknown effect" : NullToPlaceholder(row.Name); }
        private static string NullToPlaceholder(string value) { return string.IsNullOrWhiteSpace(value) ? "(none)" : value; }
        private static string TextOrNone(int value) { return value > 0 ? value.ToString(CultureInfo.InvariantCulture) : "(none)"; }

        private sealed class ConflictDomainRow { public string Domain { get; set; } public int ConflictCount { get; set; } public int SubjectCount { get; set; } public int FactCount { get; set; } }
        private sealed class ConflictListRow { public string SubjectKey { get; set; } public string FactName { get; set; } public string Domain { get; set; } public string DistinctValues { get; set; } public int ClaimCount { get; set; } }
        private sealed class TokenDefinitionListRow { public string ExampleToken { get; set; } public string FieldKey { get; set; } public string PlainEnglishMeaning { get; set; } public string Confidence { get; set; } public string ContextTagsText { get; set; } public string ExampleAbilityIdsText { get; set; } public string Notes { get; set; } }
        private sealed class DomainListRow { public string DomainKey { get; set; } public string DisplayName { get; set; } public string Confidence { get; set; } public string Canonicality { get; set; } public int ValueCount { get; set; } public int DistinctMeaningCount { get; set; } public int DuplicateMeaningCount { get; set; } public string SourceFilesText { get; set; } public string RecommendedUsage { get; set; } public string Notes { get; set; } }
        private sealed class DomainValueListRow { public string RawValue { get; set; } public string Meaning { get; set; } public string Confidence { get; set; } public string Source { get; set; } public string SourcePath { get; set; } public string SourceLocation { get; set; } public string Notes { get; set; } }
        private sealed class RequirementListRow { public ushort RequirementId { get; set; } public int RecordCount { get; set; } public int FieldCount { get; set; } public int DirectAbilityCount { get; set; } public int DirectComponentCount { get; set; } public int ParentRequirementCount { get; set; } public int ChildRequirementCount { get; set; } public string ContextTagsText { get; set; } public string SampleAbilitiesText { get; set; } public string SemanticSummary { get; set; } public string Notes { get; set; } }
        private sealed class RequirementRowListRow { public int RecordIndex { get; set; } public string ExtDataText { get; set; } public string SourcePath { get; set; } public string SourceLocation { get; set; } }
        private sealed class RequirementFieldListRow { public string FieldKey { get; set; } public int NonZeroCount { get; set; } public int DistinctValueCount { get; set; } public string SampleValuesText { get; set; } public string SemanticSummary { get; set; } public string Confidence { get; set; } public string Notes { get; set; } }
        private sealed class RequirementReferenceListRow { public string Direction { get; set; } public string SourceKind { get; set; } public ushort SourceId { get; set; } public string SourceLabel { get; set; } public string SourceField { get; set; } public ushort LinkedRequirementId { get; set; } public string RelatedAbilitiesText { get; set; } public string ContextTagsText { get; set; } public string SourcePath { get; set; } public string SourceLocation { get; set; } public string Notes { get; set; } }
        private sealed class CoverageListRow { public ushort AbilityId { get; set; } public string Name { get; set; } public string CoverageStatus { get; set; } public string EffectIdText { get; set; } public string HasClientCsvText { get; set; } public string HasClientBinText { get; set; } public string HasLocalizedNameText { get; set; } public string HasDescriptionTextText { get; set; } public string HasEffectTextText { get; set; } public string HasRootEffectRowText { get; set; } public int ComponentCount { get; set; } public int RequirementLinkCount { get; set; } public int RequirementRowCount { get; set; } public string SourcesText { get; set; } public string MissingText { get; set; } }
        private sealed class OperationSchemaListRow { public uint OperationId { get; set; } public string OperationName { get; set; } public string PriorityText { get; set; } public int ComponentCount { get; set; } public int AbilityCount { get; set; } public string ContextTagsText { get; set; } public string LayoutVariantsText { get; set; } }
        private sealed class OperationFieldListRow { public string FieldKey { get; set; } public int NonZeroCount { get; set; } public int DistinctValueCount { get; set; } public string SampleValuesText { get; set; } public string TokenRenderingsText { get; set; } public string SemanticSummary { get; set; } public string Confidence { get; set; } public string ContextTagsText { get; set; } public string Notes { get; set; } }
        private sealed class OperationAbilityListRow { public ushort AbilityId { get; set; } public string AbilityName { get; set; } public ushort ComponentId { get; set; } public int ComponentSlotIndex { get; set; } public string TriggerText { get; set; } public string ContextTagsText { get; set; } public string TextExcerpt { get; set; } public string SourcePath { get; set; } public string SourceLocation { get; set; } }
    }
}
