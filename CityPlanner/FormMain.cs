using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityPlanner;

public partial class FormMain : Form
{

    private readonly Dictionary<ToolStripButton, ToolStrip> buttonToSecondaryToolbar;
    private readonly Dictionary<ToolStripButton, MapTerrain> buttonToTerrainTool;
    private readonly Dictionary<ToolStripButton, MapBuildingType> buttonToBuildingTool;

    private string? fileName;

    public FormMain()
    {
        InitializeComponent();

        var mapModel = new MapModel(MapModel.DefaultMapSize, MapModel.DefaultMapSize);
        this.mapControl.MapModel = mapModel;
        this.mapControl.SetSizeToFullMapSize();

        this.buttonToSecondaryToolbar = new Dictionary<ToolStripButton, ToolStrip>
        {
            { btnTerrain, toolStripTerrain },
            { btnFood, toolStripFood },
            { btnIndustry, toolStripIndustry },
            { btnDistribution, toolStripDist },
            { btnEnt, toolStripEnt },
            { btnReligious, toolStripReligious },
            { btnEducation, toolStripEducation },
            { btnHealth, toolStripHealth },
            { btnMunicipal, toolStripMunicipal },
            { btnMilitary, toolStripMilitary },
        };
        foreach (var button in this.buttonToSecondaryToolbar.Keys)
        {
            button.Click += this.MainToolbarSubbarButtonClick;
        }

        this.ShowHideToolbars(null);

        this.buttonToBuildingTool = new Dictionary<ToolStripButton, MapBuildingType>
        {
            // main
            { btnRoad, MapBuildingType.Road },
            { btnPlaza, MapBuildingType.Plaza },
            { btnHouse, MapBuildingType.House },
            // food
            { btnFarm, MapBuildingType.Farm },
            { btnCattle, MapBuildingType.Cattle },
            { btnWaterLift, MapBuildingType.WaterLift },
            { btnDitch, MapBuildingType.Ditch },
            { btnHunter, MapBuildingType.Hunter },
            { btnFisher, MapBuildingType.Fisher },
            { btnWorkCamp, MapBuildingType.WorkCamp },
            // querry&mine
            { btnPlainStone, MapBuildingType.QuarryPlainStone },
            { btnLimeStone, MapBuildingType.QuarryLimestone },
            { btnSandStone, MapBuildingType.QuarrySandstone },
            { btnGranite, MapBuildingType.QuarryGranite },
            { btnGemstone, MapBuildingType.MineGems },
            { btnCopper, MapBuildingType.MineCopper },
            { btnGold, MapBuildingType.MineGold },
            // other raw materials
            { btnClay, MapBuildingType.Clay },
            { btnReed, MapBuildingType.Reed },
            { btnWood, MapBuildingType.Wood },
            // workshops
            { btnPotter, MapBuildingType.Potter },
            { btnBrewery, MapBuildingType.Brewer },
            { btnPapyrus, MapBuildingType.Papyrus },
            { btnWeaver, MapBuildingType.Weaver },
            { btnJeweler, MapBuildingType.Jeweler },
            { btnBricks, MapBuildingType.Bricks },
            { btnLamp, MapBuildingType.Lamps },
            { btnPaint, MapBuildingType.Paint },
            { btnShipwright, MapBuildingType.Shipwright },
            // guilds
            { btnBricklayers, MapBuildingType.GuildBricklayer },
            { btnCarpernters, MapBuildingType.GuildCarpenter },
            { btnStonemasons, MapBuildingType.GuildStonemason },
            { btnArtisans, MapBuildingType.GuildArtisan },
            // distribution
            { btnBazaar, MapBuildingType.Bazaar },
            { btnGranary, MapBuildingType.Granary },
            { btnStorageYard, MapBuildingType.StorageYard },
            { btnDock, MapBuildingType.Dock },
            // entertainment venues
            { btnBooth1, MapBuildingType.Booth1 },
            { btnBooth2, MapBuildingType.Booth2 },
            { btnBooth3, MapBuildingType.Booth3 },
            { btnBooth4, MapBuildingType.Booth4 },
            { btnBandstand1, MapBuildingType.Bandstand1 },
            { btnBandstand2, MapBuildingType.Bandstand2 },
            { btnBandstand3, MapBuildingType.Bandstand3 },
            { btnBandstand4, MapBuildingType.Bandstand4 },
            { btnPavilion1, MapBuildingType.Pavilion1 },
            { btnPavilion2, MapBuildingType.Pavilion2 },
            { btnPavilion3, MapBuildingType.Pavilion3 },
            { btnPavilion4, MapBuildingType.Pavilion4 },
            { btnPavilion5, MapBuildingType.Pavilion5 },
            { btnPavilion6, MapBuildingType.Pavilion6 },
            { btnPavilion7, MapBuildingType.Pavilion7 },
            { btnPavilion8, MapBuildingType.Pavilion8 },
            { btnPavilion9, MapBuildingType.Pavilion9 },
            { btnPavilion10, MapBuildingType.Pavilion10 },
            { btnPavilion11, MapBuildingType.Pavilion11 },
            { btnSenet, MapBuildingType.Senet },
            { btnZoo, MapBuildingType.Zoo },
            { btnJugglerSchool, MapBuildingType.JuggleSchool },
            { btnMusicSchool, MapBuildingType.MusicSchool },
            { btnDanceSchool, MapBuildingType.DanceSchool },
            // religious
            { btnShrine, MapBuildingType.Shrine },
            { btnTemple, MapBuildingType.Temple },
            { btnTempleComplex1, MapBuildingType.TempleComplex1 },
            { btnTempleComplex2, MapBuildingType.TempleComplex2 },
            { btnFestivalSquare, MapBuildingType.FestivalSquare },
        };
        foreach (var button in this.buttonToBuildingTool.Keys)
        {
            if (button.GetCurrentParent() == toolStripMain)
            {
                button.Click += this.MainToolbarToolButtonClick;
            }
            else
            {
                button.Click += this.SecondaryToolbarButtonClick;
            }
        }

        this.buttonToTerrainTool = new Dictionary<ToolStripButton, MapTerrain>
        {
            { btnTerrainGrass, MapTerrain.Grass },
            { btnTerrainGrassFamrland, MapTerrain.GrassFarmland },
            { btnTerrainSand, MapTerrain.Sand },
            { btnTerrainSandFamrland, MapTerrain.SandFarmland },
            { btnTerrainRock, MapTerrain.Rock },
            { btnTerrainRockOre, MapTerrain.RockOre },
            { btnTerrainDune, MapTerrain.Dune },
            { btnTerrainFlood, MapTerrain.Floodpain },
            { btnTerrainFloodEdge, MapTerrain.FloodpainEdge },
            { btnTerrainWater, MapTerrain.Water },
            { btnTerrainWaterEdge, MapTerrain.WaterEdge },
        };
        foreach (var button in this.buttonToTerrainTool.Keys)
        {
            button.Click += this.SecondaryToolbarButtonClick;
        }

        this.mapControl.ShowBuildings = btnFilterBuildings.Checked;
        this.mapControl.ShowDesirability = btnFilterDesire.Checked;
    }

