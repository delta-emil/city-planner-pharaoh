namespace CityPlanner;

public partial class FormMain : Form
{
    private readonly MapModel mapModel;
    private readonly Dictionary<ToolStripButton, ToolStrip> buttonToSecondaryToolbar;
    private readonly Dictionary<ToolStripButton, MapTerrain> buttonToTerrainTool;
    private readonly Dictionary<ToolStripButton, MapBuildingType> buttonToBuildingTool;

    public FormMain()
    {
        InitializeComponent();

        this.mapModel = new MapModel(200);
        this.mapControl.MapModel = mapModel;
        this.mapControl.SetSizeToFullMapSize();

        this.buttonToSecondaryToolbar = new Dictionary<ToolStripButton, ToolStrip>
        {
            { btnTerrain, toolStripTerrain },
            { btnCommercial, toolStripCommercial },
        };
        foreach (var button in this.buttonToSecondaryToolbar.Keys)
        {
            button.Click += this.MainToolbarSubbarButtonClick;
        }

        this.ShowHideToolbars(null);

        this.buttonToBuildingTool = new Dictionary<ToolStripButton, MapBuildingType>
        {
            { btnRoad, MapBuildingType.Road },
            { btnPlaza, MapBuildingType.Plaza },
            { btnHouse, MapBuildingType.House },
            { btnBazaar, MapBuildingType.Bazaar },
        };
        foreach (var button in this.buttonToBuildingTool.Keys)
        {
            if (button.GetCurrentParent() == toolStripMain)
                button.Click += this.MainToolbarToolButtonClick;
            else
                button.Click += this.SecondaryToolbarButtonClick;
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
}