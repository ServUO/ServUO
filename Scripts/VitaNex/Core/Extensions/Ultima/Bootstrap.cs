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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections.Generic;
using System.Reflection;

using Server;

using VitaNex;
#endregion

namespace Ultima
{
	public static class Bootstrap
	{
#if ServUO && !ServUOX
		[CallPriority(1)]
		public static void Configure()
		{
			foreach (var path in Core.DataDirectories)
			{
				Files.SetMulPath(path);
			}
		}
#else
		private static readonly Dictionary<string, Type> _Modules = new Dictionary<string, Type>();

		public static Assembly UltimaSDK { get; private set; }

		public static bool Loaded { get; private set; }
		public static bool Warned { get; private set; }

		static Bootstrap()
		{
			Load();
		}

		private static void Load()
		{
			if (Loaded)
			{
				return;
			}

			try
			{
				UltimaSDK = Assembly.LoadFrom("Ultima.dll");

				Loaded = true;
				Warned = false;
			}
			catch (Exception e)
			{
				UltimaSDK = null;

				VitaNexCore.ToConsole("Could not load Ultima.dll");

				if (!Warned)
				{
					VitaNexCore.ToConsole(e);

					Warned = true;
				}
			}

			if (Loaded)
			{
				foreach (var path in Core.DataDirectories)
				{
					Invoke("Files", "SetMulPath", path);
				}
			}
		}

		private static Type GetModule(string name)
		{
			Load();

			if (!Loaded)
			{
				return null;
			}

			try
			{
				if (!_Modules.TryGetValue(name, out var type))
				{
					_Modules[name] = type = UltimaSDK.GetType($"Ultima.{name}");
				}

				return type;
			}
			catch (Exception e)
			{
				VitaNexCore.ToConsole($"Could not find Ultima.{name}:");
				VitaNexCore.ToConsole(e);

				return null;
			}
		}

		public static T Invoke<T>(string module, string method, params object[] args)
		{
			if (Invoke(module, method, args) is T o)
			{
				return o;
			}

#if NET48_OR_GREATER
			return default;
#else
			return default(T);
#endif
		}

		public static object Invoke(string module, string method, params object[] args)
		{
			try
			{
				var result = GetModule(module).InvokeMethod(method, args);

				if (result is Exception ex)
				{
					throw ex;
				}

				return result;
			}
			catch (Exception e)
			{
				VitaNexCore.ToConsole($"Could not invoke Ultima.{module}.{method}:");
				VitaNexCore.ToConsole(e);

				return null;
			}
		}
#endif
	}
}
