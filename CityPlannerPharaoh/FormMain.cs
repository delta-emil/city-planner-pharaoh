using CityPlannerPharaoh.FileDataExtraction;
using CityPlannerPharaoh.FileFormat;
using CityPlannerPharaoh.Properties;

namespace CityPlannerPharaoh;

public partial class FormMain : Form
{
    private readonly Dictionary<ToolStripButton, SecondaryToolbarButtonData[]> buttonToSecondary;

    private string? fileName;

    public FormMain()
    {
        this.InitializeComponent();

        buttonToSecondary = this.MakeSecondaryToolbars();

        this.mapControl.SetMapModel(new MapModel(MapModel.DefaultMapSize, MapModel.DefaultMapSize));

        foreach (var button in this.buttonToSecondary.Keys)
        {
            button.Click += this.MainToolbarSubbarButtonClick;
        }

        var buttonToBuildingTool = new Dictionary<ToolStripButton, MapBuildingType>
        {
            // main
            { btnRoad, MapBuildingType.Road },
            { btnPlaza, MapBuildingType.Plaza },
        };
        foreach (var (button, buildingType) in buttonToBuildingTool)
        {
            button.Tag = buildingType.ToString();
            button.Click += this.MainToolbarToolButtonClick;
        }

        this.mapControl.ShowBuildings = btnFilterBuildings.Checked;
        this.mapControl.ShowDesirability = btnFilterDesire.Checked;
    }

    private void MainToolbarSubbarButtonClick(object? sender, EventArgs e)
    {
        var currentButton = (ToolStripButton)sender!;

        UntickOtherToolbarButtons(toolStripMain, currentButton);

        var values = this.buttonToSecondary[currentButton];

        this.SuspendLayout();
        this.ClearSecondaryToolbar();

        foreach (var itemValues in values)
        {
            ToolStripItem toolStripItem;
            if (itemValues.Display.Length > 0 || itemValues.Image != null)
            {
                var item = new ToolStripButton
                {
                    DisplayStyle
                        = itemValues.Display.Length > 0
                            ? itemValues.Image != null
                                ? ToolStripItemDisplayStyle.ImageAndText
                                : ToolStripItemDisplayStyle.Text
                            : ToolStripItemDisplayStyle.Image,
                    Size = new Size(39, 19),
                    Text = itemValues.Display,
                    Image = itemValues.Image,
                    ToolTipText = itemValues.Tooltip ?? itemValues.Tag,
                    Tag = itemValues.Tag,
                };
                item.Click += this.SecondaryToolbarButtonClick;
                toolStripItem = item;
            }
            else
            {
                var item = new ToolStripSeparator
                {
                    Size = new Size(39, 6),
                };
                toolStripItem = item;
            }
            toolStripItem.Name = "item" + this.toolStripSecondary.Items.Count;
            this.toolStripSecondary.Items.Add(toolStripItem);
        }

        this.ResumeLayout(true);

        this.mapControl.Tool = new Tool();
    }

    private void MainToolbarToolButtonClick(object? sender, EventArgs e)
    {
        var currentButton = (ToolStripButton)sender!;

        UntickOtherToolbarButtons(toolStripMain, currentButton);
        this.ClearSecondaryToolbar();

        if (currentButton == btnSelect)
        {
            this.mapControl.Tool = new Tool();
        }
        else if (currentButton == btnClear)
        {
            this.mapControl.Tool = new Tool { IsClearBuilding = true };
        }
        else
        {
            var tagString = (string)currentButton.Tag!;
            var buildingType = Enum.Parse<MapBuildingType>(tagString);
            this.mapControl.Tool = new Tool { BuildingType = buildingType };
        }
    }

    private void ClearSecondaryToolbar()
    {
        var oldItems = this.toolStripSecondary.Items.Cast<ToolStripItem>().ToList();
        this.toolStripSecondary.Items.Clear();
        foreach (var oldItem in oldItems)
        {
            oldItem.Dispose();
        }
    }

