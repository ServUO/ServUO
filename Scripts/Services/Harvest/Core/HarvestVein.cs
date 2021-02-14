namespace Server.Engines.Harvest
{
    public class HarvestVein
    {
        public HarvestVein(double veinChance, double chanceToFallback, HarvestResource primaryResource, HarvestResource fallbackResource)
        {
            VeinChance = veinChance;
            ChanceToFallback = chanceToFallback;
            PrimaryResource = primaryResource;
            FallbackResource = fallbackResource;
        }

        public double VeinChance { get; }
        public double ChanceToFallback { get; }
        public HarvestResource PrimaryResource { get; }
        public HarvestResource FallbackResource { get; }
    }
}
