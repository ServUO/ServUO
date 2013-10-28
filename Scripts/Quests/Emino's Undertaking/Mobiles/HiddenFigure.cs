using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Ninja
{
    public class HiddenFigure : BaseQuester
    {
        public static int[] Messages = new int[]
        {
            1063191, // They won’t find me here.
            1063192  // Ah, a quiet hideout.
        };
        private int m_Message;
        [Constructable]
        public HiddenFigure()
        {
            this.m_Message = Utility.RandomList(Messages);
        }

        public HiddenFigure(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Message
        {
            get
            {
                return this.m_Message;
            }
            set
            {
                this.m_Message = value;
            }
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

            this.Hue = Utility.RandomSkinHue();

            this.Female = Utility.RandomBool();

            if (this.Female)
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
            }
        }

        public override void InitOutfit()
        {
            Utility.AssignRandomHair(this);

            this.AddItem(new TattsukeHakama(this.GetRandomHue()));
            this.AddItem(new Kasa());
            this.AddItem(new HakamaShita(this.GetRandomHue()));

            if (Utility.RandomBool())
                this.AddItem(new Shoes(this.GetShoeHue()));
            else
                this.AddItem(new Sandals(this.GetShoeHue()));
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 3;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            this.PrivateOverheadMessage(MessageType.Regular, 0x3B2, this.m_Message, player.NetState);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)this.m_Message);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Message = reader.ReadInt();
        }
    }
}