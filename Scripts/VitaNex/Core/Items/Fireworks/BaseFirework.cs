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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Server;
using Server.Engines.Craft;
using Server.Items;

using VitaNex.FX;
using VitaNex.Network;
using VitaNex.SuperCrafts;
#endregion

namespace VitaNex.Items
{
	public abstract class BaseFirework : Item, ICraftable
	{
		protected virtual object UseLock => GetType();

		public virtual string DefToken => String.Empty;
		public virtual TimeSpan DefUseDelay => TimeSpan.FromSeconds(3.0);

		private string _Token;
		private Mobile _Crafter;

		[CommandProperty(AccessLevel.GameMaster)]
		public string Token
		{
			get => _Token;
			set
			{
				_Token = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Crafter
		{
			get => _Crafter;
			set
			{
				_Crafter = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan UseDelay { get; set; }

		#region Fuse
		public virtual TimeSpan DefFuseDelay => TimeSpan.FromSeconds(1.0);

		private Timer _FuseTimer;

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan FuseDelay { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FuseLit => _FuseTimer != null && _FuseTimer.Running;
		#endregion

		#region Ignite Effect
		public virtual int DefIgniteEffectID => 14276;
		public virtual int DefIgniteEffectHue => 0;
		public virtual int DefIgniteEffectSpeed => 5;
		public virtual EffectRender DefIgniteEffectRender => EffectRender.LightenMore;
		public virtual int DefIgniteEffectSound => 1232;

		[CommandProperty(AccessLevel.GameMaster)]
		public int IgniteEffectID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int IgniteEffectHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int IgniteEffectSpeed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public EffectRender IgniteEffectRender { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int IgniteEffectSound { get; set; }
		#endregion

		#region Fail Effect
		public virtual int DefFailEffectID => 14133;
		public virtual int DefFailEffectHue => 0;
		public virtual int DefFailEffectSpeed => 10;
		public virtual int DefFailEffectDuration => 30;
		public virtual EffectRender DefFailEffectRender => EffectRender.Normal;
		public virtual int DefFailEffectSound => 1488;

		[CommandProperty(AccessLevel.GameMaster)]
		public int FailEffectID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int FailEffectHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int FailEffectSpeed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int FailEffectDuration { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public EffectRender FailEffectRender { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int FailEffectSound { get; set; }
		#endregion

		public BaseFirework(int itemID, int hue)
			: base(itemID)
		{
			Name = "Firework Rocket";
			Weight = 1.0;
			Light = LightType.Circle300;

			Hue = hue;

			UseDelay = DefUseDelay;
			Token = DefToken;

			FuseDelay = DefFuseDelay;

			IgniteEffectID = DefIgniteEffectID;
			IgniteEffectHue = DefIgniteEffectHue;
			IgniteEffectSpeed = DefIgniteEffectSpeed;
			IgniteEffectRender = DefIgniteEffectRender;
			IgniteEffectSound = DefIgniteEffectSound;

			FailEffectID = DefFailEffectID;
			FailEffectHue = DefFailEffectHue;
			FailEffectSpeed = DefFailEffectSpeed;
			FailEffectDuration = DefFailEffectDuration;
			FailEffectRender = DefFailEffectRender;
			FailEffectSound = DefFailEffectSound;
		}

		public BaseFirework(Serial serial)
			: base(serial)
		{ }

		public override bool IsAccessibleTo(Mobile check)
		{
			return base.IsAccessibleTo(check) && !FuseLit;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (Crafter != null && !String.IsNullOrWhiteSpace(Crafter.RawName))
			{
				list.Add(1050043, Crafter.RawName);
			}

			var props = new StringBuilder();

			GetProperties(props);

			if (props.Length > 0)
			{
				list.Add(props.ToString());
			}
		}

		public virtual void GetProperties(StringBuilder props)
		{
			if (!Visible)
			{
				return;
			}

			if (FuseLit)
			{
				var now = DateTime.UtcNow;

				if (_FuseTimer.Next < now)
				{
					var ts = now - _FuseTimer.Next;

					if (ts > TimeSpan.Zero)
					{
						props.AppendLine("Fuse: {0}", ts.ToSimpleString("h:m:s"));
					}
				}
			}
			else
			{
				props.AppendLine("Use: Light the fuse!".WrapUOHtmlColor(Color.LawnGreen));
			}

			if (!String.IsNullOrWhiteSpace(Token))
			{
				props.AppendLine("\"{0}\"".WrapUOHtmlColor(Color.Gold), Token);
			}
		}

		public virtual int OnCraft(
			int quality,
			bool makersMark,
			Mobile m,
			CraftSystem craftSystem,
			Type typeRes,
			ITool tool,
			CraftItem craftItem,
			int resHue)
		{
			if (makersMark)
			{
				Crafter = m;
			}

			var context = craftSystem.GetContext(m);

			if (context != null && context.DoNotColor)
			{
				Hue = 0;
			}
			else if (resHue > 0)
			{
				Hue = resHue;
			}

			if (craftSystem is Pyrotechnics && craftItem != null)
			{
				var fuses = new List<CraftRes>(craftItem.Resources.Count);

				fuses.SetAll(craftItem.Resources.GetAt);
				fuses.RemoveAll(res => !res.ItemType.TypeEquals<FireworkFuse>());

				var fuseDelay = FuseDelay.TotalSeconds * fuses.Sum(r => r.Amount);

				fuses.Free(true);

				if (fuseDelay > 0)
				{
					FuseDelay = TimeSpan.FromSeconds(fuseDelay);
				}
			}

			return quality;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (FuseLit)
			{
				m.SendMessage("The fuse is already burning.");
				return;
			}

			if (!this.CheckDoubleClick(m, true, false, 10))
			{
				return;
			}

			if (!m.BeginAction(UseLock, UseDelay))
			{
				m.SendMessage("You must wait before you can light another {0}.", this.ResolveName(m));
				return;
			}

			MoveToWorld(m.Location, m.Map);

			IgniteFuse(m);

			InvalidateProperties();
		}

		protected void IgniteFuse(Mobile m)
		{
			if (m == null || m.Deleted || FuseLit)
			{
				return;
			}

			if (!OnIgnite(m))
			{
				return;
			}

			if (IgniteEffectID > 0)
			{
				var fx = new EffectInfo(
					GetWorldLocation(),
					Map,
					IgniteEffectID,
					IgniteEffectHue,
					IgniteEffectSpeed,
					(int)Math.Ceiling(FuseDelay.TotalMilliseconds / 100.0),
					IgniteEffectRender);

				fx.Send();
			}

			if (IgniteEffectSound > 0)
			{
				Effects.PlaySound(GetWorldLocation(), Map, IgniteEffectSound);
			}

			_FuseTimer = Timer.DelayCall(
				FuseDelay,
				() =>
				{
					_FuseTimer = null;

					InvalidateProperties();

					if (!OnFuseBurned(m))
					{
						Fail(m);
					}
				});
		}

		protected virtual bool OnIgnite(Mobile m)
		{
			return true;
		}

		protected abstract bool OnFuseBurned(Mobile m);

		protected void Fail(Mobile m)
		{
			if (FailEffectID > 0)
			{
				var fx = new EffectInfo(
					GetWorldLocation(),
					Map,
					FailEffectID,
					FailEffectHue,
					FailEffectSpeed,
					FailEffectDuration,
					FailEffectRender);

				fx.Send();
			}

			if (FailEffectSound > 0)
			{
				Effects.PlaySound(GetWorldLocation(), Map, 1488);
			}

			Movable = Visible = true;

			OnFail(m);
		}

		protected virtual void OnFail(Mobile m)
		{
			if (m != null)
			{
				m.SendMessage("The fuse has failed to ignite on {0}!", this.ResolveName(m));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(_Token);
					writer.Write(_Crafter);

					writer.Write(UseDelay);
					writer.Write(FuseDelay);

					writer.Write(IgniteEffectID);
					writer.Write(IgniteEffectHue);
					writer.Write(IgniteEffectSpeed);
					writer.WriteFlag(IgniteEffectRender);
					writer.Write(IgniteEffectSound);

					writer.Write(FailEffectID);
					writer.Write(FailEffectHue);
					writer.Write(FailEffectSpeed);
					writer.Write(FailEffectDuration);
					writer.WriteFlag(FailEffectRender);
					writer.Write(FailEffectSound);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					_Token = reader.ReadString();
					_Crafter = reader.ReadMobile();

					UseDelay = reader.ReadTimeSpan();
					FuseDelay = reader.ReadTimeSpan();

					IgniteEffectID = reader.ReadInt();
					IgniteEffectHue = reader.ReadInt();
					IgniteEffectSpeed = reader.ReadInt();
					IgniteEffectRender = reader.ReadFlag<EffectRender>();
					IgniteEffectSound = reader.ReadInt();

					FailEffectID = reader.ReadInt();
					FailEffectHue = reader.ReadInt();
					FailEffectSpeed = reader.ReadInt();
					FailEffectDuration = reader.ReadInt();
					FailEffectRender = reader.ReadFlag<EffectRender>();
					FailEffectSound = reader.ReadInt();
				}
				break;
			}
		}
	}
}
