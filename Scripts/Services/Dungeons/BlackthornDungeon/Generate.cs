using Server.Commands;
using Server.Items;

namespace Server.Engines.Blackthorn
{
    public static class GenerateBlackthornDungeon
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenBlackthorn", AccessLevel.Administrator, Generate);
        }

        public static void Generate(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            Generate(Map.Trammel);
            Generate(Map.Felucca);

            CommandSystem.Handle(m, CommandSystem.Prefix + "XmlLoad RevampedSpawns/BlackthornDungeonCreature.xml");
            CommandSystem.Handle(m, CommandSystem.Prefix + "XmlLoad RevampedSpawns/BlackthornDungeonAgent.xml");
        }

        public static void Generate(Map map)
        {
            DungeonHitchingPost post = new DungeonHitchingPost();
            post.MoveToWorld(new Point3D(6428, 2677, 0), map);

            MetalDoor door = new MetalDoor(DoorFacing.WestCCW);
            door.MoveToWorld(new Point3D(6409, 2695, 0), map);

            door = new MetalDoor(DoorFacing.EastCW);
            door.MoveToWorld(new Point3D(6410, 2695, 0), map);

            door = new MetalDoor(DoorFacing.WestCW);
            door.MoveToWorld(new Point3D(6409, 2664, 0), map);

            door = new MetalDoor(DoorFacing.EastCCW);
            door.MoveToWorld(new Point3D(6410, 2664, 0), map);

            door = new MetalDoor(DoorFacing.SouthCW);
            door.MoveToWorld(new Point3D(6394, 2680, 0), map);

            door = new MetalDoor(DoorFacing.NorthCCW);
            door.MoveToWorld(new Point3D(6394, 2679, 0), map);

            door = new MetalDoor(DoorFacing.NorthCW);
            door.MoveToWorld(new Point3D(6425, 2679, 0), map);

            door = new MetalDoor(DoorFacing.SouthCCW);
            door.MoveToWorld(new Point3D(6425, 2680, 0), map);

            // Tram ones already exist on create world teleporter section
            if (map == Map.Felucca)
            {
                Teleporter tele = new Teleporter(new Point3D(1517, 1417, 9), map);
                tele.MoveToWorld(new Point3D(6440, 2677, 20), map);

                tele = new Teleporter(new Point3D(1517, 1418, 9), map);
                tele.MoveToWorld(new Point3D(6440, 2678, 20), map);

                tele = new Teleporter(new Point3D(1517, 1419, 9), map);
                tele.MoveToWorld(new Point3D(6440, 2679, 20), map);

                tele = new Teleporter(new Point3D(1517, 1420, 9), map);
                tele.MoveToWorld(new Point3D(6440, 2680, 20), map);

                tele = new Teleporter(new Point3D(1517, 1420, 9), map);
                tele.MoveToWorld(new Point3D(6440, 2681, 20), map);

                tele = new Teleporter(new Point3D(6440, 2677, 20), map);
                tele.MoveToWorld(new Point3D(1517, 1417, 9), map);

                tele = new Teleporter(new Point3D(6440, 2678, 20), map);
                tele.MoveToWorld(new Point3D(1517, 1418, 9), map);

                tele = new Teleporter(new Point3D(6440, 2679, 20), map);
                tele.MoveToWorld(new Point3D(1517, 1419, 9), map);

                tele = new Teleporter(new Point3D(6440, 2680, 20), map);
                tele.MoveToWorld(new Point3D(1517, 1420, 12), map);
            }

            foreach (Point3D p in _BlockerList)
            {
                Blocker blocker = new Blocker();
                blocker.MoveToWorld(p, map);
            }
        }

        private static readonly Point3D[] _BlockerList =
        {
            new Point3D(6435, 2425, -5),
            new Point3D(6435, 2424, -5),
            new Point3D(6435, 2423, -5),
            new Point3D(6435, 2422, -5),
            new Point3D(6435, 2421, -5),
            new Point3D(6434, 2421, -5),
            new Point3D(6433, 2421, -5),
            new Point3D(6432, 2421, -5),
            new Point3D(6442, 2425, -5),
            new Point3D(6442, 2424, -5),
            new Point3D(6442, 2423, -5),
            new Point3D(6442, 2422, -5),
            new Point3D(6442, 2421, -5),
            new Point3D(6443, 2421, -5),
            new Point3D(6444, 2421, -5),
            new Point3D(6445, 2421, -5)
        };
    }
}
