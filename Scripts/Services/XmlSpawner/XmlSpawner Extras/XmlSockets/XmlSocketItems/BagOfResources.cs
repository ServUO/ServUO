using System;
using System.Data;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using Server.Gumps;
using System.Text;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    public class BagOfResources : BaseTransmutationContainer
    {

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
                (int)Recipes.BoltsToArrows,                     // transform any quantity of bolts into half the quantity of arrows
                70,30,0,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.ArmsLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(Bolt)},                                                     // ingredient list
                new int [] { 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.ArrowsToBolts,                     // transform any quantity of arrows into half the quantity of bolts
                70,30,0,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.ArmsLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(Arrow)},                                                     // ingredient list
                new int [] { 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.HidesToBandages,                     // transform any quantity of hides into bandages
                50,30,0,                                                                        // str, dex, int requirements
                null,                                        //  no skill requirements
                null,                                                               // minimum skill levels
                new Type [] {typeof(Hides)},                                                     // ingredient list
                new int [] { 0 }                                                                 // zero indicates any quantity
            );

        }

        public override void DoTransmute(Mobile from, Recipe r)
        {
            if(r == null || from == null) return;

            Recipes rid = (Recipes) r.RecipeID;
            switch(rid)
            {
                case Recipes.BoltsToArrows:
                {
                    int totalamount = 0;
                    foreach(Item i in Items)
                    {
                        totalamount += i.Amount;
                    }
                    // turn into half the amount
                    totalamount /= 2;
                    
                    // take the ingredients
                    ConsumeAll();

                    // add the new
                    if(totalamount > 0)
                        DropItem(new Arrow(totalamount));
                    break;
                }
                case Recipes.ArrowsToBolts:
                {
                    int totalamount = 0;
                    foreach(Item i in Items)
                    {
                        totalamount += i.Amount;
                    }
                    // turn into half the amount
                    totalamount /= 2;
                    
                    // take the ingredients
                    ConsumeAll();

                    // add the new
                    if(totalamount > 0)
                        DropItem(new Bolt(totalamount));
                    break;
                }
                case Recipes.HidesToBandages:
                {
                    int totalamount = 0;
                    foreach(Item i in Items)
                    {
                        totalamount += i.Amount;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // add the new
                    if(totalamount > 0)
                        DropItem(new Bandage(totalamount));
                    break;
                }

            }

            // give effects for successful transmutation
            from.PlaySound(503);
            
            base.DoTransmute(from, r);
        }

        [Constructable]
		public BagOfResources() : this(-1)
		{
		}
		
		[Constructable]
		public BagOfResources(int nuses) : base(0xE76)
		{
            Name = "Bag Of Resources";
            GumpID = 0x3D;
            Hue = 25;
            UsesRemaining = nuses;
		}

		public BagOfResources( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

    }
}
