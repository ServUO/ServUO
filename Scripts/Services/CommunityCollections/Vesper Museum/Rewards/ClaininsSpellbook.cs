using System;

namespace Server.Items
{
    public class ClaininsSpellbook : Spellbook
    {
        [Constructable]
        public ClaininsSpellbook()
            : base()
        {
            this.Hue = 0x84D;
			
            this.Attributes.SpellChanneling = 1;
            this.Attributes.RegenMana = 3;
            this.Attributes.Luck = 80;
            this.Attributes.LowerRegCost = 15;
        }

        public ClaininsSpellbook(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073262;
            }
        }// Clainin's Spellbook - Museum of Vesper Replica
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