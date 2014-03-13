using System;

namespace Server.Items
{
    //Based off Axe
    [FlipableAttribute(0x48B2, 0x48B3)]
    public class GargishAxe : BaseAxe
    {
        [Constructable]
        public GargishAxe()
            : base(0x48B2)
        {
            this.Weight = 4.0;
        }

        public GargishAxe(Serial serial)
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
                return WeaponAbility.Dismount;
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
                return 17;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 37;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.00f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 6;
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
                return 37;
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
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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
}