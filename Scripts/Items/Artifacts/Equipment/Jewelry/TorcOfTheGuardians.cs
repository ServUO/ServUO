using System;

namespace Server.Items
{
    public class TorcOfTheGuardians : GoldNecklace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TorcOfTheGuardians()
        {
            this.Name = ("Torc Of The Guardians");
		
            this.Hue = 1837;
		
            this.Attributes.BonusInt = 5;
            this.Attributes.BonusStr = 5;
            this.Attributes.BonusDex = 5;
            this.Attributes.RegenStam = 2;
            this.Attributes.RegenMana = 2;
            this.Attributes.LowerManaCost = 5;
            this.Resistances.Physical = 5;
            this.Resistances.Fire = 5;
            this.Resistances.Cold = 5;
            this.Resistances.Poison = 5;
            this.Resistances.Energy = 5;
        }

        public TorcOfTheGuardians(Serial serial)
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}