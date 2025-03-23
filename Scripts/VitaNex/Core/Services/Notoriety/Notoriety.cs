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
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Misc;
#endregion

namespace VitaNex
{
	public delegate T NotorietyHandler<out T>(Mobile x, Mobile y, out bool handled);

	public class NotorietyEntry<T>
	{
		public int Priority { get; set; }
		public NotorietyHandler<T> Handler { get; set; }

		public NotorietyEntry(int priority, NotorietyHandler<T> handler)
		{
			Priority = priority;
			Handler = handler;
		}
	}

	[CoreService("Notoriety", "1.0.0.0", TaskPriority.High)]
	public static class NotoUtility
	{
		public const int Bubble = -1;

		private static NotorietyHandler _NotorietyParent;
		private static AllowBeneficialHandler _BeneficialParent;
		private static AllowHarmfulHandler _HarmfulParent;

		public static NotorietyHandler NotorietyParent => _NotorietyParent ?? (_NotorietyParent = Notoriety.Handler);

		public static AllowBeneficialHandler BeneficialParent => _BeneficialParent ?? (_BeneficialParent = NotorietyHandlers.Mobile_AllowBeneficial);

		public static AllowHarmfulHandler HarmfulParent => _HarmfulParent ?? (_HarmfulParent = NotorietyHandlers.Mobile_AllowHarmful);

		private static readonly List<NotorietyEntry<int>> _NameHandlers = new List<NotorietyEntry<int>>();
		private static readonly List<NotorietyEntry<bool>> _BeneficialHandlers = new List<NotorietyEntry<bool>>();
		private static readonly List<NotorietyEntry<bool>> _HarmfulHandlers = new List<NotorietyEntry<bool>>();

		public static List<NotorietyEntry<int>> NameHandlers => _NameHandlers;
		public static List<NotorietyEntry<bool>> BeneficialHandlers => _BeneficialHandlers;
		public static List<NotorietyEntry<bool>> HarmfulHandlers => _HarmfulHandlers;

        //private static void CSInvoke()
        //{
        //    if (_NotorietyParent == null && Notoriety.Handler != (NotorietyHandler)MobileNotoriety)
        //    {
        //        _NotorietyParent = Notoriety.Handler ?? NotorietyHandlers.MobileNotoriety;
        //    }

        //    if (_BeneficialParent == null && Mobile.AllowBeneficialHandler != (AllowBeneficialHandler)AllowBeneficial)
        //    {
        //        _BeneficialParent = Mobile.AllowBeneficialHandler ?? NotorietyHandlers.Mobile_AllowBeneficial;
        //    }

        //    if (_HarmfulParent == null && Mobile.AllowHarmfulHandler != (AllowHarmfulHandler)AllowHarmful)
        //    {
        //        _HarmfulParent = Mobile.AllowHarmfulHandler ?? NotorietyHandlers.Mobile_AllowHarmful;
        //    }

        //    Notoriety.Handler = MobileNotoriety;
        //    Mobile.AllowBeneficialHandler = AllowBeneficial;
        //    Mobile.AllowHarmfulHandler = AllowHarmful;
        //}

		public static void CSDispose()
		{
			Notoriety.Handler = _NotorietyParent ?? NotorietyHandlers.MobileNotoriety;
			Mobile.AllowBeneficialHandler = _BeneficialParent ?? NotorietyHandlers.Mobile_AllowBeneficial;
			Mobile.AllowHarmfulHandler = _HarmfulParent ?? NotorietyHandlers.Mobile_AllowHarmful;

			_NotorietyParent = null;
			_BeneficialParent = null;
			_HarmfulParent = null;
		}

		public static void RegisterNameHandler(NotorietyHandler<int> handler, int priority = 0)
		{
			UnregisterNameHandler(handler);
			_NameHandlers.Add(new NotorietyEntry<int>(priority, handler));
		}

		public static void UnregisterNameHandler(NotorietyHandler<int> handler)
		{
			_NameHandlers.RemoveAll(e => e.Handler == handler);
		}

