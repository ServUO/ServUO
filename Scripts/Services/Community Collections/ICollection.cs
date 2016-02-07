using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server
{
    public enum Collection
    {
        VesperMuseum,
        MoonglowZoo,
		
        // Britain library
        MaceAndBlade,
        FoldedSteel,
        Trades,
        ArtsSection,
        SongsOfNote,
        UnderstandingAnimals,
        LightAndMight,
        OilAndOubliette,
        WizardsCompendium,
        BritanniaWaters,
        PastTreasures,
        SkeletonKey
    }

    public interface IComunityCollection
    {
        Collection CollectionID { get; }
        long Points { get; set; }
        long CurrentTier { get; }
        long PreviousTier { get; }
        long StartTier { get; set; }
        long NextTier { get; set; }
        long DailyDecay { get; set; }
        int Tier { get; }
        int MaxTier { get; }
        List<CollectionItem> Donations { get; }
        List<CollectionItem> Rewards { get; }
        void Donate(PlayerMobile player, CollectionItem item, int amount);

        void Reward(PlayerMobile player, CollectionItem reward, int hue);

        void DonatePet(PlayerMobile player, BaseCreature pet);
    }
}