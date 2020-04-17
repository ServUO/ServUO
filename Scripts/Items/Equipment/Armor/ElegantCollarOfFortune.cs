using Server.Engines.Craft;
using System;

namespace Server.Items
{
    public class ElegantCollarOfFortune : ElegantCollar
    {
        public override int LabelNumber => 1159225;  // elegant collar of fortune
        public override bool IsArtifact => true;

        [Constructable]
        public ElegantCollarOfFortune()
            : base()
        {
            Attributes.Luck = 300;
            Attributes.RegenMana = 1;
        }

        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 15;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public ElegantCollarOfFortune(Serial serial)
            : base(serial)
        {
        }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            PlayerConstructed = true;

            if (makersMark)
                Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

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
    }
}
