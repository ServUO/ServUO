using System;

namespace Server.Items
{
    // Based off a Longsword
    [FlipableAttribute(0x90B, 0x4074)]
    public class DreadSword : BaseSword
    {
        [Constructable]
        public DreadSword()
            : base(0x90B)
        {
            //Weight = 7.0;
        }

        public DreadSword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.CrushingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ConcussionBlow;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 14;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 18;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 30;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.50f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 25;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 5;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 33;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 35;
            }
        }
        public override int DefHitSound
        {
            get
            {
                return 0x237;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x23A;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 110;
            }
        }

        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

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