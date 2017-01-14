using System;

namespace Server.Mobiles
{
    public class FisherGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public FisherGuildmaster()
            : base("fisher")
        {
            this.SetSkill(SkillName.Fishing, 80.0, 100.0);
        }

        public FisherGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.FishermensGuild;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}