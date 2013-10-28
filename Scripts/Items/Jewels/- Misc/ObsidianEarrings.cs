using System;

namespace Server.Items
{
    public class ObsidianEarrings : GoldEarrings
    {
        [Constructable]
        public ObsidianEarrings()
            : base()
        {
            this.Name = ("Obsidian Earrings");
		
            this.Hue = 1;
			
            this.Attributes.BonusMana = 8;
            this.Attributes.RegenMana = 2;
            this.Attributes.RegenStam = 2;
            this.Attributes.SpellDamage = 8;
            this.Resistances.Physical = 4;
            this.Resistances.Fire = 10;
            this.Resistances.Cold = 10;
            this.Resistances.Poison = 3;
            this.Resistances.Energy = 13;
            //AbsorptionAttribute.CastingFocus = 4; TODO: how this shit works?
        }

        public ObsidianEarrings(Serial serial)
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