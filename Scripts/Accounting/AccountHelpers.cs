#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Server.Commands;
using Server.Diagnostics;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Accounting
{
	public partial class Account
	{
		private static MD5CryptoServiceProvider m_MD5HashProvider;
		private static SHA1CryptoServiceProvider m_SHA1HashProvider;
		private static SHA512CryptoServiceProvider m_SHA512HashProvider;
		private static byte[] m_HashBuffer;

		public static void Configure()
		{
			CommandSystem.Register("ConvertCurrency", AccessLevel.Owner, ConvertCurrency);
		}

		private static void ConvertCurrency(CommandEventArgs e)
		{
			e.Mobile.SendMessage(
				"Converting All Banked Gold from {0} to {1}.  Please wait...",
				AccountGold.Enabled ? "checks and coins" : "account treasury",
				AccountGold.Enabled ? "account treasury" : "checks and coins");

			NetState.Pause();

			double found = 0.0, converted = 0.0;

			try
			{
				BankBox box;
				long share = 0, shared;
				int diff;

				foreach (var a in Accounts.Instances.Where(a => a.Count > 0))
				{
					try
					{
						if (!AccountGold.Enabled)
						{
							share = (int)Math.Truncate(a.TotalCurrency / a.Count * CurrencyThreshold);
							found += a.TotalCurrency * CurrencyThreshold;
						}

						foreach (var m in a)
						{
							box = m.FindBankNoCreate();

							if (box == null)
							{
								continue;
							}

							if (AccountGold.Enabled)
							{
								foreach (var o in box.FindItemsByType<BankCheck>())
								{
									found += o.Worth;

									if (!a.DepositGold(o.Worth))
									{
										break;
									}

									converted += o.Worth;
									o.Delete();
								}

								foreach (var o in box.FindItemsByType<Gold>())
								{
									found += o.Amount;

									if (!a.DepositGold(o.Amount))
									{
										break;
									}

									converted += o.Amount;
									o.Delete();
								}
							}
							else
							{
								shared = share;

								while (shared > 0)
								{
									if (shared > 60000)
									{
										diff = (int)Math.Min(10000000, shared);

										if (a.WithdrawGold(diff))
										{
											box.DropItem(new BankCheck(diff));
										}
										else
										{
											break;
										}
									}
									else
									{
										diff = (int)Math.Min(60000, shared);

										if (a.WithdrawGold(diff))
										{
											box.DropItem(new Gold(diff));
										}
										else
										{
											break;
										}
									}

									converted += diff;
									shared -= diff;
								}
							}

							box.UpdateTotals();
						}
					}
					catch (Exception ex)
					{
						ExceptionLogging.LogException(ex);
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionLogging.LogException(ex);
			}

			NetState.Resume();

			e.Mobile.SendMessage("Operation complete: {0:#,0} of {1:#,0} Gold has been converted in total.", converted, found);
		}

		public static string Hash(string phrase, PasswordProtection type)
		{
			HashAlgorithm hash = null;

			switch (type)
			{
				case PasswordProtection.None: return phrase;
				case PasswordProtection.MD5: hash = m_MD5HashProvider ?? (m_MD5HashProvider = new MD5CryptoServiceProvider()); break;
				case PasswordProtection.SHA1: hash = m_SHA1HashProvider ?? (m_SHA1HashProvider = new SHA1CryptoServiceProvider()); break;
				case PasswordProtection.SHA512: hash = m_SHA512HashProvider ?? (m_SHA512HashProvider = new SHA512CryptoServiceProvider()); break;
			}

			if (hash == null)
			{
				return null;
			}

			if (m_HashBuffer == null)
			{
				m_HashBuffer = new byte[256];
			}

			var length = Encoding.ASCII.GetBytes(phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0);

			var pass = BitConverter.ToString(hash.ComputeHash(m_HashBuffer, 0, length));

			return pass.Replace("-", String.Empty);
		}

		public void ResolveGameTime()
		{
			if (m_TotalGameTime <= TimeSpan.Zero)
			{
				var totalTicks = 0L;

				foreach (var m in m_Mobiles)
				{
					if (m is PlayerMobile pm)
					{
						totalTicks += pm.GameTime.Ticks;
					}
				}

				m_TotalGameTime = TimeSpan.FromTicks(totalTicks);
			}
		}

		#region Saving Binary

		public virtual bool Save(GenericWriter writer)
		{
			try
			{
				writer.Write(0);

				writer.Write(Username);

				SavePassword(writer);

				writer.Write((int)AccessLevel);
				writer.Write(Flags);
				writer.Write(Created);
				writer.Write(LastLogin);
				writer.Write(TotalGameTime);
				writer.Write(TotalCurrency);
				writer.Write(Sovereigns);

				SaveMobiles(writer);
				SaveComments(writer);
				SaveTags(writer);
				SaveAddressList(writer);
				SaveAccessCheck(writer);

				return true;
			}
			catch (Exception ex)
			{
				ExceptionLogging.LogException(ex);

				return false;
			}
		}

		protected virtual void SavePassword(GenericWriter writer)
		{
			if (PlainPassword != null)
			{
				writer.Write((sbyte)PasswordProtection.None);
				writer.Write(PlainPassword);
			}
			else if (MD5Password != null)
			{
				writer.Write((sbyte)PasswordProtection.MD5);
				writer.Write(MD5Password);
			}
			else if (SHA1Password != null)
			{
				writer.Write((sbyte)PasswordProtection.SHA1);
				writer.Write(SHA1Password);
			}
			else if (SHA512Password != null)
			{
				writer.Write((sbyte)PasswordProtection.SHA512);
				writer.Write(SHA512Password);
			}
			else
			{
				writer.Write((sbyte)-1);
			}
		}

		protected virtual void SaveMobiles(GenericWriter writer)
		{
			if (m_Mobiles?.Length > 0)
			{
				writer.Write(m_Mobiles.Length);

				for (var i = 0; i < m_Mobiles.Length; i++)
				{
					var m = m_Mobiles[i];

					writer.Write(m_Mobiles[i]);

					if (m != null && SecureAccounts != null && SecureAccounts.TryGetValue(m, out var secureBalance))
					{
						writer.Write(secureBalance);
					}
					else
					{
						writer.Write(-1);
					}
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected virtual void SaveComments(GenericWriter writer)
		{
			m_Comments?.RemoveAll(c => c == null);

			if (m_Comments?.Count > 0)
			{
				writer.Write(m_Comments.Count);

				for (var i = 0; i < m_Comments.Count; i++)
				{
					m_Comments[i].Save(writer);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected virtual void SaveTags(GenericWriter writer)
		{
			m_Tags?.RemoveAll(t => t == null);

			if (m_Tags?.Count > 0)
			{
				writer.Write(m_Tags.Count);

				for (var i = 0; i < m_Tags.Count; i++)
				{
					m_Tags[i].Save(writer);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected virtual void SaveAddressList(GenericWriter writer)
		{
			if (LoginIPs?.Length > 0)
			{
				writer.Write(LoginIPs.Length);

				for (var i = 0; i < LoginIPs.Length; i++)
				{
					writer.Write(LoginIPs[i]);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected virtual void SaveAccessCheck(GenericWriter writer)
		{
			if (IPRestrictions?.Length > 0)
			{
				writer.Write(IPRestrictions.Length);

				for (var i = 0; i < IPRestrictions.Length; i++)
				{
					writer.Write(IPRestrictions[i]);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		#endregion

		#region Loading Binary

		public virtual bool Load(GenericReader reader)
		{
			try
			{
				reader.ReadInt();

				Username = reader.ReadString();

				LoadPassword(reader);

				AccessLevel = (AccessLevel)reader.ReadInt();
				Flags = reader.ReadInt();
				Created = reader.ReadDateTime();
				LastLogin = reader.ReadDateTime();
				TotalGameTime = reader.ReadTimeSpan();
				TotalCurrency = reader.ReadDouble();
				Sovereigns = reader.ReadInt();

				LoadMobiles(reader);
				LoadComments(reader);
				LoadTags(reader);
				LoadAddressList(reader);
				LoadAccessCheck(reader);

				ResolveGameTime();

				return true;
			}
			catch (Exception ex)
			{
				ExceptionLogging.LogException(ex);

				return false;
			}
		}

		protected virtual void LoadPassword(GenericReader reader)
		{
			var prot = reader.ReadSByte();

			if (prot < 0)
			{
				return;
			}

			var passLoaded = false;

			var pass = reader.ReadString();

			if (!String.IsNullOrWhiteSpace(pass))
			{
				pass = pass.Replace("-", String.Empty);

				switch ((PasswordProtection)prot)
				{
					case PasswordProtection.None: PlainPassword = pass; passLoaded = true; break;
					case PasswordProtection.MD5: MD5Password = pass; passLoaded = true; break;
					case PasswordProtection.SHA1: SHA1Password = pass; passLoaded = true; break;
					case PasswordProtection.SHA512: SHA512Password = pass; passLoaded = true; break;
				}
			}

			if (!passLoaded)
			{
				SetPassword(!String.IsNullOrWhiteSpace(pass) && prot == 0 ? pass : "empty");
			}
		}

		protected virtual void LoadMobiles(GenericReader reader)
		{
			var count = reader.ReadInt();

			if (count > 0)
			{
				m_Mobiles = new Mobile[count];

				for (var i = 0; i < m_Mobiles.Length; i++)
				{
					var m = reader.ReadMobile();
					var secureBalance = reader.ReadInt();

					if (m == null)
					{
						continue;
					}

					m.Account = this;

					if (secureBalance > 0)
					{
						if (SecureAccounts == null)
						{
							SecureAccounts = new Dictionary<Mobile, int>();
						}

						SecureAccounts[m] = secureBalance;
					}
				}
			}
		}

		protected virtual void LoadComments(GenericReader reader)
		{
			var count = reader.ReadInt();

			if (count > 0)
			{
				m_Comments = new List<IAccountComment>(count);

				while (--count >= 0)
				{
					var type = reader.ReadObjectType();
					var comment = (IAccountComment)Activator.CreateInstance(type, reader);

					if (comment != null)
					{
						m_Comments.Add(comment);
					}
				}
			}
		}

		protected virtual void LoadTags(GenericReader reader)
		{
			var count = reader.ReadInt();

			if (count > 0)
			{
				m_Tags = new List<IAccountTag>(count);

				while (--count >= 0)
				{
					m_Tags.Add(new AccountTag(reader));
				}
			}
		}

		protected virtual void LoadAddressList(GenericReader reader)
		{
			var count = reader.ReadInt();

			if (count > 0)
			{
				LoginIPs = new IPAddress[count];

				for (var i = 0; i < LoginIPs.Length; i++)
				{
					LoginIPs[i] = reader.ReadIPAddress();
				}
			}
		}

		protected virtual void LoadAccessCheck(GenericReader reader)
		{
			var count = reader.ReadInt();

			if (count > 0)
			{
				IPRestrictions = new string[count];

				for (var i = 0; i < IPRestrictions.Length; i++)
				{
					IPRestrictions[i] = reader.ReadString();
				}
			}
		}

		#endregion

		#region Saving Xml

		public virtual bool Save(XmlElement account)
		{
			try
			{
				var parent = account;

				if (!Insensitive.Equals(account.Name, "account"))
				{
					account = AppendNode(account, "account", null);
				}

				AppendNode(account, "username", Username);

				SavePassword(account);

				if (AccessLevel >= AccessLevel.Counselor)
				{
					AppendNode(account, "accessLevel", AccessLevel.ToString());
				}

				if (Flags != 0)
				{
					AppendNode(account, "flags", XmlConvert.ToString(Flags));
				}

				AppendNode(account, "created", XmlConvert.ToString(Created, XmlDateTimeSerializationMode.Utc));
				AppendNode(account, "lastLogin", XmlConvert.ToString(LastLogin, XmlDateTimeSerializationMode.Utc));
				AppendNode(account, "totalGameTime", XmlConvert.ToString(TotalGameTime));
				AppendNode(account, "totalCurrency", XmlConvert.ToString(TotalCurrency));
				AppendNode(account, "sovereigns", XmlConvert.ToString(Sovereigns));

				SaveMobiles(account);
				SaveComments(account);
				SaveTags(account);
				SaveAddressList(account);
				SaveAccessCheck(account);

				if (account != parent)
				{
					parent.AppendChild(account);
				}

				return true;
			}
			catch (Exception ex)
			{
				ExceptionLogging.LogException(ex);

				return false;
			}
		}

		protected virtual void SavePassword(XmlElement node)
		{
			if (PlainPassword != null)
			{
				AppendNode(node, "password", PlainPassword, ("enc", PasswordProtection.None.ToString()));
			}
			else if (MD5Password != null)
			{
				AppendNode(node, "password", MD5Password, ("enc", PasswordProtection.MD5.ToString()));
			}
			else if (SHA1Password != null)
			{
				AppendNode(node, "password", SHA1Password, ("enc", PasswordProtection.SHA1.ToString()));
			}
			else if (SHA512Password != null)
			{
				AppendNode(node, "password", SHA512Password, ("enc", PasswordProtection.SHA512.ToString()));
			}
		}

		protected virtual void SaveMobiles(XmlElement node)
		{
			if (m_Mobiles?.Length > 0)
			{
				var count = 0;

				for (var i = 0; i < m_Mobiles.Length; i++)
				{
					var m = m_Mobiles[i];

					if (m != null && !m.Deleted)
					{
						++count;
					}
				}

				if (count > 0)
				{
					var chars = AppendNode(node, "chars", null, ("count", XmlConvert.ToString(count)));

					for (var i = 0; i < m_Mobiles.Length; i++)
					{
						var m = m_Mobiles[i];

						if (m != null && !m.Deleted)
						{
							if (SecureAccounts != null && SecureAccounts.TryGetValue(m, out var secureBalance))
							{
								AppendNode(chars, "char", XmlConvert.ToString(m.Serial.Value), ("index", XmlConvert.ToString(i)), ("secureBalance", XmlConvert.ToString(secureBalance)));
							}
							else
							{
								AppendNode(chars, "char", XmlConvert.ToString(m.Serial.Value), ("index", XmlConvert.ToString(i)));
							}
						}
					}
				}
			}
		}

		protected virtual void SaveComments(XmlElement node)
		{
			if (m_Comments?.Count > 0)
			{
				var comments = AppendNode(node, "comments", null, ("count", XmlConvert.ToString(m_Comments.Count)));

				foreach (var c in m_Comments)
				{
					c.Save(comments);
				}
			}
		}

		protected virtual void SaveTags(XmlElement node)
		{
			if (m_Tags?.Count > 0)
			{
				var tags = AppendNode(node, "tags", null, ("count", XmlConvert.ToString(m_Tags.Count)));

				foreach (var t in m_Tags)
				{
					t.Save(tags);
				}
			}
		}

		protected virtual void SaveAddressList(XmlElement node)
		{
			if (LoginIPs?.Length > 0)
			{
				var list = AppendNode(node, "addressList", null, ("count", XmlConvert.ToString(LoginIPs.Length)));

				foreach (var ip in LoginIPs)
				{
					AppendNode(list, "ip", ip.ToString());
				}
			}
		}

		protected virtual void SaveAccessCheck(XmlElement node)
		{
			if (IPRestrictions?.Length > 0)
			{
				var list = AppendNode(node, "accessCheck", null, ("count", XmlConvert.ToString(IPRestrictions.Length)));

				foreach (var ip in IPRestrictions)
				{
					AppendNode(list, "ip", ip);
				}
			}
		}

		protected virtual XmlElement AppendNode(XmlElement parent, string name, string content, params (string key, string val)[] attrs)
		{
			var e = parent.OwnerDocument.CreateElement(name);

			if (content != null)
			{
				e.InnerText = content;
			}

			foreach (var (key, val) in attrs)
			{
				e.SetAttribute(key, val);
			}

			parent.AppendChild(e);

			return e;
		}

		#endregion

		#region Loading Xml

		public virtual bool Load(XmlElement node)
		{
			try
			{
				if (!Insensitive.Equals(node.Name, "account"))
				{
					return false;
				}

				Username = Utility.GetText(node["username"], "empty");

				LoadPassword(node);

				if (Enum.TryParse(Utility.GetText(node["accessLevel"], "Player"), true, out AccessLevel accessLevel))
				{
					AccessLevel = accessLevel;
				}

				Flags = Utility.GetXMLInt32(Utility.GetText(node["flags"], "0"), 0);
				Created = Utility.GetXMLDateTime(Utility.GetText(node["created"], null), DateTime.UtcNow);
				LastLogin = Utility.GetXMLDateTime(Utility.GetText(node["lastLogin"], null), DateTime.UtcNow);
				TotalGameTime = Utility.GetXMLTimeSpan(Utility.GetText(node["totalGameTime"], null), TimeSpan.Zero);
				TotalCurrency = Utility.GetXMLDouble(Utility.GetText(node["totalCurrency"], "0"), 0);
				Sovereigns = Utility.GetXMLInt32(Utility.GetText(node["sovereigns"], "0"), 0);

				LoadMobiles(node);
				LoadComments(node);
				LoadTags(node);
				LoadAddressList(node);
				LoadAccessCheck(node);

				ResolveGameTime();

				// legacy loading for secure accounts
				if (SecureAccounts == null || SecureAccounts.Count == 0)
				{
					var chars = node["SecureAccounts"];

					if (chars != null)
					{
						foreach (XmlElement ele in chars.GetElementsByTagName("char"))
						{
							var index = Utility.GetXMLInt32(Utility.GetAttribute(ele, "index", "-1"), -1);

							if (index >= 0 && index < m_Mobiles.Length)
							{
								var m = m_Mobiles[index];

								if (m != null && !m.Deleted && m.Account == this)
								{
									var balance = Utility.GetXMLInt32(Utility.GetText(ele, "0"), 0);

									if (balance > 0)
									{
										if (SecureAccounts == null)
										{
											SecureAccounts = new Dictionary<Mobile, int>();
										}

										SecureAccounts[m] = balance;
									}
								}
							}
						}
					}
				}

				if (Young)
				{
					CheckYoung();
				}

				return true;
			}
			catch (Exception ex)
			{
				ExceptionLogging.LogException(ex);

				return false;
			}
		}

		protected virtual void LoadPassword(XmlElement node)
		{
			var passLoaded = false;
			var prot = PasswordProtection.None;

			var pass = Utility.GetText(node["password"], null);

			if (!String.IsNullOrWhiteSpace(pass))
			{
				pass = pass.Replace("-", String.Empty);

				var enc = Utility.GetAttribute(node["password"], "enc", null);

				if (!String.IsNullOrWhiteSpace(enc) && Enum.TryParse(enc, true, out prot))
				{
					switch (prot)
					{
						case PasswordProtection.None: PlainPassword = pass; passLoaded = true; break;
						case PasswordProtection.MD5: MD5Password = pass; passLoaded = true; break;
						case PasswordProtection.SHA1: SHA1Password = pass; passLoaded = true; break;
						case PasswordProtection.SHA512: SHA512Password = pass; passLoaded = true; break;
					}
				}
			}

			if (!passLoaded) // handle old password api
			{
				if (!passLoaded && (passLoaded = !String.IsNullOrWhiteSpace(pass = Utility.GetText(node["cryptPassword"], null))))
				{
					prot = PasswordProtection.MD5;
				}

				if (!passLoaded && (passLoaded = !String.IsNullOrWhiteSpace(pass = Utility.GetText(node["newCryptPassword"], null))))
				{
					prot = PasswordProtection.SHA1;
				}

				if (!passLoaded && (passLoaded = !String.IsNullOrWhiteSpace(pass = Utility.GetText(node["newSecureCryptPassword"], null))))
				{
					prot = PasswordProtection.SHA512;
				}

				if (!String.IsNullOrWhiteSpace(pass))
				{
					pass = pass.Replace("-", String.Empty);
				}

				switch (prot)
				{
					case PasswordProtection.None: PlainPassword = pass; passLoaded = true; break;
					case PasswordProtection.MD5: MD5Password = pass; passLoaded = true; break;
					case PasswordProtection.SHA1: SHA1Password = pass; passLoaded = true; break;
					case PasswordProtection.SHA512: SHA512Password = pass; passLoaded = true; break;
				}
			}

			if (!passLoaded)
			{
				SetPassword(!String.IsNullOrWhiteSpace(pass) && prot == PasswordProtection.None ? pass : "empty");
			}
		}

		protected virtual void LoadMobiles(XmlElement node)
		{
			var chars = node["chars"];

			if (chars == null)
			{
				return;
			}

			m_Mobiles = new Mobile[7];

			foreach (XmlElement ele in chars.GetElementsByTagName("char"))
			{
				var index = Utility.GetXMLInt32(Utility.GetAttribute(ele, "index", "-1"), -1);
				var serial = Utility.GetXMLSerial(Utility.GetText(ele, "0"), Serial.Zero);

				if (index >= 0 && index < m_Mobiles.Length && serial.IsMobile)
				{
					var m = World.FindMobile(serial);

					if (m == null)
					{
						continue;
					}

					m_Mobiles[index] = m;

					m.Account = this;

					var secureBalance = Utility.GetXMLInt32(Utility.GetAttribute(ele, "secureBalance", "0"), 0);

					if (secureBalance > 0)
					{
						if (SecureAccounts == null)
						{
							SecureAccounts = new Dictionary<Mobile, int>();
						}

						SecureAccounts[m] = secureBalance;
					}
				}
			}
		}

		protected virtual void LoadComments(XmlElement node)
		{
			var comments = node["comments"];

			if (comments == null)
			{
				return;
			}

			var elements = comments.GetElementsByTagName("comment");

			m_Comments = new List<IAccountComment>(elements.Count);

			foreach (XmlElement comment in elements)
			{
				m_Comments.Add(new AccountComment(comment));
			}
		}

		protected virtual void LoadTags(XmlElement node)
		{
			var tags = node["tags"];

			if (tags == null)
			{
				return;
			}

			var elements = tags.GetElementsByTagName("tag");

			m_Tags = new List<IAccountTag>(elements.Count);

			foreach (XmlElement tag in elements)
			{
				m_Tags.Add(new AccountTag(tag));
			}
		}

		protected virtual void LoadAddressList(XmlElement node)
		{
			var addressList = node["addressList"];

			if (addressList == null)
			{
				return;
			}

			var elements = addressList.GetElementsByTagName("ip");

			var count = 0;

			foreach (XmlElement e in elements)
			{
				var ip = Utility.GetText(e, null);

				if (IPAddress.TryParse(ip, out var address))
				{
					++count;
				}
			}

			LoginIPs = new IPAddress[count];

			var index = -1;

			foreach (XmlElement e in elements)
			{
				var ip = Utility.GetText(e, null);

				if (IPAddress.TryParse(ip, out var address))
				{
					LoginIPs[++index] = Utility.Intern(address);
				}
			}
		}

		protected virtual void LoadAccessCheck(XmlElement node)
		{
			var accessCheck = node["accessCheck"];

			if (accessCheck == null)
			{
				return;
			}

			var elements = accessCheck.GetElementsByTagName("ip");

			var count = 0;

			foreach (XmlElement e in elements)
			{
				var ip = Utility.GetText(e, null);

				if (ip != null)
				{
					++count;
				}
			}

			IPRestrictions = new string[count];

			var index = -1;

			foreach (XmlElement e in elements)
			{
				var ip = Utility.GetText(e, null);

				if (ip != null)
				{
					IPRestrictions[++index] = ip;
				}
			}
		}

		protected virtual void LoadGameTime(XmlElement node)
		{
		}

		#endregion
	}
}
