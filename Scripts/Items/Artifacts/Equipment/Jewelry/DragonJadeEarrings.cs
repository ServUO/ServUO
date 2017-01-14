using System;

namespace Server.Items
{
    public class DragonJadeEarrings : GoldEarrings
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DragonJadeEarrings()
            : base(0x4213)
	
        {
            this.Name = ("Dragon Jade Earrings");
		
            this.Hue = 2129;

            this.Attributes.BonusDex = 5;
            this.Attributes.BonusStr = 5;
            this.Attributes.RegenHits = 2;
            this.Attributes.RegenStam = 3;
            this.Attributes.LowerManaCost = 5;
            this.Resistances.Physical = 9;
            this.Resistances.Fire = 16;
            this.Resistances.Cold = 5;
            this.Resistances.Poison = 13;
            this.Resistances.Energy = 3;
	        AbsorptionAttributes.EaterFire = 10;
        }

        public DragonJadeEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
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