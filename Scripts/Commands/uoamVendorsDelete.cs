using System;
using System.Collections;
using System.IO;
using Server.Commands;
using Server.Mobiles;

// Based on UOAMVednors command 0.8 version
namespace Server
{
    public class UOAMVendorDeletion
    {
        private static readonly Queue m_ToDelete = new Queue();
        private static int m_Count;
        public static void Initialize()
        {
            CommandSystem.Register("UOAMVendorsDelete", AccessLevel.Administrator, new CommandEventHandler(Generate_OnCommand));
        }

        public static void Parse(Mobile from)
        {
            string vendor_path = Path.Combine(Core.BaseDirectory, "Data/Common.map");
            m_Count = 0;

            if (File.Exists(vendor_path))
            {
                ArrayList list = new ArrayList();
                from.SendMessage("Finding Vendors...");

                using (StreamReader ip = new StreamReader(vendor_path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        int indexOf = line.IndexOf(':');

                        if (indexOf == -1)
                        {
                            continue;
                        }

                        string type = line.Substring(0, ++indexOf).Trim();
                        string sub = line.Substring(indexOf).Trim();

                        string[] split = sub.Split(' ');

                        if (split.Length < 3)
                        {
                            continue;
                        }

                        split = new string[] { type, split[0], split[1], split[2] };

                        switch ( split[0].ToLower() )
                        {
                            case "-healer:":
                                DeleteNPC(split[1], split[2], split[3], "Healer", "HealerGuildmaster");
                                break;
                            case "-baker:":
                                DeleteNPC(split[1], split[2], split[3], "Baker");
                                break;
                            case "-vet:":
                                DeleteNPC(split[1], split[2], split[3], "Veterinarian");
                                break;
                            case "-gypsymaiden:":
                                DeleteNPC(split[1], split[2], split[3], "GypsyMaiden");
                                break;
                            case "-gypsybank:":
                                DeleteNPC(split[1], split[2], split[3], "GypsyBanker");
                                break;
                            case "-bank:":
                                DeleteNPC(split[1], split[2], split[3], "Banker", "Minter");
                                break;
                            case "-inn:":
                                DeleteNPC(split[1], split[2], split[3], "Innkeeper");
                                break;
                            case "-provisioner:":
                                DeleteNPC(split[1], split[2], split[3], "Provisioner", "Cobbler");
                                break;
                            case "-tailor:":
                                DeleteNPC(split[1], split[2], split[3], "Tailor", "Weaver", "TailorGuildmaster");
                                break;
                            case "-tavern:":
                                DeleteNPC(split[1], split[2], split[3], "Tavernkeeper", "Waiter", "Cook", "Barkeeper");
                                break;
                            case "-reagents:":
                                DeleteNPC(split[1], split[2], split[3], "Herbalist", "Alchemist", "CustomHairstylist");
                                break;
                            case "-fortuneteller:":
                                DeleteNPC(split[1], split[2], split[3], "FortuneTeller");
                                break;
                            case "-holymage:":
                                DeleteNPC(split[1], split[2], split[3], "HolyMage");
                                break;
                            case "-chivalrykeeper:":
                                DeleteNPC(split[1], split[2], split[3], "KeeperOfChivalry");
                                break;
                            case "-mage:":
                                DeleteNPC(split[1], split[2], split[3], "Mage", "Alchemist", "MageGuildmaster");
                                break;
                            case "-arms:":
                                DeleteNPC(split[1], split[2], split[3], "Armorer", "Weaponsmith");
                                break;
                            case "-tinker:":
                                DeleteNPC(split[1], split[2], split[3], "Tinker", "TinkerGuildmaster");
                                break;
                            case "-gypsystable:":
                                DeleteNPC(split[1], split[2], split[3], "GypsyAnimalTrainer");
                                break;
                            case "-stable:":
                                DeleteNPC(split[1], split[2], split[3], "AnimalTrainer");
                                break;
                            case "-blacksmith:":
                                DeleteNPC(split[1], split[2], split[3], "Blacksmith", "BlacksmithGuildmaster");
                                break;
                            case "-bowyer:":
                            case "-fletcher:":
                                DeleteNPC(split[1], split[2], split[3], "Bowyer");
                                break;
                            case "-carpenter:":
                                DeleteNPC(split[1], split[2], split[3], "Carpenter", "Architect", "RealEstateBroker");
                                break;
                            case "-butcher:":
                                DeleteNPC(split[1], split[2], split[3], "Butcher");
                                break;
                            case "-jeweler:":
                                DeleteNPC(split[1], split[2], split[3], "Jeweler");
                                break;
                            case "-tanner:":
                                DeleteNPC(split[1], split[2], split[3], "Tanner", "Furtrader");
                                break;
                            case "-bard:":
                                DeleteNPC(split[1], split[2], split[3], "Bard", "BardGuildmaster");
                                break;
                            case "-market:":
                                DeleteNPC(split[1], split[2], split[3], "Butcher", "Farmer");
                                break;
                            case "-library:":
                                DeleteNPC(split[1], split[2], split[3], "Scribe");
                                break;
                            case "-shipwright:":
                                DeleteNPC(split[1], split[2], split[3], "Shipwright", "Mapmaker");
                                break;
                            case "-docks:":
                                DeleteNPC(split[1], split[2], split[3], "Fisherman");
                                break;
                            case "-beekeeper:":
                                DeleteNPC(split[1], split[2], split[3], "Beekeeper");
                                break;
                                // Guilds & Misc
                            case "-tinkers guild:":
                                DeleteNPC(split[1], split[2], split[3], "TinkerGuildmaster");
                                break;
                            case "-blacksmiths guild:":
                                DeleteNPC(split[1], split[2], split[3], "BlacksmithGuildmaster");
                                break;
                            case "-sorcerors guild:":
                                DeleteNPC(split[1], split[2], split[3], "MageGuildmaster");
                                break;
                            case "-customs:":
                                break;
                            case "-painter:":
                                break;
                            case "-theater:":
                                break;
                            case "-warriors guild:":
                                DeleteNPC(split[1], split[2], split[3], "WarriorGuildmaster");
                                break;
                            case "-archers guild:":
                                DeleteNPC(split[1], split[2], split[3], "RangerGuildmaster");
                                break;
                            case "-thieves guild:":
                                DeleteNPC(split[1], split[2], split[3], "ThiefGuildmaster");
                                break;
                            case "-miners guild:":
                                DeleteNPC(split[1], split[2], split[3], "MinerGuildmaster");
                                break;
                            case "-fishermans guild:":
                                DeleteNPC(split[1], split[2], split[3], "FisherGuildmaster");
                                break;
                            case "-merchants guild:":
                                DeleteNPC(split[1], split[2], split[3], "MerchantGuildmaster");
                                break;
                            case "-illusionists guild:":
                                break;
                            case "-armourers guild:":
                                break;
                            case "-sorcerers guild:":
                                break;
                            case "-mages guild:":
                                DeleteNPC(split[1], split[2], split[3], "MageGuildmaster");
                                break;
                            case "-weapons guild:":
                                break;
                            case "-bardic guild:":
                                DeleteNPC(split[1], split[2], split[3], "BardGuildmaster");
                                break;
                            case "-rogues guild:":
                                break;
                                // Skip
                            case "+landmark:":
                            case "-point of interest:":
                            case "+shrine:":
                            case "+moongate:":
                            case "+dungeon:":
                            case "+scenic:":
                            case "-gate:":
                            case "+Body of Water:":
                            case "+ruins:":
                            case "+teleporter:":
                            case "+Terrain:":
                            case "-exit:":
                            case "-bridge:":
                            case "-other:":
                            case "+champion:":
                            case "-stairs:":
                            case "-guild:":
                            case "+graveyard:":
                            case "+Island:":
                            case "+town:":
                                break;
                        /*default:
                        Console.WriteLine(split[0]);
                        break;*/
                        }
                    }
                }
                from.SendMessage("Done, deleted {0} spawners", m_Count);
            }
            else
            {
                from.SendMessage("{0} not found!", vendor_path);
            }
        }

