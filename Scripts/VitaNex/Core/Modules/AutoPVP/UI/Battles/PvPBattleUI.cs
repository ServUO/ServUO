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
using Server.Gumps;
using Server.Mobiles;

using VitaNex.Schedules;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleUI : HtmlPanelGump<PvPBattle>
	{
		public bool UseConfirmDialog { get; set; }

		public PvPBattleUI(Mobile user, Gump parent = null, PvPBattle battle = null, bool useConfirm = true)
			: base(user, parent, emptyText: "No battle selected.", title: "PvP Battle Overview", selected: battle)
		{
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;
			//AutoRefresh = true;
		}

		protected override void Compile()
		{
			base.Compile();

			if (Selected == null || Selected.Deleted)
			{
				return;
			}

			Html = String.Empty;

			if (User.AccessLevel >= AutoPvP.Access)
			{
				var errors = new List<string>();

				if (!Selected.Validate(User, errors))
				{
					Html += "*This battle has failed validation*\n\n".WrapUOHtmlBig().WrapUOHtmlColor(Color.IndianRed, false);
					Html += String.Join("\n", errors).WrapUOHtmlColor(Color.Yellow);
					Html += "\n\n";
				}
			}

			Html += Selected.ToHtmlString(User);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (Selected != null && !Selected.Deleted)
			{
				if (User.AccessLevel >= AutoPvP.Access)
				{
					list.AppendEntry(
						"Edit Options",
						b =>
						{
							Minimize();

							var pg = new PropertiesGump(User, Selected)
							{
								X = b.X,
								Y = b.Y
							};

							User.SendGump(pg);
						},
						HighlightHue);

					list.AppendEntry(
						"Edit Advanced Options",
						b =>
						{
							Minimize();

							var pg = new PropertiesGump(User, Selected.Options)
							{
								X = b.X,
								Y = b.Y
							};

							User.SendGump(pg);
						},
						HighlightHue);

					if (Selected.State == PvPBattleState.Internal)
					{
						list.AppendEntry(
							"Edit Spectate Region",
							b =>
							{
								if (Selected.SpectateRegion == null)
								{
									Selected.SpectateRegion = RegionExtUtility.Create<PvPSpectateRegion>(Selected);
								}

								new PvPSpectateBoundsGump(User, Selected, Hide(true)).Send();
							},
							HighlightHue);

						list.AppendEntry(
							"Edit Battle Region",
							b =>
							{
								if (Selected.BattleRegion == null)
								{
									Selected.BattleRegion = RegionExtUtility.Create<PvPBattleRegion>(Selected);
								}

								new PvPBattleBoundsGump(User, Selected, Hide(true)).Send();
							},
							HighlightHue);
					}

					list.AppendEntry(
						"Edit Doors",
						b => new PvPDoorsUI(User, Selected, Hide(true), UseConfirmDialog).Send(),
						HighlightHue);

					list.AppendEntry(
						"Edit Description",
						b => new TextInputPanelGump<PvPBattle>(User, Hide(true))
						{
							Title = "Battle Description (HTML/BBC Supported)",
							Input = Selected.Description,
							Limit = 1000,
							Callback = s =>
							{
								s = s.ParseBBCode();

								if (!String.IsNullOrWhiteSpace(s))
								{
									Selected.Description = s;
								}

								Refresh(true);
							}
						}.Send(),
						HighlightHue);
				}

				list.AppendEntry(
					"View Schedule",
					b => new ScheduleOverviewGump(User, Selected.Schedule, Hide(true)).Send(),
					User.AccessLevel >= AutoPvP.Access ? HighlightHue : TextHue);

				if (User.AccessLevel >= AutoPvP.Access)
				{
					list.AppendEntry(
						"View Rules/Restrictions",
						b =>
						{
							var opts = new MenuGumpOptions();

							opts.AppendEntry(
								"Inherit Rules/Restrictions",
								b2 =>
								{
									var opts2 = new MenuGumpOptions();

									foreach (var ba in AutoPvP.Battles.Values.Where(ba => ba != Selected))
									{
										opts2.AppendEntry(
											ba.Name,
											() =>
											{
												Selected.Options.Rules.CopyFrom(ba.Options.Rules);

												Selected.Options.Restrictions.Items.List = //
													new Dictionary<Type, bool>(ba.Options.Restrictions.Items.List);

												Selected.Options.Restrictions.Pets.List = //
													new Dictionary<Type, bool>(ba.Options.Restrictions.Pets.List);

												Selected.Options.Restrictions.Spells.List = //
													new Dictionary<Type, bool>(ba.Options.Restrictions.Spells.List);

												Selected.Options.Restrictions.Skills.List = //
													new Dictionary<int, bool>(ba.Options.Restrictions.Skills.List);

												Refresh(true);
											});
									}

									new MenuGump(User, this, opts2, b).Send();
								});

							opts.AppendEntry(
								new ListGumpEntry(
									"Rules",
									mb =>
									{
										Refresh();

										var g = new PropertiesGump(User, Selected.Options.Rules)
										{
											X = mb.X,
											Y = mb.Y
										};

										User.SendGump(g);
									}));

							opts.AppendEntry(
								"Items",
								mb => new PvPRestrictItemsListGump(User, Selected.Options.Restrictions.Items, Hide(true)).Send());

							opts.AppendEntry(
								"Pets",
								mb => new PvPRestrictPetsListGump(User, Selected.Options.Restrictions.Pets, Hide(true)).Send());

							opts.AppendEntry(
								"Skills",
								mb => new PvPRestrictSkillsListGump(User, Selected.Options.Restrictions.Skills, Hide(true)).Send());

							opts.AppendEntry(
								"Spells",
								mb => new PvPRestrictSpellsListGump(User, Selected.Options.Restrictions.Spells, Hide(true)).Send());

							new MenuGump(User, this, opts, b).Send();
						},
						User.AccessLevel >= AutoPvP.Access ? HighlightHue : TextHue);

					list.AppendEntry(
						"Reset Statistics",
						b =>
						{
							if (UseConfirmDialog)
							{
								new ConfirmDialogGump(User, this)
								{
									Title = "Reset Battle Statistics?",
									Html = "All data associated with the battle statistics will " +
										   "be transferred to player profiles then cleared.\nThis action can not be reversed!\n" +
										   "Do you want to continue?",
									AcceptHandler = OnConfirmResetStatistics,
									CancelHandler = Refresh
								}.Send();
							}
							else
							{
								OnConfirmResetStatistics(b);
							}
						},
						HighlightHue);

					if (Selected.State == PvPBattleState.Internal)
					{
						if (Selected.Validate(User))
						{
							list.AppendEntry(
								"Publish",
								b =>
								{
									Selected.State = PvPBattleState.Queueing;

									Refresh(true);
								},
								HighlightHue);
						}
					}
					else
					{
						list.AppendEntry(
							"Internalize",
							b =>
							{
								Selected.State = PvPBattleState.Internal;

								Refresh(true);
							},
							HighlightHue);

						if (!Selected.Hidden)
						{
							if (Selected.Validate(User))
							{
								list.AppendEntry(
									"Hide",
									b =>
									{
										Selected.Hidden = true;

										Refresh(true);
									},
									HighlightHue);
							}
						}
						else
						{
							list.AppendEntry(
								"Unhide",
								b =>
								{
									Selected.Hidden = false;

									Refresh(true);
								},
								HighlightHue);
						}
					}

					list.AppendEntry(
						"Delete",
						b =>
						{
							if (UseConfirmDialog)
							{
								new ConfirmDialogGump(User, this)
								{
									Title = "Delete Battle?",
									Html = "All data associated with this battle will be deleted.\n" +
										   "This action can not be reversed!\nDo you want to continue?",
									AcceptHandler = OnConfirmDeleteBattle,
									CancelHandler = Refresh
								}.Send();
							}
							else
							{
								OnConfirmDeleteBattle(b);
							}
						},
						HighlightHue);
				}

				list.AppendEntry(
					"Command List",
					b =>
					{
						var html = new StringBuilder();

						Selected.GetHtmlCommandList(User, html);

						new HtmlPanelGump<PvPBattle>(User, this)
						{
							Title = "Command List",
							Html = html.ToString(),
							Selected = Selected
						}.Send();
					});

				var profile = AutoPvP.EnsureProfile(User as PlayerMobile);

				if (profile != null && !profile.Deleted)
				{
					if (profile.IsSubscribed(Selected))
					{
						list.AppendEntry(
							"Unsubscribe",
							b =>
							{
								profile.Unsubscribe(Selected);

								User.SendMessage("You have unsubscribed from {0} notifications.", Selected.Name);

								Refresh(true);
							});
					}
					else
					{
						list.AppendEntry(
							"Subscribe",
							b =>
							{
								if (UseConfirmDialog)
								{
									new ConfirmDialogGump(User, this)
									{
										Title = "Subscriptions",
										Html = "Subscribing to a battle allows you to see its world broadcast notifications.\n\n" +
											   "Do you want to subscribe to " + Selected.Name + "?",
										AcceptHandler = OnConfirmSubscribe,
										CancelHandler = Refresh
									}.Send();
								}
								else
								{
									OnConfirmSubscribe(b);
								}
							});
					}
				}

				if (User is PlayerMobile)
				{
					var user = (PlayerMobile)User;

					if (Selected.IsParticipant(user))
					{
						list.AppendEntry("Quit & Leave", b => Selected.Quit(user, true));
					}
					else
					{
						if (Selected.IsQueued(user))
						{
							list.AppendEntry("Leave Queue", b => Selected.Dequeue(user));
						}
						else if (Selected.CanQueue(user))
						{
							list.AppendEntry("Join Queue", b => Selected.Enqueue(user));
						}

						if (Selected.IsSpectator(user))
						{
							list.AppendEntry("Leave Spectators", b => Selected.RemoveSpectator(user, true));
						}
						else if (Selected.CanSpectate(user))
						{
							list.AppendEntry("Join Spectators", b => Selected.AddSpectator(user, true));
						}
					}
				}
			}

			base.CompileMenuOptions(list);
		}

		protected virtual void OnConfirmSubscribe(GumpButton button)
		{
			if (Selected == null || Selected.Deleted)
			{
				Close();
				return;
			}

			var profile = AutoPvP.EnsureProfile(User as PlayerMobile);

			if (profile != null && !profile.Deleted)
			{
				profile.Subscribe(Selected);

				User.SendMessage("You have subscribed to {0} notifications.", Selected.Name);

				Refresh(true);
			}
		}

		protected virtual void OnConfirmResetStatistics(GumpButton button)
		{
			if (Selected == null || Selected.Deleted)
			{
				Close();
				return;
			}

			Selected.ResetStatistics();

			Refresh(true);
		}

		protected virtual void OnConfirmDeleteBattle(GumpButton button)
		{
			if (Selected == null || Selected.Deleted)
			{
				Close();
				return;
			}

			Selected.Delete();

			Close();
		}
	}
}