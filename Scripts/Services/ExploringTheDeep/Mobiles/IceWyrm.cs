using System;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("an ice wyrm corpse")]
    public class IceWyrm : WhiteWyrm
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }
        
        [Constructable]
        public IceWyrm()
            : base()
        {
            m_Instances.Add(this);

            this.Name = "Ice Wyrm";
            this.Hue = 2729;
            this.Body = 180;

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }

        public static IceWyrm Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;
            
            IceWyrm creature = new IceWyrm();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private IceWyrm Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(60))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((IceWyrm)p);
            }
            protected override void OnTick()
            {
                if (Mare.Map != Map.Internal)
                {
                    Mare.Delete();
                    this.Stop();
                }
            }
        }

        public override bool OnBeforeDeath()
        {
            Mobile killer = DemonKnight.FindRandomPlayer(this);

            if (killer != null)
            {
                Item item = new IceWyrmScale();

                Container pack = killer.Backpack;

                if (pack == null || !pack.TryDropItem(killer, item, false))
                    killer.BankBox.DropItem(item);

                killer.SendLocalizedMessage(1154489); // You received a Quest Item!
            }            

            return base.OnBeforeDeath();
        }

        public override bool ReacquireOnMovement { get { return true; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 20; } }
        public override int Hides { get { return 25; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Gold; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public IceWyrm(Serial serial) : base(serial)
        {
            m_Instances.Add(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}