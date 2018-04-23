// Script: GetPet
// Version: 1.0
// Author: Datguy (Morpheus)
// Servers: RunUO 2.0
// Date: 9/20/2007
// Purpose: 
// Player Command. Allows players Allows players to retrieve their pets they may have lost for a fee
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;



namespace Server.Commands
{
    public class GetPet
    {

        private int m_Pets;

        public static void Initialize()
        {
            CommandSystem.Register("GetPet", AccessLevel.Player, new CommandEventHandler(GetPet_OnCommand));
        }

        [Usage("GetPet")]
        [Description("Teleports all your pets to your location.")]
        public static void GetPet_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            ArrayList pet = new ArrayList();
            
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;

                    if ((bc.Controlled && bc.ControlMaster == from) || (bc.Summoned && bc.SummonMaster == from))
                        pet.Add(bc);
                }
            }
            from.SendGump(new GetPetGump(pet.Count));
        }   
    }
}


namespace Server.Gumps
{
    public class GetPetGump : Gump
    {
         //public GetPetGump(Mobile m) : base(150, 50)
        private int m_Price;
        private int m_Pet;
        private int m_cycle;
        public GetPetGump( int pet ) : base( 150, 50 )
        {
            m_Pet = pet;
            m_Price = 100 * m_Pet;////Price per Pet 3500

            Closable = false;

            AddPage(0);

            AddImage(0, 0, 3600);

            AddImageTiled(0, 14, 15, 200, 3603);
            AddImageTiled(380, 14, 14, 200, 3605);

            AddImage(0, 201, 3606);

            AddImageTiled(15, 201, 370, 16, 3607);
            AddImageTiled(15, 0, 370, 16, 3601);

            AddImage(380, 0, 3602);

            AddImage(380, 201, 3608);

            AddImageTiled(15, 15, 365, 190, 2624);

            AddRadio(30, 140, 9727, 9730, true, 1);
            AddHtmlLocalized(65, 145, 300, 25, 1060015, 0x7FFF, false, false); // Grudgingly pay the money

            AddRadio(30, 175, 9727, 9730, false, 0);
            AddHtml(65, 178, 300, 25, "<BASEFONT COLOR=White>I'll find my pets myself.</BASEFONT>", false, false);

            AddHtml(30, 20, 360, 35, "<BASEFONT COLOR=Red>Pay the price per pet, and your pets will be brought to you, even if your sitting on it!, so shrink it.</BASEFONT>", false, false);

            AddHtmlLocalized(30, 105, 345, 40, 1060018, 0x5B2D, false, false); // Do you accept the fee, which will be withdrawn from your Ledger or Bank?

            AddImage(65, 72, 5605);

            AddImageTiled(80, 90, 200, 1, 9107);
            AddImageTiled(95, 92, 200, 1, 9157);

            AddLabel(90, 70, 1645, m_Price.ToString());
            AddLabel(130, 70, 1645, " gold coins for " + m_Pet + " pets found in world");

            AddButton(290, 175, 247, 248, 2, GumpButtonType.Reply, 0);

            AddImageTiled(15, 14, 365, 1, 9107);
            AddImageTiled(380, 14, 1, 190, 9105);
            AddImageTiled(15, 205, 365, 1, 9107);
            AddImageTiled(15, 14, 1, 190, 9105);
            AddImageTiled(0, 0, 395, 1, 9157);
            AddImageTiled(394, 0, 1, 217, 9155);
            AddImageTiled(0, 216, 395, 1, 9157);
            AddImageTiled(0, 0, 1, 217, 9155);
        }
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            m_cycle = 0;
            from.CloseGump(typeof(GetPetGump));

            if (info.ButtonID == 1 || info.ButtonID == 2)
            {

                if (m_Price > 0)
                {
                    if (info.IsSwitched(1))
                    {
                       
                        ArrayList pets = new ArrayList();

                        foreach (Mobile m in World.Mobiles.Values)
                        {
                            if (m is BaseCreature)
                            {
                                BaseCreature bc = (BaseCreature)m;

                                if ((bc.Controlled && bc.ControlMaster == from) || (bc.Summoned && bc.SummonMaster == from))
                                    pets.Add(bc);
                            }
                        }

                        if (pets.Count > 0)
                        {
                            m_Price = 100 * pets.Count;////Price per Pet 3500

/*   To use Gold Ledger remove this line                          
                                                           Item[] items = from.Backpack.FindItemsByType(typeof(GoldLedger));
                                                           foreach (GoldLedger tl in items)
                                                           {
                                                               if (tl.Owner == from.Serial && tl.Gold - m_Price >= 0)
                                                               {
                                                                   tl.Gold = (tl.Gold - m_Price); //withdraw gold
                                                                   //Delete();
                                                                   from.SendMessage(1174, "{0} Gold to your Gold had been withdrawn from ledger.", m_Price); //send a message to the player gold was taken from ledger
                                                                   m_cycle = 1;

                                                                   for (int i = 0; i < pets.Count; ++i)
                                                                   {
                                                                       Mobile pet = (Mobile)pets[i];

                                                                       if (pet is IMount)
                                                                           ((IMount)pet).Rider = null; // make sure it's dismounted

                                                                       pet.MoveToWorld(from.Location, from.Map);
                                                                       Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 30, 5052);
                                                                       Effects.PlaySound(from.Location, from.Map, 0x201);
                                                                       //return;
                                                                   }
                                                               }
                                                           }
To use Gold Ledger remove this line   */

                            if (m_cycle == 0)
                                {
                                    if (Banker.Withdraw(from, m_Price))
                                    {
                                        from.SendLocalizedMessage(1060398, m_Price.ToString()); // Amount charged
                                        from.SendLocalizedMessage(1060022, Banker.GetBalance(from).ToString()); // Amount left, from bank

                                        for (int i = 0; i < pets.Count; ++i)
                                        {
                                            Mobile pet = (Mobile)pets[i];

                                            if (pet is IMount)
                                                ((IMount)pet).Rider = null; // make sure it's dismounted

                                            pet.MoveToWorld(from.Location, from.Map);

                                            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 30, 5052);
                                            Effects.PlaySound(from.Location, from.Map, 0x201);


                                        }
                                    }
                                    else
                                        from.SendMessage("Unfortunately, you do not have enough cash in the bank to get your pets... ya mad bro");
                                    return;
                                }
                           }
                            
                           
                              
                        }


                    }

                }
            }
        }
    }


