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

using Server;
#endregion

namespace VitaNex.Web
{
	public class WebAPIOptions : CoreServiceOptions
	{
		public List<string> Whitelist { get; private set; }
		public List<string> Blacklist { get; private set; }

		[CommandProperty(WebAPI.Access)]
		public bool UseWhitelist { get; set; }

		[CommandProperty(WebAPI.Access)]
		public short Port { get; set; }

		[CommandProperty(WebAPI.Access)]
		public int MaxConnections { get; set; }

		[CommandProperty(WebAPI.Access)]
		public int MaxSendBufferSize { get; set; }

		[CommandProperty(WebAPI.Access)]
		public int MaxReceiveBufferSize { get; set; }

		[CommandProperty(WebAPI.Access)]
		public int MaxSendBufferSizeBytes => MaxSendBufferSize * 1024 * 1024;

		[CommandProperty(WebAPI.Access)]
		public int MaxReceiveBufferSizeBytes => MaxReceiveBufferSize * 1024 * 1024;

		[CommandProperty(WebAPI.Access)]
		public bool DirectoryIndex { get; set; }

		[CommandProperty(WebAPI.Access)]
		public bool WebServer { get; set; }

		public WebAPIOptions()
			: base(typeof(WebAPI))
		{
			Whitelist = new List<string>();
			Blacklist = new List<string>();

			Port = 80;
			MaxConnections = 500;
			MaxSendBufferSize = 32;
			MaxReceiveBufferSize = 32;

			UseWhitelist = false;
			WebServer = false;
			DirectoryIndex = false;
		}

		public WebAPIOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			base.Clear();

			Whitelist.Clear();
			Blacklist.Clear();

			Port = 80;
			MaxConnections = 500;
			MaxSendBufferSize = 32;
			MaxReceiveBufferSize = 32;

			UseWhitelist = false;
			WebServer = false;
			DirectoryIndex = false;
		}

		public override void Reset()
		{
			base.Reset();

			Whitelist.Clear();
			Blacklist.Clear();

			Port = 80;
			MaxConnections = 500;
			MaxSendBufferSize = 32;
			MaxReceiveBufferSize = 32;

			UseWhitelist = false;
			WebServer = false;
			DirectoryIndex = false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(4);

			switch (version)
			{
				case 4:
				{
					writer.Write(WebServer);
					writer.Write(DirectoryIndex);
				}
				goto case 3;
				case 3:
				case 2:
				{
					writer.Write(MaxSendBufferSize);
					writer.Write(MaxReceiveBufferSize);
				}
				goto case 1;
				case 1:
				{
					writer.WriteList(Whitelist, (w, m) => w.Write(m));
					writer.WriteList(Blacklist, (w, m) => w.Write(m));

					writer.Write(UseWhitelist);
				}
				goto case 0;
				case 0:
				{
					writer.Write(Port);
					writer.Write(MaxConnections);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			if (version < 3)
			{
				MaxSendBufferSize = 32;
				MaxReceiveBufferSize = 32;
			}

			if (version < 2)
			{
				Whitelist = new List<string>();
				Blacklist = new List<string>();
			}

			switch (version)
			{
				case 4:
				{
					WebServer = reader.ReadBool();
					DirectoryIndex = reader.ReadBool();
				}
				goto case 3;
				case 3:
				case 2:
				{
					MaxSendBufferSize = reader.ReadInt();
					MaxReceiveBufferSize = reader.ReadInt();
				}
				goto case 1;
				case 1:
				{
					Whitelist = reader.ReadList(r => r.ReadString(), Whitelist);
					Blacklist = reader.ReadList(r => r.ReadString(), Blacklist);

					UseWhitelist = reader.ReadBool();
				}
				goto case 0;
				case 0:
				{
					Port = reader.ReadShort();
					MaxConnections = reader.ReadInt();
				}
				break;
			}
		}
	}
}
