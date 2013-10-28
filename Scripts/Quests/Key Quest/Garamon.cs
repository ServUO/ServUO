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
            this.Name = "Garamon";
            this.HairItemID = 0x2044;
            this.HairHue = 1153;
            this.FacialHairItemID = 0x204B;
            this.FacialHairHue = 1153;
            this.Body = 0x190;
            this.CantWalk = true;

            this.AddItem(new Server.Items.Sandals(927));
            this.AddItem(new Robe(927));

            this.Blessed = true;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 8))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(this.Location, 2))
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
                                this.Say(String.Format("Greetings Adventurer! If you are seeking to enter the Abyss, I may be of assitance to you."));
                                break;
                            }
                        case "hello":
                            {
                                this.Say(String.Format("Greetings Adventurer! If you are seeking to enter the Abyss, I may be of assitance to you."));
                                break;
                            }
                        case "Key":
                            {
                                this.Say(String.Format("It's three parts that you must find, and reunite as one!"));
                                break;
                            }
                        case "key":
                            {
                                this.Say(String.Format("It's three parts that you must find, and reunite as one!"));
                                break;
                            }
                        case "Abyss":
                            {
                                this.Say(String.Format("It's entrance is protected by stone guardians who will only grant passage to the carrier of a Tripartite Key!"));
                                break;
                            }
                        case "abyss":
                            {
                                this.Say(String.Format("It's entrance is protected by stone guardians who will only grant passage to the carrier of a Tripartite Key!"));
                                break;
                            }
                    /*case "Britain":
                    {
                    this.Direction =  GetDirectionTo( pm.Location );
                    Say( String.Format( "Britain is far North of here.. I have not been there since I was a child." ) );
                    break;
                    }
                    case "britain":
                    {
                    this.Direction =  GetDirectionTo( pm.Location );
                    Say( String.Format( "Britain is far North of here.. I have not been there since I was a child." ) );
                    break;
                    }
                    case "Moongate":
                    {
                    this.Direction =  GetDirectionTo( pm.Location );
                    Say( String.Format( "There is a Moongate South of Trinsic in the Forest." ) );
                    break;
                    }
                    case "moongate":
                    {
                    this.Direction =  GetDirectionTo( pm.Location );
                    Say( String.Format( "There is a Moongate South of Trinsic in the Forest." ) );
                    break;
                    }*/
                    }
                }
                base.OnSpeech(e);
            }
        }
    }
}