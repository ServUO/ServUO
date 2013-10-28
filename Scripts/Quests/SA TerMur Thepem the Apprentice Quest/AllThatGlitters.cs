using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class AllThatGlitters : BaseQuest
    {
        public AllThatGlitters()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(CongealedSlugAcid), "Congealed Slug Acid", 5, 0x5742));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedGoldIngots), "Pile of Inspected Gold Ingots", 1, 0x1BEA));

            this.AddReward(new BaseReward(typeof(ElixirofGoldConversion), "Elixir of Gold Conversion"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(TastyTreats);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(30);
            }
        }
        /*All That Glitters*/
        public override object Title
        {
            get
            {
                return 1112775;
            }
        }
        /*Ah, yes, welcome to our humble shop. Do you wish to buy some of our fine potions today, or perhaps have something of interest to sell?<br><br>No? Well, 
        I do have some specialty goods for sale that may be of interest to you. Unfortunately, specialty goods require specialty ingredients, which can be harder to come by.
        I'm not the adventurous sort, so if you are interested, you'll have to bring them to me.<br><br>
        Mistress Zosilem has recently discovered an efficient method of converting lesser metals in those that are more valuable.  
        The elixirs that convert the more valuable metals are for our long term customers. 
        That said, perhaps you are interested in purchasing a elixir that can turn up to 500 dull copper ingots into gold ones? 
        I will need some specialty ingredients in addition to what we have in the shop. Of course, nothing one such as yourself cannot obtain with a small bit of effort.<br><br>
        Bring me five portions of congealed slug acid, and twenty gold ingots. I will need to inspect the ingots before I accept them, 
        so give those to me before we complete the transaction.  */
        public override object Description
        {
            get
            {
                return 1112948;
            }
        }
        /*Ah, perhaps another time then.*/
        public override object Refuse
        {
            get
            {
                return 1112949;
            }
        }
        /*I will need twenty gold ingots and some congealed slug acid, which can be found on... can you guess? Yes, that's right. Acid slugs.*/
        public override object Uncomplete
        {
            get
            {
                return 1112950;
            }
        }
        /*Hello, how may I help you? Oh, wait, you were interested in the elixir of gold conversion, right? If you have the materials I asked for ready,
        hand them over and I'll get to work on your elixir right away. After that, I have other tasks to finish for the mistress, 
        but you can return in a bit if you wish to make another purchase.*/
        public override object Complete
        {
            get
            {
                return 1112951;
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