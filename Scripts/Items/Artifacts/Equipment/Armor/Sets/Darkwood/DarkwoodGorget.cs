using Server.Engines.Craft;
using System;

namespace Server.Items
{
    [Flipable(0x2B69, 0x3160)]
    public class DarkwoodGorget : WoodlandGorget
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DarkwoodGorget()
            : base()
        {
            Hue = 0x455;
            SetHue = 0x494;

            Attributes.BonusHits = 2;
            Attributes.DefendChance = 5;

            SetAttributes.ReflectPhysical = 25;
            SetAttributes.BonusStr = 10;
            SetAttributes.NightSight = 1;

            SetSelfRepair = 3;

            SetPhysicalBonus = 2;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 3;
            SetEnergyBonus = 5;
        }

        public DarkwoodGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073483;// Darkwood Gorget
        public override SetItem SetID => SetItem.Darkwood;
        public override int Pieces => 6;
        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 5;
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
            if (resHue > 0)
                Hue = resHue;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            switch (Resource)
            {
                case CraftResource.Bloodwood:
                    Attributes.RegenHits = 2;
                    break;
                case CraftResource.Heartwood:
                    Attributes.Luck = 40;
                    break;
                case CraftResource.YewWood:
                    Attributes.RegenHits = 1;
                    break;
            }

            return 0;
        }
    }
}