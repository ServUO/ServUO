using Server.Items;
using Server.Engines.SeasonalEvents;
using Server.Mobiles;

using System;

namespace Server.Engines.JollyRoger
{
    public class JollyRogerEvent : SeasonalEvent
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool QuestContentGenerated { get; set; }

        public static JollyRogerEvent Instance { get; set; }

        public JollyRogerEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public JollyRogerEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
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

        public static void GenerateQuestContent()
        {
            Item item;
            Static st;
            Map map = Map.Ilshenar;

            # region Well of Souls Decorations
            if (map.FindItem<Teleporter>(new Point3D(1528, 1341, -3)) == null)
            {
                item = new Teleporter(new Point3D(1528, 1341, -3), map);
                item.MoveToWorld(new Point3D(2264, 1574, -28), map);
            }            

            if (map.FindItem<WOSAnkhOfSacrifice>(new Point3D(2263, 1549, -28)) == null)
            {
                item = new WOSAnkhOfSacrifice();
                item.MoveToWorld(new Point3D(2263, 1549, -28), map);
            }

            if (map.FindItem<Static>(new Point3D(2263, 1553, -28)) == null)
            {
                st = new Static(0x18dc)
                {
                    Hue = 2401,
                    Name = "Essence Of A Nascent Time Gate",
                    Weight = 0
                };
                st.MoveToWorld(new Point3D(2263, 1553, -28), map);
            }
            #endregion

            if (!Siege.SiegeShard)
            {
                map = Map.Trammel;

                #region Trammel Iver's Rounding Decorations */

                if (IversRoundingAddon.InstanceTram == null)
                {
                    IversRoundingAddon.InstanceTram = new IversRoundingAddon();
                    IversRoundingAddon.InstanceTram.MoveToWorld(new Point3D(449, 2083, -5), map);
                }

                if (map.FindItem<IRTeleporter>(new Point3D(450, 2083, 34)) == null)
                {
                    item = new IRTeleporter();
                    item.MoveToWorld(new Point3D(450, 2083, 34), map);
                }
                #endregion
            }

            map = Map.Felucca;

            #region Felucca Iver's Rounding Decorations
            if (IversRoundingAddon.InstanceFel == null)
            {
                IversRoundingAddon.InstanceFel = new IversRoundingAddon();
                IversRoundingAddon.InstanceFel.MoveToWorld(new Point3D(449, 2083, -5), map);
            }

            if (map.FindItem<IRTeleporter>(new Point3D(450, 2083, 34)) == null)
            {
                item = new IRTeleporter();
                item.MoveToWorld(new Point3D(450, 2083, 34), map);
            }
            #endregion
        }

