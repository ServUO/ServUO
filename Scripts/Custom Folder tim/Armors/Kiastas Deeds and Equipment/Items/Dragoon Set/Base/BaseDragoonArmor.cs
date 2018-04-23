﻿using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public abstract class BaseDragoonArmor : KiastasBaseArmor
    {
        public BaseDragoonArmor() : base(0)
        {
        }

        public BaseDragoonArmor(int itemID) : base(itemID)
        {
            Resource = Settings.BaseEquipmentAttribute.ResourceType[Utility.Random(6)];
        }
        public BaseDragoonArmor(Serial serial) : base(serial)
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