        public static void DeleteNPC(string sx, string sy, string sm, params string[] types)
        {
            if (types.Length == 0)
            {
                return;
            }

            int x = Utility.ToInt32(sx);
            int y = Utility.ToInt32(sy);
            int map = Utility.ToInt32(sm);

            switch ( map )
            {
                case 0: //Trammel and Felucca
                    DeleteSpawner(types, x, y, Map.Felucca);
                    DeleteSpawner(types, x, y, Map.Trammel);
                    break;
                case 1: //Felucca
                    DeleteSpawner(types, x, y, Map.Felucca);
                    break;
                case 2:
                    DeleteSpawner(types, x, y, Map.Trammel);
                    break;
                case 3:
                    DeleteSpawner(types, x, y, Map.Ilshenar);
                    break;
                case 4:
                    DeleteSpawner(types, x, y, Map.Malas);
                    break;
                default:
                    Console.WriteLine("UOAM Vendor Parser: Warning, unknown map {0}", map);
                    break;
            }
        }

        public static int GetSpawnerZ(int x, int y, Map map)
        {
            int z = map.GetAverageZ(x, y);

            if (map.CanFit(x, y, z, 16, false, false, true))
            {
                return z;
            }

            for (int i = 1; i <= 20; ++i)
            {
                if (map.CanFit(x, y, z + i, 16, false, false, true))
                {
                    return z + i;
                }

                if (map.CanFit(x, y, z - i, 16, false, false, true))
                {
                    return z - i;
                }
            }

            return z;
        }

        public static void ClearSpawners(int x, int y, int z, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, z), 0);

            foreach (Item item in eable)
            {
                if (item is Spawner && item.Z == z)
                {
                    m_ToDelete.Enqueue(item);
                    ++m_Count;
                }
            }

            eable.Free();

            while (m_ToDelete.Count > 0)
            {
                ((Item)m_ToDelete.Dequeue()).Delete();
            }
        }

        [Usage("UOAMVendorsDelete")]
        [Description("Deletes vendor spawners from Data/Common.MAP (taken from UOAutoMap)")]
        private static void Generate_OnCommand(CommandEventArgs e)
        {
            Parse(e.Mobile);
        }

        private static void DeleteSpawner(string[] types, int x, int y, Map map)
        {
            if (types.Length == 0)
            {
                return;
            }

            int z = GetSpawnerZ(x, y, map);

            ClearSpawners(x, y, z, map);
        }
    }
}