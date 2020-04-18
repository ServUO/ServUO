using Server.Engines.Despise;

namespace Server.Items
{
    public class PutridHeart : Item
    {
        public override int LabelNumber => 1153424;  // putrid heart

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
            Hue = 2599;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Deleted && DespiseController.Instance != null)
            {
                //DespiseController.Instance.AddDespisePoints(from, this);
                Engines.Points.PointsSystem.DespiseCrystals.AwardPoints(from, Amount);
                Delete();
            }
        }

        public PutridHeart(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}
