using System;

namespace Server.Items
{
    [FlipableAttribute(0x48C0, 0x481)]
    public class GargishWarHammer : BaseBashing
    {
        [Constructable]
        public GargishWarHammer()
            : base(0x48C0)
        {
            Weight = 10.0;
            Layer = Layer.TwoHanded;
        }

        public GargishWarHammer(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.WhirlwindAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.CrushingBlow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 95;
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
                return 110;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Bash2H;
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