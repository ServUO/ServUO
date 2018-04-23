using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public abstract class BaseDragoonJewel : KiastasBaseJewel
    {
        public override int ArtifactRarity { get { return Settings.BaseEquipmentAttribute.EquipmentArtifactRarity; } }
        //public override int BaseGemTypeNumber { get { return 1044221; } } // star sapphire

        public BaseDragoonJewel(int itemID, Layer layer) : base(itemID, layer)
        {
            GemType = GemType.StarSapphire;
        }

        public BaseDragoonJewel(Serial serial) : base(serial)
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