    private void SecondaryToolbarButtonClick(object? sender, EventArgs e)
    {
        var currentButton = (ToolStripButton)sender!;
        var toolStrip = currentButton.GetCurrentParent()!;

        UntickOtherToolbarButtons(toolStrip, currentButton);

        var tagString = (string)currentButton.Tag!;

        if (tagString.StartsWith(TagPrefixTerrain, StringComparison.Ordinal))
        {
            var terrain = Enum.Parse<MapTerrain>(tagString[TagPrefixTerrain.Length..]);
            this.mapControl.Tool = new Tool { Terrain = terrain };
        }
        else if (tagString.StartsWith("House", StringComparison.Ordinal) && tagString.Contains('_', StringComparison.Ordinal))
        {
            int separatorIndex = tagString.IndexOf('_', StringComparison.Ordinal);
            var buildingType = Enum.Parse<MapBuildingType>(tagString[..separatorIndex]);
            var houseLevel = byte.Parse(tagString[(separatorIndex + 1)..]);
            this.mapControl.Tool = new Tool { BuildingType = buildingType, HouseLevel = houseLevel };
        }
        else
        {
            var buildingType = Enum.Parse<MapBuildingType>(tagString);
            this.mapControl.Tool = new Tool { BuildingType = buildingType };
        }
    }

    private static void UntickOtherToolbarButtons(ToolStrip toolStrip, ToolStripButton activeOne)
    {
        foreach (var toolbarButton in toolStrip.Items.OfType<ToolStripButton>())
        {
            toolbarButton.Checked = toolbarButton == activeOne;
        }
    }

    private void btnFilterBuildings_Click(object sender, EventArgs e)
    {
        this.mapControl.ShowBuildings = btnFilterBuildings.Checked;
        this.mapControl.Invalidate();
    }

    private void btnFilterDesire_Click(object sender, EventArgs e)
    {
        this.mapControl.ShowDesirability = btnFilterDesire.Checked;
        this.mapControl.Invalidate();
    }

    private void btnFilterDesireFull_Click(object sender, EventArgs e)
    {
        this.mapControl.ShowDesirabilityFull = btnFilterDesireFull.Checked;
        this.mapControl.Invalidate();
    }

    private void btnFilterSimpleHouseDesire_Click(object sender, EventArgs e)
    {
        this.mapControl.SetSimpleHouseDesire(btnFilterSimpleHouseDesire.Checked);
    }

    #region file operations

    private void btnFileNew_Click(object sender, EventArgs e)
    {
        if (this.mapControl.IsChanged)
        {
            if (!AskToSaveCurrentFile())
            {
                return;
            }
        }

        InitEmptyFile();
    }

    private void btnNewFromGameSave_Click(object sender, EventArgs e)
    {
        if (this.mapControl.IsChanged)
        {
            if (!AskToSaveCurrentFile())
            {
                return;
            }
        }

        var openDialogResult = this.openFileDialogImport.ShowDialog();
        if (openDialogResult == DialogResult.OK)
        {
            MapModel? mapModel;
            {
                using var saveFile = new PharaohFile(this.openFileDialogImport.FileName);
                mapModel = saveFile.GetMapModelFromFile();
            }

            if (mapModel != null)
            {
                mapModel.SetSimpleHouseDesire(this.mapControl.MapModel.SimpleHouseDesire);
                mapModel.SetDifficulty(this.mapControl.MapModel.EffectiveDifficulty);
                this.fileName = null;
                this.Text = "City Builder Pharaoh";
                this.mapControl.SetMapModel(mapModel);
                this.mapControl.Invalidate();
            }
            else
            {
                MessageBox.Show("Error reading map from save file", "Error");
            }
        }
    }

    private void btnFileOpen_Click(object sender, EventArgs e)
    {
        if (this.mapControl.IsChanged)
        {
            if (!AskToSaveCurrentFile())
            {
                return;
            }
        }

        var openDialogResult = this.openFileDialog.ShowDialog();
        if (openDialogResult == DialogResult.OK)
        {
            var fileNameToLoad = this.openFileDialog.FileName;
            var mapModel = LoadMapModel(fileNameToLoad);
            if (mapModel != null)
            {
                this.fileName = fileNameToLoad;
                this.Text = "City Builder Pharaoh - " + Path.GetFileName(this.fileName);
                this.mapControl.SetMapModel(mapModel);
                this.mapControl.Invalidate();
                this.SetShownDifficulty(mapModel.EffectiveDifficulty);
                this.SetShownSimpleHouseDesire(mapModel.SimpleHouseDesire);
            }
        }
    }

    private void btnFileSave_Click(object sender, EventArgs e)
    {
        if (this.fileName == null)
        {
            btnFileSaveAs_Click(sender, e);
            return;
        }

        if (!this.mapControl.IsChanged)
        {
            return;
        }

        SaveCurrentMapModel();
    }

