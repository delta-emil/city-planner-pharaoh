# City Planner - Pharaoh

A city planner tool for the classic Pharaoh city builder game.

# Installing

Currently requires Windows, at least Windows 7.

Get the latest release from the github (you're probably reading this there, so look on the right).

If you have the .NET 8 runtime installed, or are willing to install it
(".NET Desktop Runtime 8.0.6" from here: https://dotnet.microsoft.com/en-us/download/dotnet/8.0),
then you can get the small zip file.

Otherwise you can get the fat zip file that has the .NET runtime included.
This probably won't run on arm64 though, but you can then use the other option.

Either way, unzip the file in a directory and start the thing from `CityPlannerPharaoh.exe`.
Sorry, no installer at the moment. You'll have to manually make a shortcut where you want it.

# Using

The entire thing is rough around the edges because I just slapped it together for myself.

I'll describe the less obvious parts, which is the mouse and keyboard shortcuts:

* **Esc** reverts back to "select" mode if you had something selected for placement (including from paste) or the delete mode
* You can **middle-click** a building to start placing more of that building type (quicker than looking for it on the side toolbars)
* In select mode, **Shift-drag** (with the left mouse button) selects everything that is *completely* inside the rectangle
    (e.g. if you start from the middle square of a 3x3 building, that building will not be selected)
* In select mode, **Ctrl-left-click** adds or removes a building to the selection
* In select mode, you can **drag** (with the left mouse button) the selected building(s) to another location.
    If not all building can be placed, the move doesn't happen.
* You can copy or cut the selected building (toolbar, or Ctrl+C / Ctrl+X)
* You can paste (toolbar or Ctrl+V) to put the cut/copied building "in your hand" so you can place them, like placing a new building from the sidebar.
    If not all building can be placed, the placement doesn't happen.
* You can press **Del** on your keyboard to delete the selected buildings
* You can save using Ctrl+S
* You can Undo using Ctrl+Z (or the toolbar) up to 1000 actions, and Redo using Ctrl+Y or Ctrl+Shift+Z (or the toolbar)
* You can scroll horizontally using the mouse wheel while holding Shift
* You can zoom in and out using the mouse wheel while holding Ctrl
* The Glyph button exports the selected buildings, or if none, all buildings, and attempts to compose Glyphs
  (for https://pharaoh.heavengames.com/strategy/pharaohglyph/) and put them on the clipboard.
  Some building types have no glyphs, so are replaced by something else.

Notes:

* The desirabilty effects on houses on each other are also taken into consideration.
* The house level you place is your "target" level. (TODO: explain more)
* "New from game save..." lets you select a save file and it loads the terrain and buildings from it
* North is to the upper-left. (as seems to be customary in the forums)
* Currently it doesn't check if the building is allowed on the terrain type (i.e. you can place a building on a dune, or in the water).
  It only checks that you're within bounds of the map and not overlapping other buildings.

# TODO

- right-click dragging to scroll like in Julius
- nicer display names for buildings

Low prio:

- ability to add text comments to spots on the map
- monuments
- option to show walker start/finish tiles for a building
- Copy-Paste Terrain
- hold Shift to paint Terrain in a rectangle
- keyboard shortcuts
- button icons
- export to image/html(?)
- auto-downgrade plazas with a venue over them
