using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Matriarch
{
    public class SolenMatriarchQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Matriarch.DontOfferConversation),
            typeof(Matriarch.AcceptConversation),
            typeof(Matriarch.DuringKillInfiltratorsConversation),
            typeof(Matriarch.GatherWaterConversation),
            typeof(Matriarch.DuringWaterGatheringConversation),
            typeof(Matriarch.ProcessFungiConversation),
            typeof(Matriarch.DuringFungiProcessConversation),
            typeof(Matriarch.FullBackpackConversation),
            typeof(Matriarch.EndConversation),
            typeof(Matriarch.KillInfiltratorsObjective),
            typeof(Matriarch.ReturnAfterKillsObjective),
            typeof(Matriarch.GatherWaterObjective),
            typeof(Matriarch.ReturnAfterWaterObjective),
            typeof(Matriarch.ProcessFungiObjective),
            typeof(Matriarch.GetRewardObjective)
        };
        private bool m_RedSolen;
        public SolenMatriarchQuest(PlayerMobile from, bool redSolen)
            : base(from)
        {
            this.m_RedSolen = redSolen;
        }

        // Serialization
        public SolenMatriarchQuest()
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
                // Solen Matriarch Quest
                return 1054147;
            }
        }
        public override object OfferMessage
        {
            get
            {
                if (IsFriend(this.From, this.RedSolen))
                {
                    /* <I>The Solen Matriarch smiles happily as you greet her.</I><BR><BR>
                    * 
                    * Hello again. It is always good to see a friend of our colony.<BR><BR>
                    * 
                    * Would you like me to process some zoogi fungus into powder of translocation
                    * for you? I would be happy to do so if you will first undertake a couple
                    * tasks for me.<BR><BR>
                    * 
                    * First, I would like for you to eliminate some infiltrators from the other
                    * solen colony. They are spying on my colony, and I fear for the safety of my
                    * people. They must be slain.<BR><BR>
                    * 
                    * After that, I must ask that you gather some water for me. Our water supplies
                    * are inadequate, so we must try to supplement our reserve using water vats here
                    * in our lair.<BR><BR>
                    * 
                    * Will you accept my offer?
                    */
                    return 1054083;
                }
                else
                {
                    /* <I>The Solen Matriarch smiles happily as she eats the seed you offered.</I><BR><BR>
                    * 
                    * I think you for that seed. I was quite delicious. So full of flavor.<BR><BR>
                    * 
                    * Hmm... if you would like, I could make you a friend of my colony. This would stop
                    * the warriors, workers, and queens of my colony from thinking you are an intruder,
                    * thus they would not attack you. In addition, as a friend of my colony I will process
                    * zoogi fungus into powder of translocation for you.<BR><BR>
                    * 
                    * To become a friend of my colony, I ask that you complete a couple tasks for me. These
                    * are the same tasks I will ask of you when you wish me to process zoogi fungus,
                    * by the way.<BR><BR>
                    * 
                    * First, I would like for you to eliminate some infiltrators from the other solen colony.
                    * They are spying on my colony, and I fear for the safety of my people. They must
                    * be slain.<BR><BR>
                    * 
                    * After that, I must ask that you gather some water for me. Our water supplies are
                    * inadequate, so we must try to supplement our reserve using water vats here in our
                    * lair.<BR><BR>
                    * 
                    * Will you accept my offer?
                    */
                    return 1054082;
                }
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
        public static bool IsFriend(PlayerMobile player, bool redSolen)
        {
            if (redSolen)
                return player.SolenFriendship == SolenFriendship.Red;
            else
                return player.SolenFriendship == SolenFriendship.Black;
        }

        public static bool GiveRewardTo(PlayerMobile player)
        {
            Gold gold = new Gold(Utility.RandomMinMax(250, 350));

            if (player.PlaceInBackpack(gold))
            {
                player.SendLocalizedMessage(1054076); // You have been given some gold.
                return true;
            }
            else
            {
                gold.Delete();
                return false;
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