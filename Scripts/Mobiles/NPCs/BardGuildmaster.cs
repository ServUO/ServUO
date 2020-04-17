namespace Server.Mobiles
{
    public class BardGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public BardGuildmaster()
            : base("bard")
        {
            SetSkill(SkillName.Archery, 80.0, 100.0);
            SetSkill(SkillName.Discordance, 80.0, 100.0);
            SetSkill(SkillName.Musicianship, 80.0, 100.0);
            SetSkill(SkillName.Peacemaking, 80.0, 100.0);
            SetSkill(SkillName.Provocation, 80.0, 100.0);
            SetSkill(SkillName.Swords, 80.0, 100.0);
        }

        public BardGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.BardsGuild;
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