    private void MainToolbarSubbarButtonClick(object sender, EventArgs e)
    {
        var currentButton = (ToolStripButton)sender;

        UntickOtherToolbarButtons(toolStripMain, currentButton);

        var toolBarToShow = this.buttonToSecondaryToolbar[currentButton];

        ShowHideToolbars(toolBarToShow);
        this.mapControl.Tool = new Tool();
    }

    private void MainToolbarToolButtonClick(object sender, EventArgs e)
    {
        var currentButton = (ToolStripButton)sender;

        UntickOtherToolbarButtons(toolStripMain, currentButton);
        ShowHideToolbars(null);

        if (currentButton == btnSelect)
        {
            this.mapControl.Tool = new Tool();
        }
        else if (currentButton == btnClear)
        {
            this.mapControl.Tool = new Tool { IsClearBuilding = true };
        }
        else if (this.buttonToBuildingTool.TryGetValue(currentButton, out var buildingType))
        {
            this.mapControl.Tool = new Tool { BuildingType = buildingType };
        }
    }

    private void SecondaryToolbarButtonClick(object sender, EventArgs e)
    {
        var currentButton = (ToolStripButton)sender;
        var toolStrip = currentButton.GetCurrentParent();

        UntickOtherToolbarButtons(toolStrip, currentButton);

        if (this.buttonToTerrainTool.TryGetValue(currentButton, out var terrain))
        {
            this.mapControl.Tool = new Tool { Terrain = terrain };
        }
        else if (this.buttonToBuildingTool.TryGetValue(currentButton, out var buildingType))
        {
            this.mapControl.Tool = new Tool { BuildingType = buildingType };
        }
    }

    private void ShowHideToolbars(ToolStrip? toolStripToShow)
    {
        foreach (var toolbarStrip in this.buttonToSecondaryToolbar.Values)
        {
            toolbarStrip.Visible = toolbarStrip == toolStripToShow;
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
    }
}