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
using System.Linq;
using System.Text;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Targets;

using Skills = Server.Skills;
#endregion

namespace VitaNex.Commands
{
	[Flags]
	public enum FixMeFlags
	{
		None = 0x0000,
		Mount = 0x0001,
		Pets = 0x0002,
		Equip = 0x0004,
		Gumps = 0x0008,
		Tags = 0x0010,
		Skills = 0x0020,
		Quests = 0x0040,
		All = ~None
	}

	public static class FixMeCommand
	{
		public delegate void ResolveFlagsHandler(Mobile m, ref FixMeFlags flags);

		public static event Action<Mobile> OnFixMount;
		public static event Action<PlayerMobile> OnFixPets;
		public static event Action<PlayerMobile> OnFixEquip;
		public static event Action<Mobile> OnFixGumps;
		public static event Action<Mobile> OnFixTags;
		public static event Action<Mobile> OnFixSkills;
		public static event Action<PlayerMobile> OnFixQuests;

		public static event Action<FixMeGump> OnGumpSend;
		public static event Action<Mobile, FixMeFlags> OnFix;

		public static event ResolveFlagsHandler ResolveFlags;

		public static FixMeFlags DisabledFlags { get; set; }

		static FixMeCommand()
		{
			OnFixMount += FixMount;
			OnFixPets += FixPets;
			OnFixEquip += FixEquip;
			OnFixGumps += FixGumps;
			OnFixTags += FixTags;
			OnFixSkills += FixSkills;
			OnFixQuests += FixQuests;
		}

		public static void Configure()
		{
			CommandSystem.Register(
				"FixMe",
				AccessLevel.Player,
				e =>
				{
					if (e == null || !(e.Mobile is PlayerMobile))
					{
						return;
					}

					var g = SuperGump.Send(new FixMeGump((PlayerMobile)e.Mobile));

					if (OnGumpSend != null)
					{
						OnGumpSend(g);
					}
				});

			CommandSystem.Register(
				"FixThem",
				AccessLevel.GameMaster,
				e =>
				{
					if (e == null || !(e.Mobile is PlayerMobile))
					{
						return;
					}

					e.Mobile.SendMessage(0x22, "Target an online player to send them the FixMe gump.");
					e.Mobile.Target = new MobileSelectTarget<PlayerMobile>(
						(m, target) =>
						{
							if (target == null || target.Deleted)
							{
								return;
							}

							if (!target.IsOnline())
							{
								m.SendMessage(0x22, "{0} must be online.", target.RawName);
								return;
							}

							m.SendMessage(0x55, "Opening FixMe gump for {0}...", target.RawName);

							var g = SuperGump.Send(new FixMeGump(target));

							if (OnGumpSend != null)
							{
								OnGumpSend(g);
							}
						},
						m => m.SendMessage(0x22, "Target an on-line player to send them the FixMe gump."));
				});
		}

		public static string GetDescription(this FixMeFlags flags)
		{
			var html = new StringBuilder();

			switch (flags)
			{
				case FixMeFlags.Mount:
					html.Append("Attempt to correct your mount if it appears to be glitched.");
					break;
				case FixMeFlags.Pets:
				{
					html.AppendLine("All pets will be stabled or teleported and your follower count will be normalized.");
					html.Append("If mounted, the mount will not be included.");
				}
				break;
				case FixMeFlags.Equip:
					html.Append("All equipment will be validated, any invalid equipment will be unequipped.");
					break;
				case FixMeFlags.Gumps:
					html.Append("All open gumps will be refreshed.");
					break;
				case FixMeFlags.Tags:
					html.Append("All character and equipment attribute tags will be refreshed.");
					break;
				case FixMeFlags.Skills:
					html.Append("All skills will be normalized if they are detected as invalid.");
					break;
				case FixMeFlags.Quests:
					html.Append("All quests will be repaired if they are detected as invalid.");
					break;
			}

			return html.ToString();
		}

		public static void FixMe(this Mobile m, FixMeFlags flags)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			var oldFlags = flags;

			if (ResolveFlags != null)
			{
				ResolveFlags(m, ref flags);
			}

