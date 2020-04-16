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
        }
    }
}