using System;
using Server.Items;

namespace Server.Engines.Plants
{
    public class PlantResourceInfo
    {
        private static readonly PlantResourceInfo[] m_ResourceList = new PlantResourceInfo[]
        {
            new PlantResourceInfo(PlantType.ElephantEarPlant, PlantHue.BrightRed, typeof(RedLeaves)),
            new PlantResourceInfo(PlantType.PonytailPalm, PlantHue.BrightRed, typeof(RedLeaves)),
            new PlantResourceInfo(PlantType.CenturyPlant, PlantHue.BrightRed, typeof(RedLeaves)),
            new PlantResourceInfo(PlantType.Poppies, PlantHue.BrightOrange, typeof(OrangePetals)),
            new PlantResourceInfo(PlantType.Bulrushes, PlantHue.BrightOrange, typeof(OrangePetals)),
            new PlantResourceInfo(PlantType.PampasGrass, PlantHue.BrightOrange, typeof(OrangePetals)),
            new PlantResourceInfo(PlantType.SnakePlant, PlantHue.BrightGreen, typeof(GreenThorns)),
            new PlantResourceInfo(PlantType.BarrelCactus, PlantHue.BrightGreen, typeof(GreenThorns)),
            new PlantResourceInfo(PlantType.CocoaTree, PlantHue.Plain, typeof(CocoaPulp)),
            new PlantResourceInfo(PlantType.SugarCanes, PlantHue.Plain, typeof(SackOfSugar)),
            new PlantResourceInfo(PlantType.FlaxFlowers, PlantHue.Plain, typeof(Cotton)),
            new PlantResourceInfo(PlantType.CypressStraight, PlantHue.Plain, typeof(BarkFragment)),
            new PlantResourceInfo(PlantType.CypressTwisted, PlantHue.Plain, typeof(BarkFragment)),
            new PlantResourceInfo(PlantType.Vanilla, PlantHue.Plain, typeof(Vanilla))
        };

        private readonly PlantType m_PlantType;
        private readonly PlantHue m_PlantHue;
        private readonly Type m_ResourceType;
        private PlantResourceInfo(PlantType plantType, PlantHue plantHue, Type resourceType)
        {
            this.m_PlantType = plantType;
            this.m_PlantHue = plantHue;
            this.m_ResourceType = resourceType;
        }

        public PlantType PlantType
        {
            get
            {
                return this.m_PlantType;
            }
        }
        public PlantHue PlantHue
        {
            get
            {
                return this.m_PlantHue;
            }
        }
        public Type ResourceType
        {
            get
            {
                return this.m_ResourceType;
            }
        }
        public static PlantResourceInfo GetInfo(PlantType plantType, PlantHue plantHue)
        {
            foreach (PlantResourceInfo info in m_ResourceList)
            {
                if (info.PlantType == plantType && info.PlantHue == plantHue)
                    return info;
            }

            return null;
        }

        public Item CreateResource()
        {
            return (Item)Activator.CreateInstance(this.m_ResourceType);
        }
    }
}