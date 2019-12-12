using System;

namespace Server.Items
{
    [FlipableAttribute(0x4228, 0x4229)]
    public class GargishChaosShield : BaseShield
    {
		public override int BasePhysicalResistance { get { return 1; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 125; } }
        public override int AosStrReq { get { return 95; } }
        public override int ArmorBase { get { return 32; } }
		public override bool CanBeWornByGargoyles { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
		
        [Constructable]
        public GargishChaosShield()
            : base(0x4228)
        {           
            Weight = 5.0;
        }

        public GargishChaosShield(Serial serial)
            : base(serial)
        {
        }
              
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }      
    }
}