using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class TastyTreats : BaseQuest
    {
        public TastyTreats()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            this.AddObjective(new ObtainObjective(typeof(Dough), "Dough", 1, 0x103D));

            this.AddReward(new BaseReward(typeof(TastyTreat), "Tasty Treat"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(MetalHead);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(30);
            }
        }
        /*Tasty Treats*/
        public override object Title
        {
            get
            {
                return 1112774;
            }
        }
        /*Ah, yes, welcome to our humble shop. Do you wish to buy some of our fine potions today, or perhaps have something of interest to sell?
        No? Well, I do have some specialty goods for sale that may be of interest to you. Unfortunately, specialty goods require specialty ingredients, 
        which can be harder to come by. I'm not the adventurous sort, so if you are interested, you'll have to bring them to me.
        Pets can be finicky eaters at times, but I have just the solution for that. I call them 'Tasty Treats', and they're sure to please your pet. 
        In fact, Fluffy will be so happy after eating one of these that you'll find that Fluffy's abilities are noticeably improved! Are you interested in some Tasty Treats? */
        public override object Description
        {
            get
            {
                return 1112944;
            }
        }
        /*Ah, perhaps another time then.*/
        public override object Refuse
        {
            get
            {
                return 1112945;
            }
        }
        /*You will need to bring me five boura skins and a bit of dough. You can find the boura all over Ter Mur, 
        though I have heard that the tougher variety have skin that is more likely to stay intact during its slaughter.*/
        public override object Uncomplete
        {
            get
            {
                return 1112946;
            }
        }
        /*Welcome back. Did you bring the ingredients I asked for? Ah, good. Depending on the quality of the boura skins, 
        I usually do not need all five to produce five tasty treats. You can consider what is left over as payment for my services.
        The rest, I shall use... for other purposes. I have other tasks to finish right now for the master, but you can return in a bit if you wish to purchase more.*/
        public override object Complete
        {
            get
            {
                return 1112947;
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