using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Points;
using Server.Engines.Quests;

namespace Server.Engines.Khaldun
{
	public static class TreasuresOfKhaldunGeneration
	{
        public static void Initialize()
        {
            if (Core.EJ)
            {
                EventSink.WorldSave += OnWorldSave;

                if (!Siege.SiegeShard)
                {
                    KhaldunCampRegion.InstanceTram = new KhaldunCampRegion(Map.Trammel);
                }

                KhaldunCampRegion.InstanceFel = new KhaldunCampRegion(Map.Felucca);
            }
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            CheckEnabled(true);
        }

        public static void CheckEnabled(bool timed = false)
        {
            var khaldun = PointsSystem.Khaldun;

            if (khaldun.Enabled && !khaldun.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Disabling Treasures of Khaldun");

                        Remove();
                        khaldun.Enabled = false;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Treasures of Khaldun");

                    Remove();
                    khaldun.Enabled = false;
                }
            }
            else if (!khaldun.Enabled && khaldun.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Treasures of Khaldun");

                        Generate();
                        khaldun.Enabled = true;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Enabling Treasures of Khaldun");

                    Generate();
                    khaldun.Enabled = true;
                }

                if(!khaldun.QuestContentGenerated)
                {
                    GenerateQuestContent();
                    khaldun.QuestContentGenerated = true;
                }
            }
        }

        public static void Generate()
        {
            if (KhaldunResearcher.InstanceTram == null && !Siege.SiegeShard)
            {
                KhaldunResearcher.InstanceTram = new KhaldunResearcher();
                KhaldunResearcher.InstanceTram.MoveToWorld(new Point3D(6009, 3771, 21), Map.Trammel);
            }

            if (KhaldunResearcher.InstanceFel == null)
            {
                KhaldunResearcher.InstanceFel = new KhaldunResearcher();
                KhaldunResearcher.InstanceFel.MoveToWorld(new Point3D(6009, 3771, 21), Map.Felucca);
            }

            if (LeadInvestigator.InstanceTram == null && !Siege.SiegeShard)
            {
                LeadInvestigator.InstanceTram = new LeadInvestigator();
                LeadInvestigator.InstanceTram.MoveToWorld(new Point3D(6010, 3776, 19), Map.Trammel);
            }

            if (LeadInvestigator.InstanceFel == null)
            {
                LeadInvestigator.InstanceFel = new LeadInvestigator();
                LeadInvestigator.InstanceFel.MoveToWorld(new Point3D(6010, 3776, 19), Map.Felucca);
            }

            if (CaddelliteVendor.InstanceTram == null && !Siege.SiegeShard)
            {
                CaddelliteVendor.InstanceTram = new CaddelliteVendor();
                CaddelliteVendor.InstanceTram.MoveToWorld(new Point3D(6018, 3749, 21), Map.Trammel);
            }

            if (CaddelliteVendor.InstanceFel == null)
            {
                CaddelliteVendor.InstanceFel = new CaddelliteVendor();
                CaddelliteVendor.InstanceFel.MoveToWorld(new Point3D(6018, 3749, 21), Map.Felucca);
            }

            ChampionSpawn champ = null;

            if (!Siege.SiegeShard)
            {
                champ = new ChampionSpawn();
                champ.Type = ChampionSpawnType.Khaldun;
                champ.MoveToWorld(new Point3D(5469, 1461, 20), Map.Trammel);
                ChampionSystem.AllSpawns.Add(champ);
            }

            champ = new ChampionSpawn();
            champ.Type = ChampionSpawnType.Khaldun;
            champ.MoveToWorld(new Point3D(5469, 1461, 20), Map.Felucca);
            ChampionSystem.AllSpawns.Add(champ);
        }

        public static void Remove()
        {
            ChampionSystem.AllSpawns.Where(s => s.Type == ChampionSpawnType.Khaldun && Region.Find(s.Location, s.Map).IsPartOf("Khaldun")).IterateReverse(s =>
            {
                s.Delete();
            });
        }

        public static void GenerateQuestContent()
        {
            var map = Siege.SiegeShard ? Map.Felucca : Map.Trammel;

            var addon = new KhaldunDecorationAddon();
            addon.MoveToWorld(new Point3D(6232, 2887, -1), map);

            // Britain
            var door = new TrapDoor("boreas", new Point3D(6242, 2892, 17), map);
            var book = new MysteriousBook(door);
            var dust = new DustPile(door);
            var teleporter = new Teleporter(new Point3D(1369, 1465, 10), map);

            door.MoveToWorld(new Point3D(1369, 1465, 10), map);
            book.MoveToWorld(new Point3D(6240, 2885, 8), map);
            dust.MoveToWorld(new Point3D(6256, 2889, 13), map);
            teleporter.MoveToWorld(new Point3D(6242, 2892, 17), map);

            new GumshoeBottles().MoveToWorld(new Point3D(6154, 2901, 6), map);
            new GumshoeBottles().MoveToWorld(new Point3D(6154, 2902, 6), map);

            new GumshoeDeed().MoveToWorld(new Point3D(6161, 2901, 6), map);

            new GumshoeRope().MoveToWorld(new Point3D(6163, 2896, 0), map);
            new GumshoeRope().MoveToWorld(new Point3D(6163, 2896, 1), map);

            new GumshoeMap().MoveToWorld(new Point3D(6166, 2895, 6), map);
            new GumshoeMap().MoveToWorld(new Point3D(6166, 2895, 7), map);

            new GumshoeTools().MoveToWorld(new Point3D(6160, 2901, 6), map);

            // Moonglow
            door = new TrapDoor("carthax", new Point3D(6198, 2893, 17), map);
            book = new MysteriousBook(door);
            dust = new DustPile(door);
            teleporter = new Teleporter(new Point3D(4550, 1306, 8), map);

            door.MoveToWorld(new Point3D(4550, 1306, 8), map);
            book.MoveToWorld(new Point3D(6207, 2884, 7), map);
            dust.MoveToWorld(new Point3D(6208, 2885, 12), map);
            teleporter.MoveToWorld(new Point3D(6198, 2893, 17), map);

            new GumshoeBottles().MoveToWorld(new Point3D(6198, 2888, 6), map);

            new GumshoeRope().MoveToWorld(new Point3D(6200, 2887, 0), map);
            new GumshoeRope().MoveToWorld(new Point3D(6200, 2887, 1), map);

            new GumshoeMap().MoveToWorld(new Point3D(6198, 2887, 6), map);
            new GumshoeMap().MoveToWorld(new Point3D(6198, 2887, 7), map);

            new GumshoeTools().MoveToWorld(new Point3D(6198, 2889, 6), map);

            // Vesper
            door = new TrapDoor("moriens", new Point3D(6154, 2898, 17), map);
            book = new MysteriousBook(door);
            dust = new DustPile(door);
            teleporter = new Teleporter(new Point3D(2762, 848, 0), map);

            door.MoveToWorld(new Point3D(2762, 848, 0), map);
            book.MoveToWorld(new Point3D(6167, 2896, 6), map);
            dust.MoveToWorld(new Point3D(6163, 2885, 0), map);
            teleporter.MoveToWorld(new Point3D(6154, 2898, 17), map);

            new GumshoeBottles().MoveToWorld(new Point3D(6240, 2884, 6), map);
            new GumshoeBottles().MoveToWorld(new Point3D(6239, 2885, 6), map);

            new GumshoeRope().MoveToWorld(new Point3D(6241, 2884, 0), map);
            new GumshoeRope().MoveToWorld(new Point3D(6241, 2884, 1), map);

            new GumshoeMap().MoveToWorld(new Point3D(6240, 2885, 6), map);
            new GumshoeMap().MoveToWorld(new Point3D(6240, 2885, 7), map);

            new GumshoeTools().MoveToWorld(new Point3D(6239, 2886, 6), map);

            // Yew
            door = new TrapDoor("tenebrae", new Point3D(6294, 2891, 17), map);
            book = new MysteriousBook(door);
            dust = new DustPile(door);
            teleporter = new Teleporter(new Point3D(712, 1104, 0), map);

            door.MoveToWorld(new Point3D(712, 1104, 0), map);
            book.MoveToWorld(new Point3D(6294, 2887, 6), map);
            dust.MoveToWorld(new Point3D(6291, 2875, 9), map);
            teleporter.MoveToWorld(new Point3D(6294, 2891, 17), map);

            new GumshoeBottles().MoveToWorld(new Point3D(6303, 2887, 6), map);
            new GumshoeBottles().MoveToWorld(new Point3D(6304, 2887, 6), map);

            new GumshoeRope().MoveToWorld(new Point3D(6299, 2887, 0), map);
            new GumshoeRope().MoveToWorld(new Point3D(6299, 2887, 1), map);

            new GumshoeMap().MoveToWorld(new Point3D(6294, 2888, 6), map);
            new GumshoeMap().MoveToWorld(new Point3D(6294, 2888, 7), map);

            new GumshoeTools().MoveToWorld(new Point3D(6294, 2889, 6), map);

            // Gravestones
            var grave = new DamagedHeadstone(1158607); // brit
            grave.MoveToWorld(new Point3D(1378, 1445, 10), map);

            grave = new DamagedHeadstone(1158608); // vesper
            grave.ItemID = 4477;
            grave.MoveToWorld(new Point3D(2747, 882, 0), map);

            grave = new DamagedHeadstone(1158609); // moonglow
            grave.MoveToWorld(new Point3D(4545, 1316, 8), map);

            grave = new DamagedHeadstone(1158610); // yew
            grave.MoveToWorld(new Point3D(723, 1104, 0), map);

            // footprints
            var footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1452, 10), map);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1456, 10), map);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1461, 10), map);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1464, 10), map);

            footprints = new BloodyFootPrints(0x1E03);
            footprints.MoveToWorld(new Point3D(1378, 1464, 10), map);

            var st = new Static(0x2006);
            st.Stackable = true;
            st.Amount = 0x191;
            st.Hue = 0x47E;
            st.MoveToWorld(new Point3D(5808, 3270, -15), map);
            st.Name = "A Corpse of Liane";

            st = new Static(0x2006);
            st.Stackable = true;
            st.Amount = 86;
            st.Hue = 0x47E;
            st.MoveToWorld(new Point3D(5807, 3268, -15), map);
            st.Name = "A Corpse of an Ophidian Beserker";
        }
    }
}
