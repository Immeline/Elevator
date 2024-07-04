using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Elevator
{
	static class CabinHelper
	{
		public static Cabin FindCabinInside(Farmer farmhand)
		{
			foreach (Cabin item in GetCabinsInsides())
			{
				if (item.farmhandReference.Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID)
				{
					return item;
				}
            }
			return null;
		}

		public static Building FindCabinOutside(Farmer farmhand)
		{
			foreach (Building item in GetCabinsOutsides())
			{
				if ((item.indoors.Value as Cabin)?.farmhandReference.Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID)
					return item;
			}
			return null;
		}

		public static IEnumerable<Cabin> GetCabinsInsides()
		{
			foreach (Building building in GetCabinsOutsides())
			{
				yield return building.indoors.Value as Cabin;
			}
		}

		public static IEnumerable<Building> GetCabinsOutsides()
		{
			if (Game1.getFarm() != null)
			{
				foreach (Building building in Game1.getFarm().buildings)
				{
					if ((int)building.daysOfConstructionLeft.Value <= 0 && building.indoors.Value is Cabin) 
					{
						yield return building;
					}
				}
			}
		}

		public static void AddNewCabin(int type = 3)
		{
			//"Stone Cabin"/"Plank Cabin"/"Log Cabin"
			var blueprint = "Cabin";
			//var blueprint = /*new BluePrint(*/type == 1 ? "Stone Cabin" : type == 2 ? "Plank Cabin" : "Log Cabin"/*)*/;
			var building = Building.CreateInstanceFromId(blueprint, new Vector2(-10000, 0));
            building.load();
            building.FinishConstruction();
			building.InitializeIndoor(building.GetData(), false, false);
            //building.isCabin = true;
            Game1.getFarm().buildings.Add(building);


            foreach (var warp in building.GetIndoors().warps)
			{
				var d = GetDoorPositionOfFirstElevatorBuilding();
				warp.TargetX = d.X;
				warp.TargetY = d.Y;
			}
		}

		public static void SpawnElevatorBuilding()
		{
			var blueprint = "Shed";
            //{
            //	daysToConstruct = 0,
            //	magical = true,

            //	tilesWidth = ModEntry.ElevatorBuildingTexture.Width / 16
            //};


            //var building = new Building(blueprint, new Vector2(Game1.player.getTileX(), Game1.player.getTileY()));
            /*var*/

            //if (Game1.currentLocation.buildStructure(command[1], (command.Length > 3) ? new Vector2(Convert.ToInt32(command[2]), Convert.ToInt32(command[3])) : new Vector2(Game1.player.TilePoint.X + 1, Game1.player.TilePoint.Y), Game1.player, out var constructed))
            //{
            //    constructed.daysOfConstructionLeft.Value = 0;
            //}
            //        foreach (Building b in Game1.getFarm().buildings)
            //        {
            //if (CabinHelper.IsElevatorBuilding(b))
            //{
            //                b.resetTexture();
            //                building = b;

            //            }
            //	//{
            //	//	//b.

            //            //}
            //        }
            var building = Building.CreateInstanceFromId(blueprint, Game1.player.getStandingPosition());
            //Game1.getFarm().buildings.Add(building);
            //building.load();


			//building.InitializeIndoor(building.GetData(), false, false);
            //Use this to set it apart from an actual shed (UPDATE: see IsElevatorBuilding instead)


            //building.resetTexture();
            //var test = building.isThereAnythingtoPreventConstruction(Game1.getFarm(), Game1.player.getStandingPosition());
            if (Game1.currentLocation.buildStructure(blueprint, new Vector2(Game1.player.TilePoint.X + 1, Game1.player.TilePoint.Y), Game1.player, out building, false, true))
            {
				building.tilesWide.Value = ModEntry.ElevatorBuildingTexture.Width / 16;
                building.daysOfConstructionLeft.Value = 0;
				building.paintedTexture = ModEntry.ElevatorBuildingTexture;
				building.texture = new System.Lazy<Texture2D>(ModEntry.ElevatorBuildingTexture);
				building.magical.Value = true;
				building.indoors.Value.GetType()
						.GetField("uniqueName", BindingFlags.Instance | BindingFlags.Public)//readonly
						.SetValue(building.indoors.Value, new NetString("ElevatorBuilding"));//Don't set this in the blueprint or it will try to load an XNB "ElevatorBuilding"

                //building.humanDoor
                building.GetType()
                        .GetField("humanDoor", BindingFlags.Instance | BindingFlags.Public)//readonly
                        .SetValue(building, new NetPoint(new Point(building.humanDoor.Value.X + 2, building.humanDoor.Value.Y)));
            }
            //building.FinishConstruction();
            //building.FinishConstruction();


			//var hasIndoors = building.HasIndoors();
			//var hasIndoorsName = building.HasIndoorsName("ElevatorBuilding");
        }

		public static bool IsElevatorBuilding(Building building)
		{
			//return (building.nameOfIndoors == "ElevatorBuilding") || (building.indoors.Value is Shed && building.tilesWide.Value == ModEntry.ElevatorBuildingTexture.Width / 16);
			//return (building.GetIndoorsName() == "ElevatorBuilding") || (building.indoors.Value is Shed);
			return (building.GetIndoorsName() == "ElevatorBuilding") || (building.indoors.Value is Shed && building.tilesWide.Value == ModEntry.ElevatorBuildingTexture.Width / 16);
		}

		public static Point GetDoorPositionOfFirstElevatorBuilding()
		{
			foreach (Building building in Game1.getFarm().buildings)
			{
				if (IsElevatorBuilding(building))
				{
					return new Point(building.tileX.Value + 5, building.tileY.Value + 3);
				}
			}

			return Point.Zero;
		}

	}
}
