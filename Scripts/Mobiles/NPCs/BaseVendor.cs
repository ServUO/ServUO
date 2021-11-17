#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Services.Virtues;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
	public enum VendorShoeType
	{
		None,
		Shoes,
		Boots,
		Sandals,
		ThighBoots
	}

	public abstract class BaseVendor : BaseCreature, IVendor
	{
		[ConfigProperty("Vendors.UseVendorEconomy")]
		public static bool UseVendorEconomy { get => Config.Get("Vendors.UseVendorEconomy", !Siege.SiegeShard); set => Config.Set("Housing.UseVendorEconomy", value); }

		[ConfigProperty("Vendors.BuyItemChange")]
		public static int BuyItemChange { get => Config.Get("Vendors.BuyItemChange", 1000); set => Config.Set("Housing.BuyItemChange", value); }

		[ConfigProperty("Vendors.SellItemChange")]
		public static int SellItemChange { get => Config.Get("Vendors.SellItemChange", 1000); set => Config.Set("Housing.SellItemChange", value); }

		[ConfigProperty("Vendors.EconomyStockAmount")]
		public static int EconomyStockAmount { get => Config.Get("Vendors.EconomyStockAmount", 500); set => Config.Set("Housing.EconomyStockAmount", value); }

		[ConfigProperty("Vendors.RestockDelay")]
		public static TimeSpan DelayRestock { get => TimeSpan.FromMinutes(Config.Get("Vendors.RestockDelay", 60.0)); set => Config.Set("Housing.RestockDelay", value.TotalMinutes); }

		[ConfigProperty("Vendors.MaxSell")]
		public static int MaxSell { get => Config.Get("Vendors.MaxSell", 500); set => Config.Set("Housing.MaxSell", value); }

		public static List<BaseVendor> AllVendors { get; } = new List<BaseVendor>(0x4000);

		protected abstract List<SBInfo> SBInfos { get; }

		private readonly ArrayList m_ArmorBuyInfo = new ArrayList();
		private readonly ArrayList m_ArmorSellInfo = new ArrayList();

		private DateTime m_LastRestock;

		public override bool CanTeach => true;

		public override bool BardImmune => true;

		public override bool PlayerRangeSensitive => true;

		public override bool UseSmartAI => true;

		public override bool AlwaysInnocent => true;

		public virtual bool IsActiveVendor => true;
		public virtual bool IsActiveBuyer => IsActiveVendor && !Siege.SiegeShard; // response to vendor SELL
		public virtual bool IsActiveSeller => IsActiveVendor; // repsonse to vendor BUY
		public virtual bool HasHonestyDiscount => true;

		public virtual NpcGuild NpcGuild => NpcGuild.None;

		public virtual bool ChangeRace => true;

		public override bool IsInvulnerable => true;

		public DateTime NextTrickOrTreat { get; set; }

		public virtual double GetMoveDelay => Utility.RandomMinMax(30, 120);

		public override bool ShowFameTitle => false;

		public virtual bool IsValidBulkOrder(Item item)
		{
			return false;
		}

		public virtual Item CreateBulkOrder(Mobile from, bool fromContextMenu)
		{
			return null;
		}

		public virtual bool SupportsBulkOrders(Mobile from)
		{
			return false;
		}

		public virtual TimeSpan GetNextBulkOrder(Mobile from)
		{
			return TimeSpan.Zero;
		}

		public virtual void OnSuccessfulBulkOrderReceive(Mobile from)
		{ }

		public virtual BODType BODType => BODType.Smith;

		public virtual int GetPriceScalar()
		{
			return 100;
		}

		public void UpdateBuyInfo()
		{
			var priceScalar = GetPriceScalar();

			var buyinfo = (IBuyItemInfo[])m_ArmorBuyInfo.ToArray(typeof(IBuyItemInfo));

			if (buyinfo != null)
			{
				foreach (var info in buyinfo)
				{
					info.PriceScalar = priceScalar;
				}
			}
		}

		private class BulkOrderInfoEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseVendor m_Vendor;

			public BulkOrderInfoEntry(Mobile from, BaseVendor vendor)
				: base(6152, -1)
			{
				Enabled = vendor.CheckVendorAccess(from);

				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if (!m_From.InRange(m_Vendor.Location, 20))
					return;

				EventSink.InvokeBODOffered(new BODOfferEventArgs(m_From, m_Vendor));

				if (m_Vendor.SupportsBulkOrders(m_From) && m_From is PlayerMobile)
				{
					if (BulkOrderSystem.NewSystemEnabled)
					{
						if (BulkOrderSystem.CanGetBulkOrder(m_From, m_Vendor.BODType) || m_From.AccessLevel > AccessLevel.Player)
						{
							var bulkOrder = BulkOrderSystem.CreateBulkOrder(m_From, m_Vendor.BODType, true);

							if (bulkOrder is LargeBOD)
							{
								m_From.CloseGump(typeof(LargeBODAcceptGump));
								m_From.SendGump(new LargeBODAcceptGump(m_From, (LargeBOD)bulkOrder));
							}
							else if (bulkOrder is SmallBOD)
							{
								m_From.CloseGump(typeof(SmallBODAcceptGump));
								m_From.SendGump(new SmallBODAcceptGump(m_From, (SmallBOD)bulkOrder));
							}
						}
						else
						{
							var ts = BulkOrderSystem.GetNextBulkOrder(m_Vendor.BODType, (PlayerMobile)m_From);

							var totalSeconds = (int)ts.TotalSeconds;
							var totalHours = (totalSeconds + 3599) / 3600;
							var totalMinutes = (totalSeconds + 59) / 60;

							m_Vendor.SayTo(m_From, 1072058, totalMinutes.ToString(), 0x3B2); // An offer may be available in about ~1_minutes~ minutes.
						}
					}
					else
					{
						var ts = m_Vendor.GetNextBulkOrder(m_From);

						var totalSeconds = (int)ts.TotalSeconds;
						var totalHours = (totalSeconds + 3599) / 3600;
						var totalMinutes = (totalSeconds + 59) / 60;

						if (totalMinutes == 0)
						{
							m_From.SendLocalizedMessage(1049038); // You can get an order now.

							var bulkOrder = m_Vendor.CreateBulkOrder(m_From, true);

							if (bulkOrder is LargeBOD)
							{
								m_From.CloseGump(typeof(LargeBODAcceptGump));
								m_From.SendGump(new LargeBODAcceptGump(m_From, (LargeBOD)bulkOrder));
							}
							else if (bulkOrder is SmallBOD)
							{
								m_From.CloseGump(typeof(SmallBODAcceptGump));
								m_From.SendGump(new SmallBODAcceptGump(m_From, (SmallBOD)bulkOrder));
							}
						}
						else
						{
							var oldSpeechHue = m_Vendor.SpeechHue;
							m_Vendor.SpeechHue = 0x3B2;

							m_Vendor.SayTo(m_From, 1072058, totalMinutes.ToString(), 0x3B2);
							// An offer may be available in about ~1_minutes~ minutes.

							m_Vendor.SpeechHue = oldSpeechHue;
						}
					}
				}
			}
		}

		private class BribeEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseVendor m_Vendor;

			public BribeEntry(Mobile from, BaseVendor vendor)
				: base(1152294, 2)
			{
				Enabled = vendor.CheckVendorAccess(from);

				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if (!m_From.InRange(m_Vendor.Location, 2) || !(m_From is PlayerMobile))
					return;

				if (m_Vendor.SupportsBulkOrders(m_From) && m_From is PlayerMobile)
				{
					if (m_From.NetState != null && m_From.NetState.IsEnhancedClient)
					{
						Timer.DelayCall(TimeSpan.FromMilliseconds(100), m_Vendor.TryBribe, m_From);
					}
					else
					{
						m_Vendor.TryBribe(m_From);
					}
				}
			}
		}

		private class ClaimRewardsEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseVendor m_Vendor;

			public ClaimRewardsEntry(Mobile from, BaseVendor vendor)
				: base(1155593, 3)
			{
				Enabled = vendor.CheckVendorAccess(from);

				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if (!m_From.InRange(m_Vendor.Location, 3) || !(m_From is PlayerMobile))
					return;

				var context = BulkOrderSystem.GetContext(m_From);
				var pending = context.GetPendingRewardFor(m_Vendor.BODType);

				if (pending > 0)
				{
					if (context.PointsMode == PointsMode.Enabled)
					{
						m_From.SendGump(new ConfirmBankPointsGump((PlayerMobile)m_From, m_Vendor, m_Vendor.BODType, pending, pending * 0.02));
					}
					else
					{
						m_From.SendGump(new RewardsGump(m_Vendor, (PlayerMobile)m_From, m_Vendor.BODType, pending));
					}
				}
				else if (!BulkOrderSystem.CanClaimRewards(m_From))
				{
					m_Vendor.SayTo(m_From, 1157083, 0x3B2); // You must claim your last turn-in reward in order for us to continue doing business.
				}
				else
				{
					m_From.SendGump(new RewardsGump(m_Vendor, (PlayerMobile)m_From, m_Vendor.BODType));
				}
			}
		}

		public BaseVendor(string title)
			: base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 5)
		{
			AllVendors.Add(this);

			LoadSBInfo();

			Title = title;

			InitBody();
			InitOutfit();

			Container pack;
			//these packs MUST exist, or the client will crash when the packets are sent
			pack = new Backpack
			{
				Layer = Layer.ShopBuy,
				Movable = false,
				Visible = false
			};
			AddItem(pack);

			pack = new Backpack
			{
				Layer = Layer.ShopResale,
				Movable = false,
				Visible = false
			};
			AddItem(pack);

			BribeMultiplier = Utility.Random(10);

			m_LastRestock = DateTime.UtcNow;
		}

		public BaseVendor(Serial serial)
			: base(serial)
		{
			AllVendors.Add(this);
		}

		public override void OnDelete()
		{
			base.OnDelete();

			AllVendors.Remove(this);
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			AllVendors.Remove(this);
		}

		public DateTime LastRestock { get => m_LastRestock; set => m_LastRestock = value; }

		public virtual TimeSpan RestockDelay => DelayRestock;

		public Container BuyPack
		{
			get
			{
				var pack = FindItemOnLayer(Layer.ShopBuy) as Container;

				if (pack == null)
				{
					pack = new Backpack
					{
						Layer = Layer.ShopBuy,
						Visible = false
					};
					AddItem(pack);
				}

				return pack;
			}
		}

		public abstract void InitSBInfo();

		public virtual bool IsTokunoVendor => (Map == Map.Tokuno);
		public virtual bool IsStygianVendor => (Map == Map.TerMur);

		protected void LoadSBInfo()
		{
			if (SBInfos == null)
			{
				return;
			}

			m_LastRestock = DateTime.UtcNow;

			for (var i = 0; i < m_ArmorBuyInfo.Count; ++i)
			{
				var buy = m_ArmorBuyInfo[i] as GenericBuyInfo;

				if (buy != null)
				{
					buy.DeleteDisplayEntity();
				}
			}

			SBInfos.Clear();

			InitSBInfo();

			m_ArmorBuyInfo.Clear();
			m_ArmorSellInfo.Clear();

			for (var i = 0; i < SBInfos.Count; i++)
			{
				var sbInfo = SBInfos[i];
				m_ArmorBuyInfo.AddRange(sbInfo.BuyInfo);
				m_ArmorSellInfo.Add(sbInfo.SellInfo);
			}
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats(100, 100, 25);

			SpeechHue = Utility.RandomDyedHue();
			Female = GetGender();
			Hue = Race.RandomSkinHue();
			Body = Race.Body(this);

			Name = NameList.RandomName(Female ? "female" : "male");
		}

		public virtual int GetRandomHue()
		{
			switch (Utility.Random(5))
			{
				default:
				case 0:
				return Utility.RandomBlueHue();
				case 1:
				return Utility.RandomGreenHue();
				case 2:
				return Utility.RandomRedHue();
				case 3:
				return Utility.RandomYellowHue();
				case 4:
				return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if (0.1 > Utility.RandomDouble())
			{
				return 0;
			}

			return Utility.RandomNeutralHue();
		}

		public virtual VendorShoeType ShoeType => VendorShoeType.Shoes;

		public virtual void CheckMorph()
		{
			if (!ChangeRace)
				return;

			if (CheckTerMur())
			{
				return;
			}
			else if (CheckNecromancer())
			{
				return;
			}
			else if (CheckTokuno())
			{
				return;
			}
		}

		public virtual bool CheckTokuno()
		{
			if (Map != Map.Tokuno)
			{
				return false;
			}

			NameList n;

			if (Female)
			{
				n = NameList.GetNameList("tokuno female");
			}
			else
			{
				n = NameList.GetNameList("tokuno male");
			}

			if (!n.ContainsName(Name))
			{
				TurnToTokuno();
			}

			return true;
		}

		public virtual void TurnToTokuno()
		{
			if (Female)
			{
				Name = NameList.RandomName("tokuno female");
			}
			else
			{
				Name = NameList.RandomName("tokuno male");
			}
		}

		#region SA Change
		public virtual bool CheckTerMur()
		{
			var map = Map;

			if (map != Map.TerMur || Spells.SpellHelper.IsEodon(map, Location))
				return false;

			if (Body != 0x29A && Body != 0x29B)
				TurnToGargRace();

			return true;
		}
		#endregion

		public virtual bool CheckNecromancer()
		{
			var map = Map;

			if (map != Map.Malas)
			{
				return false;
			}

			if (!Region.IsPartOf("Umbra"))
			{
				return false;
			}

			if (Hue != 0x83E8)
			{
				TurnToNecromancer();
			}

			return true;
		}

		public override void OnAfterSpawn()
		{
			CheckMorph();
		}

		protected override void OnMapChange(Map oldMap)
		{
			base.OnMapChange(oldMap);

			CheckMorph();

			LoadSBInfo();
		}

		public virtual int GetRandomNecromancerHue()
		{
			switch (Utility.Random(20))
			{
				case 0:
				return 0;
				case 1:
				return 0x4E9;
				default:
				return Utility.RandomList(0x485, 0x497);
			}
		}

		public virtual void TurnToNecromancer()
		{
			for (var i = 0; i < Items.Count; ++i)
			{
				var item = Items[i];

				if (item is Hair || item is Beard)
				{
					item.Hue = 0;
				}
				else if (item is BaseClothing || item is BaseWeapon || item is BaseArmor || item is BaseTool)
				{
					item.Hue = GetRandomNecromancerHue();
				}
			}

			HairHue = 0;
			FacialHairHue = 0;

			Hue = 0x83E8;
		}

		#region SA
		public virtual void TurnToGargRace()
		{
			for (var i = 0; i < Items.Count; ++i)
			{
				var item = Items[i];

				if (item is BaseClothing)
				{
					item.Delete();
				}
			}

			Race = Race.Gargoyle;

			Hue = Race.RandomSkinHue();

			HairItemID = Race.RandomHair(Female);
			HairHue = Race.RandomHairHue();

			FacialHairItemID = Race.RandomFacialHair(Female);
			if (FacialHairItemID != 0)
			{
				FacialHairHue = Race.RandomHairHue();
			}
			else
			{
				FacialHairHue = 0;
			}

			InitGargOutfit();

			if (Female = GetGender())
			{
				Body = 0x29B;
				Name = NameList.RandomName("gargoyle female");
			}
			else
			{
				Body = 0x29A;
				Name = NameList.RandomName("gargoyle male");
			}

			CapitalizeTitle();
		}
		#endregion

		public virtual void CapitalizeTitle()
		{
			var title = Title;

			if (title == null)
			{
				return;
			}

			var split = title.Split(' ');

			for (var i = 0; i < split.Length; ++i)
			{
				if (Insensitive.Equals(split[i], "the"))
				{
					continue;
				}

				if (split[i].Length > 1)
				{
					split[i] = Char.ToUpper(split[i][0]) + split[i].Substring(1);
				}
				else if (split[i].Length > 0)
				{
					split[i] = Char.ToUpper(split[i][0]).ToString();
				}
			}

			Title = String.Join(" ", split);
		}

		public virtual int GetHairHue()
		{
			return Race.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			if (Backpack == null)
			{
				Item backpack = new Backpack
				{
					Movable = false
				};
				AddItem(backpack);
			}

			switch (Utility.Random(3))
			{
				case 0:
				SetWearable(new FancyShirt(GetRandomHue()));
				break;
				case 1:
				SetWearable(new Doublet(GetRandomHue()));
				break;
				case 2:
				SetWearable(new Shirt(GetRandomHue()));
				break;
			}

			switch (ShoeType)
			{
				case VendorShoeType.Shoes:
				SetWearable(new Shoes(GetShoeHue()));
				break;
				case VendorShoeType.Boots:
				SetWearable(new Boots(GetShoeHue()));
				break;
				case VendorShoeType.Sandals:
				SetWearable(new Sandals(GetShoeHue()));
				break;
				case VendorShoeType.ThighBoots:
				SetWearable(new ThighBoots(GetShoeHue()));
				break;
			}

			var hairHue = GetHairHue();

			Utility.AssignRandomHair(this, hairHue);
			Utility.AssignRandomFacialHair(this, hairHue);

			if (Body == 0x191)
			{
				FacialHairItemID = 0;
			}

			if (Body == 0x191)
			{
				switch (Utility.Random(6))
				{
					case 0:
					SetWearable(new ShortPants(GetRandomHue()));
					break;
					case 1:
					case 2:
					SetWearable(new Kilt(GetRandomHue()));
					break;
					case 3:
					case 4:
					case 5:
					SetWearable(new Skirt(GetRandomHue()));
					break;
				}
			}
			else
			{
				switch (Utility.Random(2))
				{
					case 0:
					SetWearable(new LongPants(GetRandomHue()));
					break;
					case 1:
					SetWearable(new ShortPants(GetRandomHue()));
					break;
				}
			}
		}

		#region SA
		public virtual void InitGargOutfit()
		{
			for (var i = 0; i < Items.Count; ++i)
			{
				var item = Items[i];

				if (item is BaseClothing)
				{
					item.Delete();
				}
			}

			if (Female)
			{
				switch (Utility.Random(2))
				{
					case 0:
					SetWearable(new FemaleGargishClothLegs(GetRandomHue()));
					SetWearable(new FemaleGargishClothKilt(GetRandomHue()));
					SetWearable(new FemaleGargishClothChest(GetRandomHue()));
					break;
					case 1:
					SetWearable(new FemaleGargishClothKilt(GetRandomHue()));
					SetWearable(new FemaleGargishClothChest(GetRandomHue()));
					break;
				}
			}
			else
			{
				switch (Utility.Random(2))
				{
					case 0:
					SetWearable(new MaleGargishClothLegs(GetRandomHue()));
					SetWearable(new MaleGargishClothKilt(GetRandomHue()));
					SetWearable(new MaleGargishClothChest(GetRandomHue()));
					break;
					case 1:
					SetWearable(new MaleGargishClothKilt(GetRandomHue()));
					SetWearable(new MaleGargishClothChest(GetRandomHue()));
					break;
				}
			}
		}
		#endregion

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ForceRestock
		{
			get => false;
			set
			{
				if (value)
				{
					Restock();
					Say("Restocked!");
				}
			}
		}

		public virtual void Restock()
		{
			m_LastRestock = DateTime.UtcNow;

			var buyInfo = GetBuyInfo();

			foreach (var bii in buyInfo)
			{
				bii.OnRestock();
			}
		}

		private static readonly TimeSpan InventoryDecayTime = TimeSpan.FromHours(1.0);

		public virtual void VendorBuy(Mobile from)
		{
			if (!IsActiveSeller)
			{
				return;
			}

			if (!from.CheckAlive())
			{
				return;
			}

			if (!CheckVendorAccess(from))
			{
				Say(501522); // I shall not treat with scum like thee!
				return;
			}

			if (DateTime.UtcNow - m_LastRestock > RestockDelay)
			{
				Restock();
			}

			UpdateBuyInfo();

			var count = 0;

			var buyInfo = GetBuyInfo();
			var sellInfo = GetSellInfo();

			var list = new List<BuyItemState>(buyInfo.Length);

			var cont = BuyPack;

			List<ObjectPropertyList> opls = null;

			for (var idx = 0; idx < buyInfo.Length && list.Count < 250; idx++)
			{
				var buyItem = buyInfo[idx];

				if (buyItem.Amount <= 0)
				{
					continue;
				}

				if (Siege.SiegeShard && !Siege.VendorCanSell(buyItem.Type))
				{
					continue;
				}

				list.Add(new BuyItemState(cont, buyItem));

				count++;

				var disp = buyItem.GetDisplayEntity();

				if (disp != null)
				{
					if (opls == null)
					{
						opls = new List<ObjectPropertyList>();
					}

					if (disp is Item)
					{
						opls.Add(((Item)disp).PropertyList);
					}
					else if (disp is Mobile)
					{
						opls.Add(((Mobile)disp).PropertyList);
					}
				}
			}

			var playerItems = cont.Items;

			for (var i = playerItems.Count - 1; i >= 0; --i)
			{
				if (i >= playerItems.Count)
				{
					continue;
				}

				var item = playerItems[i];

				if ((item.LastMoved + InventoryDecayTime) <= DateTime.UtcNow)
				{
					item.Delete();
				}
			}

			for (var i = 0; i < playerItems.Count; ++i)
			{
				var item = playerItems[i];

				if (Siege.SiegeShard && !Siege.VendorCanSell(item.GetType()))
				{
					continue;
				}

				var price = 0;
				string name = null;

				foreach (var ssi in sellInfo)
				{
					if (ssi.IsSellable(item))
					{
						price = ssi.GetBuyPriceFor(item, this);
						name = ssi.GetNameFor(item);
						break;
					}
				}

				if (name != null && list.Count < 250)
				{
					list.Add(new BuyItemState(name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue));

					count++;

					if (opls == null)
					{
						opls = new List<ObjectPropertyList>();
					}

					opls.Add(item.PropertyList);
				}
			}

			if (list.Count > 0)
			{
				list.Sort(new BuyItemStateComparer());

				SendPacksTo(from);

				var ns = from.NetState;

				if (ns == null)
				{
					return;
				}

				VendorBuyContent.Send(ns, list);

				from.Send(new VendorBuyList(this, list));

				DisplayBuyList.Send(ns, this);

				MobileStatus.Send(ns, from); //make sure their gold amount is sent

				if (opls != null)
				{
					for (var i = 0; i < opls.Count; ++i)
					{
						from.Send(opls[i]);
					}
				}

				SayTo(from, 500186, 0x3B2); // Greetings.  Have a look around.
			}
		}

		public virtual void SendPacksTo(Mobile from)
		{
			var pack = FindItemOnLayer(Layer.ShopBuy);

			if (pack == null)
			{
				pack = new Backpack
				{
					Layer = Layer.ShopBuy,
					Movable = false,
					Visible = false
				};
				SetWearable(pack);
			}

			from.Send(new EquipUpdate(pack));

			pack = FindItemOnLayer(Layer.ShopSell);

			if (pack != null)
			{
				from.Send(new EquipUpdate(pack));
			}

			pack = FindItemOnLayer(Layer.ShopResale);

			if (pack == null)
			{
				pack = new Backpack
				{
					Layer = Layer.ShopResale,
					Movable = false,
					Visible = false
				};
				SetWearable(pack);
			}

			from.Send(new EquipUpdate(pack));
		}

		public virtual void VendorSell(Mobile from)
		{
			if (!IsActiveBuyer)
			{
				return;
			}

			if (!from.CheckAlive())
			{
				return;
			}

			if (!CheckVendorAccess(from))
			{
				Say(501522); // I shall not treat with scum like thee!
				return;
			}

			var pack = from.Backpack;

			if (pack != null)
			{
				var info = GetSellInfo();

				var table = new Dictionary<Item, SellItemState>();

				foreach (var ssi in info)
				{
					var items = pack.FindItemsByType(ssi.Types);

					foreach (var item in items)
					{
						if (item is Container && (item).Items.Count != 0)
						{
							continue;
						}

						var lockable = item.Parent as LockableContainer;

						if (lockable != null && lockable.Locked)
						{
							continue;
						}

						if (item.IsStandardLoot() && item.Movable && ssi.IsSellable(item))
						{
							table[item] = new SellItemState(item, ssi.GetSellPriceFor(item, this), ssi.GetNameFor(item));
						}
					}
				}

				if (table.Count > 0)
				{
					SendPacksTo(from);

					from.Send(new VendorSellList(this, table.Values));
				}
				else
				{
					Say(true, "You have nothing I would be interested in.");
				}
			}
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			#region Honesty Item Check
			var honestySocket = dropped.GetSocket<HonestyItemSocket>();

			if (honestySocket != null)
			{
				var gainedPath = false;

				if (honestySocket.HonestyOwner == this)
				{
					VirtueHelper.Award(from, VirtueName.Honesty, 120, ref gainedPath);
					from.SendMessage(gainedPath ? "You have gained a path in Honesty!" : "You have gained in Honesty.");
					SayTo(from, 1074582); //Ah!  You found my property.  Thank you for your honesty in returning it to me.
					dropped.Delete();
					return true;
				}
				else
				{
					SayTo(from, 501550, 0x3B2); // I am not interested in this.
					return false;
				}
			}
			#endregion

			if (ConvertsMageArmor && dropped is BaseArmor && CheckConvertArmor(from, (BaseArmor)dropped))
			{
				return false;
			}

			if (dropped is SmallBOD || dropped is LargeBOD)
			{
				var pm = from as PlayerMobile;
				var bod = dropped as IBOD;

				if (bod != null && BulkOrderSystem.NewSystemEnabled && Bribes != null && Bribes.ContainsKey(from) && Bribes[from].BOD == bod)
				{
					if (BulkOrderSystem.CanExchangeBOD(from, this, bod, Bribes[from].Amount))
					{
						DoBribe(from, bod);
						return false;
					}
				}

				if (pm != null && pm.NextBODTurnInTime > DateTime.UtcNow)
				{
					SayTo(from, 1079976, 0x3B2); // You'll have to wait a few seconds while I inspect the last order.
					return false;
				}
				else if (!IsValidBulkOrder(dropped) || !SupportsBulkOrders(from))
				{
					SayTo(from, 1045130, 0x3B2); // That order is for some other shopkeeper.
					return false;
				}
				else if (!BulkOrderSystem.CanClaimRewards(from))
				{
					SayTo(from, 1157083, 0x3B2); // You must claim your last turn-in reward in order for us to continue doing business.
					return false;
				}
				else if (bod == null || !bod.Complete)
				{
					SayTo(from, 1045131, 0x3B2); // You have not completed the order yet.
					return false;
				}

				Item reward;
				int gold, fame;

				if (dropped is SmallBOD)
				{
					((SmallBOD)dropped).GetRewards(out reward, out gold, out fame);
				}
				else
				{
					((LargeBOD)dropped).GetRewards(out reward, out gold, out fame);
				}

				from.SendSound(0x3D);

				if (BulkOrderSystem.NewSystemEnabled && from is PlayerMobile)
				{
					SayTo(from, 1157204, from.Name, 0x3B2); // Ho! Ho! Thank ye ~1_PLAYER~ for giving me a Bulk Order Deed!

					var context = BulkOrderSystem.GetContext(from);

					var points = 0;
					var banked = 0.0;

					if (dropped is SmallBOD)
						BulkOrderSystem.ComputePoints((SmallBOD)dropped, out points, out banked);
					else
						BulkOrderSystem.ComputePoints((LargeBOD)dropped, out points, out banked);

					switch (context.PointsMode)
					{
						case PointsMode.Enabled:
						context.AddPending(BODType, points);
						from.SendGump(new ConfirmBankPointsGump((PlayerMobile)from, this, BODType, points, banked));
						break;
						case PointsMode.Disabled:
						context.AddPending(BODType, points);
						from.SendGump(new RewardsGump(this, (PlayerMobile)from, BODType, points));
						break;
						case PointsMode.Automatic:
						BulkOrderSystem.SetPoints(from, BODType, banked);
						from.SendGump(new RewardsGump(this, (PlayerMobile)from, BODType));
						break;
					}

					// On EA, you have to choose the reward before you get the gold/fame reward.  IF you right click the gump, you lose 
					// the gold/fame for that bod.

					Banker.Deposit(from, gold, true);
				}
				else
				{
					SayTo(from, 1045132, 0x3B2); // Thank you so much!  Here is a reward for your effort.

					if (reward != null)
					{
						from.AddToBackpack(reward);
					}

					Banker.Deposit(from, gold, true);
				}

				Titles.AwardFame(from, fame, true);

				OnSuccessfulBulkOrderReceive(from);
				Engines.CityLoyalty.CityLoyaltySystem.OnBODTurnIn(from, gold);

				if (pm != null)
				{
					pm.NextBODTurnInTime = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);
				}

				dropped.Delete();
				return true;
			}
			else if (AcceptsGift(from, dropped))
			{
				dropped.Delete();
			}

			return base.OnDragDrop(from, dropped);
		}

		public bool AcceptsGift(Mobile from, Item dropped)
		{
			string name;

			if (dropped.Name != null)
			{
				if (dropped.Amount > 0)
				{
					name = String.Format("{0} {1}", dropped.Amount, dropped.Name);
				}
				else
				{
					name = dropped.Name;
				}
			}
			else
			{
				name = Engines.VendorSearching.VendorSearch.GetItemName(dropped);
			}

			if (!String.IsNullOrEmpty(name))
			{
				PrivateOverheadMessage(MessageType.Regular, 0x3B2, true, String.Format("Thou art giving me {0}.", name), from.NetState);
			}
			else
			{
				SayTo(from, 1071971, String.Format("#{0}", dropped.LabelNumber.ToString()), 0x3B2); // Thou art giving me ~1_VAL~?
			}

			if (dropped is Gold)
			{
				SayTo(from, 501548, 0x3B2); // I thank thee.
				Titles.AwardFame(from, dropped.Amount / 100, true);

				return true;
			}

			var info = GetSellInfo();

			foreach (var ssi in info)
			{
				if (ssi.IsSellable(dropped))
				{
					SayTo(from, 501548, 0x3B2); // I thank thee.
					Titles.AwardFame(from, ssi.GetSellPriceFor(dropped, this) * dropped.Amount, true);

					return true;
				}
			}

			SayTo(from, 501550, 0x3B2); // I am not interested in this.

			return false;
		}

		#region BOD Bribing

		[ConfigProperty("Vendors.BribeDecayMinTime")]
		public static int BribeDecayMin { get => Config.Get("Vendors.BribeDecayMinTime", 25); set => Config.Set("Vendors.BribeDecayMinTime", value); }

		[ConfigProperty("Vendors.BribeDecayMaxTime")]
		public static int BribeDecayMax { get => Config.Get("Vendors.BribeDecayMaxTime", 30); set => Config.Set("Vendors.BribeDecayMaxTime", value); }

		[CommandProperty(AccessLevel.GameMaster)]
		public int BribeMultiplier { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextMultiplierDecay { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime WatchEnds { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int RecentBribes { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool UnderWatch => WatchEnds > DateTime.MinValue;

		public Dictionary<Mobile, PendingBribe> Bribes { get; set; }

		private void CheckNextMultiplierDecay(bool force = true)
		{
			var minDays = BribeDecayMin;
			var maxDays = BribeDecayMax;

			if (force || (NextMultiplierDecay > DateTime.UtcNow + TimeSpan.FromDays(maxDays)))
				NextMultiplierDecay = DateTime.UtcNow + TimeSpan.FromDays(Utility.RandomMinMax(minDays, maxDays));
		}

		public void TryBribe(Mobile m)
		{
			if (UnderWatch)
			{
				if (WatchEnds < DateTime.UtcNow)
				{
					WatchEnds = DateTime.MinValue;
					RecentBribes = 0;
				}
				else
				{
					SayTo(m, 1152293, 0x3B2); // My business is being watched by the Guild, so I can't be messing with bulk orders right now. Come back when there's less heat on me!
					return;
				}
			}

			SayTo(m, 1152295, 0x3B2); // So you want to do a little business under the table?
			m.SendLocalizedMessage(1152296); // Target a bulk order deed to show to the shopkeeper.

			m.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
			{
				var bod = targeted as IBOD;

				if (bod is Item && ((Item)bod).IsChildOf(from.Backpack))
				{
					if (BulkOrderSystem.CanExchangeBOD(from, this, bod, -1))
					{
						var amount = BulkOrderSystem.GetBribe(bod);
						amount *= BribeMultiplier;

						if (Bribes == null)
							Bribes = new Dictionary<Mobile, PendingBribe>();

						// Per EA, new bribe replaced old pending bribe
						if (!Bribes.ContainsKey(m))
						{
							Bribes[m] = new PendingBribe(bod, amount);
						}
						else
						{
							Bribes[m].BOD = bod;
							Bribes[m].Amount = amount;
						}

						SayTo(from, 1152292, amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")), 0x3B2);
						// If you help me out, I'll help you out. I can replace that bulk order with a better one, but it's gonna cost you ~1_amt~ gold coin. Payment is due immediately. Just hand me the order and I'll pull the old switcheroo.
					}
				}
				else if (bod == null)
				{
					SayTo(from, 1152297, 0x3B2); // That is not a bulk order deed.
				}
			});
		}

		public void DoBribe(Mobile m, IBOD bod)
		{
			BulkOrderSystem.MutateBOD(bod);

			RecentBribes++;

			if (RecentBribes >= 3 && Utility.Random(6) < RecentBribes)
			{
				WatchEnds = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(120, 180));
			}

			SayTo(m, 1152303, 0x3B2); // You'll find this one much more to your liking. It's been a pleasure, and I look forward to you greasing my palm again very soon.

			if (Bribes.ContainsKey(m))
			{
				Bribes.Remove(m);
			}

			BribeMultiplier++;
			CheckNextMultiplierDecay();
		}

		#endregion

		private IBuyItemInfo LookupDisplayObject(object obj)
		{
			var buyInfo = GetBuyInfo();

			for (var i = 0; i < buyInfo.Length; ++i)
			{
				if (buyInfo[i].GetDisplayEntity() == obj)
				{
					return buyInfo[i];
				}
			}

			return null;
		}

		private void ProcessSinglePurchase(BuyItemResponse buy, IBuyItemInfo bii, List<BuyItemResponse> validBuy, ref int controlSlots, ref bool fullPurchase, ref double cost)
		{
			var amount = buy.Amount;

			if (amount > bii.Amount)
			{
				amount = bii.Amount;
			}

			if (amount <= 0)
			{
				return;
			}

			var slots = bii.ControlSlots * amount;

			if (controlSlots >= slots)
			{
				controlSlots -= slots;
			}
			else
			{
				fullPurchase = false;
				return;
			}

			cost = (double)bii.Price * amount;

			validBuy.Add(buy);
		}

		private void ProcessValidPurchase(int amount, IBuyItemInfo bii, Mobile buyer, Container cont)
		{
			if (amount > bii.Amount)
			{
				amount = bii.Amount;
			}

			if (amount < 1)
			{
				return;
			}

			bii.Amount -= amount;

			var o = bii.GetEntity();

			if (o is Item item)
			{
				if (item.Stackable)
				{
					item.Amount = amount;

					if (cont == null || !cont.TryDropItem(buyer, item, false))
					{
						item.MoveToWorld(buyer.Location, buyer.Map);
					}

					bii.OnBought(buyer, this, item, amount);
				}
				else
				{
					item.Amount = 1;

					if (cont == null || !cont.TryDropItem(buyer, item, false))
					{
						item.MoveToWorld(buyer.Location, buyer.Map);
					}

					bii.OnBought(buyer, this, item, 1);

					for (var i = 1; i < amount; i++)
					{
						item = bii.GetEntity() as Item;

						if (item != null)
						{
							item.Amount = 1;

							if (cont == null || !cont.TryDropItem(buyer, item, false))
							{
								item.MoveToWorld(buyer.Location, buyer.Map);
							}

							bii.OnBought(buyer, this, item, 1);
						}
					}
				}
			}
			else if (o is Mobile m)
			{
				m.Direction = (Direction)Utility.Random(8);
				m.MoveToWorld(buyer.Location, buyer.Map);
				m.PlaySound(m.GetIdleSound());

				if (m is BaseCreature)
				{
					((BaseCreature)m).SetControlMaster(buyer);
				}

				bii.OnBought(buyer, this, m, 1);

				for (var i = 1; i < amount; ++i)
				{
					m = bii.GetEntity() as Mobile;

					if (m != null)
					{
						m.Direction = (Direction)Utility.Random(8);
						m.MoveToWorld(buyer.Location, buyer.Map);

						if (m is BaseCreature)
						{
							((BaseCreature)m).SetControlMaster(buyer);
						}

						bii.OnBought(buyer, this, m, 1);
					}
				}
			}
			else if (o is ISpawnable s)
			{
				s.MoveToWorld(buyer.Location, buyer.Map);

				bii.OnBought(buyer, this, s, 1);

				for (var i = 1; i < amount; ++i)
				{
					s = bii.GetEntity() as ISpawnable;

					if (s != null)
					{
						s.MoveToWorld(buyer.Location, buyer.Map);

						bii.OnBought(buyer, this, s, 1);
					}
				}
			}
		}

		public virtual bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
		{
			if (!IsActiveSeller)
			{
				return false;
			}

			if (!buyer.CheckAlive())
			{
				return false;
			}

			if (!CheckVendorAccess(buyer))
			{
				Say(501522); // I shall not treat with scum like thee!
				return false;
			}

			UpdateBuyInfo();

			//var buyInfo = GetBuyInfo();
			var info = GetSellInfo();
			var totalCost = 0.0;
			var validBuy = new List<BuyItemResponse>(list.Count);
			Container cont;
			bool bought;
			var fromBank = false;
			var fullPurchase = true;
			var controlSlots = buyer.FollowersMax - buyer.Followers;

			foreach (var buy in list)
			{
				var ser = buy.Serial;
				var amount = buy.Amount;
				double cost = 0;

				if (ser.IsItem)
				{
					var item = World.FindItem(ser);

					if (item == null)
					{
						continue;
					}

					var bii = LookupDisplayObject(item);

					if (bii != null)
					{
						ProcessSinglePurchase(buy, bii, validBuy, ref controlSlots, ref fullPurchase, ref cost);
					}
					else if (item != BuyPack && item.IsChildOf(BuyPack))
					{
						if (amount > item.Amount)
						{
							amount = item.Amount;
						}

						if (amount <= 0)
						{
							continue;
						}

						foreach (var ssi in info)
						{
							if (ssi.IsSellable(item))
							{
								if (ssi.IsResellable(item))
								{
									cost = (double)ssi.GetBuyPriceFor(item, this) * amount;
									validBuy.Add(buy);
									break;
								}
							}
						}
					}

					if (validBuy.Contains(buy))
					{
						if (ValidateBought(buyer, item))
						{
							totalCost += cost;
						}
						else
						{
							validBuy.Remove(buy);
						}
					}
				}
				else if (ser.IsMobile)
				{
					var mob = World.FindMobile(ser);

					if (mob == null)
					{
						continue;
					}

					var bii = LookupDisplayObject(mob);

					if (bii != null)
					{
						ProcessSinglePurchase(buy, bii, validBuy, ref controlSlots, ref fullPurchase, ref cost);
					}

					if (validBuy.Contains(buy))
					{
						if (ValidateBought(buyer, mob))
						{
							totalCost += cost;
						}
						else
						{
							validBuy.Remove(buy);
						}
					}
				}
			} //foreach

			if (fullPurchase && validBuy.Count == 0)
			{
				SayTo(buyer, 500190, 0x3B2); // Thou hast bought nothing!
			}
			else if (validBuy.Count == 0)
			{
				SayTo(buyer, 500187, 0x3B2); // Your order cannot be fulfilled, please try again.
			}

			if (validBuy.Count == 0)
			{
				return false;
			}

			bought = buyer.AccessLevel >= AccessLevel.GameMaster;
			cont = buyer.Backpack;

			var discount = 0.0;

			if (HasHonestyDiscount)
			{
				double discountPc;

				switch (VirtueHelper.GetLevel(buyer, VirtueName.Honesty))
				{
					case VirtueLevel.Seeker:
					discountPc = .1;
					break;
					case VirtueLevel.Follower:
					discountPc = .2;
					break;
					case VirtueLevel.Knight:
					discountPc = .3; break;
					default:
					discountPc = 0;
					break;
				}

				discount = totalCost - (totalCost * (1.0 - discountPc));
				totalCost -= discount;
			}

			if (!bought && cont != null && ConsumeGold(cont, totalCost))
			{
				bought = true;
			}

			if (!bought)
			{
				if (totalCost <= Int32.MaxValue)
				{
					if (Banker.Withdraw(buyer, (int)totalCost))
					{
						bought = true;
						fromBank = true;
					}
				}
				else if (buyer.Account != null && AccountGold.Enabled)
				{
					if (buyer.Account.WithdrawCurrency(totalCost / AccountGold.CurrencyThreshold))
					{
						bought = true;
						fromBank = true;
					}
				}
			}

			if (!bought)
			{
				cont = buyer.FindBankNoCreate();

				if (cont != null && ConsumeGold(cont, totalCost))
				{
					bought = true;
					fromBank = true;
				}
			}

			if (!bought)
			{
				// ? Begging thy pardon, but thy bank account lacks these funds. 
				// : Begging thy pardon, but thou casnt afford that.
				SayTo(buyer, totalCost >= 2000 ? 500191 : 500192, 0x3B2);

				return false;
			}

			buyer.PlaySound(0x32);

			cont = buyer.Backpack ?? buyer.BankBox;

			foreach (var buy in validBuy)
			{
				var ser = buy.Serial;
				var amount = buy.Amount;

				if (amount < 1)
				{
					continue;
				}

				if (ser.IsItem)
				{
					var item = World.FindItem(ser);

					if (item == null)
					{
						continue;
					}

					var bii = LookupDisplayObject(item);

					if (bii != null)
					{
						ProcessValidPurchase(amount, bii, buyer, cont);
					}
					else
					{
						if (amount > item.Amount)
						{
							amount = item.Amount;
						}

						foreach (var ssi in info)
						{
							if (ssi.IsSellable(item) && ssi.IsResellable(item))
							{
								Item buyItem;

								if (amount >= item.Amount)
								{
									buyItem = item;
								}
								else
								{
									buyItem = LiftItemDupe(item, item.Amount - amount);

									if (buyItem == null)
									{
										buyItem = item;
									}
								}

								if (cont == null || !cont.TryDropItem(buyer, buyItem, false))
								{
									buyItem.MoveToWorld(buyer.Location, buyer.Map);
								}

								break;
							}
						}
					}
				}
				else if (ser.IsMobile)
				{
					var mob = World.FindMobile(ser);

					if (mob == null)
					{
						continue;
					}

					var bii = LookupDisplayObject(mob);

					if (bii != null)
					{
						ProcessValidPurchase(amount, bii, buyer, cont);
					}
				}
			} //foreach

			if (discount > 0)
			{
				SayTo(buyer, 1151517, discount.ToString(), 0x3B2);
			}

			if (fullPurchase)
			{
				if (buyer.AccessLevel >= AccessLevel.GameMaster)
				{
					SayTo(buyer, 0x3B2, "I would not presume to charge thee anything.  Here are the goods you requested.", null, true);
				}
				else if (fromBank)
				{
					SayTo(buyer, 0x3B2, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost.ToString(), true);
				}
				else
				{
					SayTo(buyer, String.Format("The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost), 0x3B2, true);
				}
			}
			else
			{
				if (buyer.AccessLevel >= AccessLevel.GameMaster)
				{
					SayTo(buyer, 0x3B2, "I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested.", null, true);
				}
				else if (fromBank)
				{
					SayTo(buyer, 0x3B2, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost.ToString(), true);
				}
				else
				{
					SayTo(buyer, 0x3B2, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost.ToString(), true);
				}
			}

			return true;
		}

		public virtual bool ValidateBought(Mobile buyer, Item item)
		{
			return true;
		}

		public virtual bool ValidateBought(Mobile buyer, Mobile m)
		{
			return true;
		}

		public static bool ConsumeGold(Container cont, double amount)
		{
			return ConsumeGold(cont, amount, true);
		}

		public static bool ConsumeGold(Container cont, double amount, bool recurse)
		{
			var gold = new Queue<Gold>(FindGold(cont, recurse));
			var total = gold.Aggregate(0.0, (c, g) => c + g.Amount);

			if (total < amount)
			{
				gold.Clear();

				return false;
			}

			var consume = amount;

			while (consume > 0)
			{
				var g = gold.Dequeue();

				if (g.Amount > consume)
				{
					g.Consume((int)consume);

					consume = 0;
				}
				else
				{
					consume -= g.Amount;

					g.Delete();
				}
			}

			gold.Clear();

			return true;
		}

		private static IEnumerable<Gold> FindGold(Container cont, bool recurse)
		{
			if (cont == null || cont.Items.Count == 0)
			{
				yield break;
			}

			if (cont is ILockable && ((ILockable)cont).Locked)
			{
				yield break;
			}

			if (cont is TrapableContainer && ((TrapableContainer)cont).TrapType != TrapType.None)
			{
				yield break;
			}

			var count = cont.Items.Count;

			while (--count >= 0)
			{
				if (count >= cont.Items.Count)
				{
					continue;
				}

				var item = cont.Items[count];

				if (item is Container)
				{
					if (!recurse)
					{
						continue;
					}

					foreach (var gold in FindGold((Container)item, true))
					{
						yield return gold;
					}
				}
				else if (item is Gold)
				{
					yield return (Gold)item;
				}
			}
		}

		public virtual bool CheckVendorAccess(Mobile from)
		{
			var reg = (GuardedRegion)Region.GetRegion(typeof(GuardedRegion));

			if (reg != null && !reg.CheckVendorAccess(this, from))
			{
				return false;
			}

			if (Region != from.Region)
			{
				reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

				if (reg != null && !reg.CheckVendorAccess(this, from))
				{
					return false;
				}
			}

			return true;
		}

		public virtual bool OnSellItems(Mobile seller, List<SellItemResponse> list)
		{
			if (!IsActiveBuyer)
			{
				return false;
			}

			if (!seller.CheckAlive())
			{
				return false;
			}

			if (!CheckVendorAccess(seller))
			{
				Say(501522); // I shall not treat with scum like thee!
				return false;
			}

			seller.PlaySound(0x32);

			var info = GetSellInfo();
			var buyInfo = GetBuyInfo();
			var giveGold = 0;
			var sold = 0;
			Container cont;

			foreach (var resp in list)
			{
				if (resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || (resp.Item is Container && resp.Item.Items.Count != 0))
				{
					continue;
				}

				foreach (var ssi in info)
				{
					if (ssi.IsSellable(resp.Item))
					{
						sold++;
						break;
					}
				}
			}

			if (sold > MaxSell)
			{
				SayTo(seller, "You may only sell {0} items at a time!", MaxSell, 0x3B2, true);
				return false;
			}

			if (sold == 0)
			{
				return true;
			}

			foreach (var resp in list)
			{
				if (resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || (resp.Item is Container && resp.Item.Items.Count != 0))
				{
					continue;
				}

				foreach (var ssi in info)
				{
					if (ssi.IsSellable(resp.Item))
					{
						var dropItem = resp.Item;

						Container dropCont = null;

						var amount = resp.Amount;

						if (amount > dropItem.Amount)
						{
							amount = dropItem.Amount;
						}

						if (ssi.IsResellable(dropItem))
						{
							var found = false;

							foreach (var bii in buyInfo)
							{
								if (bii.Restock(dropItem, amount))
								{
									found = true;
									break;
								}
							}

							if (!found)
							{
								dropCont = cont = BuyPack;

								if (amount < dropItem.Amount)
								{
									var item = LiftItemDupe(dropItem, dropItem.Amount - amount);

									if (item != null)
									{
										dropItem = item;
									}
								}

								dropItem.SetLastMoved();
							}
						}

						var singlePrice = ssi.GetSellPriceFor(dropItem, this);

						giveGold += singlePrice * amount;

						ssi.OnSold(seller, this, dropItem, amount);

						if (dropCont != null)
						{
							dropCont.DropItem(dropItem);
						}
						else
						{
							dropItem.Consume(amount);
						}

						break;
					}
				}
			}

			if (giveGold > 0)
			{
				while (giveGold > 60000)
				{
					seller.AddToBackpack(new Gold(60000));
					giveGold -= 60000;
				}

				seller.AddToBackpack(new Gold(giveGold));

				seller.PlaySound(0x0037); //Gold dropping sound

				if (SupportsBulkOrders(seller))
				{
					var bulkOrder = CreateBulkOrder(seller, false);

					if (bulkOrder is LargeBOD)
					{
						seller.SendGump(new LargeBODAcceptGump(seller, (LargeBOD)bulkOrder));
					}
					else if (bulkOrder is SmallBOD)
					{
						seller.SendGump(new SmallBODAcceptGump(seller, (SmallBOD)bulkOrder));
					}
				}
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(4); // version

			writer.Write(BribeMultiplier);
			writer.Write(NextMultiplierDecay);
			writer.Write(RecentBribes);

			var sbInfos = SBInfos;

			for (var i = 0; sbInfos != null && i < sbInfos.Count; ++i)
			{
				var sbInfo = sbInfos[i];
				var buyInfo = sbInfo.BuyInfo;

				for (var j = 0; buyInfo != null && j < buyInfo.Count; ++j)
				{
					var bii = buyInfo[j];

					var maxAmount = bii.MaxAmount;
					var doubled = 0;

					switch (maxAmount)
					{
						case 40: doubled = 1; break;
						case 80: doubled = 2; break;
						case 160: doubled = 3; break;
						case 320: doubled = 4; break;
						case 640: doubled = 5; break;
						case 999: doubled = 6; break;
					}

					if (doubled > 0)
					{
						writer.WriteEncodedInt(1 + (j * sbInfos.Count) + i);
						writer.WriteEncodedInt(doubled);
					}
				}
			}

			writer.WriteEncodedInt(0);

			if (NextMultiplierDecay != DateTime.MinValue && NextMultiplierDecay < DateTime.UtcNow)
			{
				Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
				{
					if (BribeMultiplier > 0)
						BribeMultiplier /= 2;

					CheckNextMultiplierDecay();
				});
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			LoadSBInfo();

			var sbInfos = SBInfos;

			switch (version)
			{
				case 4:
				case 3:
				case 2:
					{
						BribeMultiplier = reader.ReadInt();
						NextMultiplierDecay = reader.ReadDateTime();

						CheckNextMultiplierDecay(false); // Reset NextMultiplierDecay if it is out of range of the config

						RecentBribes = reader.ReadInt();

						goto case 1;
					}
				case 1:
					{
						int index;

						while ((index = reader.ReadEncodedInt()) > 0)
						{
							var doubled = reader.ReadEncodedInt();

							if (version == 3)
							{
								reader.ReadEncodedInt();
								reader.ReadEncodedInt();
							}

							if (sbInfos != null && sbInfos.Count > 0)
							{
								index -= 1;

								var sbInfoIndex = index % sbInfos.Count;
								var buyInfoIndex = index / sbInfos.Count;

								if (sbInfoIndex >= 0 && sbInfoIndex < sbInfos.Count)
								{
									var sbInfo = sbInfos[sbInfoIndex];
									var buyInfo = sbInfo.BuyInfo;

									if (buyInfo != null && buyInfoIndex >= 0 && buyInfoIndex < buyInfo.Count)
									{
										var bii = buyInfo[buyInfoIndex];

										var amount = 20;

										switch (doubled)
										{
											case 0: break;
											case 1: amount = 40; break;
											case 2: amount = 80; break;
											case 3: amount = 160; break;
											case 4: amount = 320; break;
											case 5: amount = 640; break;
											case 6: amount = 999; break;
										}

										if (version == 2 && bii.Stackable)
										{
											bii.Amount = bii.MaxAmount = EconomyStockAmount;
										}
										else
										{
											bii.Amount = bii.MaxAmount = amount;
										}

										bii.TotalBought = 0;
										bii.TotalSold = 0;
									}
								}
							}
						}

						break;
					}
			}

			if (IsParagon)
			{
				IsParagon = false;
			}

			if (version == 1)
			{
				BribeMultiplier = Utility.Random(10);
			}

			Timer.DelayCall(TimeSpan.Zero, CheckMorph);
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (ConvertsMageArmor)
			{
				list.Add(new UpgradeMageArmor(from, this));
			}

			if (from.Alive && IsActiveVendor)
			{
				if (SupportsBulkOrders(from))
				{
					list.Add(new BulkOrderInfoEntry(from, this));

					if (BulkOrderSystem.NewSystemEnabled)
					{
						list.Add(new BribeEntry(from, this));
						list.Add(new ClaimRewardsEntry(from, this));
					}
				}

				if (IsActiveSeller)
				{
					list.Add(new VendorBuyEntry(from, this));
				}

				if (IsActiveBuyer)
				{
					list.Add(new VendorSellEntry(from, this));
				}
			}

			base.AddCustomContextEntries(from, list);
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return (IShopSellInfo[])m_ArmorSellInfo.ToArray(typeof(IShopSellInfo));
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray(typeof(IBuyItemInfo));
		}

		#region Mage Armor Conversion
		public virtual bool ConvertsMageArmor => false;

		private readonly List<PendingConvert> _PendingConvertEntries = new List<PendingConvert>();

		private bool CheckConvertArmor(Mobile from, BaseArmor armor)
		{
			var convert = GetConvert(from, armor);

			if (convert == null || !(from is PlayerMobile))
				return false;

			object state = convert.Armor;

			RemoveConvertEntry(convert);
			from.CloseGump(typeof(Gumps.ConfirmCallbackGump));

			from.SendGump(new Gumps.ConfirmCallbackGump((PlayerMobile)from, 1049004, 1154115, state, null,
				(m, obj) =>
				{
					var ar = obj as BaseArmor;

					if (!Deleted && ar != null && armor.IsChildOf(m.Backpack) && CanConvertArmor(m, ar))
					{
						if (!InRange(m.Location, 3))
						{
							m.SendLocalizedMessage(1149654); // You are too far away.
						}
						else if (!Banker.Withdraw(m, 250000, true))
						{
							m.SendLocalizedMessage(1019022); // You do not have enough gold.
						}
						else
						{
							ConvertMageArmor(m, ar);
						}
					}
				},
				(m, obj) =>
				{
					var con = GetConvert(m, armor);

					if (con != null)
					{
						RemoveConvertEntry(con);
					}
				}));

			return true;
		}

		protected virtual bool CanConvertArmor(Mobile from, BaseArmor armor)
		{
			if (armor == null || armor is BaseShield/*|| armor.ArtifactRarity != 0 || armor.IsArtifact*/)
			{
				from.SendLocalizedMessage(1113044); // You can't convert that.
				return false;
			}

			if (armor.ArmorAttributes.MageArmor == 0 &&
				SkillHandlers.Imbuing.GetTotalMods(armor) > 4)
			{
				from.SendLocalizedMessage(1154119); // This action would exceed a stat cap
				return false;
			}

			return true;
		}

		public void TryConvertArmor(Mobile from, BaseArmor armor)
		{
			if (CanConvertArmor(from, armor))
			{
				from.SendLocalizedMessage(1154117); // Ah yes, I will convert this piece of armor but it's gonna cost you 250,000 gold coin. Payment is due immediately. Just hand me the armor.

				var convert = GetConvert(from, armor);

				if (convert != null)
				{
					convert.ResetTimer();
				}
				else
				{
					_PendingConvertEntries.Add(new PendingConvert(from, armor, this));
				}
			}
		}

		public virtual void ConvertMageArmor(Mobile from, BaseArmor armor)
		{
			if (armor.ArmorAttributes.MageArmor > 0)
				armor.ArmorAttributes.MageArmor = 0;
			else
				armor.ArmorAttributes.MageArmor = 1;

			from.SendLocalizedMessage(1154118); // Your armor has been converted.
		}

		private void RemoveConvertEntry(PendingConvert convert)
		{
			_PendingConvertEntries.Remove(convert);

			if (convert.Timer != null)
			{
				convert.Timer.Stop();
			}
		}

		private PendingConvert GetConvert(Mobile from, BaseArmor armor)
		{
			return _PendingConvertEntries.FirstOrDefault(c => c.From == from && c.Armor == armor);
		}

		protected class PendingConvert
		{
			public Mobile From { get; set; }
			public BaseArmor Armor { get; set; }
			public BaseVendor Vendor { get; set; }

			public Timer Timer { get; set; }
			public DateTime Expires { get; set; }

			public bool Expired => DateTime.UtcNow > Expires;

			public PendingConvert(Mobile from, BaseArmor armor, BaseVendor vendor)
			{
				From = from;
				Armor = armor;
				Vendor = vendor;

				ResetTimer();
			}

			public void ResetTimer()
			{
				if (Timer != null)
				{
					Timer.Stop();
					Timer = null;
				}

				Expires = DateTime.UtcNow + TimeSpan.FromSeconds(120);

				Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
				Timer.Start();
			}

			public void OnTick()
			{
				if (Expired)
				{
					Vendor.RemoveConvertEntry(this);
				}
			}
		}
		#endregion
	}
}

namespace Server.ContextMenus
{
	public class VendorBuyEntry : ContextMenuEntry
	{
		private readonly BaseVendor m_Vendor;

		public VendorBuyEntry(Mobile from, BaseVendor vendor)
			: base(6103, 8)
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess(from);
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy(Owner.From);
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private readonly BaseVendor m_Vendor;

		public VendorSellEntry(Mobile from, BaseVendor vendor)
			: base(6104, 8)
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess(from);
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell(Owner.From);
		}
	}

	public class UpgradeMageArmor : ContextMenuEntry
	{
		public Mobile From { get; set; }
		public BaseVendor Vendor { get; set; }

		public UpgradeMageArmor(Mobile from, BaseVendor vendor)
			: base(1154114) // Convert Mage Armor
		{
			Enabled = vendor.CheckVendorAccess(from);

			From = from;
			Vendor = vendor;
		}

		public override void OnClick()
		{
			From.Target = new InternalTarget(From, Vendor);
			From.SendLocalizedMessage(1154116); // Target a piece of armor to show to the guild master.
		}

		private class InternalTarget : Target
		{
			public Mobile From { get; set; }
			public BaseVendor Vendor { get; set; }

			public InternalTarget(Mobile from, BaseVendor vendor)
				: base(1, false, TargetFlags.None)
			{
				From = from;
				Vendor = vendor;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is BaseArmor)
				{
					var armor = (BaseArmor)targeted;
					Vendor.TryConvertArmor(from, armor);
				}
			}
		}
	}
}
