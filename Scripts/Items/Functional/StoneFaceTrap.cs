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
            Light = LightType.Circle225;
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
                switch (ItemID)
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
                bool breathing = Breathing;

                ItemID = (breathing ? GetFireID(value) : GetBaseID(value));
            }
        }
        public bool Breathing
        {
            get
            {
                return (ItemID == GetFireID(Type));
            }
            set
            {
                if (value)
                    ItemID = GetFireID(Type);
                else
                    ItemID = GetBaseID(Type);
            }
        }
        public override bool PassivelyTriggered => true;
        public override TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
        public override int PassiveTriggerRange => 2;
        public override TimeSpan ResetDelay => TimeSpan.Zero;
        public static int GetBaseID(StoneFaceTrapType type)
        {
            switch (type)
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
            switch (type)
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

            Effects.PlaySound(Location, Map, 0x359);

            Breathing = true;

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), FinishBreath);
            Timer.DelayCall(TimeSpan.FromSeconds(1.0), TriggerDamage);
        }

        public virtual void FinishBreath()
        {
            Breathing = false;
        }

        public virtual void TriggerDamage()
        {
            IPooledEnumerable eable = GetMobilesInRange(1);
            foreach (Mobile mob in eable)
            {
                if (mob.Alive && !mob.IsDeadBondedPet && mob.IsPlayer())
                    Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), mob, mob, Utility.Dice(3, 15, 0));
            }
            eable.Free();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Breathing = false;
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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
