using CityPlannerPharaoh.FileDataExtraction;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityPlannerPharaoh;

public partial class FormMain : Form
{
    private readonly Dictionary<ToolStripButton, (string Tag, string Display)[]> buttonToSecondary;

    private string? fileName;

    public FormMain()
    {
        this.InitializeComponent();

        buttonToSecondary = this.MakeSecondaryToolbars();

        var mapModel = new MapModel(MapModel.DefaultMapSize, MapModel.DefaultMapSize);
        this.mapControl.MapModel = mapModel;
        this.mapControl.SetSizeToFullMapSize();

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
            if (itemValues.Display.Length > 0)
            {
                var item = new ToolStripButton
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Text,
                    Size = new Size(39, 19),
                    Text = itemValues.Display,
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
            var tagString = (string)currentButton.Tag;
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
        var toolStrip = currentButton.GetCurrentParent();

        UntickOtherToolbarButtons(toolStrip, currentButton);

        var tagString = (string)currentButton.Tag;

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
        if (this.mapControl.MapModel.IsChanged)
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
        if (this.mapControl.MapModel.IsChanged)
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
                this.fileName = null;
                this.mapControl.MapModel = mapModel;
                this.mapControl.SetSizeToFullMapSize();
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
        if (this.mapControl.MapModel.IsChanged)
        {
            if (!AskToSaveCurrentFile())
            {
                return;
            }
        }

        var openDialogResult = this.openFileDialog.ShowDialog();
        if (openDialogResult == DialogResult.OK)
        {
            this.fileName = this.openFileDialog.FileName;
            var mapModel = LoadMapModel(fileName);
            if (mapModel != null)
            {
                this.mapControl.MapModel = mapModel;
                this.mapControl.SetSizeToFullMapSize();
                this.mapControl.Invalidate();
            }
            else
            {
                InitEmptyFile();
            }
        }
        else
        {
            InitEmptyFile();
        }
    }

    private void btnFileSave_Click(object sender, EventArgs e)
    {
        if (!this.mapControl.MapModel.IsChanged)
        {
            return;
        }

        if (this.fileName == null)
        {
            btnFileSaveAs_Click(sender, e);
        }
        else
        {
            SaveMapModel(this.mapControl.MapModel, this.fileName);
        }
    }

    private void btnFileSaveAs_Click(object sender, EventArgs e)
    {
        DoSaveDialog();
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (this.mapControl.MapModel.IsChanged)
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
                return SaveMapModel(this.mapControl.MapModel, this.fileName);
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
        var saveDialogResult = this.saveFileDialog.ShowDialog();
        if (saveDialogResult == DialogResult.OK)
        {
            this.fileName = this.saveFileDialog.FileName;
            return SaveMapModel(this.mapControl.MapModel, this.fileName);
        }
        else
        {
            return false;
        }
    }

    private void InitEmptyFile()
    {
        this.mapControl.MapModel = new MapModel(MapModel.DefaultMapSize, MapModel.DefaultMapSize);
        this.mapControl.SetSizeToFullMapSize();
        this.mapControl.Invalidate();
        this.fileName = null;
    }

    public static bool SaveMapModel(MapModel mapModel, string fileName)
    {
        try
        {
            using FileStream outputStream = File.Create(fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new MapCellsJsonConverter());
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            JsonSerializer.Serialize(outputStream, mapModel, options);

            mapModel.IsChanged = false;

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
            var options = new JsonSerializerOptions();
            options.Converters.Add(new MapCellsJsonConverter());
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            return JsonSerializer.Deserialize<MapModel>(inputStream, options);
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

    #region secondary toolstrips

    private const string TagPrefixTerrain = "+";

    private Dictionary<ToolStripButton, (string Tag, string Display)[]> MakeSecondaryToolbars()
    {
        var result = new Dictionary<ToolStripButton, (string Tag, string Display)[]>();

        result[btnTerrain] = new[]
        {
            (TagPrefixTerrain + nameof(MapTerrain.Void), "Void"),
            (TagPrefixTerrain + nameof(MapTerrain.Grass), "G"),
            (TagPrefixTerrain + nameof(MapTerrain.GrassFarmland), "G+F"),
            (TagPrefixTerrain + nameof(MapTerrain.Sand), "S"),
            (TagPrefixTerrain + nameof(MapTerrain.SandFarmland), "S+F"),
            (TagPrefixTerrain + nameof(MapTerrain.Rock), "R"),
            (TagPrefixTerrain + nameof(MapTerrain.RockOre), "R+O"),
            (TagPrefixTerrain + nameof(MapTerrain.Dune), "Du"),
            (TagPrefixTerrain + nameof(MapTerrain.Floodpain), "F"),
            (TagPrefixTerrain + nameof(MapTerrain.FloodpainEdge), "F/e"),
            (TagPrefixTerrain + nameof(MapTerrain.Water), "W"),
            (TagPrefixTerrain + nameof(MapTerrain.WaterEdge), "W/e"),
            (TagPrefixTerrain + nameof(MapTerrain.Trees), "Tree"),
            (TagPrefixTerrain + nameof(MapTerrain.Reeds), "Reed"),
        };

        result[btnFood] = new[]
        {
            (nameof(MapBuildingType.Farm), "Farm"),
            (nameof(MapBuildingType.Cattle), "Cttl"),
            (nameof(MapBuildingType.WaterLift), "Lift"),
            (nameof(MapBuildingType.Ditch), "Dch"),
            (nameof(MapBuildingType.Hunter), "Hnt"),
            (nameof(MapBuildingType.Fisher), "Fish"),
            (nameof(MapBuildingType.WorkCamp), "Cmp"),
        };

        result[btnIndustry] = new[]
        {
            (nameof(MapBuildingType.QuarryPlainStone), "P.St"),
            (nameof(MapBuildingType.QuarryLimestone), "L.St"),
            (nameof(MapBuildingType.QuarrySandstone), "S.St"),
            (nameof(MapBuildingType.QuarryGranite), "Gra"),
            (nameof(MapBuildingType.MineGems), "Gem"),
            (nameof(MapBuildingType.MineCopper), "Cop"),
            (nameof(MapBuildingType.MineGold), "Gold"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Clay), "Clay"),
            (nameof(MapBuildingType.Reed), "Reed"),
            (nameof(MapBuildingType.Wood), "Wod"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Potter), "Pot"),
            (nameof(MapBuildingType.Brewer), "Brw"),
            (nameof(MapBuildingType.Papyrus), "Pap"),
            (nameof(MapBuildingType.Weaver), "Wvr"),
            (nameof(MapBuildingType.Jeweler), "Jwl"),
            (nameof(MapBuildingType.Bricks), "Bri"),
            (nameof(MapBuildingType.Lamps), "Lmp"),
            (nameof(MapBuildingType.Paint), "Pnt"),
            (nameof(MapBuildingType.Shipwright), "Shp"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.GuildBricklayer), "G.Vr"),
            (nameof(MapBuildingType.GuildCarpenter), "G.Ca"),
            (nameof(MapBuildingType.GuildStonemason), "G.St"),
            (nameof(MapBuildingType.GuildArtisan), "G.Ar"),
        };

        result[btnDistribution] = new[]
        {
            (nameof(MapBuildingType.Bazaar), "Baz"),
            (nameof(MapBuildingType.Granary), "Gra"),
            (nameof(MapBuildingType.StorageYard), "SY"),
            (nameof(MapBuildingType.Dock), "Dock"),
        };

        result[btnEnt] = new[]
        {
            (nameof(MapBuildingType.Booth1), "Bo1"),
            (nameof(MapBuildingType.Booth2), "Bo2"),
            (nameof(MapBuildingType.Booth3), "Bo3"),
            (nameof(MapBuildingType.Booth4), "Bo4"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Bandstand1), "Ba1"),
            (nameof(MapBuildingType.Bandstand2), "Ba2"),
            (nameof(MapBuildingType.Bandstand3), "Ba3"),
            (nameof(MapBuildingType.Bandstand4), "Ba4"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Pavilion1), "Pa1"),
            (nameof(MapBuildingType.Pavilion2), "Pa2"),
            (nameof(MapBuildingType.Pavilion3), "Pa3"),
            (nameof(MapBuildingType.Pavilion4), "Pa4"),
            (nameof(MapBuildingType.Pavilion5), "Pa5"),
            (nameof(MapBuildingType.Pavilion6), "Pa6"),
            (nameof(MapBuildingType.Pavilion7), "Pa7"),
            (nameof(MapBuildingType.Pavilion8), "Pa8"),
            (nameof(MapBuildingType.Pavilion9), "Pa9"),
            (nameof(MapBuildingType.Pavilion10), "Pa10"),
            (nameof(MapBuildingType.Pavilion11), "Pa11"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.JuggleSchool), "JuS"),
            (nameof(MapBuildingType.MusicSchool), "MuS"),
            (nameof(MapBuildingType.DanceSchool), "DaS"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Senet), "Sen"),
            (nameof(MapBuildingType.Zoo), "Zoo"),
        };

        result[btnReligious] = new[]
        {
            (nameof(MapBuildingType.Shrine), "Shr"),
            (nameof(MapBuildingType.Temple), "Tem"),
            (nameof(MapBuildingType.TempleComplex1), "TC1"),
            (nameof(MapBuildingType.TempleComplex2), "TC2"),
            (nameof(MapBuildingType.FestivalSquare), "FSq"),
        };

        result[btnEducation] = new[]
        {
            (nameof(MapBuildingType.ScribeSchool), "Sch"),
            (nameof(MapBuildingType.Library), "Lib"),
        };

        result[btnHealth] = new[]
        {
            (nameof(MapBuildingType.Well), "Well"),
            (nameof(MapBuildingType.WaterSupply), "Wat"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Physician), "Phy"),
            (nameof(MapBuildingType.Apothecary), "Apo"),
            (nameof(MapBuildingType.Dentist), "Den"),
            (nameof(MapBuildingType.Mortuary), "Mor"),
        };

        result[btnMunicipal] = new[]
        {
            (nameof(MapBuildingType.Firehouse), "Fi"),
            (nameof(MapBuildingType.Architect), "Ar"),
            (nameof(MapBuildingType.Police), "Po"),
            (nameof(MapBuildingType.Tax), "Tax"),
            (nameof(MapBuildingType.Courthouse), "Crt"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Roadblock), "RB"),
            (nameof(MapBuildingType.Bridge), "Bri"),
            (nameof(MapBuildingType.FerryLanding), "Fer"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Garden), "Gar"),
            (nameof(MapBuildingType.Plaza), "Pla"),
            (nameof(MapBuildingType.StatueSmall), "SSt"),
            (nameof(MapBuildingType.StatueMedium), "MSt"),
            (nameof(MapBuildingType.StatueLarge), "LSt"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.PalaceVillage), "PVi"),
            (nameof(MapBuildingType.PalaceTown), "PTo"),
            (nameof(MapBuildingType.PalaceCity), "PCi"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.MansionPersonal), "MPe"),
            (nameof(MapBuildingType.MansionFamily), "MFa"),
            (nameof(MapBuildingType.MansionDynasty), "MDy"),
        };

        result[btnMilitary] = new[]
        {
            (nameof(MapBuildingType.Wall), "Wal"),
            (nameof(MapBuildingType.Tower), "Twr"),
            (nameof(MapBuildingType.Gate1), "Gt1"),
            (nameof(MapBuildingType.Gate2), "Gt2"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Recruiter), "Rec"),
            (nameof(MapBuildingType.Academy), "Aca"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Weaponsmith), "Wpn"),
            (nameof(MapBuildingType.Chariot), "Cha"),
            (string.Empty, string.Empty),
            (nameof(MapBuildingType.Fort), "Frt"),
            (nameof(MapBuildingType.Warship), "WSh"),
            (nameof(MapBuildingType.TransportShip), "TSh"),
        };

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
    }
}