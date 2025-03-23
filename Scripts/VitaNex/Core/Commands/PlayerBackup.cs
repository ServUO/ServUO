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
using System.IO;
using System.Linq;
using System.Text;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.Commands
{
	public static class PlayerBackup
	{
		public static void Configure()
		{
			EventSink.DeleteRequest += HandleDeleteRequest;
		}

		public static void Initialize()
		{
			CommandUtility.Register("BackupState", AccessLevel.Administrator, OnBackupCommand);
			CommandUtility.Register("RestoreState", AccessLevel.Administrator, OnRestoreCommand);
		}

		private static void HandleDeleteRequest(DeleteRequestEventArgs e)
		{
			var state = e.State;
			var index = e.Index;

			var acct = state.Account as Account;

			if (acct == null)
			{
				return;
			}

			var m = acct[index] as PlayerMobile;

			if (m != null && !m.Deleted && m.GameTime.TotalHours >= 24)
			{
				try
				{
					BackupState(m, true);
				}
				catch
				{ }
			}
		}

		[Usage("BackupState [DisableLogs=false]"), Description("Writes a binary data file containing information for a character's Bank, Pack and Equipment.")]
		public static void OnBackupCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
			{
				return;
			}

			e.Mobile.SendMessage("Target a PlayerMobile to backup...");
			e.Mobile.BeginTarget<PlayerMobile>((m, t) =>
			{
				BackupState(t, !e.GetBoolean(0), out var count, out var fails);

				if (fails > 0)
				{
					e.Mobile.SendMessage("Backup: {0:#,0} / {1:#,0} saved item states.", count - fails, count);
				}
				else
				{
					e.Mobile.SendMessage("Backup: {0:#,0} saved item states.", count);
				}
			}, null);
		}

		[Usage("RestoreState <Serial=-1, MoveExisting=false, Logging=true>"), Description("Reads a binary data file containing information for a character's Bank, Pack and Equipment.")]
		public static void OnRestoreCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
			{
				return;
			}

			var serial = Serial.MinusOne;

			var moveExisting = false;
			var logging = true;

			if (e.Arguments.Length > 0)
			{
#if ServUOX
				serial = e.GetSerial(0);
#else
				serial = e.GetInt32(0);
#endif
			}

			if (e.Arguments.Length > 1)
			{
				moveExisting = e.GetBoolean(1);
			}

			if (e.Arguments.Length > 2)
			{
				logging = e.GetBoolean(2);
			}

			e.Mobile.SendMessage("Target a PlayerMobile to restore...");
			e.Mobile.BeginTarget<PlayerMobile>((m, t) =>
			{

				RestoreState(t, serial, moveExisting, logging, out var created, out var deleted, out var ignored, out var moved);

				e.Mobile.SendMessage("Restore: {0:#,0} created, {1:#,0} deleted, {2:#,0} ignored, and {3:#,0} moved item states.", created, deleted, ignored, moved);
			}, null);
		}

		public static void BackupState(PlayerMobile m, bool logging)
		{

			BackupState(m, logging, out var count, out var fails);
		}

		public static void BackupState(PlayerMobile m, bool logging, out int count, out int fails)
		{
			var root = VitaNexCore.DataDirectory + "/PlayerBackup/" + m.Account.Username + "/" + m.Serial;

			var idxFile = IOUtility.EnsureFile(root + ".idx", true);
			var binFile = IOUtility.EnsureFile(root + ".bin", true);

			var logFile = logging ? IOUtility.EnsureFile(root + ".log") : null;
			var log = logging ? new StringBuilder() : null;

			if (log != null)
			{
				log.AppendLine();
				log.AppendLine(new string('*', 10));
				log.AppendLine();
				log.AppendLine("BACKUP:\tDate[{0}]\tMobile[{1}]", DateTime.UtcNow, m);
				log.AppendLine();
			}

			var idxLength = 0L;
			var idxCount = 0;
			var idxFails = 0;

			idxFile.Serialize(idx =>
			{
				var v = idx.SetVersion(1);

				idx.Write(m.Serial.Value);

				if (v > 0)
				{
					WriteLength(idx, false, idxLength, idxCount);
				}
				else
				{
					idx.Write(idxLength);
					idx.Write(idxCount);
				}

				binFile.Serialize(bin =>
				{
					bin.SetVersion(0);

					var items = m.FindItemsByType<Item>(true, i => i != null && !i.Deleted);

					foreach (var item in items)
					{
#if NEWPARENT
						var parent = item.Parent != null ? item.Parent.Serial : Serial.MinusOne;
						var logParent = item.Parent ?? (object)Serial.MinusOne;
#else
						var parent = item.ParentEntity != null ? item.ParentEntity.Serial : Serial.MinusOne;
						var logParent = item.ParentEntity ?? (object)Serial.MinusOne;
#endif
						var pos = bin.Position;

						Exception x = null;
						string status;

						try
						{
							item.Serialize(bin);

							status = "SAVED";
						}
						catch (Exception e)
						{
							++idxFails;
							x = e;

							status = "ERROR";
						}

						var len = bin.Position - pos;

						if (log != null)
						{
							log.AppendLine("WRITE:\tIndex[{0}]\t\tLength[{1}]\tStatus[{2}]\tItem[{3}]\t\t\tParent[{4}]", pos, len, status, item, logParent);

							if (x != null)
							{
								log.AppendLine();
								log.AppendLine(new string('*', 10));
								log.AppendLine(x.ToString());
								log.AppendLine(new string('*', 10));
								log.AppendLine();
							}
						}

						WriteIndex(idx, item.GetType(), item.Serial, parent, pos, len);

						idxLength += len;
						++idxCount;
					}
				});

				WriteLength(idx, true, idxLength, idxCount);
			});

			count = idxCount;
			fails = idxFails;

			if (log == null)
			{
				return;
			}

			log.AppendLine();
			log.AppendLine("RESULT:\tCount[{0}]\tFails[{1}]\tLength[{2}]", count - fails, fails, idxLength);
			log.AppendLine();
			logFile.AppendText(false, log.ToString());
		}

		public static void RestoreState(PlayerMobile m, Serial serial, bool moveExisting, bool logging)
		{
			RestoreState(m, serial, moveExisting, logging, out var created, out var deleted, out var ignored, out var moved);
		}

		public static void RestoreState(PlayerMobile m, Serial serial, bool moveExisting, bool logging, out int created, out int deleted, out int ignored, out int moved)
		{
			var pack = m.Backpack;

			if (pack == null || pack.Deleted)
			{
				m.AddItem(pack = new Backpack
				{
					Movable = false
				});
			}

			var bank = m.BankBox;

			if (bank == null || bank.Deleted)
			{
				m.AddItem(bank = new BankBox(m)
				{
					Movable = false
				});
			}

			if (serial == Serial.MinusOne)
			{
				serial = m.Serial;
			}

			var root = VitaNexCore.DataDirectory + "/PlayerBackup/" + m.Account.Username + "/" + serial;

			var idxFile = IOUtility.EnsureFile(root + ".idx");
			var binFile = IOUtility.EnsureFile(root + ".bin");

			var logFile = logging ? IOUtility.EnsureFile(root + ".log") : null;
			var log = logging ? new StringBuilder() : null;

			if (log != null)
			{
				log.AppendLine();
				log.AppendLine(new string('*', 10));
				log.AppendLine();
				log.AppendLine("RESTORE:\tDate[{0}]\tMobile[{1}]", DateTime.UtcNow, m);
				log.AppendLine();
			}

			int idxCreated = 0, idxDeleted = 0, idxIgnored = 0, idxMoved = 0;

			idxFile.Deserialize(idx =>
			{
				var v = idx.GetVersion();

				int ser;

				if (v > 0)
				{
					ser = idx.ReadInt();
				}
				else
				{
					ser = serial.Value;
				}

				if (ser != serial.Value)
				{
					if (log != null)
					{
						log.AppendLine("INVALID:\tSerial[{0:X8}]", ser);
					}

					return;
				}

				long idxLength;
				int idxCount;

				if (v > 0)
				{
					ReadLength(idx, false, out idxLength, out idxCount);
				}
				else
				{
					idxLength = idx.ReadLong();
					idxCount = idx.ReadInt();
				}

				if (log != null)
				{
					log.AppendLine("INDEX:\tCount[{0}]\tLength[{1}]", idxCount, idxLength);
				}

				var items = new Tuple<Item, Serial, long, long, string>[idxCount];

				binFile.Deserialize(bin =>
				{
					bin.GetVersion();

					var restored = new Dictionary<Item, Serial>();

					Backpack oldPack = null;
					BankBox oldBank = null;

					for (var i = 0; i < idxCount; i++)
					{

						ReadIndex(idx, out var type, out var s, out var parent, out var binIndex, out var binLength);

						var valid = s.IsValid && s.IsItem;
						var exists = World.Items.ContainsKey(s);

						Item item = null;

						if (exists)
						{
							item = World.Items[s];

							if (item == null || item.Deleted)
							{
								World.Items.Remove(s);
								exists = false;
							}
						}

						object logItem;
						string status;

						if (!exists && valid && type.IsEqualOrChildOf<Item>())
						{
							item = type.CreateInstanceSafe<Item>(s);

							if (item == null)
							{
								++idxIgnored;

								logItem = s;
								status = "NULL";
							}
							else if (item.Deleted)
							{
								++idxDeleted;

								item = null;
								logItem = s;
								status = "DELETED";
							}
							else
							{
								++idxCreated;

								World.AddItem(item);

								logItem = item;
								status = "CREATED";
							}
						}
						else if (exists && valid && moveExisting && item.RootParent != m)
						{
							++idxMoved;

							logItem = item;
							status = "MOVE";
						}
						else
						{
							++idxIgnored;

							item = null;
							logItem = s;
							status = exists ? "EXISTS" : "INVALID";
						}

						if (log != null)
						{
							log.AppendLine("DATA:\tIndex[{0}]\t\tLength[{1}]\tStatus[{2}]\tItem[{3}]\t\t\tParent[{4}]", binIndex, binLength, status, logItem, parent);
						}

						items[i] = Tuple.Create(item, parent, binIndex, binLength, status);
					}

					foreach (var t in items)
					{
						var item = t.Item1;
						var parent = t.Item2;
						var index = t.Item3;
						var length = t.Item4;
						var status = t.Item5;

						bin.Seek(index, SeekOrigin.Begin);

						if (item == null)
						{
							bin.Seek(index + length, SeekOrigin.Begin);
							continue;
						}

						Exception x = null;

						if (status == "MOVE")
						{
							bin.Seek(index + length, SeekOrigin.Begin);

							status = "IGNORED";
						}
						else
						{
							try
							{
								item.Deserialize(bin);

								status = "LOADED";
							}
							catch (Exception e)
							{
								--idxCreated;
								++idxDeleted;

								item.Delete();
								x = e;

								status = "ERROR";
							}
						}

						if (log != null)
						{
							log.AppendLine("READ:\tIndex[{0}]\tLength[{1}]\tStatus[{2}]\tItem[{3}]\t\t\tParent[{4}]", index, length, status, item, parent);

							if (x != null)
							{
								log.AppendLine();
								log.AppendLine(new string('*', 10));
								log.AppendLine(x.ToString());
								log.AppendLine(new string('*', 10));
								log.AppendLine();
							}
						}

						if (parent == m.Serial)
						{
							if (item is BankBox)
							{
								oldBank = (BankBox)item;
							}
							else if (item is Backpack)
							{
								oldPack = (Backpack)item;
							}
						}

						restored.Add(item, parent);
					}

					if (log != null)
					{
						log.AppendLine();
					}

					Point3D p;

					foreach (var kv in restored.Where(kv => !kv.Key.Deleted).OrderBy(kv => kv.Value))
					{
						var item = kv.Key;

						if ((item == oldPack || item == oldBank) && item != pack && item != bank)
						{
							if (item.Parent is Item)
							{
								((Item)item.Parent).RemoveItem(item);
							}
							else if (item.Parent is Mobile)
							{
								((Mobile)item.Parent).RemoveItem(item);
							}

							item.Parent = null;
							continue;
						}

						var parent = World.FindEntity(kv.Value);

						if (item != pack && item != bank && (item.Parent == oldPack || item.Parent == oldBank))
						{
							((Item)item.Parent).RemoveItem(item);
						}

						if (parent != null)
						{
							if (item == pack || item == bank)
							{
								m.AddItem(item);
							}
							else if (parent == pack || parent == oldPack)
							{
								p = item.Location;
								pack.DropItem(item);
								item.Location = p;
							}
							else if (parent == bank || parent == oldBank)
							{
								p = item.Location;
								bank.DropItem(item);
								item.Location = p;
							}
							else if (parent is Container)
							{
								if (parent.Deleted)
								{
									bank.DropItem(item);
								}
								else
								{
									p = item.Location;
									((Container)parent).DropItem(item);
									item.Location = p;
								}
							}
							else if (parent is Mobile)
							{
								if (!m.EquipItem(item))
								{
									pack.DropItem(item);
								}
							}
							else
							{
								bank.DropItem(item);
							}

							item.SetLastMoved();
							item.UpdateTotals();
							item.Delta(ItemDelta.Update);
						}
						else if (Cleanup.IsBuggable(item))
						{
							--idxCreated;
							++idxDeleted;

							item.Delete();
						}
						else
						{
							item.Internalize();
						}
					}

					if (oldPack != null && oldPack != pack && !restored.ContainsKey(oldPack))
					{
						oldPack.Delete();
					}

					if (oldBank != null && oldBank != bank && !restored.ContainsKey(oldBank))
					{
						oldBank.Delete();
					}

					if (log != null)
					{
						log.AppendLine();
					}

					foreach (var kv in restored)
					{
						if (kv.Key.Deleted)
						{
							if (log != null)
							{
								log.AppendLine("DELETED:\tItem[{0}]\t\tParent[{1}]", kv.Key, kv.Value);
							}
						}
						else if (kv.Key.RootParent == m && kv.Key.Map == Map.Internal && kv.Key.Location == Point3D.Zero)
						{
							if (log != null)
							{
								log.AppendLine("INTERNAL:\tItem[{0}]\t\tParent[{1}]", kv.Key, kv.Value);
							}
						}
						else if (kv.Key.RootParent != m)
						{
							if (log != null)
							{
								log.AppendLine("IGNORED:\tItem[{0}]\t\tParent[{1}]", kv.Key, kv.Value);
							}
						}
						else
						{
							if (log != null)
							{
								log.AppendLine("RESTORED:\tItem[{0}]\t\tParent[{1}]", kv.Key, kv.Key.Parent);
							}
						}
					}

					restored.Clear();

					m.SendEverything();
				});
			});

			created = idxCreated;
			deleted = idxDeleted;
			ignored = idxIgnored;
			moved = idxMoved;

			if (log == null)
			{
				return;
			}

			log.AppendLine();
			log.AppendLine("RESULT:\tCreated[{0}]\t\tDeleted[{1}]\t\tIgnored[{2}]\t\tMoved[{3}]", created, deleted, ignored, moved);

			logFile.AppendText(false, log.ToString());
		}

		private static void WriteLength(GenericWriter idx, bool reset, long length, int count)
		{
			var index = idx.Seek(0, SeekOrigin.Current);

			idx.Seek(8, SeekOrigin.Begin);

			idx.Write(length);
			idx.Write(count);

			if (reset)
			{
				idx.Seek(index, SeekOrigin.Begin);
			}
		}

		private static void ReadLength(GenericReader idx, bool reset, out long length, out int count)
		{
			var index = idx.Seek(0, SeekOrigin.Current);

			idx.Seek(8, SeekOrigin.Begin);

			length = idx.ReadLong();
			count = idx.ReadInt();

			if (reset)
			{
				idx.Seek(index, SeekOrigin.Begin);
			}
		}

		private static void WriteIndex(GenericWriter idx, Type type, Serial serial, Serial parent, long index, long length)
		{
			idx.WriteType(type);
			idx.Write(serial);
			idx.Write(parent);
			idx.Write(index);
			idx.Write(length);
		}

		private static void ReadIndex(GenericReader idx, out Type type, out Serial serial, out Serial parent, out long index, out long length)
		{
			type = idx.ReadType();
			serial = idx.ReadSerial();
			parent = idx.ReadSerial();
			index = idx.ReadLong();
			length = idx.ReadLong();
		}
	}
}