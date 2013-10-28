using System;

namespace Server.Items
{
    public class MysticsGarb : Robe
    {
        [Constructable]
        public MysticsGarb()
            : base()
        {
            //TODO: GargishRobe ItemID
            this.Name = ("Mystic's Garb");
		
            this.Hue = 1420;
			
            this.Attributes.BonusMana = 5;
            this.Attributes.LowerManaCost = 1;
        }

        public MysticsGarb(Serial serial)
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