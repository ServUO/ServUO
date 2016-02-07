using System;

namespace Server.Factions
{
    public class FactionSawTrap : BaseFactionTrap
    {
        [Constructable]
        public FactionSawTrap()
            : this(null)
        {
        }

        public FactionSawTrap(Serial serial)
            : base(serial)
        {
        }

        public FactionSawTrap(Faction f)
            : this(f, null)
        {
        }

        public FactionSawTrap(Faction f, Mobile m)
            : base(f, m, 0x11AC)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041047;
            }
        }// faction saw trap
        public override int AttackMessage
        {
            get
            {
                return 1010544;
            }
        }// The blade cuts deep into your skin!
        public override int DisarmMessage
        {
            get
            {
                return 1010540;
            }
        }// You carefully dismantle the saw mechanism and disable the trap.
        public override int EffectSound
        {
            get
            {
                return 0x218;
            }
        }
        public override int MessageHue
        {
            get
            {
                return 0x5A;
            }
        }
        public override AllowedPlacing AllowedPlacing
        {
            get
            {
                return AllowedPlacing.ControlledFactionTown;
            }
        }
        public override void DoVisibleEffect()
        {
            Effects.SendLocationEffect(this.Location, this.Map, 0x11AD, 25, 10);
        }

        public override void DoAttackEffect(Mobile m)
        {
            m.Damage(Utility.Dice(6, 10, 40), m);
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

    public class FactionSawTrapDeed : BaseFactionTrapDeed
    {
        public FactionSawTrapDeed()
            : base(0x1107)
        {
        }

        public FactionSawTrapDeed(Serial serial)
            : base(serial)
        {
        }

        public override Type TrapType
        {
            get
            {
                return typeof(FactionSawTrap);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044604;
            }
        }// faction saw trap deed
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