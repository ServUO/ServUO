using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Ambitious
{
    public class AmbitiousQueenQuest : QuestSystem
    {
        private bool m_RedSolen;
        public AmbitiousQueenQuest(PlayerMobile from, bool redSolen)
            : base(from)
        {
            m_RedSolen = redSolen;
        }

        // Serialization
        public AmbitiousQueenQuest()
        {
        }

        public override object Name =>
                // Ambitious Solen Queen Quest
                1054146;
        public override object OfferMessage =>
                /* <I>The Solen queen considers you eagerly for a moment then says,</I><BR><BR>
* 
* Yes. Yes, I think you could be of use. Normally, of course, I would handle
* these things on my own, but these are busy times. Much to do, much to do.
* And besides, if I am to one day become the Matriarch, then it will be good to
* have experience trusting others to carry out various tasks for me. Yes.<BR><BR>
* 
* That is my plan, you see - I will become the next Matriarch. Our current
* Matriarch is fine and all, but she won't be around forever. And when she steps
* down, I intend to be the next in line. Ruling others is my destiny, you see.<BR><BR>
* 
* What I ask of you is quite simple. First, I need you to remove some of the
* - well - competition, I suppose. Though I dare say most are hardly competent to
* live up to such a title. I'm referring to the other queens of this colony,
* of course. My dear sisters, so to speak. If you could remove 5 of them, I would
* be most pleased. *sighs* By remove, I mean kill them. Don't make that face
* at me - this is how things work in a proper society, and ours has been more proper
* than most since the dawn of time. It's them or me, and whenever I give it
* any thought, I'm quite sure I'd prefer it to be them.<BR><BR>
* 
* I also need you to gather some zoogi fungus for me - 50 should do the trick.<BR><BR>
* 
* Will you accept my offer?
*/
                1054060;
        public override TimeSpan RestartDelay => TimeSpan.Zero;
        public override bool IsTutorial => false;
        public override int Picture => 0x15C9;
        public bool RedSolen => m_RedSolen;
        public static void GiveRewardTo(PlayerMobile player, ref bool bagOfSending, ref bool powderOfTranslocation, ref bool gold)
        {
            if (bagOfSending)
            {
                Item reward = new BagOfSending();

                if (player.PlaceInBackpack(reward))
                {
                    player.SendLocalizedMessage(1054074, "", 0x59); // You have been given a bag of sending.
                    bagOfSending = false;
                }
                else
                {
                    reward.Delete();
                }
            }

            if (powderOfTranslocation)
            {
                Item reward = new PowderOfTranslocation(Utility.RandomMinMax(10, 12));

                if (player.PlaceInBackpack(reward))
                {
                    player.SendLocalizedMessage(1054075, "", 0x59); // You have been given some powder of translocation.
                    powderOfTranslocation = false;
                }
                else
                {
                    reward.Delete();
                }
            }

            if (gold)
            {
                Item reward = new Gold(Utility.RandomMinMax(250, 350));

                if (player.PlaceInBackpack(reward))
                {
                    player.SendLocalizedMessage(1054076, "", 0x59); // You have been given some gold.
                    gold = false;
                }
                else
                {
                    reward.Delete();
                }
            }
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_RedSolen = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_RedSolen);
        }

        public override void Accept()
        {
            base.Accept();

            AddConversation(new AcceptConversation());
        }
    }
}