    private void btnFileSaveAs_Click(object sender, EventArgs e)
    {
        DoSaveDialog();
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (this.mapControl.IsChanged)
        {
            if (!AskToSaveCurrentFile())
            {
                e.Cancel = true;
            }
        }
    }

    private bool AskToSaveCurrentFile()
    {
        var result = MessageBox.Show("Do you want to save the current file?", "Save?", MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Cancel)
        {
            return false;
        }
        else if (result == DialogResult.Yes)
        {
            if (this.fileName != null)
            {
                return SaveCurrentMapModel();
            }
            else
            {
                return DoSaveDialog();
            }
        }
        else if (result == DialogResult.No)
        {
            return true;
        }
        else
        {
            throw new Exception($"Unknown DialogResult: {result}");
        }
    }

    private bool DoSaveDialog()
    {
        if (this.fileName != null)
        {
            this.saveFileDialog.FileName = this.fileName;
        }

        var saveDialogResult = this.saveFileDialog.ShowDialog();
        if (saveDialogResult == DialogResult.OK)
        {
            this.fileName = this.saveFileDialog.FileName;
            this.Text = "City Builder Pharaoh - " + Path.GetFileName(this.fileName);
            return SaveCurrentMapModel();
        }
        else
        {
            return false;
        }
    }

    private void InitEmptyFile()
    {
        var mapModel = new MapModel(MapModel.DefaultMapSize, MapModel.DefaultMapSize);
        mapModel.SetSimpleHouseDesire(this.mapControl.MapModel.SimpleHouseDesire);
        mapModel.SetDifficulty(this.mapControl.MapModel.EffectiveDifficulty);
        this.mapControl.SetMapModel(mapModel);
        this.mapControl.Invalidate();
        this.fileName = null;
        this.Text = "City Builder Pharaoh";
    }

    public bool SaveCurrentMapModel()
    {
        if (!SaveMapModel(this.mapControl.MapModel, this.fileName!))
        {
            return false;
        }

        this.mapControl.ChangesSaved();
        return true;
    }

