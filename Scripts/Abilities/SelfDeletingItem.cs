// Created by Peoharen for the Mobile Abilities Package.
using System;

namespace Server.Items
{
    public class SelfDeletingItem : Item
    {
        [Constructable]
        public SelfDeletingItem(int id, string name, int duration)
            : base(8391)
        {
            this.Weight = 1.0;
            this.ItemID = id;
            this.Name = "name";
            this.Movable = false;

            Timer.DelayCall(TimeSpan.FromSeconds(duration), new TimerCallback(Expire));
        }

        public SelfDeletingItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerCallback(Expire));
        }

        private void Expire()
        {
            if (this.Deleted)
                return;

            this.Delete();
        }
    }
}