using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Engines.SeasonalEvents;

using System.Linq;

namespace Server.Engines.Khaldun
{
    public class TreasuresOfKhaldunEvent : SeasonalEvent
    {
        public static TreasuresOfKhaldunEvent Instance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool QuestContentGenerated { get; set; }

        public TreasuresOfKhaldunEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public TreasuresOfKhaldunEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        public override void CheckEnabled()
        {
            base.CheckEnabled();

            if (Running && IsActive() && !QuestContentGenerated)
            {
                GenerateQuestContent();
                QuestContentGenerated = true;
            }
        }

        protected override void Generate()
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
                champ = new ChampionSpawn
                {
                    Type = ChampionSpawnType.Khaldun
                };
                champ.MoveToWorld(new Point3D(5469, 1461, 20), Map.Trammel);
                ChampionSystem.AllSpawns.Add(champ);
            }

            champ = new ChampionSpawn
            {
                Type = ChampionSpawnType.Khaldun
            };
            champ.MoveToWorld(new Point3D(5469, 1461, 20), Map.Felucca);
            ChampionSystem.AllSpawns.Add(champ);

            if (ChestSpawner.InstanceFel == null)
            {
                ChestSpawner.InstanceFel = new ChestSpawner();
            }

            ChestSpawner.InstanceFel.CheckChests();

            if (!Siege.SiegeShard)
            {
                if (ChestSpawner.InstanceTram == null)
                {
                    ChestSpawner.InstanceTram = new ChestSpawner();
                }

                ChestSpawner.InstanceTram.CheckChests();
            }
        }

        protected override void Remove()
        {
            ChampionSystem.AllSpawns.Where(s => s.Type == ChampionSpawnType.Khaldun && Region.Find(s.Location, s.Map).IsPartOf("Khaldun")).IterateReverse(s =>
            {
                s.Delete();
            });

            if (ChestSpawner.InstanceFel != null)
            {
                if (ChestSpawner.InstanceFel.Chests != null)
                {
                    ColUtility.SafeDelete(ChestSpawner.InstanceFel.Chests);
                }

                ChestSpawner.InstanceFel = null;
            }

            if (ChestSpawner.InstanceTram != null)
            {
                if (ChestSpawner.InstanceTram.Chests != null)
                {
                    ColUtility.SafeDelete(ChestSpawner.InstanceTram.Chests);
                }

                ChestSpawner.InstanceTram = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(QuestContentGenerated);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = InheritInsertion ? 0 : reader.ReadInt();

            switch (v)
            {
                case 1:
                    QuestContentGenerated = reader.ReadBool();
                    break;
            }
        }

        public static void GenerateQuestContent()
        {
            Map map = Siege.SiegeShard ? Map.Felucca : Map.Trammel;

            KhaldunDecorationAddon addon = new KhaldunDecorationAddon();
            addon.MoveToWorld(new Point3D(6232, 2887, -1), map);

            // Britain
            TrapDoor door = new TrapDoor("boreas", new Point3D(6242, 2892, 17), map);
            MysteriousBook book = new MysteriousBook(door);
            DustPile dust = new DustPile(door);
            Teleporter teleporter = new Teleporter(new Point3D(1369, 1465, 10), map);

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
            DamagedHeadstone grave = new DamagedHeadstone(1158607); // brit
            grave.MoveToWorld(new Point3D(1378, 1445, 10), map);

            grave = new DamagedHeadstone(1158608)
            {
                ItemID = 4477
            }; // vesper
            grave.MoveToWorld(new Point3D(2747, 882, 0), map);

            grave = new DamagedHeadstone(1158609); // moonglow
            grave.MoveToWorld(new Point3D(4545, 1316, 8), map);

            grave = new DamagedHeadstone(1158610); // yew
            grave.MoveToWorld(new Point3D(723, 1104, 0), map);

            // footprints
            BloodyFootPrints footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1452, 10), map);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1456, 10), map);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1461, 10), map);

            footprints = new BloodyFootPrints(0x1E06);
            footprints.MoveToWorld(new Point3D(1383, 1464, 10), map);

            footprints = new BloodyFootPrints(0x1E03);
            footprints.MoveToWorld(new Point3D(1378, 1464, 10), map);

            Static st = new Static(0x2006)
            {
                Stackable = true,
                Amount = 0x191,
                Hue = 0x47E
            };
            st.MoveToWorld(new Point3D(5808, 3270, -15), map);
            st.Name = "A Corpse of Liane";

            st = new Static(0x2006)
            {
                Stackable = true,
                Amount = 86,
                Hue = 0x47E
            };
            st.MoveToWorld(new Point3D(5807, 3268, -15), map);
            st.Name = "A Corpse of an Ophidian Beserker";
        }

        public static void Initialize()
        {
            if (!Siege.SiegeShard)
            {
                KhaldunCampRegion.InstanceTram = new KhaldunCampRegion(Map.Trammel);
            }

            KhaldunCampRegion.InstanceFel = new KhaldunCampRegion(Map.Felucca);

            if (Instance != null && Instance.Running)
            {
                if (ChestSpawner.InstanceFel == null)
                {
                    ChestSpawner.InstanceFel = new ChestSpawner();
                }

                ChestSpawner.InstanceFel.CheckChests();

                if (!Siege.SiegeShard)
                {
                    if (ChestSpawner.InstanceTram == null)
                    {
                        ChestSpawner.InstanceTram = new ChestSpawner();
                    }

                    ChestSpawner.InstanceTram.CheckChests();
                }
            }
            else
            {
                if (ChestSpawner.InstanceFel != null)
                {
                    if (ChestSpawner.InstanceFel.Chests != null)
                    {
                        ColUtility.SafeDelete(ChestSpawner.InstanceFel.Chests);
                    }

                    ChestSpawner.InstanceFel = null;
                }

                if (ChestSpawner.InstanceTram != null)
                {
                    if (ChestSpawner.InstanceTram.Chests != null)
                    {
                        ColUtility.SafeDelete(ChestSpawner.InstanceTram.Chests);
                    }

                    ChestSpawner.InstanceTram = null;
                }
            }
        }
    }
}
