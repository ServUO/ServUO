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
using System.IO;
using System.Linq;
using System.Net;

using Server;

using VitaNex.Crypto;
#endregion

namespace VitaNex.IO
{
	public sealed class VirtualAsset : Grid<Color>, IEquatable<VirtualAsset>
	{
		private static readonly object _CacheLock = new object();

		public static readonly VirtualAsset Empty = new VirtualAsset();

		public static Dictionary<string, VirtualAsset> AssetCache { get; private set; }

		public static int CacheCapacity { get; set; }

		static VirtualAsset()
		{
			AssetCache = new Dictionary<string, VirtualAsset>(CacheCapacity = 512);
		}

		public static bool IsNullOrEmpty(VirtualAsset asset)
		{
			return asset == null || asset == Empty || asset.Hash == Empty.Hash || asset.Capacity == 0 || asset.Count == 0;
		}

		public static VirtualAsset CreateInstance(FileInfo file)
		{
			return CreateInstance(file, true);
		}

		public static VirtualAsset CreateInstance(FileInfo file, bool cache)
		{
			return CreateInstance(file, cache, false);
		}

		public static VirtualAsset CreateInstance(FileInfo file, bool cache, bool reload)
		{
			if (file == null || !file.Exists)
			{
				return Empty;
			}

			return VitaNexCore.TryCatchGet(
				() =>
				{
					var hash = CryptoGenerator.GenString(CryptoHashType.MD5, file.FullName);

					VirtualAsset a;

					lock (_CacheLock)
					{
						if (!AssetCache.TryGetValue(hash, out a))
						{
							a = Empty;
						}
					}

					if (reload || IsNullOrEmpty(a))
					{
						using (var img = new Bitmap(file.FullName, true))
						{
							a = new VirtualAsset(file, img);
						}
					}

					if (IsNullOrEmpty(a))
					{
						return Empty;
					}

					lock (_CacheLock)
					{
						if (cache)
						{
							AssetCache[a.Hash] = a;
						}
						else
						{
							AssetCache.Remove(a.Hash);
						}

						if (AssetCache.Count > CacheCapacity)
						{
							AssetCache.Pop();
						}
					}

					return a;
				});
		}

		public static bool IsValidAsset(string path)
		{
			if (String.IsNullOrWhiteSpace(path))
			{
				return false;
			}

			path = path.ToLower();

			return path.EndsWith("bmp") || path.EndsWith("jpg") || path.EndsWith("jpeg") || path.EndsWith("png") ||
				   path.EndsWith("gif") || path.EndsWith("tiff") || path.EndsWith("exif");
		}

		public static VirtualAsset LoadAsset(string path)
		{
			if (!IsValidAsset(path))
			{
				return Empty;
			}

			if (!Insensitive.StartsWith(path, "http://") && !Insensitive.StartsWith(path, "https://"))
			{
				return LoadAsset(new FileInfo(path));
			}

			return LoadAsset(new Uri(path));
		}

		public static VirtualAsset LoadAsset(Uri url)
		{
			if (url == null)
			{
				return Empty;
			}

			return VitaNexCore.TryCatchGet(
				() =>
				{
					if (!IsValidAsset(url.LocalPath))
					{
						return Empty;
					}

					var file = new FileInfo(VitaNexCore.DataDirectory + "/Assets/" + url.Host + "/" + url.LocalPath);

					if (!file.Exists)
					{
						file = file.EnsureFile();

						using (var c = new WebClient())
						{
							c.DownloadFile(url, file.FullName);
						}
					}

					return LoadAsset(file);
				});
		}

		public static VirtualAsset LoadAsset(FileInfo file)
		{
			if (file == null || !IsValidAsset(file.FullName))
			{
				return Empty;
			}

			return VitaNexCore.TryCatchGet(
				() =>
				{
					var asset = CreateInstance(file);

					if (IsNullOrEmpty(asset))
					{
						return Empty;
					}

					if (file.Exists)
					{
						return asset;
					}

					file.EnsureFile();

					using (var img = new Bitmap(asset.Width, asset.Height))
					{
						asset.ForEach(img.SetPixel);
						img.Save(file.FullName);
					}

					return asset;
				});
		}

		public string File { get; private set; }
		public string Name { get; private set; }
		public string Hash { get; private set; }

		private VirtualAsset()
			: base(0, 0)
		{
			File = String.Empty;
			Name = String.Empty;
			Hash = CryptoGenerator.GenString(CryptoHashType.MD5, File);

			DefaultValue = Color.Transparent;
		}

		private VirtualAsset(FileInfo file, Bitmap img)
			: base(img.Width, img.Height)
		{
			File = file.FullName;
			Name = Path.GetFileName(File);
			Hash = CryptoGenerator.GenString(CryptoHashType.MD5, File);

			DefaultValue = Color.Transparent;
			SetAllContent(img.GetPixel);
		}

		public override string ToString()
		{
			return File;
		}

		public override int GetHashCode()
		{
			return Hash.Aggregate(Hash.Length, (h, c) => unchecked((h * 397) ^ c));
		}

		public override bool Equals(object obj)
		{
			return obj is VirtualAsset && Equals((VirtualAsset)obj);
		}

		public bool Equals(VirtualAsset other)
		{
			return !ReferenceEquals(other, null) && Hash == other.Hash;
		}

		public static bool operator ==(VirtualAsset l, VirtualAsset r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(VirtualAsset l, VirtualAsset r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}