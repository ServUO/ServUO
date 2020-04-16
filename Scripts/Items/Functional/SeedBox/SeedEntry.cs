namespace Server.Engines.Plants
{
    public class SeedEntry
    {
        public Seed Seed { get; set; }
        public int Image { get; set; }

        public SeedEntry(Seed seed)
        {
            Seed = seed;

            seed.ShowType = true;

            Image = Utility.Random(2183, 3);
        }

        public SeedEntry(GenericReader reader)
        {
            int v = reader.ReadInt();

            Seed = reader.ReadItem() as Seed;
            Image = reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Seed);
            writer.Write(Image);
        }
    }
}