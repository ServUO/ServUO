using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines
{
    public static class GenerateWrongRevamp
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenWrongRevamp", AccessLevel.Administrator, Generate_NewWrong);
        }

        public static void Generate_NewWrong(CommandEventArgs e)
        {
            DeleteOldWrong(e.Mobile);

            CommandSystem.Handle(e.Mobile, CommandSystem.Prefix + "XmlLoad RevampedSpawns/WrongRevamped.xml");

            Decorate.Generate("wrong", "Data/Decoration/Wrong", Map.Trammel, Map.Felucca);

            Item spawner = new BedrollSpawner();
            spawner.MoveToWorld(new Point3D(5823, 601, 0), Map.Felucca);

            spawner = new BedrollSpawner();
            spawner.MoveToWorld(new Point3D(5823, 601, 0), Map.Trammel);

            EnchantedHotItem.SpawnChests(Map.Trammel);
            EnchantedHotItem.SpawnChests(Map.Felucca);

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

            teleporter = new Teleporter(new Point3D(2041, 215, 14), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5824, 631, 5), Map.Felucca);

            teleporter = new Teleporter(new Point3D(2043, 215, 14), Map.Felucca);
            teleporter.MoveToWorld(new Point3D(5825, 631, 5), Map.Felucca);

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

            teleporter = new Teleporter(new Point3D(2041, 215, 14), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5824, 631, 5), Map.Trammel);

            teleporter = new Teleporter(new Point3D(2043, 215, 14), Map.Trammel);
            teleporter.MoveToWorld(new Point3D(5825, 631, 5), Map.Trammel);

            e.Mobile.SendMessage("Wrong Revamp generation complete.");
        }

        public static void DeleteOldWrong(Mobile m)
        {
            int count = 0;

            IPooledEnumerable eable = Map.Felucca.GetItemsInBounds(new Rectangle2D(5633, 511, 253, 510));

            foreach (Item item in eable)
            {
                if (item is XmlSpawner || item is Teleporter || item.ItemID == 0x375A || item is BarredMetalDoor || item is SecretDungeonDoor)
                {
                    count++;
                    item.Delete();
                }
            }

            eable.Free();

            eable = Map.Trammel.GetItemsInBounds(new Rectangle2D(5633, 511, 253, 510));

            foreach (Item item in eable)
            {
                if (item is XmlSpawner || item is Teleporter || item.ItemID == 0x375A || item is BarredMetalDoor || item is SecretDungeonDoor)
                {
                    count++;
                    item.Delete();
                }
            }

            eable.Free();

            m.SendMessage("{0} items deleted.", count);
        }
    }
}
