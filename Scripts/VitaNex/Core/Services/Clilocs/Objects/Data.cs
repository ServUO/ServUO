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
using System.IO;
using System.Text;

using Server;
#endregion

namespace VitaNex
{
	public sealed class ClilocData
	{
		private ClilocInfo _Info;

		public ClilocLNG Language { get; private set; }

		public int Index { get; private set; }
		public long Offset { get; private set; }
		public long Length { get; private set; }

		public ClilocData(ClilocLNG lng, int index, long offset, long length)
		{
			Language = lng;
			Index = index;
			Offset = offset;
			Length = length;
		}

		public void Clear()
		{
			_Info = null;
		}

		public void Load(GenericReader bin)
		{
			bin.Seek(Offset, SeekOrigin.Begin);

			var data = new byte[Length];

			for (long i = 0; i < data.Length; i++)
			{
				data[i] = bin.ReadByte();
			}

			_Info = new ClilocInfo(Language, Index, Encoding.UTF8.GetString(data));
		}

		public ClilocInfo Lookup(FileInfo file, bool forceUpdate = false)
		{
			if (_Info == null || forceUpdate)
			{
				VitaNexCore.TryCatch(f => f.Deserialize(Load), file, Clilocs.CSOptions.ToConsole);
			}

			return _Info;
		}
	}
}
