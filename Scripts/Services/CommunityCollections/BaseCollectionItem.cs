using System;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseCollectionItem : Item, IComunityCollection
    { 
        #region IComunityCollection		
        public abstract Collection CollectionID { get; }
        public abstract int MaxTier { get; }
		
        private List<CollectionItem> m_Donations;
        private List<CollectionItem> m_Rewards;
		
        public List<CollectionItem> Donations
        { 
            get
            {
                return m_Donations;
            }
        }
		
        public List<CollectionItem> Rewards
        { 
            get
            {
                return m_Rewards;
            }
        }
		
        private long m_Points;
        private long m_StartTier;
        private long m_NextTier;
        private long m_DailyDecay;
        private int m_Tier;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long Points
        {
            get
            {
                return m_Points;
            }
            set
            { 
                m_Points = value; 
				
                if (m_Points < 0)
                    m_Points = 0;
				
                while (m_Tier > 0 && m_Points < PreviousTier)
                    DecreaseTier();
					
                while (m_Tier < MaxTier && m_Points > CurrentTier)
                    IncreaseTier();
				
                InvalidateProperties(); 
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long PreviousTier
        {
            get
            {
                if (m_Tier > 2)
                {
                    long tier = m_StartTier * 2;
					
                    for (int i = 0; i < m_Tier - 2; i ++)
                        tier += (i + 3) * m_NextTier;				
					
                    return tier; 
                }
				
                return m_StartTier * m_Tier;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long CurrentTier
        {
            get
            {
                if (m_Tier > 1)
                    return PreviousTier + (m_Tier + 1) * m_NextTier;
				
                return m_StartTier + m_StartTier * m_Tier;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long StartTier
        {
            get
            {
                return m_StartTier;
            }
            set
            {
                m_StartTier = value;
                InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long NextTier
        {
            get
            {
                return m_NextTier;
            }
            set
            {
                m_NextTier = value;
                InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long DailyDecay
        {
            get
            {
                return m_DailyDecay;
            }
            set
            {
                m_DailyDecay = value;
                InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int Tier
        {
            get
            {
                return m_Tier;
            }
        }
        #endregion
		
        private List<List<object>> m_Tiers;
		
        public List<List<object>> Tiers
        {
            get
            {
                return m_Tiers;
            }
        }
		
        public BaseCollectionItem(int itemID)
            : base(itemID)
        {
            Movable = false;
		
            Init();
        }
		
        public BaseCollectionItem(Serial serial)
            : base(serial)
        {
        }
		
        public override void OnDoubleClick(Mobile from)
        {
            if (from.Alive)
            {
                if (from.NetState == null || !from.NetState.SupportsExpansion(Expansion.ML))
                {
                    from.SendLocalizedMessage(1073651); // You must have Mondain's Legacy before proceeding...			
                    return;
                }
                else if (!MondainsLegacy.PublicDonations && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(1042753, "Public donations"); // ~1_SOMETHING~ has been temporarily disabled.
                    return;
                }
				
                if (from.InRange(Location, 2) && from is PlayerMobile && CanDonate((PlayerMobile)from))
                {
                    from.CloseGump(typeof(CommunityCollectionGump));
                    from.SendGump(new CommunityCollectionGump((PlayerMobile)from, this, Location));
                }
                else
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }
		
        public override void GetProperties(ObjectPropertyList list)
        {
            AddNameProperty(list);
			
            list.Add(1072819, m_Tier.ToString()); // Current Tier: ~1_TIER~
            list.Add(1072820, m_Points.ToString()); // Current Points: ~1_POINTS~
            list.Add(1072821, m_Tier > MaxTier ? 0.ToString() : CurrentTier.ToString()); // Points until next tier: ~1_POINTS~
        }
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((long)m_Points);
            writer.Write((long)m_StartTier);
            writer.Write((long)m_NextTier);
            writer.Write((long)m_DailyDecay);
            writer.Write((int)m_Tier);
			
            writer.Write((int)m_Tiers.Count);				
			
            for (int i = 0; i < m_Tiers.Count; i ++)
            {
                writer.Write((int)m_Tiers[i].Count);
				
                for (int j = 0; j < m_Tiers[i].Count; j ++)					
                    QuestWriter.Object(writer, m_Tiers[i][j]);
            }
        }
		
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            m_Points = reader.ReadLong();
            m_StartTier = reader.ReadLong();
            m_NextTier = reader.ReadLong();
            m_DailyDecay = reader.ReadLong();
            m_Tier = reader.ReadInt();			
			
            Init();
			
            for (int i = reader.ReadInt(); i > 0; i --)
            {
                List<object> list = new List<object>();
			
                for (int j = reader.ReadInt(); j > 0; j --)
                    list.Add(QuestReader.Object(reader));
					
                m_Tiers.Add(list);
            }
        }
		
        #region IComunityCollection
        public virtual void Donate(PlayerMobile player, CollectionItem item, int amount)
        {
            int points = (int)Math.Round(amount * item.Points);
		
            player.AddCollectionPoints(CollectionID, points);
			
            player.SendLocalizedMessage(1072816); // Thank you for your donation!
            player.SendLocalizedMessage(1072817, points.ToString()); // You have earned ~1_POINTS~ reward points for this donation.	
            player.SendLocalizedMessage(1072818, points.ToString()); // The Collection has been awarded ~1_POINTS~ points
			
            Points += points;
			
            InvalidateProperties();
        }
		
        public virtual void Reward(PlayerMobile player, CollectionItem reward, int hue)
        {
            Item item = QuestHelper.Construct(reward.Type) as Item;
			
            if (item != null && player.AddToBackpack(item))
            {
                if (hue > 0)
                    item.Hue = hue;
				
                player.AddCollectionPoints(CollectionID, (int)reward.Points * -1);
                player.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                player.PlaySound(0x5A7);

                if (reward.QuestItem)
                    CollectionsObtainObjective.CheckReward(player, item);

                reward.OnGiveReward(player, item, this, hue);
            }
            else if (item != null)
            {
                player.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                item.Delete();
            }

			player.CloseGump(typeof(CommunityCollectionGump));
            player.SendGump(new CommunityCollectionGump(player, this, Location));
        }
		
        public virtual void DonatePet(PlayerMobile player, BaseCreature pet)
        {
            for (int i = 0; i < m_Donations.Count; i ++)
                if (m_Donations[i].Type == pet.GetType())
                {
                    pet.Delete();
                    Donate(player, m_Donations[i], 1);
                    return;
                }
				
            player.SendLocalizedMessage(1073113); // This Collection is not accepting that type of creature.
        }

        #endregion
		
        public virtual void IncreaseTier()
        { 
            m_Tier += 1;
        }
		
        public virtual void DecreaseTier()
        { 
            m_Tier -= 1;
			
            if (m_Tiers != null && m_Tiers.Count > 0)
            {
                for (int i = 0; i < m_Tiers[m_Tiers.Count - 1].Count; i ++)
                {
                    if (m_Tiers[m_Tiers.Count - 1][i] is Item)
                        ((Item)m_Tiers[m_Tiers.Count - 1][i]).Delete();
                    else if (m_Tiers[m_Tiers.Count - 1][i] is Mobile)
                        ((Mobile)m_Tiers[m_Tiers.Count - 1][i]).Delete();
                }
				
                m_Tiers.RemoveAt(m_Tiers.Count - 1);
            }
        }
		
        public virtual void Init()
        { 
            if (m_Donations == null)
                m_Donations = new List<CollectionItem>();
				
            if (m_Rewards == null)
                m_Rewards = new List<CollectionItem>();
			
            if (m_Tiers == null)
                m_Tiers = new List<List<object>>();
					
            // start decay timer
            if (m_DailyDecay > 0)
            {
                DateTime today = DateTime.Today.AddDays(1);
				
                new CollectionDecayTimer(this, today - DateTime.UtcNow);
            }
        }
		
        public virtual bool CanDonate(PlayerMobile player)
        {
            return true;
        }
    }
}
