using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVEpaulette : Epaulette
    {
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public VvVEpaulette()
        {
            Attributes.AttackChance = 5;
        }

        public VvVEpaulette(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();


            if (version == 0)
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
        }
    }

    public class VvVGargishEpaulette : GargishEpaulette
    {
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public VvVGargishEpaulette()
        {
            Attributes.AttackChance = 5;
        }

        public VvVGargishEpaulette(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();


            if (version == 0)
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
        }
    }
}