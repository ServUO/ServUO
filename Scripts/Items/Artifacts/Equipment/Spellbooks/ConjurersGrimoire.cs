using System;

namespace Server.Items
{
    public class ConjurersGrimoire : Spellbook, ITokunoDyable
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1094799; } } // Conjurer's Grimoire

        [Constructable]
        public ConjurersGrimoire()
            : base()
        {
            this.Hue = 1157;
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