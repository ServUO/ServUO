using Server;
using Server.Commands;
using ChatterBotAPI;
using System.Threading;
using System.Web;
using Server.Items;
using System;
using System.Collections.Generic;


namespace Bittiez.CleverBot
{

    public class CleverAI : Mobile
    {
        private ChatterBotFactory factory;
        private ChatterBot bot1;
        private ChatterBotSession bot1session;
        private List<chatter> chatters = new List<chatter>();
        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.Alive)
                return true;
            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile.Alive && InRange(e.Mobile, 4) && listentomobile(e.Mobile))
            {
                Direction = GetDirectionTo(e.Mobile);
                ThinkIt(e.Speech);
            }
            else
            {
                //this.Say("No.. no.. I clean your house.");
            }
        }

        public CleverAI(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public CleverAI()
        {

            factory = new ChatterBotFactory();
            bot1 = factory.Create(ChatterBotType.CLEVERBOT);
            bot1session = bot1.CreateSession();

            InitStats(100, 100, 25);
            Title = "the town blabber";
            Hue = Utility.RandomSkinHue();

            if (!Core.AOS)
                NameHue = 0x35;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
            }

            AddItem(new FancyShirt(Utility.RandomBlueHue()));

            Item skirt;

            switch (Utility.Random(2))
            {
                case 0: skirt = new Skirt(); break;
                default:
                case 1: skirt = new Kilt(); break;
            }

            skirt.Hue = Utility.RandomGreenHue();

            AddItem(skirt);

            AddItem(new FeatheredHat(Utility.RandomGreenHue()));

            Item boots;

            switch (Utility.Random(2))
            {
                case 0: boots = new Boots(); break;
                default:
                case 1: boots = new ThighBoots(); break;
            }

            AddItem(boots);

            Utility.AssignRandomHair(this);
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public void ThinkIt(string s)
        {
            thoughargs ss = new thoughargs(s);
            ss.Text = s;
            Thread clientThread = new Thread(new ParameterizedThreadStart(on_think));
            clientThread.Start(ss);
        }

        public void on_think(object s)
        {
            if (s is thoughargs && bot1session != null)
            {

                thoughargs args = (thoughargs)s;
                string ss = bot1session.Think(args.Text);
                //Console.WriteLine(bot1session.Think(args.Text));
                this.Say("" + ss);
                //this.Say("[You Said: " + args.Text + "]");
            }
            else
            {
                factory = new ChatterBotFactory();
                bot1 = factory.Create(ChatterBotType.CLEVERBOT);
                bot1session = bot1.CreateSession();
                this.Say("No.. no.. More lemon pledge...");
            }
        }

        private bool listentomobile(Mobile from)
        {
            foreach (chatter ch in chatters)
            {
                if (ch.MOBILE == from)
                {
                    DateTime m = DateTime.UtcNow;
                    if (m > ch.LASTCHAT.AddSeconds(15))
                    {
                        ch.LASTCHAT = m;
                        return true;
                    }
                    else return false;
                }
            }

            chatters.Add(new chatter(from, DateTime.UtcNow));
            return true;
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
            factory = new ChatterBotFactory();
            bot1 = factory.Create(ChatterBotType.CLEVERBOT);
            bot1session = bot1.CreateSession();
            if (Core.AOS && NameHue == 0x35)
                NameHue = -1;
        }



    }

    public class thoughargs
    {
        public string Text { get { return text; } set { text = value; } }
        private string text;
        public thoughargs(string txt)
        {
            text = txt;
        }
    }

    public class chatter
    {
        public Mobile MOBILE { get{return m;} set{m = value;} }
        private Mobile m;
        public DateTime LASTCHAT { get { return l; } set { l = value; } }
        private DateTime l;
        public chatter(Mobile mobile, DateTime datetime)
        {
            m = mobile;
            l = datetime;
        }
    }
}