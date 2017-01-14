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
                return this.m_Donations;
            }
        }
		
        public List<CollectionItem> Rewards
        { 
            get
            {
                return this.m_Rewards;
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
                return this.m_Points;
            }
            set
            { 
                this.m_Points = value; 
				
                if (this.m_Points < 0)
                    this.m_Points = 0;
				
                while (this.m_Tier > 0 && this.m_Points < this.PreviousTier)
                    this.DecreaseTier();
					
                while (this.m_Tier < this.MaxTier && this.m_Points > this.CurrentTier)
                    this.IncreaseTier();
				
                this.InvalidateProperties(); 
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long PreviousTier
        {
            get
            {
                if (this.m_Tier > 2)
                {
                    long tier = this.m_StartTier * 2;
					
                    for (int i = 0; i < this.m_Tier - 2; i ++)
                        tier += (i + 3) * this.m_NextTier;				
					
                    return tier; 
                }
				
                return this.m_StartTier * this.m_Tier;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long CurrentTier
        {
            get
            {
                if (this.m_Tier > 1)
                    return this.PreviousTier + (this.m_Tier + 1) * this.m_NextTier;
				
                return this.m_StartTier + this.m_StartTier * this.m_Tier;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long StartTier
        {
            get
            {
                return this.m_StartTier;
            }
            set
            {
                this.m_StartTier = value;
                this.InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long NextTier
        {
            get
            {
                return this.m_NextTier;
            }
            set
            {
                this.m_NextTier = value;
                this.InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public long DailyDecay
        {
            get
            {
                return this.m_DailyDecay;
            }
            set
            {
                this.m_DailyDecay = value;
                this.InvalidateProperties();
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int Tier
        {
            get
            {
                return this.m_Tier;
            }
        }
        #endregion
		
        private List<List<object>> m_Tiers;
		
        public List<List<object>> Tiers
        {
            get
            {
                return this.m_Tiers;
            }
        }
		
        public BaseCollectionItem(int itemID)
            : base(itemID)
        {
            this.Movable = false;
		
            this.Init();
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
				
                if (from.InRange(this.Location, 2) && from is PlayerMobile && this.CanDonate((PlayerMobile)from))
                {
                    from.CloseGump(typeof(ComunityCollectionGump));
                    from.SendGump(new ComunityCollectionGump((PlayerMobile)from, this, this.Location));
                }
                else
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }
		
        public override void GetProperties(ObjectPropertyList list)
        {
            this.AddNameProperty(list);
			
            list.Add(1072819, this.m_Tier.ToString()); // Current Tier: ~1_TIER~
            list.Add(1072820, this.m_Points.ToString()); // Current Points: ~1_POINTS~
            list.Add(1072821, this.m_Tier > this.MaxTier ? 0.ToString() : this.CurrentTier.ToString()); // Points until next tier: ~1_POINTS~
        }
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((long)this.m_Points);
            writer.Write((long)this.m_StartTier);
            writer.Write((long)this.m_NextTier);
            writer.Write((long)this.m_DailyDecay);
            writer.Write((int)this.m_Tier);
			
            writer.Write((int)this.m_Tiers.Count);				
			
            for (int i = 0; i < this.m_Tiers.Count; i ++)
            {
                writer.Write((int)this.m_Tiers[i].Count);
				
                for (int j = 0; j < this.m_Tiers[i].Count; j ++)					
                    QuestWriter.Object(writer, this.m_Tiers[i][j]);
            }
        }
		
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_Points = reader.ReadLong();
            this.m_StartTier = reader.ReadLong();
            this.m_NextTier = reader.ReadLong();
            this.m_DailyDecay = reader.ReadLong();
            this.m_Tier = reader.ReadInt();			
			
            this.Init();
			
            for (int i = reader.ReadInt(); i > 0; i --)
            {
                List<object> list = new List<object>();
			
                for (int j = reader.ReadInt(); j > 0; j --)
                    list.Add(QuestReader.Object(reader));
					
                this.m_Tiers.Add(list);
            }
        }
		
        #region IComunityCollection
        public virtual void Donate(PlayerMobile player, CollectionItem item, int amount)
        {
            int points = (int)(amount * item.Points);
		
            player.AddCollectionPoints(this.CollectionID, points);
			
            player.SendLocalizedMessage(1072816); // Thank you for your donation!
            player.SendLocalizedMessage(1072817, points.ToString()); // You have earned ~1_POINTS~ reward points for this donation.	
            player.SendLocalizedMessage(1072818, points.ToString()); // The Collection has been awarded ~1_POINTS~ points
			
            this.Points += points;
			
            this.InvalidateProperties();
        }
		
        public virtual void Reward(PlayerMobile player, CollectionItem reward, int hue)
        {
            Item item = QuestHelper.Construct(reward.Type) as Item;
			
            if (item != null && player.AddToBackpack(item))
            {
                if (hue > 0)
                    item.Hue = hue;
				
                player.AddCollectionPoints(this.CollectionID, (int)reward.Points * -1);
                player.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                player.PlaySound(0x5A7);

                if (reward.QuestItem)
                    CollectionsObtainObjective.CheckReward(player, item);
            }
            else if (item != null)
            {
                player.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                item.Delete();
            }
			
            reward.OnGiveReward(player, this, hue);

			player.CloseGump(typeof(ComunityCollectionGump));
            player.SendGump(new ComunityCollectionGump(player, this, this.Location));
        }
		
        public virtual void DonatePet(PlayerMobile player, BaseCreature pet)
        {
            for (int i = 0; i < this.m_Donations.Count; i ++)
                if (this.m_Donations[i].Type == pet.GetType())
                {
                    pet.Delete();
                    this.Donate(player, this.m_Donations[i], 1);
                    return;
                }
				
            player.SendLocalizedMessage(1073113); // This Collection is not accepting that type of creature.
        }

        #endregion
		
        public virtual void IncreaseTier()
        { 
            this.m_Tier += 1;
        }
		
        public virtual void DecreaseTier()
        { 
            this.m_Tier -= 1;
			
            if (this.m_Tiers != null && this.m_Tiers.Count > 0)
            {
                for (int i = 0; i < this.m_Tiers[this.m_Tiers.Count - 1].Count; i ++)
                {
                    if (this.m_Tiers[this.m_Tiers.Count - 1][i] is Item)
                        ((Item)this.m_Tiers[this.m_Tiers.Count - 1][i]).Delete();
                    else if (this.m_Tiers[this.m_Tiers.Count - 1][i] is Mobile)
                        ((Mobile)this.m_Tiers[this.m_Tiers.Count - 1][i]).Delete();
                }
				
                this.m_Tiers.RemoveAt(this.m_Tiers.Count - 1);
            }
        }
		
        public virtual void Init()
        { 
            if (this.m_Donations == null)
                this.m_Donations = new List<CollectionItem>();
				
            if (this.m_Rewards == null)
                this.m_Rewards = new List<CollectionItem>();
			
            if (this.m_Tiers == null)
                this.m_Tiers = new List<List<object>>();
					
            // start decay timer
            if (this.m_DailyDecay > 0)
            {
                DateTime today = DateTime.Today;
                today.AddDays(1);
				
                new CollectionDecayTimer(this, today - DateTime.UtcNow);
            }
        }
		
        public virtual bool CanDonate(PlayerMobile player)
        {
            return true;
        }
    }
}