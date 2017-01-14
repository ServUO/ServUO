#region Header
// **********
// ServUO - HelpPersistence.cs
// **********
#endregion

#region References

using System.IO;

#endregion

namespace Server.Engines.Help
{
	public static class HelpPersistence
	{
		private static readonly string _FilePath = Path.Combine("Saves", "Help", "Pages.bin");


		[CallPriority(900)]
		public static void Configure()
		{
			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;
		}

		private static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				_FilePath,
				writer =>
				{
					writer.Write(0); // version

					writer.Write(PageQueue.List.Count);

					foreach (PageEntry pe in PageQueue.List)
					{
						writer.Write(pe.Sender);
						writer.Write(pe.Message);
						writer.Write((int)pe.Type);
						writer.Write(pe.Handler);
						writer.Write(pe.Sent);
						writer.Write(pe.PageLocation);
						writer.Write(pe.PageMap);

					}
				});
		}

		private static void OnLoad()
		{
			Persistence.Deserialize(
				_FilePath,
				reader =>
				{
					var version = reader.ReadInt();
					var count = reader.ReadInt();

					switch (version)
					{
						case 0:
							{
								for (var i = 0; i < count; ++i)
								{
									var sender = reader.ReadMobile();
									var message = reader.ReadString();
									var type = (PageType)reader.ReadInt();
									PageEntry pe = new PageEntry(sender, message, type)
									{
										Handler = reader.ReadMobile(),
										Sent = reader.ReadDateTime(),
										PageLocation = reader.ReadPoint3D(),
										PageMap = reader.ReadMap()
									};
									pe.Stop();

									PageQueue.Enqueue(pe);
								}
							}
							break;
					}
				});
		}
	}
}