        #region Generate/Remove decoration
        protected override void Generate()
        {
            BaseMulti shipwreck;
            Item item;
            XmlSpawner sp;
            Static st;
            DarkWoodDoor door;
            Map map = Map.Ilshenar;

            XmlSpawner.SpawnObject[] so = new XmlSpawner.SpawnObject[Ghost.Length];

            for (int i = 0; i < Ghost.Length; i++)
            {
                so[i] = new XmlSpawner.SpawnObject(Ghost[i], 1);
            }

            #region Well of Souls Mobile
            if (HawkwindTimeLord.Instance == null)
            {
                HawkwindTimeLord.Instance = new HawkwindTimeLord();
                HawkwindTimeLord.Instance.MoveToWorld(new Point3D(2263, 1554, -28), map);
            }

            if (HawkwindSpeak.Instance == null)
            {
                HawkwindSpeak.Instance = new HawkwindSpeak();
                HawkwindSpeak.Instance.MoveToWorld(new Point3D(2267, 1563, -28), map);
            }
            #endregion

            #region Castle Sallé Dacil Decorations
            if (map.FindItem<CastleTrapDoor>(new Point3D(1316, 231, -26)) == null)
            {
                item = new CastleTrapDoor(new Point3D(982, 1126, 65), map, false);
                item.MoveToWorld(new Point3D(1316, 231, -26), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<CastleTrapDoor>(new Point3D(982, 1126, 65)) == null)
            {
                item = new CastleTrapDoor(new Point3D(1316, 231, -26), Map.Ilshenar, true);
                item.MoveToWorld(new Point3D(982, 1126, 65), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (CastleAddon.Instance == null)
            {
                CastleAddon.Instance = new CastleAddon();
                CastleAddon.Instance.MoveToWorld(new Point3D(994, 1140, 43), map);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(981, 1131, 65)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.WestCCW);
                door.MoveToWorld(new Point3D(981, 1131, 65), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(990, 1134, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.SouthCCW);
                door.MoveToWorld(new Point3D(990, 1134, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(990, 1133, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.NorthCW);
                door.MoveToWorld(new Point3D(990, 1133, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(998, 1131, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.WestCW);
                door.MoveToWorld(new Point3D(998, 1131, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1012, 1131, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.WestCCW);
                door.MoveToWorld(new Point3D(1012, 1131, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1004, 1136, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.EastCCW);
                door.MoveToWorld(new Point3D(1004, 1136, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1003, 1136, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.WestCW);
                door.MoveToWorld(new Point3D(1003, 1136, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1003, 1141, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.WestCCW);
                door.MoveToWorld(new Point3D(1003, 1141, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1004, 1141, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.EastCW);
                door.MoveToWorld(new Point3D(1004, 1141, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(990, 1154, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.SouthCCW);
                door.MoveToWorld(new Point3D(990, 1154, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(990, 1153, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.NorthCW);
                door.MoveToWorld(new Point3D(990, 1153, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(995, 1153, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.NorthCCW);
                door.MoveToWorld(new Point3D(995, 1153, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(995, 1154, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.SouthCW);
                door.MoveToWorld(new Point3D(995, 1154, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1001, 1154, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.SouthCCW);
                door.MoveToWorld(new Point3D(1001, 1154, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1001, 1153, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.NorthCW);
                door.MoveToWorld(new Point3D(1001, 1153, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1006, 1154, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.NorthCCW);
                door.MoveToWorld(new Point3D(1006, 1154, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }

            if (map.FindItem<DarkWoodDoor>(new Point3D(1006, 1155, 48)) == null)
            {
                door = new DarkWoodDoor(DoorFacing.SouthCW);
                door.MoveToWorld(new Point3D(1006, 1155, 48), map);
                WeakEntityCollection.Add(EntityName, door);
            }
            #endregion

            if (!Siege.SiegeShard)
            {
                map = Map.Trammel;

                #region Trammel Admiral Jack's Shipwreck Decorations
                if (AdmiralJacksShipwreckAddon.InstanceTram == null)
                {
                    AdmiralJacksShipwreckAddon.InstanceTram = new AdmiralJacksShipwreckAddon();
                    AdmiralJacksShipwreckAddon.InstanceTram.MoveToWorld(new Point3D(4269, 560, -14), map);
                }

                if (map.FindItem<BaseMulti>(new Point3D(4268, 568, 0)) == null)
                {
                    shipwreck = new BaseMulti(33);
                    shipwreck.MoveToWorld(new Point3D(4268, 568, 0), map);
                    WeakEntityCollection.Add(EntityName, shipwreck);
                }

                if (map.FindItem<ShipwreckBook>(new Point3D(4266, 572, 0)) == null)
                {
                    item = new ShipwreckBook();
                    item.MoveToWorld(new Point3D(4266, 572, 0), map);
                    WeakEntityCollection.Add(EntityName, item);
                }

                if (JackCorpse.InstanceTram == null)
                {
                    JackCorpse.InstanceTram = new JackCorpse();
                    JackCorpse.InstanceTram.MoveToWorld(new Point3D(4267, 574, 0), map);
                }
                #endregion

                #region Trammel Iver's Rounding Spawn
                if (Shamino.InstanceTram == null)
                {
                    Shamino.InstanceTram = new Shamino();
                    Shamino.InstanceTram.MoveToWorld(new Point3D(450, 2082, 34), map);
                }

                if (map.FindItem<XmlSpawner>(new Point3D(468, 2091, 7)) == null)
                {
                    sp = new XmlSpawner(Guid.NewGuid(), 0, 0, 0, 0, "#JollyRogerGhost", 5, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(0), -1, 0x1F4, 1, 0, 10, false, so, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(0), null, null, null, null, null, null, null, null, null, 1, null, false, XmlSpawner.TODModeType.Realtime, 1, false, -1, null, false, false, false, null, TimeSpan.FromHours(0), null, false, null);
                    WeakEntityCollection.Add(EntityName, sp);
                    sp.SpawnRange = 15;
                    sp.MoveToWorld(new Point3D(468, 2091, 7), map);
                    sp.Respawn();
                }
                #endregion

                #region Trammel Castle British Decorations
                for (int i = 0; i < SherryTheMouse.LuteLocations.Length; i++)
                {
                    Point3D p = SherryTheMouse.LuteLocations[i];

                    if (map.FindItem<Static>(new Point3D(p.X, p.Y, 72)) == null)
                    {
                        st = new Static(0xEBB);
                        st.MoveToWorld(new Point3D(p.X, p.Y, 72), map);
                        WeakEntityCollection.Add(EntityName, st);
                    }
                }

                if (map.FindItem<Static>(new Point3D(1347, 1642, 72)) == null)
                {
                    st = new Static(0x118F);
                    st.MoveToWorld(new Point3D(1347, 1642, 72), map);
                    WeakEntityCollection.Add(EntityName, st);
                }

                if (SherryTheMouse.InstanceTram == null)
                {
                    SherryTheMouse.InstanceTram = new SherryTheMouse();
                    SherryTheMouse.InstanceTram.MoveToWorld(new Point3D(1347, 1644, 72), map);
                }
                #endregion

                ShrineBattleGenerate(map);
            }

            map = Map.Felucca;

            #region Felucca Admiral Jack's Shipwreck Decorations
            if (AdmiralJacksShipwreckAddon.InstanceFel == null)
            {
                AdmiralJacksShipwreckAddon.InstanceFel = new AdmiralJacksShipwreckAddon();
                AdmiralJacksShipwreckAddon.InstanceFel.MoveToWorld(new Point3D(4269, 560, -14), map);
            }

            if (map.FindItem<BaseMulti>(new Point3D(4268, 568, 0)) == null)
            {
                shipwreck = new BaseMulti(33);
                shipwreck.MoveToWorld(new Point3D(4268, 568, 0), map);
                WeakEntityCollection.Add(EntityName, shipwreck);
            }

            if (map.FindItem<ShipwreckBook>(new Point3D(4266, 572, 0)) == null)
            {
                item = new ShipwreckBook();
                item.MoveToWorld(new Point3D(4266, 572, 0), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (JackCorpse.InstanceFel == null)
            {
                JackCorpse.InstanceFel = new JackCorpse();
                JackCorpse.InstanceFel.MoveToWorld(new Point3D(4267, 574, 0), map);
            }
            #endregion

            #region Felucca Iver's Rounding Spawn
            if (Shamino.InstanceFel == null)
            {
                Shamino.InstanceFel = new Shamino();
                Shamino.InstanceFel.MoveToWorld(new Point3D(450, 2082, 34), map);
            }

            if (map.FindItem<XmlSpawner>(new Point3D(468, 2091, 7)) == null)
            {
                sp = new XmlSpawner(Guid.NewGuid(), 0, 0, 0, 0, "#JollyRogerGhost", 5, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(0), -1, 0x1F4, 1, 0, 10, false, so, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(0), null, null, null, null, null, null, null, null, null, 1, null, false, XmlSpawner.TODModeType.Realtime, 1, false, -1, null, false, false, false, null, TimeSpan.FromHours(0), null, false, null);
                WeakEntityCollection.Add(EntityName, sp);
                sp.SpawnRange = 15;
                sp.MoveToWorld(new Point3D(468, 2091, 7), map);
                sp.Respawn();
            }
            #endregion

            #region Felucca Castle British Decorations
            for (int i = 0; i < SherryTheMouse.LuteLocations.Length; i++)
            {
                Point3D p = SherryTheMouse.LuteLocations[i];

                if (map.FindItem<Static>(new Point3D(p.X, p.Y, 72)) == null)
                {
                    st = new Static(0xEBB);
                    st.MoveToWorld(new Point3D(p.X, p.Y, 72), map);
                    WeakEntityCollection.Add(EntityName, st);
                }
            }

            if (map.FindItem<Static>(new Point3D(1347, 1642, 72)) == null)
            {
                st = new Static(0x118F);
                st.MoveToWorld(new Point3D(1347, 1642, 72), map);
                WeakEntityCollection.Add(EntityName, st);
            }

            if (SherryTheMouse.InstanceFel == null)
            {
                SherryTheMouse.InstanceFel = new SherryTheMouse();
                SherryTheMouse.InstanceFel.MoveToWorld(new Point3D(1347, 1644, 72), map);
            }
            #endregion

            ShrineBattleGenerate(map);
        }

        protected override void Remove()
        {
            RemoveDecoration();
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

        public static void RemoveDecoration()
        {
            WeakEntityCollection.Delete(EntityName);

            /* Trammel Remove */

            if (AdmiralJacksShipwreckAddon.InstanceTram != null)
            {
                AdmiralJacksShipwreckAddon.InstanceTram.Delete();
                AdmiralJacksShipwreckAddon.InstanceTram = null;
            }

            if (JackCorpse.InstanceTram != null)
            {
                JackCorpse.InstanceTram.Delete();
                JackCorpse.InstanceTram = null;
            }

            if (Shamino.InstanceTram != null)
            {
                Shamino.InstanceTram.Delete();
                Shamino.InstanceTram = null;
            }

            if (SherryTheMouse.InstanceTram != null)
            {
                SherryTheMouse.InstanceTram.Delete();
                SherryTheMouse.InstanceTram = null;
            }

            /* Felucca Remove */

            if (AdmiralJacksShipwreckAddon.InstanceFel != null)
            {
                AdmiralJacksShipwreckAddon.InstanceFel.Delete();
                AdmiralJacksShipwreckAddon.InstanceFel = null;
            }

            if (JackCorpse.InstanceFel != null)
            {
                JackCorpse.InstanceFel.Delete();
                JackCorpse.InstanceFel = null;
            }

            if (Shamino.InstanceFel != null)
            {
                Shamino.InstanceFel.Delete();
                Shamino.InstanceFel = null;
            }

            if (SherryTheMouse.InstanceFel != null)
            {
                SherryTheMouse.InstanceFel.Delete();
                SherryTheMouse.InstanceFel = null;
            }

            /* Ilshenar Remove */
            if (CastleAddon.Instance != null)
            {
                CastleAddon.Instance.Delete();
                CastleAddon.Instance = null;
            }            

            if (HawkwindSpeak.Instance != null)
            {
                HawkwindSpeak.Instance.Delete();
                HawkwindSpeak.Instance = null;
            }

            if (HawkwindTimeLord.Instance != null)
            {
                HawkwindTimeLord.Instance.Delete();
                HawkwindTimeLord.Instance = null;
            }
        }

        #endregion

        private static string[] Ghost =
        {
            "Ghost,One",
            "Ghost,Two",
            "Ghost,Three",
            "Ghost,Four",
            "Ghost,Five",
        };

        public static readonly string EntityName = "JollyRoger";

        public static void ShrineBattleGenerate(Map map)
        {
            Item item;

            if (map.FindItem<ShrineBattleController>(new Point3D(1859, 878, -1)) == null)
            {
                item = new ShrineBattleController(Shrine.Compassion);
                item.MoveToWorld(new Point3D(1859, 878, -1), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(4211, 564, 65)) == null)
            {
                item = new ShrineBattleController(Shrine.Honesty);
                item.MoveToWorld(new Point3D(4211, 564, 65), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(1728, 3531, 3)) == null)
            {
                item = new ShrineBattleController(Shrine.Honor);
                item.MoveToWorld(new Point3D(1728, 3531, 3), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(4272, 3692, 0)) == null)
            {
                item = new ShrineBattleController(Shrine.Humility);
                item.MoveToWorld(new Point3D(4272, 3692, 0), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(1298, 629, 16)) == null)
            {
                item = new ShrineBattleController(Shrine.Justice);
                item.MoveToWorld(new Point3D(1298, 629, 16), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(3352, 287, 4)) == null)
            {
                item = new ShrineBattleController(Shrine.Sacrifice);
                item.MoveToWorld(new Point3D(3352, 287, 4), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(1600, 2488, 5)) == null)
            {
                item = new ShrineBattleController(Shrine.Spirituality);
                item.MoveToWorld(new Point3D(1600, 2488, 5), map);
                WeakEntityCollection.Add(EntityName, item);
            }

            if (map.FindItem<ShrineBattleController>(new Point3D(2489, 3927, 5)) == null)
            {
                item = new ShrineBattleController(Shrine.Valor);
                item.MoveToWorld(new Point3D(2489, 3927, 5), map);
                WeakEntityCollection.Add(EntityName, item);
            }
        }
    }
}
