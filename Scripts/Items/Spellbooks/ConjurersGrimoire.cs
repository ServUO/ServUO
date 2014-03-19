using System;

namespace Server.Items
{
    public class ConjurersGrimoire : Spellbook
    {
        [Constructable]
        public ConjurersGrimoire()
            : base()
        {
            this.Hue = 0x4AA;
			this.Name = "Conjurer's Grimoire";
			this.Slayer = SlayerName.Silver;
            this.Attributes.LowerManaCost = 10;
            this.Attributes.BonusInt = 8;
            this.Attributes.SpellDamage = 15;
            this.SkillBonuses.SetValues(0, SkillName.Magery, 15.0);
        }

        public ConjurersGrimoire(Serial serial)
            : base(serial)
        {
        }

        // Conjurer's Grimoire
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