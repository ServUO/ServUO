using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Points;
using Server.Engines.Quests;

namespace Server.Engines.Khaldun
{
	public static class TreasuresOfKhaldunGeneration
	{
        public static void Intialize()
        {
            EventSink.WorldSave += OnWorldSave;
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
            // TODO: Enable Champ Spawn?
            if (KhaldunResearcher.IntanceTram == null)
            {
                KhaldunResearcher.IntanceTram = new KhaudunResearcher();
                KhaldunResearcher.IntanceTram.MoveToWorld(new Point3D(6017, 3752, 20), Map.Trammel);
            }

            if (KhaldunResearcher.IntanceFel == null)
            {
                KhaldunResearcher.IntanceFel = new KhaudunResearcher();
                KhaldunResearcher.IntanceFel.MoveToWorld(new Point3D(6017, 3752, 20), Map.Trammel);
            }
        }

        public static void Remove()
        {
            // TODO: Disable champ spawn?
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

            var entAddon = new KhaldunEntranceAddon();
            ent.AddonMoveToWorld(new Point3D(6013, 3785, 18), Map.Trammel);

            var campAddon = new KhaldunCampAddon();
            campAddon.AddonMoveToWorld(new Point3D(6003, 3772, 24), Map.Trammel);

            var tele = new Teleporter();
        }
    }
}
