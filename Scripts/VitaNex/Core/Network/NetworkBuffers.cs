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
using System.Reflection;

using Server;
using Server.Network;
#endregion

namespace VitaNex.Network
{
	public static class NetworkBuffers
	{
		private static void Resize(Type owner, string field, int resize)
		{
			var f = owner.GetField(field, BindingFlags.Static | BindingFlags.NonPublic);

			if (f != null)
			{
				var b = f.GetValue(null) as BufferPool;

				if (b != null)
				{
					b.GetInfo(out var name, out var freeCount, out var initialCapacity, out var currentCapacity, out var bufferSize, out var misses);

					if (bufferSize < resize && b.SetFieldValue("m_BufferSize", resize))
					{
						if (b.GetFieldValue("m_FreeBuffers", out Queue<byte[]> buffers))
						{
							buffers.Clear();

							for (var i = 0; i < initialCapacity; i++)
							{
								buffers.Enqueue(new byte[resize]);
							}
						}

						VitaNexCore.ToConsole(owner.Name + "." + field + " Buffers: {0:#,0} -> {1:#,0}", bufferSize, resize);
					}
				}
			}
		}

		[CallPriority(Int32.MaxValue)]
		public static void Configure()
		{
			VitaNexCore.OnInitialized += () =>
			{
				Resize(typeof(Packet), "m_CompressorBuffers", 0x100000);
				Resize(typeof(DisplayGumpPacked), "m_PackBuffers", 0x100000);
			};
		}
	}
}