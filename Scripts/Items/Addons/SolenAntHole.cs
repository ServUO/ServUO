using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System.Collections.Generic;

namespace Server.Items
{
    public class SolenAntHoleComponent : AddonComponent
    {
        public SolenAntHoleComponent(int itemID)
            : base(itemID)
        {
        }

        public SolenAntHoleComponent(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this, 2))
            {
                Map map = Map;

                if (map == Map.Trammel || map == Map.Felucca)
                {
                    from.MoveToWorld(new Point3D(5922, 2024, 0), map);
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, true, string.Format("* {0} dives into the hole and disappears!*", from.Name));
                }
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SolenAntHole : BaseAddon
    {
        private List<Mobile> m_Spawned;
        [Constructable]
        public SolenAntHole()
            : base()
        {
            m_Spawned = new List<Mobile>();

            AddComponent(new AddonComponent(0x914), "dirt", 0, 0, 0, 0);
            AddComponent(new SolenAntHoleComponent(0x122A), "a hole", 0x1, 0, 0, 0);
            AddComponent(new AddonComponent(0x1B23), "dirt", 0x970, 1, 1, 0);
            AddComponent(new AddonComponent(0xEE0), "dirt", 0, 1, 0, 0);
            AddComponent(new AddonComponent(0x1B24), "dirt", 0x970, 1, -1, 0);
            AddComponent(new AddonComponent(0xEE1), "dirt", 0, 0, -1, 0);
            AddComponent(new AddonComponent(0x1B25), "dirt", 0x970, -1, -1, 0);
            AddComponent(new AddonComponent(0xEE2), "dirt", 0, -1, 0, 0);
            AddComponent(new AddonComponent(0x1B26), "dirt", 0x970, -1, 1, 0);
            AddComponent(new AddonComponent(0xED3), "dirt", 0, 0, 1, 0);
        }

        public SolenAntHole(Serial serial)
            : base(serial)
        {
        }

        public override bool ShareHue => false;
        public override bool HandlesOnMovement => true;
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Player || !m.Alive || m.Hidden || !SpawnKilled())
                return;

            if (Utility.InRange(Location, m.Location, 3) && !Utility.InRange(Location, oldLocation, 3))
            {
                int count = 1 + Utility.Random(4);

                for (int i = 0; i < count; i++)
                    SpawnAnt();

                if (0.05 > Utility.RandomDouble())
                    SpawnAnt(new Beetle());
            }
        }

        public void AddComponent(AddonComponent c, string name, int hue, int x, int y, int z)
        {
            c.Hue = hue;
            c.Name = name;
            AddComponent(c, x, y, z);
        }

        public void SpawnAnt()
        {
            int random = Utility.Random(3);
            Map map = Map;

            if (map == Map.Trammel)
            {
                if (random < 2)
                    SpawnAnt(new RedSolenWorker());
                else
                    SpawnAnt(new RedSolenWarrior());
            }
            else if (map == Map.Felucca)
            {
                if (random < 2)
                    SpawnAnt(new BlackSolenWorker());
                else
                    SpawnAnt(new BlackSolenWarrior());
            }
        }

        public void SpawnAnt(BaseCreature ant)
        {
            m_Spawned.Add(ant);

            Map map = Map;
            Point3D p = Location;

            for (int i = 0; i < 5; i++)
                if (SpellHelper.FindValidSpawnLocation(map, ref p, false))
                    break;

            ant.MoveToWorld(p, map);
            ant.Home = Location;
            ant.RangeHome = 10;
        }

        public bool SpawnKilled()
        {
            for (int i = m_Spawned.Count - 1; i >= 0; i--)
            {
                if (!m_Spawned[i].Alive || m_Spawned[i].Deleted)
                    m_Spawned.RemoveAt(i);
            }

            return m_Spawned.Count < 2;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteMobileList(m_Spawned);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Spawned = reader.ReadStrongMobileList<Mobile>();
        }
    }
}