using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Matriarch
{
    public class SolenMatriarchQuest : QuestSystem
    {
        private bool m_RedSolen;
        public SolenMatriarchQuest(PlayerMobile from, bool redSolen)
            : base(from)
        {
            m_RedSolen = redSolen;
        }

        // Serialization
        public SolenMatriarchQuest()
        {
        }

        public override object Name =>
                // Solen Matriarch Quest
                1054147;
        public override object OfferMessage
        {
            get
            {
                if (IsFriend(From, RedSolen))
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
        public override TimeSpan RestartDelay => TimeSpan.Zero;
        public override bool IsTutorial => false;
        public override int Picture => 0x15C9;
        public bool RedSolen => m_RedSolen;
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
