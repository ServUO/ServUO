using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Garamons Corpse")]
    public class Garamon : Mobile
    {
        [Constructable]
        public Garamon()
        {
            Str = 100;
            Int = 100;
            Dex = 100;

            Name = "Garamon";
            HairItemID = 0x2044;
            HairHue = 0x44E;
            FacialHairItemID = 0x44E;
            FacialHairHue = 1153;
            Body = 0x190;
            Hue = 33821;
            CantWalk = true;
            Direction = Direction.South;

            AddItem(new Shoes(1810));
            AddItem(new Robe(946));

            Blessed = true;
        }

        public Garamon(Serial serial)
            : base(serial)
        {
        }

        public virtual bool IsInvulnerable
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                HairItemID = 0x2044;
                HairHue = 0x44E;
                FacialHairItemID = 0x44E;
                FacialHairHue = 1153;
                Body = 0x190;
                Hue = 33821;
                CantWalk = true;
                Direction = Direction.South;

                Item item = FindItemOnLayer(Layer.OuterTorso);
                if (item != null)
                    item.Hue = 946;

                item = FindItemOnLayer(Layer.Shoes);
                if (item != null)
                    item.Hue = 1810;
            }
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(Location, 8))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(Location, 2))
            {
                PlayerMobile pm = e.Mobile as PlayerMobile;

                if (pm.AbyssEntry)
                {
                    pm.SendMessage("You have completed a Sacred quest already!");
                }
                else
                {
                    string keyword = e.Speech;

                    switch (keyword)
                    {
                        case "Hello":
                            {
                                Say(String.Format("Greetings Adventurer! If you are seeking to enter the Abyss, I may be of assitance to you."));
                                break;
                            }
                        case "hello":
                            {
                                Say(String.Format("Greetings Adventurer! If you are seeking to enter the Abyss, I may be of assitance to you."));
                                break;
                            }
                        case "Key":
                            {
                                Say(String.Format("It's three parts that you must find, and reunite as one!"));
                                break;
                            }
                        case "key":
                            {
                                Say(String.Format("It's three parts that you must find, and reunite as one!"));
                                break;
                            }
                        case "Abyss":
                            {
                                Say(String.Format("It's entrance is protected by stone guardians who will only grant passage to the carrier of a Tripartite Key!"));
                                break;
                            }
                        case "abyss":
                            {
                                Say(String.Format("It's entrance is protected by stone guardians who will only grant passage to the carrier of a Tripartite Key!"));
                                break;
                            }
                    }
                }
                base.OnSpeech(e);
            }
        }
    }
}