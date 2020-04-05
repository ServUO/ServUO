using System;
using Server.Engines.Harvest;

namespace Server.Items
{
    public class GargoylesPickaxe : BaseAxe, IUsesRemaining
    {
		public override int LabelNumber { get { return 1041281; } }// a gargoyle's pickaxe
		
        [Constructable]
        public GargoylesPickaxe()
            : this(Utility.RandomMinMax(101, 125))
        {
        }

        [Constructable]
        public GargoylesPickaxe(int uses)
            : base(0xE85 + Utility.Random(2))
        {
            Weight = 11.0;
            UsesRemaining = uses;
            ShowUsesRemaining = true;
            Hue = 0x76c;
        }

        public GargoylesPickaxe(Serial serial)
            : base(serial)
        {
        }
        
        public override HarvestSystem HarvestSystem
        {
            get
            {
                return Mining.System;
            }
        }
        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.DoubleStrike;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
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
                return 3.00f;
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
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Slash1H;
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
