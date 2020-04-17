using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Khaldun
{
    public class Cryptologist : BaseVendor
    {
        public static Cryptologist TramInstance { get; set; }

        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new Cryptologist();
                TramInstance.MoveToWorld(new Point3D(4325, 949, 10), Map.Trammel);
                TramInstance.Direction = Direction.South;
            }
        }

        public Cryptologist()
            : base("the Cryptologist")
        {
        }

        public override void InitBody()
        {
            Name = NameList.RandomName("male");
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Race = Race.Human;
            Hue = Race.RandomSkinHue();
            HairHue = Race.RandomHairHue();
            FacialHairHue = Race.RandomHairHue();
            HairItemID = Race.RandomHair(false);
            FacialHairItemID = Race.RandomFacialHair(false);
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new HakamaShita());
            SetWearable(new ShortPants(), 1157);
            SetWearable(new Obi(), 1157);
            SetWearable(new Sandals());
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && InRange(m.Location, 5))
            {
                GoingGumshoeQuest3 quest = QuestHelper.GetQuest<GoingGumshoeQuest3>((PlayerMobile)m);

                if (quest != null)
                {
                    if (!quest.FoundCipherBook)
                    {
                        m.SendLocalizedMessage(1158620, null, 0x23); /*You've spoken to the Cryptologist who has agreed to help you if you acquire the Cipher Text.*/
                        m.SendGump(new InternalGump(1158619));
                    }
                    else
                    {
                        m.SendLocalizedMessage(1158621, null, 0x23); /*The Cytologist has successfully begun decrypting the copies of the books you found. He informs you he
                                                                      * will send them to Headquarters when he is finished. Return to Inspector Jasper to follow up on the case.*/
                        m.SendGump(new InternalGump(1158624));

                        m.PlaySound(quest.UpdateSound);
                        quest.BegunDecrypting = true;
                    }
                }
                else
                {
                    SayTo(m, 1073989, 1154);
                    Effects.PlaySound(Location, Map, 0x441);
                }
            }
        }

        private class InternalGump : Gump
        {
            public InternalGump(int cliloc)
                : base(50, 50)
            {
                AddBackground(0, 0, 720, 285, 9300);
                AddImage(0, 0, 1744);
                AddHtmlLocalized(300, 25, 408, 250, cliloc, false, false);

                /*Shhh! Can't you see the students are...*the Cryptologist pauses as you explain the reason for your visit* 
                 * Oh, Inspector Jasper sent you did he? Well my service to the RBG is something I hold in -very- high regard.
                 * What can I assist you with? *You show the Cryptologist your copies of the encrypted notes* Yes...yes...a
                 * masterful cipher if I have ever seen one, but one that can easily be cracked! The only thing we need is the
                 * cipher text - but just as it were a number of our texts have been hidden by some students having a bit of 
                 * fun with the season. I've already searched the dormitories out back, but I'm certain it's somewhere in the
                 * Lycaeum bookcases, if you find it, I'll be happy to assist you in decrypting the texts.  */

                // or //

                /*Wait till I catch the students who did this! Well, I must thank you for recovering this text, it's very useful
                 * in my daily studies. No matter, I will begin decryption of the copies of these clues you gave me. I'll send a
                 * courier to headquarters when I am through. Thank you again for your assistance! */
            }
        }

        public Cryptologist(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
        }
    }
}
