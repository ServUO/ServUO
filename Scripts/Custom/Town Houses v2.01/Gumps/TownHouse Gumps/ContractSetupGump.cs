using System;
using System.Collections;
using Server;
using Server.Targeting;

namespace Knives.TownHouses
{
	public class ContractSetupGump : GumpPlusLight
	{
		public enum Page { Blocks, Floors, Sign, LocSec, Length, Price }
		public enum TargetType { SignLoc, MinZ, MaxZ, BlockOne, BlockTwo }

		private RentalContract c_Contract;
		private Page c_Page;

		public ContractSetupGump( Mobile m, RentalContract contract ) : base( m, 50, 50 )
		{
			m.CloseGump( typeof( ContractSetupGump ) );

			c_Contract = contract;
		}

		protected override void BuildGump()
		{
            int width = 300;
            int y = 0;

			switch( c_Page )
			{
				case Page.Blocks: BlocksPage(width, ref y); break;
                case Page.Floors: FloorsPage(width, ref y); break;
                case Page.Sign: SignPage(width, ref y); break;
                case Page.LocSec: LocSecPage(width, ref y); break;
                case Page.Length: LengthPage(width, ref y); break;
                case Page.Price: PricePage(width, ref y); break;
			}
        
            AddBackgroundZero(0, 0, width, y+40, 0x13BE);
        }

		private void BlocksPage(int width, ref int y)
		{
			if ( c_Contract == null )
				return;

			c_Contract.ShowAreaPreview( Owner );

			AddHtml( 0, y+=10, width, "<CENTER>Create the Area");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			y+=25;

			if ( !General.HasOtherContract( c_Contract.ParentHouse, c_Contract ) )
			{
				AddHtml( 60, y, 90, "Entire House");
				AddButton( 30, y, c_Contract.EntireHouse ? 0xD3 : 0xD2, "Entire House", new GumpCallback(EntireHouse ));
			}

			if ( !c_Contract.EntireHouse )
			{
				AddHtml( 170, y, 70, "Add Area");
				AddButton( 240, y, 0x15E1, 0x15E5, "Add Area", new GumpCallback( AddBlock ) );

				AddHtml( 170, y+=20, 70, "Clear All");
				AddButton( 240, y, 0x15E1, 0x15E5, "Clear All", new GumpCallback( ClearBlocks ) );
			}

			string helptext = String.Format( "   Welcome to the rental contract setup menu!  To begin, you must " + 
				"first create the area which you wish to sell.  As seen above, there are two ways to do this: " +
				"rent the entire house, or parts of it.  As you create the area, a simple preview will show you exactly " +
				"what area you've selected so far.  You can make all sorts of odd shapes by using multiple areas!" );

			AddHtml( 10, y+=35, width-20, 170, helptext, false, false );

            y += 170;

			if ( c_Contract.EntireHouse || c_Contract.Blocks.Count != 0 )
			{
				AddHtml( width-60, y+=20, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)c_Page  + ( c_Contract.EntireHouse ? 4 : 1 ) );
			}
		}

		private void FloorsPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Floors");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 40, y+=25, 80, "Base Floor");
			AddButton( 110, y, 0x15E1, 0x15E5, "Base Floor", new GumpCallback( MinZSelect ) );

			AddHtml( 160, y, 80, "Top Floor");
			AddButton( 230, y, 0x15E1, 0x15E5, "Top Floor", new GumpCallback( MaxZSelect ) );

			AddHtml( 100, y+=25, 100, String.Format( "{0} total floor{1}", c_Contract.Floors > 10 ? "1" : "" + c_Contract.Floors, c_Contract.Floors == 1 || c_Contract.Floors > 10 ? "" : "s" ));

			string helptext = String.Format( "   Now you will need to target the floors you wish to rent out.  " + 
				"If you only want one floor, you can skip targeting the top floor.  Everything within the base " +
				"and highest floor will come with the rental, and the more floors, the higher the cost later on." );

			AddHtml( 10, y+=35, width-20, 120, helptext, false, false );

            y += 120;

