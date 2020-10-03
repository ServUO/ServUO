using Server.Engines.SeasonalEvents;
using Server.Commands;
using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class TreasuresOfKotlCityEvent : SeasonalEvent
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasGenerated { get; set; }

        public TreasuresOfKotlCityEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
        }

        public TreasuresOfKotlCityEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
        }

        protected override void Generate()
        {
            CommandSystem.Handle(null, CommandSystem.Prefix + "XmlLoad Spawns/TreasuresOfKotl.xml");

            Map map = Map.TerMur;

            if (!FindItem<KotlDoor>(new Point3D(610, 2319, 0), map))
            {
                KotlDoor door = new KotlDoor();
                door.MoveToWorld(new Point3D(610, 2319, 0), map);
            }

            if (!FindItem<WheelsOfTime>(new Point3D(595, 2289, 8), map))
            {
                KotlCityPuzzle puzzle = new KotlCityPuzzle();
                puzzle.MoveToWorld(new Point3D(595, 2289, 8), map);
            }

            if (KotlBattleSimulator.Instance == null)
            {
                KotlBattleSimulator simulator = new KotlBattleSimulator();
                simulator.MoveToWorld(new Point3D(545, 2272, 0), map);
            }

            if (!FindItem<WheelsOfTime>(new Point3D(607, 2323, 0), map))
            {
                WheelsOfTime wheels = new WheelsOfTime();
                wheels.MoveToWorld(new Point3D(607, 2323, 0), map);
            }

            if (!FindItem<EnergyTileAddon>(new Point3D(592, 2393, 0), map))
            {
                EnergyTileAddon tiles = new EnergyTileAddon(13, Direction.South);
                tiles.MoveToWorld(new Point3D(592, 2393, 0), map);
            }

            if (!FindItem<EnergyTileAddon>(new Point3D(600, 2393, 0), map))
            {
                EnergyTileAddon tiles = new EnergyTileAddon(13, Direction.South);
                tiles.MoveToWorld(new Point3D(600, 2393, 0), map);
            }

            if (!FindItem<EnergyTileAddon>(new Point3D(608, 2393, 0), map))
            {
                EnergyTileAddon tiles = new EnergyTileAddon(13, Direction.South);
                tiles.MoveToWorld(new Point3D(608, 2393, 0), map);
            }

            if (!FindItem<EnergyTileAddon>(new Point3D(616, 2393, 0), map))
            {
                EnergyTileAddon tiles = new EnergyTileAddon(13, Direction.South);
                tiles.MoveToWorld(new Point3D(616, 2393, 0), map);
            }

            if (!FindItem<EnergyTileAddon>(new Point3D(624, 2393, 0), map))
            {
                EnergyTileAddon tiles = new EnergyTileAddon(13, Direction.South);
                tiles.MoveToWorld(new Point3D(624, 2393, 0), map);
            }

            if (!HasGenerated)
            {
                GenTeleporters();
                GenStations();
                GenLOSBlockers();
                GenChests();

                HasGenerated = true;
            }

            if (Hal.Instance == null)
            {
                Hal hal = new Hal();
                hal.MoveToWorld(new Point3D(489, 1606, 40), map);
            }
        }

        private static bool FindItem<T>(Point3D p, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (var item in eable)
            {
                if (item.GetType() == typeof(T))
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        private static void GenTeleporters()
        {
            Map map = Map.TerMur;

            Teleporter tele = new Teleporter(new Point3D(542, 2478, 0), map);
            tele.MoveToWorld(new Point3D(492, 1613, 40), map);

            tele = new Teleporter(new Point3D(543, 2478, 0), map);
            tele.MoveToWorld(new Point3D(493, 1613, 40), map);

            tele = new Teleporter(new Point3D(542, 2479, 0), map);
            tele.MoveToWorld(new Point3D(492, 1614, 40), map);

            tele = new Teleporter(new Point3D(543, 2479, 0), map);
            tele.MoveToWorld(new Point3D(493, 1614, 40), map);

            tele = new Teleporter(new Point3D(492, 1613, 40), map);
            tele.MoveToWorld(new Point3D(542, 2478, 0), map);

            tele = new Teleporter(new Point3D(493, 1613, 40), map);
            tele.MoveToWorld(new Point3D(543, 2478, 0), map);

            tele = new Teleporter(new Point3D(492, 1614, 40), map);
            tele.MoveToWorld(new Point3D(542, 2479, 0), map);

            tele = new Teleporter(new Point3D(493, 1614, 40), map);
            tele.MoveToWorld(new Point3D(543, 2479, 0), map);

            tele = new Teleporter(new Point3D(542, 2478, 0), map);
            tele.MoveToWorld(new Point3D(643, 2307, 0), map);

            tele = new Teleporter(new Point3D(543, 2478, 0), map);
            tele.MoveToWorld(new Point3D(644, 2307, 0), map);

            tele = new Teleporter(new Point3D(542, 2479, 0), map);
            tele.MoveToWorld(new Point3D(643, 2308, 0), map);

            tele = new Teleporter(new Point3D(543, 2479, 2), map);
            tele.MoveToWorld(new Point3D(644, 2308, 0), map);

            tele = new Teleporter(new Point3D(631, 2423, -20), map);
            tele.MoveToWorld(new Point3D(638, 2399, 0), map);

            tele = new Teleporter(new Point3D(632, 2423, -20), map);
            tele.MoveToWorld(new Point3D(639, 2399, 0), map);

            tele = new Teleporter(new Point3D(631, 2424, -20), map);
            tele.MoveToWorld(new Point3D(638, 2400, 0), map);

            tele = new Teleporter(new Point3D(632, 2424, -20), map);
            tele.MoveToWorld(new Point3D(639, 2400, 0), map);

            tele = new Teleporter(new Point3D(543, 2303, 0), map);
            tele.MoveToWorld(new Point3D(575, 2463, 0), map);

            tele = new Teleporter(new Point3D(544, 2303, 0), map);
            tele.MoveToWorld(new Point3D(576, 2463, 0), map);

            tele = new Teleporter(new Point3D(543, 2304, 0), map);
            tele.MoveToWorld(new Point3D(575, 2464, 0), map);

            tele = new Teleporter(new Point3D(544, 2304, 0), map);
            tele.MoveToWorld(new Point3D(576, 2464, 0), map);
        }

        private static void GenStations()
        {
            Map map = Map.TerMur;

            PowerCoreDockingStation station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(623, 2447, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(639, 2447, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(647, 2455, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(647, 2463, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(639, 2471, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(615, 2455, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(615, 2463, -20), map);

            station = new PowerCoreDockingStation(true);
            station.MoveToWorld(new Point3D(623, 2471, -20), map);
        }

        private static void GenLOSBlockers()
        {
            LOSBlocker blocker = null;

            for (int y = 2392; y < 2407; y++)
            {
                blocker = new LOSBlocker();
                blocker.MoveToWorld(new Point3D(583, y, 0), Map.TerMur);
            }
        }

        private static void GenChests()
        {
            Map map = Map.TerMur;

            KotlRegalChest chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(484, 2289, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(483, 2305, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(485, 2321, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(497, 2289, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(499, 2305, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(500, 2321, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(634, 2321, 0), map);

            chest = new KotlRegalChest
            {
                ItemID = 0x4D0D
            };
            chest.MoveToWorld(new Point3D(633, 2292, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(649, 2289, 0), map);

            chest = new KotlRegalChest();
            chest.MoveToWorld(new Point3D(649, 2321, 0), map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(HasGenerated);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = InheritInsertion ? 0 : reader.ReadInt();

            switch (v)
            {
                case 0:
                    Timer.DelayCall(() =>
                    {
                        HasGenerated = Hal.Instance != null;
                    });
                    break;
                case 1:
                    HasGenerated = reader.ReadBool();
                    break;
            }
        }
    }
}