    public static bool SaveMapModel(MapModel mapModel, string fileName)
    {
        try
        {
            using FileStream outputStream = File.Create(fileName);
            Writer.Write(outputStream, mapModel);

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public static MapModel? LoadMapModel(string fileName)
    {
        try
        {
            using FileStream inputStream = File.Open(fileName, FileMode.Open);
            return Reader.Read(inputStream);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    #endregion

    #region cut-copy-paste

    private void btnCutBuildings_Click(object sender, EventArgs e)
    {
        this.mapControl.BuildingsCut();
    }

    private void btnCopyBuildings_Click(object sender, EventArgs e)
    {
        this.mapControl.BuildingsCopy();
    }

    private void btnPasteBuildings_Click(object sender, EventArgs e)
    {
        this.mapControl.BuildingsPasteGhost();
    }

    #endregion

    private void mapControl_GlobalStatsChanged(object sender, MapGlobalStatsChangeEventArgs e)
    {
        this.toolStripTotalPop.Text = e.Pop.ToString();
        this.toolStripTotalWork23.Text = ((int)((e.Pop - e.PopScribe) * 0.23)).ToString();
        this.toolStripTotalEmpl.Text = e.Empl.ToString();
    }

    private void mapControl_SelectionChanged(object sender, MapSelectionChangeEventArgs e)
    {
        this.toolStripSelectedEmpl.Text = e.SelectedEmpl.ToString();
        this.toolStripLabelRoadLength.Text = e.SelectedRoadLength.ToString();
        this.toolStrip2x2HouseCount.Text = e.Selected2x2HouseCount.ToString();
    }

    private void mapControl_UndoStackChanged(object sender, MapUndoStackChangeEventArgs e)
    {
        this.btnUndo.Enabled = e.CanUndo;
        this.btnRedo.Enabled = e.CanRedo;
        if (e.UpdateContentControls)
        {
            this.SetShownDifficulty(e.Difficulty);
            this.SetShownSimpleHouseDesire(e.SimpleHouseDesire);
        }
    }

    private void mapControl_MouseCoordsChanged(object sender, MouseCoordsChangeEventArgs e)
    {
        string text = $"x:{e.CellX:D3},y:{e.CellY:D3}";
        this.toolStripLabelCoords.Text = text;
    }

    #region secondary toolstrips

    private const string TagPrefixTerrain = "+";

    private Dictionary<ToolStripButton, SecondaryToolbarButtonData[]> MakeSecondaryToolbars()
    {
        var result = new Dictionary<ToolStripButton, SecondaryToolbarButtonData[]>();

        result[btnTerrain] =
        [
            new(TagPrefixTerrain + nameof(MapTerrain.Void),          "Void"),
            new(TagPrefixTerrain + nameof(MapTerrain.Grass),         "G"),
            new(TagPrefixTerrain + nameof(MapTerrain.GrassFarmland), "G+F"),
            new(TagPrefixTerrain + nameof(MapTerrain.Sand),          "S"),
            new(TagPrefixTerrain + nameof(MapTerrain.SandFarmland),  "S+F"),
            new(TagPrefixTerrain + nameof(MapTerrain.Rock),          "R"),
            new(TagPrefixTerrain + nameof(MapTerrain.RockOre),       "R+O"),
            new(TagPrefixTerrain + nameof(MapTerrain.Dune),          "Du"),
            new(TagPrefixTerrain + nameof(MapTerrain.Floodpain),     "F"),
            new(TagPrefixTerrain + nameof(MapTerrain.FloodpainEdge), "F/e"),
            new(TagPrefixTerrain + nameof(MapTerrain.Water),         "W"),
            new(TagPrefixTerrain + nameof(MapTerrain.WaterEdge),     "W/e"),
            new(TagPrefixTerrain + nameof(MapTerrain.Trees),         "Tree"),
            new(TagPrefixTerrain + nameof(MapTerrain.Reeds),         "Reed"),
        ];

        result[btnHouse] = [..
            Enumerable.Range(1, 10)
            .Select(lvl => new SecondaryToolbarButtonData($"{nameof(MapBuildingType.House)}_{lvl}", $"{lvl}.{HouseLevelData.LabelsShort[lvl]}", Tooltip: HouseLevelData.LabelsFull[lvl]))
        ];
        result[btnHouse2] = [..
            Enumerable.Range(1, 14)
            .Select(lvl => new SecondaryToolbarButtonData($"{nameof(MapBuildingType.House2)}_{lvl}", $"{lvl}.{HouseLevelData.LabelsShort[lvl]}", Tooltip: HouseLevelData.LabelsFull[lvl]))
        ];
        result[btnHouse3] = [..
            Enumerable.Range(15, 4)
            .Select(lvl => new SecondaryToolbarButtonData($"{nameof(MapBuildingType.House3)}_{lvl}", $"{lvl}.{HouseLevelData.LabelsShort[lvl]}", Tooltip: HouseLevelData.LabelsFull[lvl]))
        ];
        result[btnHouse4] = [..
            Enumerable.Range(19, 2)
            .Select(lvl => new SecondaryToolbarButtonData($"{nameof(MapBuildingType.House4)}_{lvl}", $"{lvl}.{HouseLevelData.LabelsShort[lvl]}", Tooltip: HouseLevelData.LabelsFull[lvl]))
        ];

        result[btnFood] =
        [
            new(nameof(MapBuildingType.Farm), "Farm"),
            new(nameof(MapBuildingType.Cattle), "Cttl"),
            new(nameof(MapBuildingType.WaterLift), "Lift"),
            new(nameof(MapBuildingType.Ditch), "Dch"),
            new(nameof(MapBuildingType.Hunter), "Hnt"),
            new(nameof(MapBuildingType.Fisher), "Fish"),
            new(nameof(MapBuildingType.WorkCamp), "Cmp"),
        ];

        result[btnIndustry] =
        [
            new(nameof(MapBuildingType.QuarryPlainStone), "P.St"),
            new(nameof(MapBuildingType.QuarryLimestone), "L.St"),
            new(nameof(MapBuildingType.QuarrySandstone), "S.St"),
            new(nameof(MapBuildingType.QuarryGranite), "Gra"),
            new(nameof(MapBuildingType.MineGems), "Gem"),
            new(nameof(MapBuildingType.MineCopper), "Cop"),
            new(nameof(MapBuildingType.MineGold), "Gold"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Clay), "Clay"),
            new(nameof(MapBuildingType.Reed), "Reed"),
            new(nameof(MapBuildingType.Wood), "Wod"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Potter), "Pot"),
            new(nameof(MapBuildingType.Brewer), "Brw"),
            new(nameof(MapBuildingType.Papyrus), "Pap"),
            new(nameof(MapBuildingType.Weaver), "Wvr"),
            new(nameof(MapBuildingType.Jeweler), "Jwl"),
            new(nameof(MapBuildingType.Bricks), "Bri"),
            new(nameof(MapBuildingType.Lamps), "Lmp"),
            new(nameof(MapBuildingType.Paint), "Pnt"),
            new(nameof(MapBuildingType.Shipwright), "Shp"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.GuildBricklayer), "G.Vr"),
            new(nameof(MapBuildingType.GuildCarpenter), "G.Ca"),
            new(nameof(MapBuildingType.GuildStonemason), "G.St"),
            new(nameof(MapBuildingType.GuildArtisan), "G.Ar"),
        ];

        result[btnDistribution] =
        [
            new(nameof(MapBuildingType.Bazaar), "Baz"),
            new(nameof(MapBuildingType.Granary), "Gra"),
            new(nameof(MapBuildingType.StorageYard), "SY"),
            new(nameof(MapBuildingType.Dock), "Dock"),
        ];

        result[btnEnt] =
        [
            new(nameof(MapBuildingType.Booth), "Boo"), // Resources.Booth1),
            new(nameof(MapBuildingType.Bandstand), "Ban"), // Resources.Bandstand1),
            new(nameof(MapBuildingType.Pavilion), "Pav"), // Resources.Pavilion01),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.JuggleSchool), "JuS"),
            new(nameof(MapBuildingType.MusicSchool), "MuS"),
            new(nameof(MapBuildingType.DanceSchool), "DaS"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Senet), "Sen"),
            new(nameof(MapBuildingType.Zoo), "Zoo"),
        ];

        result[btnReligious] =
        [
            new(nameof(MapBuildingType.Shrine), "Shr"),
            new(nameof(MapBuildingType.Temple), "Tem"),
            new(nameof(MapBuildingType.TempleComplex1), "TC1"),
            new(nameof(MapBuildingType.TempleComplex2), "TC2"),
            new(nameof(MapBuildingType.FestivalSquare), "FSq"),
        ];

        result[btnEducation] =
        [
            new(nameof(MapBuildingType.ScribeSchool), "Sch"),
            new(nameof(MapBuildingType.Library), "Lib"),
        ];

        result[btnHealth] =
        [
            new(nameof(MapBuildingType.Well), "Well"),
            new(nameof(MapBuildingType.WaterSupply), "Wat"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Physician), "Phy"),
            new(nameof(MapBuildingType.Apothecary), "Apo"),
            new(nameof(MapBuildingType.Dentist), "Den"),
            new(nameof(MapBuildingType.Mortuary), "Mor"),
        ];

