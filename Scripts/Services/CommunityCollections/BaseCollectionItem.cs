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

		public List<CollectionItem> Donations { get; private set; }
		public List<CollectionItem> Rewards { get; private set; }

		private long m_Points;

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

		private long m_StartTier;

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

		private long m_NextTier;

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

		private long m_DailyDecay;

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

		public List<List<object>> Tiers { get; private set; }

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
			if (from?.Deleted == false && from.Alive)
			{
				if (!MondainsLegacy.PublicDonations && from.AccessLevel < AccessLevel.GameMaster)
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
			AddNameProperty(list);

			list.Add(1072819, Tier.ToString("N0")); // Current Tier: ~1_TIER~
			list.Add(1072820, m_Points.ToString("N0")); // Current Points: ~1_POINTS~
			list.Add(1072821, Tier > MaxTier ? "0" : CurrentTier.ToString("N0")); // Points until next tier: ~1_POINTS~
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_Points);
			writer.Write(m_StartTier);
			writer.Write(m_NextTier);
			writer.Write(m_DailyDecay);
			writer.Write(Tier);

			writer.Write(Tiers.Count);

			for (var i = 0; i < Tiers.Count; i++)
			{
				writer.Write(Tiers[i].Count);

				for (var j = 0; j < Tiers[i].Count; j++)
				{
					QuestWriter.Object(writer, Tiers[i][j]);
				}
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt(); // version

			m_Points = reader.ReadLong();
			m_StartTier = reader.ReadLong();
			m_NextTier = reader.ReadLong();
			m_DailyDecay = reader.ReadLong();
			Tier = reader.ReadInt();

			Init();

			for (var i = reader.ReadInt(); i > 0; i--)
			{
				var list = new List<object>();

				for (var j = reader.ReadInt(); j > 0; j--)
				{
					list.Add(QuestReader.Object(reader));
				}

				Tiers.Add(list);
			}
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

			player.CloseGump(typeof(CommunityCollectionGump));
			player.SendGump(new CommunityCollectionGump(player, this, Location));
		}

		public virtual void DonatePet(PlayerMobile player, BaseCreature pet)
		{
			for (var i = 0; i < Donations.Count; i++)
			{
				if (Donations[i].Type == pet.GetType() || MoonglowDonationBox.HasGroup(pet.GetType(), Donations[i].Type))
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

			if (Tiers?.Count > 0)
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
				Tiers = new List<List<object>>();
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
	}
}
