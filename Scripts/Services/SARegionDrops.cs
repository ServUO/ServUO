using Server.Items;

namespace Server.Services
{
    public class SARegionDrops
    {
        public static void GetSADrop(Container c)
        {
            if (Utility.RandomDouble() <= 0.90) // 10% chance to move forward and drop 
                return;

            var reg = Region.Find(c.GetWorldLocation(), c.Map);

            if (reg == null || reg.Name == null)
                return;
            
            var regionname = reg.Name;

            switch (regionname)
            {
                case "Crimson Veins":
                {
                    c.DropItem(new EssencePrecision());
                    break;
                }
                case "Fire Temple Ruins":
                {
                    c.DropItem(new EssenceOrder());
                    break;
                }
                case "Lava Caldera":
                {
                    c.DropItem(new EssencePassion());
                    break;
                }
                case "Secret Garden":
                {
                    switch (Utility.Random(2))
                    {
                        case 0:
                            c.DropItem(new EssenceFeeling());
                            break;
                        case 1:
                            c.DropItem(new FaeryDust());
                            break;
                    }
                    break;
                }
                case "Cavern of the Discarded":
                {
                    switch (Utility.Random(12))
                    {
                        case 0:
                            c.DropItem(new AbyssalCloth());
                            break;
                        case 1:
                            c.DropItem(new PowderedIron());
                            break;
                        case 2:
                            c.DropItem(new CrystallineBlackrock());
                            break;
                        case 3:
                            c.DropItem(new EssenceBalance());
                            break;
                        case 4:
                            c.DropItem(new CrystalShards());
                            break;
                        case 5:
                            c.DropItem(new ArcanicRuneStone());
                            break;
                        case 6:
                            c.DropItem(new DelicateScales());
                            break;
                        case 7:
                            c.DropItem(new SeedRenewal());
                            break;
                        case 8:
                            c.DropItem(new CrushedGlass());
                            break;
                        case 9:
                            c.DropItem(new ElvenFletchings());
                            break;
                        case 10:
                            c.DropItem(new Lodestone());
                            break;
                        case 11:
                            c.DropItem(new ReflectiveWolfEye());
                            break;
                    }
                    break;
                }
                case "Abyssal Lair":
                {
                    c.DropItem(new EssenceAchievement());
                    break;
                }
                case "StygianDragonLair":
                {
                    c.DropItem(new EssenceDiligence());
                    break;
                }
                case "Skeletal Dragon":
                {
                    c.DropItem(new EssencePersistence());
                    break;
                }
                case "Lands of the Lich":
                {
                    c.DropItem(new EssenceDirection());
                    break;
                }
                case "Passage of Tears":
                {
                    c.DropItem(new EssenceSingularity());
                    break;
                }
                case "Enslaved Goblins":
                {
                    c.DropItem(new EssenceControl());
                    break;
                }
				case "Fairy Dragon Lair":
                {
                    c.DropItem(new FaeryDust());
                    break;
                }				
            }
        }
    }
}
