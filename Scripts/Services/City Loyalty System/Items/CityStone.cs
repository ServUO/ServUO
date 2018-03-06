using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using System.Globalization;
using Server.ContextMenus;
using Server.Targeting;

namespace Server.Engines.CityLoyalty
{
	public class CityStone : Item
	{
        [CommandProperty(AccessLevel.GameMaster)]
		public CityLoyaltySystem City { get; set; }

        public List<BallotBox> Boxes { get; set; }
		
		public CityStone(CityLoyaltySystem city) : base(0xED4)
		{
			City = city;
            Movable = false;

            City.Stone = this;
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (CityLoyaltySystem.Enabled && CityLoyaltySystem.IsSetup() && from is PlayerMobile && from.InRange(from.Location, 3))
            {
                if (City != null && City.IsCitizen(from))
                    from.SendGump(new CityStoneGump(from as PlayerMobile, City));
                else
                    from.SendLocalizedMessage(1153888); // Only Citizens of this City may use the Election Stone. 
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if(City != null)
                list.Add(1153887, String.Format("#{0}", CityLoyaltySystem.GetCityLocalization(City.City)));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CityLoyaltySystem.Enabled || City == null)
                return;

            if (City.GovernorElect != null)
                list.Add(1154066, City.GovernorElect.Name); // Governor-Elect: ~1_NAME~
            else
                list.Add(1154067, City.PendingGovernor ? "#1154102" : City.Governor != null ? City.Governor.Name : "#1154072"); // Governor: ~1_NAME~

            if (City.Election != null)
            {
                DateTime dt;

                if (City.Election.CanNominate(out dt))
                    list.Add(1155756, dt.ToShortDateString()); // Nomination period ends after: ~1_DATE~

                if (City.Election.CanVote(out dt))
                    list.Add(1155757, dt.ToShortDateString()); // Voting Period Ends After: ~1_DATE~
            }

            list.Add(1154023, City.Treasury > 0 ? City.Treasury.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : City.Treasury.ToString()); // City Treasury Balance: ~1_AMT~
            list.Add(1154059, String.Format("#{0}", City.ActiveTradeDeal == TradeDeal.None ? 1011051 : (int)City.ActiveTradeDeal - 12)); // Current Trade Deal: ~1_GUILD~
            list.Add(1154907, City.CompletedTrades.ToString(CultureInfo.GetCultureInfo("en-US"))); // Trade Orders Delivered: ~1_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (!CityLoyaltySystem.Enabled || City == null)
                return;

            if (!City.IsCitizen(from))
            {
                if (City.Herald != null)
                    City.Herald.SayTo(from, 1154061, City.Definition.Name); // Only citizens of ~1_CITY~ may use the City Stone!
                else
                    from.SendLocalizedMessage(1154061, City.Definition.Name); // Only citizens of ~1_CITY~ may use the City Stone!

                return;
            }

            list.Add(new SimpleContextMenuEntry(from, 1154018, m => // Grant Citizen Title
                {
                    if(City.IsGovernor(m))
                    {
                        m.SendLocalizedMessage(1154027); // Which Citizen do you wish to bestow a title?
                        m.BeginTarget(10, false, TargetFlags.None, (mob, targeted) =>
                            {
                                PlayerMobile pm = targeted as PlayerMobile;

                                if (pm != null)
                                {
                                    if (City.IsCitizen(pm))
                                    {
                                        mob.SendGump(new PlayerTitleGump(mob as PlayerMobile, pm, City));
                                    }
                                    else
                                        mob.SendLocalizedMessage(1154029); // You may only bestow a title on citizens of this city!
                                }
                                else
                                    mob.SendLocalizedMessage(1154028); // You can only bestow a title on a player!
                            });
                    }
                }, enabled: City.IsGovernor(from)));

            list.Add(new SimpleContextMenuEntry(from, 1154031, m => // Open Trade Deal
            {
                if (City.IsGovernor(m))
                {
                    m.SendGump(new ChooseTradeDealGump(m as PlayerMobile, City));
                }
            }, enabled: City.IsGovernor(from)));

            list.Add(new SimpleContextMenuEntry(from, 1154277, m => // Open Inventory WTF is this?
            {
                if (City.IsGovernor(m))
                {
                    m.SendGump(new OpenInventoryGump(City));
                }
            }, enabled: City.IsGovernor(from)));

            list.Add(new SimpleContextMenuEntry(from, 1154278, m => // Place Ballot Box
            {
                if (City.IsGovernor(m))
                {
                    if (Boxes != null && Boxes.Count >= CityLoyaltySystem.MaxBallotBoxes)
                    {
                        m.SendMessage("You have reached the maximum amount of ballot boxes in your city.");
                        return;
                    }

                    m.SendMessage("Where would you like to place a ballot box?");
                    m.BeginTarget(3, true, TargetFlags.None, (mob, targeted) =>
                        {
                            if (targeted is IPoint3D)
                            {
                                IPoint3D p = targeted as IPoint3D;
                                Server.Spells.SpellHelper.GetSurfaceTop(ref p);
                                BallotBox box = new BallotBox();

                                if (CheckLocation(m, box, p))
                                {
                                    box.Owner = m;
                                    box.Movable = false;

                                    if (Boxes == null)
                                        Boxes = new List<BallotBox>();

                                    Boxes.Add(box);
                                    box.MoveToWorld(new Point3D(p), this.Map);

                                    m.SendMessage("{0} of {1} ballot boxes placed.", Boxes.Count.ToString(), CityLoyaltySystem.MaxBallotBoxes.ToString());
                                }
                                else
                                    box.Delete();
                            }
                        });
                }
            }, enabled: City.IsGovernor(from)));

            list.Add(new SimpleContextMenuEntry(from, 1154060, m => // Utilize Trade Deal
            {
                City.TryUtilizeTradeDeal(from);
            }, enabled: City.ActiveTradeDeal != TradeDeal.None));

            CityLoyaltyEntry entry = City.GetPlayerEntry<CityLoyaltyEntry>(from);

            list.Add(new SimpleContextMenuEntry(from, 1154019, m => // Remove City Title
            {
                if (entry != null && entry.CustomTitle != null)
                {
                    entry.CustomTitle = null;

                    if(m is PlayerMobile)
                        ((PlayerMobile)m).RemoveRewardTitle(1154017, true);

                    m.SendMessage("City Title removed.");
                }
            }, enabled: entry != null && entry.CustomTitle != null));

            list.Add(new SimpleContextMenuEntry(from, 1154068, m => // Accept Office
            {
                if (m == City.GovernorElect && City.Governor == null)
                {
                    m.SendGump(new AcceptOfficeGump(m as PlayerMobile, City));
                }
            }, enabled: City.GovernorElect == from && City.Governor == null && City.GetLoyaltyRating(from) >= LoyaltyRating.Unknown));
        }

        public bool CheckLocation(Mobile m, BallotBox box, IPoint3D p)
        {
            Region r = Region.Find(new Point3D(p), this.Map);

            if (!r.IsPartOf(City.Definition.Region))
            {
                m.SendMessage("You can only place a ballot box within the {0} city limits!", City.Definition.Name);
                return false;
            }

            if (!box.DropToWorld(new Point3D(p), this.Map))
            {
                m.SendMessage("You cannot place a ballot box there!");
                return false;
            }

            return true;
        }

		public CityStone(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write((int)City.City);

            writer.Write(Boxes == null ? 0 : Boxes.Count);
            if (Boxes != null)
            {
                Boxes.ForEach(b => writer.Write(b));
            }
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			City = CityLoyaltySystem.GetCityInstance((City)reader.ReadInt());

            if(City != null)
                City.Stone = this;

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                BallotBox box = reader.ReadItem() as BallotBox;

                if (box != null)
                {
                    if (Boxes == null)
                        Boxes = new List<BallotBox>();

                    Boxes.Add(box);
                }
            }
		}
	}
}