			AddHtml( 30, y+=20, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)c_Page-1 );

			if ( c_Contract.MinZ != short.MinValue )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)c_Page+1 );
			}
		}

		private void SignPage(int width, ref int y)
		{
			if ( c_Contract == null )
				return;

			c_Contract.ShowSignPreview();

			AddHtml( 0, y+=10, width, "<CENTER>Their Sign Location");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 100, y+=25, 80, "Set Location");
			AddButton( 180, y, 0x15E1, 0x15E5, "Sign Loc", new GumpCallback( SignLocSelect ) );

			string helptext = String.Format( "   With this sign, the rentee will have all the powers an owner has " + 
				"over their area.  If they use this power to demolish their rental unit, they have broken their " +
				"contract and will not receive their security deposit.  They can also ban you from their rental home!" );

			AddHtml( 10, y+=35, width-20, 110, helptext, false, false );

            y += 110;

			AddHtml( 30, y+=20, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)c_Page-1 );

			if ( c_Contract.SignLoc != Point3D.Zero )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)c_Page+1 );
			}
		}

		private void LocSecPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Lockdowns and Secures");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>Suggest Secures");
            AddButton(width / 2 - 70, y + 3, 0x2716, "Suggest LocSec", new GumpCallback(SuggestLocSec));
            AddButton(width / 2 + 60, y + 3, 0x2716, "Suggest LocSec", new GumpCallback(SuggestLocSec));

            AddHtml(30, y += 25, width / 2 - 20, "<DIV ALIGN=RIGHT>Secures (Max: " + (General.RemainingSecures(c_Contract.ParentHouse) + c_Contract.Secures) + ")");
			AddTextField( width/2+50, y, 50, 20, 0x480, 0xBBC, "Secures", c_Contract.Secures.ToString() );
            AddButton(width / 2 + 25, y + 3, 0x2716, "Secures", new GumpCallback(Secures));

            AddHtml(30, y += 20, width / 2 - 20, "<DIV ALIGN=RIGHT>Lockdowns (Max: " + (General.RemainingLocks(c_Contract.ParentHouse) + c_Contract.Locks) + ")");
			AddTextField( width/2+50, y, 50, 20, 0x480, 0xBBC, "Lockdowns", c_Contract.Locks.ToString() );
            AddButton(width / 2 + 25, y + 3, 0x2716, "Lockdowns", new GumpCallback(Lockdowns));

			string helptext = String.Format( "   Without giving storage, this wouldn't be much of a home!  Here you give them lockdowns " +
				"and secures from your own home.  Use the suggest button for an idea of how much you should give.  Be very careful when " + 
				"renting your property: if you use too much storage you begin to use storage you reserved for your clients.  " +
				"You will receive a 48 hour warning when this happens, but after that the contract disappears!" );

			AddHtml( 10, y+=35, width-20, 180, helptext, false, false );

            y += 180;

			AddHtml( 30, y+=20, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)c_Page-1 );

			if ( c_Contract.Locks != 0 && c_Contract.Secures != 0 )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)c_Page+1 );
			}
		}

		private void LengthPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Time Period");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 120, y+=25, 50, c_Contract.PriceType);
			AddButton( 170, y+8, 0x985, "LengthUp", new GumpCallback( LengthUp ) );
			AddButton( 170, y-2, 0x983, "LengthDown", new GumpCallback( LengthDown ) );

			string helptext = String.Format( "   Every {0} the bank will automatically transfer the rental cost from them to you.  " + 
				"By using the arrows, you can cycle through other time periods to something better fitting your needs.", c_Contract.PriceTypeShort.ToLower() );

			AddHtml( 10, y+=35, width-20, 100, helptext, false, false );

            y += 100;

			AddHtml( 30, y+=20, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)c_Page - ( c_Contract.EntireHouse ? 4 : 1 ) );

			AddHtml( width-60, y, 60, "Next");
			AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)c_Page+1 );
		}

		private void PricePage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Charge Per Period");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>Free");
            AddButton(width / 2 - 80, y, c_Contract.Free ? 0xD3 : 0xD2, "Free", new GumpCallback(Free));
            AddButton(width / 2 + 60, y, c_Contract.Free ? 0xD3 : 0xD2, "Free", new GumpCallback(Free));

			if ( !c_Contract.Free )
			{
				AddHtml( 0, y+=25, width/2-20, "<DIV ALIGN=RIGHT>Per " + c_Contract.PriceTypeShort);
				AddTextField( width/2+20, y, 70, 20, 0x480, 0xBBC, "Price", c_Contract.Price.ToString() );
                AddButton(width / 2 - 5, y + 3, 0x2716, "Price", new GumpCallback(Price));

				AddHtml( 0, y+=20, width, "<CENTER>Suggest");
                AddButton(width / 2 - 70, y + 3, 0x2716, "Suggest", new GumpCallback(SuggestPrice));
                AddButton(width / 2 + 60, y + 3, 0x2716, "Suggest", new GumpCallback(SuggestPrice));
            }

			string helptext = String.Format( "   Now you can finalize the contract by including your price per {0}.  " + 
				"Once you finalize, the only way you can modify it is to dump it and start a new contract!  By " +
				"using the suggest button, a price will automatically be figured based on the following:<BR>", c_Contract.PriceTypeShort );

			helptext += String.Format( "<CENTER>Volume: {0}<BR>", c_Contract.CalcVolume() );
			helptext += String.Format( "Cost per unit: {0} gold</CENTER>", General.SuggestionFactor );
			helptext += "<br>   You may also give this space away for free using the option above.";

			AddHtml( 10, y+=35, width-20, 150, helptext, false, true );

            y += 150;

			AddHtml( 30, y+=20, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)c_Page-1 );

			if ( c_Contract.Price != 0 )
			{
				AddHtml( width-70, y, 60, "Finalize");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Finalize", new GumpCallback( FinalizeSetup ) );
			}
		}

		protected override void OnClose()
		{
			c_Contract.ClearPreview();
		}

		private void SuggestPrice()
		{
			if ( c_Contract == null )
				return;

			c_Contract.Price = c_Contract.CalcVolume()*General.SuggestionFactor;

			if ( c_Contract.RentByTime == TimeSpan.FromDays( 1 ) )
				c_Contract.Price /= 60;
			if ( c_Contract.RentByTime == TimeSpan.FromDays( 7 ) )
				c_Contract.Price = (int)((double)c_Contract.Price/8.57);
			if ( c_Contract.RentByTime == TimeSpan.FromDays( 30 ) )
				c_Contract.Price /= 2;

			NewGump();
		}

		private void SuggestLocSec()
		{
			int price = c_Contract.CalcVolume()*General.SuggestionFactor;
			c_Contract.Secures = price/75;
			c_Contract.Locks = c_Contract.Secures/2;

			c_Contract.FixLocSec();

			NewGump();
		}

        private void Price()
        {
            c_Contract.Price = GetTextFieldInt("Price");
            Owner.SendMessage("Price set!");
            NewGump();
        }

        private void Secures()
        {
            c_Contract.Secures = GetTextFieldInt("Secures");
            Owner.SendMessage("Secures set!");
            NewGump();
        }

        private void Lockdowns()
        {
            c_Contract.Locks = GetTextFieldInt("Lockdowns");
            Owner.SendMessage("Lockdowns set!");
            NewGump();
        }

        private void ChangePage(object obj)
		{
			if ( c_Contract == null || !(obj is int) )
				return;

			c_Contract.ClearPreview();

			c_Page = (Page)(int)obj;

			NewGump();
		}

		private void EntireHouse()
		{
			if ( c_Contract == null || c_Contract.ParentHouse == null )
				return;

			c_Contract.EntireHouse = !c_Contract.EntireHouse;

			c_Contract.ClearPreview();

			if ( c_Contract.EntireHouse )
			{
                ArrayList list = new ArrayList();

                bool once = false;
                foreach (Rectangle3D rect in RUOVersion.RegionArea(c_Contract.ParentHouse.Region))
                {
                    list.Add(new Rectangle2D(new Point2D(rect.Start.X, rect.Start.Y), new Point2D(rect.End.X, rect.End.Y)));

                    if (once)
                        continue;

                    if (rect.Start.Z >= rect.End.Z)
                    {
                        c_Contract.MinZ = rect.End.Z;
                        c_Contract.MaxZ = rect.Start.Z;
                    }
                    else
                    {
                        c_Contract.MinZ = rect.Start.Z;
                        c_Contract.MaxZ = rect.End.Z;
                    }

                    once = true;
                }

				c_Contract.Blocks = list;
			}
			else
			{
				c_Contract.Blocks.Clear();
				c_Contract.MinZ = short.MinValue;
				c_Contract.MaxZ = short.MinValue;
			}

			NewGump();
		}

		private void SignLocSelect()
		{
			Owner.Target = new InternalTarget( this, c_Contract, TargetType.SignLoc );
		}

		private void MinZSelect()
		{
			Owner.SendMessage( "Target the base floor for your rental area." );
			Owner.Target = new InternalTarget( this, c_Contract, TargetType.MinZ );
		}


		private void MaxZSelect()
		{
			Owner.SendMessage( "Target the highest floor for your rental area." );
			Owner.Target = new InternalTarget( this, c_Contract, TargetType.MaxZ );
		}

		private void LengthUp()
		{
			if ( c_Contract == null )
				return;

			c_Contract.NextPriceType();

			if ( c_Contract.RentByTime == TimeSpan.FromDays( 0 ) )
				c_Contract.RentByTime = TimeSpan.FromDays( 1 );

			NewGump();
		}

		private void LengthDown()
		{
			if ( c_Contract == null )
				return;

			c_Contract.PrevPriceType();

			if ( c_Contract.RentByTime == TimeSpan.FromDays( 0 ) )
				c_Contract.RentByTime = TimeSpan.FromDays( 30 );

			NewGump();
		}

		private void Free()
		{
			c_Contract.Free = !c_Contract.Free;

			NewGump();
		}

		private void AddBlock()
		{
			Owner.SendMessage( "Target the north western corner." );
			Owner.Target = new InternalTarget( this, c_Contract, TargetType.BlockOne );
		}

		private void ClearBlocks()
		{
			if ( c_Contract == null )
				return;

			c_Contract.Blocks.Clear();

			c_Contract.ClearPreview();

			NewGump();
		}

		private void FinalizeSetup()
		{
			if ( c_Contract == null )
				return;

			if ( c_Contract.Price == 0 )
			{
				Owner.SendMessage( "You can't rent the area for 0 gold!" );
				NewGump();
				return;
			}

			c_Contract.Completed = true;
			c_Contract.BanLoc = c_Contract.ParentHouse.Region.GoLocation;

			if ( c_Contract.EntireHouse )
			{
				Point3D point = c_Contract.ParentHouse.Sign.Location;
				c_Contract.SignLoc = new Point3D( point.X, point.Y, point.Z-5 );
				c_Contract.Secures = Core.AOS ? c_Contract.ParentHouse.GetAosMaxSecures() : c_Contract.ParentHouse.MaxSecures;
				c_Contract.Locks = Core.AOS ? c_Contract.ParentHouse.GetAosMaxLockdowns() : c_Contract.ParentHouse.MaxLockDowns;
			}

			Owner.SendMessage( "You have finalized this rental contract.  Now find someone to sign it!" );
		}

		private class InternalTarget : Target
		{
			private ContractSetupGump c_Gump;
			private RentalContract c_Contract;
			private TargetType c_Type;
			private Point3D c_BoundOne;

			public InternalTarget( ContractSetupGump gump, RentalContract contract, TargetType type ) : this( gump, contract, type, Point3D.Zero ){}

			public InternalTarget( ContractSetupGump gump, RentalContract contract, TargetType type, Point3D point ) : base( 20, true, TargetFlags.None )
			{
				c_Gump = gump;
				c_Contract = contract;
				c_Type = type;
				c_BoundOne = point;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				IPoint3D point = (IPoint3D)o;

				if ( c_Contract == null || c_Contract.ParentHouse == null )
					return;

				if ( !c_Contract.ParentHouse.Region.Contains( new Point3D( point.X, point.Y, point.Z ) ) )
				{
					m.SendMessage( "You must target within the home." );
					m.Target = new InternalTarget( c_Gump, c_Contract, c_Type, c_BoundOne );
					return;
				}

				switch( c_Type )
				{
					case TargetType.SignLoc:
						c_Contract.SignLoc = new Point3D( point.X, point.Y, point.Z );
						c_Contract.ShowSignPreview();
						c_Gump.NewGump();
						break;

					case TargetType.MinZ:
                        if (!c_Contract.ParentHouse.Region.Contains(new Point3D(point.X, point.Y, point.Z)))
							m.SendMessage( "That isn't within your house." );
						else if ( c_Contract.HasContractedArea( point.Z ) )
							m.SendMessage( "That area is already taken by another rental contract." );
						else
						{
							c_Contract.MinZ = point.Z;

							if ( c_Contract.MaxZ < c_Contract.MinZ+19 )
								c_Contract.MaxZ = point.Z+19;
						}

                        c_Contract.ShowFloorsPreview(m);
						c_Gump.NewGump();
						break;

					case TargetType.MaxZ:
						if ( !c_Contract.ParentHouse.Region.Contains(new Point3D(point.X, point.Y, point.Z)) )
							m.SendMessage( "That isn't within your house." );
						else if ( c_Contract.HasContractedArea( point.Z ) )
							m.SendMessage( "That area is already taken by another rental contract." );
						else
						{
							c_Contract.MaxZ = point.Z+19;

							if ( c_Contract.MinZ > c_Contract.MaxZ )
								c_Contract.MinZ = point.Z;
						}

                        c_Contract.ShowFloorsPreview(m);
                        c_Gump.NewGump();
						break;

					case TargetType.BlockOne:
						m.SendMessage( "Now target the south eastern corner." );
						m.Target = new InternalTarget( c_Gump, c_Contract, TargetType.BlockTwo, new Point3D( point.X, point.Y, point.Z ) );
						break;

					case TargetType.BlockTwo:
						Rectangle2D rect = TownHouseSetupGump.FixRect( new Rectangle2D( c_BoundOne, new Point3D( point.X+1, point.Y+1, point.Z ) ) );

						if ( c_Contract.HasContractedArea( rect, point.Z ) )
							m.SendMessage( "That area is already taken by another rental contract." );
						else
						{
							c_Contract.Blocks.Add( rect );
							c_Contract.ShowAreaPreview( m );
						}

						c_Gump.NewGump();
						break;
				}
			}

			protected override void OnTargetCancel( Mobile m, TargetCancelType cancelType )
			{
				c_Gump.NewGump();
			}
		}
	}
}