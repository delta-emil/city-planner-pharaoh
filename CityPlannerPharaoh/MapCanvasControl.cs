using System.Drawing.Drawing2D;

namespace CityPlannerPharaoh;

public class MapCanvasControl : Control
{
    public const int MinCellSideLength = 8;
    public const int MaxCellSideLength = 64;
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
    private readonly Brush incorrectTextBrush;
    private readonly Brush warningTextBrush;
    private readonly Brush tooCloseToVoidToBuildBrush;
    private readonly Brush[] desirablityBrushes;

    private int cellSideLength = 24;
    private Rectangle? ghostsBoundingRect;
    private List<(Rectangle Location, bool IsValid)> ghosts = new();
    private (int x, int y) selectionDragStartCell;
    private (int x, int y) selectionDragEndCell;
    private bool? moveValid;

    public MapCanvasControl()
    {
        this.DoubleBuffered = true;
        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint
            | ControlStyles.OptimizedDoubleBuffer,
            true);
        this.UpdateStyles();

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
            new SolidBrush(Color.FromArgb(128, 172, 189, 208)), // Venue,
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
        this.incorrectTextBrush = new SolidBrush(Color.Red);
        this.warningTextBrush = new SolidBrush(Color.FromArgb(0xBB, 0, 0));
        this.tooCloseToVoidToBuildBrush = new HatchBrush(HatchStyle.Percent50, Color.FromArgb(128, 0, 0, 0), Color.FromArgb(0, 0, 0, 0));
        this.desirablityBrushes = new Brush[110];

