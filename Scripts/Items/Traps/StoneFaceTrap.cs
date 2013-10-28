using System;

namespace Server.Items
{
    public enum StoneFaceTrapType
    {
        NorthWestWall,
        NorthWall,
        WestWall
    }

    public class StoneFaceTrap : BaseTrap
    {
        [Constructable]
        public StoneFaceTrap()
            : base(0x10FC)
        {
            this.Light = LightType.Circle225;
        }

        public StoneFaceTrap(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public StoneFaceTrapType Type
        {
            get
            {
                switch ( this.ItemID )
                {
                    case 0x10F5:
                    case 0x10F6:
                    case 0x10F7:
                        return StoneFaceTrapType.NorthWestWall;
                    case 0x10FC:
                    case 0x10FD:
                    case 0x10FE:
                        return StoneFaceTrapType.NorthWall;
                    case 0x110F:
                    case 0x1110:
                    case 0x1111:
                        return StoneFaceTrapType.WestWall;
                }

                return StoneFaceTrapType.NorthWestWall;
            }
            set
            {
                bool breathing = this.Breathing;

                this.ItemID = (breathing ? GetFireID(value) : GetBaseID(value));
            }
        }
        public bool Breathing
        {
            get
            {
                return (this.ItemID == GetFireID(this.Type));
            }
            set
            {
                if (value)
                    this.ItemID = GetFireID(this.Type);
                else
                    this.ItemID = GetBaseID(this.Type);
            }
        }
        public override bool PassivelyTriggered
        {
            get
            {
                return true;
            }
        }
        public override TimeSpan PassiveTriggerDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override int PassiveTriggerRange
        {
            get
            {
                return 2;
            }
        }
        public override TimeSpan ResetDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public static int GetBaseID(StoneFaceTrapType type)
        {
            switch ( type )
            {
                case StoneFaceTrapType.NorthWestWall:
                    return 0x10F5;
                case StoneFaceTrapType.NorthWall:
                    return 0x10FC;
                case StoneFaceTrapType.WestWall:
                    return 0x110F;
            }

            return 0;
        }

        public static int GetFireID(StoneFaceTrapType type)
        {
            switch ( type )
            {
                case StoneFaceTrapType.NorthWestWall:
                    return 0x10F7;
                case StoneFaceTrapType.NorthWall:
                    return 0x10FE;
                case StoneFaceTrapType.WestWall:
                    return 0x1111;
            }

            return 0;
        }

        public override void OnTrigger(Mobile from)
        {
            if (!from.Alive || from.IsStaff())
                return;

            Effects.PlaySound(this.Location, this.Map, 0x359);

            this.Breathing = true;

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), new TimerCallback(FinishBreath));
            Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(TriggerDamage));
        }

        public virtual void FinishBreath()
        {
            this.Breathing = false;
        }

        public virtual void TriggerDamage()
        {
            foreach (Mobile mob in this.GetMobilesInRange(1))
            {
                if (mob.Alive && !mob.IsDeadBondedPet && mob.IsPlayer())
                    Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), mob, mob, Utility.Dice(3, 15, 0));
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

            this.Breathing = false;
        }
    }

    public class StoneFaceTrapNoDamage : StoneFaceTrap
    {
        [Constructable]
        public StoneFaceTrapNoDamage()
        {
        }

        public StoneFaceTrapNoDamage(Serial serial)
            : base(serial)
        {
        }

        public override void TriggerDamage()
        {
            // nothing..
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
}