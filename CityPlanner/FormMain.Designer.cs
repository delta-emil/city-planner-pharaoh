namespace CityPlanner
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
            canvas = new Panel();
            mapControl = new MapCanvasControl();
            toolStripTop = new ToolStrip();
            btnFileNew = new ToolStripButton();
            btnFileOpen = new ToolStripButton();
            btnFileSave = new ToolStripButton();
            btnFileSaveAs = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnFilterBuildings = new ToolStripButton();
            btnFilterDesire = new ToolStripButton();
            toolStripLabelRoadLengthLabel = new ToolStripLabel();
            toolStripLabelRoadLength = new ToolStripLabel();
            toolStripSeparator20 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            toolStrip2x2HouseCount = new ToolStripLabel();
            toolStripSeparator2 = new ToolStripSeparator();
            btnCutBuildings = new ToolStripButton();
            btnCopyBuildings = new ToolStripButton();
            toolStripLabelPasteBuildings = new ToolStripLabel();
            toolStripSeparator3 = new ToolStripSeparator();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            toolStripSecondary = new ToolStrip();
            btnNewFromGameSave = new ToolStripButton();
            openFileDialogImport = new OpenFileDialog();
            toolStripMain.SuspendLayout();
            canvas.SuspendLayout();
            toolStripTop.SuspendLayout();
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
            canvas.Size = new Size(1243, 804);
            canvas.TabIndex = 1;
            // 
            // mapControl
            // 
            mapControl.Location = new Point(0, 0);
            mapControl.Name = "mapControl";
            mapControl.ShowBuildings = false;
            mapControl.ShowCellCoords = false;
            mapControl.ShowDesirability = false;
            mapControl.Size = new Size(200, 200);
            mapControl.TabIndex = 0;
            mapControl.Text = "mapCanvasControl1";
            tool1.BuildingType = null;
            tool1.IsClearBuilding = false;
            tool1.Terrain = null;
            mapControl.Tool = tool1;
            mapControl.SelectionChanged += mapControl_SelectionChanged;
            // 
            // toolStripTop
            // 
            toolStripTop.AutoSize = false;
            toolStripTop.Items.AddRange(new ToolStripItem[] { btnFileNew, btnNewFromGameSave, btnFileOpen, btnFileSave, btnFileSaveAs, toolStripSeparator1, btnFilterBuildings, btnFilterDesire, toolStripLabelRoadLengthLabel, toolStripLabelRoadLength, toolStripSeparator20, toolStripLabel1, toolStrip2x2HouseCount, toolStripSeparator2, btnCutBuildings, btnCopyBuildings, toolStripLabelPasteBuildings, toolStripSeparator3 });
            toolStripTop.Location = new Point(0, 0);
            toolStripTop.Name = "toolStripTop";
            toolStripTop.Size = new Size(1246, 25);
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
            // btnFileOpen
            // 
            btnFileOpen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileOpen.Name = "btnFileOpen";
            btnFileOpen.Size = new Size(49, 22);
            btnFileOpen.Text = "Open...";
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
            btnFileSaveAs.Size = new Size(60, 22);
            btnFileSaveAs.Text = "Save As...";
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
            btnFilterBuildings.Size = new Size(92, 22);
            btnFilterBuildings.Text = "Show Buildings";
            btnFilterBuildings.Click += btnFilterBuildings_Click;
            // 
            // btnFilterDesire
            // 
            btnFilterDesire.Checked = true;
            btnFilterDesire.CheckOnClick = true;
            btnFilterDesire.CheckState = CheckState.Checked;
            btnFilterDesire.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilterDesire.Name = "btnFilterDesire";
            btnFilterDesire.Size = new Size(104, 22);
            btnFilterDesire.Text = "Show Desirablility";
            btnFilterDesire.Click += btnFilterDesire_Click;
            // 
            // toolStripLabelRoadLengthLabel
            // 
            toolStripLabelRoadLengthLabel.Name = "toolStripLabelRoadLengthLabel";
            toolStripLabelRoadLengthLabel.Size = new Size(37, 22);
            toolStripLabelRoadLengthLabel.Text = "Road:";
            // 
            // toolStripLabelRoadLength
            // 
            toolStripLabelRoadLength.AutoSize = false;
            toolStripLabelRoadLength.Name = "toolStripLabelRoadLength";
            toolStripLabelRoadLength.Size = new Size(25, 22);
            toolStripLabelRoadLength.Text = "0";
            toolStripLabelRoadLength.TextAlign = ContentAlignment.MiddleRight;
            // 
            // toolStripSeparator20
            // 
            toolStripSeparator20.Name = "toolStripSeparator20";
            toolStripSeparator20.Size = new Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(40, 22);
            toolStripLabel1.Text = "2x2 H:";
            // 
            // toolStrip2x2HouseCount
            // 
            toolStrip2x2HouseCount.AutoSize = false;
            toolStrip2x2HouseCount.Name = "toolStrip2x2HouseCount";
            toolStrip2x2HouseCount.Size = new Size(25, 22);
            toolStrip2x2HouseCount.Text = "100";
            toolStrip2x2HouseCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
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
            // toolStripLabelPasteBuildings
            // 
            toolStripLabelPasteBuildings.Name = "toolStripLabelPasteBuildings";
            toolStripLabelPasteBuildings.Size = new Size(114, 22);
            toolStripLabelPasteBuildings.Text = "(right-click to paste)";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
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
            toolStripSecondary.Location = new Point(1246, 0);
            toolStripSecondary.Name = "toolStripSecondary";
            toolStripSecondary.Size = new Size(39, 837);
            toolStripSecondary.TabIndex = 14;
            toolStripSecondary.Text = "toolStrip5";
            // 
            // btnNewFromGameSave
            // 
            btnNewFromGameSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnNewFromGameSave.Name = "btnNewFromGameSave";
            btnNewFromGameSave.Size = new Size(132, 22);
            btnNewFromGameSave.Text = "New from game save...";
            btnNewFromGameSave.Click += btnNewFromGameSave_Click;
            // 
            // openFileDialogImport
            // 
            openFileDialogImport.Filter = "Pharaoh save files|*.sav|All files|*.*";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1324, 837);
            Controls.Add(toolStripTop);
            Controls.Add(toolStripSecondary);
            Controls.Add(canvas);
            Controls.Add(toolStripMain);
            Name = "FormMain";
            Text = "City Planner";
            FormClosing += FormMain_FormClosing;
            toolStripMain.ResumeLayout(false);
            toolStripMain.PerformLayout();
            canvas.ResumeLayout(false);
            toolStripTop.ResumeLayout(false);
            toolStripTop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStripMain;
        private ToolStripButton btnClear;
        private ToolStripButton btnRoad;
        private Panel canvas;
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
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnCopyBuildings;
        private ToolStripButton btnCutBuildings;
        private ToolStripLabel toolStripLabelPasteBuildings;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripLabel toolStripLabelRoadLengthLabel;
        private ToolStripLabel toolStripLabelRoadLength;
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
        private ToolStripLabel toolStripLabel1;
        private ToolStripLabel toolStrip2x2HouseCount;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStrip toolStripSecondary;
        private ToolStripButton btnNewFromGameSave;
        private OpenFileDialog openFileDialogImport;
    }
}