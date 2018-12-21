using System;

namespace Server.Items
{
    public class RecipeScrollFilter
    {
        public bool IsDefault
        {
            get { return (Skill == 0 && Expansion == 0 && Amount == 0); }
        }

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
                writer.Write((int)0);
            }
            else
            {
                writer.Write(1);

                writer.Write((int)Skill);
                writer.Write((int)Expansion);
                writer.Write((int)Amount);
            }
        }
    }
}
