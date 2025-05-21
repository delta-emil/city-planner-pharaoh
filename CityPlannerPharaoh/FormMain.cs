using CityPlannerPharaoh.FileDataExtraction;
using CityPlannerPharaoh.Properties;
using System.Text.Json;
using System.Windows.Forms;

namespace CityPlannerPharaoh;

public partial class FormMain : Form
{
    private readonly Dictionary<ToolStripButton, (string Tag, string Display, Image? Image)[]> buttonToSecondary;

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
            { btnHouse, MapBuildingType.House },
            { btnHouse2, MapBuildingType.House2 },
            { btnHouse3, MapBuildingType.House3 },
            { btnHouse4, MapBuildingType.House4 },
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
                    ToolTipText = itemValues.Tag, // TODO: improve maybe have a field about it
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

        if (tagString.StartsWith(TagPrefixTerrain))
        {
            var terrain = Enum.Parse<MapTerrain>(tagString[TagPrefixTerrain.Length..]);
            this.mapControl.Tool = new Tool { Terrain = terrain };
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
            JsonSerializer.Serialize(outputStream, mapModel, ExternalHelper.JsonSerializerOptions);

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
            return JsonSerializer.Deserialize<MapModel>(inputStream, ExternalHelper.JsonSerializerOptions);
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

    #endregion

    private void mapControl_SelectionChanged(object sender, MapSelectionChangeEventArgs e)
    {
        this.toolStripLabelRoadLength.Text = e.SelectedRoadLength.ToString();
        this.toolStrip2x2HouseCount.Text = e.Selected2x2HouseCount.ToString();
    }

    private void mapControl_UndoStackChanged(object sender, MapUndoStackChangeEventArgs e)
    {
        this.btnUndo.Enabled = e.CanUndo;
        this.btnRedo.Enabled = e.CanRedo;
        this.SetShownDifficulty(e.Difficulty);
    }

    #region secondary toolstrips

    private const string TagPrefixTerrain = "+";

    private Dictionary<ToolStripButton, (string Tag, string Display, Image? Image)[]> MakeSecondaryToolbars()
    {
        var result = new Dictionary<ToolStripButton, (string Tag, string Display, Image? Image)[]>();

        result[btnTerrain] =
        [
            (TagPrefixTerrain + nameof(MapTerrain.Void), "Void", null),
            (TagPrefixTerrain + nameof(MapTerrain.Grass), "G", null),
            (TagPrefixTerrain + nameof(MapTerrain.GrassFarmland), "G+F", null),
            (TagPrefixTerrain + nameof(MapTerrain.Sand), "S", null),
            (TagPrefixTerrain + nameof(MapTerrain.SandFarmland), "S+F", null),
            (TagPrefixTerrain + nameof(MapTerrain.Rock), "R", null),
            (TagPrefixTerrain + nameof(MapTerrain.RockOre), "R+O", null),
            (TagPrefixTerrain + nameof(MapTerrain.Dune), "Du", null),
            (TagPrefixTerrain + nameof(MapTerrain.Floodpain), "F", null),
            (TagPrefixTerrain + nameof(MapTerrain.FloodpainEdge), "F/e", null),
            (TagPrefixTerrain + nameof(MapTerrain.Water), "W", null),
            (TagPrefixTerrain + nameof(MapTerrain.WaterEdge), "W/e", null),
            (TagPrefixTerrain + nameof(MapTerrain.Trees), "Tree", null),
            (TagPrefixTerrain + nameof(MapTerrain.Reeds), "Reed", null),
        ];

        result[btnFood] =
        [
            (nameof(MapBuildingType.Farm), "Farm", null),
            (nameof(MapBuildingType.Cattle), "Cttl", null),
            (nameof(MapBuildingType.WaterLift), "Lift", null),
            (nameof(MapBuildingType.Ditch), "Dch", null),
            (nameof(MapBuildingType.Hunter), "Hnt", null),
            (nameof(MapBuildingType.Fisher), "Fish", null),
            (nameof(MapBuildingType.WorkCamp), "Cmp", null),
        ];

        result[btnIndustry] =
        [
            (nameof(MapBuildingType.QuarryPlainStone), "P.St", null),
            (nameof(MapBuildingType.QuarryLimestone), "L.St", null),
            (nameof(MapBuildingType.QuarrySandstone), "S.St", null),
            (nameof(MapBuildingType.QuarryGranite), "Gra", null),
            (nameof(MapBuildingType.MineGems), "Gem", null),
            (nameof(MapBuildingType.MineCopper), "Cop", null),
            (nameof(MapBuildingType.MineGold), "Gold", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Clay), "Clay", null),
            (nameof(MapBuildingType.Reed), "Reed", null),
            (nameof(MapBuildingType.Wood), "Wod", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Potter), "Pot", null),
            (nameof(MapBuildingType.Brewer), "Brw", null),
            (nameof(MapBuildingType.Papyrus), "Pap", null),
            (nameof(MapBuildingType.Weaver), "Wvr", null),
            (nameof(MapBuildingType.Jeweler), "Jwl", null),
            (nameof(MapBuildingType.Bricks), "Bri", null),
            (nameof(MapBuildingType.Lamps), "Lmp", null),
            (nameof(MapBuildingType.Paint), "Pnt", null),
            (nameof(MapBuildingType.Shipwright), "Shp", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.GuildBricklayer), "G.Vr", null),
            (nameof(MapBuildingType.GuildCarpenter), "G.Ca", null),
            (nameof(MapBuildingType.GuildStonemason), "G.St", null),
            (nameof(MapBuildingType.GuildArtisan), "G.Ar", null),
        ];

        result[btnDistribution] =
        [
            (nameof(MapBuildingType.Bazaar), "Baz", null),
            (nameof(MapBuildingType.Granary), "Gra", null),
            (nameof(MapBuildingType.StorageYard), "SY", null),
            (nameof(MapBuildingType.Dock), "Dock", null),
        ];

        result[btnEnt] =
        [
            (nameof(MapBuildingType.Booth1), string.Empty, Resources.Booth1),
            (nameof(MapBuildingType.Booth2), string.Empty, Resources.Booth2),
            (nameof(MapBuildingType.Booth3), string.Empty, Resources.Booth3),
            (nameof(MapBuildingType.Booth4), string.Empty, Resources.Booth4),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Bandstand1), string.Empty, Resources.Bandstand1),
            (nameof(MapBuildingType.Bandstand2), string.Empty, Resources.Bandstand2),
            (nameof(MapBuildingType.Bandstand3), string.Empty, Resources.Bandstand3),
            (nameof(MapBuildingType.Bandstand4), string.Empty, Resources.Bandstand4),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Pavilion1),  string.Empty, Resources.Pavilion01),
            (nameof(MapBuildingType.Pavilion2),  string.Empty, Resources.Pavilion02),
            (nameof(MapBuildingType.Pavilion3),  string.Empty, Resources.Pavilion03),
            (nameof(MapBuildingType.Pavilion4),  string.Empty, Resources.Pavilion04),
            (nameof(MapBuildingType.Pavilion5),  string.Empty, Resources.Pavilion05),
            (nameof(MapBuildingType.Pavilion6),  string.Empty, Resources.Pavilion06),
            (nameof(MapBuildingType.Pavilion7),  string.Empty, Resources.Pavilion07),
            (nameof(MapBuildingType.Pavilion8),  string.Empty, Resources.Pavilion08),
            (nameof(MapBuildingType.Pavilion9),  string.Empty, Resources.Pavilion09),
            (nameof(MapBuildingType.Pavilion10), string.Empty, Resources.Pavilion10),
            (nameof(MapBuildingType.Pavilion11), string.Empty, Resources.Pavilion11),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.JuggleSchool), "JuS", null),
            (nameof(MapBuildingType.MusicSchool), "MuS", null),
            (nameof(MapBuildingType.DanceSchool), "DaS", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Senet), "Sen", null),
            (nameof(MapBuildingType.Zoo), "Zoo", null),
        ];

        result[btnReligious] =
        [
            (nameof(MapBuildingType.Shrine), "Shr", null),
            (nameof(MapBuildingType.Temple), "Tem", null),
            (nameof(MapBuildingType.TempleComplex1), "TC1", null),
            (nameof(MapBuildingType.TempleComplex2), "TC2", null),
            (nameof(MapBuildingType.FestivalSquare), "FSq", null),
        ];

        result[btnEducation] =
        [
            (nameof(MapBuildingType.ScribeSchool), "Sch", null),
            (nameof(MapBuildingType.Library), "Lib", null),
        ];

        result[btnHealth] =
        [
            (nameof(MapBuildingType.Well), "Well", null),
            (nameof(MapBuildingType.WaterSupply), "Wat", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Physician), "Phy", null),
            (nameof(MapBuildingType.Apothecary), "Apo", null),
            (nameof(MapBuildingType.Dentist), "Den", null),
            (nameof(MapBuildingType.Mortuary), "Mor", null),
        ];

        result[btnMunicipal] =
        [
            (nameof(MapBuildingType.Firehouse), "Fi", null),
            (nameof(MapBuildingType.Architect), "Ar", null),
            (nameof(MapBuildingType.Police), "Po", null),
            (nameof(MapBuildingType.Tax), "Tax", null),
            (nameof(MapBuildingType.Courthouse), "Crt", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Roadblock), "RB", null),
            (nameof(MapBuildingType.Bridge), "Bri", null),
            (nameof(MapBuildingType.FerryLanding), "Fer", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Garden), "Gar", null),
            (nameof(MapBuildingType.Plaza), "Pla", null),
            (nameof(MapBuildingType.StatueSmall), "SSt", null),
            (nameof(MapBuildingType.StatueMedium), "MSt", null),
            (nameof(MapBuildingType.StatueLarge), "LSt", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.PalaceVillage), "PVi", null),
            (nameof(MapBuildingType.PalaceTown), "PTo", null),
            (nameof(MapBuildingType.PalaceCity), "PCi", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.MansionPersonal), "MPe", null),
            (nameof(MapBuildingType.MansionFamily), "MFa", null),
            (nameof(MapBuildingType.MansionDynasty), "MDy", null),
        ];

        result[btnMilitary] =
        [
            (nameof(MapBuildingType.Wall), "Wal", null),
            (nameof(MapBuildingType.Tower), "Twr", null),
            (nameof(MapBuildingType.Gate1), "Gt1", null),
            (nameof(MapBuildingType.Gate2), "Gt2", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Recruiter), "Rec", null),
            (nameof(MapBuildingType.Academy), "Aca", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Weaponsmith), "Wpn", null),
            (nameof(MapBuildingType.Chariot), "Cha", null),
            (string.Empty, string.Empty, null),
            (nameof(MapBuildingType.Fort), "Frt", null),
            (nameof(MapBuildingType.Warship), "WSh", null),
            (nameof(MapBuildingType.TransportShip), "TSh", null),
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
            _                   => "Hard",
        };
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
}
