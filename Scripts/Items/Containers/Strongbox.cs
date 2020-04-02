using System;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    [FlipableAttribute(0xE80, 0x9A8)]
    public class StrongBox : BaseContainer, IChopable
    {
        private Mobile m_Owner;
        private BaseHouse m_House;
        public StrongBox(Mobile owner, BaseHouse house)
            : base(0xE80)
        {
            this.m_Owner = owner;
            this.m_House = house;

            this.MaxItems = 25;
        }

        public StrongBox(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 100;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1023712;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
                this.InvalidateProperties();
            }
        }
        public override int DefaultMaxWeight
        {
            get
            {
                return 0;
            }
        }
        public override bool Decays
        {
            get
            {
                if (this.m_House != null && this.m_Owner != null && !this.m_Owner.Deleted)
                    return !this.m_House.IsCoOwner(this.m_Owner);
                else
                    return true;
            }
        }
        public override TimeSpan DecayTime
        {
            get
            {
                return TimeSpan.FromMinutes(30.0);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Owner);
            writer.Write(this.m_House);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Owner = reader.ReadMobile();
                        this.m_House = reader.ReadItem() as BaseHouse;

                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(Validate));
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.m_Owner != null)
                list.Add(1042887, this.m_Owner.Name); // a strong box owned by ~1_OWNER_NAME~
            else
                base.AddNameProperty(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Owner != null)
            {
                this.LabelTo(from, 1042887, this.m_Owner.Name); // a strong box owned by ~1_OWNER_NAME~

                if (this.CheckContentDisplay(from))
                    this.LabelTo(from, "({0} items, {1} stones)", this.TotalItems, this.TotalWeight);
            }
            else
            {
                base.OnSingleClick(from);
            }
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            if (this.m_Owner == null || this.m_Owner.Deleted || this.m_House == null || this.m_House.Deleted || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return m == this.m_Owner && this.m_House.IsCoOwner(m) && base.IsAccessibleTo(m);
        }

        public void OnChop(Mobile from)
        {
            if (this.m_House != null && !this.m_House.Deleted && this.m_Owner != null && !this.m_Owner.Deleted)
            {
                if (from == this.m_Owner || this.m_House.IsOwner(from))
                    this.Chop(from);
            }
            else
            {
                this.Chop(from);
            }
        }

        public Container ConvertToStandardContainer()
        {
            Container metalBox = new MetalBox();
            List<Item> subItems = new List<Item>(this.Items);

            foreach (Item subItem in subItems)
            {
                metalBox.AddItem(subItem);
            }

            this.Delete();

            return metalBox;
        }

        private void Validate()
        {
            if (this.m_Owner != null && this.m_House != null && !this.m_House.IsCoOwner(this.m_Owner))
            {
                Console.WriteLine("Warning: Destroying strongbox of {0}", this.m_Owner.Name);
                this.Destroy();
            }
        }

        private void Chop(Mobile from)
        {
            Effects.PlaySound(this.Location, this.Map, 0x3B3);
            from.SendLocalizedMessage(500461); // You destroy the item.
            this.Destroy();
        }
    }
}