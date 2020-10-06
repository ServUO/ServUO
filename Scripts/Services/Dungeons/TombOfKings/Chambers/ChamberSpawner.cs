using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.TombOfKings
{
    public class ChamberSpawner : Item
    {
        public static void Generate()
        {
            for (int i = 0; i < m_Positions.Length; i++)
            {
                WeakEntityCollection.Add("sa", new ChamberSpawner(m_Positions[i], Map.TerMur));
            }
        }

        private static readonly string _TimerID = "ChamberSpawner";

        private static readonly Point3D[] m_Positions = new Point3D[]
        {
            new Point3D( 9, 199, -9 ),
            new Point3D( 9, 183, -9 ),
            new Point3D( 9, 167, -9 ),
            new Point3D( 9, 151, -9 ),
            new Point3D( 9, 135, -9 ),
            new Point3D( 9, 119, -9 ),

            new Point3D( 60, 199, -9 ),
            new Point3D( 60, 183, -9 ),
            new Point3D( 60, 167, -9 ),
            new Point3D( 60, 151, -9 ),
            new Point3D( 60, 135, -9 ),
            new Point3D( 60, 119, -9 ),
        };

        private static readonly Type[] m_CreatureTypes = new Type[]
        {
            typeof( SilverSerpent ),
            typeof( UndeadGuardian ),
            typeof( PutridUndeadGuardian )
        };

        private List<Mobile> m_Creatures;

        public ChamberSpawner(Point3D loc, Map map)
            : base(0x2006)
        {
            Stackable = true;
            Amount = loc.Y <= 152 ? 754 : 753;

            Movable = false;

            m_Creatures = new List<Mobile>();

            MoveToWorld(loc, map);
            Respawn();

            StartTimer();
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add(1070701, "gargoyle"); // a ~1_CORPSENAME~ corpse
        }

        public bool IsEmpty()
        {
            for (int i = 0; i < m_Creatures.Count; i++)
                if (m_Creatures[i].Alive)
                    return false;

            return true;
        }

        public void CheckSpawn()
        {
            if (Utility.RandomBool() && IsEmpty())
                Respawn();
        }

        public void Respawn()
        {
            m_Creatures.Clear();

            int total = Utility.RandomMinMax(3, 8);
            Type creatureType = m_CreatureTypes[Utility.Random(m_CreatureTypes.Length)];

            for (int i = 0; i < total; i++)
            {
                Mobile m = (Mobile)Activator.CreateInstance(creatureType);
                m.MoveToWorld(Map.GetSpawnPosition(Location, 4), Map);

                BaseCreature bc = m as BaseCreature;

                if (bc != null)
                {
                    bc.RangeHome = 5;
                    bc.Home = Location;
                }

                m_Creatures.Add(m);
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < m_Creatures.Count; i++)
                m_Creatures[i].Delete();
        }

        private void StartTimer()
        {
            TimerRegistry.Register(_TimerID, this, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0), false, spawner => spawner.CheckSpawn());
        }

        public ChamberSpawner(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Creatures.Count);

            for (int i = 0; i < m_Creatures.Count; i++)
                writer.Write(m_Creatures[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Creatures = new List<Mobile>();
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();

                if (m != null)
                    m_Creatures.Add(m);
            }

            StartTimer();
        }
    }
}
