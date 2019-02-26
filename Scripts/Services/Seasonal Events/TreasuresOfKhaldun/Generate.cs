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

                KhaldunCampRegion.InstanceTram = new KhaldunCampRegion(Map.Trammel);
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
            if (KhaldunResearcher.InstanceTram == null)
            {
                KhaldunResearcher.InstanceTram = new KhaldunResearcher();
                KhaldunResearcher.InstanceTram.MoveToWorld(new Point3D(6009, 3771, 21), Map.Trammel);
            }

            if (KhaldunResearcher.InstanceFel == null)
            {
                KhaldunResearcher.InstanceFel = new KhaldunResearcher();
                KhaldunResearcher.InstanceFel.MoveToWorld(new Point3D(6009, 3771, 21), Map.Felucca);
            }

            if (LeadInvestigator.InstanceTram == null)
            {
                LeadInvestigator.InstanceTram = new LeadInvestigator();
                LeadInvestigator.InstanceTram.MoveToWorld(new Point3D(6010, 3776, 19), Map.Trammel);
            }

            if (LeadInvestigator.InstanceFel == null)
            {
                LeadInvestigator.InstanceFel = new LeadInvestigator();
                LeadInvestigator.InstanceFel.MoveToWorld(new Point3D(6010, 3776, 19), Map.Felucca);
            }

            if (CaddelliteVendor.InstanceTram == null)
            {
                CaddelliteVendor.InstanceTram = new CaddelliteVendor();
                CaddelliteVendor.InstanceTram.MoveToWorld(new Point3D(6018, 3749, 21), Map.Trammel);
            }

            if (CaddelliteVendor.InstanceFel == null)
            {
                CaddelliteVendor.InstanceFel = new CaddelliteVendor();
                CaddelliteVendor.InstanceFel.MoveToWorld(new Point3D(6018, 3749, 21), Map.Felucca);
            }

            var champ = new ChampionSpawn();
            champ.Type = ChampionSpawnType.Khaldun;
            champ.MoveToWorld(new Point3D(5469, 1461, 20), Map.Trammel);
            ChampionSystem.AllSpawns.Add(champ);

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
            var addon = new KhaldunDecorationAddon();
            addon.MoveToWorld(new Point3D(6232, 2887, -1), Map.Trammel);

            // Britain
            var door = new TrapDoor("boreas", new Point3D(6242, 2892, 17), Map.Trammel);
            var book = new MysteriousBook(door);
            var dust = new DustPile(door);
            var teleporter = new Teleporter(new Point3D(1369, 1465, 10), Map.Trammel);

            door.MoveToWorld(new Point3D(1369, 1465, 10), Map.Trammel);
            book.MoveToWorld(new Point3D(6240, 2885, 8), Map.Trammel);
            dust.MoveToWorld(new Point3D(6256, 2889, 13), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(6242, 2892, 17), Map.Trammel);

            // Moonglow
            door = new TrapDoor("carthax", new Point3D(6198, 2893, 17), Map.Trammel);
            book = new MysteriousBook(door);
            dust = new DustPile(door);
            teleporter = new Teleporter(new Point3D(4550, 1306, 8), Map.Trammel);

            door.MoveToWorld(new Point3D(4550, 1306, 8), Map.Trammel);
            book.MoveToWorld(new Point3D(6207, 2884, 7), Map.Trammel);
            dust.MoveToWorld(new Point3D(6208, 2885, 12), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(6198, 2893, 17), Map.Trammel);

            // Vesper
            door = new TrapDoor("moriens", new Point3D(6154, 2898, 17), Map.Trammel);
            book = new MysteriousBook(door);
            dust = new DustPile(door);
            teleporter = new Teleporter(new Point3D(2762, 848, 0), Map.Trammel);

            door.MoveToWorld(new Point3D(2762, 848, 0), Map.Trammel);
            book.MoveToWorld(new Point3D(6167, 2896, 6), Map.Trammel);
            dust.MoveToWorld(new Point3D(6163, 2886, 0), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(6154, 2898, 17), Map.Trammel);

            // Yew
            door = new TrapDoor("tenebrae", new Point3D(6294, 2891, 17), Map.Trammel);
            book = new MysteriousBook(door);
            dust = new DustPile(door);
            teleporter = new Teleporter(new Point3D(712, 1104, 0), Map.Trammel);

            door.MoveToWorld(new Point3D(712, 1104, 0), Map.Trammel);
            book.MoveToWorld(new Point3D(6294, 2887, 6), Map.Trammel);
            dust.MoveToWorld(new Point3D(6291, 2875, 9), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(6294, 2891, 17), Map.Trammel);

            // Gravestones
            var grave = new DamagedHeadstone(1158607); // brit
            grave.MoveToWorld(new Point3D(1378, 1445, 10), Map.Trammel);

            grave = new DamagedHeadstone(1158608); // vesper
            grave.ItemID = 4477;
            grave.MoveToWorld(new Point3D(2747, 882, 0), Map.Trammel);

            grave = new DamagedHeadstone(1158609); // moonglow
            grave.MoveToWorld(new Point3D(4545, 1316, 8), Map.Trammel);

            grave = new DamagedHeadstone(1158610); // yew
            grave.MoveToWorld(new Point3D(723, 1104, 0), Map.Trammel);

            // footprints
            var footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1452, 10), Map.Trammel);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1456, 10), Map.Trammel);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1461, 10), Map.Trammel);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1464, 10), Map.Trammel);

            footprints = new BloodyFootPrints(0x1E03);
            footprints.MoveToWorld(new Point3D(1378, 1464, 10), Map.Trammel);

            var st = new Static(0x2006);
            st.Stackable = true;
            st.Amount = 0x191;
            st.Hue = 0x47E;
            st.MoveToWorld(new Point3D(5808, 3270, -15), Map.Trammel);
            st.Name = "A Corpse of Liane";

            st = new Static(0x2006);
            st.Stackable = true;
            st.Amount = 86;
            st.Hue = 0x47E;
            st.MoveToWorld(new Point3D(5807, 3268, -15), Map.Trammel);
            st.Name = "A Corpse of an Ophidian Beserker";
        }
    }
}
