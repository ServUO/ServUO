using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.JollyRoger
{
    public class Shamino : BaseQuester
    {
        public static Shamino InstanceTram { get; set; }
        public static Shamino InstanceFel { get; set; }

        [Constructable]
        public Shamino()
            : base("the Spirit")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;

            Utility.AssignRandomHair(this);
        }

        public override void InitOutfit()
        {
            SetWearable(new Tunic(), 2305);
            SetWearable(new Kilt(), 2305);
            SetWearable(new ThighBoots());
            SetWearable(new GoldNecklace());
            SetWearable(new GoldBracelet());
            SetWearable(new GoldRing());
            SetWearable(new GoldEarrings());
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3))
            {
                if (FellowshipMedallion.IsDressed(from))
                {
                    Gump g = new Gump(100, 100);
                    g.AddBackground(0, 0, 570, 295, 0x2454);
                    g.AddImage(0, 0, 0x9CD6);
                    g.AddHtmlLocalized(335, 24, 223, 261, 1159381, 0xC63, false, true); // *squeak squeak* Have you come to play the instruments? *squeak squeak* I bet you are as good as Iolo! “Practice! Practice!” he would say, “Practice your musical scales!” *squeak squeak* My fingers were too tiny for the lute, but I could sing! Do Re Mi Fa So La Ti Do! *squeak squeak*  <br><br>Long ago, I was supposed to perform at the wedding of Shamino and Princess Beatrix. Ready to go at noon on the nose I would have been, but that was before...everything. *frowns* Sometimes I still visit Castle Sallé Dacil through a secret door in Ilshenar. Even though the castle has seen better days, I am still reminded of the Pure Love Princess Beatrix had for Shamino! *squeak squeak*

                    from.SendGump(g);
                    from.PlaySound(1664);
                }
                else
                {
                    from.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159380,
                        from.NetState); // * You attempt to understand the spirit but your connection to them is weak... *
                }
            }
        }

        public Shamino(Serial serial)
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
