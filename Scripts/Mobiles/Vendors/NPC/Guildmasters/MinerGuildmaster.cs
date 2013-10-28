using System;

namespace Server.Mobiles
{
    public class MinerGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public MinerGuildmaster()
            : base("miner")
        {
            this.SetSkill(SkillName.ItemID, 60.0, 83.0);
            this.SetSkill(SkillName.Mining, 90.0, 100.0);
        }

        public MinerGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.MinersGuild;
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