using Server.Commands;
using Server.Items;
using System;
using System.Linq;

namespace Server.Engines.CityLoyalty
{
    public class CityLoyaltySetup
    {
        public static void Initialize()
        {
            if (CityLoyaltySystem.Enabled)
            {
                CommandSystem.Register("SetupCityLoyaltySystem", AccessLevel.GameMaster, Setup);
                CommandSystem.Register("DeleteCityLoyaltySystem", AccessLevel.GameMaster, Delete);
            }
        }

        public static void Delete(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            CityLoyaltySystem.Cities.ForEach(city =>
                {
                    if (city.Stone != null) city.Stone.Delete();
                    if (city.Herald != null) city.Herald.Delete();

                    if (city.Captain != null)
                    {
                        if (city.Captain.Box != null)
                            city.Captain.Box.Delete();

                        city.Captain.Delete();
                    }

                    if (city.Minister != null)
                    {
                        if (city.Minister.DonationCrate != null)
                            city.Minister.DonationCrate.Delete();

                        if (city.Minister.DonationPost != null)
                            city.Minister.DonationPost.Delete();

                        city.Minister.Delete();
                    }
                });
        }

        public static void Setup(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            TradeMinister minister;
            CityHerald herald;
            GuardCaptain capt;
            CityStone stone;
            CityItemDonation itemdonation;
            CityPetDonation petdonation;
            BoxOfRopes box;
            CityMessageBoard board;

            foreach (int c in Enum.GetValues(typeof(City)))
            {
                City city = (City)c;
                CityLoyaltySystem sys = null;

                switch (city)
                {
                    case City.Moonglow: sys = CityLoyaltySystem.Moonglow; break;
                    case City.Britain: sys = CityLoyaltySystem.Britain; break;
                    case City.Jhelom: sys = CityLoyaltySystem.Jhelom; break;
                    case City.Yew: sys = CityLoyaltySystem.Yew; break;
                    case City.Minoc: sys = CityLoyaltySystem.Minoc; break;
                    case City.Trinsic: sys = CityLoyaltySystem.Trinsic; break;
                    case City.SkaraBrae: sys = CityLoyaltySystem.SkaraBrae; break;
                    case City.NewMagincia: sys = CityLoyaltySystem.NewMagincia; break;
                    case City.Vesper: sys = CityLoyaltySystem.Vesper; break;
                }

                if (sys != null)
                {
                    minister = new TradeMinister(sys.City);
                    herald = new CityHerald(sys.City);
                    capt = new GuardCaptain(sys.City);
                    stone = new CityStone(sys);
                    itemdonation = new CityItemDonation(sys.City, minister);
                    petdonation = new CityPetDonation(sys.City, minister);
                    box = new BoxOfRopes(sys.City);
                    board = new CityMessageBoard(sys.City, 0xA0C5);

                    if (!HasType(sys, minister.GetType()))
                    {
                        sys.Minister = minister;
                        minister.MoveToWorld(sys.Definition.TradeMinisterLocation, CityLoyaltySystem.SystemMap);
                    }
                    else
                        minister.Delete();

                    if (!HasType(sys, herald.GetType()))
                    {
                        sys.Herald = herald;
                        herald.MoveToWorld(sys.Definition.HeraldLocation, CityLoyaltySystem.SystemMap);
                    }
                    else
                        herald.Delete();

                    if (!HasType(sys, capt.GetType()))
                    {
                        sys.Captain = capt;
                        capt.MoveToWorld(sys.Definition.GuardsmanLocation, CityLoyaltySystem.SystemMap);
                    }
                    else
                        capt.Delete();

                    if (!HasType(sys, stone.GetType()))
                    {
                        sys.Stone = stone;
                        stone.MoveToWorld(sys.Definition.StoneLocation, CityLoyaltySystem.SystemMap);
                    }
                    else
                        stone.Delete();

                    if (!HasType(sys, itemdonation.GetType()))
                        itemdonation.MoveToWorld(new Point3D(sys.Definition.TradeMinisterLocation.X, sys.Definition.TradeMinisterLocation.Y - 1, sys.Definition.TradeMinisterLocation.Z), CityLoyaltySystem.SystemMap);
                    else
                        itemdonation.Delete();

                    if (!HasType(sys, petdonation.GetType()))
                        petdonation.MoveToWorld(new Point3D(sys.Definition.TradeMinisterLocation.X, sys.Definition.TradeMinisterLocation.Y - 2, sys.Definition.TradeMinisterLocation.Z), CityLoyaltySystem.SystemMap);
                    else
                        petdonation.Delete();

                    if (!HasType(sys, box.GetType()))
                        box.MoveToWorld(new Point3D(sys.Definition.GuardsmanLocation.X, sys.Definition.GuardsmanLocation.Y - 1, sys.Definition.GuardsmanLocation.Z), CityLoyaltySystem.SystemMap);
                    else
                        box.Delete();

                    if (!HasType(sys, board.GetType()))
                    {
                        board.MoveToWorld(sys.Definition.BoardLocation, CityLoyaltySystem.SystemMap);
                        sys.Board = board;
                    }
                    else
                        board.Delete();

                    sys.CanUtilize = true;

                    m.SendMessage("{0} setup!", sys.Definition.Name);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                string name;
                Point3D p;

                switch (i)
                {
                    default:
                    case 0:
                        name = "Ocllo";
                        p = new Point3D(3674, 2648, 0);
                        break;
                    case 1:
                        name = "Nujel'm";
                        p = new Point3D(3765, 1219, 0);
                        break;
                    case 2:
                        name = "Serpent's Hold";
                        p = new Point3D(3017, 3452, 15);
                        break;
                }

                Region r = Region.Regions.FirstOrDefault(reg => reg.Map == Map.Felucca && reg.Name == name);

                if (r != null)
                {
                    SlimTheFence slim = new SlimTheFence();

                    if (!HasType(r, slim.GetType()))
                        slim.MoveToWorld(p, Map.Felucca);
                    else
                        slim.Delete();
                }
                else
                    Console.WriteLine("WARNING: {0} Region not found!", name);
            }

        }

        public static bool HasType(CityLoyaltySystem system, Type t)
        {
            return HasType(system.Definition.Region, t);
        }

        public static bool HasType(Region r, Type t)
        {
            if (r == null)
                return false;

            if (t.IsSubclassOf(typeof(Mobile)))
            {
                foreach (Mobile m in r.GetEnumeratedMobiles())
                {
                    if (m.GetType() == t)
                        return true;
                }
            }
            else if (t.IsSubclassOf(typeof(Item)))
            {
                foreach (Item i in r.GetEnumeratedItems())
                {
                    if (i.GetType() == t)
                        return true;
                }
            }

            return false;
        }

    }
}