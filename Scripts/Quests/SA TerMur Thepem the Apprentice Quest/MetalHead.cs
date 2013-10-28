using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class MetalHead : BaseQuest
    {
        public MetalHead()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(UndamagedIronBeetleScale), "Undamaged IronBeetle Scale", 10, 0x5742));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedDullCopperIngots), "Pile of Inspected DullCopper Ingots", 1, 0x1BEA));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedShadowIronIngots), "Pile of Inspected ShadowIron Ingots", 1, 0x1BEA));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedCopperIngots), "Pile of Inspected Copper Ingots", 1, 0x1BEA));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedBronzeIngots), "Pile of Inspected Bronze Ingots", 1, 0x1BEA));

            this.AddReward(new BaseReward(typeof(ElixirofMetalConversion), "Elixir of Metal Conversion"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(PinkistheNewBlack);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        /*Metal Head*/
        public override object Title
        {
            get
            {
                return 1112776;
            }
        }
        /*Welcome back to our shop. As one of our valued customers, 
        I assume that you are here to make a specialty purchase? Mistress Zosilem has authorized me to make available to you a very special elixir 
        that is able to convert common iron into something a bit more valuable.<br><br
        That we can make this available at all is due to some very cutting edge research that the mistress has been doing. 
        In fact, the results are a bit unpredictable, but guaranteed to be worth your time. If you are interested, 
        I'll need you to bring me twenty each of the lesser four colored ingots, as well as ten undamaged iron beetle scales. 
        Does that sound good to you?<br><br>I will need to inspect the ingots before I accept them, so give those to me before we complete the transaction.*/
        public override object Description
        {
            get
            {
                return 1112952;
            }
        }
        /*As always, feel free to return to our shop when you find yourself in need. Farewell.*/
        public override object Refuse
        {
            get
            {
                return 1112957;
            }
        }
        /*I'll need you to bring me twenty each of the lesser four colored ingots, dull copper, shadow iron, copper and bronze, as well as ten undamaged iron beetle scales.*/
        public override object Uncomplete
        {
            get
            {
                return 1112954;
            }
        }
        /*I see that you have returned. Did you still want the elixir of metal conversion? Yes? Good, just hand over the ingredients I asked for, 
        and I'll mix this up for you immediately. I'll be busy for a couple hours, but return after that if you wish to purchase more.*/
        public override object Complete
        {
            get
            {
                return 1112955;
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