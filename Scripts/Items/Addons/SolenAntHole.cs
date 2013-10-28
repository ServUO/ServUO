using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

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
                Map map = this.Map;

                if (map == Map.Trammel || map == Map.Felucca)
                {
                    from.MoveToWorld(new Point3D(5922, 2024, 0), map);
                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, String.Format("* {0} dives into the hole and disappears!*", from.Name)); 
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
            this.m_Spawned = new List<Mobile>();

            this.AddComponent(new AddonComponent(0x914), "dirt", 0, 0, 0, 0);
            this.AddComponent(new SolenAntHoleComponent(0x122A), "a hole", 0x1, 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1B23), "dirt", 0x970, 1, 1, 0);
            this.AddComponent(new AddonComponent(0xEE0), "dirt", 0, 1, 0, 0);
            this.AddComponent(new AddonComponent(0x1B24), "dirt", 0x970, 1, -1, 0);
            this.AddComponent(new AddonComponent(0xEE1), "dirt", 0, 0, -1, 0);
            this.AddComponent(new AddonComponent(0x1B25), "dirt", 0x970, -1, -1, 0);
            this.AddComponent(new AddonComponent(0xEE2), "dirt", 0, -1, 0, 0);
            this.AddComponent(new AddonComponent(0x1B26), "dirt", 0x970, -1, 1, 0);
            this.AddComponent(new AddonComponent(0xED3), "dirt", 0, 0, 1, 0);
        }

        public SolenAntHole(Serial serial)
            : base(serial)
        {
        }

        public override bool ShareHue
        {
            get
            {
                return false;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Player || !m.Alive || m.Hidden || !this.SpawnKilled())
                return;
				
            if (Utility.InRange(this.Location, m.Location, 3) && !Utility.InRange(this.Location, oldLocation, 3))
            {
                int count = 1 + Utility.Random(4);

                for (int i = 0; i < count; i++)
                    this.SpawnAnt();

                if (0.05 > Utility.RandomDouble())
                    this.SpawnAnt(new Beetle());
            }
        }

        public void AddComponent(AddonComponent c, string name, int hue, int x, int y, int z)
        {
            c.Hue = hue;
            c.Name = name;
            this.AddComponent(c, x, y, z);
        }

        public void SpawnAnt()
        {
            int random = Utility.Random(3);
            Map map = this.Map;

            if (map == Map.Trammel)
            {
                if (random < 2)
                    this.SpawnAnt(new RedSolenWorker());
                else
                    this.SpawnAnt(new RedSolenWarrior());
            }
            else if (map == Map.Felucca)
            {
                if (random < 2)
                    this.SpawnAnt(new BlackSolenWorker());
                else
                    this.SpawnAnt(new BlackSolenWarrior());
            }
        }

        public void SpawnAnt(BaseCreature ant)
        {
            this.m_Spawned.Add(ant);

            Map map = this.Map;
            Point3D p = this.Location;

            for (int i = 0; i < 5; i++)
                if (SpellHelper.FindValidSpawnLocation(map, ref p, false))
                    break;

            ant.MoveToWorld(p, map);
            ant.Home = this.Location;
            ant.RangeHome = 10;
        }

        public bool SpawnKilled()
        {
            for (int i = this.m_Spawned.Count - 1; i >= 0; i--)
            {
                if (!this.m_Spawned[i].Alive || this.m_Spawned[i].Deleted)
                    this.m_Spawned.RemoveAt(i);
            }

            return this.m_Spawned.Count < 2;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteMobileList<Mobile>(this.m_Spawned);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Spawned = reader.ReadStrongMobileList<Mobile>();
        }
    }
}