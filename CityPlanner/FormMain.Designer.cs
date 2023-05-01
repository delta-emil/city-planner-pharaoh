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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            Tool tool1 = new Tool();
            toolStripMain = new ToolStrip();
            btnSelect = new ToolStripButton();
            btnTerrain = new ToolStripButton();
            btnClear = new ToolStripButton();
            btnHouse = new ToolStripButton();
            btnRoad = new ToolStripButton();
            btnPlaza = new ToolStripButton();
            btnCommercial = new ToolStripButton();
            canvas = new Panel();
            mapControl = new MapCanvasControl();
            toolStripTerrain = new ToolStrip();
            btnTerrainGrass = new ToolStripButton();
            btnTerrainGrassFamrland = new ToolStripButton();
            btnTerrainSand = new ToolStripButton();
            btnTerrainSandFamrland = new ToolStripButton();
            btnTerrainRock = new ToolStripButton();
            btnTerrainRockOre = new ToolStripButton();
            btnTerrainDune = new ToolStripButton();
            btnTerrainFlood = new ToolStripButton();
            btnTerrainFloodEdge = new ToolStripButton();
            btnTerrainWater = new ToolStripButton();
            btnTerrainWaterEdge = new ToolStripButton();
            toolStripCommercial = new ToolStrip();
            btnBazaar = new ToolStripButton();
            toolStripTop = new ToolStrip();
            btnFileNew = new ToolStripButton();
            btnFileOpen = new ToolStripButton();
            btnFileSave = new ToolStripButton();
            btnFileSaveAs = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnFilterBuildings = new ToolStripButton();
            btnFilterDesire = new ToolStripButton();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            toolStripMain.SuspendLayout();
            canvas.SuspendLayout();
            toolStripTerrain.SuspendLayout();
            toolStripCommercial.SuspendLayout();
            toolStripTop.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripMain
            // 
            toolStripMain.Dock = DockStyle.Right;
            toolStripMain.Items.AddRange(new ToolStripItem[] { btnSelect, btnTerrain, btnClear, btnHouse, btnRoad, btnPlaza, btnCommercial });
            toolStripMain.Location = new Point(916, 0);
            toolStripMain.Name = "toolStripMain";
            toolStripMain.Size = new Size(39, 571);
            toolStripMain.TabIndex = 0;
            toolStripMain.Text = "toolStrip1";
            // 
            // btnSelect
            // 
            btnSelect.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSelect.Image = (Image)resources.GetObject("btnSelect.Image");
            btnSelect.ImageTransparentColor = Color.Magenta;
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(36, 19);
            btnSelect.Text = "SEL";
            btnSelect.ToolTipText = "Select";
            btnSelect.Click += MainToolbarToolButtonClick;
            // 
            // btnTerrain
            // 
            btnTerrain.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrain.Image = (Image)resources.GetObject("btnTerrain.Image");
            btnTerrain.ImageTransparentColor = Color.Magenta;
            btnTerrain.Name = "btnTerrain";
            btnTerrain.Size = new Size(36, 19);
            btnTerrain.Text = "TER";
            btnTerrain.ToolTipText = "Terrain";
            // 
            // btnClear
            // 
            btnClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnClear.Image = (Image)resources.GetObject("btnClear.Image");
            btnClear.ImageTransparentColor = Color.Magenta;
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(36, 19);
            btnClear.Text = "DEL";
            btnClear.ToolTipText = "Clear";
            btnClear.Click += MainToolbarToolButtonClick;
            // 
            // btnHouse
            // 
            btnHouse.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHouse.Image = (Image)resources.GetObject("btnHouse.Image");
            btnHouse.ImageTransparentColor = Color.Magenta;
            btnHouse.Name = "btnHouse";
            btnHouse.Size = new Size(36, 19);
            btnHouse.Text = "H";
            btnHouse.ToolTipText = "Housing";
            // 
            // btnRoad
            // 
            btnRoad.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnRoad.Image = (Image)resources.GetObject("btnRoad.Image");
            btnRoad.ImageTransparentColor = Color.Magenta;
            btnRoad.Name = "btnRoad";
            btnRoad.Size = new Size(36, 19);
            btnRoad.Text = "Road";
            // 
            // btnPlaza
            // 
            btnPlaza.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnPlaza.Image = (Image)resources.GetObject("btnPlaza.Image");
            btnPlaza.ImageTransparentColor = Color.Magenta;
            btnPlaza.Name = "btnPlaza";
            btnPlaza.Size = new Size(36, 19);
            btnPlaza.Text = "Plaza";
            // 
            // btnCommercial
            // 
            btnCommercial.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnCommercial.Image = (Image)resources.GetObject("btnCommercial.Image");
            btnCommercial.ImageTransparentColor = Color.Magenta;
            btnCommercial.Name = "btnCommercial";
            btnCommercial.Size = new Size(36, 19);
            btnCommercial.Text = "Co";
            btnCommercial.ToolTipText = "Commercial";
            // 
            // canvas
            // 
            canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            canvas.AutoScroll = true;
            canvas.Controls.Add(mapControl);
            canvas.Location = new Point(0, 28);
            canvas.Name = "canvas";
            canvas.Size = new Size(877, 543);
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
            // 
            // toolStripTerrain
            // 
            toolStripTerrain.Dock = DockStyle.Right;
            toolStripTerrain.Items.AddRange(new ToolStripItem[] { btnTerrainGrass, btnTerrainGrassFamrland, btnTerrainSand, btnTerrainSandFamrland, btnTerrainRock, btnTerrainRockOre, btnTerrainDune, btnTerrainFlood, btnTerrainFloodEdge, btnTerrainWater, btnTerrainWaterEdge });
            toolStripTerrain.Location = new Point(880, 0);
            toolStripTerrain.Name = "toolStripTerrain";
            toolStripTerrain.Size = new Size(36, 571);
            toolStripTerrain.TabIndex = 2;
            toolStripTerrain.Text = "toolStrip1";
            // 
            // btnTerrainGrass
            // 
            btnTerrainGrass.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainGrass.Image = (Image)resources.GetObject("btnTerrainGrass.Image");
            btnTerrainGrass.ImageTransparentColor = Color.Magenta;
            btnTerrainGrass.Name = "btnTerrainGrass";
            btnTerrainGrass.Size = new Size(33, 19);
            btnTerrainGrass.Text = "G";
            btnTerrainGrass.ToolTipText = "Grass - free with water";
            // 
            // btnTerrainGrassFamrland
            // 
            btnTerrainGrassFamrland.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainGrassFamrland.Image = (Image)resources.GetObject("btnTerrainGrassFamrland.Image");
            btnTerrainGrassFamrland.ImageTransparentColor = Color.Magenta;
            btnTerrainGrassFamrland.Name = "btnTerrainGrassFamrland";
            btnTerrainGrassFamrland.Size = new Size(33, 19);
            btnTerrainGrassFamrland.Text = "G+F";
            btnTerrainGrassFamrland.ToolTipText = "Grass Farmland";
            // 
            // btnTerrainSand
            // 
            btnTerrainSand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainSand.Image = (Image)resources.GetObject("btnTerrainSand.Image");
            btnTerrainSand.ImageTransparentColor = Color.Magenta;
            btnTerrainSand.Name = "btnTerrainSand";
            btnTerrainSand.Size = new Size(33, 19);
            btnTerrainSand.Text = "S";
            btnTerrainSand.ToolTipText = "Sand - free w/o water";
            // 
            // btnTerrainSandFamrland
            // 
            btnTerrainSandFamrland.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainSandFamrland.Image = (Image)resources.GetObject("btnTerrainSandFamrland.Image");
            btnTerrainSandFamrland.ImageTransparentColor = Color.Magenta;
            btnTerrainSandFamrland.Name = "btnTerrainSandFamrland";
            btnTerrainSandFamrland.Size = new Size(33, 19);
            btnTerrainSandFamrland.Text = "S+F";
            btnTerrainSandFamrland.ToolTipText = "Sand Farmland";
            // 
            // btnTerrainRock
            // 
            btnTerrainRock.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainRock.Image = (Image)resources.GetObject("btnTerrainRock.Image");
            btnTerrainRock.ImageTransparentColor = Color.Magenta;
            btnTerrainRock.Name = "btnTerrainRock";
            btnTerrainRock.Size = new Size(33, 19);
            btnTerrainRock.Text = "R";
            btnTerrainRock.ToolTipText = "Rocks";
            // 
            // btnTerrainRockOre
            // 
            btnTerrainRockOre.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainRockOre.Image = (Image)resources.GetObject("btnTerrainRockOre.Image");
            btnTerrainRockOre.ImageTransparentColor = Color.Magenta;
            btnTerrainRockOre.Name = "btnTerrainRockOre";
            btnTerrainRockOre.Size = new Size(33, 19);
            btnTerrainRockOre.Text = "R+O";
            btnTerrainRockOre.ToolTipText = "Rocks with Ore";
            // 
            // btnTerrainDune
            // 
            btnTerrainDune.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainDune.Image = (Image)resources.GetObject("btnTerrainDune.Image");
            btnTerrainDune.ImageTransparentColor = Color.Magenta;
            btnTerrainDune.Name = "btnTerrainDune";
            btnTerrainDune.Size = new Size(33, 19);
            btnTerrainDune.Text = "Du";
            btnTerrainDune.ToolTipText = "Dunes";
            // 
            // btnTerrainFlood
            // 
            btnTerrainFlood.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainFlood.Image = (Image)resources.GetObject("btnTerrainFlood.Image");
            btnTerrainFlood.ImageTransparentColor = Color.Magenta;
            btnTerrainFlood.Name = "btnTerrainFlood";
            btnTerrainFlood.Size = new Size(33, 19);
            btnTerrainFlood.Text = "F";
            btnTerrainFlood.ToolTipText = "Floorplains";
            // 
            // btnTerrainFloodEdge
            // 
            btnTerrainFloodEdge.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainFloodEdge.Image = (Image)resources.GetObject("btnTerrainFloodEdge.Image");
            btnTerrainFloodEdge.ImageTransparentColor = Color.Magenta;
            btnTerrainFloodEdge.Name = "btnTerrainFloodEdge";
            btnTerrainFloodEdge.Size = new Size(33, 19);
            btnTerrainFloodEdge.Text = "F/e";
            btnTerrainFloodEdge.ToolTipText = "Floorplain bank";
            // 
            // btnTerrainWater
            // 
            btnTerrainWater.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainWater.Image = (Image)resources.GetObject("btnTerrainWater.Image");
            btnTerrainWater.ImageTransparentColor = Color.Magenta;
            btnTerrainWater.Name = "btnTerrainWater";
            btnTerrainWater.Size = new Size(33, 19);
            btnTerrainWater.Text = "W";
            btnTerrainWater.ToolTipText = "Water";
            // 
            // btnTerrainWaterEdge
            // 
            btnTerrainWaterEdge.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTerrainWaterEdge.Image = (Image)resources.GetObject("btnTerrainWaterEdge.Image");
            btnTerrainWaterEdge.ImageTransparentColor = Color.Magenta;
            btnTerrainWaterEdge.Name = "btnTerrainWaterEdge";
            btnTerrainWaterEdge.Size = new Size(33, 19);
            btnTerrainWaterEdge.Text = "W/e";
            btnTerrainWaterEdge.ToolTipText = "Water bank";
            // 
            // toolStripCommercial
            // 
            toolStripCommercial.Dock = DockStyle.Right;
            toolStripCommercial.Items.AddRange(new ToolStripItem[] { btnBazaar });
            toolStripCommercial.Location = new Point(850, 0);
            toolStripCommercial.Name = "toolStripCommercial";
            toolStripCommercial.Size = new Size(30, 571);
            toolStripCommercial.TabIndex = 3;
            toolStripCommercial.Text = "toolStrip1";
            // 
            // btnBazaar
            // 
            btnBazaar.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnBazaar.Image = (Image)resources.GetObject("btnBazaar.Image");
            btnBazaar.ImageTransparentColor = Color.Magenta;
            btnBazaar.Name = "btnBazaar";
            btnBazaar.Size = new Size(27, 19);
            btnBazaar.Text = "Baz";
            // 
            // toolStripTop
            // 
            toolStripTop.Items.AddRange(new ToolStripItem[] { btnFileNew, btnFileOpen, btnFileSave, btnFileSaveAs, toolStripSeparator1, btnFilterBuildings, btnFilterDesire });
            toolStripTop.Location = new Point(0, 0);
            toolStripTop.Name = "toolStripTop";
            toolStripTop.Size = new Size(850, 25);
            toolStripTop.TabIndex = 4;
            toolStripTop.Text = "toolStrip1";
            // 
            // btnFileNew
            // 
            btnFileNew.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileNew.Image = (Image)resources.GetObject("btnFileNew.Image");
            btnFileNew.ImageTransparentColor = Color.Magenta;
            btnFileNew.Name = "btnFileNew";
            btnFileNew.Size = new Size(35, 22);
            btnFileNew.Text = "New";
            btnFileNew.Click += btnFileNew_Click;
            // 
            // btnFileOpen
            // 
            btnFileOpen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileOpen.Image = (Image)resources.GetObject("btnFileOpen.Image");
            btnFileOpen.ImageTransparentColor = Color.Magenta;
            btnFileOpen.Name = "btnFileOpen";
            btnFileOpen.Size = new Size(49, 22);
            btnFileOpen.Text = "Open...";
            btnFileOpen.Click += btnFileOpen_Click;
            // 
            // btnFileSave
            // 
            btnFileSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileSave.Image = (Image)resources.GetObject("btnFileSave.Image");
            btnFileSave.ImageTransparentColor = Color.Magenta;
            btnFileSave.Name = "btnFileSave";
            btnFileSave.Size = new Size(35, 22);
            btnFileSave.Text = "Save";
            btnFileSave.Click += btnFileSave_Click;
            // 
            // btnFileSaveAs
            // 
            btnFileSaveAs.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFileSaveAs.Image = (Image)resources.GetObject("btnFileSaveAs.Image");
            btnFileSaveAs.ImageTransparentColor = Color.Magenta;
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
            btnFilterBuildings.Image = (Image)resources.GetObject("btnFilterBuildings.Image");
            btnFilterBuildings.ImageTransparentColor = Color.Magenta;
            btnFilterBuildings.Name = "btnFilterBuildings";
            btnFilterBuildings.Size = new Size(92, 22);
            btnFilterBuildings.Text = "Show Buildings";
            btnFilterBuildings.Click += btnFilterBuildings_Click;
            // 
            // btnFilterDesire
            // 
            btnFilterDesire.CheckOnClick = true;
            btnFilterDesire.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilterDesire.Image = (Image)resources.GetObject("btnFilterDesire.Image");
            btnFilterDesire.ImageTransparentColor = Color.Magenta;
            btnFilterDesire.Name = "btnFilterDesire";
            btnFilterDesire.Size = new Size(104, 22);
            btnFilterDesire.Text = "Show Desirablility";
            btnFilterDesire.Click += btnFilterDesire_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "City Planner Pharaoh files|*.cityph|All files|*.*";
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "City Planner Pharaoh files|*.cityph|All files|*.*";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(955, 571);
            Controls.Add(toolStripTop);
            Controls.Add(toolStripCommercial);
            Controls.Add(toolStripTerrain);
            Controls.Add(canvas);
            Controls.Add(toolStripMain);
            Name = "FormMain";
            Text = "City Planner";
            toolStripMain.ResumeLayout(false);
            toolStripMain.PerformLayout();
            canvas.ResumeLayout(false);
            toolStripTerrain.ResumeLayout(false);
            toolStripTerrain.PerformLayout();
            toolStripCommercial.ResumeLayout(false);
            toolStripCommercial.PerformLayout();
            toolStripTop.ResumeLayout(false);
            toolStripTop.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStripMain;
        private ToolStripButton btnClear;
        private ToolStripButton btnRoad;
        private Panel canvas;
        private MapCanvasControl mapControl;
        private ToolStrip toolStripTerrain;
        private ToolStripButton btnTerrainGrass;
        private ToolStripButton btnSelect;
        private ToolStripButton btnTerrain;
        private ToolStripButton btnHouse;
        private ToolStripButton btnTerrainGrassFamrland;
        private ToolStripButton btnTerrainSand;
        private ToolStripButton btnTerrainSandFamrland;
        private ToolStripButton btnTerrainRock;
        private ToolStripButton btnTerrainRockOre;
        private ToolStripButton btnTerrainDune;
        private ToolStripButton btnTerrainFlood;
        private ToolStripButton btnTerrainFloodEdge;
        private ToolStripButton btnTerrainWater;
        private ToolStripButton btnTerrainWaterEdge;
        private ToolStripButton btnPlaza;
        private ToolStrip toolStripCommercial;
        private ToolStripButton btnBazaar;
        private ToolStripButton btnCommercial;
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
    }
}