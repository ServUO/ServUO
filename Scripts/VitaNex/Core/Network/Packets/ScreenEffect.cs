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
using Server.Network;
#endregion

namespace VitaNex.Network
{
	public enum ScreenFX
	{
		FadeOut = 0x00,
		FadeIn = 0x01,
		LightFlash = 0x02,
		FadeInOut = 0x03,
		DarkFlash = 0x04
	}

	public static class ScreenEffects
	{
		public static void Send(this ScreenFX effect, Mobile to)
		{
			if (to == null)
			{
				return;
			}

			switch (effect)
			{
				case ScreenFX.FadeOut:
					to.Send(VNScreenFadeOut.Instance);
					break;
				case ScreenFX.FadeIn:
					to.Send(VNScreenFadeIn.Instance);
					break;
				case ScreenFX.LightFlash:
					to.Send(VNScreenLightFlash.Instance);
					break;
				case ScreenFX.FadeInOut:
					to.Send(VNScreenFadeInOut.Instance);
					break;
				case ScreenFX.DarkFlash:
					to.Send(VNScreenDarkFlash.Instance);
					break;
			}
		}
	}

	public class VNScreenEffect : Packet
	{
		public VNScreenEffect(ScreenEffectType type)
			: base(0x70, 28)
		{
			m_Stream.Write((byte)0x04);
			m_Stream.Fill(8);
			m_Stream.Write((short)type);
			m_Stream.Fill(16);
		}
	}

	public sealed class VNScreenFadeOut : VNScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new VNScreenFadeOut());

		public VNScreenFadeOut()
			: base(ScreenEffectType.FadeOut)
		{ }
	}

	public sealed class VNScreenFadeIn : VNScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new VNScreenFadeIn());

		public VNScreenFadeIn()
			: base(ScreenEffectType.FadeIn)
		{ }
	}

	public sealed class VNScreenFadeInOut : VNScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new VNScreenFadeInOut());

		public VNScreenFadeInOut()
			: base(ScreenEffectType.FadeInOut)
		{ }
	}

	public sealed class VNScreenLightFlash : VNScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new VNScreenLightFlash());

		public VNScreenLightFlash()
			: base(ScreenEffectType.LightFlash)
		{ }
	}

	public sealed class VNScreenDarkFlash : VNScreenEffect
	{
		public static readonly Packet Instance = SetStatic(new VNScreenDarkFlash());

		public VNScreenDarkFlash()
			: base(ScreenEffectType.DarkFlash)
		{ }
	}
}