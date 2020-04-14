using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Fellowship
{
    public class Follower : BaseQuester
    {
        public static Follower InstanceTram { get; set; }
        public static Follower InstanceFel { get; set; }

        [Constructable]
        public Follower()
            : base("the Follower")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            SetWearable(new Surcoat(), 1255);
            SetWearable(new LongPants(), 2722);
            SetWearable(new Kilt(), 2012);
            SetWearable(new Boots());
            SetWearable(new GoldEarrings());
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 3;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            if (!player.HasGump(typeof(FollowerGump)))
            {
                player.SendGump(new FollowerGump());
            }
        }

        public class FollowerGump : Gump
        {
            public FollowerGump()
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 720, 285, 0x2454);
                AddImage(0, 0, 0x6CF);
                AddHtmlLocalized(300, 24, 408, 251, 1159043, 0xC63, false, true); // Welcome to the Fellowship Hall.  It isn't much, but we've been able to help the many refugees from across Britannia that Hook and his vile minions have displaced with their plundering ways! If you'd like to make a donation, simply drop your trade cargo in the donation box. With enough donations you too can become a member of the Fellowship!<BR><BR><br>If you have taken up the charge of cleansing Britannia's souls of the vile evils that plague our society, you may deposit the soulbinders in the chest here for cleansing by our Adepts.
            }
        }

        public Follower(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
