using System;

namespace Server.Items
{
    public class BagOfResources : BaseTransmutationContainer
    {
        [Constructable]
        public BagOfResources()
            : this(-1)
        {
        }

        [Constructable]
        public BagOfResources(int nuses)
            : base(0xE76)
        {
            this.Name = "Bag Of Resources";
            this.GumpID = 0x3D;
            this.Hue = 25;
            this.UsesRemaining = nuses;
        }

        public BagOfResources(Serial serial)
            : base(serial)
        {
        }

        public enum Recipes
        {
            BoltsToArrows,
            ArrowsToBolts,
            HidesToBandages
        }
        public static void Initialize()
        {
            //
            // define the recipes and their use requirements
            //
            // ideally, you have a definition for every Recipes enum.  Although it isnt absolutely necessary,
            // if it isnt defined here, it will not be available for use
            AddRecipe(
                (int)Recipes.BoltsToArrows, // transform any quantity of bolts into half the quantity of arrows
                70, 30, 0, // str, dex, int requirements
                new SkillName[] { SkillName.ArmsLore }, //  skill requirement list
                new int[] { 50 }, // minimum skill levels
                new Type[] { typeof(Bolt) }, // ingredient list
                new int[] { 0 });
            AddRecipe(
                (int)Recipes.ArrowsToBolts, // transform any quantity of arrows into half the quantity of bolts
                70, 30, 0, // str, dex, int requirements
                new SkillName[] { SkillName.ArmsLore }, //  skill requirement list
                new int[] { 50 }, // minimum skill levels
                new Type[] { typeof(Arrow) }, // ingredient list
                new int[] { 0 });
            AddRecipe(
                (int)Recipes.HidesToBandages, // transform any quantity of hides into bandages
                50, 30, 0, // str, dex, int requirements
                null, //  no skill requirements
                null, // minimum skill levels
                new Type[] { typeof(Hides) }, // ingredient list
                new int[] { 0 });
        }

        public override void DoTransmute(Mobile from, Recipe r)
        {
            if (r == null || from == null)
                return;

            Recipes rid = (Recipes)r.RecipeID;
            switch(rid)
            {
                case Recipes.BoltsToArrows:
                    {
                        int totalamount = 0;
                        foreach (Item i in this.Items)
                        {
                            totalamount += i.Amount;
                        }
                        // turn into half the amount
                        totalamount /= 2;
                    
                        // take the ingredients
                        this.ConsumeAll();

                        // add the new
                        if (totalamount > 0)
                            this.DropItem(new Arrow(totalamount));
                        break;
                    }
                case Recipes.ArrowsToBolts:
                    {
                        int totalamount = 0;
                        foreach (Item i in this.Items)
                        {
                            totalamount += i.Amount;
                        }
                        // turn into half the amount
                        totalamount /= 2;
                    
                        // take the ingredients
                        this.ConsumeAll();

                        // add the new
                        if (totalamount > 0)
                            this.DropItem(new Bolt(totalamount));
                        break;
                    }
                case Recipes.HidesToBandages:
                    {
                        int totalamount = 0;
                        foreach (Item i in this.Items)
                        {
                            totalamount += i.Amount;
                        }

                        // take the ingredients
                        this.ConsumeAll();

                        // add the new
                        if (totalamount > 0)
                            this.DropItem(new Bandage(totalamount));
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