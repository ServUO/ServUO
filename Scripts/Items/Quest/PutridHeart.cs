using Server;
using System;
using Server.Engines.Despise;

namespace Server.Items
{
    public class PutridHeart : Item
    {
        public override int LabelNumber { get { return 1153424; } } // putrid heart

        [Constructable]
        public PutridHeart()
            : this(1)
        {
        }

        [Constructable]
        public PutridHeart(int amount)
            : base(0xF91)
        {
            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.Deleted && DespiseController.Instance != null)
            {
                DespiseController.Instance.AddDespisePoints(from, this);
            }
        }

        public PutridHeart(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}