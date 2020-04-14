using Server.Items;
using System;

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
            new PlantResourceInfo(PlantType.FlaxFlowers, PlantHue.Plain, typeof(Flax)),
            new PlantResourceInfo(PlantType.CypressStraight, PlantHue.Plain, typeof(BarkFragment)),
            new PlantResourceInfo(PlantType.CypressTwisted, PlantHue.Plain, typeof(BarkFragment)),
            new PlantResourceInfo(PlantType.Vanilla, PlantHue.Plain, typeof(Vanilla)),
            new PlantResourceInfo(PlantType.PoppyPatch, PlantHue.Plain, typeof(PoppiesDust)),
            new PlantResourceInfo(PlantType.Vanilla, PlantHue.Plain, typeof(Vanilla))
        };

        private PlantResourceInfo(PlantType plantType, PlantHue plantHue, Type resourceType)
        {
            PlantType = plantType;
            PlantHue = plantHue;
            ResourceType = resourceType;
        }

        public PlantType PlantType { get; set; }
        public PlantHue PlantHue { get; set; }
        public Type ResourceType { get; set; }

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
            return (Item)Activator.CreateInstance(ResourceType);
        }
    }
}
