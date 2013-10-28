using System;

namespace Server.Items
{
    [Flipable(0x1EC0, 0x1EC3)]
    public class PickpocketDip : AddonComponent
    {
        private double m_MinSkill;
        private double m_MaxSkill;
        private Timer m_Timer;
        public PickpocketDip(int itemID)
            : base(itemID)
        {
            this.m_MinSkill = -25.0;
            this.m_MaxSkill = +25.0;
        }

        public PickpocketDip(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkill
        {
            get
            {
                return this.m_MinSkill;
            }
            set
            {
                this.m_MinSkill = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSkill
        {
            get
            {
                return this.m_MaxSkill;
            }
            set
            {
                this.m_MaxSkill = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Swinging
        {
            get
            {
                return (this.m_Timer != null);
            }
        }
        public void UpdateItemID()
        {
            int baseItemID = 0x1EC0 + (((this.ItemID - 0x1EC0) / 3) * 3);

            this.ItemID = baseItemID + (this.Swinging ? 1 : 0);
        }

        public void BeginSwing()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();

            this.UpdateItemID();
        }

        public void EndSwing()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            this.UpdateItemID();
        }

        public void Use(Mobile from)
        {
            from.Direction = from.GetDirectionTo(this.GetWorldLocation());

            Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x4F);

            if (from.CheckSkill(SkillName.Stealing, this.m_MinSkill, this.m_MaxSkill))
            {
                this.SendLocalizedMessageTo(from, 501834); // You successfully avoid disturbing the dip while searching it.
            }
            else
            {
                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x390);

                this.BeginSwing();
                this.ProcessDelta();
                this.SendLocalizedMessageTo(from, 501831); // You carelessly bump the dip and start it swinging.
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
                this.SendLocalizedMessageTo(from, 501816); // You are too far away to do that.
            else if (this.Swinging)
                this.SendLocalizedMessageTo(from, 501815); // You have to wait until it stops swinging.
            else if (from.Skills[SkillName.Stealing].Base >= this.m_MaxSkill)
                this.SendLocalizedMessageTo(from, 501830); // Your ability to steal cannot improve any further by simply practicing on a dummy.
            else if (from.Mounted)
                this.SendLocalizedMessageTo(from, 501829); // You can't practice on this while on a mount.
            else
                this.Use(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(this.m_MinSkill);
            writer.Write(this.m_MaxSkill);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_MinSkill = reader.ReadDouble();
                        this.m_MaxSkill = reader.ReadDouble();

                        if (this.m_MinSkill == 0.0 && this.m_MaxSkill == 30.0)
                        {
                            this.m_MinSkill = -25.0;
                            this.m_MaxSkill = +25.0;
                        }

                        break;
                    }
            }

            this.UpdateItemID();
        }

        private class InternalTimer : Timer
        {
            private readonly PickpocketDip m_Dip;
            public InternalTimer(PickpocketDip dip)
                : base(TimeSpan.FromSeconds(3.0))
            {
                this.m_Dip = dip;
                this.Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                this.m_Dip.EndSwing();
            }
        }
    }

    public class PickpocketDipEastAddon : BaseAddon
    {
        [Constructable]
        public PickpocketDipEastAddon()
        {
            this.AddComponent(new PickpocketDip(0x1EC3), 0, 0, 0);
        }

        public PickpocketDipEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new PickpocketDipEastDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PickpocketDipEastDeed : BaseAddonDeed
    {
        [Constructable]
        public PickpocketDipEastDeed()
        {
        }

        public PickpocketDipEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new PickpocketDipEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044337;
            }
        }// pickpocket dip (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PickpocketDipSouthAddon : BaseAddon
    {
        [Constructable]
        public PickpocketDipSouthAddon()
        {
            this.AddComponent(new PickpocketDip(0x1EC0), 0, 0, 0);
        }

        public PickpocketDipSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new PickpocketDipSouthDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PickpocketDipSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public PickpocketDipSouthDeed()
        {
        }

        public PickpocketDipSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new PickpocketDipSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044338;
            }
        }// pickpocket dip (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}