			if (flags.HasFlag(FixMeFlags.Mount) && OnFixMount != null)
			{
				OnFixMount(m);
				m.SendMessage(0x55, "Your mount has been validated.");
			}
			else if (oldFlags.HasFlag(FixMeFlags.Mount))
			{
				m.SendMessage(0x22, "Fixing mounts is currently unavailable.");
			}

			if (flags.HasFlag(FixMeFlags.Pets) && OnFixPets != null && m is PlayerMobile)
			{
				OnFixPets((PlayerMobile)m);
				m.SendMessage(
					0x55,
					"Your pets have been stabled or teleported to you and your follower count has been normalized, it is now {0}.",
					m.Followers);
			}
			else if (oldFlags.HasFlag(FixMeFlags.Pets))
			{
				m.SendMessage(0x22, "Fixing pets is currently unavailable.");
			}

			if (flags.HasFlag(FixMeFlags.Equip) && OnFixEquip != null && m is PlayerMobile)
			{
				OnFixEquip((PlayerMobile)m);
				m.SendMessage(0x55, "Your equipment has been validated.");
			}
			else if (oldFlags.HasFlag(FixMeFlags.Equip))
			{
				m.SendMessage(0x22, "Fixing equipment is currently unavailable.");
			}

			if (flags.HasFlag(FixMeFlags.Gumps) && OnFixGumps != null)
			{
				OnFixGumps(m);
				m.SendMessage(0x55, "Your gumps have been refreshed.");
			}
			else if (oldFlags.HasFlag(FixMeFlags.Gumps))
			{
				m.SendMessage(0x22, "Fixing gumps is currently unavailable.");
			}

			if (flags.HasFlag(FixMeFlags.Tags) && OnFixTags != null)
			{
				OnFixTags(m);
				m.SendMessage(0x55, "Your character and equipment tags have been refreshed.");
			}
			else if (oldFlags.HasFlag(FixMeFlags.Tags))
			{
				m.SendMessage(0x22, "Fixing character and equipment tags is currently unavailable.");
			}

			if (flags.HasFlag(FixMeFlags.Skills) && OnFixSkills != null)
			{
				OnFixSkills(m);
				m.SendMessage(0x55, "Your skills have been normalized.");
			}
			else if (oldFlags.HasFlag(FixMeFlags.Skills))
			{
				m.SendMessage(0x22, "Fixing skills is currently unavailable.");
			}

			if (flags.HasFlag(FixMeFlags.Quests) && OnFixQuests != null && m is PlayerMobile)
			{
				OnFixQuests((PlayerMobile)m);
				m.SendMessage(0x55, "Your quests have been validated.");
			}
			else if (oldFlags.HasFlag(FixMeFlags.Quests))
			{
				m.SendMessage(0x22, "Fixing quests is currently unavailable.");
			}

			if (OnFix != null)
			{
				OnFix(m, flags);
			}

			m.SendMessage(0x55, "FixMe completed! If you still have issues, contact a member of staff.");
		}

		public static void FixMount(Mobile m)
		{
			if (!m.Mounted)
			{
				return;
			}

			var mountItem = m.FindItemOnLayer(Layer.Mount) as IMountItem;

			if (mountItem != null)
			{
				if (mountItem.Mount == null || mountItem.Mount != m.Mount)
				{
					m.RemoveItem(mountItem as Item);
				}
				else if (mountItem.Mount.Rider == null)
				{
					mountItem.Mount.Rider = m;
				}
			}
			else if (m.Mount != null && m.Mount.Rider == null)
			{
				m.Mount.Rider = m;
			}

			m.Delta(MobileDelta.Followers);
		}

