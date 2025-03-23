#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Linq;

using Server;
using Server.Engines.Craft;
using Server.Items;

using VitaNex.SuperCrafts;
#endregion

namespace VitaNex.Items
{
	public abstract class BaseFireworkStar : FireworkComponent
	{
		public static CraftResource GetMetal(Type star)
		{
			if (!star.IsEqualOrChildOf<BaseFireworkStar>() || star.IsEqualOrChildOf<FireworkStarCustom>())
			{
				return CraftResource.None;
			}

			if (star.IsEqualOrChildOf<FireworkStarIron>())
			{
				return CraftResource.Iron;
			}

			if (star.IsEqualOrChildOf<FireworkStarDullCopper>())
			{
				return CraftResource.DullCopper;
			}

			if (star.IsEqualOrChildOf<FireworkStarShadowIron>())
			{
				return CraftResource.ShadowIron;
			}

			if (star.IsEqualOrChildOf<FireworkStarCopper>())
			{
				return CraftResource.Copper;
			}

			if (star.IsEqualOrChildOf<FireworkStarBronze>())
			{
				return CraftResource.Bronze;
			}

			if (star.IsEqualOrChildOf<FireworkStarGold>())
			{
				return CraftResource.Gold;
			}

			if (star.IsEqualOrChildOf<FireworkStarAgapite>())
			{
				return CraftResource.Agapite;
			}

			if (star.IsEqualOrChildOf<FireworkStarVerite>())
			{
				return CraftResource.Verite;
			}

			if (star.IsEqualOrChildOf<FireworkStarValorite>())
			{
				return CraftResource.Valorite;
			}

			return CraftResource.None;
		}

		public static int[] GetEffectHues(Type star)
		{
			var res = GetMetal(star);

			return res != CraftResource.None ? GetEffectHues(ref res) : new int[0];
		}

		public static int[] GetEffectHues(ref CraftResource res)
		{
			LimitResource(ref res);

			int hue;

			switch (res)
			{
				case CraftResource.Iron:
					hue = 48; // Gold
					break;
				case CraftResource.DullCopper:
					hue = 98; // Dark Blue
					break;
				case CraftResource.ShadowIron:
					hue = 43; // Orange
					break;
				case CraftResource.Copper:
					hue = 88; // Blue
					break;
				case CraftResource.Bronze:
					hue = 38; // Red
					break;
				case CraftResource.Gold:
					hue = 63; // Green
					break;
				case CraftResource.Agapite:
					hue = 53; // Yellow 
					break;
				case CraftResource.Verite:
					hue = 13; // Purple
					break;
				case CraftResource.Valorite:
					hue = 183; // Aqua
					break;
				default:
					hue = 48;
					break;
			}

			var hues = new int[9];

			hues.SetAll(
				i =>
				{
					if (i > 0 && i % 3 == 0)
					{
						hue += 100;
					}

					return hue++;
				});

			return hues;
		}

		public static void LimitResource(ref CraftResource res)
		{
			var r = (int)res;

			if (r <= 0 || r > 9)
			{
				res = CraftResource.Iron;
			}
		}

		private CraftResource _Resource;

		[CommandProperty(AccessLevel.GameMaster)]
		public CraftResource Resource
		{
			get
			{
				LimitResource(ref _Resource);

				return _Resource;
			}
			set
			{
				LimitResource(ref value);

				if (_Resource == value)
				{
					return;
				}

				var old = _Resource;
				_Resource = value;

				if (Hue == 0 || Hue == CraftResources.GetHue(old))
				{
					Hue = CraftResources.GetHue(_Resource);
				}

				InvalidateProperties();
			}
		}

		public override double DefaultWeight => 0.1;

		public BaseFireworkStar(CraftResource res, int amount)
			: base(16932)
		{
			Name = "Firework Star";
			Resource = res;
			Stackable = true;
			Amount = Math.Max(1, Math.Min(60000, amount));
		}

		public BaseFireworkStar(Serial serial)
			: base(serial)
		{ }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			var res = CraftResources.GetName(Resource);

