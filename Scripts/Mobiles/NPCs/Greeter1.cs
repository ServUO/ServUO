using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Mobiles
{
    public class Greeter1 : BaseGuildmaster
    {
        private static bool m_Talked;// flag to prevent spam 
        readonly string[] kfcsay = new string[] // things to say while greating 
        {
            "Greetings Adventurer! If you are seeking to enter the Abyss, I may be of assitance to you.",
        };
        [Constructable]
        public Greeter1()
            : base("Greeter1")
        {
        }

        public Greeter1(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.TailorsGuild;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 100);
            this.Name = "Garamon";
            this.Body = 0x190;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Robe(1));
            this.AddItem(new Sandals(1));

            this.HairItemID = 0x203B;
            this.HairHue = 0;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_Talked == false)
            {
                if (m.InRange(this, 2))
                {
                    m_Talked = true;
                    SayRandom(this.kfcsay, this);
                    this.Move(this.GetDirectionTo(m.Location));

                    SpamTimer t = new SpamTimer();
                    t.Start();
                }
            }
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 2))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from is PlayerMobile && from.InRange(this.Location, 2) && e.Speech.Contains("abyss"))
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (e.Speech.Contains("abyss"))
                    this.SayTo(from, "It's entrance is protected by stone guardians, who will only grant passage to the carrier of a Tripartite Key!");

                else if (e.Speech.Contains("key"))
                    this.SayTo(from, "It's three parts you must find and re-unite as one!");

                e.Handled = true;
            }
            base.OnSpeech(e);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new Greeter1Entry(from, this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private static void SayRandom(string[] say, Mobile m)
        {
            m.Say(say[Utility.Random(say.Length)]);
        }

        public class Greeter1Entry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_Giver;
            public Greeter1Entry(Mobile from, Mobile giver)
                : base(6146, 3)
            {
                this.m_Mobile = from;
                this.m_Giver = giver;
            }
        }

        private class SpamTimer : Timer
        {
            public SpamTimer()
                : base(TimeSpan.FromSeconds(90))
            {
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Talked = false;
            }
        }
    }
}