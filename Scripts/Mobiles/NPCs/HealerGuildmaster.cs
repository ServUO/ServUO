using Server.Items;

namespace Server.Mobiles
{
    public class HealerGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public HealerGuildmaster()
            : base("healer")
        {
            SetSkill(SkillName.Anatomy, 85.0, 100.0);
            SetSkill(SkillName.Healing, 90.0, 100.0);
            SetSkill(SkillName.Forensics, 75.0, 98.0);
            SetSkill(SkillName.MagicResist, 75.0, 98.0);
            SetSkill(SkillName.SpiritSpeak, 65.0, 88.0);
        }

        public HealerGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.HealersGuild;
        public override VendorShoeType ShoeType => VendorShoeType.Sandals;
        public override void InitOutfit()
        {
            base.InitOutfit();

			SetWearable(new Robe(), Utility.RandomYellowHue(), 1);
        }

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