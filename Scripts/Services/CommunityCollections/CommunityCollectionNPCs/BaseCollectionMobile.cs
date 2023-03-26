using System;
using System.Collections.Generic;

using Server.Engines.Quests;
using Server.Gumps;
using Server.Network;
using Server.Services.Community_Collections;

namespace Server.Mobiles
{
	public abstract class BaseCollectionMobile : BaseVendor, IComunityCollection
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos => m_SBInfos;

		public override bool IsActiveVendor => false;

		#region IComunityCollection

		public abstract Collection CollectionID { get; }
		public abstract int MaxTier { get; }

		public List<CollectionItem> Donations { get; private set; }
		public List<CollectionItem> Rewards { get; private set; }

		private long m_Points;
		private long m_StartTier;
		private long m_NextTier;
		private long m_DailyDecay;

		[CommandProperty(AccessLevel.GameMaster)]
		public long Points
		{
			get => m_Points;
			set
			{
				m_Points = value;

				if (m_Points < 0)
				{
					m_Points = 0;
				}

				while (Tier > 0 && m_Points < PreviousTier)
				{
					DecreaseTier();
				}

				while (Tier < MaxTier && m_Points > CurrentTier)
				{
					IncreaseTier();
				}

				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long PreviousTier
		{
			get
			{
				if (Tier > 2)
				{
					var tier = m_StartTier * 2;

					for (var i = 0; i < Tier - 2; i++)
					{
						tier += (i + 3) * m_NextTier;
					}

					return tier;
				}

				return m_StartTier * Tier;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long CurrentTier
		{
			get
			{
				if (Tier > 1)
				{
					return PreviousTier + (Tier + 1) * m_NextTier;
				}

				return m_StartTier + m_StartTier * Tier;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long StartTier
		{
			get => m_StartTier;
			set
			{
				m_StartTier = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long NextTier
		{
			get => m_NextTier;
			set
			{
				m_NextTier = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long DailyDecay
		{
			get => m_DailyDecay;
			set
			{
				m_DailyDecay = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Tier { get; private set; }

		#endregion

		public List<HashSet<object>> Tiers { get; private set; }

		private TextDefinition m_DonationTitle;

		[CommandProperty(AccessLevel.GameMaster)]
		public int DonationLabel
		{
			get => m_DonationTitle.Number;
			set => m_DonationTitle = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string DonationString
		{
			get => m_DonationTitle.String;
			set => m_DonationTitle = value;
		}

		public BaseCollectionMobile(string name, string title)
			: base(title)
		{
			Name = name;
			Frozen = true;
			CantWalk = true;

			Init();

			CollectionsSystem.RegisterMobile(this);
		}

		public BaseCollectionMobile(Serial serial)
			: base(serial)
		{
		}

		public override void InitSBInfo()
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.Alive)
			{
				if (!MondainsLegacy.PublicDonations && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
				{
					from.SendLocalizedMessage(1042753, "Public donations"); // ~1_SOMETHING~ has been temporarily disabled.
					return;
				}

				if (from.InRange(Location, 2) && from is PlayerMobile p && CanDonate(p))
				{
					p.CloseGump(typeof(CommunityCollectionGump));
					p.SendGump(new CommunityCollectionGump(p, this, Location));
				}
				else
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				}
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1072819, Tier.ToString()); // Current Tier: ~1_TIER~
			list.Add(1072820, m_Points.ToString()); // Current Points: ~1_POINTS~
			list.Add(1072821, Tier > MaxTier ? 0.ToString() : CurrentTier.ToString()); // Points until next tier: ~1_POINTS~

			if (DonationLabel > 0)
			{
				list.Add(DonationLabel);
			}
			else if (DonationString != null)
			{
				list.Add(DonationString);
			}
		}

		public CollectionData GetData()
		{
			var ret = new CollectionData
			{
				Collection = CollectionID,
				Points = Points,
				StartTier = StartTier,
				NextTier = NextTier,
				DailyDecay = DailyDecay,
				Tier = Tier,
				DonationTitle = m_DonationTitle,
			};

			ret.Tiers.AddRange(Tiers);

			return ret;
		}

		public void SetData(CollectionData data)
		{
			m_Points = data.Points;
			m_StartTier = data.StartTier;
			m_NextTier = data.NextTier;
			m_DailyDecay = data.DailyDecay;
			Tier = data.Tier;
			m_DonationTitle = data.DonationTitle;
			Tiers = data.Tiers;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version			
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			Init();

			if (version == 0)
			{
				m_Points = reader.ReadLong();
				m_StartTier = reader.ReadLong();
				m_NextTier = reader.ReadLong();
				m_DailyDecay = reader.ReadLong();
				Tier = reader.ReadInt();
				
				var title = QuestReader.Object(reader);

				if (title is TextDefinition def)
				{
					m_DonationTitle = def;
				}
				else if (title is string str)
				{
					m_DonationTitle = str;
				}
				else if (title is int num)
				{
					m_DonationTitle = num;
				}

				for (var i = reader.ReadInt(); i > 0; i--)
				{
					var list = new HashSet<object>();

					for (var j = reader.ReadInt(); j > 0; j--)
					{
						list.Add(QuestReader.Object(reader));
					}

					Tiers.Add(list);
				}
			}

			if (CantWalk)
			{
				Frozen = true;
			}

			CollectionsSystem.RegisterMobile(this);
		}

		#region IComunityCollection
		public virtual void Donate(PlayerMobile player, CollectionItem item, int amount)
		{
			var points = (int)Math.Round(amount * item.Points);

			player.AddCollectionPoints(CollectionID, points);

			player.SendLocalizedMessage(1072816); // Thank you for your donation!
			player.SendLocalizedMessage(1072817, points.ToString()); // You have earned ~1_POINTS~ reward points for this donation.	
			player.SendLocalizedMessage(1072818, points.ToString()); // The Collection has been awarded ~1_POINTS~ points

			Points += points;

			InvalidateProperties();
		}

		public virtual void Reward(PlayerMobile player, CollectionItem reward, int hue)
		{
			var item = Loot.Construct(reward.Type);

			if (item != null && player.AddToBackpack(item))
			{
				if (hue > 0)
				{
					item.Hue = hue;
				}

				player.AddCollectionPoints(CollectionID, (int)reward.Points * -1);
				player.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
				player.PlaySound(0x5A7);

				if (reward.QuestItem)
				{
					CollectionsObtainObjective.CheckReward(player, item);
				}

				reward.OnGiveReward(player, item, this, hue);
			}
			else if (item != null)
			{
				player.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
				item.Delete();
			}

			player.SendGump(new CommunityCollectionGump(player, this, Location));
		}

		public virtual void DonatePet(PlayerMobile player, BaseCreature pet)
		{
			for (var i = 0; i < Donations.Count; i++)
			{
				if (Donations[i].Type == pet.GetType())
				{
					pet.Delete();
					Donate(player, Donations[i], 1);
					return;
				}
			}

			player.SendLocalizedMessage(1073113); // This Collection is not accepting that type of creature.
		}

		#endregion

		public virtual void IncreaseTier()
		{
			Tier += 1;
		}

		public virtual void DecreaseTier()
		{
			Tier -= 1;

			if (Tiers != null && Tiers.Count > 0)
			{
				foreach (var obj in Tiers[Tiers.Count - 1])
				{
					if (obj is IEntity e)
					{
						e.Delete();
					}
				}

				Tiers.RemoveAt(Tiers.Count - 1);
			}
		}

		public virtual void Init()
		{
			if (Donations == null)
			{
				Donations = new List<CollectionItem>();
			}

			if (Rewards == null)
			{
				Rewards = new List<CollectionItem>();
			}

			if (Tiers == null)
			{
				Tiers = new List<HashSet<object>>();
			}

			// start decay timer
			if (m_DailyDecay > 0)
			{
				var today = DateTime.Today.AddDays(1);

				var timer = new CollectionDecayTimer(this, today - DateTime.UtcNow);

				timer.Start();
			}
		}

		public virtual bool CanDonate(PlayerMobile player)
		{
			return true;
		}

		public override void OnDelete()
		{
			base.OnDelete();

			CollectionsSystem.UnregisterMobile(this);
		}
	}
}
