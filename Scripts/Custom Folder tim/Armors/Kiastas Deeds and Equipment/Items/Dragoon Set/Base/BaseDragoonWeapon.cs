using System;
using Server;
using Server.Items;
using Server.Targets;

namespace Server.Kiasta
{
    public abstract class BaseDragoonWeapon : KiastasBaseWeapon
    {
        public override int ArtifactRarity { get { return Settings.BaseEquipmentAttribute.EquipmentArtifactRarity; } }

        public BaseDragoonWeapon(int itemID) : base(itemID)
        {
            Resource = Settings.BaseEquipmentAttribute.ResourceType[Utility.Random(6)];
        }

        public BaseDragoonWeapon(Serial serial) : base(serial)
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