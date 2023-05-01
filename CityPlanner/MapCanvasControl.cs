using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CityPlanner;

public partial class MapCanvasControl : Control
{
    public const int CellSideLength = 32;
    public const int BorderWidth = 1;
    public const int BorderWidthDouble = 2 * BorderWidth;

    private readonly Pen borderSoftPen;
    private readonly Pen borderBuildingPen;
    private readonly Pen borderBuildingSelectedPen;
    private readonly Brush textBrush;
    private readonly Brush ghostValidBrush;
    private readonly Brush ghostInvalidBrush;
    private readonly Font smallFont;
    private readonly Brush[] terrainBrushes;
    private readonly Brush[] buildingBrushes;

    private readonly HashSet<MapBuilding> selectedBuildings;
    private Rectangle? ghostLocation;
    private (int x, int y) selectionDragStartCell;
    private (int x, int y) selectionDragEndCell;
    private bool? moveValid;

    public MapCanvasControl()
    {
        InitializeComponent();

        this.DoubleBuffered = true;

        this.borderSoftPen = new Pen(Color.Gray, BorderWidth);
        this.borderBuildingPen = new Pen(Color.Black, BorderWidth);
        this.borderBuildingSelectedPen = new Pen(Color.LightBlue, BorderWidth);
        this.textBrush = new SolidBrush(Color.Black);
        this.smallFont = new Font("Bahnschrift Condensed", 8.25F, FontStyle.Regular, GraphicsUnit.Point);

        this.ghostValidBrush = new SolidBrush(Color.FromArgb(96, 0, 255, 0));
        this.ghostInvalidBrush = new SolidBrush(Color.FromArgb(96, 255, 0, 0));

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
            //new SolidBrush(Color.FromArgb(231, 195, 156)), // Path,
            new SolidBrush(Color.FromArgb(191, 181, 166)), // Path,
            new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(216, 132, 255), Color.FromArgb(191, 181, 166)), // Plaza,
            new SolidBrush(Color.FromArgb(255, 233, 127)), // House,
            new SolidBrush(Color.FromArgb(255, 106, 0)), // Commercial,
        };

        // mostly for the designer preview
        this.MapModel = new MapModel(this.DesignMode ? MapModel.DefaultMapSize : 0, this.DesignMode ? MapModel.DefaultMapSize : 0);
        this.tool = new Tool();
        this.selectedBuildings = new();
    }

    public MapModel MapModel { get; set; }

    private Tool tool;
    public Tool Tool
    {
        get => tool;
        set
        {
            tool = value;
            ClearGhost();
        }
    }

    public bool ShowBuildings { get; set; }

    public bool ShowDesirability { get; set; }

    public bool ShowCellCoords { get; set; }

    public void SetSizeToFullMapSize()
    {
        this.Size = new Size(this.MapModel.MapSideX * CellSideLength, this.MapModel.MapSideY * CellSideLength);
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        var graphics = pe.Graphics;

        var buildingToPaint = new List<MapBuilding>();

        for (int cellX = 0; cellX < this.MapModel.MapSideX; cellX++)
        {
            for (int cellY = 0; cellY < this.MapModel.MapSideY; cellY++)
            {
                var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                if (!cellRect.IntersectsWith(pe.ClipRectangle))
                {
                    continue;
                }

                var cellModel = this.MapModel.Cells[cellX, cellY];

                if (cellModel.Building != null && this.ShowBuildings)
                {
                    buildingToPaint.Add(cellModel.Building);
                }
                else
                {
                    PaintTerrainCell(graphics, cellX, cellY, cellRect, cellModel);
                }
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
            var borderPen
                = this.selectedBuildings.Contains(building) ? this.borderBuildingSelectedPen
                : building.BuildingType.HasSoftBorder()     ? this.borderSoftPen
                : this.borderBuildingPen;
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
        {
            return;
        }

        if (this.Tool.SupportsDrag)
        {
            this.Capture = true;
        }

        this.ApplyTool(e, isMove: false);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        this.Capture = false;

        if (this.moveValid != null)
        {
            if (this.moveValid.Value)
            {
                var offsetX = this.selectionDragEndCell.x - this.selectionDragStartCell.x;
                var offsetY = this.selectionDragEndCell.y - this.selectionDragStartCell.y;
                this.MapModel.MoveBuildingsByOffset(this.selectedBuildings, offsetX, offsetY);
            }

            this.ghostLocation = null;
            this.moveValid = null;
            this.Invalidate();
        }
        else
        {
            // finish single click selection here, so we can distinguish it from drag-move
            ApplySingleClickSelection(e);
        }
    }

    private void ClearGhost(Graphics? existingGraphics = null)
    {
        if (this.ghostLocation == null)
        {
            return;
        }

        bool disposeGraphics = existingGraphics == null;
        var graphics = existingGraphics ?? this.CreateGraphics();
        try
        {
            this.InvokePaint(this, new PaintEventArgs(graphics, this.ghostLocation.Value));
        }
        finally
        {
            this.ghostLocation = null;
            if (disposeGraphics)
            {
                graphics.Dispose();
            }
        }
    }

    private void ShowGhostOnCells(Rectangle newGhostRect, bool isValid, Graphics graphics, bool append = false)
    {
        var fillBrush = isValid ? this.ghostValidBrush : this.ghostInvalidBrush;
        graphics.FillRectangle(fillBrush, newGhostRect);

        if (append && this.ghostLocation != null)
        {
            this.ghostLocation = Rectangle.Union(this.ghostLocation.Value, newGhostRect);
        }
        else
        {
            this.ghostLocation = newGhostRect;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (this.Tool.BuildingType != null)
        {
            var buildingType = this.Tool.BuildingType.Value;

            var (cellX, cellY) = GetCellCoordidates(e);
            var (width, height) = buildingType.GetSize();

            var newGhostRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, width * CellSideLength, height * CellSideLength);
            if (this.ghostLocation == null || !this.ghostLocation.Equals(newGhostRect))
            {
                using var graphics = this.CreateGraphics();
                ClearGhost(graphics);
                var isValid = this.MapModel.CanAddBuilding(cellX, cellY, buildingType);
                ShowGhostOnCells(newGhostRect, isValid, graphics);
            }
        }

        if (this.Capture)
        {
            this.ApplyTool(e, isMove: true);
        }
    }

    private void ApplyTool(MouseEventArgs e, bool isMove)
    {
        var (cellX, cellY) = GetCellCoordidates(e);
        if (!AreCellCoordidatesValid(cellX, cellY))
        {
            return;
        }

        var cellModel = this.MapModel.Cells[cellX, cellY];
        if (this.Tool.IsEmpty)
        {
            ApplySelectMove(isMove, cellX, cellY, cellModel);
        }
        else if (this.Tool.Terrain != null)
        {
            if (cellModel.Terrain == this.Tool.Terrain.Value)
            {
                // no change needed
                return;
            }

            this.MapModel.IsChanged = true;
            cellModel.Terrain = this.Tool.Terrain.Value;

            if (cellModel.Building == null)
            {
                using var graphics = this.CreateGraphics();
                var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                PaintTerrainCell(graphics, cellX, cellY, cellRect, cellModel);
            }
        }
        else if (this.Tool.BuildingType != null)
        {
            var building = this.MapModel.AddBuilding(cellX, cellY, this.Tool.BuildingType.Value);
            if (building != null)
            {
                this.MapModel.IsChanged = true;
                var buildingRect = GetBuildingRectangle(building);
                Invalidate(buildingRect);
            }
        }
        else if (this.Tool.IsClearBuilding)
        {
            var building = cellModel.Building;
            if (building == null)
            {
                return;
            }

            this.MapModel.IsChanged = true;
            this.MapModel.RemoveBuilding(building);
            var buildingRect = GetBuildingRectangle(building);
            Invalidate(buildingRect);
        }
        else
        {
            return;
        }
    }

    private void ApplySelectMove(bool isMouseMove, int cellX, int cellY, MapCellModel cellModel)
    {
        bool shiftHeld = ModifierKeys == Keys.Shift;
        bool invalidate = false;
        Rectangle? invalidateRect = null;

        if (!isMouseMove)
        {
            this.selectionDragStartCell = this.selectionDragEndCell = (cellX, cellY);
        }

        if (shiftHeld)
        {
            bool selectionChanged;
            if (!isMouseMove)
            {
                selectionChanged = true;
            }
            else
            {
                var newEndCell = (cellX, cellY);
                selectionChanged = this.selectionDragEndCell != newEndCell;
                this.selectionDragEndCell = newEndCell;
            }

            if (selectionChanged)
            {
                if (this.selectedBuildings.Count > 0)
                {
                    this.selectedBuildings.Clear();
                    invalidate = true;
                }

                var buildings = this.MapModel.GetAllBuildingsInRectangle(this.selectionDragStartCell, this.selectionDragEndCell);
                if (buildings.Count > 0)
                {
                    this.selectedBuildings.UnionWith(buildings);
                    invalidate = true;
                }
            }
        }
        else if (isMouseMove)
        {
            // TODO: how do we move when it's a mutliple selection? the initial mouse-down reduces our selection to the clicked building
            // attempting to move stuff
            if (this.selectedBuildings.Count > 0)
            {
                var newEndCell = (cellX, cellY);
                if (this.selectionDragEndCell != newEndCell)
                {
                    // if haven't started moving yet, check if we're starting from a selected building -> only then can we drag-move
                    if (this.moveValid == null)
                    {
                        var startCell = this.MapModel.Cells[this.selectionDragStartCell.x, this.selectionDragStartCell.y];
                        if (startCell.Building == null || !this.selectedBuildings.Contains(startCell.Building))
                        {
                            return;
                        }
                    }

                    this.selectionDragEndCell = newEndCell;
                    var offsetX = this.selectionDragEndCell.x - this.selectionDragStartCell.x;
                    var offsetY = this.selectionDragEndCell.y - this.selectionDragStartCell.y;

                    using var graphics = this.CreateGraphics();
                    ClearGhost(graphics);

                    if (offsetX != 0 || offsetY != 0)
                    {
                        this.moveValid = true;
                        foreach (var building in this.selectedBuildings)
                        {
                            var newCellX = building.Left + offsetX;
                            var newCellY = building.Top + offsetY;

                            var isValid = this.MapModel.CanAddBuilding(newCellX, newCellY, building.BuildingType, selectedBuildings);
                            this.moveValid &= isValid;

                            var (width, height) = building.BuildingType.GetSize();
                            var newGhostRect = new Rectangle(newCellX * CellSideLength, newCellY * CellSideLength, width * CellSideLength, height * CellSideLength);
                            ShowGhostOnCells(newGhostRect, isValid, graphics, append: true);
                        }
                    }
                    else
                    {
                        this.moveValid = false;
                    }
                }
            }
        }
        else
        {
            bool controlHeld = ModifierKeys == Keys.Control;
            var buildingOnCell = cellModel.Building;

            if (controlHeld)
            {
                if (buildingOnCell != null)
                {
                    if (this.selectedBuildings.Contains(buildingOnCell))
                    {
                        this.selectedBuildings.Remove(buildingOnCell);
                    }
                    else
                    {
                        this.selectedBuildings.Add(buildingOnCell);
                    }

                    invalidateRect = GetBuildingRectangle(buildingOnCell);
                    invalidate = true;
                }
            }
            else
            {
                // will be done in mouse-up
            }
        }

        if (invalidate)
        {
            if (invalidateRect != null)
            {
                this.Invalidate(invalidateRect.Value);
            }
            else
            {
                this.Invalidate();
            }
        }
    }

    private void ApplySingleClickSelection(MouseEventArgs e)
    {
        if (!this.Tool.IsEmpty
            || ModifierKeys == Keys.Shift
            || ModifierKeys == Keys.Control)
        {
            return;
        }
        
        bool invalidate = false;
        Rectangle? invalidateRect = null;

        var (cellX, cellY) = GetCellCoordidates(e);
        if (!AreCellCoordidatesValid(cellX, cellY))
        {
            return;
        }

        if (this.selectionDragStartCell != (cellX, cellY))
        {
            return;
        }

        var cellModel = this.MapModel.Cells[cellX, cellY];
        var buildingOnCell = cellModel.Building;
        if (buildingOnCell == null)
        {
            if (this.selectedBuildings.Count > 0)
            {
                this.selectedBuildings.Clear();
                invalidate = true;
            }
        }
        else if (this.selectedBuildings.Count == 0)
        {
            this.selectedBuildings.Add(buildingOnCell);
            invalidateRect = GetBuildingRectangle(buildingOnCell);
            invalidate = true;
        }
        else
        {
            this.selectedBuildings.Clear();
            this.selectedBuildings.Add(buildingOnCell);
            invalidate = true;
        }

        if (invalidate)
        {
            if (invalidateRect != null)
            {
                this.Invalidate(invalidateRect.Value);
            }
            else
            {
                this.Invalidate();
            }
        }
    }

    private static (int x, int y) GetCellCoordidates(MouseEventArgs e)
    {
        var clientLocation = e.Location;
        var cellX = clientLocation.X / CellSideLength;
        var cellY = clientLocation.Y / CellSideLength;
        return (cellX, cellY);
    }

    private bool AreCellCoordidatesValid(int cellX, int cellY)
    {
        if (cellX < 0 || cellY < 0 || cellX >= this.MapModel.MapSideX || cellY >= this.MapModel.MapSideY)
        {
            return false;
        }
        else
        {
            return true;
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
        graphics.DrawRectangle(borderSoftPen, borderRect);

        // draw cell insides
        var innerRect = new Rectangle(
            cellRect.Left + BorderWidth, cellRect.Top + BorderWidth,
            cellRect.Width - BorderWidthDouble, cellRect.Height - BorderWidthDouble);
        var fillBrush = this.terrainBrushes[(int)cellModel.Terrain];
        graphics.FillRectangle(fillBrush, innerRect);

        // draw cell number (mostly for debugging)
        if (this.ShowCellCoords)
        {
            graphics.DrawString($"{cellX},{cellY}", this.smallFont, this.textBrush, cellRect.Left + 2, cellRect.Top + 2);
        }
    }
}
