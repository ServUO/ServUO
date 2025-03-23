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
using System.IO;
using System.Threading.Tasks;

using VitaNex.IO;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		private static readonly bool[] _EmptyBools = new bool[0];
		private static readonly string[] _EmptyStrings = new string[0];

		protected virtual string[] PreloadAssets => _EmptyStrings;

		protected bool[] LoadAssets(string[] paths)
		{
			if (paths == null || paths.Length == 0)
			{
				return _EmptyBools;
			}

			var results = new bool[paths.Length];

			Parallel.For(0, paths.Length, i => results[i] = LoadAsset(paths[i]));

			return results;
		}

		protected bool LoadAsset(string path)
		{
			return !VirtualAsset.IsNullOrEmpty(VirtualAsset.LoadAsset(path));
		}

		protected virtual void InitAssets()
		{
			var assets = PreloadAssets;

			if (assets == null || assets.Length == 0)
			{
				return;
			}

			var results = LoadAssets(assets);

			if (results == null || results.Length == 0)
			{
				return;
			}

			for (var i = 0; i < assets.Length && i < results.Length; i++)
			{
				if (!results[i])
				{
					VitaNexCore.ToConsole("Warning: {0} failed to pre-load asset '{1}'", GetType(), assets[i]);
				}
			}
		}

		public virtual void AddAsset(int x, int y, string path)
		{
			Add(new GumpAsset(x, y, path));
		}

		public virtual void AddAsset(int x, int y, Uri url)
		{
			Add(new GumpAsset(x, y, url));
		}

		public virtual void AddAsset(int x, int y, FileInfo file)
		{
			Add(new GumpAsset(x, y, file));
		}

		public virtual void AddAsset(int x, int y, VirtualAsset asset)
		{
			Add(new GumpAsset(x, y, asset));
		}
	}
}