using System;
using Server;

namespace Knives.TownHouses
{
	public class ContractConfirmGump : GumpPlusLight
	{
		private RentalContract c_Contract;

		public ContractConfirmGump( Mobile m, RentalContract rc ) : base( m, 100, 100 )
		{
			m.CloseGump( typeof( ContractConfirmGump ) );

			c_Contract = rc;
		}

		protected override void BuildGump()
		{
            int width = 300;
            int y = 0;

			if ( c_Contract.RentalClient == null )
				AddHtml( 0, y+5, width, HTML.Black + "<CENTER>Rent this House?");
			else
				AddHtml( 0, y+5, width, HTML.Black + "<CENTER>Rental Agreement");

			string text = String.Format( "  I, {0}, agree to rent this property from {1} for the sum of {2} every {3}.  " +
				"The funds for this payment will be taken directly from my bank.  In the case where " +
				"I cannot pay this fee, the property will return to {1}.  I may cancel this agreement at any time by " +
				"demolishing the property.  {1} may also cancel this agreement at any time by either demolishing their " +
				"property or canceling the contract, in which case your security deposit will be returned.",
				c_Contract.RentalClient == null ? "_____" : c_Contract.RentalClient.Name,
				c_Contract.RentalMaster.Name,
				c_Contract.Free ? 0 : c_Contract.Price,
				c_Contract.PriceTypeShort.ToLower() );

			text += "<BR>   Here is some more info reguarding this property:<BR>";

			text += String.Format( "<CENTER>Lockdowns: {0}<BR>", c_Contract.Locks );
			text += String.Format( "Secures: {0}<BR>", c_Contract.Secures );
			text += String.Format( "Floors: {0}<BR>", (c_Contract.MaxZ-c_Contract.MinZ < 200) ? (c_Contract.MaxZ-c_Contract.MinZ)/20+1 : 1 );
			text += String.Format( "Space: {0} cubic units", c_Contract.CalcVolume() );

			AddHtml( 40, y+=30, width-60, 200, HTML.Black + text, false, true );

            y += 200;

			if ( c_Contract.RentalClient == null )
			{
				AddHtml( 60, y+=20, 60, HTML.Black + "Preview");
				AddButton( 40, y+3, 0x837, 0x838, "Preview", new GumpCallback( Preview ) );

				bool locsec = c_Contract.ValidateLocSec();

				if ( Owner != c_Contract.RentalMaster && locsec )
				{
					AddHtml( width-100, y, 60, HTML.Black + "Accept");
					AddButton( width-60, y+3, 0x232C, 0x232D, "Accept", new GumpCallback( Accept ) );
				}
				else
					AddImage( width-60, y-10, 0x232C );

				if ( !locsec )
					Owner.SendMessage( (Owner == c_Contract.RentalMaster ? "You don't have the lockdowns or secures available for this contract." : "The owner of this contract cannot rent this property at this time.") );
			}
			else
			{
				if ( Owner == c_Contract.RentalMaster )
				{
					AddHtml( 60, y+=20, 100, HTML.Black + "Cancel Contract");
					AddButton( 40, y+3, 0x837, 0x838, "Cancel Contract", new GumpCallback( CancelContract ) );
				}
                else
				    AddImage( width-60, y+=20, 0x232C );
			}

            AddBackgroundZero( 0, 0, width, y+23, 0x24A4 );
        }

		protected override void OnClose()
		{
			c_Contract.ClearPreview();
		}

		private void Preview()
		{
            c_Contract.ShowAreaPreview(Owner);
			NewGump();
		}

		private void CancelContract()
		{
			if ( Owner == c_Contract.RentalClient )
				c_Contract.House.Delete();
			else
				c_Contract.Delete();
		}

		private void Accept()
		{
			if ( !c_Contract.ValidateLocSec() )
			{
				Owner.SendMessage( "The owner of this contract cannot rent this property at this time." );
				return;
			}

			c_Contract.Purchase( Owner );

			if ( !c_Contract.Owned )
				return;

			c_Contract.Visible = true;
			c_Contract.RentalClient = Owner;
			c_Contract.RentalClient.AddToBackpack( new RentalContractCopy( c_Contract ) );
		}
	}
}