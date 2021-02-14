using System;

namespace Server.Mobiles
{
    public class HPKraken : Kraken
    {
        [Constructable]
        public HPKraken()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(30), Delete);
        }

        public HPKraken(Serial serial) : base(serial)
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
            reader.ReadInt();

            Delete();
        }
    }

    public class HPDeepSeaSerpent : DeepSeaSerpent
    {
        [Constructable]
        public HPDeepSeaSerpent()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(30), Delete);
        }

        public HPDeepSeaSerpent(Serial serial) : base(serial)
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
            reader.ReadInt();

            Delete();
        }
    }

    public class HPSeaSerpent : SeaSerpent
    {
        [Constructable]
        public HPSeaSerpent()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(30), Delete);
        }

        public HPSeaSerpent(Serial serial) : base(serial)
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
            reader.ReadInt();

            Delete();
        }
    }

    public class HPWaterElemental : WaterElemental
    {
        [Constructable]
        public HPWaterElemental()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(30), Delete);
        }

        public HPWaterElemental(Serial serial) : base(serial)
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
            reader.ReadInt();

            Delete();
        }
    }
}
