using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Points;
using System.Collections.Generic;
using Server.Gumps;
using Server.Engines.Craft;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlCityRewardGump : BaseRewardGump
    {
        public override int YDist { get { return 15; } }

        public KotlCityRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, KotlCityRewards.Rewards, 1156988)
        {
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.TreasuresOfKotlCity.GetPoints(m);
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            Item item = null;

            if (citem.Type == typeof(RecipeScroll))
            {
                switch (index)
                {
                    default:
                    case 3: item = new RecipeScroll((int)CarpRecipes.KotlBlackRod); break;
                    case 4: item = new RecipeScroll((int)TinkerRecipes.DrSpectorLenses); break;
                    case 5: item = new RecipeScroll((int)TinkerRecipes.KotlAutomatonHead); break;
                }
            }
            else if (citem.Type == typeof(TreasuresOfKotlRewardDeed))
            {
                item = new TreasuresOfKotlRewardDeed(citem.Tooltip);
            }
            else if (citem.Type == typeof(TribalBanner))
            {
                switch (index)
                {
                    case 10: item = new TribalBanner(EodonTribe.Urali); break;
                    case 11: item = new TribalBanner(EodonTribe.Barrab); break;
                    case 12: item = new TribalBanner(EodonTribe.Sakkhra); break;
                    case 13: item = new TribalBanner(EodonTribe.Barako); break;
                    case 14: item = new TribalBanner(EodonTribe.Kurak); break;
                    case 15: item = new TribalBanner(EodonTribe.Jukari); break;
                }
            }

            if (item != null)
            {
                if (User.Backpack == null || !User.Backpack.TryDropItem(User, item, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    item.Delete();
                }
                else
                {
                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    User.PlaySound(0x5A7);
                }
            }
            else
            {
                base.OnConfirmed(citem, index);
            }

            PointsSystem.TreasuresOfKotlCity.DeductPoints(User, citem.Points);
        }
    }
}