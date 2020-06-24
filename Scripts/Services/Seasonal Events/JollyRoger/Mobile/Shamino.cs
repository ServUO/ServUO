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
                    g.AddImage(0, 0, 0x9D3D);
                    g.AddHtmlLocalized(335, 24, 223, 261, 1159379, 0xC63, false, true); // Tis true, I am unlike the spirits you will see roaming nearby - the Well of Souls has allowed me a more robust form between the plane of reality and the ethereal void. Tis no matter though, I return to Britannia only to remember my beloved Beatrix. My how beautiful my Beatrix was... We were to be wed you know! <br><br>What a wedding it would have been! Castle Sallé Dacil was decorated in full regalia and the ceremony was to begin promptly at noon. Elegantly carved swans, a feast set for a king, and a generous gift from our Forgotten King - a magnificent sandalwood box! <br><br>Alas, none of that happened. I was rushed off to the war against Mondain in service of our Forgotten King. <br><br>Ah, I remember my first meeting with him well, I nearly chopped off my own leg from the shock!  Offworld travelers were a rarity then, those would be crowned king even more so!  Little about him was ordinary, right up to his coronation - tis no small feat to assemble such a crowd at midnight! It was very important to pay attention to the clocks so we wouldn't miss the spectacle, but many were in attendance to rejoice in the occasion despite the late hour.<br><br>Time is a funny thing - and my time in the ethereal plane has allowed me to see what would have been. *gestures to the glowing Well* I am afraid in this age of uncertainty, travel to the ethereal void via the Well of Souls is most perilous. Only those bearing the blessing of the Forgotten King will be able to pass. <br><br>And pass you must, for the Time Lord, Hawkwind, has been bound there by dark magics, and only a mortal being, virtuous and true, will be able to free him!  Even bound, time is a fickle thing and the influence of its agent no doubt still at hand. Your quest on this timeline is the one true way forward...<br><br>Thank you for listening to me reminisce. It is especially nice to remember my beloved Beatrix. *smiles*<br>

                    from.SendGump(g);
                    from.PlaySound(1664);
                }
                else
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159380,
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
