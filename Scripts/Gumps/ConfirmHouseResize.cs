using System;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class ConfirmHouseResize : Gump
    {
        private readonly Mobile m_Mobile;
        private readonly BaseHouse m_House;
        public ConfirmHouseResize(Mobile mobile, BaseHouse house)
            : base(110, 100)
        {
            this.m_Mobile = mobile;
            this.m_House = house;

            mobile.CloseGump(typeof(ConfirmHouseResize));

            this.Closable = false;

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 280, 0x13BE);
            this.AddImageTiled(10, 10, 400, 20, 0xA40);
            this.AddAlphaRegion(10, 10, 400, 20);
            this.AddHtmlLocalized(10, 10, 400, 20, 1060635, 0x7800, false, false); // <CENTER>WARNING</CENTER>
            this.AddImageTiled(10, 40, 400, 200, 0xA40);
            this.AddAlphaRegion(10, 40, 400, 200);

            /* You are attempting to resize your house. You will be refunded the house's 
            value directly to your bank box. All items in the house will *remain behind* 
            and can be *freely picked up by anyone*. Once the house is demolished, however, 
            only this account will be able to place on the land for one hour. This *will* 
            circumvent the normal 7-day waiting period (if it applies to you). This action 
            will not un-condemn any other houses on your account. If you have other, 
            grandfathered houses, this action *WILL* condemn them. Are you sure you wish 
            to continue?*/
            this.AddHtmlLocalized(10, 40, 400, 200, 1080196, 0x7F00, false, true); 

            this.AddImageTiled(10, 250, 400, 20, 0xA40);
            this.AddAlphaRegion(10, 250, 400, 20);
            this.AddButton(10, 250, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            this.AddButton(210, 250, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(40, 250, 170, 20, 1011036, 0x7FFF, false, false); // OKAY
            this.AddHtmlLocalized(240, 250, 170, 20, 1011012, 0x7FFF, false, false); // CANCEL
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1 && !this.m_House.Deleted)
            {
                if (this.m_House.IsOwner(this.m_Mobile))
                {
                    if (this.m_House.MovingCrate != null || this.m_House.InternalizedVendors.Count > 0)
                    {
                        this.m_Mobile.SendLocalizedMessage(1080455); // You can not resize your house at this time. Please remove all items fom the moving crate and try again.
                        return;
                    }
                    else if (!Guilds.Guild.NewGuildSystem && this.m_House.FindGuildstone() != null)
                    {
                        this.m_Mobile.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
                        return;
                    }
                    /*else if ( m_House.PlayerVendors.Count > 0 )
                    {
                    m_Mobile.SendLocalizedMessage( 503236 ); // You need to collect your vendor's belongings before moving.
                    return;
                    }*/
                    else if (this.m_House.HasRentedVendors && this.m_House.VendorInventories.Count > 0)
                    {
                        this.m_Mobile.SendLocalizedMessage(1062679); // You cannot do that that while you still have contract vendors or unclaimed contract vendor inventory in your house.
                        return;
                    }
                    else if (this.m_House.HasRentedVendors)
                    {
                        this.m_Mobile.SendLocalizedMessage(1062680); // You cannot do that that while you still have contract vendors in your house.
                        return;
                    }
                    else if (this.m_House.VendorInventories.Count > 0)
                    {
                        this.m_Mobile.SendLocalizedMessage(1062681); // You cannot do that that while you still have unclaimed contract vendor inventory in your house.
                        return;
                    }

                    if (this.m_Mobile.AccessLevel >= AccessLevel.GameMaster)
                    {
                        this.m_Mobile.SendMessage("You do not get a refund for your house as you are not a player");
                        this.m_House.RemoveKeys(this.m_Mobile);
                        new TempNoHousingRegion(this.m_House, this.m_Mobile);
                        this.m_House.Delete();
                    }
                    else
                    {
                        Item toGive = null;

                        if (this.m_House.IsAosRules)
                        {
                            if (this.m_House.Price > 0)
                                toGive = new BankCheck(this.m_House.Price);
                            else
                                toGive = this.m_House.GetDeed();
                        }
                        else
                        {
                            toGive = this.m_House.GetDeed();

                            if (toGive == null && this.m_House.Price > 0)
                                toGive = new BankCheck(this.m_House.Price);
                        }

                        if (toGive != null)
                        {
                            BankBox box = this.m_Mobile.BankBox;

                            if (box.TryDropItem(this.m_Mobile, toGive, false))
                            {
                                if (toGive is BankCheck)
                                    this.m_Mobile.SendLocalizedMessage(1060397, ((BankCheck)toGive).Worth.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.

                                this.m_House.RemoveKeys(this.m_Mobile);
                                new TempNoHousingRegion(this.m_House, this.m_Mobile);
                                this.m_House.Delete();
                            }
                            else
                            {
                                toGive.Delete();
                                this.m_Mobile.SendLocalizedMessage(500390); // Your bank box is full.
                            }
                        }
                        else
                        {
                            this.m_Mobile.SendMessage("Unable to refund house.");
                        }
                    }
                }
                else
                {
                    this.m_Mobile.SendLocalizedMessage(501320); // Only the house owner may do this.
                }
            }
            else if (info.ButtonID == 0)
            {
                this.m_Mobile.CloseGump(typeof(ConfirmHouseResize));
                this.m_Mobile.SendGump(new HouseGumpAOS(HouseGumpPageAOS.Customize, this.m_Mobile, this.m_House));
            }
        }
    }
}