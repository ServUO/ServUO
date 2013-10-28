using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class EnshroudedFigure : BaseQuester
    {
        [Constructable]
        public EnshroudedFigure()
        {
        }

        public EnshroudedFigure(Serial serial)
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

            this.Hue = 0x8401;
            this.Female = false;
            this.Body = 0x190;
            this.Name = "enshrouded figure";
        }

        public override void InitOutfit()
        {
            this.AddItem(new DeathShroud());
            this.AddItem(new ThighBoots());
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