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
using System.Text;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
using VitaNex.Targets;
#endregion

namespace VitaNex.Modules.Voting
{
	public sealed class VoteAdminGump : ListGump<IVoteSite>
	{
		public VoteAdminGump(Mobile user, Gump parent = null)
			: base(user, parent, emptyText: "There are no sites to display.", title: "Voting Control Panel")
		{
			ForceRecompile = true;
		}

		public override string GetSearchKeyFor(IVoteSite key)
		{
			return key != null ? key.Name : base.GetSearchKeyFor(null);
		}

		protected override int GetLabelHue(int index, int pageIndex, IVoteSite entry)
		{
			return entry != null ? (entry.Enabled ? HighlightHue : ErrorHue) : base.GetLabelHue(index, pageIndex, null);
		}

		protected override string GetLabelText(int index, int pageIndex, IVoteSite entry)
		{
			return entry != null ? String.Format("{0}", entry.Name) : base.GetLabelText(index, pageIndex, null);
		}

		protected override void CompileList(List<IVoteSite> list)
		{
			list.Clear();
			list.AddRange(Voting.VoteSites.Values);
			base.CompileList(list);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (User.AccessLevel >= Voting.Access)
			{
				list.AppendEntry(new ListGumpEntry("System Options", OpenConfig, HighlightHue));
				list.AppendEntry(new ListGumpEntry("Add Vote Site", AddSite, HighlightHue));
			}

			list.AppendEntry(new ListGumpEntry("View Profiles", ShowProfiles));
			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));
			base.CompileMenuOptions(list);
		}

		protected override void SelectEntry(GumpButton button, IVoteSite entry)
		{
			base.SelectEntry(button, entry);

			var opts = new MenuGumpOptions();

			if (User.AccessLevel >= Voting.Access)
			{
				opts.AppendEntry(
					new ListGumpEntry(
						"Options",
						b =>
						{
							Refresh();

							var pg = new PropertiesGump(User, Selected)
							{
								X = b.X,
								Y = b.Y
							};
							User.SendGump(pg);
						},
						HighlightHue));

				opts.AppendEntry(
					new ListGumpEntry(
						entry.Enabled ? "Disable" : "Enable",
						b1 =>
						{
							entry.Enabled = !entry.Enabled;
							Refresh(true);
						},
						entry.Enabled ? ErrorHue : HighlightHue));

				opts.AppendEntry(new ListGumpEntry("Delete", DeleteSite, ErrorHue));

				opts.AppendEntry(
					new ListGumpEntry(
						"Add To Stone",
						b =>
						{
							Minimize(b);
							User.SendMessage(0x55, "Select a compatible Voting Stone...");
							ItemSelectTarget<VotingStone>.Begin(
								User,
								(u, s) =>
								{
									if (s != null && !s.Deleted)
									{
										s.SiteUID = entry.UID;
										User.SendMessage(0x55, "The Voting Stone site was changed to {0}.", entry.Name);
									}
									else
									{
										User.SendMessage(0x22, "That is not a compatible Voting Stone.");
									}

									Maximize();
								},
								u => Maximize());
						}));

				opts.AppendEntry(
					new ListGumpEntry(
						"Create Stone",
						b => Send(
							new ConfirmDialogGump(User, this)
							{
								Title = "Create Voting Stone?",
								Html = "You didn't select a compatible Voting Stone.\nDo you want to create one in your pack?\n" +
									   "Click OK to create a new Voting Stone for this site.",
								AcceptHandler = ab =>
								{
									var stone = new VotingStone(entry.UID);

									if (User.AddToBackpack(stone))
									{
										User.SendMessage(0x22, "The new Voting Stone was placed in your pack.");
									}
									else
									{
										stone.MoveToWorld(User.Location, User.Map);
										User.SendMessage(0x22, "The new Voting Stone was placed at your feet.");
									}
								}
							})));

				opts.AppendEntry(new ListGumpEntry("Cancel", b => { }));
			}

			Send(new MenuGump(User, Refresh(), opts, button));
		}

		private void DeleteSite()
		{
			if (Selected == null)
			{
				return;
			}

			Send(
				new ConfirmDialogGump(User, Refresh())
				{
					Title = "Delete Site?",
					Html = "All data associated with this site will be deleted.\n" +
						   "This action can not be reversed!\nDo you want to continue?",
					AcceptHandler = OnDeleteSiteConfirm
				});
		}

		private void OnDeleteSiteConfirm(GumpButton button)
		{
			if (Selected != null)
			{
				Selected.Delete();
			}

			Refresh();
		}

		private void AddSite(GumpButton btn)
		{
			var opts = new MenuGumpOptions();

			Voting.SiteTypes.ForEach(t => opts.AppendEntry(new ListGumpEntry(t.Name, b => OnAddSite(t))));

			Refresh();
			Send(new MenuGump(User, btn.Parent, opts, btn));
		}

		private void OnAddSite(Type t)
		{
			var site = VitaNexCore.TryCatchGet(() => t.CreateInstance<IVoteSite>());

			if (site != null && !Voting.VoteSites.ContainsKey(site.UID))
			{
				Voting.VoteSites.Add(site.UID, site);
			}

			Refresh(true);
		}

		private void OpenConfig(GumpButton btn)
		{
			Minimize();

			var p = new PropertiesGump(User, Voting.CMOptions)
			{
				X = X + btn.X,
				Y = Y + btn.Y
			};

			User.SendGump(p);
		}

		private void ShowProfiles(GumpButton button)
		{
			if (User != null && !User.Deleted)
			{
				Send(new VoteProfilesGump(User, Hide()));
			}
		}

		private void ShowHelp(GumpButton button)
		{
			if (User == null || User.Deleted)
			{
				return;
			}

			var sb = VoteGumpUtility.GetHelpText(User);
			Send(
				new HtmlPanelGump<StringBuilder>(User, Refresh())
				{
					Selected = sb,
					Html = sb.ToString(),
					Title = "Voting Help",
					HtmlColor = Color.SkyBlue
				});
		}
	}
}