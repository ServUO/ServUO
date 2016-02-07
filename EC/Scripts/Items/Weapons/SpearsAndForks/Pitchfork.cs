using System;

namespace Server.Items
{
    [FlipableAttribute(0xE87, 0xE88)]
    public class Pitchfork : BaseSpear
    {
        [Constructable]
        public Pitchfork()
            : base(0xE87)
        {
            this.Weight = 11.0;
        }

        public Pitchfork(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 55;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 12;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 43;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.50f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 15;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 4;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 45;
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
                return 60;
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

            if (this.Weight == 10.0)
                this.Weight = 11.0;
        }
    }
}