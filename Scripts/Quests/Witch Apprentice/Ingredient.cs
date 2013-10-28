using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Hag
{
    public enum Ingredient
    {
        SheepLiver,
        RabbitsFoot,
        MongbatWing,
        ChickenGizzard,
        RatTail,
        FrogsLeg,
        DeerHeart,
        LizardTongue,
        SlimeOoze,
        SpiritEssence,
        SwampWater,
        RedMushrooms,
        Bones,
        StarChart,
        Whiskey
    }

    public class IngredientInfo
    {
        private static readonly IngredientInfo[] m_Table = new IngredientInfo[]
        {
            // sheep liver
            new IngredientInfo(1055020, 5, typeof(Sheep)),
            // rabbit's foot
            new IngredientInfo(1055021, 5, typeof(Rabbit), typeof(JackRabbit)),
            // mongbat wing
            new IngredientInfo(1055022, 5, typeof(Mongbat), typeof(GreaterMongbat)),
            // chicken gizzard
            new IngredientInfo(1055023, 5, typeof(Chicken)),
            // rat tail
            new IngredientInfo(1055024, 5, typeof(Rat), typeof(GiantRat), typeof(Sewerrat)),
            // frog's leg
            new IngredientInfo(1055025, 5, typeof(BullFrog)),
            // deer heart
            new IngredientInfo(1055026, 5, typeof(Hind), typeof(GreatHart)),
            // lizard tongue
            new IngredientInfo(1055027, 5, typeof(LavaLizard), typeof(Lizardman)),
            // slime ooze
            new IngredientInfo(1055028, 5, typeof(Slime)),
            // spirit essence
            new IngredientInfo(1055029, 5, typeof(Ghoul), typeof(Spectre), typeof(Shade), typeof(Wraith), typeof(Bogle)),
            // Swamp Water
            new IngredientInfo(1055030, 1),
            // Freshly Cut Red Mushrooms
            new IngredientInfo(1055031, 1),
            // Bones Buried In Hallowed Ground
            new IngredientInfo(1055032, 1),
            // Star Chart
            new IngredientInfo(1055033, 1),
            // Captain Blackheart's Whiskey
            new IngredientInfo(1055034, 1)
        };
        private readonly int m_Name;
        private readonly Type[] m_Creatures;
        private readonly int m_Quantity;
        private IngredientInfo(int name, int quantity, params Type[] creatures)
        {
            this.m_Name = name;
            this.m_Creatures = creatures;
            this.m_Quantity = quantity;
        }

        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public Type[] Creatures
        {
            get
            {
                return this.m_Creatures;
            }
        }
        public int Quantity
        {
            get
            {
                return this.m_Quantity;
            }
        }
        public static IngredientInfo Get(Ingredient ingredient)
        {
            int index = (int)ingredient;

            if (index >= 0 && index < m_Table.Length)
                return m_Table[index];
            else
                return m_Table[0];
        }

        public static Ingredient RandomIngredient(Ingredient[] oldIngredients)
        {
            int length = m_Table.Length - oldIngredients.Length;
            Ingredient[] ingredients = new Ingredient[length];

            for (int i = 0, n = 0; i < m_Table.Length && n < ingredients.Length; i++)
            {
                Ingredient currIngredient = (Ingredient)i;

                bool found = false;
                for (int j = 0; !found && j < oldIngredients.Length; j++)
                {
                    if (oldIngredients[j] == currIngredient)
                        found = true;
                }

                if (!found)
                    ingredients[n++] = currIngredient;
            }

            int index = Utility.Random(ingredients.Length);

            return ingredients[index];
        }
    }
}