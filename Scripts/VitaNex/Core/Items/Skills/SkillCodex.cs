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

using Server;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Items
{
	public enum SkillCodexFlags : byte
	{
		Base = 0x00,
		Cap = 0x01,
		Both = 0x02
	}

	public enum SkillCodexMode : byte
	{
		Fixed = 0x00,
		Increase = 0x01,
		Decrease = 0x02
	}

	public abstract class SkillCodex : Item
	{
		private SuperGump SelectionGump { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillCodexMode Mode { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SkillCodexFlags Flags { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Count { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public double Value { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ValueFixed { get => (int)(Value * 10.0); set => Value = value / 10.0; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool DeleteWhenEmpty { get; set; }

		public List<SkillName> IgnoredSkills { get; protected set; }
		public List<SkillName> SelectedSkills { get; protected set; }

		public SkillCodex()
			: this(1)
		{ }

		public SkillCodex(int count)
			: this(count, 100.0)
		{ }

		public SkillCodex(int count, double value)
			: this(count, value, true)
		{ }

		public SkillCodex(int count, double value, bool deleteWhenEmpty)
			: this(count, value, deleteWhenEmpty, SkillCodexMode.Fixed)
		{ }

		public SkillCodex(int count, double value, bool deleteWhenEmpty, SkillCodexMode mode)
			: this(count, value, deleteWhenEmpty, mode, SkillCodexFlags.Base)
		{ }

		public SkillCodex(int count, double value, bool deleteWhenEmpty, SkillCodexMode mode, SkillCodexFlags flags)
			: base(8793)
		{
			SelectedSkills = new List<SkillName>();
			IgnoredSkills = new List<SkillName>();

			Mode = mode;
			Flags = flags;
			Count = count;
			Value = value;
			DeleteWhenEmpty = deleteWhenEmpty;

			Name = "Codex of Wisdom";
			LootType = LootType.Blessed;
			Stackable = false;
		}

		public SkillCodex(Serial serial)
			: base(serial)
		{ }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			string html = String.Empty, flags = String.Empty;

			html += String.Format("<basefont color=#{0:X6}>Use: ", Color.LawnGreen.ToRgb());

			switch (Flags)
			{
				case SkillCodexFlags.Base:
					flags = "value";
					break;
				case SkillCodexFlags.Cap:
					flags = "cap";
					break;
				case SkillCodexFlags.Both:
					flags = "value and cap";
					break;
			}

			switch (Mode)
			{
				case SkillCodexMode.Increase:
				{
					html += String.Format(
						"Increase {0} skill{1} {2} by {3:F2}%",
						Count,
						Count == 1 ? String.Empty : "s",
						flags,
						Value);
				}
				break;
				case SkillCodexMode.Decrease:
				{
					html += String.Format(
						"Decrease {0} skill{1} {2} by {3:F2}%",
						Count,
						Count == 1 ? String.Empty : "s",
						flags,
						Value);
				}
				break;
				case SkillCodexMode.Fixed:
				{
					html += String.Format("Set {0} skill{1} {2} to {3:F2}%", Count, Count == 1 ? String.Empty : "s", flags, Value);
				}
				break;
			}

			list.Add(html);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || from.Deleted || !Validate(from, true))
			{
				return;
			}

			SelectSkills(from);
			InvalidateProperties();
		}

		public virtual bool ValidateSkill(Mobile user, Skill skill, bool message)
		{
			if (user == null || user.Deleted || skill == null)
			{
				return false;
			}

			switch (Mode)
			{
				case SkillCodexMode.Increase:
				{
					if (Flags == SkillCodexFlags.Base || Flags == SkillCodexFlags.Both)
					{
						if (user.SkillsTotal + ValueFixed > user.SkillsCap)
						{
							if (!CanReduceSkills(user))
							{
								if (message)
								{
									user.SendMessage(
										SuperGump.DefaultErrorHue,
										"You already know everything this codex can offer, reduce some skills to make room for more knowledge.");
								}

								return false;
							}
						}

						if (skill.IsCapped() || skill.WillCap(Value, false))
						{
							if (message)
							{
								user.SendMessage(
									SuperGump.DefaultErrorHue,
									"You already know everything this codex can offer about {0}.",
									skill.Name);
							}

							return false;
						}

						if (!skill.IsLocked(SkillLock.Up))
						{
							if (message)
							{
								user.SendMessage(SuperGump.DefaultErrorHue, "The skill {0} is locked.", skill.Name);
							}

							return false;
						}
					}
				}
				break;
				case SkillCodexMode.Decrease:
				{
					if (Flags == SkillCodexFlags.Base || Flags == SkillCodexFlags.Both)
					{
						if (user.SkillsTotal - ValueFixed < 0)
						{
							if (message)
							{
								user.SendMessage(SuperGump.DefaultErrorHue, "You already forgot everything this codex can offer.");
							}

							return false;
						}

						if (skill.IsZero() || skill.WillZero(Value, false))
						{
							if (message)
							{
								user.SendMessage(
									SuperGump.DefaultErrorHue,
									"You already forgot everything this codex can offer about {0}, any further and you'll forget how to breath!",
									skill.Name);
							}

							return false;
						}

						if (!skill.IsLocked(SkillLock.Down))
						{
							if (message)
							{
								user.SendMessage(SuperGump.DefaultErrorHue, "The skill {0} is locked.", skill.Name);
							}

							return false;
						}
					}
				}
				break;
				case SkillCodexMode.Fixed:
				{
					if (Flags == SkillCodexFlags.Cap)
					{
						if (skill.CapFixedPoint == ValueFixed)
						{
							if (message)
							{
								user.SendMessage(
									SuperGump.DefaultErrorHue,
									"You already know everything this codex can offer about {0}.",
									skill.Name);
							}

							return false;
						}
					}

					if (Flags == SkillCodexFlags.Base || Flags == SkillCodexFlags.Both)
					{
						if (skill.IsLocked(SkillLock.Locked))
						{
							if (message)
							{
								user.SendMessage(SuperGump.DefaultErrorHue, "The skill {0} is locked.", skill.Name);
							}

							return false;
						}

						if (ValueFixed < skill.BaseFixedPoint)
						{
							if (!skill.IsLocked(SkillLock.Down))
							{
								if (message)
								{
									user.SendMessage(SuperGump.DefaultErrorHue, "You can not decrease this skill because it is not locked down!");
								}

								return false;
							}

							if (user.SkillsTotal - (skill.BaseFixedPoint - ValueFixed) < 0)
							{
								if (message)
								{
									user.SendMessage(
										SuperGump.DefaultErrorHue,
										"You already forgot everything this codex can offer, any further and you'll forget how to breath!");
								}

								return false;
							}
						}
						else if (ValueFixed > skill.BaseFixedPoint)
						{
							if (!skill.IsLocked(SkillLock.Up))
							{
								if (message)
								{
									user.SendMessage(SuperGump.DefaultErrorHue, "You can not increase this skill because it is not locked up!");
								}

								return false;
							}

							if (user.SkillsTotal + (ValueFixed - skill.BaseFixedPoint) > user.SkillsCap)
							{
								if (!CanReduceSkills(user))
								{
									if (message)
									{
										user.SendMessage(
											SuperGump.DefaultErrorHue,
											"You already know everything this codex can offer, reduce some skills to make room for more knowledge.");
									}

									return false;
								}
							}
						}
						else
						{
							if (message)
							{
								user.SendMessage(
									SuperGump.DefaultErrorHue,
									"You already know everything this codex can offer about {0}.",
									skill.Name);
							}

							return false;
						}
					}
				}
				break;
			}

			return true;
		}

		public virtual bool Validate(Mobile user, bool message)
		{
			if (user == null || user.Deleted || !user.CanSee(this))
			{
				return false;
			}

			if (!IsChildOf(user.Backpack))
			{
				if (message)
				{
					user.SendMessage(SuperGump.DefaultErrorHue, "This codex must be in your backpack to read it.");
				}

				return false;
			}

			if (Count <= 0)
			{
				if (message)
				{
					user.SendMessage(SuperGump.DefaultErrorHue, "This codex does not contain any skills to learn.");
				}

				return false;
			}

			return true;
		}

		public virtual void SelectSkills(Mobile user)
		{
			if (user == null || user.Deleted || !Validate(user, true))
			{
				return;
			}

			if (SelectionGump != null)
			{
				SelectionGump.Close(true);
			}

			SelectionGump = new SkillSelectionGump(
				user,
				null,
				Count,
				null,
				null,
				skills =>
				{
					if (!ApplySkills(user, skills))
					{
						SelectSkills(user);
					}
				},
				IgnoredSkills.ToArray()).Send();
		}

		public virtual bool ApplySkills(Mobile user, SkillName[] skills)
		{
			if (user == null || user.Deleted || skills == null || skills.Length == 0 || !Validate(user, true))
			{
				return false;
			}

			foreach (var sn in skills)
			{
				if (user.Deleted || !Validate(user, true))
				{
					return false;
				}

				var skill = user.Skills[sn];

				if (!ValidateSkill(user, skill, true))
				{
					continue;
				}

				var charge = false;

				if (Flags == SkillCodexFlags.Cap || Flags == SkillCodexFlags.Both)
				{
					switch (Mode)
					{
						case SkillCodexMode.Increase:
						{
							skill.IncreaseCap(Value);
						}
						break;
						case SkillCodexMode.Decrease:
						{
							skill.DecreaseCap(Value);
						}
						break;
						case SkillCodexMode.Fixed:
						{
							skill.SetCap(Value);
						}
						break;
					}

					if (Flags != SkillCodexFlags.Both)
					{
						EndApply(user, skill);
						return true;
					}

					charge = true;
				}

				if (Flags == SkillCodexFlags.Base || Flags == SkillCodexFlags.Both)
				{
					charge = false;

					switch (Mode)
					{
						case SkillCodexMode.Increase:
						{
							if (user.SkillsTotal + ValueFixed > user.SkillsCap)
							{
								if (TryReduceSkills(user) && skill.IncreaseBase(Value))
								{
									charge = true;
								}
							}
							else if (skill.IncreaseBase(Value))
							{
								charge = true;
							}
						}
						break;
						case SkillCodexMode.Decrease:
						{
							if (skill.DecreaseBase(Value))
							{
								charge = true;
							}
						}
						break;
						case SkillCodexMode.Fixed:
						{
							if (ValueFixed > skill.BaseFixedPoint)
							{
								if (user.SkillsTotal + ValueFixed > user.SkillsCap)
								{
									if (TryReduceSkills(user) && skill.SetBase(Value))
									{
										charge = true;
									}
								}
								else if (skill.SetBase(Value))
								{
									charge = true;
								}
							}
							else if (ValueFixed < skill.BaseFixedPoint && skill.SetBase(Value))
							{
								charge = true;
							}
						}
						break;
					}
				}

				if (charge)
				{
					EndApply(user, skill);
				}
			}

			return true;
		}

		protected virtual void EndApply(Mobile user, Skill skill)
		{
			skill.Normalize();

			UseCharges();

			if (Count <= 0 && DeleteWhenEmpty)
			{
				Delete();
			}
		}

		protected virtual bool CanReduceSkills(Mobile user)
		{
			var target = (user.SkillsTotal + ValueFixed) - user.SkillsCap;
			var canReduceBy = 0;

			foreach (var s in user.Skills)
			{
				if (s.IsLocked(SkillLock.Down))
				{
					if (canReduceBy + s.BaseFixedPoint > target)
					{
						canReduceBy += (target - canReduceBy);
					}
					else
					{
						canReduceBy += s.BaseFixedPoint;
					}
				}

				if (canReduceBy >= target)
				{
					return true;
				}
			}

			return false;
		}

		protected virtual bool TryReduceSkills(Mobile user)
		{
			if (!CanReduceSkills(user))
			{
				return false;
			}

			var target = (user.SkillsTotal + ValueFixed) - user.SkillsCap;
			var reducedBy = 0;

			foreach (var s in user.Skills)
			{
				if (s.IsLocked(SkillLock.Down))
				{
					if (reducedBy + s.BaseFixedPoint > target)
					{
						var diff = (target - reducedBy);

						if (s.DecreaseBase(diff / 10))
						{
							reducedBy += diff;
						}
					}
					else
					{
						if (s.SetBase(0))
						{
							reducedBy += s.BaseFixedPoint;
						}
					}
				}

				if (reducedBy >= target)
				{
					return true;
				}
			}

			return false;
		}

		public virtual void UseCharges(int count = 1)
		{
			Count = Math.Max(0, Count - count);
			InvalidateProperties();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.WriteFlag(Mode);
					writer.WriteFlag(Flags);
					writer.Write(Count);
					writer.Write(Value);
					writer.Write(DeleteWhenEmpty);
					writer.WriteList(IgnoredSkills, skill => writer.WriteFlag(skill));
				}
				break;
				case 0:
				{
					writer.Write((byte)Mode);
					writer.Write((byte)Flags);
					writer.Write(Count);
					writer.Write(Value);
					writer.Write(DeleteWhenEmpty);
					writer.WriteList(IgnoredSkills, (w, skill) => w.Write((short)skill));
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
				case 1:
				{
					Mode = reader.ReadFlag<SkillCodexMode>();
					Flags = reader.ReadFlag<SkillCodexFlags>();
					Count = reader.ReadInt();
					Value = reader.ReadDouble();
					DeleteWhenEmpty = reader.ReadBool();
					IgnoredSkills = reader.ReadList(r => r.ReadFlag<SkillName>());
				}
				break;
				case 0:
				{
					Mode = (SkillCodexMode)reader.ReadByte();
					Flags = (SkillCodexFlags)reader.ReadByte();
					Count = reader.ReadInt();
					Value = reader.ReadDouble();
					DeleteWhenEmpty = reader.ReadBool();
					IgnoredSkills = reader.ReadList(r => (SkillName)r.ReadShort());
				}
				break;
			}
		}
	}
}