        result[btnMunicipal] =
        [
            new(nameof(MapBuildingType.Firehouse), "Fi"),
            new(nameof(MapBuildingType.Architect), "Ar"),
            new(nameof(MapBuildingType.Police), "Po"),
            new(nameof(MapBuildingType.Tax), "Tax"),
            new(nameof(MapBuildingType.Courthouse), "Crt"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Roadblock), "RB"),
            new(nameof(MapBuildingType.Bridge), "Bri"),
            new(nameof(MapBuildingType.FerryLanding), "Fer"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Garden), "Gar"),
            new(nameof(MapBuildingType.Plaza), "Pla"),
            new(nameof(MapBuildingType.StatueSmall), "SSt"),
            new(nameof(MapBuildingType.StatueMedium), "MSt"),
            new(nameof(MapBuildingType.StatueLarge), "LSt"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.PalaceVillage), "PVi"),
            new(nameof(MapBuildingType.PalaceTown), "PTo"),
            new(nameof(MapBuildingType.PalaceCity), "PCi"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.MansionPersonal), "MPe"),
            new(nameof(MapBuildingType.MansionFamily), "MFa"),
            new(nameof(MapBuildingType.MansionDynasty), "MDy"),
        ];

        result[btnMilitary] =
        [
            new(nameof(MapBuildingType.Wall), "Wal"),
            new(nameof(MapBuildingType.Tower), "Twr"),
            new(nameof(MapBuildingType.Gate1), "Gt1"),
            new(nameof(MapBuildingType.Gate2), "Gt2"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Recruiter), "Rec"),
            new(nameof(MapBuildingType.Academy), "Aca"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Weaponsmith), "Wpn"),
            new(nameof(MapBuildingType.Chariot), "Cha"),
            new(string.Empty, string.Empty),
            new(nameof(MapBuildingType.Fort), "Frt"),
            new(nameof(MapBuildingType.Warship), "WSh"),
            new(nameof(MapBuildingType.TransportShip), "TSh"),
        ];

        return result;
    }

    #endregion

    private void canvas_Zoom(object arg1, ZoomEventArgs e)
    {
        var newSize = this.mapControl.CellSideLength + e.Delta * 2;
        this.mapControl.CellSideLength = newSize;
    }

    private void FormMain_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            if (!this.mapControl.Tool.IsEmpty)
            {
                this.mapControl.Tool = new Tool();
            }
            this.mapControl.ClearBuildingsToPaste();

            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.S && ModifierKeys == Keys.Control)
        {
            this.btnFileSave_Click(this.btnFileSave, EventArgs.Empty);
        }
        else if (e.KeyCode == Keys.C && ModifierKeys == Keys.Control)
        {
            this.mapControl.BuildingsCopy();
        }
        else if (e.KeyCode == Keys.X && ModifierKeys == Keys.Control)
        {
            this.mapControl.BuildingsCut();
        }
        else if (e.KeyCode == Keys.V && ModifierKeys == Keys.Control)
        {
            this.mapControl.BuildingsPasteGhost();
        }
        else if (e.KeyCode == Keys.Z && ModifierKeys == Keys.Control)
        {
            this.mapControl.Undo();
        }
        else if (e.KeyCode == Keys.Z && ModifierKeys == (Keys.Control | Keys.Shift))
        {
            this.mapControl.Redo();
        }
        else if (e.KeyCode == Keys.Y && ModifierKeys == Keys.Control)
        {
            this.mapControl.Redo();
        }
        else if (e.KeyCode == Keys.Delete)
        {
            this.mapControl.BuildingsDelete();
        }
    }

    private void btnDifficulty_Click(object sender, EventArgs e)
    {
        var menuItem = (ToolStripMenuItem)sender;
        Difficulty? difficultyOption
            = menuItem == btnDifficultyVE ? Difficulty.VeryEasy
            : menuItem == btnDifficultyE  ? Difficulty.Easy
            : menuItem == btnDifficultyN  ? Difficulty.Normal
            : menuItem == btnDifficultyH  ? Difficulty.Hard
            : menuItem == btnDifficultyVH ? Difficulty.VeryHard
            : null;
        if (difficultyOption is not Difficulty difficulty)
        {
            return;
        }

        ddDifficulty.Text = menuItem.Text;
        this.mapControl.SetDifficulty(difficulty);
    }

    private void SetShownDifficulty(Difficulty effectiveDifficulty)
    {
        ddDifficulty.Text = effectiveDifficulty switch
        {
            Difficulty.VeryEasy => "Very Easy",
            Difficulty.Easy     => "Easy",
            Difficulty.Normal   => "Normal",
            Difficulty.Hard     => "Hard",
            Difficulty.VeryHard => "Very Hard",
            _                   => MapModel.DefaultDifficulty.ToString(),
        };
    }

    private void SetShownSimpleHouseDesire(bool simpleHouseDesire)
    {
        btnFilterSimpleHouseDesire.Checked = simpleHouseDesire;
    }

    private void btnGlyph_Click(object sender, EventArgs e)
    {
        IEnumerable<MapBuilding> buildings
            = this.mapControl.SelectedBuildings.Count > 0
                ? this.mapControl.SelectedBuildings
                : this.mapControl.MapModel.Buildings;

        var (glyphs, error) = Glyphs.GetGlyphs(buildings);
        if (error != null)
        {
            MessageBox.Show(this, error, "Could not create glyphs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        ExternalHelper.PutTextOnClipboard(glyphs, this);
    }

    private void btnUndo_Click(object sender, EventArgs e)
    {
        this.mapControl.Undo();
    }

    private void btnRedo_Click(object sender, EventArgs e)
    {
        this.mapControl.Redo();
    }

    private record SecondaryToolbarButtonData(string Tag, string Display, Image? Image = null, string? Tooltip = null);
}
