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
using System.Drawing;

using Server;
using Server.Spells;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.CastBars
{
	public class SpellCastBar : ProgressBarGump
	{
		private bool _Initialized;
		private Timer _CloseTimer;

		private bool _Casting;

		public Spell ActiveSpell { get; set; }
		public DateTime CastStart { get; set; }
		public bool Preview { get; set; }

		public SpellCastBar(Mobile user, int? x = null, int? y = null, Action<ProgressBarGump, double> valueChanged = null)
			: base(user, x: x, y: y, text: "Casting", valueChanged: valueChanged)
		{
			CanMove = true;

			UseSounds = false;

			AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
			AutoRefresh = true;
		}

		protected override void Compile()
		{
			if (!_Initialized)
			{
				Width = SpellCastBars.CMOptions.GumpWidth;
				Height = SpellCastBars.CMOptions.GumpHeight;
				BackgroundID = SpellCastBars.CMOptions.GumpBackground;
				ForegroundID = SpellCastBars.CMOptions.GumpForeground;
				Padding = SpellCastBars.CMOptions.GumpPadding;
				Flow = SpellCastBars.CMOptions.GumpFlow;
				TextColor = Color.FromKnownColor(SpellCastBars.CMOptions.GumpTextColor);
				DisplayPercent = SpellCastBars.CMOptions.GumpDisplayPercent;

				_Initialized = true;
			}

			UpdateSpell();

			base.Compile();
		}

		public override void Reset()
		{
			base.Reset();

			if (_CloseTimer != null)
			{
				_CloseTimer.Running = false;
				_CloseTimer = null;
			}

			ActiveSpell = null;
			MaxValue = 100;

			if (Preview)
			{
				Text = "Placeholder";
				InternalValue = 50;
			}
			else
			{
				Text = "Casting";
				InternalValue = 0;
			}
		}

		public void UpdateSpell()
		{
			if (Preview)
			{
				Reset();
				return;
			}

			_Casting = false;

			if (ActiveSpell == null || ActiveSpell != User.Spell)
			{
				ActiveSpell = User.Spell as Spell;

				if (ActiveSpell == null)
				{
					return;
				}

				if (_CloseTimer != null)
				{
					_CloseTimer.Running = false;
					_CloseTimer = null;
				}

				Text = ActiveSpell.Name;
				CastStart = DateTime.UtcNow;
				MaxValue = ActiveSpell.GetCastDelay().TotalMilliseconds - 100.0;

				if (SpellCastBars.CMOptions.ModuleDebug)
				{
					SpellCastBars.CMOptions.ToConsole(
						"'{0}', {1}: [{2}/{3}] ({4})",
						Text,
						CastStart,
						InternalValue,
						MaxValue,
						PercentComplete);
				}

				_Casting = ActiveSpell.IsCasting;
			}
			else
			{
				_Casting = true;
			}

			if (!_Casting)
			{
				return;
			}

			InternalValue = (DateTime.UtcNow - CastStart).TotalMilliseconds;

			if (SpellCastBars.CMOptions.ModuleDebug)
			{
				SpellCastBars.CMOptions.ToConsole("[{0} / {1}] ({2})", InternalValue, MaxValue, PercentComplete);
			}
		}

		private void Finish()
		{
			if ((!IsOpen && !Hidden) || (_CloseTimer != null && _CloseTimer.Running))
			{
				return;
			}

			InternalValue = MaxValue;

			if (_CloseTimer != null)
			{
				_CloseTimer.Running = true;
			}
			else
			{
				_CloseTimer = Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => Close());
			}
		}

		protected override void OnSend()
		{
			base.OnSend();

			if (!Preview && (ActiveSpell == null || Completed))
			{
				Finish();
			}
		}

		public override void Close(bool all)
		{
			_Initialized = Preview = false;

			Reset();

			base.Close(all);
		}

		protected override bool CanAutoRefresh()
		{
			return ActiveSpell != null && base.CanAutoRefresh();
		}

		public override string FormatText(bool html = false)
		{
			return SpellCastBars.CMOptions.GumpDisplayText ? base.FormatText(html) : String.Empty;
		}
	}
}