using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ambitious
{
    public class AmbitiousQueenQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Ambitious.DontOfferConversation),
            typeof(Ambitious.AcceptConversation),
            typeof(Ambitious.DuringKillQueensConversation),
            typeof(Ambitious.GatherFungiConversation),
            typeof(Ambitious.DuringFungiGatheringConversation),
            typeof(Ambitious.EndConversation),
            typeof(Ambitious.FullBackpackConversation),
            typeof(Ambitious.End2Conversation),
            typeof(Ambitious.KillQueensObjective),
            typeof(Ambitious.ReturnAfterKillsObjective),
            typeof(Ambitious.GatherFungiObjective),
            typeof(Ambitious.GetRewardObjective)
        };
        private bool m_RedSolen;
        public AmbitiousQueenQuest(PlayerMobile from, bool redSolen)
            : base(from)
        {
            this.m_RedSolen = redSolen;
        }

        // Serialization
        public AmbitiousQueenQuest()
        {
        }

        public override Type[] TypeReferenceTable
        {
            get
            {
                return m_TypeReferenceTable;
            }
        }
        public override object Name
        {
            get
            {
                // Ambitious Solen Queen Quest
                return 1054146;
            }
        }
        public override object OfferMessage
        {
            get
            {
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
                return 1054060;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override bool IsTutorial
        {
            get
            {
                return false;
            }
        }
        public override int Picture
        {
            get
            {
                return 0x15C9;
            }
        }
        public bool RedSolen
        {
            get
            {
                return this.m_RedSolen;
            }
        }
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

            this.m_RedSolen = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_RedSolen);
        }

        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }
    }
}