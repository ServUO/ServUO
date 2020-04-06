using System;

namespace Server.Items
{
    [FlipableAttribute(0x48B4, 0x48B5)]
    public class GargishBardiche : BasePoleArm
    {
        [Constructable]
        public GargishBardiche()
            : base(0x48B4)
        {
            this.Weight = 7.0;
        }

        public GargishBardiche(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 45;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 17;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 20;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.75f;
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
                return 100;
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
