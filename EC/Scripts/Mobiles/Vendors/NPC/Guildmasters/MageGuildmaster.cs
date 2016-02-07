using System;

namespace Server.Mobiles
{
    public class MageGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public MageGuildmaster()
            : base("mage")
        {
            this.SetSkill(SkillName.EvalInt, 85.0, 100.0);
            this.SetSkill(SkillName.Inscribe, 65.0, 88.0);
            this.SetSkill(SkillName.MagicResist, 64.0, 100.0);
            this.SetSkill(SkillName.Magery, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 60.0, 83.0);
            this.SetSkill(SkillName.Meditation, 85.0, 100.0);
            this.SetSkill(SkillName.Macing, 36.0, 68.0);
        }

        public MageGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.MagesGuild;
            }
        }
        public override VendorShoeType ShoeType
        {
            get
            {
                return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals;
            }
        }
        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.Robe(Utility.RandomBlueHue()));
            this.AddItem(new Server.Items.GnarledStaff());
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