		public static void FixPets(PlayerMobile m)
		{
			if (m.AllFollowers == null || m.AllFollowers.Count == 0)
			{
				return;
			}

			BaseCreature pet;

			var count = m.AllFollowers.Count;

			while (--count >= 0)
			{
				pet = m.AllFollowers[count] as BaseCreature;

				if (pet == null || pet.IsStabled)
				{
					continue;
				}

				if (pet.Deleted || !pet.IsControlledBy(m))
				{
					m.AllFollowers.RemoveAt(count);
					continue;
				}

				if (pet == m.Mount || pet.Stable(false))
				{
					continue;
				}

				pet.MoveToWorld(m.Location, m.Map);
				pet.ControlTarget = m;
				pet.ControlOrder = OrderType.Follow;
			}

			m.Followers = m.AllFollowers.OfType<BaseCreature>()
						   .Where(p => !p.IsStabled && p.Map == m.Map)
						   .Aggregate(0, (c, p) => c + p.ControlSlots);

			m.Followers = Math.Max(0, m.Followers);

			m.Delta(MobileDelta.Followers);
		}

		public static void FixEquip(PlayerMobile m)
		{
			m.ValidateEquipment();
		}

		public static void FixGumps(Mobile m)
		{
			if (!m.IsOnline())
			{
				return;
			}

			var gumps = m.NetState.Gumps.ToList();

			foreach (var gump in gumps)
			{
				if (gump is SuperGump)
				{
					((SuperGump)gump).Refresh(true);
				}
				else
				{
					m.NetState.Send(new CloseGump(gump.TypeID, 0));
					m.NetState.RemoveGump(gump);

					gump.OnServerClose(m.NetState);

					m.SendGump(gump);
				}
			}

			gumps.Free(true);
		}

		public static void FixTags(Mobile m)
		{
			m.Items.ForEach(
				item =>
				{
					if (item != null && !item.Deleted)
					{
						item.InvalidateProperties();
					}
				});

			if (m.Backpack != null)
			{
				m.Backpack.InvalidateProperties();
				var list = m.Backpack.FindItemsByType<Item>(true);

				list.ForEach(
					item =>
					{
						if (item != null && !item.Deleted)
						{
							item.InvalidateProperties();
						}
					});
			}

			m.InvalidateProperties();
		}

		public static void FixSkills(Mobile m)
		{
			if (m.Skills == null)
			{
				m.Skills = new Skills(m);
			}

			foreach (var skill in SkillInfo.Table.Select(si => m.Skills[si.SkillID]))
			{
				skill.Normalize();
			}
		}

		public static void FixQuests(PlayerMobile m)
		{
			if (m.Quest == null)
			{
				return;
			}

			if (m.Quest.From == null)
			{
				m.Quest.From = m;
			}

			if (m.Quest.Objectives == null || m.Quest.Objectives.Count == 0 || m.Quest.Conversations == null ||
				m.Quest.Conversations.Count == 0)
			{
				m.Quest.Cancel();
			}
		}
	}

	public sealed class FixMeGump : ListGump<FixMeFlags>
	{
		private static readonly FixMeFlags[] _FixMeFlags = default(FixMeFlags)
			.GetValues<FixMeFlags>()
			.Not(f => f == FixMeFlags.All || f == FixMeFlags.None)
			.ToArray();

		public FixMeGump(Mobile user, Gump parent = null)
			: base(user, parent, title: "Fix Me!", emptyText: "There are no operations to display.")
		{
			Modal = true;
			CanMove = false;
			CanResize = false;
			BlockSpeech = true;
			BlockMovement = true;
		}

		protected override void CompileList(List<FixMeFlags> list)
		{
			list.Clear();
			list.AddRange(_FixMeFlags.Not(f => FixMeCommand.DisabledFlags.HasFlag(f)));

			base.CompileList(list);
		}

		protected override void SelectEntry(GumpButton button, FixMeFlags entry)
		{
			base.SelectEntry(button, entry);

			var html = new StringBuilder();

			html.AppendFormat("This operation will fix your {0}.", entry.ToString().ToLower());
			html.AppendLine();
			html.Append(entry.GetDescription());
			html.AppendLine();
			html.AppendLine("Do you want to continue?");

			Send(
				new ConfirmDialogGump(
					User,
					Refresh(),
					title: "Confirm Operation",
					html: html.ToString(),
					onAccept: b =>
					{
						User.FixMe(entry);
						Refresh(true);
					}));
		}
	}
}
