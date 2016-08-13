using Server;
using System;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Items
{
    public static class GenerateHighSeas
    {
        public static void Initialize()
        {
            if (Core.HS)
            {
                if (CharydbisSpawner.SpawnInstance == null)
                {
                    CharydbisSpawner.GenerateCharydbisSpawner();
                    BountyQuestSpawner.GenerateShipSpawner();

                    GenerateDeco();
                }
            }
        }

        public static void GenerateDeco()
        {
            CorgulAltar altar;

            altar = new CorgulAltar();
            altar.MoveToWorld(new Point3D(2453, 865, 0), Map.Felucca);

            altar = new CorgulAltar();
            altar.MoveToWorld(new Point3D(2453, 865, 0), Map.Trammel);

            ProfessionalBountyBoard board;
            
            board = new ProfessionalBountyBoard();
            board.MoveToWorld(new Point3D(4544, 2298, -1), Map.Trammel);

            board = new ProfessionalBountyBoard();
            board.MoveToWorld(new Point3D(4544, 2298, -1), Map.Felucca);

            LocalizedSign sign;

            sign = new LocalizedSign(3025, 1152653); //The port of Zento Parking Area
            sign.MoveToWorld(new Point3D(713, 1359, 53), Map.Tokuno);

            sign = new LocalizedSign(3023, 1149821); //Winds Tavern
            sign.MoveToWorld(new Point3D(4548, 2300, -6), Map.Trammel);

            sign = new LocalizedSign(3023, 1149821); //Winds Tavern
            sign.MoveToWorld(new Point3D(4548, 2300, -6), Map.Felucca);

            sign = new LocalizedSign(3023, 1149820); //General Store
            sign.MoveToWorld(new Point3D(4543, 2317, -3), Map.Trammel);

            sign = new LocalizedSign(3023, 1149820); //General Store
            sign.MoveToWorld(new Point3D(4543, 2317, -3), Map.Felucca);

            Server.Regions.SeaMarketRegion.OnGenerate();

            XmlSpawner sp;
            string toSpawn = "FishMonger";

            //Britain
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1482, 1754, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1482, 1754, -2), Map.Felucca);
            sp.Respawn();

            //Moonglow
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4406, 1049, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4406, 1049, -2), Map.Felucca);
            sp.Respawn();

            //Trinsic
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2061, 2855, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2061, 2855, -2), Map.Felucca);
            sp.Respawn();

            //Vesper
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(3009, 826, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(3009, 826, -2), Map.Felucca);
            sp.Respawn();

            //Jhelom
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1373, 3885, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1373, 3885, -2), Map.Felucca);
            sp.Respawn();

            //Skara Brae
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(641, 2234, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.MoveToWorld(new Point3D(641, 2234, -2), Map.Felucca);
            sp.Respawn();

            //Papua
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(5827, 3258, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(5827, 3258, -2), Map.Felucca);
            sp.Respawn();

            //Floating Eproriam
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 0;
            sp.HomeRange = 0;
            sp.MoveToWorld(new Point3D(4552, 2299, -1), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 0;
            sp.HomeRange = 0;
            sp.MoveToWorld(new Point3D(4540, 2321, -1), Map.Felucca);
            sp.Respawn();

            toSpawn = "DocksAlchemist";

            //Britain
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1482, 1754, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1482, 1754, -2), Map.Felucca);
            sp.Respawn();

            //Moonglow
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4406, 1049, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4406, 1049, -2), Map.Felucca);
            sp.Respawn();

            //Trinsic
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2061, 2855, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2061, 2855, -2), Map.Felucca);
            sp.Respawn();

            //Vesper
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(3009, 826, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(3009, 826, -2), Map.Felucca);
            sp.Respawn();

            //Jhelom
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1373, 3885, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1373, 3885, -2), Map.Felucca);
            sp.Respawn();

            //Skara Brae
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(641, 2234, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.MoveToWorld(new Point3D(641, 2234, -2), Map.Felucca);
            sp.Respawn();

            //Papua
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(5827, 3258, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(5827, 3258, -2), Map.Felucca);
            sp.Respawn();

            //Floating Eproriam
            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4552, 2299, -1), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4540, 2321, -1), Map.Felucca);
            sp.Respawn();

            toSpawn = "GBBigglesby";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4544, 2302, -1), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4544, 2302, -1), Map.Felucca);
            sp.Respawn();

            toSpawn = "GBBigglesby/Name/Mitsubishi/Title/the fleet officer";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 10;
            sp.MoveToWorld(new Point3D(713, 1370, 6), Map.Tokuno);
            sp.Respawn();

            toSpawn = "BoatPainter";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(4552, 2337, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(4552, 2337, -2), Map.Felucca);
            sp.Respawn();

            toSpawn = "Banker";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4554, 2315, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4554, 2315, -2), Map.Felucca);
            sp.Respawn();

            toSpawn = "CrabFisher";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(4552, 2336, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(4552, 2336, -2), Map.Felucca);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(4552, 2378, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(4552, 2378, -2), Map.Felucca);
            sp.Respawn();

            toSpawn = "DockMaster";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 10;
            sp.MoveToWorld(new Point3D(4565, 2307, -2), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 10;
            sp.MoveToWorld(new Point3D(4565, 2307, -2), Map.Felucca);
            sp.Respawn();

            toSpawn = "SeaMarketTavernKeeper";

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4544, 2302, -1), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner(toSpawn);
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4544, 2302, -1), Map.Felucca);
            sp.Respawn();

            Console.WriteLine("High Seas Content generated.");
        }
    }
}