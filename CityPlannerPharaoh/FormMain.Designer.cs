namespace CityPlannerPharaoh
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Tool tool1 = new Tool();
            toolStripMain = new ToolStrip();
            btnSelect = new ToolStripButton();
            btnTerrain = new ToolStripButton();
            btnClear = new ToolStripButton();
            btnHouse = new ToolStripButton();
            btnHouse2 = new ToolStripButton();
            btnHouse3 = new ToolStripButton();
            btnHouse4 = new ToolStripButton();
            btnRoad = new ToolStripButton();
            btnPlaza = new ToolStripButton();
            btnFood = new ToolStripButton();
            btnIndustry = new ToolStripButton();
            btnDistribution = new ToolStripButton();
            btnEnt = new ToolStripButton();
            btnReligious = new ToolStripButton();
            btnEducation = new ToolStripButton();
            btnHealth = new ToolStripButton();
            btnMunicipal = new ToolStripButton();
            btnMilitary = new ToolStripButton();
            canvas = new SanelyScolledPanel();
            mapControl = new MapCanvasControl();
            toolStripTop = new ToolStrip();
            btnFileNew = new ToolStripButton();
            btnNewFromGameSave = new ToolStripButton();
            btnFileOpen = new ToolStripButton();
            btnFileSave = new ToolStripButton();
            btnFileSaveAs = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnFilterBuildings = new ToolStripButton();
            btnFilterDesire = new ToolStripButton();
            btnFilterDesireFull = new ToolStripButton();
            btnFilterSimpleHouseDesire = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            btnCutBuildings = new ToolStripButton();
            btnCopyBuildings = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            btnUndo = new ToolStripButton();
            btnRedo = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            ddDifficulty = new ToolStripDropDownButton();
            btnDifficultyVE = new ToolStripMenuItem();
            btnDifficultyE = new ToolStripMenuItem();
            btnDifficultyN = new ToolStripMenuItem();
            btnDifficultyH = new ToolStripMenuItem();
            btnDifficultyVH = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            btnGlyph = new ToolStripButton();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            toolStripSecondary = new ToolStrip();
            openFileDialogImport = new OpenFileDialog();
            statusStripMain = new StatusStrip();
            toolStripLabelCoords = new ToolStripStatusLabel();
            toolStripStatusTotal = new ToolStripStatusLabel();
            toolStripTotalPopLabel = new ToolStripStatusLabel();
            toolStripTotalPop = new ToolStripStatusLabel();
            toolStripTotalWork23Label = new ToolStripStatusLabel();
            toolStripTotalWork23 = new ToolStripStatusLabel();
            toolStripTotalEmplLabel = new ToolStripStatusLabel();
            toolStripTotalEmpl = new ToolStripStatusLabel();
            toolStripStatusSelection = new ToolStripStatusLabel();
            toolStripSelectedEmplLabel = new ToolStripStatusLabel();
            toolStripSelectedEmpl = new ToolStripStatusLabel();
            toolStripStatusLabelRoadLabel = new ToolStripStatusLabel();
            toolStripLabelRoadLength = new ToolStripStatusLabel();
            toolStripStatusLabelHousesLabel = new ToolStripStatusLabel();
            toolStrip2x2HouseCount = new ToolStripStatusLabel();
            btnPasteBuildings = new ToolStripButton();
            toolStripMain.SuspendLayout();
            canvas.SuspendLayout();
            toolStripTop.SuspendLayout();
            statusStripMain.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripMain
            // 
            toolStripMain.AutoSize = false;
            toolStripMain.Dock = DockStyle.Right;
            toolStripMain.Items.AddRange(new ToolStripItem[] { btnSelect, btnTerrain, btnClear, btnHouse, btnHouse2, btnHouse3, btnHouse4, btnRoad, btnPlaza, btnFood, btnIndustry, btnDistribution, btnEnt, btnReligious, btnEducation, btnHealth, btnMunicipal, btnMilitary });
            toolStripMain.Location = new Point(1285, 0);
            toolStripMain.Name = "toolStripMain";
            toolStripMain.Size = new Size(39, 837);
            toolStripMain.TabIndex = 0;
            toolStripMain.Text = "toolStrip1";
            // 
            // btnSelect
            // 
            btnSelect.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(37, 19);
            btnSelect.Text = "SEL";
            btnSelect.ToolTipText = "Select";
            btnSelect.Click += MainToolbarToolButtonClick;
            // 
            // btnTerrain
            // 
            btnTerrain.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrain.Name = "btnTerrain";
            btnTerrain.Size = new Size(37, 19);
            btnTerrain.Text = "TER";
            btnTerrain.ToolTipText = "Terrain";
            // 
            // btnClear
            // 
            btnClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnClear.ImageTransparentColor = Color.Magenta;
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(37, 19);
            btnClear.Text = "DEL";
            btnClear.ToolTipText = "Clear";
            btnClear.Click += MainToolbarToolButtonClick;
            // 
            // btnHouse
            // 
            btnHouse.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHouse.Name = "btnHouse";
            btnHouse.Size = new Size(37, 19);
            btnHouse.Text = "H";
            btnHouse.ToolTipText = "Housing 1x1";
            // 
            // btnHouse2
            // 
            btnHouse2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHouse2.Name = "btnHouse2";
            btnHouse2.Size = new Size(37, 19);
            btnHouse2.Text = "H2";
            btnHouse2.ToolTipText = "Housing 2x2";
            // 
            // btnHouse3
            // 
            btnHouse3.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHouse3.Name = "btnHouse3";
            btnHouse3.Size = new Size(37, 19);
            btnHouse3.Text = "H3";
            btnHouse3.ToolTipText = "Housing 3x3";
            // 
            // btnHouse4
            // 
            btnHouse4.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHouse4.Name = "btnHouse4";
            btnHouse4.Size = new Size(37, 19);
            btnHouse4.Text = "H4";
            btnHouse4.ToolTipText = "Housing 4x4";
            // 
            // btnRoad
            // 
            btnRoad.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnRoad.Name = "btnRoad";
            btnRoad.Size = new Size(37, 19);
            btnRoad.Text = "Road";
            // 
            // btnPlaza
            // 
            btnPlaza.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnPlaza.Name = "btnPlaza";
            btnPlaza.Size = new Size(37, 19);
            btnPlaza.Text = "Plaza";
            // 
            // btnFood
            // 
            btnFood.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFood.Name = "btnFood";
            btnFood.Size = new Size(37, 19);
            btnFood.Text = "Food";
            // 
            // btnIndustry
            // 
            btnIndustry.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnIndustry.Name = "btnIndustry";
            btnIndustry.Size = new Size(37, 19);
            btnIndustry.Text = "Ind";
            btnIndustry.ToolTipText = "Industry";
            // 
            // btnDistribution
            // 
            btnDistribution.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDistribution.Name = "btnDistribution";
            btnDistribution.Size = new Size(37, 19);
            btnDistribution.Text = "Dist";
            btnDistribution.ToolTipText = "Distribution";
            // 
            // btnEnt
            // 
            btnEnt.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEnt.Name = "btnEnt";
            btnEnt.Size = new Size(37, 19);
            btnEnt.Text = "Ent";
            btnEnt.ToolTipText = "Entertainment";
            // 
            // btnReligious
            // 
            btnReligious.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnReligious.Name = "btnReligious";
            btnReligious.Size = new Size(37, 19);
            btnReligious.Text = "Rel";
            btnReligious.ToolTipText = "Religious";
            // 
            // btnEducation
            // 
            btnEducation.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEducation.Name = "btnEducation";
            btnEducation.Size = new Size(37, 19);
            btnEducation.Text = "Edu";
            btnEducation.ToolTipText = "Education";
            // 
            // btnHealth
            // 
            btnHealth.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHealth.Name = "btnHealth";
            btnHealth.Size = new Size(37, 19);
            btnHealth.Text = "Hlth";
            btnHealth.ToolTipText = "Health";
            // 
            // btnMunicipal
            // 
            btnMunicipal.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnMunicipal.Name = "btnMunicipal";
            btnMunicipal.Size = new Size(37, 19);
            btnMunicipal.Text = "Mun";
            btnMunicipal.ToolTipText = "Municipal";
            // 
            // btnMilitary
            // 
            btnMilitary.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnMilitary.Name = "btnMilitary";
            btnMilitary.Size = new Size(37, 19);
            btnMilitary.Text = "Mil";
            btnMilitary.ToolTipText = "Military";
            // 
            // canvas
            // 
            canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            canvas.AutoScroll = true;
            canvas.Controls.Add(mapControl);
            canvas.Location = new Point(0, 28);
            canvas.Name = "canvas";
            canvas.Size = new Size(1229, 782);
            canvas.TabIndex = 1;
            canvas.Zoom += canvas_Zoom;
            // 
            // mapControl
            // 
            mapControl.CellSideLength = 24;
            mapControl.Location = new Point(0, 0);
            mapControl.Name = "mapControl";
            mapControl.ShowBuildings = false;
            mapControl.ShowCellCoords = false;
            mapControl.ShowDesirability = false;
            mapControl.ShowDesirabilityFull = false;
            mapControl.Size = new Size(200, 200);
            mapControl.TabIndex = 0;
            mapControl.Text = "mapCanvasControl1";
            tool1.BuildingType = null;
            tool1.HouseLevel = 0;
            tool1.IsClearBuilding = false;
            tool1.Terrain = null;
            mapControl.Tool = tool1;
            mapControl.GlobalStatsChanged += mapControl_GlobalStatsChanged;
            mapControl.SelectionChanged += mapControl_SelectionChanged;
            mapControl.UndoStackChanged += mapControl_UndoStackChanged;
            mapControl.MouseCoordsChanged += mapControl_MouseCoordsChanged;
            // 
            // toolStripTop
            // 
            toolStripTop.AutoSize = false;
            toolStripTop.Items.AddRange(new ToolStripItem[] { btnFileNew, btnNewFromGameSave, btnFileOpen, btnFileSave, btnFileSaveAs, toolStripSeparator1, btnFilterBuildings, btnFilterDesire, btnFilterDesireFull, btnFilterSimpleHouseDesire, toolStripSeparator6, btnCutBuildings, btnCopyBuildings, btnPasteBuildings, toolStripSeparator3, btnUndo, btnRedo, toolStripSeparator4, ddDifficulty, toolStripSeparator5, btnGlyph });
            toolStripTop.Location = new Point(0, 0);
            toolStripTop.Name = "toolStripTop";
            toolStripTop.Size = new Size(1232, 25);
            toolStripTop.TabIndex = 4;
            toolStripTop.Text = "toolStrip1";
            // 
            // btnFileNew
            // 
            btnFileNew.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileNew.Name = "btnFileNew";
            btnFileNew.Size = new Size(35, 22);
            btnFileNew.Text = "New";
            btnFileNew.Click += btnFileNew_Click;
            // 
            // btnNewFromGameSave
            // 
            btnNewFromGameSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnNewFromGameSave.Name = "btnNewFromGameSave";
            btnNewFromGameSave.Size = new Size(98, 22);
            btnNewFromGameSave.Text = "From game save";
            btnNewFromGameSave.ToolTipText = "New from game save...";
            btnNewFromGameSave.Click += btnNewFromGameSave_Click;
            // 
            // btnFileOpen
            // 
            btnFileOpen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileOpen.Name = "btnFileOpen";
            btnFileOpen.Size = new Size(40, 22);
            btnFileOpen.Text = "Open";
            btnFileOpen.Click += btnFileOpen_Click;
            // 
            // btnFileSave
            // 
            btnFileSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileSave.Name = "btnFileSave";
            btnFileSave.Size = new Size(35, 22);
            btnFileSave.Text = "Save";
            btnFileSave.Click += btnFileSave_Click;
            // 
            // btnFileSaveAs
            // 
            btnFileSaveAs.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileSaveAs.Name = "btnFileSaveAs";
            btnFileSaveAs.Size = new Size(49, 22);
            btnFileSaveAs.Text = "Save as";
            btnFileSaveAs.Click += btnFileSaveAs_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btnFilterBuildings
            // 
            btnFilterBuildings.Checked = true;
            btnFilterBuildings.CheckOnClick = true;
            btnFilterBuildings.CheckState = CheckState.Checked;
            btnFilterBuildings.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilterBuildings.Name = "btnFilterBuildings";
            btnFilterBuildings.Size = new Size(33, 22);
            btnFilterBuildings.Text = "Blds";
            btnFilterBuildings.ToolTipText = "Show Buildings";
            btnFilterBuildings.Click += btnFilterBuildings_Click;
            // 
            // btnFilterDesire
            // 
            btnFilterDesire.Checked = true;
            btnFilterDesire.CheckOnClick = true;
            btnFilterDesire.CheckState = CheckState.Checked;
            btnFilterDesire.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilterDesire.Name = "btnFilterDesire";
            btnFilterDesire.Size = new Size(30, 22);
            btnFilterDesire.Text = "Des";
            btnFilterDesire.ToolTipText = "Show Desirablility1";
            btnFilterDesire.Click += btnFilterDesire_Click;
            // 
            // btnFilterDesireFull
            // 
            btnFilterDesireFull.CheckOnClick = true;
            btnFilterDesireFull.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilterDesireFull.Name = "btnFilterDesireFull";
            btnFilterDesireFull.Size = new Size(23, 22);
            btnFilterDesireFull.Text = "F";
            btnFilterDesireFull.ToolTipText = "Show desirability even on buildings not affected by it.";
            btnFilterDesireFull.Click += btnFilterDesireFull_Click;
            // 
            // btnFilterSimpleHouseDesire
            // 
            btnFilterSimpleHouseDesire.CheckOnClick = true;
            btnFilterSimpleHouseDesire.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilterSimpleHouseDesire.Name = "btnFilterSimpleHouseDesire";
            btnFilterSimpleHouseDesire.Size = new Size(23, 22);
            btnFilterSimpleHouseDesire.Text = "S";
            btnFilterSimpleHouseDesire.ToolTipText = "Simple house desirability effect.";
            btnFilterSimpleHouseDesire.Click += btnFilterSimpleHouseDesire_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // btnCutBuildings
            // 
            btnCutBuildings.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnCutBuildings.Name = "btnCutBuildings";
            btnCutBuildings.Size = new Size(30, 22);
            btnCutBuildings.Text = "Cut";
            btnCutBuildings.Click += btnCutBuildings_Click;
            // 
            // btnCopyBuildings
            // 
            btnCopyBuildings.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnCopyBuildings.Name = "btnCopyBuildings";
            btnCopyBuildings.Size = new Size(39, 22);
            btnCopyBuildings.Text = "Copy";
            btnCopyBuildings.Click += btnCopyBuildings_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // btnUndo
            // 
            btnUndo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnUndo.Enabled = false;
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(40, 22);
            btnUndo.Text = "Undo";
            btnUndo.Click += btnUndo_Click;
            // 
            // btnRedo
            // 
            btnRedo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnRedo.Enabled = false;
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(38, 22);
            btnRedo.Text = "Redo";
            btnRedo.Click += btnRedo_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // ddDifficulty
            // 
            ddDifficulty.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ddDifficulty.DropDownItems.AddRange(new ToolStripItem[] { btnDifficultyVE, btnDifficultyE, btnDifficultyN, btnDifficultyH, btnDifficultyVH });
            ddDifficulty.ImageTransparentColor = Color.Magenta;
            ddDifficulty.Name = "ddDifficulty";
            ddDifficulty.Size = new Size(46, 22);
            ddDifficulty.Text = "Hard";
            ddDifficulty.ToolTipText = "Difficulty";
            // 
            // btnDifficultyVE
            // 
            btnDifficultyVE.Name = "btnDifficultyVE";
            btnDifficultyVE.Size = new Size(125, 22);
            btnDifficultyVE.Text = "Very Easy";
            btnDifficultyVE.Click += btnDifficulty_Click;
            // 
            // btnDifficultyE
            // 
            btnDifficultyE.Name = "btnDifficultyE";
            btnDifficultyE.Size = new Size(125, 22);
            btnDifficultyE.Text = "Easy";
            btnDifficultyE.Click += btnDifficulty_Click;
            // 
            // btnDifficultyN
            // 
            btnDifficultyN.Name = "btnDifficultyN";
            btnDifficultyN.Size = new Size(125, 22);
            btnDifficultyN.Text = "Normal";
            btnDifficultyN.Click += btnDifficulty_Click;
            // 
            // btnDifficultyH
            // 
            btnDifficultyH.Name = "btnDifficultyH";
            btnDifficultyH.Size = new Size(125, 22);
            btnDifficultyH.Text = "Hard";
            btnDifficultyH.Click += btnDifficulty_Click;
            // 
            // btnDifficultyVH
            // 
            btnDifficultyVH.Name = "btnDifficultyVH";
            btnDifficultyVH.Size = new Size(125, 22);
            btnDifficultyVH.Text = "Very Hard";
            btnDifficultyVH.Click += btnDifficulty_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // btnGlyph
            // 
            btnGlyph.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnGlyph.ImageTransparentColor = Color.Magenta;
            btnGlyph.Name = "btnGlyph";
            btnGlyph.Size = new Size(42, 22);
            btnGlyph.Text = "Glyph";
            btnGlyph.Click += btnGlyph_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "City Planner Pharaoh files|*.cityph|All files|*.*";
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "City Planner Pharaoh files|*.cityph|All files|*.*";
            // 
            // toolStripSecondary
            // 
            toolStripSecondary.AutoSize = false;
            toolStripSecondary.Dock = DockStyle.Right;
            toolStripSecondary.Location = new Point(1232, 0);
            toolStripSecondary.Name = "toolStripSecondary";
            toolStripSecondary.Size = new Size(53, 837);
            toolStripSecondary.TabIndex = 14;
            toolStripSecondary.Text = "toolStrip5";
            // 
            // openFileDialogImport
            // 
            openFileDialogImport.Filter = "Pharaoh save files|*.sav|All files|*.*";
            // 
            // statusStripMain
            // 
            statusStripMain.Items.AddRange(new ToolStripItem[] { toolStripLabelCoords, toolStripStatusTotal, toolStripTotalPopLabel, toolStripTotalPop, toolStripTotalWork23Label, toolStripTotalWork23, toolStripTotalEmplLabel, toolStripTotalEmpl, toolStripStatusSelection, toolStripSelectedEmplLabel, toolStripSelectedEmpl, toolStripStatusLabelRoadLabel, toolStripLabelRoadLength, toolStripStatusLabelHousesLabel, toolStrip2x2HouseCount });
            statusStripMain.Location = new Point(0, 813);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Size = new Size(1232, 24);
            statusStripMain.TabIndex = 15;
            // 
            // toolStripLabelCoords
            // 
            toolStripLabelCoords.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripLabelCoords.Name = "toolStripLabelCoords";
            toolStripLabelCoords.Size = new Size(70, 19);
            toolStripLabelCoords.Text = "x:999, y:999";
            // 
            // toolStripStatusTotal
            // 
            toolStripStatusTotal.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            toolStripStatusTotal.BorderStyle = Border3DStyle.Etched;
            toolStripStatusTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripStatusTotal.Name = "toolStripStatusTotal";
            toolStripStatusTotal.Size = new Size(40, 19);
            toolStripStatusTotal.Text = "total:";
            // 
            // toolStripTotalPopLabel
            // 
            toolStripTotalPopLabel.Name = "toolStripTotalPopLabel";
            toolStripTotalPopLabel.Size = new Size(31, 19);
            toolStripTotalPopLabel.Text = "pop:";
            // 
            // toolStripTotalPop
            // 
            toolStripTotalPop.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripTotalPop.Name = "toolStripTotalPop";
            toolStripTotalPop.Size = new Size(17, 19);
            toolStripTotalPop.Text = "0";
            // 
            // toolStripTotalWork23Label
            // 
            toolStripTotalWork23Label.Name = "toolStripTotalWork23Label";
            toolStripTotalWork23Label.Size = new Size(66, 19);
            toolStripTotalWork23Label.Text = "work(23%):";
            // 
            // toolStripTotalWork23
            // 
            toolStripTotalWork23.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripTotalWork23.Name = "toolStripTotalWork23";
            toolStripTotalWork23.Size = new Size(17, 19);
            toolStripTotalWork23.Text = "0";
            // 
            // toolStripTotalEmplLabel
            // 
            toolStripTotalEmplLabel.Name = "toolStripTotalEmplLabel";
            toolStripTotalEmplLabel.Size = new Size(37, 19);
            toolStripTotalEmplLabel.Text = "empl:";
            // 
            // toolStripTotalEmpl
            // 
            toolStripTotalEmpl.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripTotalEmpl.Name = "toolStripTotalEmpl";
            toolStripTotalEmpl.Size = new Size(17, 19);
            toolStripTotalEmpl.Text = "0";
            // 
            // toolStripStatusSelection
            // 
            toolStripStatusSelection.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            toolStripStatusSelection.BorderStyle = Border3DStyle.Etched;
            toolStripStatusSelection.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripStatusSelection.Name = "toolStripStatusSelection";
            toolStripStatusSelection.Size = new Size(61, 19);
            toolStripStatusSelection.Text = "selected:";
            // 
            // toolStripSelectedEmplLabel
            // 
            toolStripSelectedEmplLabel.Name = "toolStripSelectedEmplLabel";
            toolStripSelectedEmplLabel.Size = new Size(37, 19);
            toolStripSelectedEmplLabel.Text = "empl:";
            // 
            // toolStripSelectedEmpl
            // 
            toolStripSelectedEmpl.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripSelectedEmpl.Name = "toolStripSelectedEmpl";
            toolStripSelectedEmpl.Size = new Size(17, 19);
            toolStripSelectedEmpl.Text = "0";
            // 
            // toolStripStatusLabelRoadLabel
            // 
            toolStripStatusLabelRoadLabel.Name = "toolStripStatusLabelRoadLabel";
            toolStripStatusLabelRoadLabel.Size = new Size(34, 19);
            toolStripStatusLabelRoadLabel.Text = "road:";
            // 
            // toolStripLabelRoadLength
            // 
            toolStripLabelRoadLength.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripLabelRoadLength.Name = "toolStripLabelRoadLength";
            toolStripLabelRoadLength.Size = new Size(17, 19);
            toolStripLabelRoadLength.Text = "0";
            // 
            // toolStripStatusLabelHousesLabel
            // 
            toolStripStatusLabelHousesLabel.Name = "toolStripStatusLabelHousesLabel";
            toolStripStatusLabelHousesLabel.Size = new Size(67, 19);
            toolStripStatusLabelHousesLabel.Text = "2x2 houses:";
            // 
            // toolStrip2x2HouseCount
            // 
            toolStrip2x2HouseCount.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStrip2x2HouseCount.Name = "toolStrip2x2HouseCount";
            toolStrip2x2HouseCount.Size = new Size(17, 19);
            toolStrip2x2HouseCount.Text = "0";
            // 
            // btnPasteBuildings
            // 
            btnPasteBuildings.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnPasteBuildings.Name = "btnPasteBuildings";
            btnPasteBuildings.Size = new Size(39, 22);
            btnPasteBuildings.Text = "Paste";
            btnPasteBuildings.Click += btnPasteBuildings_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1324, 837);
            Controls.Add(statusStripMain);
            Controls.Add(toolStripTop);
            Controls.Add(toolStripSecondary);
            Controls.Add(canvas);
            Controls.Add(toolStripMain);
            KeyPreview = true;
            Name = "FormMain";
            Text = "City Planner Pharaoh";
            FormClosing += FormMain_FormClosing;
            KeyDown += FormMain_KeyDown;
            toolStripMain.ResumeLayout(false);
            toolStripMain.PerformLayout();
            canvas.ResumeLayout(false);
            toolStripTop.ResumeLayout(false);
            toolStripTop.PerformLayout();
            statusStripMain.ResumeLayout(false);
            statusStripMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStripMain;
        private ToolStripButton btnClear;
        private ToolStripButton btnRoad;
        private SanelyScolledPanel canvas;
        private MapCanvasControl mapControl;
        private ToolStripButton btnSelect;
        private ToolStripButton btnTerrain;
        private ToolStripButton btnHouse;
        private ToolStripButton btnPlaza;
        private ToolStripButton btnDistribution;
        private ToolStrip toolStripTop;
        private ToolStripButton btnFileNew;
        private ToolStripButton btnFileOpen;
        private ToolStripButton btnFileSave;
        private ToolStripButton btnFileSaveAs;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btnFilterBuildings;
        private ToolStripButton btnFilterDesire;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private ToolStripButton btnCopyBuildings;
        private ToolStripButton btnCutBuildings;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton btnFood;
        private ToolStripButton btnIndustry;
        private ToolStripButton btnEnt;
        private ToolStripButton btnReligious;
        private ToolStripButton btnEducation;
        private ToolStripButton btnHealth;
        private ToolStripButton btnMunicipal;
        private ToolStripButton btnMilitary;
        private ToolStripButton btnHouse2;
        private ToolStripButton btnHouse3;
        private ToolStripButton btnHouse4;
        private ToolStrip toolStripSecondary;
        private ToolStripButton btnNewFromGameSave;
        private OpenFileDialog openFileDialogImport;
        private ToolStripDropDownButton ddDifficulty;
        private ToolStripMenuItem btnDifficultyVE;
        private ToolStripMenuItem btnDifficultyE;
        private ToolStripMenuItem btnDifficultyN;
        private ToolStripMenuItem btnDifficultyH;
        private ToolStripMenuItem btnDifficultyVH;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton btnGlyph;
        private ToolStripButton btnUndo;
        private ToolStripButton btnRedo;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton btnFilterDesireFull;
        private ToolStripButton btnFilterSimpleHouseDesire;
        private ToolStripSeparator toolStripSeparator6;
        private StatusStrip statusStripMain;
        private ToolStripStatusLabel toolStripLabelCoords;
        private ToolStripStatusLabel toolStripStatusLabelRoadLabel;
        private ToolStripStatusLabel toolStripLabelRoadLength;
        private ToolStripStatusLabel toolStripStatusLabelHousesLabel;
        private ToolStripStatusLabel toolStrip2x2HouseCount;
        private ToolStripStatusLabel toolStripTotalEmplLabel;
        private ToolStripStatusLabel toolStripTotalEmpl;
        private ToolStripStatusLabel toolStripSelectedEmplLabel;
        private ToolStripStatusLabel toolStripSelectedEmpl;
        private ToolStripStatusLabel toolStripStatusSelection;
        private ToolStripStatusLabel toolStripStatusTotal;
        private ToolStripStatusLabel toolStripTotalPopLabel;
        private ToolStripStatusLabel toolStripTotalPop;
        private ToolStripStatusLabel toolStripTotalWork23Label;
        private ToolStripStatusLabel toolStripTotalWork23;
        private ToolStripButton btnPasteBuildings;
    }
}