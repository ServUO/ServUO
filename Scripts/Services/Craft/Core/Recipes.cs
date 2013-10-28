using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;

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
            this.m_ID = id;
            this.m_System = system;
            this.m_CraftItem = item;

            if (m_Recipes.ContainsKey(id))
                throw new Exception("Attempting to create recipe with preexisting ID.");

            m_Recipes.Add(id, this);
            m_LargestRecipeID = Math.Max(id, m_LargestRecipeID);
        }

        public static Dictionary<int, Recipe> Recipes
        {
            get
            {
                return m_Recipes;
            }
        }
        public static int LargestRecipeID
        {
            get
            {
                return m_LargestRecipeID;
            }
        }
        public CraftSystem CraftSystem
        {
            get
            {
                return this.m_System;
            }
            set
            {
                this.m_System = value;
            }
        }
        public CraftItem CraftItem
        {
            get
            {
                return this.m_CraftItem;
            }
            set
            {
                this.m_CraftItem = value;
            }
        }
        public int ID
        {
            get
            {
                return this.m_ID;
            }
        }
        public TextDefinition TextDefinition
        {
            get
            {
                if (this.m_TD == null)
                    this.m_TD = new TextDefinition(this.m_CraftItem.NameNumber, this.m_CraftItem.NameString);

                return this.m_TD;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("LearnAllRecipes", AccessLevel.GameMaster, new CommandEventHandler(LearnAllRecipes_OnCommand));
            CommandSystem.Register("ForgetAllRecipes", AccessLevel.GameMaster, new CommandEventHandler(ForgetAllRecipes_OnCommand));
        }

        [Usage("LearnAllRecipes")]
        [Description("Teaches a player all available recipes.")]
        private static void LearnAllRecipes_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            m.SendMessage("Target a player to teach them all of the recipies.");

            m.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, new TargetCallback(
                delegate(Mobile from, object targeted)
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
                }));
        }

        [Usage("ForgetAllRecipes")]
        [Description("Makes a player forget all the recipies they've learned.")]
        private static void ForgetAllRecipes_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            m.SendMessage("Target a player to have them forget all of the recipies they've learned.");

            m.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, new TargetCallback(
                delegate(Mobile from, object targeted)
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
                }));
        }
    }
}