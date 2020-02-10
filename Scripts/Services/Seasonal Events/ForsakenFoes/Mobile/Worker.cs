using System;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Engines.Fellowship
{
    public class Worker : BaseQuester
    {
        public static Worker InstanceTram { get; set; }
        public static Worker InstanceFel { get; set; }

        [Constructable]
        public Worker()
            : base("the Worker")
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
            SetWearable(new FullApron(), 902);
            SetWearable(new HalfApron(), 946);
            SetWearable(new Shirt(), 1513);
            SetWearable(new LeatherGloves());
            SetWearable(new Boots(), 2013);
            SetWearable(new ShortPants());
            SetWearable(new SmithHammer());
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && InRange(m.Location, 5))
            {
                if (!m.HasGump(typeof(WorkerGump)))
                {
                    m.SendGump(new WorkerGump());
                }
            }
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            if (!player.HasGump(typeof(WorkerGump)))
            {
                player.SendGump(new WorkerGump());
            }
        }

        public class WorkerGump : Gump
        {
            public WorkerGump()
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 620, 328, 0x2454);
                AddImage(0, 0, 0x61A);
                AddHtmlLocalized(335, 14, 273, 18, 1114513, "#1159237", 0xC63, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(335, 51, 273, 267, 1159238, 0xC63, false, true); // This castle stinks! Miracle the collapse in the sewers didn't bring the whole place down! Since the cave in, the sewer has flooded most of the dungeon. May have been a bit dangerous before with all the critters and what not running around, but now it's a death trap! Crew's been working at it nonstop to contain the leaks and sure up what's left. Mages got down here and setup a teleporter network to get us to where it's dry. Gotta insist you stay back though, too dangerous to let civilians in!
            }
        }

        public Worker(Serial serial)
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
