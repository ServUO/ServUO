namespace Server.Mobiles
{
    [CorpseName("a drake corpse")]
    public class FrostDrake : ColdDrake
    {
        [Constructable]
        public FrostDrake()
        {
            Name = "a cold drake";
        }

        public FrostDrake(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}