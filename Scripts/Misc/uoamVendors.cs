using System;
using System.Collections;
using System.IO;
using Server.Commands;
using Server.Mobiles;

// Version 0.8
namespace Server
{
    public class UOAMVendorGenerator
    {
        private static readonly TimeSpan MinTime = TimeSpan.FromMinutes(2.5);//min spawn time
        private static readonly TimeSpan MaxTime = TimeSpan.FromMinutes(10.0);//max spawn time
        private static readonly Queue m_ToDelete = new Queue();
        private static int m_Count;
        //configuration
        private const int NPCCount = 2;//2 npcs per type (so a mage spawner will spawn 2 npcs, a alchemist and herbalist spawner will spawn 4 npcs total)
        private const int HomeRange = 5;//How far should they wander?
        private const bool TotalRespawn = true;//Should we spawn them up right away?
        private const int Team = 0;//"team" the npcs are on
        public static void Initialize()
        {
            CommandSystem.Register("UOAMVendors", AccessLevel.Administrator, new CommandEventHandler(Generate_OnCommand));
        }

        public static void Parse(Mobile from)
        {
            string vendor_path = Path.Combine(Core.BaseDirectory, "Data/Common.map");
            m_Count = 0;

            if (File.Exists(vendor_path))
            {
                ArrayList list = new ArrayList();
                from.SendMessage("Generating Vendors...");

                using (StreamReader ip = new StreamReader(vendor_path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        int indexOf = line.IndexOf(':');

                        if (indexOf == -1)
                            continue;

                        string type = line.Substring(0, ++indexOf).Trim();
                        string sub = line.Substring(indexOf).Trim();

                        string[] split = sub.Split(' ');

                        if (split.Length < 3)
                            continue;

                        split = new string[] { type, split[0], split[1], split[2] };

                        switch(split[0].ToLower()) 
                        {
                            case "-healer:":
                                PlaceNPC(split[1], split[2], split[3], "Healer", "HealerGuildmaster");
                                break;
                            case "-baker:":
                                PlaceNPC(split[1], split[2], split[3], "Baker");
                                break;
                            case "-vet:":
                                PlaceNPC(split[1], split[2], split[3], "Veterinarian");
                                break;
                            case "-gypsymaiden:":
                                PlaceNPC(split[1], split[2], split[3], "GypsyMaiden");
                                break;
                            case "-gypsybank:":
                                PlaceNPC(split[1], split[2], split[3], "GypsyBanker");
                                break;
                            case "-bank:":
                                PlaceNPC(split[1], split[2], split[3], "Banker", "Minter");
                                break;
                            case "-inn:":
                                PlaceNPC(split[1], split[2], split[3], "Innkeeper");
                                break;
                            case "-provisioner:":
                                PlaceNPC(split[1], split[2], split[3], "Provisioner", "Cobbler");
                                break;
                            case "-tailor:":
                                PlaceNPC(split[1], split[2], split[3], "Tailor", "Weaver", "TailorGuildmaster");
                                break;
                            case "-tavern:":
                                PlaceNPC(split[1], split[2], split[3], "Tavernkeeper", "Waiter", "Cook", "Barkeeper");
                                break;
                            case "-reagents:":
                                PlaceNPC(split[1], split[2], split[3], "Herbalist", "Alchemist", "CustomHairstylist");
                                break;
                            case "-fortuneteller:":
                                PlaceNPC(split[1], split[2], split[3], "FortuneTeller");
                                break;
                            case "-holymage:":
                                PlaceNPC(split[1], split[2], split[3], "HolyMage");
                                break;
                            case "-chivalrykeeper:":
                                PlaceNPC(split[1], split[2], split[3], "KeeperOfChivalry");
                                break;
                            case "-mage:":
                                PlaceNPC(split[1], split[2], split[3], "Mage", "Alchemist", "MageGuildmaster");
                                break;
                            case "-arms:":
                                PlaceNPC(split[1], split[2], split[3], "Armorer", "Weaponsmith");
                                break;
                            case "-tinker:":
                                PlaceNPC(split[1], split[2], split[3], "Tinker", "TinkerGuildmaster");
                                break;
                            case "-gypsystable:":
                                PlaceNPC(split[1], split[2], split[3], "GypsyAnimalTrainer");
                                break;
                            case "-stable:":
                                PlaceNPC(split[1], split[2], split[3], "AnimalTrainer");
                                break;
                            case "-blacksmith:":
                                PlaceNPC(split[1], split[2], split[3], "Blacksmith", "BlacksmithGuildmaster");
                                break;
                            case "-bowyer:":
                            case "-fletcher:":
                                PlaceNPC(split[1], split[2], split[3], "Bowyer");
                                break;
                            case "-carpenter:":
                                PlaceNPC(split[1], split[2], split[3], "Carpenter", "Architect", "RealEstateBroker");
                                break;
                            case "-butcher:":
                                PlaceNPC(split[1], split[2], split[3], "Butcher");
                                break;
                            case "-jeweler:":
                                PlaceNPC(split[1], split[2], split[3], "Jeweler");
                                break;
                            case "-tanner:":
                                PlaceNPC(split[1], split[2], split[3], "Tanner", "Furtrader");
                                break;
                            case "-bard:":
                                PlaceNPC(split[1], split[2], split[3], "Bard", "BardGuildmaster");
                                break;
                            case "-market:":
                                PlaceNPC(split[1], split[2], split[3], "Butcher", "Farmer");
                                break;
                            case "-library:":
                                PlaceNPC(split[1], split[2], split[3], "Scribe");
                                break;
                            case "-shipwright:":
                                PlaceNPC(split[1], split[2], split[3], "Shipwright", "Mapmaker");
                                break;
                            case "-docks:":
                                PlaceNPC(split[1], split[2], split[3], "Fisherman");
                                break;
                            case "-beekeeper:":
                                PlaceNPC(split[1], split[2], split[3], "Beekeeper");
                                break;
                                // Guilds & Misc
                            case "-tinkers guild:":
                                PlaceNPC(split[1], split[2], split[3], "TinkerGuildmaster");
                                break;
                            case "-blacksmiths guild:":
                                PlaceNPC(split[1], split[2], split[3], "BlacksmithGuildmaster");
                                break;
                            case "-sorcerors guild:":
                                PlaceNPC(split[1], split[2], split[3], "MageGuildmaster");
                                break;
                            case "-customs:":
                                break;
                            case "-painter:":
                                break;
                            case "-theater:":
                                break;
                            case "-warriors guild:":
                                PlaceNPC(split[1], split[2], split[3], "WarriorGuildmaster");
                                break;
                            case "-archers guild:":
                                PlaceNPC(split[1], split[2], split[3], "RangerGuildmaster");
                                break;
                            case "-thieves guild:":
                                PlaceNPC(split[1], split[2], split[3], "ThiefGuildmaster");
                                break;
                            case "-miners guild:":
                                PlaceNPC(split[1], split[2], split[3], "MinerGuildmaster");
                                break;
                            case "-fishermans guild:":
                                PlaceNPC(split[1], split[2], split[3], "FisherGuildmaster");
                                break;
                            case "-merchants guild:":
                                PlaceNPC(split[1], split[2], split[3], "MerchantGuildmaster");
                                break;
                            case "-illusionists guild:":
                                break;
                            case "-armourers guild:":
                                break;
                            case "-sorcerers guild:":
                                break;
                            case "-mages guild:":
                                PlaceNPC(split[1], split[2], split[3], "MageGuildmaster");
                                break;
                            case "-weapons guild:":
                                break;
                            case "-bardic guild:":
                                PlaceNPC(split[1], split[2], split[3], "BardGuildmaster");
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
                from.SendMessage("Done, added {0} spawners", m_Count);
            }
            else
            {
                from.SendMessage("{0} not found!", vendor_path);
            }
        }

        public static void PlaceNPC(string sx, string sy, string sm, params string[] types)
        {
            if (types.Length == 0)
                return;

            int x = Utility.ToInt32(sx);
            int y = Utility.ToInt32(sy);
            int map = Utility.ToInt32(sm);

            switch ( map )
            {
                case 0://Trammel and Felucca
                    MakeSpawner(types, x, y, Map.Felucca);
                    MakeSpawner(types, x, y, Map.Trammel);
                    break;
                case 1://Felucca
                    MakeSpawner(types, x, y, Map.Felucca);
                    break;
                case 2:
                    MakeSpawner(types, x, y, Map.Trammel);
                    break;
                case 3:
                    MakeSpawner(types, x, y, Map.Ilshenar);
                    break;
                case 4:
                    MakeSpawner(types, x, y, Map.Malas);
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
                return z;

            for (int i = 1; i <= 20; ++i)
            {
                if (map.CanFit(x, y, z + i, 16, false, false, true))
                    return z + i;

                if (map.CanFit(x, y, z - i, 16, false, false, true))
                    return z - i;
            }

            return z;
        }

        public static void ClearSpawners(int x, int y, int z, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, z), 0);

            foreach (Item item in eable)
            {
                if (item is Spawner && item.Z == z)
                    m_ToDelete.Enqueue(item);
            }

            eable.Free();

            while (m_ToDelete.Count > 0)
                ((Item)m_ToDelete.Dequeue()).Delete();
        }

        [Usage("UOAMVendors")]
        [Description("Generates vendor spawners from Data/Common.MAP (taken from UOAutoMap)")]
        private static void Generate_OnCommand(CommandEventArgs e)
        {
            Parse(e.Mobile);
        }

        private static void MakeSpawner(string[] types, int x, int y, Map map)
        {
            if (types.Length == 0)
                return;

            int z = GetSpawnerZ(x, y, map);

            ClearSpawners(x, y, z, map);

            for (int i = 0; i < types.Length; ++i)
            {
                bool isGuildmaster = (types[i].EndsWith("Guildmaster"));

                Spawner sp = new Spawner(types[i]);

                if (isGuildmaster)
                    sp.Count = 1;
                else
                    sp.Count = NPCCount;

                sp.MinDelay = MinTime;
                sp.MaxDelay = MaxTime;
                sp.Team = Team;
                sp.HomeRange = HomeRange;

                sp.MoveToWorld(new Point3D(x, y, z), map);

                if (TotalRespawn)
                {
                    sp.Respawn();
                    sp.BringToHome();
                }

                ++m_Count;
            }
        }
    }
}