		public static void RegisterBeneficialHandler(NotorietyHandler<bool> handler, int priority = 0)
		{
			UnregisterBeneficialHandler(handler);
			_BeneficialHandlers.Add(new NotorietyEntry<bool>(priority, handler));
		}

		public static void UnregisterBeneficialHandler(NotorietyHandler<bool> handler)
		{
			_BeneficialHandlers.RemoveAll(e => e.Handler == handler);
		}

		public static void RegisterHarmfulHandler(NotorietyHandler<bool> handler, int priority = 0)
		{
			UnregisterHarmfulHandler(handler);
			_HarmfulHandlers.Add(new NotorietyEntry<bool>(priority, handler));
		}

		public static void UnregisterHarmfulHandler(NotorietyHandler<bool> handler)
		{
			_HarmfulHandlers.RemoveAll(e => e.Handler == handler);
		}

		public static bool AllowBeneficial(Mobile a, Mobile b)
		{
			if (_BeneficialParent == null)
			{
				_BeneficialParent = NotorietyHandlers.Mobile_AllowBeneficial;
			}

			if (a == null || a.Deleted || b == null || b.Deleted)
			{
				return _BeneficialParent(a, b);
			}

			foreach (var handler in _BeneficialHandlers.Where(e => e.Handler != null)
													   .OrderByDescending(e => e.Priority)
													   .Select(e => e.Handler))
			{
				var result = handler(a, b, out var handled);

				if (handled)
				{
					return result;
				}
			}

			return _BeneficialParent(a, b);
		}

#if ServUO
		public static bool AllowHarmful(Mobile a, IDamageable b)
		{
			if (b is Mobile)
			{
				return AllowHarmful(a, (Mobile)b);
			}

			if (_HarmfulParent == null)
			{
				_HarmfulParent = NotorietyHandlers.Mobile_AllowHarmful;
			}

			return _HarmfulParent(a, b);
		}
#endif

		public static bool AllowHarmful(Mobile a, Mobile b)
		{
			if (_HarmfulParent == null)
			{
				_HarmfulParent = NotorietyHandlers.Mobile_AllowHarmful;
			}

			if (a == null || a.Deleted || b == null || b.Deleted)
			{
				return _HarmfulParent(a, b);
			}

			foreach (var handler in _HarmfulHandlers.Where(e => e.Handler != null)
													.OrderByDescending(e => e.Priority)
													.Select(e => e.Handler))
			{
				var result = handler(a, b, out var handled);

				if (handled)
				{
					return result;
				}
			}

			return _HarmfulParent(a, b);
		}

#if ServUO
		public static int MobileNotoriety(Mobile a, IDamageable b)
		{
			if (b is Mobile)
			{
				return MobileNotoriety(a, (Mobile)b);
			}

			if (_NotorietyParent == null)
			{
				_NotorietyParent = NotorietyHandlers.MobileNotoriety;
			}

			return _NotorietyParent(a, b);
		}
#endif

		public static int MobileNotoriety(Mobile a, Mobile b)
		{
			if (_NotorietyParent == null)
			{
				_NotorietyParent = NotorietyHandlers.MobileNotoriety;
			}

			if (a == null || a.Deleted || b == null || b.Deleted)
			{
				return _NotorietyParent(a, b);
			}

			foreach (var handler in _NameHandlers.Where(e => e.Handler != null)
												 .OrderByDescending(e => e.Priority)
												 .Select(e => e.Handler))
			{
				var result = handler(a, b, out var handled);

				if (handled)
				{
					if (result <= Bubble)
					{
						break;
					}

					return result;
				}
			}

			return _NotorietyParent(a, b);
		}

		public static bool Resolve<T1, T2>(Mobile a, Mobile b, out T1 x, out T2 y)
			where T1 : Mobile
			where T2 : Mobile
		{
			x = null;
			y = null;

			if (a == null || a.Deleted || b == null || b.Deleted)
			{
				return false;
			}

			if (!a.IsControlled(out x))
			{
				x = a as T1;
			}

			if (!b.IsControlled(out y))
			{
				y = b as T2;
			}

			return x != null && y != null;
		}
	}
}
