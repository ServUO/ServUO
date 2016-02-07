using System;

namespace Server.Factions
{
    public class FactionExplosionTrap : BaseFactionTrap
    {
        [Constructable]
        public FactionExplosionTrap()
            : this(null)
        {
        }

        public FactionExplosionTrap(Faction f)
            : this(f, null)
        {
        }

        public FactionExplosionTrap(Faction f, Mobile m)
            : base(f, m, 0x11C1)
        {
        }

        public FactionExplosionTrap(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1044599;
            }
        }// faction explosion trap
        public override int AttackMessage
        {
            get
            {
                return 1010543;
            }
        }// You are enveloped in an explosion of fire!
        public override int DisarmMessage
        {
            get
            {
                return 1010539;
            }
        }// You carefully remove the pressure trigger and disable the trap.
        public override int EffectSound
        {
            get
            {
                return 0x307;
            }
        }
        public override int MessageHue
        {
            get
            {
                return 0x78;
            }
        }
        public override AllowedPlacing AllowedPlacing
        {
            get
            {
                return AllowedPlacing.AnyFactionTown;
            }
        }
        public override void DoVisibleEffect()
        {
            Effects.SendLocationEffect(this.GetWorldLocation(), this.Map, 0x36BD, 15, 10);
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

    public class FactionExplosionTrapDeed : BaseFactionTrapDeed
    {
        public FactionExplosionTrapDeed()
            : base(0x36D2)
        {
        }

        public FactionExplosionTrapDeed(Serial serial)
            : base(serial)
        {
        }

        public override Type TrapType
        {
            get
            {
                return typeof(FactionExplosionTrap);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044603;
            }
        }// faction explosion trap deed
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