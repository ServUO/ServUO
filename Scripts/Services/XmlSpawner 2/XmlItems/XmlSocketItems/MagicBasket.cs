using System;

namespace Server.Items
{
    public class MagicBasket : BaseTransmutationContainer
    {
        [Constructable]
        public MagicBasket()
            : this(-1)
        {
        }

        [Constructable]
        public MagicBasket(int nuses)
            : base(0xE7A)
        {
            this.Name = "Magical Basket";
            this.GumpID = 63;
            this.UsesRemaining = nuses;
        }

        public MagicBasket(Serial serial)
            : base(serial)
        {
        }

        public enum Recipes
        {
            UnbakedMeatPie
        }
        public static void Initialize()
        {
            //
            // define the recipes and their use requirements
            //
            // ideally, you have a definition for every Recipes enum.  Although it isnt absolutely necessary,
            // if it isnt defined here, it will not be available for use
            AddRecipe(
                (int)Recipes.UnbakedMeatPie, // makes an uncooked meat pie
                0, 30, 20, // str, dex, int requirements
                new SkillName[] { SkillName.Cooking }, //  skill requirement list
                new int[] { 30 }, // minimum skill levels
                new Type[] { typeof(BowlFlour), typeof(Pitcher), typeof(RawRibs), typeof(Garlic), typeof(Carrot) }, // ingredient list                                                    // ingredient list
                new int[] { 1, 1, 2, 1, 4 }, // quantities
                new string[] { null, "IsFull=true & Content=#Milk", null, null, null });
        }

        public override void DoTransmute(Mobile from, Recipe r)
        {
            if (r == null || from == null)
                return;

            Recipes rid = (Recipes)r.RecipeID;
            switch(rid)
            {
                case Recipes.UnbakedMeatPie:
                    {
                        // take the ingredients
                        this.ConsumeAll();

                        // add the pie
                        this.DropItem(new UnbakedMeatPie());
                        break;
                    }
            }

            // give effects for successful transmutation
            from.PlaySound(503);
            
            base.DoTransmute(from, r);
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