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

using Server;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;

using VitaNex.FX;
using VitaNex.Network;
using VitaNex.Targets;
#endregion

namespace VitaNex.Items
{
	public abstract class BaseThrowable<TEntity> : Item, IBaseThrowable
		where TEntity : IEntity
	{
		public PollTimer UpdateTimer { get; protected set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Silent { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string Usage { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string Token { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool AllowNoOwner { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool AllowCombat { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool AllowDeadUser { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Consumable { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool ClearHands { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool DismountUser { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int EffectID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int EffectHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int EffectSpeed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual EffectRender EffectRender { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual TargetFlags TargetFlags { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int ThrowSound { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int ImpactSound { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int ThrowRange { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual TimeSpan ThrowRecovery { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual DateTime ThrownLast { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual SkillName RequiredSkill { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual double RequiredSkillValue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual Mobile User { get; set; }

		public BaseThrowable(int itemID)
			: this(itemID, 1)
		{ }

		public BaseThrowable(int itemID, int amount)
			: base(itemID)
		{
			Stackable = true;
			Amount = amount;
			Weight = 1.0;

			Usage = "Throw Me!";
			AllowNoOwner = false;
			AllowCombat = true;
			AllowDeadUser = false;
			Consumable = true;
			ClearHands = true;
			DismountUser = true;
			EffectID = 0;
			EffectHue = 0;
			EffectRender = EffectRender.Normal;
			EffectSpeed = 10;
			ThrowSound = -1;
			ImpactSound = -1;
			ThrowRecovery = TimeSpan.FromSeconds(60.0);
			ThrowRange = 12;
			RequiredSkill = SkillName.Throwing;
			RequiredSkillValue = 0;
		}

		public BaseThrowable(Serial serial)
			: base(serial)
		{ }

		public override void OnAfterDuped(Item newItem)
		{
			base.OnAfterDuped(newItem);

			var t = newItem as BaseThrowable<TEntity>;

			if (t != null)
			{
				t.UpdateTimer = null;
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (UpdateTimer == null)
			{
				return;
			}

			UpdateTimer.Running = false;
			UpdateTimer = null;
		}

		public virtual bool CanThrow(Mobile m, bool message)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (m.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			if (!AllowNoOwner && RootParent != m)
			{
				if (message)
				{
					m.SendMessage(37, "You must own the {0} before you can throw it!", this.ResolveName(m));
				}

				return false;
			}

			if (!m.Alive && !AllowDeadUser)
			{
				if (message)
				{
					m.SendMessage(37, "You can't throw the {0} while dead.", this.ResolveName(m));
				}

				return false;
			}

			if (RequiredSkillValue > 0 && m.Skills[RequiredSkill].Value < RequiredSkillValue)
			{
				if (message)
				{
					m.SendMessage(37, "You do not have the skill required to throw the {0}.", this.ResolveName(m));
				}

				return false;
			}

			if (!m.CanBeginAction(GetType()))
			{
				if (message)
				{
					m.SendMessage(37, "Your arms are tired, you must wait to throw another {0}!", this.ResolveName(m));
				}

				return false;
			}

			if (!AllowCombat && SpellHelper.CheckCombat(m))
			{
				if (message)
				{
					m.SendMessage(37, "You can't throw the {0} while in combat!", this.ResolveName(m));
				}

				return false;
			}

			if (m.BodyMod > 0 && !m.BodyMod.IsHuman)
			{
				if (message)
				{
					m.SendMessage(37, "How are you supposed to throw the {0} with no hands, beast?", this.ResolveName(m));
				}

				return false;
			}

			return true;
		}

		public virtual bool CanThrowAt(Mobile m, TEntity target, bool message)
		{
			if (m == null || m.Deleted || target == null)
			{
				return false;
			}

			if (m == target as Mobile)
			{
				if (message)
				{
					m.SendMessage(37, "You can't throw the {0} at yourself!", this.ResolveName(m));
				}

				return false;
			}

			if (m.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			if (!m.CanSee(target) || !m.InLOS(target))
			{
				if (message)
				{
					m.SendMessage(37, "You can't aim at something you can't see!");
				}

				return false;
			}

			return true;
		}

		public virtual void BeginThrow(Mobile m, TEntity target)
		{
			if (m == null || m.Deleted || target == null)
			{
				return;
			}

			if (!CanThrow(m, true) || !CanThrowAt(m, target, true) ||
				(m.AccessLevel < AccessLevel.GameMaster && !m.BeginAction(GetType())))
			{
				return;
			}

			User = m;

			OnBeforeThrownAt(m, target);

			var effect = EffectID > 0 ? EffectID : ItemID;
			var hue = EffectHue > 0 ? EffectHue : Hue;

			new MovingEffectInfo(m, target, target.Map, effect, hue, EffectSpeed, EffectRender).MovingImpact(
				() => FinishThrow(m, target));
		}

		public virtual void FinishThrow(Mobile m, TEntity target)
		{
			if (m != null && !m.Deleted && target != null)
			{
				OnThrownAt(m, target);
			}
		}

		public virtual Target BeginTarget(Mobile m)
		{
			return new GenericSelectTarget<TEntity>(BeginThrow, t => { }, ThrowRange, false, TargetFlags);
		}

		protected virtual void OnBeginTarget(Mobile m, bool message)
		{
			if (m == null || m.Deleted || !message || Silent)
			{
				return;
			}

			m.LocalOverheadMessage(
				MessageType.Emote,
				0x55,
				true,
				String.Format("You begin to swing the {0}...", this.ResolveName(m)));
			m.NonlocalOverheadMessage(
				MessageType.Emote,
				0x55,
				true,
				String.Format("{0} begins to swing the {1}...", m.RawName, this.ResolveName(m)));
		}

		protected virtual void OnBeforeThrownAt(Mobile m, TEntity target)
		{
			if (m == null || m.Deleted || target == null || !CanThrowAt(m, target, false))
			{
				return;
			}

			if (m.Spell != null)
			{
				m.Spell.OnCasterUsingObject(this);
			}

			if (DismountUser)
			{
				EtherealMount.StopMounting(m);
			}

			if (ClearHands)
			{
				m.ClearHands();
			}

			if (DismountUser && m.Mounted)
			{
				BaseMount.Dismount(m);
			}

			if ((m.Direction & Direction.Running) != Direction.Running)
			{
				m.Direction = m.GetDirectionTo(target.Location);
			}

			m.Animate(11, 5, 1, true, false, 0);

			if (ThrowSound >= 0)
			{
				m.PlaySound(ThrowSound);
			}
		}

		protected virtual void OnThrownAt(Mobile m, TEntity target)
		{
			if (m == null || m.Deleted || target == null)
			{
				return;
			}

			if (ImpactSound >= 0)
			{
				Effects.PlaySound(target.Location, target.Map, ImpactSound);
			}

			ThrownLast = DateTime.UtcNow;

			if (Consumable)
			{
				Consume();
			}

			if (ThrowRecovery > TimeSpan.Zero)
			{
				if (UpdateTimer == null)
				{
					UpdateTimer = PollTimer.FromSeconds(
						1.0,
						() =>
						{
							ClearProperties();
							Delta(ItemDelta.Properties);

							var readyWhen = ThrownLast + ThrowRecovery;

							if (DateTime.UtcNow < readyWhen)
							{
								return;
							}

							m.EndAction(GetType());

							if (UpdateTimer == null)
							{
								return;
							}

							UpdateTimer.Running = false;
							UpdateTimer = null;
						});
				}
				else
				{
					UpdateTimer.Running = true;
				}
			}
			else
			{
				if (UpdateTimer != null)
				{
					UpdateTimer.Running = false;
					UpdateTimer = null;
				}

				ClearProperties();
				Delta(ItemDelta.Properties);
				m.EndAction(GetType());
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if (UpdateTimer == null)
			{
				return;
			}

			UpdateTimer.Running = false;
			UpdateTimer = null;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (!CanThrow(m, true))
			{
				return;
			}

			if (m.Spell != null)
			{
				m.Spell.OnCasterUsingObject(this);
			}

			if (ClearHands)
			{
				m.ClearHands();
			}

			if (DismountUser)
			{
				EtherealMount.StopMounting(m);
			}

			if (DismountUser && m.Mounted)
			{
				BaseMount.Dismount(m);
			}

			m.Target = BeginTarget(m);
			OnBeginTarget(m, true);
		}

		public override void OnAosSingleClick(Mobile from)
		{
			OnSingleClick(from);
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			DateTime now = DateTime.UtcNow, readyWhen = ThrownLast + ThrowRecovery;
			var diff = TimeSpan.Zero;

			if (readyWhen > now)
			{
				diff = readyWhen - now;
			}

			if (diff > TimeSpan.Zero)
			{
				var time = String.Format("{0:D2}:{1:D2}:{2:D2}", diff.Hours, diff.Minutes, diff.Seconds);
				LabelTo(from, "Use: {0}", time);
			}
			else if (!String.IsNullOrWhiteSpace(Usage))
			{
				LabelTo(from, "Use: {0}", Usage);
			}

			if (!String.IsNullOrWhiteSpace(Token))
			{
				LabelTo(from, "\"{0}\"", Token);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(2);

			switch (version)
			{
				case 3:
					writer.Write(AllowNoOwner);
					goto case 2;
				case 2:
				case 1:
					writer.Write(Silent);
					goto case 0;
				case 0:
				{
					writer.Write(Usage);
					writer.Write(Token);
					writer.Write(AllowCombat);
					writer.Write(AllowDeadUser);
					writer.Write(Consumable);
					writer.Write(ClearHands);
					writer.Write(DismountUser);
					writer.Write(EffectID);
					writer.Write(EffectHue);
					writer.Write(EffectSpeed);

					if (version < 2)
					{
						writer.Write((short)EffectRender);
						writer.Write((byte)TargetFlags);
					}
					else
					{
						writer.WriteFlag(EffectRender);
						writer.WriteFlag(TargetFlags);
					}

					writer.Write(ThrowSound);
					writer.Write(ImpactSound);
					writer.Write(ThrowRange);
					writer.Write(ThrowRecovery);
					writer.Write(ThrownLast);

					if (version < 2)
					{
						writer.Write((short)RequiredSkill);
					}
					else
					{
						writer.WriteFlag(RequiredSkill);
					}

					writer.Write(RequiredSkillValue);
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
				case 3:
					AllowNoOwner = reader.ReadBool();
					goto case 2;
				case 2:
				case 1:
					Silent = reader.ReadBool();
					goto case 0;
				case 0:
				{
					Usage = reader.ReadString();
					Token = reader.ReadString();
					AllowCombat = reader.ReadBool();
					AllowDeadUser = reader.ReadBool();
					Consumable = reader.ReadBool();
					ClearHands = reader.ReadBool();
					DismountUser = reader.ReadBool();
					EffectID = reader.ReadInt();
					EffectHue = reader.ReadInt();
					EffectSpeed = reader.ReadInt();

					if (version < 2)
					{
						EffectRender = (EffectRender)reader.ReadShort();
						TargetFlags = (TargetFlags)reader.ReadByte();
					}
					else
					{
						EffectRender = reader.ReadFlag<EffectRender>();
						TargetFlags = reader.ReadFlag<TargetFlags>();
					}

					ThrowSound = reader.ReadInt();
					ImpactSound = reader.ReadInt();
					ThrowRange = reader.ReadInt();
					ThrowRecovery = reader.ReadTimeSpan();
					ThrownLast = reader.ReadDateTime();

					if (version < 2)
					{
						RequiredSkill = (SkillName)reader.ReadShort();
					}
					else
					{
						RequiredSkill = reader.ReadFlag<SkillName>();
					}

					RequiredSkillValue = reader.ReadDouble();
				}
				break;
			}
		}
	}
}
