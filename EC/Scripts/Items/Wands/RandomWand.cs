using System;

namespace Server.Items
{
    public class RandomWand
    {
        public static BaseWand CreateWand()
        {
            return CreateRandomWand();
        }

        public static BaseWand CreateRandomWand()
        {
            return Loot.RandomWand();
            /*
            switch ( Utility.Random( 11 ) )
            {
            default:
            case  0: return new ClumsyWand();
            case  1: return new FeebleWand();
            case  2: return new FireballWand();
            case  3: return new GreaterHealWand();
            case  4: return new HarmWand();
            case  5: return new HealWand();
            case  6: return new IDWand();
            case  7: return new LightningWand();
            case  8: return new MagicArrowWand();
            case  9: return new ManaDrainWand();
            case 10: return new WeaknessWand();
            }
            */
        }
    }
}