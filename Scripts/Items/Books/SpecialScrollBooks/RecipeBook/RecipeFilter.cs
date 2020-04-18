namespace Server.Items
{
    public class RecipeScrollFilter
    {
        public bool IsDefault => (Skill == 0 && Expansion == 0 && Amount == 0);

        public void Clear()
        {
            Skill = 0;
            Expansion = 0;
            Amount = 0;
        }

        public int Skill { get; set; }

        public int Expansion { get; set; }

        public int Amount { get; set; }

        public RecipeScrollFilter()
        {
        }

        public RecipeScrollFilter(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Skill = reader.ReadInt();
                    Expansion = reader.ReadInt();
                    Amount = reader.ReadInt();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            if (IsDefault)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(1);

                writer.Write(Skill);
                writer.Write(Expansion);
                writer.Write(Amount);
            }
        }
    }
}
