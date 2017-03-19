using Server;
using Server.Commands;
using Server.Items;

namespace Server.Engines
{
    public static class GenerateWrongRewamp
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenWrongRewamp", AccessLevel.Administrator, Generate);
        }

        public static void Generate(CommandEventArgs e)
        {
            CommandSystem.Handle(e.Mobile, Server.Commands.CommandSystem.Prefix + "XmlLoad Spawns/WrongRevamped.xml");

            Decorate.Generate("wrong", "Data/Decoration/Wrong", Map.Trammel, Map.Felucca);

            Item spawner = new BedrollSpawner();
            spawner.MoveToWorld(new Point3D(5823, 601, 0), Map.Felucca);

            spawner = new BedrollSpawner();
            spawner.MoveToWorld(new Point3D(5823, 601, 0), Map.Trammel);

            Teleporter teleporter = new Teleporter(new Point3D(5690, 569, 25), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5827, 590, 1), Map.Felucca);

            teleporter = new Teleporter(new Point3D(5829, 595, 0), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5690, 573, 25), Map.Felucca);

            teleporter = new Teleporter(new Point3D(5690, 569, 25), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5872, 532, 24), Map.Felucca);

            teleporter = new Teleporter(new Point3D(5827, 593, 0), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5732, 554, 24), Map.Felucca);

            teleporter = new Teleporter(new Point3D(5703, 639, 0), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5708, 625, 0), Map.Felucca);

            teleporter = new Teleporter(new Point3D(5792, 526, 10), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5698, 662, 0), Map.Felucca);
            
            teleporter = new Teleporter(new Point3D(5690, 569, 25), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5827, 590, 1), Map.Trammel);

            teleporter = new Teleporter(new Point3D(5829, 595, 0), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5690, 573, 25), Map.Trammel);

            teleporter = new Teleporter(new Point3D(5690, 569, 25), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5872, 532, 24), Map.Trammel);

            teleporter = new Teleporter(new Point3D(5827, 593, 0), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5732, 554, 24), Map.Trammel);

            teleporter = new Teleporter(new Point3D(5703, 639, 0), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5708, 625, 0), Map.Trammel);

            teleporter = new Teleporter(new Point3D(5792, 526, 10), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5698, 662, 0), Map.Trammel);

            e.Mobile.SendMessage("Wrong Revamep generation complete.");
        }
    }
}