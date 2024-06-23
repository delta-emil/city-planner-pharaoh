using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CityPlannerPharaoh;

public class MapCanvasControl : Control
{
    public const int MinCellSideLength = 8;
    public const int MaxCellSideLength = 32;
    public const int BorderWidth = 1;
    public const int BorderWidthDouble = 2 * BorderWidth;

    private readonly Pen borderSoftPen;
    private readonly Pen borderBuildingPen;
    private readonly Pen borderBuildingSelectedPen;
    private readonly Brush textBrush;
    private readonly Brush ghostValidBrush;
    private readonly Brush ghostInvalidBrush;
    private readonly Font smallFont;
    private readonly Font smallFontBold;
    private readonly Font bigHouseFont;
    private readonly Brush[] terrainBrushes;
    private readonly Brush[] buildingBrushes;
    private readonly Brush farmMeadowBrush;
    private readonly Brush farmIrrigatedTextBrush;
    private readonly Brush tooCloseToVoidToBuildBrush;
    private readonly Brush[] desirablityBrushes;

    private int cellSideLength = 24;
    private readonly HashSet<MapBuilding> selectedBuildings;
    private readonly HashSet<MapBuilding> clipboardBuildings;
    private Rectangle? ghostLocation;
    private (int x, int y) selectionDragStartCell;
    private (int x, int y) selectionDragEndCell;
    private bool? moveValid;

