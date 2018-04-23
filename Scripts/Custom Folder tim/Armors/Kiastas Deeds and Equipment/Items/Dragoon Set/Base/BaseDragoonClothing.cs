using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public abstract class BaseDragoonClothing : KiastasBaseClothing
    {
        public override int ArtifactRarity { get { return Settings.BaseEquipmentAttribute.EquipmentArtifactRarity; } }

        public BaseDragoonClothing(int itemID, Layer layer) : this(itemID, layer, 0)
        {
            Resource = Settings.BaseEquipmentAttribute.ResourceType[Utility.Random(6)];
        }

        public BaseDragoonClothing(int itemID, Layer layer, int hue) : base(itemID, layer, hue)
        {
        }

        public BaseDragoonClothing(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}