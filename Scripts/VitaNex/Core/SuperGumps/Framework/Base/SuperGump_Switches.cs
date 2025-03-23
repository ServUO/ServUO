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

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		private Dictionary<GumpCheck, Action<GumpCheck, bool>> _Switches;

		public Dictionary<GumpCheck, Action<GumpCheck, bool>> Switches
		{
			get => _Switches;
			protected set => _Switches = value;
		}

		public Action<GumpCheck, bool> SwitchHandler { get; set; }

		public new void AddCheck(int x, int y, int offID, int onID, bool state, int switchID)
		{
			AddCheck(x, y, offID, onID, switchID, state, null);
		}

		public void AddCheck(int x, int y, int offID, int onID, bool state)
		{
			AddCheck(x, y, offID, onID, NewSwitchID(), state, null);
		}

		public void AddCheck(int x, int y, int offID, int onID, bool state, Action<GumpCheck, bool> handler)
		{
			AddCheck(x, y, offID, onID, NewSwitchID(), state, handler);
		}

		public void AddCheck(int x, int y, int offID, int onID, int switchID, bool state)
		{
			AddCheck(x, y, offID, onID, switchID, state, null);
		}

		public void AddCheck(int x, int y, int offID, int onID, int switchID, bool state, Action<GumpCheck, bool> handler)
		{
			AddCheck(new GumpCheck(x, y, offID, onID, state, switchID), handler);
		}

		protected void AddCheck(GumpCheck entry, Action<GumpCheck, bool> handler)
		{
			if (entry == null)
			{
				return;
			}

			Switches[entry] = handler;

			Add(entry);
		}

		public virtual void HandleSwitch(GumpCheck entry, bool state)
		{
			if (SwitchHandler != null)
			{
				SwitchHandler(entry, state);
			}
			else if (Switches[entry] != null)
			{
				Switches[entry](entry, state);
			}
		}

		public virtual bool CanDisplay(GumpCheck entry)
		{
			return entry != null;
		}

		public GumpCheck GetSwitchEntry(int switchID)
		{
			return Switches.Keys.FirstOrDefault(check => check.SwitchID == switchID);
		}
	}
}