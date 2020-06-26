using Server.Engines.Craft;
using System;

namespace Server.Items
{
    public class ScrappersCompendium : Spellbook
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ScrappersCompendium()
            : base()
        {
            Hue = 0x494;
            Attributes.SpellDamage = 25;
            Attributes.LowerManaCost = 10;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 1;
        }

        public ScrappersCompendium(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072940;// scrappers compendium
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            
            if (Utility.RandomDouble() < 0.5)
            {
                double magery = from.Skills.Magery.Value - 100;

                if (magery < 0)
                    magery = 0;

                int count = Math.Min(3, (int)Math.Round(magery * Utility.RandomDouble() / 5));

                BaseRunicTool.ApplyAttributesTo(this, true, 0, count, 70, 80);
            }

            Attributes.SpellDamage = 25;
            Attributes.LowerManaCost = 10;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 1;

            if (makersMark)
                Crafter = from;

            return quality;
        }
    }
}
