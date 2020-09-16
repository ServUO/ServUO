using Server.Commands;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
    public class Recipe
    {
        private static readonly Dictionary<int, Recipe> m_Recipes = new Dictionary<int, Recipe>();
        private static int m_LargestRecipeID;
        private readonly int m_ID;
        private CraftSystem m_System;
        private CraftItem m_CraftItem;
        private TextDefinition m_TD;
        public Recipe(int id, CraftSystem system, CraftItem item)
        {
            m_ID = id;
            m_System = system;
            m_CraftItem = item;

            if (m_Recipes.ContainsKey(id))
                throw new Exception("Attempting to create recipe with preexisting ID.");

            m_Recipes.Add(id, this);
            m_LargestRecipeID = Math.Max(id, m_LargestRecipeID);
        }

        public static Dictionary<int, Recipe> Recipes => m_Recipes;

        public static int LargestRecipeID => m_LargestRecipeID;

        public CraftSystem CraftSystem
        {
            get
            {
                return m_System;
            }
            set
            {
                m_System = value;
            }
        }
        public CraftItem CraftItem
        {
            get
            {
                return m_CraftItem;
            }
            set
            {
                m_CraftItem = value;
            }
        }

        public int ID => m_ID;

        public TextDefinition TextDefinition
        {
            get
            {
                if (m_TD == null)
                    m_TD = new TextDefinition(m_CraftItem.NameNumber, m_CraftItem.NameString);

                return m_TD;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("LearnAllRecipes", AccessLevel.GameMaster, LearnAllRecipes_OnCommand);
            CommandSystem.Register("ForgetAllRecipes", AccessLevel.GameMaster, ForgetAllRecipes_OnCommand);
        }

        [Usage("LearnAllRecipes")]
        [Description("Teaches a player all available recipes.")]
        private static void LearnAllRecipes_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            m.SendMessage("Target a player to teach them all of the recipies.");

            m.BeginTarget(-1, false, Targeting.TargetFlags.None, delegate (Mobile from, object targeted)
            {
                if (targeted is PlayerMobile)
                {
                    foreach (KeyValuePair<int, Recipe> kvp in m_Recipes)
                        ((PlayerMobile)targeted).AcquireRecipe(kvp.Key);

                    m.SendMessage("You teach them all of the recipies.");
                }
                else
                {
                    m.SendMessage("That is not a player!");
                }
            });
        }

        [Usage("ForgetAllRecipes")]
        [Description("Makes a player forget all the recipies they've learned.")]
        private static void ForgetAllRecipes_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            m.SendMessage("Target a player to have them forget all of the recipies they've learned.");

            m.BeginTarget(-1, false, Targeting.TargetFlags.None, delegate (Mobile from, object targeted)
            {
                if (targeted is PlayerMobile)
                {
                    ((PlayerMobile)targeted).ResetRecipes();

                    m.SendMessage("They forget all their recipies.");
                }
                else
                {
                    m.SendMessage("That is not a player!");
                }
            });
        }
    }
}
