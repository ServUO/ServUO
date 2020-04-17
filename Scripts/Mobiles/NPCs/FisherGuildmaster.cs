namespace Server.Mobiles
{
    public class FisherGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public FisherGuildmaster()
            : base("fisher")
        {
            SetSkill(SkillName.Fishing, 80.0, 100.0);
        }

        public FisherGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.FishermensGuild;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}