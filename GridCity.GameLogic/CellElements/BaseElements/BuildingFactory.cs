using GridCity.GameLogic.CellElements.Buildings;
using System;
using System.Collections.Generic;

namespace GridCity.GameLogic.CellElements.BaseElements
{
    public static class BuildingFactory
    {
        private static Dictionary<BuildingType, Func<Building>> buildingCreators =
            new Dictionary<BuildingType, Func<Building>>
        {
        { BuildingType.SuburbanHouse, () => new SuburbanHouse() },
        { BuildingType.Park, () => new Park() },
        { BuildingType.ShoppingMall, () => new ShoppingMall() },
        { BuildingType.Factory, () => new Factory() },
        { BuildingType.Hospital, () => new Hospital() },
        { BuildingType.University, () => new University() },
        { BuildingType.Office, () => new Office() },
        { BuildingType.Bar, () => new Bar() },
        { BuildingType.NeighborhoodShop, () => new NeighborhoodShop() },
        { BuildingType.TownHall, () => new TownHall() },
        { BuildingType.ApartmentBuilding, () => new ApartmentBuilding() },
        { BuildingType.PowerPlant, () => new PowerPlant() }
        };

        public static Building CreateBuilding(BuildingType type)
        {
            if (buildingCreators.TryGetValue(type, out var creator))
            {
                return creator();
            }
            throw new ArgumentException($"Unknown building type: {type}");
        }
    }
}