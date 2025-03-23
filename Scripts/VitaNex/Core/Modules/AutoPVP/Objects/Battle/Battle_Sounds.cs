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
using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		public void SendGlobalSound(int soundID)
		{
			if (!Options.Sounds.Enabled || soundID <= 0)
			{
				return;
			}

			foreach (var m in GetLocalBroadcastList())
			{
				SendSound(m, soundID);
			}
		}

		public void SendSound(int soundID)
		{
			if (Options.Sounds.Enabled && soundID > 0)
			{
				ForEachTeam(t => SendSound(t, soundID));
			}
		}

		public void SendSound(PvPTeam t, int soundID)
		{
			if (Options.Sounds.Enabled && t != null && !t.Deleted && soundID > 0)
			{
				t.SendSound(soundID);
			}
		}

		public virtual void SendSound(Mobile m, int soundID)
		{
			if (Options.Sounds.Enabled && m != null && !m.Deleted && soundID > 0)
			{
				m.SendSound(soundID);
			}
		}

		public void PlayGlobalSound(int soundID)
		{
			if (!Options.Sounds.Enabled || soundID <= 0)
			{
				return;
			}

			foreach (var m in GetLocalBroadcastList())
			{
				PlaySound(m, soundID);
			}
		}

		public void PlaySound(int soundID)
		{
			if (Options.Sounds.Enabled && soundID > 0)
			{
				ForEachTeam(t => PlaySound(t, soundID));
			}
		}

		public void PlaySound(PvPTeam t, int soundID)
		{
			if (Options.Sounds.Enabled && t != null && !t.Deleted && soundID > 0)
			{
				t.PlaySound(soundID);
			}
		}

		public virtual void PlaySound(Mobile m, int soundID)
		{
			if (Options.Sounds.Enabled && m != null && !m.Deleted && soundID > 0)
			{
				m.PlaySound(soundID);
			}
		}
	}
}