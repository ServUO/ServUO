using System;

namespace Server.Items
{
    [FlipableAttribute(0xF62, 0xF63)]
    public class TribalSpear : BaseSpear
    {
		public override int LabelNumber { get { return 1062474; } } // Tribal Spear
		
        [Constructable]
        public TribalSpear()
            : base(0xF62)
        {
            Weight = 7.0;
            Hue = 837;
			Attributes.WeaponDamage = 20;
			WeaponAttributes.DurabilityBonus = 20;
        }

        public TribalSpear(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ArmorIgnore;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 50;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.75f;
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
                return 80;
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
