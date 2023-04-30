using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace CityPlanner;

public partial class MapCanvasControl : Control
{
    public const int CellSideLength = 32;
    public const int BorderWidth = 1;
    public const int BorderWidthDouble = 2 * BorderWidth;

    private readonly Pen borderPen;
    private readonly Brush textBrush;
    private readonly Font smallFont;
    private readonly Brush[] terrainBrushes;
    private readonly Brush[] buildingBrushes;

    public MapCanvasControl()
    {
        InitializeComponent();

        this.DoubleBuffered = true;

        this.borderPen = new Pen(Color.Black, BorderWidth);
        this.textBrush = new SolidBrush(Color.Black);
        this.smallFont = new Font("Bahnschrift Condensed", 8.25F, FontStyle.Regular, GraphicsUnit.Point);

        this.terrainBrushes = new Brush[]
        {
            new SolidBrush(Color.FromArgb(82, 138, 33)), // Grass,
            new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(255, 216, 0), Color.FromArgb(82, 138, 33)), // GrassFarmland,
            new SolidBrush(Color.FromArgb(231, 203, 148)), // Sand,
            new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(153, 127, 0), Color.FromArgb(231, 203, 148)), // SandFarmland,
            new SolidBrush(Color.FromArgb(66, 48, 49)), // Rock,
            new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(255, 216, 0), Color.FromArgb(66, 48, 49)), // RockOre,
            new SolidBrush(Color.FromArgb(160, 140, 104)), // Dune,
            new SolidBrush(Color.FromArgb(96, 40, 28)), // Floodpain,
            new SolidBrush(Color.FromArgb(94, 75, 71)), // FloodpainEdge,
            new SolidBrush(Color.FromArgb(33, 81, 82)), // Water,
            new SolidBrush(Color.FromArgb(62, 78, 79)), // WaterEdge,
        };

        this.buildingBrushes = new Brush[]
        {
            new SolidBrush(Color.FromArgb(231, 195, 156)), // Path,
            new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(255, 216, 0), Color.FromArgb(231, 195, 156)), // Plaza,
            new SolidBrush(Color.FromArgb(255, 233, 127)), // House,
            new SolidBrush(Color.FromArgb(255, 106, 0)), // Commercial,
        };

        // mostly for the designer preview
        this.MapModel = new MapModel(this.DesignMode ? 20 : 0);
        this.Tool = new Tool();
    }

    public MapModel MapModel { get; set; }

    public Tool Tool { get; set; }

    public bool ShowCellCoords { get; set; }

    public void SetSizeToFullMapSize()
    {
        this.Size = new Size(this.MapModel.MapSideLength * CellSideLength, this.MapModel.MapSideLength * CellSideLength);
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        var graphics = pe.Graphics;

        var buildingToPaint = new List<MapBuilding>();

        for (int cellX = 0; cellX < this.MapModel.MapSideLength; cellX++)
        {
            for (int cellY = 0; cellY < this.MapModel.MapSideLength; cellY++)
            {
                var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                if (!cellRect.IntersectsWith(pe.ClipRectangle))
                    continue;

                var cellModel = this.MapModel.Cells[cellX, cellY];

                if (cellModel.Building != null)
                    buildingToPaint.Add(cellModel.Building);
                else
                    PaintTerrainCell(graphics, cellX, cellY, cellRect, cellModel);
            }
        }

        foreach (MapBuilding building in buildingToPaint)
        {
            var buildingRect = GetBuildingRectangle(building);
            
            var buildingCategory = building.BuildingType.GetCategory();
            var brush = this.buildingBrushes[(int)buildingCategory];

            // draw border
            var borderRect = new Rectangle(
                buildingRect.Left, buildingRect.Top,
                buildingRect.Width - BorderWidth, buildingRect.Height - BorderWidth);
            graphics.DrawRectangle(borderPen, borderRect);

            // draw cell insides
            var innerRect = new Rectangle(
                buildingRect.Left + BorderWidth, buildingRect.Top + BorderWidth,
                buildingRect.Width - BorderWidthDouble, buildingRect.Height - BorderWidthDouble);
            graphics.FillRectangle(brush, innerRect);

            // draw building name
            if (building.BuildingType.ShowName())
            {
                string text = building.BuildingType.ToString();
                var textSize = graphics.MeasureString(text, this.smallFont);
                graphics.DrawString(
                    text, this.smallFont, this.textBrush,
                    buildingRect.Left + buildingRect.Width / 2 - textSize.Width / 2,
                    buildingRect.Top + buildingRect.Height / 2 - textSize.Height / 2);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        if (Tool.SupportsDrag)
        {
            this.Capture = true;
            Debug.WriteLine("start drag");
        }

        ApplyTool(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        this.Capture = false;
        Debug.WriteLine("OnMouseUp");
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (Tool.BuildingType != null)
        {
            // TODO: show ghost
        }

        if (this.Capture)
            ApplyTool(e);
    }

    private void ApplyTool(MouseEventArgs e)
    {
        var clientLocation = e.Location;
        var cellX = clientLocation.X / CellSideLength;
        var cellY = clientLocation.Y / CellSideLength;

        if (cellX >= this.MapModel.MapSideLength || cellY >= this.MapModel.MapSideLength)
            return;

        var cellModel = this.MapModel.Cells[cellX, cellY];
        if (Tool.Terrain != null)
        {
            if (cellModel.Terrain == Tool.Terrain.Value)
            {
                // no change needed
                return;
            }

            cellModel.Terrain = Tool.Terrain.Value;

            if (cellModel.Building == null)
            {
                var graphics = this.CreateGraphics();
                var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                PaintTerrainCell(graphics, cellX, cellY, cellRect, cellModel);
            }
        }
        else if (Tool.BuildingType != null)
        {
            var building = this.MapModel.AddBuilding(cellX, cellY, Tool.BuildingType.Value);
            if (building != null)
            {
                var buildingRect = GetBuildingRectangle(building);
                Invalidate(buildingRect);
            }
        }
        else if (Tool.IsClearBuilding)
        {
            var building = cellModel.Building;
            if (building == null)
                return;

            this.MapModel.RemoveBuilding(building);
            var buildingRect = GetBuildingRectangle(building);
            Invalidate(buildingRect);
        }
        else
        {
            return;
        }
    }

    private static Rectangle GetBuildingRectangle(MapBuilding building)
    {
        var size = building.BuildingType.GetSize();
        return new Rectangle(
            building.Left * CellSideLength,
            building.Top * CellSideLength,
            size.width * CellSideLength,
            size.height * CellSideLength);
    }

    private void PaintTerrainCell(Graphics graphics, int cellX, int cellY, Rectangle cellRect, MapCellModel cellModel)
    {
        // draw cell border
        var borderRect = new Rectangle(
            cellRect.Left, cellRect.Top,
            cellRect.Width - BorderWidth, cellRect.Height - BorderWidth);
        graphics.DrawRectangle(borderPen, borderRect);

        // draw cell insides
        var innerRect = new Rectangle(
            cellRect.Left + BorderWidth, cellRect.Top + BorderWidth,
            cellRect.Width - BorderWidthDouble, cellRect.Height - BorderWidthDouble);
        var fillBrush = this.terrainBrushes[(int)cellModel.Terrain];
        graphics.FillRectangle(fillBrush, innerRect);

        // draw cell number (mostly for debugging)
        if (this.ShowCellCoords)
            graphics.DrawString($"{cellX},{cellY}", this.smallFont, this.textBrush, cellRect.Left + 2, cellRect.Top + 2);
    }
}
