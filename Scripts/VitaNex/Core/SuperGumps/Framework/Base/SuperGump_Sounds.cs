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

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		public static int DefaultSendSound = -1;
		public static int DefaultHideSound = -1;
		public static int DefaultRefreshSound = -1;
		public static int DefaultCloseSound = 999;
		public static int DefaultClickSound = 1235;
		public static int DefaultDoubleClickSound = 74;

		public virtual int SendSound { get; set; }
		public virtual int HideSound { get; set; }
		public virtual int RefreshSound { get; set; }
		public virtual int CloseSound { get; set; }
		public virtual int ClickSound { get; set; }
		public virtual int DoubleClickSound { get; set; }

		public virtual bool UseSounds { get; set; }

		protected virtual void InitSounds()
		{
			SendSound = DefaultSendSound;
			HideSound = DefaultHideSound;
			RefreshSound = DefaultRefreshSound;
			CloseSound = DefaultCloseSound;
			ClickSound = DefaultClickSound;
			DoubleClickSound = DefaultDoubleClickSound;
		}

		protected virtual void PlaySendSound()
		{
			PlaySound(SendSound);
		}

		protected virtual void PlayHideSound()
		{
			PlaySound(HideSound);
		}

		protected virtual void PlayRefreshSound()
		{
			PlaySound(RefreshSound);
		}

		protected virtual void PlayCloseSound()
		{
			PlaySound(CloseSound);
		}

		protected virtual void PlayClickSound()
		{
			PlaySound(ClickSound);
		}

		protected virtual void PlayDoubleClickSound()
		{
			PlaySound(DoubleClickSound);
		}

		public void PlaySound(int soundID)
		{
			PlaySound(soundID, false);
		}

		public virtual void PlaySound(int soundID, bool loud)
		{
			if (!UseSounds)
			{
				return;
			}

			if (loud)
			{
				User.PlaySound(soundID);
			}
			else
			{
				User.SendSound(soundID);
			}
		}
	}
}