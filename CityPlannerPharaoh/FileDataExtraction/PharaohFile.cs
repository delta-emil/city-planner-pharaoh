// This class is a C# port (modified to produce a different result) of a C++ class found here:
// http://pecunia.nerdcamp.net/downloads/citybuilding
// With the following license:

/*   Citybuilding Mappers - create minimaps from citybuilding game files
 *   Copyright (C) 2007, 2008  Bianca van Schaik
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License along
 *   with this program; if not, write to the Free Software Foundation, Inc.,
 *   51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Diagnostics;
using System.Drawing;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CityPlannerPharaoh.FileDataExtraction;

public class PharaohFile : GameFile
{
    public PharaohFile(string fileName)
        : base(fileName)
    {
        MAX_MAPSIZE = 228;
        MAX_WALKERS = 0; // 2000; // I don't need the walkers
        MAX_BUILDINGS = 4000;
    }

    public MapModel? GetMapModelFromFile()
    {
        ok = true;

        // Init grids for use
        uint[,]? building_grid = null, terrain = null;
        byte[,]? edges = null, random = null;
        Walker[]? walkers = null;
        Building[]? buildings = null;
        int mapsize;
        bool is_scenario = false;
        var fourcc = new byte[4];
        InStream.Read(fourcc);
        if (Encoding.ASCII.GetString(fourcc) == "MAPS")
        {
            is_scenario = true;
        }

        if (is_scenario)
        {
            // Read scenario info
            InStream.Seek(0x177c, SeekOrigin.Begin);
            building_grid = readIntGrid();
            edges = readByteGrid();
            terrain = readIntGrid();
            skipBytes(51984);
            random = readByteGrid();
		    InStream.Seek(0x99C78, SeekOrigin.Begin);
            mapsize = (int)readInt();
        }
        else
        {
            InStream.Seek(0x177c, SeekOrigin.Begin);
            building_grid = readCompressedIntGrid();
            edges = readCompressedByteGrid();
            skipCompressed(); // building IDs
            terrain = readCompressedIntGrid();
            random = getRandomData();
            walkers = getWalkers();
            buildings = getBuildings();
            mapsize = getMapsize();
        }

        // Lil' sanity check
        if (!ok || mapsize > MAX_MAPSIZE)
        {
            return null;
        }

        // Transform it to something useful

        var mapModel = new MapModel(mapsize, mapsize, initTerrainType: MapTerrain.Void, hasTooCloseToVoidToBuild: true);
        var booths = new HashSet<(int x, int y)>();
        var bandstands = new HashSet<(int x, int y)>();
        var pavilions = new HashSet<(int x, int y)>();

        int half = MAX_MAPSIZE / 2;
        int border = (MAX_MAPSIZE - mapsize) / 2;
        int max = border + mapsize;
        int coordX, coordY;
        int start, end;
        uint t_terrain, t_building;

        var unknownCodes = new Dictionary<uint, uint>();

        for (int y = border; y < max; y++)
        {
            start = (y < half) ? (border + half - y - 1) : (border + y - half);
            end = (y < half) ? (half + y + 1 - border) : (3 * half - y - border);
            for (int x = start; x < end; x++)
            {
                t_terrain = terrain![x, y];
                t_building = building_grid![x, y];

                // Set pixel colours
                coordX = x - border;
                coordY = y - border;
                // getBitmapCoordinates(x - border, y - border, mapsize, out coordX, out coordY);

                if (0 <= coordX && coordX < mapsize && 0 <= coordY && coordY < mapsize)
                {
                    var mapTerrain = translateTerrain(t_terrain, unknownCodes);

                    mapModel.Cells[coordX, coordY].Terrain = mapTerrain;

                    var buildingType = translateTerrainToBuilding(t_terrain, t_building);
                    if (buildingType != null)
                    {
                        mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = buildingType.Value, Left = coordX, Top = coordY });
                    }

                    if (t_building is 0x3703)
                    {
                        // booth North (top-left)
                        booths.Add((coordX, coordY));
                    }
                    else if (t_building is 0x3704)
                    {
                        // booth East (top-right)
                        booths.Add((coordX - 1, coordY));
                    }
                    else if (t_building == 0x370B)
                    {
                        // bandstand center
                        bandstands.Add((coordX - 1, coordY - 1));
                    }
                    else if (t_building == 0x3715)
                    {
                        // pavilion center's North (top-left)
                        pavilions.Add((coordX - 1, coordY - 1));
                    }
                    else if (t_building == 0x3716)
                    {
                        // pavilion center's East (top-right)
                        pavilions.Add((coordX - 2, coordY - 1));
                    }
                    else if (t_building == 0x3719)
                    {
                        // pavilion center's West (bottom-left)
                        pavilions.Add((coordX - 1, coordY - 2));
                    }
                }
            }
        }

        foreach (var u in unknownCodes.Keys.OrderBy(x => x))
        {
            Debug.WriteLine($"Unknown terrain: {u:X} encountered {unknownCodes[u]} times.");
        }

        CalculateWaterEdges(mapModel.Cells, mapsize);

        List<MapBuilding> houses = new();
        List<MapBuilding> stages = new();
        List<Building> templeComplexBuildings = new();
        if (buildings != null)
        {
            for (int i = 0; i < buildings.Length; i++)
            {
                if (buildings[i].type != 0)
                {
                    AddBuilding(mapModel, houses, stages, templeComplexBuildings, buildings[i]);
                }
            }
        }

        ResolveTempleComplexes(mapModel, templeComplexBuildings);
        ResolveVenues(mapModel, stages, booths, bandstands, pavilions);

        foreach (var house in houses)
        {
            mapModel.AddBuildingAfterCheck(house);
        }

        mapModel.SetTooCloseToVoidToBuildAfterInit();
        return mapModel;
    }

    private static void ResolveTempleComplexes(MapModel mapModel, List<Building> templeComplexBuildings)
    {
        if (templeComplexBuildings.Count < 3)
        {
            return;
        }

        var sortedBuildings = templeComplexBuildings.OrderBy(b => b.x).ThenBy(b => b.y).ToList();
        if (sortedBuildings[0].y == sortedBuildings[1].y)
        {
            mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = MapBuildingType.TempleComplex1, Left = sortedBuildings[0].x, Top = sortedBuildings[0].y - 2 });
        }
        else
        {
            mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = MapBuildingType.TempleComplex2, Left = sortedBuildings[0].x - 2, Top = sortedBuildings[0].y });
        }
    }

    private static void ResolveVenues(
        MapModel mapModel,
        List<MapBuilding> stages,
        HashSet<(int x, int y)> booths,
        HashSet<(int x, int y)> bandstands,
        HashSet<(int x, int y)> pavilions)
    {
        foreach (var (x, y) in booths)
        {
            mapModel.AddBuilding(left: x, top: y, MapBuildingType.Booth, null);
        }
        foreach (var (x, y) in bandstands)
        {
            mapModel.AddBuilding(left: x, top: y, MapBuildingType.Bandstand, null);
        }
        foreach (var (x, y) in pavilions)
        {
            mapModel.AddBuilding(left: x, top: y, MapBuildingType.Pavilion, null);
        }
    }

    private static void AddBuilding(MapModel mapModel, List<MapBuilding> houses, List<MapBuilding> stages, List<Building> templeComplexBuildings, Building building)
    {
        MapBuildingType mapBuildingType;
        if (building.type < BuildingTypeMap.Length)
        {
            mapBuildingType = BuildingTypeMap[building.type];
        }
        else
        {
            mapBuildingType = (MapBuildingType)(-1);
        }

        if (mapBuildingType < (MapBuildingType)0)
        {
            if (mapBuildingType == (MapBuildingType)(-1))
            {
                Debug.WriteLine("| type / hex  | size | rotation |   x   |   y   |");
                Debug.WriteLine($"| {building.type,4} / {building.type,4:X} | {building.size,4} | {building.rotation,8} | {building.x,5} | {building.y,5} |");
                Debug.WriteLine("|-------------|------|----------|-------|-------|");
            }
            return;
        }

        if (mapBuildingType == MapBuildingType.House)
        {
            if (1 <= building.size && building.size <= 4)
            {
                mapBuildingType = building.size switch
                {
                    1 => MapBuildingType.House,
                    2 => MapBuildingType.House2,
                    3 => MapBuildingType.House3,
                    4 => MapBuildingType.House4,
                    _ => throw new Exception("Should have already been checked"),
                };
                houses.Add(
                    new MapBuilding
                    {
                        BuildingType = mapBuildingType,
                        Left = building.x,
                        Top = building.y,
                        MaxHouseLevel = building.type - BuildingTypeHouseLevel1 + 1,
                    });
            }
            else
            {
                // log?
            }
        }
        else if (mapBuildingType is MapBuildingType.DanceStage or MapBuildingType.MusicStage or MapBuildingType.JuggleStage)
        {
            stages.Add(new MapBuilding { BuildingType = mapBuildingType, Left = building.x, Top = building.y });
        }
        else if (mapBuildingType == MapBuildingType.FortBuilding)
        {
            mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = MapBuildingType.Fort, Left = building.x, Top = building.y - 1 });
        }
        else if (mapBuildingType == MapBuildingType.FortYard)
        {
            // skip
        }
        else if (mapBuildingType == MapBuildingType.TempleComplexBuilding)
        {
            templeComplexBuildings.Add(building);
        }
        else if (mapBuildingType == MapBuildingType.StorageYardTower)
        {
            mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = MapBuildingType.StorageYard, Left = building.x, Top = building.y });
        }
        else if (mapBuildingType == MapBuildingType.StorageYard)
        {
            // skip
        }
        else if (mapBuildingType == MapBuildingType.Gate1)
        {
            // the N 2x2 only, Gate1 is rot:1, Gate2 is rot:0
            mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = building.rotation != 0 ? MapBuildingType.Gate1 : MapBuildingType.Gate2, Left = building.x, Top = building.y });
        }
        else if (mapBuildingType == MapBuildingType.Roadblock)
        {
            var existingBuilding = mapModel.Cells[building.x, building.y].Building;
            if (existingBuilding != null)
            {
                existingBuilding.BuildingType = MapBuildingType.Roadblock;
            }
            else
            {
                mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = mapBuildingType, Left = building.x, Top = building.y });
            }
        }
        else
        {
            // default
            mapModel.AddBuildingAfterCheck(new MapBuilding { BuildingType = mapBuildingType, Left = building.x, Top = building.y });
        }
    }


    private const int BuildingTypeHouseLevel1 = 10;

    private static readonly MapBuildingType[] BuildingTypeMap =
        [
            (MapBuildingType)(-1), // 0
            (MapBuildingType)(-1), // 1
            (MapBuildingType)(-1), // 2
            (MapBuildingType)(-1), // 3
            (MapBuildingType)(-1), // 4
            (MapBuildingType)(-1), // 5
            (MapBuildingType)(-1), // 6
            (MapBuildingType)(-1), // 7
            (MapBuildingType)(-1), // 8
            (MapBuildingType)(-1), // 9
            MapBuildingType.House, // 10
            MapBuildingType.House, // 11
            MapBuildingType.House, // 12
            MapBuildingType.House, // 13
            MapBuildingType.House, // 14
            MapBuildingType.House, // 15
            MapBuildingType.House, // 16
            MapBuildingType.House, // 17
            MapBuildingType.House, // 18
            MapBuildingType.House, // 19
            MapBuildingType.House, // 20
            MapBuildingType.House, // 21
            MapBuildingType.House, // 22
            MapBuildingType.House, // 23
            MapBuildingType.House, // 24
            MapBuildingType.House, // 25
            MapBuildingType.House, // 26
            MapBuildingType.House, // 27
            MapBuildingType.House, // 28
            MapBuildingType.House, // 29
            MapBuildingType.MusicStage, // 30
            MapBuildingType.JuggleStage, // 31
            MapBuildingType.Senet, // 32
            MapBuildingType.DanceStage, // 33
            MapBuildingType.MusicSchool, // 34
            MapBuildingType.DanceSchool, // 35
            MapBuildingType.JuggleSchool, // 36
            (MapBuildingType)(-1), // 37
            (MapBuildingType)(-1), // 38
            (MapBuildingType)(-1), // 39 Garden?
            (MapBuildingType)(-1), // 40
            MapBuildingType.StatueSmall, // 41
            MapBuildingType.StatueMedium, // 42
            MapBuildingType.StatueLarge, // 43
            (MapBuildingType)(-1), // 44
            (MapBuildingType)(-1), // 45
            MapBuildingType.Apothecary, // 46
            MapBuildingType.Mortuary, // 47
            (MapBuildingType)(-1), // 48
            MapBuildingType.Dentist, // 49
            (MapBuildingType)(-1), // 50
            MapBuildingType.ScribeSchool, // 51
            (MapBuildingType)(-1), // 52
            MapBuildingType.Library, // 53
            MapBuildingType.FortYard, // 54
            MapBuildingType.Police, // 55
            (MapBuildingType)(-1), // 56
            MapBuildingType.FortBuilding, // 57
            (MapBuildingType)(-1), // 58
            (MapBuildingType)(-1), // 59
            MapBuildingType.Temple, // 60
            MapBuildingType.Temple, // 61
            MapBuildingType.Temple, // 62
            MapBuildingType.Temple, // 63
            MapBuildingType.Temple, // 64
            MapBuildingType.TempleComplexBuilding, // 65 (rot 0 is TC1, rot 6 is TC2)
            MapBuildingType.TempleComplexBuilding, // 66
            MapBuildingType.TempleComplexBuilding, // 67
            MapBuildingType.TempleComplexBuilding, // 68
            MapBuildingType.TempleComplexBuilding, // 69
            MapBuildingType.Bazaar, // 70
            MapBuildingType.Granary, // 71
            MapBuildingType.StorageYardTower, // 72
            MapBuildingType.StorageYard, // 73 - each 1x1 tile of the storage
            MapBuildingType.Shipwright, // 74
            MapBuildingType.Dock, // 75
            MapBuildingType.Fisher, // 76
            MapBuildingType.MansionPersonal, // 77
            MapBuildingType.MansionFamily, // 78
            MapBuildingType.MansionDynasty, // 79
            (MapBuildingType)(-1), // 80
            MapBuildingType.Architect, // 81
            (MapBuildingType)(-1), // 82
            (MapBuildingType)(-1), // 83
            (MapBuildingType)(-1), // 84
            (MapBuildingType)(-1), // 85
            MapBuildingType.Tax, // 86
            (MapBuildingType)(-1), // 87
            (MapBuildingType)(-1), // 88
            (MapBuildingType)(-1), // 89
            MapBuildingType.WaterLift, // 90
            (MapBuildingType)(-1), // 91
            MapBuildingType.Well, // 92
            (MapBuildingType)(-1), // 93
            MapBuildingType.Academy, // 94
            MapBuildingType.Recruiter, // 95
            (MapBuildingType)(-1), // 96
            (MapBuildingType)(-1), // 97
            (MapBuildingType)(-1), // 98
            (MapBuildingType)(-1), // 99
            MapBuildingType.Farm, // 100
            MapBuildingType.Farm, // 101
            MapBuildingType.Farm, // 102
            MapBuildingType.Farm, // 103
            MapBuildingType.Farm, // 104
            MapBuildingType.Farm, // 105
            MapBuildingType.QuarryPlainStone, // 106
            MapBuildingType.QuarryLimestone, // 107
            MapBuildingType.Wood, // 108
            MapBuildingType.Clay, // 109
            MapBuildingType.Brewer, // 110
            MapBuildingType.Weaver, // 111
            MapBuildingType.Weaponsmith, // 112
            MapBuildingType.Jeweler, // 113
            MapBuildingType.Potter, // 114
            MapBuildingType.Hunter, // 115
            (MapBuildingType)(-1), // 116
            (MapBuildingType)(-1), // 117
            (MapBuildingType)(-1), // 118
            (MapBuildingType)(-1), // 119
            (MapBuildingType)(-1), // 120
            (MapBuildingType)(-1), // 121
            (MapBuildingType)(-1), // 122
            (MapBuildingType)(-1), // 123
            (MapBuildingType)(-1), // 124
            (MapBuildingType)(-1), // 125
            (MapBuildingType)(-1), // 126
            (MapBuildingType)(-1), // 127
            (MapBuildingType)(-1), // 128
            (MapBuildingType)(-1), // 129
            (MapBuildingType)(-1), // 130
            (MapBuildingType)(-1), // 131
            (MapBuildingType)(-1), // 132
            (MapBuildingType)(-1), // 133
            (MapBuildingType)(-1), // 134
            (MapBuildingType)(-1), // 135
            MapBuildingType.FerryLanding, // 136
            (MapBuildingType)(-1), // 137
            MapBuildingType.Roadblock, // 138
            (MapBuildingType)(-1), // 139
            MapBuildingType.Shrine, // 140
            MapBuildingType.Shrine, // 141
            MapBuildingType.Shrine, // 142
            MapBuildingType.Shrine, // 143
            MapBuildingType.Shrine, // 144
            (MapBuildingType)(-1), // 145
            (MapBuildingType)(-1), // 146
            (MapBuildingType)(-1), // 147
            (MapBuildingType)(-1), // 148
            (MapBuildingType)(-1), // 149
            (MapBuildingType)(-1), // 150
            (MapBuildingType)(-1), // 151
            (MapBuildingType)(-1), // 152
            (MapBuildingType)(-1), // 153
            (MapBuildingType)(-1), // 154
            (MapBuildingType)(-1), // 155
            (MapBuildingType)(-1), // 156
            (MapBuildingType)(-1), // 157
            (MapBuildingType)(-1), // 158
            (MapBuildingType)(-1), // 159
            (MapBuildingType)(-1), // 160
            MapBuildingType.MineGold, // 161
            MapBuildingType.MineGems, // 162
            (MapBuildingType)(-1), // 163
            (MapBuildingType)(-1), // 164
            (MapBuildingType)(-1), // 165
            (MapBuildingType)(-1), // 166
            MapBuildingType.Firehouse, // 167
            (MapBuildingType)(-1), // 168
            (MapBuildingType)(-1), // 169
            (MapBuildingType)(-1), // 170
            (MapBuildingType)(-1), // 171
            (MapBuildingType)(-1), // 172
            MapBuildingType.Tower, // 173
            (MapBuildingType)(-1), // 174
            (MapBuildingType)(-1), // 175
            (MapBuildingType)(-1), // 176
            MapBuildingType.GuildCarpenter, // 177
            MapBuildingType.GuildBricklayer, // 178
            MapBuildingType.GuildStonemason, // 179
            MapBuildingType.WaterSupply, // 180
            MapBuildingType.TransportShip, // 181
            MapBuildingType.Warship, // 182
            (MapBuildingType)(-2), // 183 // pyramid 2x2 piece
            MapBuildingType.Courthouse, // 184
            (MapBuildingType)(-1), // 185
            (MapBuildingType)(-1), // 186
            MapBuildingType.PalaceVillage, // 187
            MapBuildingType.PalaceTown, // 188
            MapBuildingType.PalaceCity, // 189
            (MapBuildingType)(-1), // 190
            (MapBuildingType)(-1), // 191
            (MapBuildingType)(-1), // 192
            (MapBuildingType)(-1), // 193
            MapBuildingType.Cattle, // 194
            MapBuildingType.Reed, // 195
            MapBuildingType.Farm, // 196
            (MapBuildingType)(-1), // 197
            (MapBuildingType)(-1), // 198
            MapBuildingType.WorkCamp, // 199
            (MapBuildingType)(-1), // 200
            (MapBuildingType)(-1), // 201
            MapBuildingType.Gate1, // 202 // the N 2x2 only, Gate1 is rot:1, Gate2 is rot:0
            MapBuildingType.Papyrus, // 203
            MapBuildingType.Bricks, // 204
            MapBuildingType.Chariot, // 205
            MapBuildingType.Physician, // 206
            (MapBuildingType)(-2), // 207  // obelisk
            (MapBuildingType)(-1), // 208
            MapBuildingType.FestivalSquare, // 209
            (MapBuildingType)(-1), // 210
            (MapBuildingType)(-2), // 211 // TC upgrages size 0
            (MapBuildingType)(-2), // 212 // TC upgrages size 0
            (MapBuildingType)(-1), // 213
            (MapBuildingType)(-1), // 214
            (MapBuildingType)(-1), // 215
            MapBuildingType.QuarryGranite, // 216
            MapBuildingType.MineCopper, // 217
            (MapBuildingType)(-1), // 218
            (MapBuildingType)(-1), // 219
            (MapBuildingType)(-1), // 220
            MapBuildingType.QuarrySandstone, // 221
            (MapBuildingType)(-1), // 222
            (MapBuildingType)(-1), // 223
            (MapBuildingType)(-1), // 224
            (MapBuildingType)(-1), // 225
            MapBuildingType.Zoo, // 226
            (MapBuildingType)(-1), // 227
            (MapBuildingType)(-1), // 228
            (MapBuildingType)(-1), // 229
            (MapBuildingType)(-1), // 230
            MapBuildingType.GuildArtisan, // 231
            MapBuildingType.Lamps, // 232
            MapBuildingType.Paint, // 233
            (MapBuildingType)(-1), // 234
            (MapBuildingType)(-1), // 235
            (MapBuildingType)(-1), // 236
            (MapBuildingType)(-1), // 237
            (MapBuildingType)(-1), // 238
            (MapBuildingType)(-1), // 239
            (MapBuildingType)(-1), // 240
            (MapBuildingType)(-1), // 241
            (MapBuildingType)(-1), // 242
            (MapBuildingType)(-1), // 243
            (MapBuildingType)(-1), // 244
            (MapBuildingType)(-1), // 245
            (MapBuildingType)(-1), // 246
            (MapBuildingType)(-1), // 247
            (MapBuildingType)(-1), // 248
            (MapBuildingType)(-1), // 249
            (MapBuildingType)(-1), // 250
            (MapBuildingType)(-1), // 251
            (MapBuildingType)(-1), // 252
            (MapBuildingType)(-1), // 253
            (MapBuildingType)(-1), // 254
            (MapBuildingType)(-1), // 255
        ];

    private void CalculateWaterEdges(MapCellModel[,] cells, int mapsize)
    {
        for (int x = 0; x < mapsize; x++)
        {
            for (int y = 0; y < mapsize; y++)
            {
                var terrain = cells[x, y].Terrain;
                if (terrain == MapTerrain.Void) continue;
                if (terrain == MapTerrain.Water)
                {
                    if (   IsLand(cells, x - 1, y - 1)
                        || IsLand(cells, x - 1, y)
                        || IsLand(cells, x - 1, y + 1)
                        || IsLand(cells, x,     y - 1)
                        || IsLand(cells, x,     y + 1)
                        || IsLand(cells, x + 1, y - 1)
                        || IsLand(cells, x + 1, y)
                        || IsLand(cells, x + 1, y + 1))
                    {
                        cells[x, y].Terrain = MapTerrain.WaterEdge;
                    }
                }
                else if (terrain == MapTerrain.Floodpain)
                {
                    SetClearLandTo(cells, x - 1, y - 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x - 1, y    , MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x - 1, y + 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x,     y - 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x,     y + 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x + 1, y - 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x + 1, y,     MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x + 1, y + 1, MapTerrain.FloodpainEdge);
                }
            }
        }
    }

    private static bool IsLand(MapCellModel[,] cells, int x, int y)
    {
        if (x < 0 || cells.GetLength(0) <= x
         || y < 0 || cells.GetLength(1) <= y)
        {
            return false;
        }
        var terrain = cells[x, y].Terrain;
        return terrain != MapTerrain.Void
            && terrain != MapTerrain.Water
            && terrain != MapTerrain.WaterEdge;
    }

    private static void SetClearLandTo(MapCellModel[,] cells, int x, int y, MapTerrain newTerrain)
    {
        if (x < 0 || cells.GetLength(0) <= x
         || y < 0 || cells.GetLength(1) <= y)
        {
            return;
        }

        var cell = cells[x, y];
        var terrain = cell.Terrain;
        if (terrain == MapTerrain.Grass
            || terrain == MapTerrain.GrassFarmland
            || terrain == MapTerrain.Sand
            || terrain == MapTerrain.SandFarmland)
        {
            cell.Terrain = newTerrain;
        }
    }

    /**
    * Figures out what the terrain colours for this tile are
    */
    private MapTerrain translateTerrain(uint terrain, Dictionary<uint, uint> unknownCodes)
    {
        if ((terrain & 0x8_00_00) != 0)
        {
            return MapTerrain.Void;
        }

        if ((terrain & 0x1) != 0)
        {
            return MapTerrain.Trees;
        }
        else if ((terrain & 0x4_00_00) != 0)
        {
            return MapTerrain.Reeds;
        }
        else if ((terrain & 0x2) != 0)
        {
            if ((terrain & 0x10_00_00) != 0)
            {
                return MapTerrain.RockOre;
            }
            else
            {
                return MapTerrain.Rock;
            }
        }
        else if ((terrain & 0x4) != 0)
        {
            return MapTerrain.Water;
        }
        else if ((terrain & 0x2_00_00_00) != 0)
        {
            return MapTerrain.Dune;
        }
        else if ((terrain & 0x1_00_00) != 0)
        {
            return MapTerrain.Floodpain;
        }
        else if ((terrain & 0x80) != 0)
        {
            if ((terrain & 0x8_00) != 0)
            {
                return MapTerrain.GrassFarmland;
            }
            else
            {
                return MapTerrain.Grass;
            }
        }
        else if ((terrain & 0x8_00) != 0)
        {
            return MapTerrain.SandFarmland;
        }
        // start ignored codes
        else if ((terrain & 0x40) != 0)
        {
            // road
        }
        // log unknown codes
        else
        {
            if (!unknownCodes.TryGetValue(terrain, out var count))
            {
                // Debug.WriteLine($"Unknown terrain: {terrain:X}");
                count = 0;
            }

            unknownCodes[terrain] = count + 1;
        }
        
        // empty land or watered land
        return MapTerrain.Sand;
    }

    private MapBuildingType? translateTerrainToBuilding(uint terrain, uint building)
    {
        if ((terrain & 0x8_00_00) != 0)
        {
            // MapTerrain.Void
            return null;
        }

        if ((terrain & 0x40) != 0)
        { // road
            if ((terrain & 0x8) != 0)
            { // road + building -> could be roadblock or venue roads - they will remove it if needed
                return MapBuildingType.Road;
            }
            else if ((terrain & 0x4) != 0)
            { // road + water = bridge
                if ((terrain & 0x4_00_00_00) != 0)
                { // ferry path?
                    //return null;
                    return MapBuildingType.Bridge;
                }
                else
                {
                    return MapBuildingType.Bridge;
                }
            }
            else if (building is 0x3739 or 0x373A or 0x373B)
            {
                return MapBuildingType.Plaza;
            }
            else
            {
                return MapBuildingType.Road;
            }
        }
        else if ((terrain & 0x20) != 0)
        { // garden
            if ((terrain & 0x8) != 0)
            { // garden + building = garden that's part of a venue
                // TODO
            }
            else
            {
                return MapBuildingType.Garden;
            }
        }
        else if ((terrain & 0x100) != 0)
        { // irrigation
            return MapBuildingType.Ditch;
        }
        else if ((terrain & 0x804000) != 0)
        {// wall
            return MapBuildingType.Wall;
        }
        else if ((terrain & 0x10000000) != 0)
        { // monument plazas, etc
            return MapBuildingType.Plaza; // ??
        }

        return null;
    }

    /**
    * Reads the random data grid
    */
    private byte[,]? getRandomData()
    {
        // Assume the first 4 chunks of data have been read
        // The next 4 (useless) compressed parts are stored in the same way
        for (int i = 0; i < 4; i++)
        {
            skipCompressed();
        }

        // Here be the random data
        return readByteGrid();
    }

    /**
    * Gets the walker info from the saved game, which is buried after
    * a few "useless" compressed chunks.
    */
    private Walker[]? getWalkers()
    {
        if (!ok) return null;

        // Assume random data has been read already
        // Which is followed by some more compressed data blocks
        for (int i = 0; i < 5; i++)
        {
            skipCompressed();
        }

        // Next come the walkers in compressed format
        var walkers = new Walker[MAX_WALKERS];
        int length = (int)readInt();

        using var pk = new PKWareInputStream(InStream, length, false);

        // Walker entry = 388 bytes
        for (int i = 0; i < MAX_WALKERS; i++)
        {
            if (pk.hasError()) break;

            pk.skip(10);
            walkers[i].type = pk.readShort();
            pk.skip(8);
            walkers[i].x = pk.readShort();
            walkers[i].y = pk.readShort();
            pk.skip(364);
        }
        pk.empty();

        if (pk.hasError())
        {
            ok = false;
            return null;
        }
        return walkers;
    }

    /**
    * Gets the building info from the saved game
    */
    private Building[]? getBuildings()
    {
        if (!ok) return null;

        // Assume walkers have been read already

        // Three compressed blocks
        for (int i = 0; i < 3; i++)
        {
            skipCompressed();
        }

        // Three ints
        skipBytes(12);

        // Compressed block, followed by 72 bytes, followed by walkers
        skipCompressed();
        skipBytes(72);

        var buildings = new Building[MAX_BUILDINGS];
        int length = (int)readInt();

        using var pk = new PKWareInputStream(InStream, length, false);

        // Building entry = 264 bytes
        for (int i = 0; i < MAX_BUILDINGS; i++)
        {
            if (pk.hasError()) break;

            pk.skip(3);
            buildings[i].size = pk.readByte();
            pk.skip(2);
            buildings[i].x = pk.readShort();
            buildings[i].y = pk.readShort();
            pk.skip(6);
            buildings[i].type = pk.readShort();
            pk.skip(152);
            buildings[i].rotation = pk.readByte();
            pk.skip(93);
        }
        pk.empty();

        if (pk.hasError())
        {
            ok = false;
            return null;
        }
        return buildings;
    }

    /**
    * Returns the map size of a saved game. Assumes everything up to
    * and including buildings have been read already.
    */
    private int getMapsize()
    {
        if (!ok) return 0;

        // Assume buildings have been read already

        // 68 bytes, followed by one compressed
        skipBytes(68);
        skipCompressed();

        // Start of data copied from the .map file!
        // 704 bytes to skip
        skipBytes(704);
        // Next int = map size
        int mapsize = (int)readInt();

        return mapsize;
    }

    private struct Walker
    {
        public ushort type;
        public ushort x;
        public ushort y;
    }

    private struct Building
    {
        public ushort type;
        public ushort x;
        public ushort y;
        public byte size;
        public byte rotation;
    }
}
