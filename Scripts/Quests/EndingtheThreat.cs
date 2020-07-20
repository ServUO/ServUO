using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class EndingtheThreat : BaseQuest
    {
        public EndingtheThreat()
            : base()
        {
            AddObjective(new SlayObjective(typeof(GrayGoblin), "gray goblin", 10, 1095190, "Abyss"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072583));
        }

        public override bool DoneOnce => true;

        /* Ending the Threat */
        public override object Title => 1095012;

        /* Travel into the Stygian Abyss and kill Gray Goblins until they are subdued and will not attack
         * the settlers anymore.<br><center>-----</center><br>You have done much for us and I need you to
         * be our champion again.  We cannot sleep in these halls as long as the goblins threaten our safety.
         * Return to the place where you found Neville and kill the gray goblins until they cower before you.
         * Let them know we have a champion and they must seek elsewhere for a query.  If you do this for us,
         * I will reward you with our highest honor.
         */
        public override object Description => 1095014;

        /*So be it, traveler.  I thank you for the courage you have shown in the past.
         * Speak to me again if you change your mind.
         */
        public override object Refuse => 1095015;

        /*The goblins must learn again to fear humans and elves. They can be cowed into submission,
         * but if it is not done quickly they will become emboldened and start to venture out of
         * the mountain to the villages and other settlements. You must not let that happen.
         */
        public override object Uncomplete => 1095016;

        /*Oh, have mercy on us!  Have you come to kill every one of us? 
        Take what you will and go!  Your kind is more terrible than the mistress!  
        Woe are we, the gray goblins, we serve the mistress and yet she leaves us to be...*/
        public override object Complete => 1095017;

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
