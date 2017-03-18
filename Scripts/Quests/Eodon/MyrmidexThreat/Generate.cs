using Server;
using System;
using Server.Engines.Quests;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Server.Engines.MyrmidexInvasion
{
	public static class GenerateMyrmidexQuest
	{
        public static void Generate()
        {
            Map map = Map.TerMur;
            int z = -19;

            Static s = new Static(40142);
            s.MoveToWorld(new Point3D(888, 2303, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(889, 2304, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(896, 2295, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(897, 2296, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(904, 2303, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(905, 2304, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(896, 2311, z), map);

            s = new Static(40142);
            s.MoveToWorld(new Point3D(897, 2312, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(887, 2304, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(888, 2305, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(895, 2296, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(896, 2297, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(903, 2304, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(904, 2305, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(895, 2312, z), map);

            s = new Static(40169);
            s.MoveToWorld(new Point3D(896, 2313, z), map);

            z = -20;

            s = new Static(40161);
            s.MoveToWorld(new Point3D(897, 2305, z), map);

            s = new Static(40165);
            s.MoveToWorld(new Point3D(894, 2306, z), map);

            MoonstonePowerGeneratorAddon addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(884, 2292, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(896, 2286, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(909, 2291, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(914, 2305, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(909, 2318, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(896, 2323, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(884, 2316, z), map);

            addon = new MoonstonePowerGeneratorAddon(true);
            addon.MoveToWorld(new Point3D(878, 2304, z), map);

            var idol = new MyrmidexIdol();
            idol.MoveToWorld(new Point3D(488, 1856, 111), map);

            Teleporter teleporter = new Teleporter(new Point3D(487, 1857, 95), map);
            teleporter.MoveToWorld(new Point3D(896, 2304, 45), map);

            teleporter = new MyrmidexQueenTeleporter(new Point3D(711, 2302, 0), map);
            teleporter.MoveToWorld(new Point3D(121, 1682, -3), map);

            teleporter = new MyrmidexQueenTeleporter(new Point3D(711, 2303, 0), Map.TerMur);
            teleporter.MoveToWorld(new Point3D(121, 1683, -2), map);

            teleporter = new MyrmidexQueenTeleporter(new Point3D(711, 2304, 0), Map.TerMur);
            teleporter.MoveToWorld(new Point3D(121, 1684, -1), map);

            teleporter = new Teleporter(new Point3D(120, 1682, -1), map);
            teleporter.MoveToWorld(new Point3D(711, 2302, 0), map);

            teleporter = new Teleporter(new Point3D(120, 1683, 1), map);
            teleporter.MoveToWorld(new Point3D(711, 2303, 0), map);

            teleporter = new Teleporter(new Point3D(120, 1684, 2), map);
            teleporter.MoveToWorld(new Point3D(711, 2304, 0), map);

            teleporter = new Teleporter(new Point3D(120, 1684, 2), map);
            teleporter.MoveToWorld(new Point3D(711, 2305, 0), map);

            teleporter = new MyrmidexPitTeleporter(Allegiance.Myrmidex, new Point3D(985, 1883, 0), map);
            teleporter.MoveToWorld(new Point3D(176, 1835, 90), map);

            teleporter = new MyrmidexPitTeleporter(Allegiance.Myrmidex, new Point3D(986, 1883, 0), map);
            teleporter.MoveToWorld(new Point3D(177, 1835, 90), map);

            teleporter = new Teleporter(new Point3D(176, 1834, 90), map);
            teleporter.MoveToWorld(new Point3D(985, 1887, 0), map);

            teleporter = new Teleporter(new Point3D(177, 1834, 90), map);
            teleporter.MoveToWorld(new Point3D(986, 1887, 0), map);

            teleporter = new MyrmidexPitTeleporter(Allegiance.Tribes, new Point3D(853, 1777, 0), map);
            teleporter.MoveToWorld(new Point3D(224, 1724, 6), map);

            teleporter = new MyrmidexPitTeleporter(Allegiance.Tribes, new Point3D(854, 1777, 0), map);
            teleporter.MoveToWorld(new Point3D(224, 1725, 6), map);

            teleporter = new Teleporter(new Point3D(225, 1724, 6), map);
            teleporter.MoveToWorld(new Point3D(853, 1776, 11), map);

            teleporter = new Teleporter(new Point3D(225, 1725, 6), map);
            teleporter.MoveToWorld(new Point3D(854, 1776, 11), map);

            teleporter = new Teleporter(new Point3D(225, 1725, 6), map);
            teleporter.MoveToWorld(new Point3D(855, 1776, 11), map);

            var los = new LOSBlocker();
            los.MoveToWorld(new Point3D(121, 1682, -3), map);

            los = new LOSBlocker();
            los.MoveToWorld(new Point3D(121, 1683, -3), map);

            los = new LOSBlocker();
            los.MoveToWorld(new Point3D(121, 1684, -3), map);

            var allegianceIdol = new AllegianceIdol(Allegiance.Tribes);
            allegianceIdol.MoveToWorld(new Point3D(267, 1741, 85), map);

            allegianceIdol = new AllegianceIdol(Allegiance.Myrmidex);
            allegianceIdol.MoveToWorld(new Point3D(176, 1813, 91), map);

            HealerCamp camp = new HealerCamp();
            camp.Map = Map.TerMur;
            camp.Location = new Point3D(262, 1716, 80);

            GeoffreyCampAddon gcamp = new GeoffreyCampAddon();
            gcamp.MoveToWorld(new Point3D(264, 1732, 80), Map.TerMur);

            BattleSpawner spawner = new BattleSpawner();
            spawner.MoveToWorld(new Point3D(851, 1776, 0), Map.TerMur);

            Item st = new Static(0xA1F);
            st.MoveToWorld(new Point3D(913, 1871, 0), Map.TerMur);

            st = new Static(0xA1F);
            st.MoveToWorld(new Point3D(914, 1871, 0), Map.TerMur);

            BattleFlag bflag = new BattleFlag(0x42CB, 0);
            bflag.MoveToWorld(new Point3D(914, 1872, 5), Map.TerMur);

            st = new Static(0xA1F);
            st.MoveToWorld(new Point3D(913, 1792, 0), Map.TerMur);

            bflag = new BattleFlag(0x42C, 2520);
            bflag.MoveToWorld(new Point3D(914, 1793, 6), Map.TerMur);

            bflag = new BattleFlag(0x42D, 2520);
            bflag.MoveToWorld(new Point3D(913, 1793, 6), Map.TerMur);

            var sp = new XmlSpawner("Yar");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(169, 1813, 80), map);
            sp.Respawn();

            sp = new XmlSpawner("Bront");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1448, 3774, 0), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("CollectorOfOddities");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(4305, 1003, 0), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("Carroll");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(2878, 723, 0), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("Eriathwen");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(1426, 1653, 0), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("EllieRafkin");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(261, 1742, 80), map);
            sp.Respawn();

            sp = new XmlSpawner("Yero");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(3692, 2252, 26), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("Alida");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(5257, 4012, 40), Map.Trammel);
            sp.Respawn();

            sp = new XmlSpawner("Foxx");
            sp.SpawnRange = 1;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(269, 1727, 80), map);
            sp.Respawn();

            sp = new XmlSpawner("MyrmidexQueen/Cantwalk/true");
            sp.SpawnRange = 0;
            sp.HomeRange = 1;
            sp.MoveToWorld(new Point3D(768, 2303, 0), map);
            sp.Respawn();

        }
	}
}