    public MapCanvasControl()
    {
        this.DoubleBuffered = true;

        this.borderSoftPen = new Pen(Color.Gray, BorderWidth);
        this.borderBuildingPen = new Pen(Color.Black, BorderWidth);
        this.borderBuildingSelectedPen = new Pen(Color.LightBlue, BorderWidth);
        this.textBrush = new SolidBrush(Color.Black);
        this.smallFont = new Font("Bahnschrift Condensed", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        this.smallFontBold = new Font("Bahnschrift Condensed", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
        this.bigHouseFont = new Font("Bahnschrift", 9F, FontStyle.Regular, GraphicsUnit.Point);

        this.ghostValidBrush = new SolidBrush(Color.FromArgb(96, 0, 255, 0));
        this.ghostInvalidBrush = new SolidBrush(Color.FromArgb(96, 255, 0, 0));

        this.terrainBrushes = new Brush[]
        {
            new SolidBrush(Color.Black), // Void,
            new SolidBrush(Color.FromArgb(157, 198, 121)), // Grass,
            new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(255, 216, 0), Color.FromArgb(157, 198, 121)), // GrassFarmland,
            new SolidBrush(Color.FromArgb(224, 205, 170)), // Sand,
            new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(153, 127, 0), Color.FromArgb(224, 205, 170)), // SandFarmland,
            new SolidBrush(Color.FromArgb(66, 48, 49)), // Rock,
            new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(255, 216, 0), Color.FromArgb(66, 48, 49)), // RockOre,
            new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(66, 48, 49), Color.FromArgb(224, 205, 170)), // Dune,
            new SolidBrush(Color.FromArgb(96, 40, 28)), // Floodpain,
            new SolidBrush(Color.FromArgb(94, 75, 71)), // FloodpainEdge,
            new SolidBrush(Color.FromArgb(33, 81, 82)), // Water,
            new SolidBrush(Color.FromArgb(62, 78, 79)), // WaterEdge,
            new HatchBrush(HatchStyle.Wave, Color.Brown, Color.Green), // Trees,
            new HatchBrush(HatchStyle.Wave, Color.Blue, Color.Green), // Reeds,
        };

        this.buildingBrushes = new Brush[]
        {
            new SolidBrush(Color.FromArgb(191, 181, 166)), // Path,
            new HatchBrush(HatchStyle.HorizontalBrick, Color.FromArgb(158, 255, 33), Color.FromArgb(191, 181, 166)), // Plaza,
            new SolidBrush(Color.FromArgb(129, 212, 26)), // Beauty,
            new SolidBrush(Color.FromArgb(255, 255, 215)), // House,
            new SolidBrush(Color.FromArgb(255, 222, 89)), // Food,
            new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(255, 222, 89), Color.FromArgb(33, 81, 82)), // Ditch,
            new SolidBrush(Color.FromArgb(198, 118, 37)), // QuarryMine,
            new SolidBrush(Color.FromArgb(198, 118, 37)), // RawMaterials,
            new SolidBrush(Color.FromArgb(255, 151, 47)), // Workshop,
            new SolidBrush(Color.FromArgb(181, 162, 143)), // Guild,
            new SolidBrush(Color.FromArgb(255, 219, 182)), // Distribution,
            new SolidBrush(Color.FromArgb(114, 159, 207)), // VenueStage,
            new SolidBrush(Color.FromArgb(172, 189, 208)), // Venue,
            new SolidBrush(Color.FromArgb(50, 115, 181)), // EntSchool,
            new SolidBrush(Color.FromArgb(191, 129, 158)), // Religious,
            new SolidBrush(Color.FromArgb(212, 234, 107)), // Education,
            new SolidBrush(Color.FromArgb(170, 220, 247)), // Water,
            new SolidBrush(Color.FromArgb(247, 209, 213)), // Health,
            new SolidBrush(Color.FromArgb(129, 172, 166)), // Municipal,
            new HatchBrush(HatchStyle.DiagonalCross, Color.Red, Color.FromArgb(191, 181, 166)), // Roadblock,
            new HatchBrush(HatchStyle.HorizontalBrick, Color.FromArgb(33, 81, 82), Color.FromArgb(191, 181, 166)), // Bridge,
            new HatchBrush(HatchStyle.HorizontalBrick, Color.FromArgb(191, 181, 166), Color.FromArgb(129, 172, 166)), // Ferry,
            new SolidBrush(Color.FromArgb(206, 136, 136)), // Wall,
            new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(255, 79, 79), Color.FromArgb(191, 181, 166)), // GatePath,
            new SolidBrush(Color.FromArgb(255, 79, 79)), // Military,
        };

        this.farmMeadowBrush = new HatchBrush(HatchStyle.DashedVertical, Color.FromArgb(157, 198, 121), Color.FromArgb(255, 222, 89));
        this.farmIrrigatedTextBrush = new SolidBrush(Color.Teal);
        this.tooCloseToVoidToBuildBrush = new HatchBrush(HatchStyle.Percent70, Color.FromArgb(160, 0, 0, 0), Color.FromArgb(0, 0, 0, 0));
        this.desirablityBrushes = new Brush[110];

        // mostly for the designer preview
        this.cellSideLength = 24;
        this.MapModel = new MapModel(8, 8);
        this.tool = new Tool();
        this.selectedBuildings = new();
        this.clipboardBuildings = new();
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

    public int CellSideLength
    {
        get => this.cellSideLength;
        set
        {
            var boundedValue = Math.Max(MinCellSideLength, Math.Min(MaxCellSideLength, value));
            if (this.cellSideLength == boundedValue)
            {
                return;
            }
            this.cellSideLength = boundedValue;
            this.SetSizeToFullMapSize();
            this.Invalidate();
        }
    }

    public event Action<object, MapSelectionChangeEventArgs>? SelectionChanged;

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
            PaintBuilding(graphics, building);
        }
    }

    private void PaintBuilding(Graphics graphics, MapBuilding building)
    {
        var buildingRect = GetBuildingRectangle(building, includingDesire: false);

        var buildingCategory = building.BuildingType.GetCategory();
        var ignoreMainBuilding = building.BuildingType.IgnoreMainBuilding();
        if (!ignoreMainBuilding)
        {
            var brush = this.buildingBrushes[(int)buildingCategory];

            // draw cell insides
            var innerRect = new Rectangle(
                buildingRect.Left + BorderWidth, buildingRect.Top + BorderWidth,
                buildingRect.Width - BorderWidthDouble, buildingRect.Height - BorderWidthDouble);
            graphics.FillRectangle(brush, innerRect);
        }

        foreach (var subBuilding in building.GetSubBuildings())
        {
            PaintBuilding(graphics, subBuilding);
        }

        // draw border
        if (!ignoreMainBuilding || this.selectedBuildings.Contains(building))
        {
            var borderRect = new Rectangle(
                buildingRect.Left, buildingRect.Top,
                buildingRect.Width - BorderWidth, buildingRect.Height - BorderWidth);
            var borderPen
                = this.selectedBuildings.Contains(building) ? this.borderBuildingSelectedPen
                : building.BuildingType.HasSoftBorder() ? this.borderSoftPen
                : this.borderBuildingPen;
            graphics.DrawRectangle(borderPen, borderRect);
        }

        if (!ignoreMainBuilding)
        {
            bool isMeadowFarm = false;
            if (building.BuildingType == MapBuildingType.Farm)
            {
                var size = building.BuildingType.GetSize();

                for (int cellX = building.Left; cellX < building.Left + size.width; cellX++)
                {
                    for (int cellY = building.Top; cellY < building.Top + size.height; cellY++)
                    {
                        var cellModel = this.MapModel.Cells[cellX, cellY];
                        if (cellModel.Terrain is MapTerrain.GrassFarmland or MapTerrain.SandFarmland or MapTerrain.Floodpain)
                        {
                            var innerRect = new Rectangle(
                                cellX * CellSideLength + BorderWidth, cellY * CellSideLength + BorderWidth,
                                CellSideLength - BorderWidthDouble, CellSideLength - BorderWidthDouble);
                            graphics.FillRectangle(this.farmMeadowBrush, innerRect);

                            isMeadowFarm |= cellModel.Terrain is MapTerrain.GrassFarmland or MapTerrain.SandFarmland;
                        }
                    }
                }
            }

            // draw building name
            if (building.BuildingType.ShowName())
            {
                string text = building.BuildingType.GetDisplayString();
                var textSize = graphics.MeasureString(text, this.smallFont);

                var textBrushToUse = this.textBrush;
                if (isMeadowFarm && this.MapModel.IsFarmIrrigated(building))
                {
                    textBrushToUse = this.farmIrrigatedTextBrush;
                }

                graphics.DrawString(
                    text, this.smallFont, textBrushToUse,
                    buildingRect.Left + buildingRect.Width / 2 - textSize.Width / 2,
                    buildingRect.Top + buildingRect.Height / 2 - textSize.Height / 2);
            }

            if (buildingCategory == MapBuildingCategory.House)
            {
                var size = building.BuildingType.GetSize();

                // draw desire
                DrawDesireabilityOnBuilding(graphics, building, size);

                // draw max house level
                string text = "H" + building.HouseLevel;
                var houseLabelFont = size.width > 1 ? this.bigHouseFont : this.smallFont;
                var textSize = graphics.MeasureString(text, houseLabelFont);

                int positionShift
                    = size.width switch
                    {
                        1 => 3,
                        3 => 9,
                        _ => -3
                    };

                graphics.DrawString(
                    text, houseLabelFont, this.textBrush,
                    buildingRect.Left + buildingRect.Width / 2 - textSize.Width / 2 + positionShift,
                    buildingRect.Top + buildingRect.Height / 2 - textSize.Height / 2 + positionShift);
            }

            if (building.BuildingType is MapBuildingType.Bazaar or MapBuildingType.WaterSupply)
            {
                DrawDesireabilityOnBuilding(graphics, building);
            }
        }
    }

    private void DrawDesireabilityOnBuilding(Graphics graphics, MapBuilding building, (int width, int height)? preCalculatedSize = null)
    {
        if (this.ShowDesirability)
        {
            var size = preCalculatedSize ?? building.BuildingType.GetSize();
            int maxDesire = this.MapModel.GetBuildingMaxDesirability(building);
            for (int cellX = building.Left; cellX < building.Left + size.width; cellX++)
            {
                for (int cellY = building.Top; cellY < building.Top + size.height; cellY++)
                {
                    var desire = this.MapModel.Cells[cellX, cellY].Desirability;
                    var font = desire == maxDesire ? this.smallFontBold : this.smallFont;
                    var brush = this.GetDesirabilityBrush(desire);
                    graphics.DrawString(desire.ToString(), font, brush, cellX * CellSideLength + 2, cellY * CellSideLength + 2);
                }
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // looks like this is not needed (?)
            if (this.Tool.SupportsDrag)
            {
                this.Capture = true;
            }

            this.ApplyTool(e, isMove: false);
        }
        else if (e.Button == MouseButtons.Right)
        {
            var (cellX, cellY) = GetCellCoordidates(e);
            this.BuildingsPaste(cellX, cellY);
        }
        else if (e.Button == MouseButtons.Middle)
        {
            var (cellX, cellY) = GetCellCoordidates(e);
            var mapBuilding = this.MapModel.Cells[cellX, cellY].Building;
            if (mapBuilding != null)
            {
                this.Tool = new Tool { BuildingType = mapBuilding.BuildingType };
            }
        }
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
                this.MapModel.IsChanged = true;
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
                var buildingRect = GetBuildingRectangle(building, includingDesire: this.ShowDesirability);
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
            var buildingRect = GetBuildingRectangle(building, includingDesire: this.ShowDesirability);
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

                this.OnSelectionChanged();
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
                    // TODO: keep the same buffer until a map with a different size is loaded
                    using BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(graphics, Rectangle.Round(this.ClientRectangle));
                    var screenRect = this.RectangleToScreen(this.ClientRectangle);
                    bufferedGraphics.Graphics.CopyFromScreen(screenRect.Left, screenRect.Top, 0, 0, screenRect.Size);
                    ClearGhost(bufferedGraphics.Graphics);

                    if (offsetX != 0 || offsetY != 0)
                    {
                        this.moveValid = true;
                        foreach (var building in this.selectedBuildings)
                        {
                            var newCellX = building.Left + offsetX;
                            var newCellY = building.Top + offsetY;

                            var isValid = this.MapModel.CanAddBuilding(newCellX, newCellY, building.BuildingType, selectedBuildings);
                            this.moveValid &= isValid;

                            if (building.BuildingType.IgnoreMainBuilding())
                            {
                                foreach (var subBuilding in building.GetSubBuildings())
                                {
                                    var (width, height) = subBuilding.BuildingType.GetSize();
                                    var newGhostRect = new Rectangle(
                                        (subBuilding.Left + offsetX) * CellSideLength,
                                        (subBuilding.Top + offsetY) * CellSideLength,
                                        width * CellSideLength,
                                        height * CellSideLength);
                                    ShowGhostOnCells(newGhostRect, isValid, bufferedGraphics.Graphics, append: true);
                                }
                            }
                            else
                            {
                                var (width, height) = building.BuildingType.GetSize();
                                var newGhostRect = new Rectangle(newCellX * CellSideLength, newCellY * CellSideLength, width * CellSideLength, height * CellSideLength);
                                ShowGhostOnCells(newGhostRect, isValid, bufferedGraphics.Graphics, append: true);
                            }
                        }
                    }
                    else
                    {
                        this.moveValid = false;
                    }

                    bufferedGraphics.Render(graphics);
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

                    this.OnSelectionChanged();

                    invalidateRect = GetBuildingRectangle(buildingOnCell, includingDesire: false);
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
            invalidateRect = GetBuildingRectangle(buildingOnCell, includingDesire: false);
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
            this.OnSelectionChanged();

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

    private (int x, int y) GetCellCoordidates(MouseEventArgs e)
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

    private Rectangle GetBuildingRectangle(MapBuilding building, bool includingDesire)
    {
        var size = building.BuildingType.GetSize();
        if (includingDesire)
        {
            var desireRange = new[] { building }.Concat(building.GetSubBuildings())
                .Select(x => this.MapModel.GetBuildingDesire(x).Range)
                .Max();
            return new Rectangle(
                (building.Left - desireRange) * CellSideLength,
                (building.Top - desireRange) * CellSideLength,
                (size.width + 2 * desireRange) * CellSideLength,
                (size.height + 2 * desireRange) * CellSideLength);
        }
        else
        {
            return new Rectangle(
                building.Left * CellSideLength,
                building.Top * CellSideLength,
                size.width * CellSideLength,
                size.height * CellSideLength);
        }
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

        if (cellModel.Terrain != MapTerrain.Void)
        {
            if (cellModel.TooCloseToVoidToBuild)
            {
                graphics.FillRectangle(this.tooCloseToVoidToBuildBrush, innerRect);
            }
            else if (this.ShowDesirability && cellModel.Desirability != 0)
            {
                var brush = this.GetDesirabilityBrush(cellModel.Desirability);
                graphics.DrawString(cellModel.Desirability.ToString(), this.smallFont, brush, cellRect.Left + 2, cellRect.Top + 2);
            }
        }
    }

    private Brush GetDesirabilityBrush(int desirability)
    {
        int notableDesirability = Math.Max(HouseLevelData.MinNotableDesirability, Math.Min(HouseLevelData.MaxNotableDesirability, desirability));
        int index = notableDesirability - HouseLevelData.MinNotableDesirability;
        var brush = this.desirablityBrushes[index];
        if (brush != null)
        {
            return brush;
        }

        int midpoint = (HouseLevelData.MaxNotableDesirability + HouseLevelData.MinNotableDesirability) / 2;
        var lowColor = desirability <= midpoint ? (160f, 0f, 0f) : (0f, 160f, 90f);
        var highColor = desirability <= midpoint ? (0f, 160f, 90f) : (60f, 60f, 255f);

        int halfWidth = (HouseLevelData.MaxNotableDesirability - HouseLevelData.MinNotableDesirability + 1) / 2;
        float position = (notableDesirability - HouseLevelData.MinNotableDesirability) % halfWidth;
        var color = Color.FromArgb(
            LerpToInt(lowColor.Item1, highColor.Item1, position, halfWidth),
            LerpToInt(lowColor.Item2, highColor.Item2, position, halfWidth),
            LerpToInt(lowColor.Item3, highColor.Item3, position, halfWidth));
        brush = new SolidBrush(color);
        this.desirablityBrushes[index] = brush;
        return brush;
    }

    private static int LerpToInt(float low, float high, float positon, float range)
    {
        return (int)(low + (high - low) * positon / range);
    }

    #region buildings cut-copy-paste

    public void BuildingsDelete()
    {
        BuildingsCutOrDelete(putOnClipboard: false);
    }

    public void BuildingsCut()
    {
        BuildingsCutOrDelete(putOnClipboard: true);
    }

    private void BuildingsCutOrDelete(bool putOnClipboard)
    {
        if (this.selectedBuildings.Count > 0)
        {
            if (putOnClipboard)
            {
                BuildingsCopy();
            }

            this.MapModel.IsChanged = true;
            foreach (var building in this.selectedBuildings)
            {
                this.MapModel.RemoveBuilding(building);
            }

            this.Invalidate();
        }
    }

    public void BuildingsCopy()
    {
        if (this.selectedBuildings.Count > 0)
        {
            this.clipboardBuildings.Clear();

            int minX = int.MaxValue;
            int minY = int.MaxValue;
            foreach (var building in this.selectedBuildings)
            {
                this.clipboardBuildings.Add(building.GetCopy());
                minX = Math.Min(minX, building.Left);
                minY = Math.Min(minY, building.Top);
            }

            // make their Left & Top relative to the top-left of their bounding rect
            foreach (var building in this.clipboardBuildings)
            {
                building.Left -= minX;
                building.Top -= minY;
            }
        }
    }

    private void BuildingsPaste(int cellX, int cellY)
    {
        if (this.clipboardBuildings.Count == 0)
        {
            return;
        }

        foreach (var building in this.clipboardBuildings)
        {
            var left = building.Left + cellX;
            var top = building.Top + cellY;
            if (!this.MapModel.CanAddBuilding(left, top, building.BuildingType))
            {
                return;
            }
        }

        foreach (var building in this.clipboardBuildings)
        {
            var left = building.Left + cellX;
            var top = building.Top + cellY;
            this.MapModel.AddBuilding(left, top, building.BuildingType);
        }

        this.Invalidate();
    }

    #endregion

    private void OnSelectionChanged()
    {
        if (this.SelectionChanged != null)
        {
            var roadLength = this.selectedBuildings.Count(building => building.BuildingType is MapBuildingType.Road or MapBuildingType.Plaza);
            var house2x2Count = this.selectedBuildings.Count(building => building.BuildingType is MapBuildingType.House2);
            var args = new MapSelectionChangeEventArgs
            {
                SelectedRoadLength = roadLength,
                Selected2x2HouseCount = house2x2Count,
            };
            this.SelectionChanged(this, args);
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        if (this.MapModel.EffectiveDifficulty == difficulty)
        {
            return;
        }

        this.MapModel.SetDifficulty(difficulty);
        this.Invalidate();
    }
}
