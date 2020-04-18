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
            FacialHairItemID = 0x204B;
            HairHue = 0x44E;
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

        public virtual bool IsInvulnerable => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 1)
            {
                FacialHairItemID = 0x204B;
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
                    pm.SendMessage("You have completed the Sacred Quest already!");
                }
                else
                {
                    string keyword = e.Speech.ToLower();

                    switch (keyword)
                    {
                        case "hello":
                            {
                                Say("Greetings Adventurer! If you are seeking to enter the Abyss, I may be of assitance to you.");
                                break;
                            }
                        case "secret":
                            {
                                Say("He who pays close attention to the walls may notice something unusual.");
                                break;
                            }
                        case "teleporter":
                            {
                                Say("You will find many within the dungeon. They will facilitate your travels.");
                                break;
                            }
                        case "vines":
                            {
                                Say("Aaah yes! Tricky things they are. Try to find something that could burn through them.");
                                break;
                            }
                        case "burn":
                            {
                                Say("I can tell you right away it's not fire based. Surely something within the dungeon will yield what you need.");
                                break;
                            }
                        case "abyss":
                            {
                                Say("It's entrance is protected by stone guardians who will only grant passage to the carrier of a Tripartite Key!");
                                break;
                            }
                        case "stone guardian":
                            {
                                Say("They will not let you enter the Abyss unless you can present a Tripartite Key");
                                break;
                            }
                        case "key":
                            {
                                Say("It's three parts that you must find, and reunite as one!");
                                break;
                            }
                        case "parts":
                            {
                                Say("Two can be found hidden in secret rooms within the Underworld. The third you must take from a shadow of evil.");
                                break;
                            }
                        case "shadow of evil":
                            {
                                Say("A most foul traitor. Once you have the first two parts, challenge him for the third! He dwells beyond the void in the Shrine.");
                                break;
                            }
                        case "shrine":
                            {
                                Say("Find your way there through the dungeon. You must use a teleporter to reach it.");
                                break;
                            }
                    }
                }
                base.OnSpeech(e);
            }
        }
    }
}