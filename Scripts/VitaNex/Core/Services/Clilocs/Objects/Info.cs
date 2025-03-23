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
using System.Linq;
using System.Text;
using System.Threading;

using Server;

using VitaNex.Collections;
#endregion

namespace VitaNex
{
	public sealed class ClilocInfo : IDisposable
	{
		private volatile object[] _Buffer;

		public ClilocLNG Language { get; private set; }

		public int Index { get; private set; }
		public int Count { get; private set; }

		public string Text { get; private set; }
		public string Format { get; private set; }

		public bool HasArgs { get; private set; }

		public ClilocInfo(ClilocLNG lng, int index, string text)
		{
			Language = lng;
			Index = index;

			Text = text ?? String.Empty;

			Format = Clilocs.VarPattern.Replace(Text, e => $"{{{Count++}}}");

			HasArgs = Count > 0;
		}

		private string Compile(object[] args)
		{
			if (!HasArgs)
			{
				return Text;
			}

			if (args == null)
			{
				args = Array.Empty<object>();
			}

			if (_Buffer == null)
			{
				_Buffer = new object[Count];
			}

			var sep = Count > 1 ? "\t" : null;

			var max = Math.Max(args.Length, _Buffer.Length);
			var lim = _Buffer.Length - 1;

			for (var i = 0; i < max; i++)
			{
				if (i >= _Buffer.Length)
				{
					_Buffer[lim] = $"{_Buffer[lim]} {args[i]}";
				}
				else if (i < args.Length)
				{
					if (i < lim)
					{
						_Buffer[i] = $"{args[i]}{sep}";
					}
					else
					{
						_Buffer[i] = args[i];
					}
				}
				else
				{
					if (i < lim)
					{
						_Buffer[i] = sep;
					}
					else
					{
						_Buffer[i] = null;
					}
				}
			}

			var result = String.Format(Format, _Buffer);

			Array.Clear(_Buffer, 0, _Buffer.Length);

			return Clilocs.NumPattern.Replace(result, match =>
			{
				if (Int32.TryParse(match.Groups["index"].Value, out var sid))
				{
					return Clilocs.GetRawString(Language, sid);
				}

				return match.Value;
			});
		}

		public override string ToString()
		{
			return Text;
		}

		public string ToString(StringBuilder args)
		{
			return ToString(args?.ToString());
		}

		public string ToString(string args)
		{
			return ToString(args?.Split('\t'));
		}

		public string ToString(params object[] args)
		{
			return Compile(args);
		}

		public TextDefinition ToDefinition()
		{
			return new TextDefinition(Index, Text);
		}

		void IDisposable.Dispose()
		{
			GC.SuppressFinalize(this);

			_Buffer = null;

			Text = null;
			Format = null;
		}
	}
}