			if (CraftResources.IsStandard(Resource))
			{
				res = String.Empty;
			}
			else
			{
				if (Insensitive.Contains(res, "Normal") || Insensitive.Contains(res, "Plain"))
				{
					var typ = CraftResources.GetType(Resource).ToString();

					res = res.Replace("Normal", typ);
					res = res.Replace("Plain", typ);
				}

				res = res.Replace("Scales", "Scale");
				res = res.Replace("Crystals", "Crystal");
			}

			if (Amount <= 1)
			{
				list.Add(1050039, "{0}\t{1}", res, this.ResolveName());
			}
			else
			{
				list.Add(1050039, "{0}\t{1} {2}", Amount, res, this.ResolveName());
			}
		}

		public override bool StackWith(Mobile m, Item dropped, bool playSound)
		{
			if (m == null || !(dropped is BaseFireworkStar) || ((BaseFireworkStar)dropped)._Resource != _Resource)
			{
				return false;
			}

			return base.StackWith(m, dropped, playSound);
		}

		public override int OnCraft(
			int quality,
			bool makersMark,
			Mobile m,
			CraftSystem craftSystem,
			Type typeRes,
			ITool tool,
			CraftItem craftItem,
			int resHue)
		{
			if (craftSystem is Pyrotechnics && craftItem != null)
			{
				var resourceType = typeRes ?? craftItem.Resources.GetAt(0).ItemType;

				Resource = CraftResources.GetFromType(resourceType);
			}

			return base.OnCraft(quality, makersMark, m, craftSystem, typeRes, tool, craftItem, resHue);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteFlag(_Resource);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			_Resource = reader.ReadFlag<CraftResource>();
		}
	}

	public sealed class FireworkStarCustom : BaseFireworkStar
	{
		private bool _Converted;

		[Constructable]
		public FireworkStarCustom()
			: this(CraftResource.Iron, 1)
		{ }

		[Constructable]
		public FireworkStarCustom(CraftResource res, int amount)
			: base(res, amount)
		{ }

		public FireworkStarCustom(Serial serial)
			: base(serial)
		{ }

		public override void ReplaceWith(Item newItem)
		{
			if (Parent is Container)
			{
				var cont = (Container)Parent;

				if (newItem is BaseFireworkStar)
				{
					var star = (BaseFireworkStar)newItem;
					var starType = star.GetType();
					var stackWith = cont.FindItemsByType<BaseFireworkStar>()
										.FirstOrDefault(
											s => s.Stackable && s.Resource == star.Resource && s.Amount + star.Amount <= 60000 &&
												 s.GetType().IsEqual(starType));

					if (stackWith != null)
					{
						stackWith.Amount += newItem.Amount;
						newItem.Delete();
					}
					else
					{
						cont.AddItem(newItem);

						newItem.Location = Location;
					}
				}
				else
				{
					cont.AddItem(newItem);

					newItem.Location = Location;
				}
			}
			else
			{
				newItem.MoveToWorld(GetWorldLocation(), Map);
			}

			Delete();
		}

		public void Convert()
		{
			if (_Converted || Deleted || Map == null || Map == Map.Internal)
			{
				return;
			}

			_Converted = true;

			switch (Resource)
			{
				case CraftResource.Iron:
					ReplaceWith(new FireworkStarIron(Amount));
					break;
				case CraftResource.DullCopper:
					ReplaceWith(new FireworkStarDullCopper(Amount));
					break;
				case CraftResource.ShadowIron:
					ReplaceWith(new FireworkStarShadowIron(Amount));
					break;
				case CraftResource.Copper:
					ReplaceWith(new FireworkStarCopper(Amount));
					break;
				case CraftResource.Bronze:
					ReplaceWith(new FireworkStarBronze(Amount));
					break;
				case CraftResource.Gold:
					ReplaceWith(new FireworkStarGold(Amount));
					break;
				case CraftResource.Agapite:
					ReplaceWith(new FireworkStarAgapite(Amount));
					break;
				case CraftResource.Verite:
					ReplaceWith(new FireworkStarVerite(Amount));
					break;
				case CraftResource.Valorite:
					ReplaceWith(new FireworkStarValorite(Amount));
					break;
				default:
					ReplaceWith(new FireworkStarIron(Amount));
					break;
			}
		}

#if NEWPARENT
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
		{
			base.OnAdded(parent);

			if (Map != null && Map != Map.Internal)
			{
				Convert();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}
