using Server.Items;

namespace Server.Engines.Quests
{
    public class Missing : BaseQuest
    {
        public Missing()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(ArielHavenWritofMembership), "Ariel Haven Writ of Membership", 4, 0x2831));

            AddReward(new BaseReward(typeof(CandlewoodTorch), "Candlewood Torch"));
        }

        public override bool DoneOnce => true;

        /* Missing */
        public override object Title => 1094949;

        /* Search the Underworld for the lost adventurers.  Either bring word of
		 * their progress or evidence of their fate to Elder Dugan for your reward.
		 * 
		 * Well met, traveler.  I am Elder Dugan, leader of the Society of Ariel
		 * Haven.  We are a society of prospectors who who have obtained writs of
		 * commission from Baron Almric to colonize this mountain.
		 * 
		 * After this mysterious mountain appeared complete with its own door...
		 * we sent in scouts and discovered that it is an ancient city, abandoned
		 * and overrun with wild creatures.  Thus, with the blessing of our Baron,
		 * we have come to claim and settle this new area and it will be called
		 * Ariel Haven.
		 * 
		 * We have been sending out adventuring parties to slowly push back the
		 * wild beasts and thus far we have managed to secure this hall. Yesterday
		 * we sent a party in further to find our next area to claim and they have
		 * not returned... I am becoming concerned.
		 * 
		 * In your travels in this mountain would you look for my people?  If they
		 * are well, please bring me word, if misfortune has befallen them, please
		 * bring me evidence of their fate.
		 */
        public override object Description => 1094951;

        /* Very well, fare thee well traveler.  I would not press our problems upon
		 * you if you are not willing.  I pray that my people are simply trying to
		 * secure some treasure they found.
		 */
        public override object Refuse => 1094952;

        /* Greetings, have you any news of my people?  I am encouraged to see you well!
		 */
        public override object Uncomplete => 1094953;

        /* Oh, this is indeed sad news.  It seems my worst fears have been realized!
		 * These writs were given to Evan and Kevin Brightwhistle, Sergio Taylor, and
		 * Sarah Bootwell.  Based on your description of the ghastly scene, they have
		 * met a most untimely end!  This is a great setback to our society as they
		 * were each great friends to me and an asset to the society.
		 * 
		 * 'Tis strange that there were only four bodies.... There was a fifth member
		 * of the party, Neville Brightwhistle, but he was the youngest and least
		 * experienced of the party so if his elder brothers are lost, surely young
		 * Neville met a similar fate. ‘Tis a tragedy, surely.
		 * 
		 * Please take this torch with my thanks.  It may not seem like much, but it
		 * is magic and will never burn out.  You will find that rotworms fear fire
		 * so it will protect you from them as you venture further into these cursed
		 * halls.
		 * 
		 * Tread carefully traveller, each member of the party had one of these so I
		 * suspect that the rotworms are not what ended their lives.
		 */
        public override object Complete => 1094956;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
