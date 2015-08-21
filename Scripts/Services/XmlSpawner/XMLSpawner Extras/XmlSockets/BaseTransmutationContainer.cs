using System;
using System.Data;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using System.Text;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    public class BaseTransmutationContainer : Container
    {

        public virtual void DoTransmute(Mobile from, Recipe r)
        {
            if(UsesRemaining > 0)
            {
                UsesRemaining--;
            }
        }


        public void ConsumeAll()
        {
            ArrayList dlist = new ArrayList();
            foreach(Item i in Items)
            {
                dlist.Add(i);
            }
            foreach(Item i in dlist)
            {
                i.Delete();
            }

        }

        public class Recipe
        {
            public int         RecipeID;  // recipe id
            public int StrReq;             // str requirements for this recipe
            public int DexReq;             // dex requirements for this recipe
            public int IntReq;             // int requirements for this recipe
            public Type [] Ingredients;       // ingredient list used for this recipe
            public int [] Quantity;        // ingredients quantity list
            public SkillName [] Skills;    // list of skill requirements for this recipe
            public int [] MinSkillLevel;   // minimum skill levels
            public string [] PropertyTests; // additional property tests on the ingredients

            public Recipe( int id, int minstr, int mindex, int minint,
            SkillName [] skills, int [] minlevel, Type [] ingredients, int [] quantity, string [] proptests)
            {
                RecipeID = id;
                StrReq = minstr;
                DexReq = mindex;
                IntReq = minint;
                Ingredients = ingredients;
                Quantity = quantity;
                Skills = skills;
                MinSkillLevel = minlevel;
                PropertyTests = proptests;
            }
        }

        private int m_UsesRemaining;

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; InvalidateProperties(); } }

        private static ArrayList AllRecipes = new ArrayList();

        public static void AddRecipe( int id, int minstr, int mindex, int minint,
            SkillName [] skills, int [] minlevel, Type [] ingredients, int [] quantity)
        {
            AllRecipes.Add(new Recipe( id,
            minstr, mindex, minint, skills, minlevel, ingredients, quantity, null) );
        }
        
        public static void AddRecipe( int id, int minstr, int mindex, int minint,
            SkillName [] skills, int [] minlevel, Type [] ingredients, int [] quantity, string [] proptests)
        {
            AllRecipes.Add(new Recipe( id,
            minstr, mindex, minint, skills, minlevel, ingredients, quantity, proptests) );
        }

    	public override int DefaultGumpID{ get{ return 0x4B; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 16, 51, 168, 73 ); }
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

            if(m_UsesRemaining >= 0)
                list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

                
        private class TransmuteEntry : ContextMenuEntry
		{
            Mobile m_From;
            BaseTransmutationContainer m_Box;

			public TransmuteEntry(Mobile from, BaseTransmutationContainer box ) : base( 6190, 2 )
			{
                m_From = from;
                m_Box = box;
			}

			public override void OnClick()
			{
                if(m_From == null || m_Box == null || m_Box.Deleted) return;

				// open the transmutation gump
                m_From.CloseGump( typeof(TransmuteGump) );
                m_From.SendGump( new TransmuteGump( m_From, m_Box, null) );

			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries( from, list );

			list.Add( new TransmuteEntry(from, this) );
		}

        public override void OnItemLifted( Mobile from, Item item )
        {
            if(from != null)
            {
                from.CloseGump( typeof(TransmuteGump) );
                from.SendGump( new TransmuteGump( from, this, item) );
            }
        }


		public override bool OnDragDrop( Mobile from, Item dropped )
        {
            bool diddrop = base.OnDragDrop( from, dropped);
            
            if(from != null && diddrop)
            {
                from.CloseGump( typeof(TransmuteGump) );
                from.SendGump( new TransmuteGump( from, this, null) );
            }
                
            return diddrop;

        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            bool diddrop = base.OnDragDropInto( from, item, p);

            if(from != null && diddrop)
            {
                from.CloseGump( typeof(TransmuteGump) );
                from.SendGump( new TransmuteGump( from, this, null) );
            }

            return diddrop;
        }


        public static bool CheckRequirements(Mobile from, Recipe s)
        {
            if(from == null || s == null) return false;

            // test for str, dex, int requirements
            if(from.Str < s.StrReq)
            {
                from.SendMessage("Need {0} Str to transmute this", s.StrReq);
                return false;
            }
            if(from.Dex < s.DexReq)
            {
                from.SendMessage("Need {0} Dex to transmute this", s.DexReq);
                return false;
            }
            if(from.Int < s.IntReq)
            {
                from.SendMessage("Need {0} Int to transmute this", s.IntReq);
                return false;
            }

            // test for skill requirements
            if(s.Skills != null && s.MinSkillLevel != null)
            {
                if(from.Skills == null) return false;

                for(int i = 0; i < s.Skills.Length; i++)
                {
                    // and check level
                    if(i < s.MinSkillLevel.Length)
                    {
                        Skill skill =  from.Skills[s.Skills[i]];
                        if(skill != null && s.MinSkillLevel[i] > skill.Base)
                        {
                            from.SendMessage("Need {0} {1} to transmute this", s.MinSkillLevel[i], s.Skills[i].ToString());
                            return false;
                        }
                    } else
                    {
                        from.SendMessage(33,"Error in skill level specification for {0}", s.RecipeID);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ContainsInterface(Type [] typearray, Type type)
        {
            if(typearray == null || type == null) return false;

            foreach(Type t in typearray)
            {
                if(t == type) return true;
            }

            return false;
        }

        public void Transmute(Mobile from)
        {
            if(from == null) return;

            if(UsesRemaining == 0)
            {
                from.SendMessage("{0} is exhausted", Name);
                return;
            }
            // go through each recipe and determine if the conditions are met
            foreach(Recipe r in AllRecipes)
            {
                if(r.Ingredients == null || r.Ingredients.Length == 0 || r.Quantity == null || r.Ingredients.Length != r.Quantity.Length) continue;

                // go through all of the items in the container
                bool validrecipe = true;
                int [] quantity = new int[r.Quantity.Length];

                foreach(Item i in Items)
                {
                    // is this in the recipe?
                    bool hasingredient = false;
                    string status_str;
                    for(int j = 0; j< r.Ingredients.Length; j++)
                    {
                        Type rt = r.Ingredients[j];
                        Type it = i.GetType();
                        if(it != null && rt != null && (it.Equals(rt) || it.IsSubclassOf( rt ) ||
                        (rt.IsInterface && ContainsInterface(it.GetInterfaces(), rt))))
                        {
                            // check any additional property requirements
                            if(r.PropertyTests != null && r.PropertyTests[j] != null && !BaseXmlSpawner.CheckPropertyString(null, i, r.PropertyTests[j], null, out status_str))
                            {
                                // failed to meet the requirement so skip it
                                continue;
                            }

                            // found it, so add the quantity
                            quantity[j] += i.Amount;

                            hasingredient = true;
                            break;
                        }
                    }

                    // an item is present that is not an ingredient
                    // that means an invalid recipe
                    if(!hasingredient)
                    {
                        validrecipe = false;
                        break;
                    }
                }

                // check to see if all of the ingredient quantities have been satisfied
                for(int j = 0; j< r.Ingredients.Length; j++)
                {
                    if((quantity[j] == 0) || (r.Quantity[j] != 0 && quantity[j] != r.Quantity[j]))
                    {
                        validrecipe = false;
                        break;
                    }
                }


                if(validrecipe)
                {
                    // check on stat and skill requirements
                    if(CheckRequirements(from,r))
                    {
                        // all recipe conditions are satisfied, so carry out the transmutation
                        DoTransmute(from, r);
                        break;
                    }
                }
            }

        }

		public BaseTransmutationContainer(int itemid) : base(itemid)
		{
            Name = "Transmutation Container";
		}

		public BaseTransmutationContainer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			// version 0
			writer.Write( m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			switch(version)
			{
			case 0:
                m_UsesRemaining = reader.ReadInt();
                break;
			}
		}

		private class TransmuteGump : Gump
        {
            private Mobile m_From;
            private BaseTransmutationContainer m_Target;
            
            private const int LineSpacing = 20;
            private const int ContentColor = 0x384;
            private const int TitleColor = 53;
            private const int ContentTitleColor = 2101;

            public TransmuteGump(Mobile from, BaseTransmutationContainer target, Item lifted) : base ( 0, 0 )
            {

                if(target == null || from == null) return;

                m_From = from;
                m_Target = target;

            	Closable = true;
    			Dragable = true;

    			int count = 0;
    			// figure out how many items are in the box
    			if(target.Items != null)
    			{
                    count = target.Items.Count;
    			}
    			
    			int displayeditems = count;
    			if(lifted != null && count > 0)
    			{
                    displayeditems = count - 1;
    			} 

    			int height = 160 + LineSpacing*displayeditems;
    			int width = 350;

                AddPage( 0 );
                //AddBackground( 0, 0, width, height, 0x242C );
                AddBackground( 0, 0, width, height, 0xA28 );
                //AddAlphaRegion( 2, 2, width - 4, height - 4 );

                AddLabel( width/2 - 55, 15, TitleColor, target.Name );

                AddLabel( 20, 45, ContentTitleColor, String.Format("Contents:") );

                AddImageTiled( 15, 65, width - 30, 20, 0x242D );
                AddImageTiled( 15, height - 70, width - 30, 20, 0x242D );


                // go through and list all of the items in the box
                int y = 85;
                for(int i = 0;i < count; i++)
                {
                    if(target.Items[i] is Item)
                    {
                        Item item = target.Items[i] as Item;
                        
                        if(item == lifted) continue;

                        string name = null;
                        if(item.Name != null)
                        {
                            name = item.Name;
                        } else
                        {
                            name = item.GetType().Name;
                        }
                        AddLabel( 80, y, ContentColor, name );
                        AddLabel( 20, y, ContentColor, item.Amount.ToString() );
                        y += LineSpacing;
                    }
                }

    			//AddButton( width/2 - 20, height - 35, 2130, 2129, 1, GumpButtonType.Reply, 0 ); // Okay button
    			AddButton( width/2 - 43, height - 45, 0x1454, 0x1455, 1, GumpButtonType.Reply, 0 ); // Apply button
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {

                if(info == null || state == null || state.Mobile == null || m_From == null || m_Target == null) return;


                switch(info.ButtonID)
                {
                    case 1:
                    {
                       // transmute
                       m_Target.Transmute(state.Mobile);
                       state.Mobile.SendGump( new TransmuteGump( state.Mobile, m_Target, null) );
                        break;
                    }
                }
            }
        }

    }
}
