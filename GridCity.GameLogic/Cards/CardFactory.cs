using System.Collections.Generic;
using System;
using GridCity.GameLogic.CellElements.BaseElements;

namespace GridCity.GameLogic.Cards
{
    public static class CardFactory
    {
        private static Dictionary<CardType, Func<Card>> buildingCreators =
            new Dictionary<CardType, Func<Card>>
        {
        { CardType.SuburbanHouse, () => new BuildingCard(
            CardCategory.Building,
            CardType.SuburbanHouse,
            BuildingFactory.CreateBuilding(BuildingType.SuburbanHouse)) },

        { CardType.Park, () => new BuildingCard(
            CardCategory.Building,
            CardType.Park,
            BuildingFactory.CreateBuilding(BuildingType.Park)) },

        { CardType.ShoppingMall, () => new BuildingCard(
            CardCategory.Building,
            CardType.ShoppingMall,
            BuildingFactory.CreateBuilding(BuildingType.ShoppingMall)) },

        { CardType.Factory, () => new BuildingCard(
            CardCategory.Building,
            CardType.Factory,
            BuildingFactory.CreateBuilding(BuildingType.Factory)) },

        { CardType.Hospital, () => new BuildingCard(
            CardCategory.Building,
            CardType.Hospital,
            BuildingFactory.CreateBuilding(BuildingType.Hospital)) },

        { CardType.University, () => new BuildingCard(
            CardCategory.Building,
            CardType.University,
            BuildingFactory.CreateBuilding(BuildingType.University)) },

        { CardType.Office, () => new BuildingCard(
            CardCategory.Building,
            CardType.Office,
            BuildingFactory.CreateBuilding(BuildingType.Office)) },

        { CardType.Bar, () => new BuildingCard(
            CardCategory.Building,
            CardType.Bar,
            BuildingFactory.CreateBuilding(BuildingType.Bar)) },

        { CardType.NeighborhoodShop, () => new BuildingCard(
            CardCategory.Building,
            CardType.NeighborhoodShop,
            BuildingFactory.CreateBuilding(BuildingType.NeighborhoodShop)) },

        { CardType.TownHall, () => new BuildingCard(
            CardCategory.Building,
            CardType.TownHall,
            BuildingFactory.CreateBuilding(BuildingType.TownHall)) },

        { CardType.ApartmentBuilding, () => new BuildingCard(
            CardCategory.Building,
            CardType.ApartmentBuilding,
            BuildingFactory.CreateBuilding(BuildingType.ApartmentBuilding)) },

        { CardType.PowerPlant, () => new BuildingCard(
            CardCategory.Building,
            CardType.PowerPlant,
            BuildingFactory.CreateBuilding(BuildingType.PowerPlant)) },

        };

        public static Card CreateCard(CardType type)
        {
            if (buildingCreators.TryGetValue(type, out var creator))
            {
                return creator();
            }

            throw new ArgumentException($"Unknown card  type: {type}");
        }
    }
}