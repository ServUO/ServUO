using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Server.Accounting
{
	public enum AccountFormat
	{
		Detect,
		Xml,
		Bin,
		Json,
		Sql,
	}

	public delegate void AccountLoadHandler(AccountFormat format);

	public delegate void AccountSaveHandler(AccountFormat format);

	public delegate IAccount AccountConstructor(string username, string password);

	public delegate IAccount AccountXmlConstructor(XmlElement node);

	public delegate IAccount AccountBinConstructor(GenericReader node);

	public static class Accounts
	{
		private static readonly Dictionary<string, IAccount> m_Accounts = new Dictionary<string, IAccount>(1024, StringComparer.OrdinalIgnoreCase);

		public static IEnumerable<IAccount> Instances
		{
			get
			{
				foreach (var a in m_Accounts.Values)
				{
					if (a?.Deleted == false)
					{
						yield return a;
					}
				}
			}
		}

		public static int Count => m_Accounts.Count;

		[ConfigProperty("Accounts.YoungDuration")]
		public static TimeSpan YoungDuration { get => Config.Get("Accounts.YoungDuration", TimeSpan.FromHours(40.0)); set => Config.Set("Accounts.YoungDuration", value); }

		[ConfigProperty("Accounts.InactiveDuration")]
		public static TimeSpan InactiveDuration { get => Config.Get("Accounts.InactiveDuration", TimeSpan.FromDays(180.0)); set => Config.Set("Accounts.InactiveDuration", value); }

		[ConfigProperty("Accounts.EmptyInactiveDuration")]
		public static TimeSpan EmptyInactiveDuration { get => Config.Get("Accounts.EmptyInactiveDuration", TimeSpan.FromDays(30.0)); set => Config.Set("Accounts.EmptyInactiveDuration", value); }

		[ConfigProperty("Accounts.Construct")]
		public static AccountConstructor Construct { get => Config.GetDelegate<AccountConstructor>("Accounts.Construct", InternalConstruct); set => Config.SetDelegate("Accounts.Construct", value); }

		[ConfigProperty("Accounts.ConstructXml")]
		public static AccountXmlConstructor ConstructXml { get => Config.GetDelegate<AccountXmlConstructor>("Accounts.ConstructXml", InternalConstructXml); set => Config.SetDelegate("Accounts.ConstructXml", value); }

		[ConfigProperty("Accounts.ConstructBin")]
		public static AccountBinConstructor ConstructBin { get => Config.GetDelegate<AccountBinConstructor>("Accounts.ConstructBin", InternalConstructBin); set => Config.SetDelegate("Accounts.ConstructBin", value); }

		[ConfigProperty("Accounts.LoadHandler")]
		public static AccountLoadHandler Load { get => Config.GetDelegate<AccountLoadHandler>("Accounts.LoadHandler", InternalLoad); set => Config.SetDelegate("Accounts.LoadHandler", value); }

		[ConfigProperty("Accounts.SaveHandler")]
		public static AccountSaveHandler Save { get => Config.GetDelegate<AccountSaveHandler>("Accounts.SaveHandler", InternalSave); set => Config.SetDelegate("Accounts.SaveHandler", value); }

		[ConfigProperty("Accounts.Format")]
		public static AccountFormat Format { get => Config.GetEnum("Accounts.Format", AccountFormat.Detect); set => Config.SetEnum("Accounts.Format", value); }

		public static string SaveRoot => Path.Combine(Core.BaseDirectory, "Saves", "Accounts");

		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			EventSink.WorldLoad += () =>
			{
				(Load ?? InternalLoad).Invoke(Format);
			};

			EventSink.BeforeWorldSave += e =>
			{
				Defragment();
			};

			EventSink.WorldSave += e =>
			{
				(Save ?? InternalSave).Invoke(Format);
			};
		}

		public static int Defragment()
		{
			var usernames = new HashSet<string>(32);

			foreach (var kv in m_Accounts)
			{
				if (kv.Value?.Deleted != false)
				{
					usernames.Add(kv.Key);
				}
			}

			var count = usernames.Count;

			if (count > 0)
			{
				foreach (var username in usernames)
				{
					m_Accounts.Remove(username);
				}
			}

			ColUtility.Free(usernames);

			if (count > 0)
			{
				Utility.WriteLine(ConsoleColor.Green, $"[Accounts]: Cleared {count:N0} deleted accounts.");
			}

			return count;
		}

		public static IEnumerable<IAccount> GetAccounts()
		{
			return Instances;
		}

		public static bool TryGetAccount(string username, out IAccount account)
		{
			return (account = GetAccount(username)) != null;
		}

		public static IAccount GetAccount(string username)
		{
			if (username != null)
			{
				m_Accounts.TryGetValue(username, out var a);

				return a;
			}

			return null;
		}

		public static void Add(IAccount a)
		{
			if (a != null && !a.Deleted && a.Username != null)
			{
				m_Accounts[a.Username] = a;
			}
		}

		public static void Remove(string username)
		{
			if (username != null)
			{
				m_Accounts.Remove(username);
			}
		}

		public static IAccount Create(string username, string password)
		{
			var a = GetAccount(username);

			if (a != null && !a.Deleted)
			{
				return a;
			}

			a = (Construct ?? InternalConstruct)(username, password);

			if (a != null && !a.Deleted && a.Username != null)
			{
				return m_Accounts[a.Username] = a;
			}

			return null;
		}

		private static IAccount InternalConstruct(string username, string password)
		{
			return new Account(username, password);
		}

		private static IAccount InternalConstructXml(XmlElement node)
		{
			return new Account(node);
		}

		private static IAccount InternalConstructBin(GenericReader reader)
		{
			return new Account(reader);
		}

		private static void InternalLoad(AccountFormat format)
		{
			try
			{
				if (!ResolveLoadPath(ref format, out var path))
				{
					return;
				}

				switch (format)
				{
					case AccountFormat.Xml:
					{
						Persistence.Load(path, "accounts", accounts =>
						{
							var ctor = ConstructXml ?? InternalConstructXml;

							foreach (XmlElement node in accounts.GetElementsByTagName("account"))
							{
								Add(ctor(node));
							}
						});
					}
					break;

					case AccountFormat.Bin:
					{
						Persistence.Deserialize(path, reader =>
						{
							var ctor = ConstructBin ?? InternalConstructBin;

							reader.ReadInt();

							var count = reader.ReadInt();

							while (--count >= 0)
							{
								Add(ctor(reader));
							}
						});
					}
					break;

					default: throw new NotImplementedException($"{format} is not supported.");
				}
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e, "Accounts load failed");
			}
		}

		private static void InternalSave(AccountFormat format)
		{
			try
			{
				if (!ResolveSavePath(ref format, out var path))
				{
					return;
				}

				switch (format)
				{
					case AccountFormat.Xml:
					{
						Persistence.Save(path, "accounts", accounts =>
						{
							accounts.SetAttribute("count", XmlConvert.ToString(m_Accounts.Count));

							foreach (var a in m_Accounts.Values)
							{
								a.Save(accounts);
							}
						});
					}
					break;

					case AccountFormat.Bin:
					{
						Persistence.Serialize(path, writer =>
						{
							writer.Write(0);

							writer.Write(m_Accounts.Count);

							foreach (var a in m_Accounts.Values)
							{
								a.Save(writer);
							}
						});
					}
					break;

					default: throw new NotImplementedException($"{format} is not supported.");
				}
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e, "Accounts save failed");
			}
		}

		private static bool ResolveLoadPath(ref AccountFormat format, out string path)
		{
			path = null;

			var root = SaveRoot;

			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
				return false;
			}

			if (format != AccountFormat.Detect)
			{
				path = Path.Combine(root, $"accounts.{format.ToString().ToLower()}");

				if (File.Exists(path))
				{
					return true;
				}
			}

			foreach (AccountFormat f in Enum.GetValues(typeof(AccountFormat)))
			{
				if (f == AccountFormat.Detect || f == format)
				{
					continue;
				}

				var file = Path.Combine(root, "accounts." + f.ToString().ToLower());

				if (File.Exists(file))
				{
					format = f;
					path = file;

					return true;
				}
			}

			return false;
		}

		private static bool ResolveSavePath(ref AccountFormat format, out string path)
		{
			path = null;

			var root = SaveRoot;

			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}

			if (format == AccountFormat.Detect)
			{
				foreach (AccountFormat f in Enum.GetValues(typeof(AccountFormat)))
				{
					if (f == AccountFormat.Detect || f == format)
					{
						continue;
					}

					var file = Path.Combine(root, "accounts." + f.ToString().ToLower());

					if (File.Exists(file))
					{
						format = f;
						path = file;

						return true;
					}
				}

				return false;
			}

			path = Path.Combine(root, $"accounts.{format.ToString().ToLower()}");

			return true;
		}
	}
}
