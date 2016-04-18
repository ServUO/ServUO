using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class JedahEntille : BaseQuester
    {
        [Constructable]
        public JedahEntille()
            : base("the Silent")
        {
        }

        public JedahEntille(Serial serial)
            : base(serial)
        {
        }

        public override int TalkNumber
        {
            get
            {
                return -1;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83FE;
            this.Female = true;
            this.Body = 0x191;
            this.Name = "Jedah Entille";
        }

        public override void InitOutfit()
        {
            this.HairItemID = 0x203C;
            this.HairHue = 0x6BE;

            this.AddItem(new PlainDress(0x528));
            this.AddItem(new ThighBoots());
            this.AddItem(new FloppyHat());
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}