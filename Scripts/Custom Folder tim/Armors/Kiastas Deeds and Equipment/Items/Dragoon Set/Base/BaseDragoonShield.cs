using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public abstract class BaseDragoonShield : BaseShield
    {
        public BaseDragoonShield(int itemID) : base(itemID)
        {
            Resource = Settings.BaseEquipmentAttribute.ResourceType[Utility.Random(6)];
        }

        public BaseDragoonShield(Serial serial) : base(serial)
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