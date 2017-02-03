using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class EndingtheThreat : BaseQuest
    {
        public EndingtheThreat()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(GrayGoblin), "Gray Goblin", 10));

            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        public override object Title
        {
            get
            {
                return 1095012;
            }
        }//Ending the Threat

        /*So be it, traveler.  I thank you for the courage you have shown in the past.  Speak to me again if you change your mind.*/
        public override object Description
        {
            get
            {
                return 1095014;
            }
        }
        /*So be it, traveler.  I thank you for the courage you have shown in the past.  Speak to me again if you change your mind.*/
        public override object Refuse
        {
            get
            {
                return 1095015;
            }
        }
        /*The goblins must learn again to fear humans and elves.
        They can be cowed into submission, but if it is not done quickly they will become emboldened and start to venture out of the mountain to the villages and other settlements.  
        You must not let that happen.*/
        public override object Uncomplete
        {
            get
            {
                return 1095016;
            }
        }
        /*Oh, have mercy on us!  Have you come to kill every one of us? 
        Take what you will and go!  Your kind is more terrible than the mistress!  
        Woe are we, the gray goblins, we serve the mistress and yet she leaves us to be...*/
        public override object Complete
        {
            get
            {
                return 1095017;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}