        // mostly for the designer preview
        this.cellSideLength = 24;
        this.MapModel = new MapModel(8, 8);
        this.tool = new Tool();
        this.SelectedBuildings = new();
        this.undoStack = new UndoStack<MapModel>(1000);
    }

    public MapModel MapModel { get; private set; }
    public HashSet<MapBuilding> SelectedBuildings { get; }
    public HashSet<MapBuilding>? BuildingsToPaste { get; private set; }

    private readonly UndoStack<MapModel> undoStack;

    public void SetMapModel(MapModel mapModel)
    {
        this.MapModel = mapModel;
        this.SetSizeToFullMapSize();
        this.OnBuildingsChanged();

        this.undoStack.Init(mapModel);
        this.OnUndoStackChanged();

        this.SelectedBuildings.Clear();
        this.OnSelectionChanged();
    }

    private Tool tool;
    public Tool Tool
    {
        get => tool;
        set
        {
            tool = value;
            if (this.ghostsBoundingRect != null)
            {
                var repaintRect = this.ghostsBoundingRect.Value;
                ClearGhost();
                this.Invalidate(repaintRect);
            }
        }
    }

    public bool ShowBuildings { get; set; }

    public bool ShowDesirability { get; set; }

    public bool ShowDesirabilityFull { get; set; }

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

    public event Action<object, MapGlobalStatsChangeEventArgs>? GlobalStatsChanged;
    public event Action<object, MapSelectionChangeEventArgs>? SelectionChanged;
    public event Action<object, MapUndoStackChangeEventArgs>? UndoStackChanged;
    public event Action<object, MouseCoordsChangeEventArgs>? MouseCoordsChanged;

    private void SetSizeToFullMapSize()
    {
        this.Size = new Size(this.MapModel.MapSideX * CellSideLength, this.MapModel.MapSideY * CellSideLength);
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        var graphics = pe.Graphics;

        var buildingsToPaint = new List<MapBuilding>();
        var buildingsToPaintLater = new HashSet<MapBuilding>();

        for (int cellX = 0; cellX < this.MapModel.MapSideX; cellX++)
        {
            for (int cellY = 0; cellY < this.MapModel.MapSideY; cellY++)
            {
                // check for overload a bit wider, so that we catch venues that overlap roads on the edge of the ClipRectangle
                var cellRectExt = new Rectangle((cellX - 4) * CellSideLength, (cellY - 4) * CellSideLength, CellSideLength * 9, CellSideLength * 9);
                if (!cellRectExt.IntersectsWith(pe.ClipRectangle))
                {
                    continue;
                }

                var cellModel = this.MapModel.Cells[cellX, cellY];

                if (cellModel.Building != null && this.ShowBuildings)
                {
                    var buildingType = cellModel.Building.BuildingType;
                    if (buildingType.IgnoreMainBuilding() && buildingType.DrawMainBuildingBackground())
                    {
                        buildingsToPaintLater.Add(cellModel.Building);
                    }
                    else
                    {
                        buildingsToPaint.Add(cellModel.Building);
                    }
                }
                else
                {
                    var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                    PaintTerrainCell(graphics, cellX, cellY, cellRect, cellModel);
                }
            }
        }

        foreach (MapBuilding building in buildingsToPaint)
        {
            PaintBuilding(graphics, building);
        }
        foreach (MapBuilding building in buildingsToPaintLater)
        {
            PaintBuilding(graphics, building);
        }

        foreach (var (ghostLocation, isValid) in this.ghosts)
        {
            if (ghostLocation.IntersectsWith(pe.ClipRectangle))
            {
                var fillBrush = isValid ? this.ghostValidBrush : this.ghostInvalidBrush;
                graphics.FillRectangle(fillBrush, ghostLocation);
            }
        }

        // draw cell number (mostly for debugging)
        if (this.ShowCellCoords)
        {
            for (int cellX = 0; cellX < this.MapModel.MapSideX; cellX++)
            {
                for (int cellY = 0; cellY < this.MapModel.MapSideY; cellY++)
                {
                    var cellRectExt = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                    if (!cellRectExt.IntersectsWith(pe.ClipRectangle))
                    {
                        continue;
                    }

                    var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                    var coordsText = $"{cellX},{cellY}";
                    var coordsFont = this.smallFont;
                    var textSize = graphics.MeasureString(coordsText, coordsFont);
                    graphics.DrawString(coordsText, coordsFont, this.textBrush, cellRect.Right - textSize.Width - 2, cellRect.Bottom - textSize.Height - 2);
                }
            }
        }
    }

    private void PaintBuilding(Graphics graphics, MapBuilding building)
    {
        var buildingRect = GetBuildingRectangle(building, includingDesire: false);

        var buildingCategory = building.BuildingType.GetCategory();
        if (building.BuildingType.DrawMainBuildingBackground())
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
        var ignoreMainBuilding = building.BuildingType.IgnoreMainBuilding();
        if (!ignoreMainBuilding || this.SelectedBuildings.Contains(building))
        {
            var borderRect = new Rectangle(
                buildingRect.Left, buildingRect.Top,
                buildingRect.Width - BorderWidth, buildingRect.Height - BorderWidth);
            var borderPen
                = this.SelectedBuildings.Contains(building) ? this.borderBuildingSelectedPen
                : building.BuildingType.HasSoftBorder() ? this.borderSoftPen
                : this.borderBuildingPen;
            graphics.DrawRectangle(borderPen, borderRect);
        }

        bool isFarm = false;

        foreach (var (cellModel, cellX, cellY) in this.MapModel.EnumerateInsideBuildingWithCoords(building))
        {
            Brush? brushToApply = null;
            if (cellModel.TooCloseToVoidToBuild)
            {
                brushToApply = this.tooCloseToVoidToBuildBrush;
            }
            else if (building.BuildingType == MapBuildingType.Farm
                && cellModel.Terrain is MapTerrain.GrassFarmland or MapTerrain.SandFarmland or MapTerrain.Floodpain)
            {
                brushToApply = this.farmMeadowBrush;

                isFarm = true;
            }
                
            if (brushToApply != null)
            {
                var innerRect = new Rectangle(
                    cellX * CellSideLength + BorderWidth, cellY * CellSideLength + BorderWidth,
                    CellSideLength - BorderWidthDouble, CellSideLength - BorderWidthDouble);
                graphics.FillRectangle(brushToApply, innerRect);
            }
        }

        // draw building name
        if (building.BuildingType.ShowName())
        {
            string text = building.BuildingType.GetDisplayString();
            if (this.MapModel.IsBuildingUpgraged(building))
            {
                text += "\r\nLv.2";
            }

            var textSize = graphics.MeasureString(text, this.smallFont);

            var textBrushToUse = this.textBrush;
            if (isFarm && this.MapModel.IsFarmIrrigated(building))
            {
                textBrushToUse = this.farmIrrigatedTextBrush;
            }
            else if (this.MapModel.IsMissingRequiredWater(building)
                || this.MapModel.IsMissingRequiredCrossroad(building))
            {
                textBrushToUse = this.incorrectTextBrush;
            }

            int shiftY
                = building.BuildingType switch
                {
                    MapBuildingType.Pavilion => -7,
                    _ => 0,
                };

            graphics.DrawString(
                text, this.smallFont, textBrushToUse,
                buildingRect.Left + buildingRect.Width / 2 - textSize.Width / 2,
                buildingRect.Top + buildingRect.Height / 2 - textSize.Height / 2 + shiftY);
        }

        if (buildingCategory == MapBuildingCategory.House)
        {
            var size = building.BuildingType.GetSize();

            // draw desire
            DrawDesireabilityOnBuilding(graphics, building, size);

            string[] levelLabels = size.width == 4 ? HouseLevelData.LabelsMid : HouseLevelData.LabelsShort;

            // draw max house level
            var longLabelOfMax = levelLabels[building.MaxHouseLevel];
            string text
                = building.MaxHouseLevelExceedable
                    ? $"{longLabelOfMax}(+)"
                : building.HouseLevel == building.MaxHouseLevel
                    ? $"{longLabelOfMax}"
                :
                      $"{longLabelOfMax}({HouseLevelData.GetNeededDesire(this.MapModel.EffectiveDifficulty, building.MaxHouseLevel)}){Environment.NewLine}({levelLabels[building.HouseLevel]})";

            Brush brushToUse
                = building.HouseLevel == building.MaxHouseLevel
                    ? this.textBrush
                : building.HouseWouldNotDowngrade
                    ? this.warningTextBrush
                :
                      this.incorrectTextBrush;

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
                text, houseLabelFont, brushToUse,
                buildingRect.Left + buildingRect.Width / 2 - textSize.Width / 2 + positionShift,
                buildingRect.Top + buildingRect.Height / 2 - textSize.Height / 2 + positionShift);
        }

        DrawDesireabilityOnBuilding(graphics, building);
    }

    private void DrawDesireabilityOnBuilding(Graphics graphics, MapBuilding building, (int width, int height)? preCalculatedSize = null)
    {
        if (this.ShowDesirability)
        {
            bool fullColor, boldMaxValue;
            fullColor = boldMaxValue = building.BuildingType.GetCategory() == MapBuildingCategory.House;

            bool northCellColor = building.BuildingType is MapBuildingType.Bazaar or MapBuildingType.WaterSupply;

            bool northCellOnly;
            if (this.ShowDesirabilityFull)
            {
                northCellOnly = false;
            }
            else
            {
                if (fullColor)
                {
                    northCellOnly = false;
                }
                else if(northCellColor)
                {
                    northCellOnly = true;
                }
                else
                {
                    return;
                }
            }

            (int width, int height) size
                = northCellOnly
                ? (width: 1, height: 1)
                : preCalculatedSize ?? building.BuildingType.GetSize();

            int maxDesire = boldMaxValue
                ? this.MapModel.GetBuildingMaxDesirability(building)
                : int.MaxValue;

            for (int cellX = building.Left; cellX < building.Left + size.width; cellX++)
            {
                for (int cellY = building.Top; cellY < building.Top + size.height; cellY++)
                {
                    var desire = this.MapModel.Cells[cellX, cellY].Desirability;
                    var font = desire == maxDesire ? this.smallFontBold : this.smallFont;

                    var brush
                        = fullColor || (northCellColor && cellX == building.Left && cellY == building.Top)
                        ? this.GetDesirabilityBrush(desire)
                        : this.textBrush;
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

            var (cellX, cellY) = GetCellCoordidates(e);
            if (this.BuildingsToPaste != null)
            {
                this.BuildingsPastePlaceGhost(cellX, cellY);
            }
            else
            {
                this.ApplyTool(e, cellX, cellY, isMove: false);
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            // TODO: hold to scroll?
        }
        else if (e.Button == MouseButtons.Middle)
        {
            var (cellX, cellY) = GetCellCoordidates(e);
            var mapBuilding = this.MapModel.Cells[cellX, cellY].Building;
            if (mapBuilding != null)
            {
                this.Tool = new Tool { BuildingType = mapBuilding.BuildingType, HouseLevel = mapBuilding.MaxHouseLevel };
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
                this.RegisterUndoPoint();
                this.MapModel.MoveBuildingsByOffset(this.SelectedBuildings, offsetX, offsetY);
            }

            this.ClearGhost();
            this.moveValid = null;
            this.Invalidate();
        }
        else
        {
            // finish single click selection here, so we can distinguish it from drag-move
            ApplySingleClickSelection(e);
        }
    }

    private void ClearGhost()
    {
        this.ghostsBoundingRect = null;
        this.ghosts.Clear();
    }

    private void UpdateGhostLocation(Rectangle newGhostRect, bool isValid, bool append = false)
    {
        if (append && this.ghostsBoundingRect != null)
        {
            this.ghostsBoundingRect = Rectangle.Union(this.ghostsBoundingRect.Value, newGhostRect);
        }
        else
        {
            this.ghostsBoundingRect = newGhostRect;
            this.ghosts.Clear();
        }

        this.ghosts.Add((newGhostRect, isValid));
    }

    private static Rectangle RectUnion(Rectangle? optionalRect1, Rectangle rect2)
    {
        return optionalRect1 != null ? Rectangle.Union(optionalRect1.Value, rect2) : rect2;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var (cellX, cellY) = GetCellCoordidates(e);

        if (this.BuildingsToPaste != null)
        {
            this.UpdateBuildingDestinationGhost(this.BuildingsToPaste, cellX, cellY, zeroOffsetIsValid: true);
        }
        else
        {
            if (this.Tool.BuildingType != null)
            {
                var buildingType = this.Tool.BuildingType.Value;

                var (width, height) = buildingType.GetSize();

                var newGhostRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, width * CellSideLength, height * CellSideLength);
                if (this.ghostsBoundingRect == null || !this.ghostsBoundingRect.Equals(newGhostRect))
                {
                    var repaintRect = RectUnion(this.ghostsBoundingRect, newGhostRect);

                    var isValid = this.MapModel.CanAddBuilding(cellX, cellY, buildingType, subBuildings: null);
                    this.UpdateGhostLocation(newGhostRect, isValid);

                    Invalidate(repaintRect);
                }
            }

            if (this.Capture)
            {
                this.ApplyTool(e, cellX, cellY, isMove: true);
            }
        }

        if (this.MouseCoordsChanged != null)
        {
            var args = new MouseCoordsChangeEventArgs
            {
                CellX = cellX,
                CellY = cellY,
            };
            this.MouseCoordsChanged(this, args);
        }
    }

    private void ApplyTool(MouseEventArgs e, int cellX, int cellY, bool isMove)
    {
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

            this.RegisterUndoPoint();
            cellModel.Terrain = this.Tool.Terrain.Value;

            if (cellModel.Building == null)
            {
                var cellRect = new Rectangle(cellX * CellSideLength, cellY * CellSideLength, CellSideLength, CellSideLength);
                this.Invalidate(cellRect);
            }
        }
        else if (this.Tool.BuildingType != null)
        {
            var buildingType = this.Tool.BuildingType.Value;
            if (this.MapModel.CanAddBuilding(cellX, cellY, buildingType, subBuildings: null))
            {
                this.RegisterUndoPoint();
                this.MapModel.AddBuilding(cellX, cellY, buildingType, subBuildings: null, houseLevel: this.Tool.HouseLevel);

                // clear the ghost
                this.ClearGhost();

                this.OnBuildingsChanged();
                this.Invalidate();
            }
        }
        else if (this.Tool.IsClearBuilding)
        {
            var building = cellModel.Building;
            if (building == null)
            {
                return;
            }

            this.RegisterUndoPoint();
            this.MapModel.RemoveBuilding(building);
            this.OnBuildingsChanged();
            this.Invalidate();
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
                if (this.SelectedBuildings.Count > 0)
                {
                    this.SelectedBuildings.Clear();
                    invalidate = true;
                }

                var buildings = this.MapModel.GetAllBuildingsInRectangle(this.selectionDragStartCell, this.selectionDragEndCell);
                if (buildings.Count > 0)
                {
                    this.SelectedBuildings.UnionWith(buildings);
                    invalidate = true;
                }

                this.OnSelectionChanged();
            }
        }
        else if (isMouseMove)
        {
            // TODO: how do we move when it's a mutliple selection? the initial mouse-down reduces our selection to the clicked building
            // attempting to move stuff
            if (this.SelectedBuildings.Count > 0)
            {
                var newEndCell = (cellX, cellY);
                if (this.selectionDragEndCell != newEndCell)
                {
                    // if haven't started moving yet, check if we're starting from a selected building -> only then can we drag-move
                    if (this.moveValid == null)
                    {
                        var startCell = this.MapModel.Cells[this.selectionDragStartCell.x, this.selectionDragStartCell.y];
                        if (startCell.Building == null || !this.SelectedBuildings.Contains(startCell.Building))
                        {
                            return;
                        }
                    }

                    this.selectionDragEndCell = newEndCell;
                    var offsetX = this.selectionDragEndCell.x - this.selectionDragStartCell.x;
                    var offsetY = this.selectionDragEndCell.y - this.selectionDragStartCell.y;

                    this.moveValid = this.UpdateBuildingDestinationGhost(this.SelectedBuildings, offsetX, offsetY, zeroOffsetIsValid: false);
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
                    if (this.SelectedBuildings.Contains(buildingOnCell))
                    {
                        this.SelectedBuildings.Remove(buildingOnCell);
                    }
                    else
                    {
                        this.SelectedBuildings.Add(buildingOnCell);
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

    private bool UpdateBuildingDestinationGhost(HashSet<MapBuilding> buildingsToPlace, int offsetX, int offsetY, bool zeroOffsetIsValid)
    {
        var repaintRect = this.ghostsBoundingRect;
        this.ClearGhost();

        bool isPlaceValid;
        if (zeroOffsetIsValid || offsetX != 0 || offsetY != 0)
        {
            isPlaceValid = true;
            foreach (var building in buildingsToPlace)
            {
                var buildingCopy = building.GetCopy();
                buildingCopy.MoveLocation(deltaX: offsetX, deltaY: offsetY);

                var isValid = this.MapModel.CanAddBuilding(buildingCopy.Left, buildingCopy.Top, buildingCopy.BuildingType, buildingCopy.SubBuildings, buildingsToPlace);
                isPlaceValid &= isValid;

                if (buildingCopy.BuildingType.IgnoreMainBuilding())
                {
                    foreach (var subBuilding in buildingCopy.GetSubBuildings())
                    {
                        var (width, height) = subBuilding.BuildingType.GetSize();
                        var newGhostRect = new Rectangle(
                            subBuilding.Left * CellSideLength,
                            subBuilding.Top * CellSideLength,
                            width * CellSideLength,
                            height * CellSideLength);
                        this.UpdateGhostLocation(newGhostRect, isValid, append: true);
                        repaintRect = RectUnion(repaintRect, newGhostRect);
                    }
                }
                else
                {
                    var (width, height) = buildingCopy.BuildingType.GetSize();
                    var newGhostRect = new Rectangle(
                        buildingCopy.Left * CellSideLength,
                        buildingCopy.Top * CellSideLength,
                        width * CellSideLength,
                        height * CellSideLength);
                    this.UpdateGhostLocation(newGhostRect, isValid, append: true);
                    repaintRect = RectUnion(repaintRect, newGhostRect);
                }
            }
        }
        else
        {
            isPlaceValid = false;
        }

        if (repaintRect != null)
        {
            this.Invalidate(repaintRect.Value);
        }

        return isPlaceValid;
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
            if (this.SelectedBuildings.Count > 0)
            {
                this.SelectedBuildings.Clear();
                invalidate = true;
            }
        }
        else if (this.SelectedBuildings.Count == 0)
        {
            this.SelectedBuildings.Add(buildingOnCell);
            invalidateRect = GetBuildingRectangle(buildingOnCell, includingDesire: false);
            invalidate = true;
        }
        else
        {
            this.SelectedBuildings.Clear();
            this.SelectedBuildings.Add(buildingOnCell);
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
        return GetCellCoordidates(e.Location);
    }

    private (int x, int y) GetCellCoordidates(Point clientLocation)
    {
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

        if (cellModel.Terrain != MapTerrain.Void)
        {
            if (cellModel.TooCloseToVoidToBuild)
            {
                graphics.FillRectangle(this.tooCloseToVoidToBuildBrush, innerRect);
            }
            
            if (this.ShowDesirability && cellModel.Desirability != 0)
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
        if (this.SelectedBuildings.Count > 0)
        {
            if (putOnClipboard)
            {
                BuildingsCopy();
            }

            this.RegisterUndoPoint();

            foreach (var building in this.SelectedBuildings)
            {
                this.MapModel.RemoveBuilding(building);
            }

            this.OnBuildingsChanged();
            this.Invalidate();
        }
    }

    public void BuildingsCopy()
    {
        if (this.SelectedBuildings.Count > 0)
        {
            var clipboardBuildings = new List<MapBuilding>();

            int minX = int.MaxValue;
            int minY = int.MaxValue;
            foreach (var building in this.SelectedBuildings)
            {
                clipboardBuildings.Add(building.GetCopy());
                minX = Math.Min(minX, building.Left);
                minY = Math.Min(minY, building.Top);
            }

            // make their Left & Top relative to the top-left of their bounding rect
            foreach (var building in clipboardBuildings)
            {
                building.MoveLocation(deltaX: -minX, deltaY: -minY);
            }

            ExternalHelper.PutJsonOnClipboard(clipboardBuildings, this);
        }
    }

    public void BuildingsPasteGhost()
    {
        var clipboardBuildings = ExternalHelper.GetFromClipboardJson<List<MapBuilding>>(this);
        if (clipboardBuildings == null || clipboardBuildings.Count == 0)
        {
            return;
        }

        if (!this.Tool.IsEmpty)
        {
            this.Tool = new Tool();
        }

        this.BuildingsToPaste = [.. clipboardBuildings];

        Point mousePosInControl = this.PointToClient(Cursor.Position);
        var (cellX, cellY) = this.GetCellCoordidates(mousePosInControl);
        this.UpdateBuildingDestinationGhost(this.BuildingsToPaste, cellX, cellY, zeroOffsetIsValid: true);
    }

    private void BuildingsPastePlaceGhost(int cellX, int cellY)
    {
        if (this.BuildingsToPaste == null)
        {
            return;
        }

        var newBuildings = new List<MapBuilding>(this.BuildingsToPaste.Count);
        foreach (var building in this.BuildingsToPaste)
        {
            var newBuilding = building.GetCopy();
            newBuilding.MoveLocation(deltaX: cellX, deltaY: cellY);
            newBuildings.Add(newBuilding);
            if (!this.MapModel.CanAddBuilding(newBuilding.Left, newBuilding.Top, newBuilding.BuildingType, newBuilding.SubBuildings))
            {
                return;
            }
        }

        foreach (var newBuilding in newBuildings)
        {
            this.MapModel.AddBuildingAfterCheck(newBuilding);
        }

        if (this.SelectedBuildings.Count > 0)
        {
            this.SelectedBuildings.Clear();
            this.OnSelectionChanged();
        }

        this.ClearBuildingsToPaste();
    }

    public void ClearBuildingsToPaste()
    {
        if (this.BuildingsToPaste == null)
        {
            return;
        }

        this.ClearGhost();
        this.BuildingsToPaste = null;
        this.Invalidate();
    }

    #endregion

    private void OnBuildingsChanged()
    {
        if (this.GlobalStatsChanged != null)
        {
            var pop = this.MapModel.Buildings.Sum(building => building.GetMaxPopulation());
            var empl = this.MapModel.Buildings.Sum(this.MapModel.GetEmployees);
            var args = new MapGlobalStatsChangeEventArgs
            {
                Pop = pop,
                Empl = empl,
            };
            this.GlobalStatsChanged(this, args);
        }
    }

    private void OnSelectionChanged()
    {
        if (this.SelectionChanged != null)
        {
            var empl = this.SelectedBuildings.Sum(this.MapModel.GetEmployees);
            var roadLength = this.SelectedBuildings.Count(building => building.BuildingType is MapBuildingType.Road or MapBuildingType.Plaza);
            var house2x2Count = this.SelectedBuildings.Count(building => building.BuildingType is MapBuildingType.House2);
            var args = new MapSelectionChangeEventArgs
            {
                SelectedEmpl = empl,
                SelectedRoadLength = roadLength,
                Selected2x2HouseCount = house2x2Count,
            };
            this.SelectionChanged(this, args);
        }
    }

    private void OnUndoStackChanged(bool updateContentControls = true)
    {
        if (this.UndoStackChanged != null)
        {
            var args = new MapUndoStackChangeEventArgs
            {
                CanUndo = this.undoStack.CanUndo,
                CanRedo = this.undoStack.CanRedo,
                UpdateContentControls = updateContentControls,
                Difficulty = this.MapModel.EffectiveDifficulty,
                SimpleHouseDesire = this.MapModel.SimpleHouseDesire,
            };
            this.UndoStackChanged(this, args);
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        if (this.MapModel.EffectiveDifficulty == difficulty)
        {
            return;
        }

        this.RegisterUndoPoint();
        this.MapModel.SetDifficulty(difficulty);
        this.Invalidate();
    }

    public void SetSimpleHouseDesire(bool simpleHouseDesire)
    {
        if (this.MapModel.SimpleHouseDesire == simpleHouseDesire)
        {
            return;
        }

        this.RegisterUndoPoint();
        this.MapModel.SetSimpleHouseDesire(simpleHouseDesire);
        this.Invalidate();
    }

    public void RegisterUndoPoint()
    {
        var mapModelCopy = this.MapModel.GetDeepCopy();
        this.undoStack.Do(mapModelCopy);

        // do not update controls like difficulty shown because we're registering the undo point before the change
        // and if the change is in them, we would be undoing them
        this.OnUndoStackChanged(updateContentControls: false);
    }

    public void Undo()
    {
        if (!this.undoStack.CanUndo)
        {
            this.OnUndoStackChanged();
            return;
        }

        var mapModel = this.undoStack.Undo();
        this.MapModel = mapModel;
        this.OnUndoStackChanged();

        this.SelectedBuildings.Clear();
        this.OnSelectionChanged();

        this.Invalidate();
    }

    public void Redo()
    {
        if (!this.undoStack.CanRedo)
        {
            this.OnUndoStackChanged();
            return;
        }

        var mapModel = this.undoStack.Redo();
        this.MapModel = mapModel;
        this.OnUndoStackChanged();

        this.SelectedBuildings.Clear();
        this.OnSelectionChanged();

        this.Invalidate();
    }

    public void ChangesSaved()
    {
        this.undoStack.Save();
    }

    public bool IsChanged => this.undoStack.IsChanged;
}
