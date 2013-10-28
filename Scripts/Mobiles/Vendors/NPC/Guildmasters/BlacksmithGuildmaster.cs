using System;

namespace Server.Mobiles
{
    public class BlacksmithGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public BlacksmithGuildmaster()
            : base("blacksmith")
        {
            this.SetSkill(SkillName.ArmsLore, 65.0, 88.0);
            this.SetSkill(SkillName.Blacksmith, 90.0, 100.0);
            this.SetSkill(SkillName.Macing, 36.0, 68.0);
            this.SetSkill(SkillName.Parry, 36.0, 68.0);
        }

        public BlacksmithGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.BlacksmithsGuild;
            }
        }
        public override bool IsActiveVendor
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return true;
            }
        }
        public override VendorShoeType ShoeType
        {
            get
            {
                return VendorShoeType.ThighBoots;
            }
        }
        public override void InitSBInfo()
        {
            this.SBInfos.Add(new SBBlacksmith());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            Item item = (Utility.RandomBool() ? null : new Server.Items.RingmailChest());

            if (item != null && !this.EquipItem(item))
            {
                item.Delete();
                item = null;
            }

            if (item == null)
                this.AddItem(new Server.Items.FullApron());

            this.AddItem(new Server.Items.Bascinet());
            this.AddItem(new Server.Items.SmithHammer());
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