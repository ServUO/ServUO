using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class AllThatGlitters : BaseQuest, ITierQuest
    {
        public AllThatGlitters()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(CongealedSlugAcid), "Congealed Slug Acid", 5, 0x5742));
            AddObjective(new ObtainObjective(typeof(PileofInspectedGoldIngots), "Pile of Inspected Gold Ingots", 1, 0x1BEA));

            AddReward(new BaseReward(typeof(ElixirofGoldConversion), "Elixir of Gold Conversion"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Thepem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /*All That Glitters*/
        public override object Title => 1112775;
        /*Ah, yes, welcome to our humble shop. Do you wish to buy some of our fine potions today, or perhaps have something of interest to sell?<br><br>No? Well, 
        I do have some specialty goods for sale that may be of interest to you. Unfortunately, specialty goods require specialty ingredients, which can be harder to come by.
        I'm not the adventurous sort, so if you are interested, you'll have to bring them to me.<br><br>
        Mistress Zosilem has recently discovered an efficient method of converting lesser metals in those that are more valuable.  
        The elixirs that convert the more valuable metals are for our long term customers. 
        That said, perhaps you are interested in purchasing a elixir that can turn up to 500 dull copper ingots into gold ones? 
        I will need some specialty ingredients in addition to what we have in the shop. Of course, nothing one such as yourself cannot obtain with a small bit of effort.<br><br>
        Bring me five portions of congealed slug acid, and twenty gold ingots. I will need to inspect the ingots before I accept them, 
        so give those to me before we complete the transaction.  */
        public override object Description => 1112948;
        /*Ah, perhaps another time then.*/
        public override object Refuse => 1112949;
        /*I will need twenty gold ingots and some congealed slug acid, which can be found on... can you guess? Yes, that's right. Acid slugs.*/
        public override object Uncomplete => 1112950;
        /*Hello, how may I help you? Oh, wait, you were interested in the elixir of gold conversion, right? If you have the materials I asked for ready,
        hand them over and I'll get to work on your elixir right away. After that, I have other tasks to finish for the mistress, 
        but you can return in a bit if you wish to make another purchase.*/
        public override object Complete => 1112951;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TastyTreats : BaseQuest, ITierQuest
    {
        public TastyTreats()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            AddObjective(new ObtainObjective(typeof(Dough), "Dough", 1, 0x103D));

            AddReward(new BaseReward(typeof(TastyTreat), "Tasty Treat"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Thepem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /*Tasty Treats*/
        public override object Title => 1112774;
        /*Ah, yes, welcome to our humble shop. Do you wish to buy some of our fine potions today, or perhaps have something of interest to sell?
        No? Well, I do have some specialty goods for sale that may be of interest to you. Unfortunately, specialty goods require specialty ingredients, 
        which can be harder to come by. I'm not the adventurous sort, so if you are interested, you'll have to bring them to me.
        Pets can be finicky eaters at times, but I have just the solution for that. I call them 'Tasty Treats', and they're sure to please your pet. 
        In fact, Fluffy will be so happy after eating one of these that you'll find that Fluffy's abilities are noticeably improved! Are you interested in some Tasty Treats? */
        public override object Description => 1112944;
        /*Ah, perhaps another time then.*/
        public override object Refuse => 1112945;
        /*You will need to bring me five boura skins and a bit of dough. You can find the boura all over Ter Mur, 
        though I have heard that the tougher variety have skin that is more likely to stay intact during its slaughter.*/
        public override object Uncomplete => 1112946;
        /*Welcome back. Did you bring the ingredients I asked for? Ah, good. Depending on the quality of the boura skins, 
        I usually do not need all five to produce five tasty treats. You can consider what is left over as payment for my services.
        The rest, I shall use... for other purposes. I have other tasks to finish right now for the master, but you can return in a bit if you wish to purchase more.*/
        public override object Complete => 1112947;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MetalHead : BaseQuest, ITierQuest
    {
        public MetalHead()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(UndamagedIronBeetleScale), "Undamaged IronBeetle Scale", 10, 0x5742));
            AddObjective(new ObtainObjective(typeof(PileofInspectedDullCopperIngots), "Pile of Inspected DullCopper Ingots", 1, 0x1BEA));
            AddObjective(new ObtainObjective(typeof(PileofInspectedShadowIronIngots), "Pile of Inspected ShadowIron Ingots", 1, 0x1BEA));
            AddObjective(new ObtainObjective(typeof(PileofInspectedCopperIngots), "Pile of Inspected Copper Ingots", 1, 0x1BEA));
            AddObjective(new ObtainObjective(typeof(PileofInspectedBronzeIngots), "Pile of Inspected Bronze Ingots", 1, 0x1BEA));

            AddReward(new BaseReward(typeof(ElixirofMetalConversion), "Elixir of Metal Conversion"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Thepem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /*Metal Head*/
        public override object Title => 1112776;
        /*Welcome back to our shop. As one of our valued customers, 
        I assume that you are here to make a specialty purchase? Mistress Zosilem has authorized me to make available to you a very special elixir 
        that is able to convert common iron into something a bit more valuable.<br><br
        That we can make this available at all is due to some very cutting edge research that the mistress has been doing. 
        In fact, the results are a bit unpredictable, but guaranteed to be worth your time. If you are interested, 
        I'll need you to bring me twenty each of the lesser four colored ingots, as well as ten undamaged iron beetle scales. 
        Does that sound good to you?<br><br>I will need to inspect the ingots before I accept them, so give those to me before we complete the transaction.*/
        public override object Description => 1112952;
        /*As always, feel free to return to our shop when you find yourself in need. Farewell.*/
        public override object Refuse => 1112957;
        /*I'll need you to bring me twenty each of the lesser four colored ingots, dull copper, shadow iron, copper and bronze, as well as ten undamaged iron beetle scales.*/
        public override object Uncomplete => 1112954;
        /*I see that you have returned. Did you still want the elixir of metal conversion? Yes? Good, just hand over the ingredients I asked for, 
        and I'll mix this up for you immediately. I'll be busy for a couple hours, but return after that if you wish to purchase more.*/
        public override object Complete => 1112955;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PinkistheNewBlack : BaseQuest, ITierQuest
    {
        public PinkistheNewBlack()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(SearedFireAntGoo), "Seared Fire Ant Goo", 5, 0x0976));
            AddObjective(new ObtainObjective(typeof(PileofInspectedAgapiteIngots), "Pile of Inspected Agapite Ingots", 1, 0x1BEA));

            AddReward(new BaseReward(typeof(ElixirofAgapiteConversion), "Elixir of Agapite Conversion"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Thepem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /*Pink Is The New Black */
        public override object Title => 1112777;

        /*It is good to see you. As one of our valued customers, Mistress Zosilem has given me permission to offer you another special elixir, 
        one able to convert the more common shadow iron into valuable agapite. I'll need twenty agapite ingots and some seared fire ant goo for the mixture. 
        Are you interested?<br><br>I will need to inspect the ingots before I accept them, so give those to me before we complete the transaction.*/
        public override object Description => 1112956;
        /*As always, feel free to return to our shop when you find yourself in need. Farewell.*/
        public override object Refuse => 1112957;
        /*I will need twenty agapite ingots and some seared fire ant goo which, unsurprisingly, can be found on fire ants.*/
        public override object Uncomplete => 1112958;
        /*Good to see you again, have you come to bring me the ingredients for the elixir of agapite conversion? Good, 
        I'll take those in return for this elixir I made earlier. I'll be busy the rest of the day, but come back tomorrow if you need more.*/
        public override object Complete => 1112959;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
