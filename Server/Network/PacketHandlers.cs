#region Header
// **********
// ServUO - PacketHandlers.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;

using Server.Accounting;
using Server.ContextMenus;
using Server.Diagnostics;
using Server.Gumps;
using Server.HuePickers;
using Server.Items;
using Server.Menus;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;

using CV = Server.ClientVersion;
#endregion

namespace Server.Network
{
	public enum MessageType
	{
		Regular = 0x00,
		System = 0x01,
		Emote = 0x02,
		Label = 0x06,
		Focus = 0x07,
		Whisper = 0x08,
		Yell = 0x09,
		Spell = 0x0A,

		Guild = 0x0D,
		Alliance = 0x0E,
		Command = 0x0F,

		Encoded = 0xC0
	}

	public static class PacketHandlers
	{
		private static readonly PacketHandler[] m_Handlers;
		private static readonly PacketHandler[] m_6017Handlers;

		private static readonly PacketHandler[] m_ExtendedHandlersLow;
		private static readonly Dictionary<int, PacketHandler> m_ExtendedHandlersHigh;

		private static readonly EncodedPacketHandler[] m_EncodedHandlersLow;
		private static readonly Dictionary<int, EncodedPacketHandler> m_EncodedHandlersHigh;

		public static PacketHandler[] Handlers { get { return m_Handlers; } }

		static PacketHandlers()
		{
			m_Handlers = new PacketHandler[0x100];
			m_6017Handlers = new PacketHandler[0x100];

			m_ExtendedHandlersLow = new PacketHandler[0x100];
			m_ExtendedHandlersHigh = new Dictionary<int, PacketHandler>();

			m_EncodedHandlersLow = new EncodedPacketHandler[0x100];
			m_EncodedHandlersHigh = new Dictionary<int, EncodedPacketHandler>();

			Register(0x00, 104, false, CreateCharacter);
			Register(0x01, 5, false, Disconnect);
			Register(0x02, 7, true, MovementReq);
			Register(0x03, 0, true, AsciiSpeech);
			Register(0x04, 2, true, GodModeRequest);
			Register(0x05, 5, true, AttackReq);
			Register(0x06, 5, true, UseReq);
			Register(0x07, 7, true, LiftReq);
			Register(0x08, 14, true, DropReq);
			Register(0x09, 5, true, LookReq);
			Register(0x0A, 11, true, Edit);
			Register(0x12, 0, true, TextCommand);
			Register(0x13, 10, true, EquipReq);
			Register(0x14, 6, true, ChangeZ);
			Register(0x22, 3, true, Resynchronize);
			Register(0x2C, 2, true, DeathStatusResponse);
			Register(0x34, 10, true, MobileQuery);
			Register(0x3A, 0, true, ChangeSkillLock);
			Register(0x3B, 0, true, VendorBuyReply);
			Register(0x47, 11, true, NewTerrain);
			Register(0x48, 73, true, NewAnimData);
			Register(0x58, 106, true, NewRegion);
			Register(0x5D, 73, false, PlayCharacter);
			Register(0x61, 9, true, DeleteStatic);
			Register(0x6C, 19, true, TargetResponse);
			Register(0x6F, 0, true, SecureTrade);
			Register(0x72, 5, true, SetWarMode);
			Register(0x73, 2, false, PingReq);
			Register(0x75, 35, true, RenameRequest);
			Register(0x79, 9, true, ResourceQuery);
			Register(0x7E, 2, true, GodviewQuery);
			Register(0x7D, 13, true, MenuResponse);
			Register(0x80, 62, false, AccountLogin);
			Register(0x83, 39, false, DeleteCharacter);
			Register(0x91, 65, false, GameLogin);
			Register(0x95, 9, true, HuePickerResponse);
			Register(0x96, 0, true, GameCentralMoniter);
			Register(0x98, 0, true, MobileNameRequest);
			Register(0x9A, 0, true, AsciiPromptResponse);
			Register(0x9B, 258, true, HelpRequest);
			Register(0x9D, 51, true, GMSingle);
			Register(0x9F, 0, true, VendorSellReply);
			Register(0xA0, 3, false, PlayServer);
			Register(0xA4, 149, false, SystemInfo);
			Register(0xA7, 4, true, RequestScrollWindow);
			Register(0xAD, 0, true, UnicodeSpeech);
			Register(0xB1, 0, true, DisplayGumpResponse);
			Register(0xB5, 64, true, ChatRequest);
			Register(0xB6, 9, true, ObjectHelpRequest);
			Register(0xB8, 0, true, ProfileReq);
			Register(0xBB, 9, false, AccountID);
			Register(0xBD, 0, true, ClientVersion);
			Register(0xBE, 0, true, AssistVersion);
			Register(0xBF, 0, true, ExtendedCommand);
			Register(0xC2, 0, true, UnicodePromptResponse);
			Register(0xC8, 2, true, SetUpdateRange);
			Register(0xC9, 6, true, TripTime);
			Register(0xCA, 6, true, UTripTime);
			Register(0xCF, 0, false, AccountLogin);
			Register(0xD0, 0, true, ConfigurationFile);
			Register(0xD1, 2, true, LogoutReq);
			Register(0xD6, 0, true, BatchQueryProperties);
			Register(0xD7, 0, true, EncodedCommand);
			Register(0xE1, 0, false, ClientType);
			Register(0xEF, 21, false, LoginServerSeed);
			Register(0xF4, 0, false, CrashReport);
			Register(0xF8, 106, false, CreateCharacter70160);

			Register6017(0x08, 15, true, DropReq6017);

			RegisterExtended(0x05, false, ScreenSize);
			RegisterExtended(0x06, true, PartyMessage);
			RegisterExtended(0x07, true, QuestArrow);
			RegisterExtended(0x09, true, DisarmRequest);
			RegisterExtended(0x0A, true, StunRequest);
			RegisterExtended(0x0B, false, Language);
			RegisterExtended(0x0C, true, CloseStatus);
			RegisterExtended(0x0E, true, Animate);
			RegisterExtended(0x0F, false, Empty); // What's this?
			RegisterExtended(0x10, true, QueryProperties);
			RegisterExtended(0x13, true, ContextMenuRequest);
			RegisterExtended(0x15, true, ContextMenuResponse);
			RegisterExtended(0x1A, true, StatLockChange);
			RegisterExtended(0x1C, true, CastSpell);
			RegisterExtended(0x24, false, UnhandledBF);
			RegisterExtended(0x2C, true, BandageTarget);

			#region Stygian Abyss
			RegisterExtended(0x32, true, ToggleFlying);
			#endregion

			RegisterEncoded(0x19, true, SetAbility);
			RegisterEncoded(0x28, true, GuildGumpRequest);

			RegisterEncoded(0x32, true, QuestGumpRequest);
		}

		public static void Register(int packetID, int length, bool ingame, OnPacketReceive onReceive)
		{
			m_Handlers[packetID] = new PacketHandler(packetID, length, ingame, onReceive);

			if (m_6017Handlers[packetID] == null)
			{
				m_6017Handlers[packetID] = new PacketHandler(packetID, length, ingame, onReceive);
			}
		}

		public static PacketHandler GetHandler(int packetID)
		{
			return m_Handlers[packetID];
		}

		public static void Register6017(int packetID, int length, bool ingame, OnPacketReceive onReceive)
		{
			m_6017Handlers[packetID] = new PacketHandler(packetID, length, ingame, onReceive);
		}

		public static PacketHandler Get6017Handler(int packetID)
		{
			return m_6017Handlers[packetID];
		}

		public static void RegisterExtended(int packetID, bool ingame, OnPacketReceive onReceive)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				m_ExtendedHandlersLow[packetID] = new PacketHandler(packetID, 0, ingame, onReceive);
			}
			else
			{
				m_ExtendedHandlersHigh[packetID] = new PacketHandler(packetID, 0, ingame, onReceive);
			}
		}

		public static PacketHandler GetExtendedHandler(int packetID)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				return m_ExtendedHandlersLow[packetID];
			}
			else
			{
				PacketHandler handler;
				m_ExtendedHandlersHigh.TryGetValue(packetID, out handler);
				return handler;
			}
		}

		public static void RemoveExtendedHandler(int packetID)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				m_ExtendedHandlersLow[packetID] = null;
			}
			else
			{
				m_ExtendedHandlersHigh.Remove(packetID);
			}
		}

		public static void RegisterEncoded(int packetID, bool ingame, OnEncodedPacketReceive onReceive)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				m_EncodedHandlersLow[packetID] = new EncodedPacketHandler(packetID, ingame, onReceive);
			}
			else
			{
				m_EncodedHandlersHigh[packetID] = new EncodedPacketHandler(packetID, ingame, onReceive);
			}
		}

		public static EncodedPacketHandler GetEncodedHandler(int packetID)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				return m_EncodedHandlersLow[packetID];
			}
			else
			{
				EncodedPacketHandler handler;
				m_EncodedHandlersHigh.TryGetValue(packetID, out handler);
				return handler;
			}
		}

		public static void RemoveEncodedHandler(int packetID)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				m_EncodedHandlersLow[packetID] = null;
			}
			else
			{
				m_EncodedHandlersHigh.Remove(packetID);
			}
		}

		public static void RegisterThrottler(int packetID, ThrottlePacketCallback t)
		{
			PacketHandler ph = GetHandler(packetID);

			if (ph != null)
			{
				ph.ThrottleCallback = t;
			}

			ph = Get6017Handler(packetID);

			if (ph != null)
			{
				ph.ThrottleCallback = t;
			}
		}

		private static void UnhandledBF(NetState state, PacketReader pvSrc)
		{ }

		public static void Empty(NetState state, PacketReader pvSrc)
		{ }

		public static void SetAbility(NetState state, IEntity e, EncodedReader reader)
		{
			EventSink.InvokeSetAbility(new SetAbilityEventArgs(state.Mobile, reader.ReadInt32()));
		}

		public static void GuildGumpRequest(NetState state, IEntity e, EncodedReader reader)
		{
			EventSink.InvokeGuildGumpRequest(new GuildGumpRequestArgs(state.Mobile));
		}

		public static void QuestGumpRequest(NetState state, IEntity e, EncodedReader reader)
		{
			EventSink.InvokeQuestGumpRequest(new QuestGumpRequestArgs(state.Mobile));
		}

		public static void EncodedCommand(NetState state, PacketReader pvSrc)
		{
			IEntity e = World.FindEntity(pvSrc.ReadInt32());
			int packetID = pvSrc.ReadUInt16();

			EncodedPacketHandler ph = GetEncodedHandler(packetID);

			if (ph != null)
			{
				if (ph.Ingame && state.Mobile == null)
				{
					Console.WriteLine(
						"Client: {0}: Sent ingame packet (0xD7x{1:X2}) before having been attached to a mobile", state, packetID);
					state.Dispose();
				}
				else if (ph.Ingame && state.Mobile.Deleted)
				{
					state.Dispose();
				}
				else
				{
					ph.OnReceive(state, e, new EncodedReader(pvSrc));
				}
			}
			else
			{
				pvSrc.Trace(state);
			}
		}

		public static void RenameRequest(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;
			Mobile targ = World.FindMobile(pvSrc.ReadInt32());

			if (targ != null)
			{
				EventSink.InvokeRenameRequest(new RenameRequestEventArgs(from, targ, pvSrc.ReadStringSafe()));
			}
		}

		public static void ChatRequest(NetState state, PacketReader pvSrc)
		{
			EventSink.InvokeChatRequest(new ChatRequestEventArgs(state.Mobile));
		}

		public static void SecureTrade(NetState state, PacketReader pvSrc)
		{
			switch (pvSrc.ReadByte())
			{
				case 1: // Cancel
				{
					Serial serial = pvSrc.ReadInt32();

					SecureTradeContainer cont = World.FindItem(serial) as SecureTradeContainer;

					if (cont != null)
					{
						SecureTrade trade = cont.Trade;

						if (trade != null)
						{
							if (trade.From.Mobile == state.Mobile || trade.To.Mobile == state.Mobile)
							{
								trade.Cancel();
							}
						}
					}
				}
					break;
				case 2: // Check
				{
					Serial serial = pvSrc.ReadInt32();

					SecureTradeContainer cont = World.FindItem(serial) as SecureTradeContainer;

					if (cont != null)
					{
						SecureTrade trade = cont.Trade;

						bool value = pvSrc.ReadInt32() != 0;

						if (trade != null)
						{
							if (trade.From.Mobile == state.Mobile)
							{
								trade.From.Accepted = value;
								trade.Update();
							}
							else if (trade.To.Mobile == state.Mobile)
							{
								trade.To.Accepted = value;
								trade.Update();
							}
						}
					}
				}
					break;
				case 3: // Update Gold
				{
					Serial serial = pvSrc.ReadInt32();

					SecureTradeContainer cont = World.FindItem(serial) as SecureTradeContainer;

					if (cont != null)
					{
						int gold = pvSrc.ReadInt32();
						int plat = pvSrc.ReadInt32();

						SecureTrade trade = cont.Trade;

						if (trade != null)
						{
							if (trade.From.Mobile == state.Mobile)
							{
								trade.From.Gold = gold;
								trade.From.Plat = plat;
								trade.UpdateFromCurrency();
							}
							else if (trade.To.Mobile == state.Mobile)
							{
								trade.To.Gold = gold;
								trade.To.Plat = plat;
								trade.UpdateToCurrency();
							}
						}
					}
				}
					break;
			}
		}

		public static void VendorBuyReply(NetState state, PacketReader pvSrc)
		{
			pvSrc.Seek(1, SeekOrigin.Begin);

			int msgSize = pvSrc.ReadUInt16();
			Mobile vendor = World.FindMobile(pvSrc.ReadInt32());
			byte flag = pvSrc.ReadByte();

			if (vendor == null)
			{
				return;
			}
			else if (vendor.Deleted || !Utility.RangeCheck(vendor.Location, state.Mobile.Location, 10))
			{
				state.Send(new EndVendorBuy(vendor));
				return;
			}

			if (flag == 0x02)
			{
				msgSize -= 1 + 2 + 4 + 1;

				if ((msgSize / 7) > 100)
				{
					return;
				}

				var buyList = new List<BuyItemResponse>(msgSize / 7);
				for (; msgSize > 0; msgSize -= 7)
				{
					byte layer = pvSrc.ReadByte();
					Serial serial = pvSrc.ReadInt32();
					int amount = pvSrc.ReadInt16();

					buyList.Add(new BuyItemResponse(serial, amount));
				}

				if (buyList.Count > 0)
				{
					IVendor v = vendor as IVendor;

					if (v != null && v.OnBuyItems(state.Mobile, buyList))
					{
						state.Send(new EndVendorBuy(vendor));
					}
				}
			}
			else
			{
				state.Send(new EndVendorBuy(vendor));
			}
		}

		public static void VendorSellReply(NetState state, PacketReader pvSrc)
		{
			Serial serial = pvSrc.ReadInt32();
			Mobile vendor = World.FindMobile(serial);

			if (vendor == null)
			{
				return;
			}
			else if (vendor.Deleted || !Utility.RangeCheck(vendor.Location, state.Mobile.Location, 10))
			{
				state.Send(new EndVendorSell(vendor));
				return;
			}

			int count = pvSrc.ReadUInt16();
			if (count < 100 && pvSrc.Size == (1 + 2 + 4 + 2 + (count * 6)))
			{
				var sellList = new List<SellItemResponse>(count);

				for (int i = 0; i < count; i++)
				{
					Item item = World.FindItem(pvSrc.ReadInt32());
					int Amount = pvSrc.ReadInt16();

					if (item != null && Amount > 0)
					{
						sellList.Add(new SellItemResponse(item, Amount));
					}
				}

				if (sellList.Count > 0)
				{
					IVendor v = vendor as IVendor;

					if (v != null && v.OnSellItems(state.Mobile, sellList))
					{
						state.Send(new EndVendorSell(vendor));
					}
				}
			}
		}

		public static void DeleteCharacter(NetState state, PacketReader pvSrc)
		{
			pvSrc.Seek(30, SeekOrigin.Current);
			int index = pvSrc.ReadInt32();

			EventSink.InvokeDeleteRequest(new DeleteRequestEventArgs(state, index));
		}

		public static void ResourceQuery(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{ }
		}

		public static void GameCentralMoniter(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				int type = pvSrc.ReadByte();
				int num1 = pvSrc.ReadInt32();

				Console.WriteLine("God Client: {0}: Game central moniter", state);
				Console.WriteLine(" - Type: {0}", type);
				Console.WriteLine(" - Number: {0}", num1);

				pvSrc.Trace(state);
			}
		}

		public static void GodviewQuery(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				Console.WriteLine("God Client: {0}: Godview query 0x{1:X}", state, pvSrc.ReadByte());
			}
		}

		public static void GMSingle(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				pvSrc.Trace(state);
			}
		}

		public static void DeathStatusResponse(NetState state, PacketReader pvSrc)
		{
			// Ignored
		}

		public static void ObjectHelpRequest(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			Serial serial = pvSrc.ReadInt32();
			int unk = pvSrc.ReadByte();
			string lang = pvSrc.ReadString(3);

			if (serial.IsItem)
			{
				Item item = World.FindItem(serial);

				if (item != null && from.Map == item.Map && Utility.InUpdateRange(item.GetWorldLocation(), from.Location) &&
					from.CanSee(item))
				{
					item.OnHelpRequest(from);
				}
			}
			else if (serial.IsMobile)
			{
				Mobile m = World.FindMobile(serial);

				if (m != null && from.Map == m.Map && Utility.InUpdateRange(m.Location, from.Location) && from.CanSee(m))
				{
					m.OnHelpRequest(m);
				}
			}
		}

		public static void MobileNameRequest(NetState state, PacketReader pvSrc)
		{
			Mobile m = World.FindMobile(pvSrc.ReadInt32());

			if (m != null && Utility.InUpdateRange(state.Mobile, m) && state.Mobile.CanSee(m))
			{
				state.Send(new MobileName(m));
			}
		}

		public static void RequestScrollWindow(NetState state, PacketReader pvSrc)
		{
			int lastTip = pvSrc.ReadInt16();
			int type = pvSrc.ReadByte();
		}

		public static void AttackReq(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;
			Mobile m = World.FindMobile(pvSrc.ReadInt32());

			if (m != null)
			{
				from.Attack(m);
			}
		}

		public static void HuePickerResponse(NetState state, PacketReader pvSrc)
		{
			int serial = pvSrc.ReadInt32();
			int value = pvSrc.ReadInt16();
			int hue = pvSrc.ReadInt16() & 0x3FFF;

			hue = Utility.ClipDyedHue(hue);

			foreach (HuePicker huePicker in state.HuePickers)
			{
				if (huePicker.Serial == serial)
				{
					state.RemoveHuePicker(huePicker);

					huePicker.OnResponse(hue);

					break;
				}
			}
		}

		public static void TripTime(NetState state, PacketReader pvSrc)
		{
			int unk1 = pvSrc.ReadByte();
			int unk2 = pvSrc.ReadInt32();

			state.Send(new TripTimeResponse(unk1));
		}

		public static void UTripTime(NetState state, PacketReader pvSrc)
		{
			int unk1 = pvSrc.ReadByte();
			int unk2 = pvSrc.ReadInt32();

			state.Send(new UTripTimeResponse(unk1));
		}

		public static void ChangeZ(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				int x = pvSrc.ReadInt16();
				int y = pvSrc.ReadInt16();
				int z = pvSrc.ReadSByte();

				Console.WriteLine("God Client: {0}: Change Z ({1}, {2}, {3})", state, x, y, z);
			}
		}

		public static void SystemInfo(NetState state, PacketReader pvSrc)
		{
			int v1 = pvSrc.ReadByte();
			int v2 = pvSrc.ReadUInt16();
			int v3 = pvSrc.ReadByte();
			string s1 = pvSrc.ReadString(32);
			string s2 = pvSrc.ReadString(32);
			string s3 = pvSrc.ReadString(32);
			string s4 = pvSrc.ReadString(32);
			int v4 = pvSrc.ReadUInt16();
			int v5 = pvSrc.ReadUInt16();
			int v6 = pvSrc.ReadInt32();
			int v7 = pvSrc.ReadInt32();
			int v8 = pvSrc.ReadInt32();
		}

		public static void Edit(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				int type = pvSrc.ReadByte(); // 10 = static, 7 = npc, 4 = dynamic
				int x = pvSrc.ReadInt16();
				int y = pvSrc.ReadInt16();
				int id = pvSrc.ReadInt16();
				int z = pvSrc.ReadSByte();
				int hue = pvSrc.ReadUInt16();

				Console.WriteLine("God Client: {0}: Edit {6} ({1}, {2}, {3}) 0x{4:X} (0x{5:X})", state, x, y, z, id, hue, type);
			}
		}

		public static void DeleteStatic(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				int x = pvSrc.ReadInt16();
				int y = pvSrc.ReadInt16();
				int z = pvSrc.ReadInt16();
				int id = pvSrc.ReadUInt16();

				Console.WriteLine("God Client: {0}: Delete Static ({1}, {2}, {3}) 0x{4:X}", state, x, y, z, id);
			}
		}

		public static void NewAnimData(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				Console.WriteLine("God Client: {0}: New tile animation", state);

				pvSrc.Trace(state);
			}
		}

		public static void NewTerrain(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				int x = pvSrc.ReadInt16();
				int y = pvSrc.ReadInt16();
				int id = pvSrc.ReadUInt16();
				int width = pvSrc.ReadInt16();
				int height = pvSrc.ReadInt16();

				Console.WriteLine("God Client: {0}: New Terrain ({1}, {2})+({3}, {4}) 0x{5:X4}", state, x, y, width, height, id);
			}
		}

		public static void NewRegion(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				string name = pvSrc.ReadString(40);
				int unk = pvSrc.ReadInt32();
				int x = pvSrc.ReadInt16();
				int y = pvSrc.ReadInt16();
				int width = pvSrc.ReadInt16();
				int height = pvSrc.ReadInt16();
				int zStart = pvSrc.ReadInt16();
				int zEnd = pvSrc.ReadInt16();
				string desc = pvSrc.ReadString(40);
				int soundFX = pvSrc.ReadInt16();
				int music = pvSrc.ReadInt16();
				int nightFX = pvSrc.ReadInt16();
				int dungeon = pvSrc.ReadByte();
				int light = pvSrc.ReadInt16();

				Console.WriteLine("God Client: {0}: New Region '{1}' ('{2}')", state, name, desc);
			}
		}

		public static void AccountID(NetState state, PacketReader pvSrc)
		{ }

		public static bool VerifyGC(NetState state)
		{
			if (state.Mobile == null || state.Mobile.IsPlayer())
			{
				if (state.Running)
				{
					Console.WriteLine("Warning: {0}: Player using godclient, disconnecting", state);
				}

				state.Dispose();
				return false;
			}
			else
			{
				return true;
			}
		}

		public static void TextCommand(NetState state, PacketReader pvSrc)
		{
			int type = pvSrc.ReadByte();
			string command = pvSrc.ReadString();

			Mobile m = state.Mobile;

			switch (type)
			{
				case 0x00: // Go
					{
						if (VerifyGC(state))
						{
							try
							{
								var split = command.Split(' ');

								int x = Utility.ToInt32(split[0]);
								int y = Utility.ToInt32(split[1]);

								int z;

								if (split.Length >= 3)
								{
									z = Utility.ToInt32(split[2]);
								}
								else if (m.Map != null)
								{
									z = m.Map.GetAverageZ(x, y);
								}
								else
								{
									z = 0;
								}

								m.Location = new Point3D(x, y, z);
							}
							catch
							{ }
						}

						break;
					}
				case 0xC7: // Animate
					{
						EventSink.InvokeAnimateRequest(new AnimateRequestEventArgs(m, command));

						break;
					}
				case 0x24: // Use skill
					{
						int skillIndex;

						if (!int.TryParse(command.Split(' ')[0], out skillIndex))
						{
							break;
						}

						Skills.UseSkill(m, skillIndex);

						break;
					}
				case 0x43: // Open spellbook
					{
						int booktype;

						if (!int.TryParse(command, out booktype))
						{
							booktype = 1;
						}

						EventSink.InvokeOpenSpellbookRequest(new OpenSpellbookRequestEventArgs(m, booktype));

						break;
					}
				case 0x27: // Cast spell from book
					{
						var split = command.Split(' ');

						if (split.Length > 0)
						{
							int spellID = Utility.ToInt32(split[0]) - 1;
							int serial = split.Length > 1 ? Utility.ToInt32(split[1]) : -1;

							EventSink.InvokeCastSpellRequest(new CastSpellRequestEventArgs(m, spellID, World.FindItem(serial)));
						}

						break;
					}
				case 0x58: // Open door
					{
						EventSink.InvokeOpenDoorMacroUsed(new OpenDoorMacroEventArgs(m));

						break;
					}
				case 0x56: // Cast spell from macro
					{
						int spellID = Utility.ToInt32(command) - 1;

						EventSink.InvokeCastSpellRequest(new CastSpellRequestEventArgs(m, spellID, null));

						break;
					}
				case 0xF4: // Invoke virtues from macro
					{
						int virtueID = Utility.ToInt32(command) - 1;

						EventSink.InvokeVirtueMacroRequest(new VirtueMacroRequestEventArgs(m, virtueID));

						break;
					}
				case 0x2F: // Old scroll double click
					{
						/*
					 * This command is still sent for items 0xEF3 - 0xEF9
					 *
					 * Command is one of three, depending on the item ID of the scroll:
					 * - [scroll serial]
					 * - [scroll serial] [target serial]
					 * - [scroll serial] [x] [y] [z]
					 */
						break;
					}
				default:
					{
						Console.WriteLine("Client: {0}: Unknown text-command type 0x{1:X2}: {2}", state, type, command);
						break;
					}
			}
		}

		public static void GodModeRequest(NetState state, PacketReader pvSrc)
		{
			if (VerifyGC(state))
			{
				state.Send(new GodModeReply(pvSrc.ReadBoolean()));
			}
		}

		public static void AsciiPromptResponse(NetState state, PacketReader pvSrc)
		{
			int serial = pvSrc.ReadInt32();
			int prompt = pvSrc.ReadInt32();
			int type = pvSrc.ReadInt32();
			string text = pvSrc.ReadStringSafe();

			if (text.Length > 128)
			{
				return;
			}

			Mobile from = state.Mobile;
			Prompt p = from.Prompt;

			if (p != null && p.Serial == serial && p.Serial == prompt)
			{
				from.Prompt = null;

				if (type == 0)
				{
					p.OnCancel(from);
				}
				else
				{
					p.OnResponse(from, text);
				}
			}
		}

		public static void UnicodePromptResponse(NetState state, PacketReader pvSrc)
		{
			int serial = pvSrc.ReadInt32();
			int prompt = pvSrc.ReadInt32();
			int type = pvSrc.ReadInt32();
			string lang = pvSrc.ReadString(4);
			string text = pvSrc.ReadUnicodeStringLESafe();

			if (text.Length > 128)
			{
				return;
			}

			Mobile from = state.Mobile;
			Prompt p = from.Prompt;

			if (p != null && p.Serial == serial && p.Serial == prompt)
			{
				from.Prompt = null;

				if (type == 0)
				{
					p.OnCancel(from);
				}
				else
				{
					p.OnResponse(from, text);
				}
			}
		}

		public static void MenuResponse(NetState state, PacketReader pvSrc)
		{
			int serial = pvSrc.ReadInt32();
			int menuID = pvSrc.ReadInt16(); // unused in our implementation
			int index = pvSrc.ReadInt16();
			int itemID = pvSrc.ReadInt16();
			int hue = pvSrc.ReadInt16();

			index -= 1; // convert from 1-based to 0-based

			foreach (IMenu menu in state.Menus)
			{
				if (menu.Serial == serial)
				{
					state.RemoveMenu(menu);

					if (index >= 0 && index < menu.EntryLength)
					{
						menu.OnResponse(state, index);
					}
					else
					{
						menu.OnCancel(state);
					}

					break;
				}
			}
		}

		public static void ProfileReq(NetState state, PacketReader pvSrc)
		{
			int type = pvSrc.ReadByte();
			Serial serial = pvSrc.ReadInt32();

			Mobile beholder = state.Mobile;
			Mobile beheld = World.FindMobile(serial);

			if (beheld == null)
			{
				return;
			}

			switch (type)
			{
				case 0x00: // display request
					{
						EventSink.InvokeProfileRequest(new ProfileRequestEventArgs(beholder, beheld));

						break;
					}
				case 0x01: // edit request
					{
						pvSrc.ReadInt16(); // Skip
						int length = pvSrc.ReadUInt16();

						if (length > 511)
						{
							return;
						}

						string text = pvSrc.ReadUnicodeString(length);

						EventSink.InvokeChangeProfileRequest(new ChangeProfileRequestEventArgs(beholder, beheld, text));

						break;
					}
			}
		}

		public static void Disconnect(NetState state, PacketReader pvSrc)
		{
			int minusOne = pvSrc.ReadInt32();
		}

		public static void LiftReq(NetState state, PacketReader pvSrc)
		{
			Serial serial = pvSrc.ReadInt32();
			int amount = pvSrc.ReadUInt16();
			Item item = World.FindItem(serial);

			bool rejected;
			LRReason reject;

			state.Mobile.Lift(item, amount, out rejected, out reject);
		}

		public static void EquipReq(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;
			Item item = from.Holding;

			bool valid = (item != null && item.HeldBy == from && item.Map == Map.Internal);

			from.Holding = null;

			if (!valid)
			{
				return;
			}

			pvSrc.Seek(5, SeekOrigin.Current);
			Mobile to = World.FindMobile(pvSrc.ReadInt32());

			if (to == null)
			{
				to = from;
			}

			if (!to.AllowEquipFrom(from) || !to.EquipItem(item))
			{
				item.Bounce(from);
			}

			item.ClearBounce();
		}

		public static void DropReq(NetState state, PacketReader pvSrc)
		{
			pvSrc.ReadInt32(); // serial, ignored
			int x = pvSrc.ReadInt16();
			int y = pvSrc.ReadInt16();
			int z = pvSrc.ReadSByte();
			Serial dest = pvSrc.ReadInt32();

			Point3D loc = new Point3D(x, y, z);

			Mobile from = state.Mobile;

			if (dest.IsMobile)
			{
				from.Drop(World.FindMobile(dest), loc);
			}
			else if (dest.IsItem)
			{
				Item item = World.FindItem(dest);

				if (item is BaseMulti && ((BaseMulti)item).AllowsRelativeDrop)
				{
					loc.m_X += item.X;
					loc.m_Y += item.Y;
					from.Drop(loc);
				}
				else
				{
					from.Drop(item, loc);
				}
			}
			else
			{
				from.Drop(loc);
			}
		}

		public static void DropReq6017(NetState state, PacketReader pvSrc)
		{
			pvSrc.ReadInt32(); // serial, ignored
			int x = pvSrc.ReadInt16();
			int y = pvSrc.ReadInt16();
			int z = pvSrc.ReadSByte();
			pvSrc.ReadByte(); // Grid Location?
			Serial dest = pvSrc.ReadInt32();

			Point3D loc = new Point3D(x, y, z);

			Mobile from = state.Mobile;

			if (dest.IsMobile)
			{
				from.Drop(World.FindMobile(dest), loc);
			}
			else if (dest.IsItem)
			{
				Item item = World.FindItem(dest);

				if (item is BaseMulti && ((BaseMulti)item).AllowsRelativeDrop)
				{
					loc.m_X += item.X;
					loc.m_Y += item.Y;
					from.Drop(loc);
				}
				else
				{
					from.Drop(item, loc);
				}
			}
			else
			{
				from.Drop(loc);
			}
		}

		public static void ConfigurationFile(NetState state, PacketReader pvSrc)
		{ }

		public static void LogoutReq(NetState state, PacketReader pvSrc)
		{
			state.Send(new LogoutAck());
		}

		public static void ChangeSkillLock(NetState state, PacketReader pvSrc)
		{
			Skill s = state.Mobile.Skills[pvSrc.ReadInt16()];

			if (s != null)
			{
				s.SetLockNoRelay((SkillLock)pvSrc.ReadByte());
			}
		}

		public static void HelpRequest(NetState state, PacketReader pvSrc)
		{
			EventSink.InvokeHelpRequest(new HelpRequestEventArgs(state.Mobile));
		}

		public static void TargetResponse(NetState state, PacketReader pvSrc)
		{
			int type = pvSrc.ReadByte();
			int targetID = pvSrc.ReadInt32();
			int flags = pvSrc.ReadByte();
			Serial serial = pvSrc.ReadInt32();
			int x = pvSrc.ReadInt16(), y = pvSrc.ReadInt16(), z = pvSrc.ReadInt16();
			int graphic = pvSrc.ReadUInt16();

			if (targetID == unchecked((int)0xDEADBEEF))
			{
				return;
			}

			Mobile from = state.Mobile;

			Target t = from.Target;

			if (t != null)
			{
				TargetProfile prof = TargetProfile.Acquire(t.GetType());

				if (prof != null)
				{
					prof.Start();
				}

				try
				{
					if (x == -1 && y == -1 && !serial.IsValid)
					{
						// User pressed escape
						t.Cancel(from, TargetCancelType.Canceled);
					}
					else if (Target.TargetIDValidation && t.TargetID != targetID)
					{
						// Prevent fake target, reported by AssistUO Team!
						return;
					}
					else
					{
						object toTarget;

						if (type == 1)
						{
							if (graphic == 0)
							{
								toTarget = new LandTarget(new Point3D(x, y, z), from.Map);
							}
							else
							{
								Map map = from.Map;

								if (map == null || map == Map.Internal)
								{
									t.Cancel(from, TargetCancelType.Canceled);
									return;
								}
								else
								{
									var tiles = map.Tiles.GetStaticTiles(x, y, !t.DisallowMultis);

									bool valid = false;

									if (state.HighSeas)
									{
										ItemData id = TileData.ItemTable[graphic & TileData.MaxItemValue];
										if (id.Surface)
										{
											z -= id.Height;
										}
									}

									for (int i = 0; !valid && i < tiles.Length; ++i)
									{
										if (tiles[i].Z == z && tiles[i].ID == graphic)
										{
											valid = true;
										}
									}

									if (!valid)
									{
										t.Cancel(from, TargetCancelType.Canceled);
										return;
									}
									else
									{
										toTarget = new StaticTarget(new Point3D(x, y, z), graphic);
									}
								}
							}
						}
						else if (serial.IsMobile)
						{
							toTarget = World.FindMobile(serial);
						}
						else if (serial.IsItem)
						{
							toTarget = World.FindItem(serial);
						}
						else
						{
							t.Cancel(from, TargetCancelType.Canceled);
							return;
						}

						t.Invoke(from, toTarget);
					}
				}
				finally
				{
					if (prof != null)
					{
						prof.Finish();
					}
				}
			}
		}

		public static void DisplayGumpResponse(NetState state, PacketReader pvSrc)
		{
			int serial = pvSrc.ReadInt32();
			int typeID = pvSrc.ReadInt32();
			int buttonID = pvSrc.ReadInt32();

			foreach (Gump gump in state.Gumps)
			{
				if (gump.Serial == serial && gump.TypeID == typeID)
				{
					int switchCount = pvSrc.ReadInt32();

					if (switchCount < 0 || switchCount > gump.m_Switches)
					{
						Utility.PushColor(ConsoleColor.DarkRed);
						state.WriteConsole("Invalid gump response, disconnecting...");
						Utility.PopColor();
						state.Dispose();
						return;
					}

					var switches = new int[switchCount];

					for (int j = 0; j < switches.Length; ++j)
					{
						switches[j] = pvSrc.ReadInt32();
					}

					int textCount = pvSrc.ReadInt32();

					if (textCount < 0 || textCount > gump.m_TextEntries)
					{
						Utility.PushColor(ConsoleColor.DarkRed);
						state.WriteConsole("Invalid gump response, disconnecting...");
						Utility.PopColor();
						state.Dispose();
						return;
					}

					var textEntries = new TextRelay[textCount];

					for (int j = 0; j < textEntries.Length; ++j)
					{
						int entryID = pvSrc.ReadUInt16();
						int textLength = pvSrc.ReadUInt16();

						if (textLength > 239)
						{
							Utility.PushColor(ConsoleColor.DarkRed);
							state.WriteConsole("Invalid gump response, disconnecting...");
							Utility.PopColor();
							state.Dispose();
							return;
						}

						string text = pvSrc.ReadUnicodeStringSafe(textLength);
						textEntries[j] = new TextRelay(entryID, text);
					}

					state.RemoveGump(gump);

					GumpProfile prof = GumpProfile.Acquire(gump.GetType());

					if (prof != null)
					{
						prof.Start();
					}

					gump.OnResponse(state, new RelayInfo(buttonID, switches, textEntries));

					if (prof != null)
					{
						prof.Finish();
					}

					return;
				}
			}

			if (typeID == 461)
			{
				// Virtue gump
				int switchCount = pvSrc.ReadInt32();

				if (buttonID == 1 && switchCount > 0)
				{
					Mobile beheld = World.FindMobile(pvSrc.ReadInt32());

					if (beheld != null)
					{
						EventSink.InvokeVirtueGumpRequest(new VirtueGumpRequestEventArgs(state.Mobile, beheld));
					}
				}
				else
				{
					Mobile beheld = World.FindMobile(serial);

					if (beheld != null)
					{
						EventSink.InvokeVirtueItemRequest(new VirtueItemRequestEventArgs(state.Mobile, beheld, buttonID));
					}
				}
			}
		}

		public static void SetWarMode(NetState state, PacketReader pvSrc)
		{
			state.Mobile.DelayChangeWarmode(pvSrc.ReadBoolean());
		}

		public static void Resynchronize(NetState state, PacketReader pvSrc)
		{
			Mobile m = state.Mobile;

			if (state.StygianAbyss)
			{
				state.Send(new MobileUpdate(m));
			}
			else
			{
				state.Send(new MobileUpdateOld(m));
			}

			state.Send(MobileIncoming.Create(state, m, m));

			m.SendEverything();

			state.Sequence = 0;

			m.ClearFastwalkStack();
		}

		private static readonly int[] m_EmptyInts = new int[0];

		public static void AsciiSpeech(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			MessageType type = (MessageType)pvSrc.ReadByte();
			int hue = pvSrc.ReadInt16();
			pvSrc.ReadInt16(); // font
			string text = pvSrc.ReadStringSafe().Trim();

			if (text.Length <= 0 || text.Length > 128)
			{
				return;
			}

			if (!Enum.IsDefined(typeof(MessageType), type))
			{
				type = MessageType.Regular;
			}

			from.DoSpeech(text, m_EmptyInts, type, Utility.ClipDyedHue(hue));
		}

		private static readonly KeywordList m_KeywordList = new KeywordList();

		public static void UnicodeSpeech(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			MessageType type = (MessageType)pvSrc.ReadByte();
			int hue = pvSrc.ReadInt16();
			pvSrc.ReadInt16(); // font
			string lang = pvSrc.ReadString(4);
			string text;

			bool isEncoded = (type & MessageType.Encoded) != 0;
			int[] keywords;

			if (isEncoded)
			{
				int value = pvSrc.ReadInt16();
				int count = (value & 0xFFF0) >> 4;
				int hold = value & 0xF;

				if (count < 0 || count > 50)
				{
					return;
				}

				KeywordList keyList = m_KeywordList;

				for (int i = 0; i < count; ++i)
				{
					int speechID;

					if ((i & 1) == 0)
					{
						hold <<= 8;
						hold |= pvSrc.ReadByte();
						speechID = hold;
						hold = 0;
					}
					else
					{
						value = pvSrc.ReadInt16();
						speechID = (value & 0xFFF0) >> 4;
						hold = value & 0xF;
					}

					if (!keyList.Contains(speechID))
					{
						keyList.Add(speechID);
					}
				}

				text = pvSrc.ReadUTF8StringSafe();

				keywords = keyList.ToArray();
			}
			else
			{
				text = pvSrc.ReadUnicodeStringSafe();

				keywords = m_EmptyInts;
			}

			text = text.Trim();

			if (text.Length <= 0 || text.Length > 128)
			{
				return;
			}

			type &= ~MessageType.Encoded;

			if (!Enum.IsDefined(typeof(MessageType), type))
			{
				type = MessageType.Regular;
			}

			from.Language = lang;
			from.DoSpeech(text, keywords, type, Utility.ClipDyedHue(hue));
		}

		public static void UseReq(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			if (from.IsStaff() || Core.TickCount - from.NextActionTime >= 0)
			{
				int value = pvSrc.ReadInt32();

				if ((value & ~0x7FFFFFFF) != 0)
				{
					from.OnPaperdollRequest();
				}
				else
				{
					Serial s = value;

					if (s.IsMobile)
					{
						Mobile m = World.FindMobile(s);

						if (m != null && !m.Deleted)
						{
							from.Use(m);
						}
					}
					else if (s.IsItem)
					{
						Item item = World.FindItem(s);

						if (item != null && !item.Deleted)
						{
							from.Use(item);
						}
					}
				}

				from.NextActionTime = Core.TickCount + Mobile.ActionDelay;
			}
			else
			{
				from.SendActionMessage();
			}
		}

		private static bool m_SingleClickProps;

		public static bool SingleClickProps { get { return m_SingleClickProps; } set { m_SingleClickProps = value; } }

		public static void LookReq(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			Serial s = pvSrc.ReadInt32();

			if (s.IsMobile)
			{
				Mobile m = World.FindMobile(s);

				if (m != null && from.CanSee(m) && Utility.InUpdateRange(from, m))
				{
					if (m_SingleClickProps)
					{
						m.OnAosSingleClick(from);
					}
					else
					{
						if (from.Region.OnSingleClick(from, m))
						{
							m.OnSingleClick(from);
						}
					}
				}
			}
			else if (s.IsItem)
			{
				Item item = World.FindItem(s);

				if (item != null && !item.Deleted && from.CanSee(item) &&
					Utility.InUpdateRange(from.Location, item.GetWorldLocation()))
				{
					if (m_SingleClickProps)
					{
						item.OnAosSingleClick(from);
					}
					else if (from.Region.OnSingleClick(from, item))
					{
						if (item.Parent is Item)
						{
							((Item)item.Parent).OnSingleClickContained(from, item);
						}

						item.OnSingleClick(from);
					}
				}
			}
		}

		public static void PingReq(NetState state, PacketReader pvSrc)
		{
			state.Send(PingAck.Instantiate(pvSrc.ReadByte()));
		}

		public static void SetUpdateRange(NetState state, PacketReader pvSrc)
		{
			state.Send(ChangeUpdateRange.Instantiate(18));
		}

		private const int BadFood = unchecked((int)0xBAADF00D);
		private const int BadUOTD = unchecked((int)0xFFCEFFCE);

		public static void MovementReq(NetState state, PacketReader pvSrc)
		{
			Direction dir = (Direction)pvSrc.ReadByte();
			int seq = pvSrc.ReadByte();
			int key = pvSrc.ReadInt32();

			Mobile m = state.Mobile;

			if ((state.Sequence == 0 && seq != 0) || !m.Move(dir))
			{
				state.Send(new MovementRej(seq, m));
				state.Sequence = 0;

				m.ClearFastwalkStack();
			}
			else
			{
				++seq;

				if (seq == 256)
				{
					seq = 1;
				}

				state.Sequence = seq;
			}
		}

		public static int[] m_ValidAnimations = new[]
		{
			6, 21, 32, 33, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
			120, 121, 123, 124, 125, 126, 127, 128
		};

		public static int[] ValidAnimations { get { return m_ValidAnimations; } set { m_ValidAnimations = value; } }

		public static void Animate(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;
			int action = pvSrc.ReadInt32();

			bool ok = false;

			for (int i = 0; !ok && i < m_ValidAnimations.Length; ++i)
			{
				ok = (action == m_ValidAnimations[i]);
			}

			if (from != null && ok && from.Alive && from.Body.IsHuman && !from.Mounted)
			{
				from.Animate(action, 7, 1, true, false, 0);
			}
		}

		public static void QuestArrow(NetState state, PacketReader pvSrc)
		{
			bool rightClick = pvSrc.ReadBoolean();
			Mobile from = state.Mobile;

			if (from != null && from.QuestArrow != null)
			{
				from.QuestArrow.OnClick(rightClick);
			}
		}

		public static void ExtendedCommand(NetState state, PacketReader pvSrc)
		{
			int packetID = pvSrc.ReadUInt16();

			PacketHandler ph = GetExtendedHandler(packetID);

			if (ph != null)
			{
				if (ph.Ingame && state.Mobile == null)
				{
					Console.WriteLine(
						"Client: {0}: Sent ingame packet (0xBFx{1:X2}) before having been attached to a mobile", state, packetID);
					state.Dispose();
				}
				else if (ph.Ingame && state.Mobile.Deleted)
				{
					state.Dispose();
				}
				else
				{
					ph.OnReceive(state, pvSrc);
				}
			}
			else
			{
				pvSrc.Trace(state);
			}
		}

		public static void CastSpell(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			if (from == null)
			{
				return;
			}

			Item spellbook = null;

			if (pvSrc.ReadInt16() == 1)
			{
				spellbook = World.FindItem(pvSrc.ReadInt32());
			}

			int spellID = pvSrc.ReadInt16() - 1;

			EventSink.InvokeCastSpellRequest(new CastSpellRequestEventArgs(from, spellID, spellbook));
		}

		public static void BandageTarget(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			if (from == null)
			{
				return;
			}

			if (from.IsStaff() || Core.TickCount - from.NextActionTime >= 0)
			{
				Item bandage = World.FindItem(pvSrc.ReadInt32());

				if (bandage == null)
				{
					return;
				}

				Mobile target = World.FindMobile(pvSrc.ReadInt32());

				if (target == null)
				{
					return;
				}

				EventSink.InvokeBandageTargetRequest(new BandageTargetRequestEventArgs(from, bandage, target));

				from.NextActionTime = Core.TickCount + Mobile.ActionDelay;
			}
			else
			{
				from.SendActionMessage();
			}
		}

		#region Stygain Abyss
		public static void ToggleFlying(NetState state, PacketReader pvSrc)
		{
			state.Mobile.ToggleFlying();
		}
		#endregion

		public static void BatchQueryProperties(NetState state, PacketReader pvSrc)
		{
			if (!ObjectPropertyList.Enabled)
			{
				return;
			}

			Mobile from = state.Mobile;

			int length = pvSrc.Size - 3;

			if (length < 0 || (length % 4) != 0)
			{
				return;
			}

			int count = length / 4;

			for (int i = 0; i < count; ++i)
			{
				Serial s = pvSrc.ReadInt32();

				if (s.IsMobile)
				{
					Mobile m = World.FindMobile(s);

					if (m != null && from.CanSee(m) && Utility.InUpdateRange(from, m))
					{
						m.SendPropertiesTo(from);
					}
				}
				else if (s.IsItem)
				{
					Item item = World.FindItem(s);

					if (item != null && !item.Deleted && from.CanSee(item) &&
						Utility.InUpdateRange(from.Location, item.GetWorldLocation()))
					{
						item.SendPropertiesTo(from);
					}
				}
			}
		}

		public static void QueryProperties(NetState state, PacketReader pvSrc)
		{
			if (!ObjectPropertyList.Enabled)
			{
				return;
			}

			Mobile from = state.Mobile;

			Serial s = pvSrc.ReadInt32();

			if (s.IsMobile)
			{
				Mobile m = World.FindMobile(s);

				if (m != null && from.CanSee(m) && Utility.InUpdateRange(from, m))
				{
					m.SendPropertiesTo(from);
				}
			}
			else if (s.IsItem)
			{
				Item item = World.FindItem(s);

				if (item != null && !item.Deleted && from.CanSee(item) &&
					Utility.InUpdateRange(from.Location, item.GetWorldLocation()))
				{
					item.SendPropertiesTo(from);
				}
			}
		}

		public static void PartyMessage(NetState state, PacketReader pvSrc)
		{
			if (state.Mobile == null)
			{
				return;
			}

			switch (pvSrc.ReadByte())
			{
				case 0x01:
					PartyMessage_AddMember(state, pvSrc);
					break;
				case 0x02:
					PartyMessage_RemoveMember(state, pvSrc);
					break;
				case 0x03:
					PartyMessage_PrivateMessage(state, pvSrc);
					break;
				case 0x04:
					PartyMessage_PublicMessage(state, pvSrc);
					break;
				case 0x06:
					PartyMessage_SetCanLoot(state, pvSrc);
					break;
				case 0x08:
					PartyMessage_Accept(state, pvSrc);
					break;
				case 0x09:
					PartyMessage_Decline(state, pvSrc);
					break;
				default:
					pvSrc.Trace(state);
					break;
			}
		}

		public static void PartyMessage_AddMember(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnAdd(state.Mobile);
			}
		}

		public static void PartyMessage_RemoveMember(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnRemove(state.Mobile, World.FindMobile(pvSrc.ReadInt32()));
			}
		}

		public static void PartyMessage_PrivateMessage(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnPrivateMessage(
					state.Mobile, World.FindMobile(pvSrc.ReadInt32()), pvSrc.ReadUnicodeStringSafe());
			}
		}

		public static void PartyMessage_PublicMessage(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnPublicMessage(state.Mobile, pvSrc.ReadUnicodeStringSafe());
			}
		}

		public static void PartyMessage_SetCanLoot(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnSetCanLoot(state.Mobile, pvSrc.ReadBoolean());
			}
		}

		public static void PartyMessage_Accept(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnAccept(state.Mobile, World.FindMobile(pvSrc.ReadInt32()));
			}
		}

		public static void PartyMessage_Decline(NetState state, PacketReader pvSrc)
		{
			if (PartyCommands.Handler != null)
			{
				PartyCommands.Handler.OnDecline(state.Mobile, World.FindMobile(pvSrc.ReadInt32()));
			}
		}

		public static void StunRequest(NetState state, PacketReader pvSrc)
		{
			EventSink.InvokeStunRequest(new StunRequestEventArgs(state.Mobile));
		}

		public static void DisarmRequest(NetState state, PacketReader pvSrc)
		{
			EventSink.InvokeDisarmRequest(new DisarmRequestEventArgs(state.Mobile));
		}

		public static void StatLockChange(NetState state, PacketReader pvSrc)
		{
			int stat = pvSrc.ReadByte();
			int lockValue = pvSrc.ReadByte();

			if (lockValue > 2)
			{
				lockValue = 0;
			}

			Mobile m = state.Mobile;

			if (m != null)
			{
				switch (stat)
				{
					case 0:
						m.StrLock = (StatLockType)lockValue;
						break;
					case 1:
						m.DexLock = (StatLockType)lockValue;
						break;
					case 2:
						m.IntLock = (StatLockType)lockValue;
						break;
				}
			}
		}

		public static void ScreenSize(NetState state, PacketReader pvSrc)
		{
			int width = pvSrc.ReadInt32();
			int unk = pvSrc.ReadInt32();
		}

		public static void ContextMenuResponse(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			if (from != null)
			{
				ContextMenu menu = from.ContextMenu;

				from.ContextMenu = null;

				if (menu != null && from != null && from == menu.From)
				{
					IEntity entity = World.FindEntity(pvSrc.ReadInt32());

					if (entity != null && entity == menu.Target && from.CanSee(entity))
					{
						Point3D p;

						if (entity is Mobile)
						{
							p = entity.Location;
						}
						else if (entity is Item)
						{
							p = ((Item)entity).GetWorldLocation();
						}
						else
						{
							return;
						}

						int index = pvSrc.ReadUInt16();

						if (index >= 0 && index < menu.Entries.Length)
						{
							ContextMenuEntry e = menu.Entries[index];

							int range = e.Range;

							if (range == -1)
							{
								range = 18;
							}

							if (e.Enabled && from.InRange(p, range))
							{
								e.OnClick();
							}
						}
					}
				}
			}
		}

		public static void ContextMenuRequest(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;
			IEntity target = World.FindEntity(pvSrc.ReadInt32());

			if (from != null && target != null && from.Map == target.Map && from.CanSee(target))
			{
				if (target is Mobile && !Utility.InUpdateRange(from.Location, target.Location))
				{
					return;
				}
				else if (target is Item && !Utility.InUpdateRange(from.Location, ((Item)target).GetWorldLocation()))
				{
					return;
				}

				if (!from.CheckContextMenuDisplay(target))
				{
					return;
				}

				ContextMenu c = new ContextMenu(from, target);

				if (c.Entries.Length > 0)
				{
					if (target is Item)
					{
						object root = ((Item)target).RootParent;

						if (root is Mobile && root != from && ((Mobile)root).AccessLevel >= from.AccessLevel)
						{
							for (int i = 0; i < c.Entries.Length; ++i)
							{
								if (!c.Entries[i].NonLocalUse)
								{
									c.Entries[i].Enabled = false;
								}
							}
						}
					}

					from.ContextMenu = c;
				}
			}
		}

		public static void CloseStatus(NetState state, PacketReader pvSrc)
		{
			Serial serial = pvSrc.ReadInt32();
		}

		public static void Language(NetState state, PacketReader pvSrc)
		{
			string lang = pvSrc.ReadString(4);

			if (state.Mobile != null)
			{
				state.Mobile.Language = lang;
			}
		}

		public static void AssistVersion(NetState state, PacketReader pvSrc)
		{
			int unk = pvSrc.ReadInt32();
			string av = pvSrc.ReadString();
		}

		public static void ClientVersion(NetState state, PacketReader pvSrc)
		{
			CV version = state.Version = new CV(pvSrc.ReadString());

			EventSink.InvokeClientVersionReceived(new ClientVersionReceivedArgs(state, version));
		}

		public static void ClientType(NetState state, PacketReader pvSrc)
		{
			pvSrc.ReadUInt16();

			int type = pvSrc.ReadUInt16();
			CV version = state.Version = new CV(pvSrc.ReadString());

			//EventSink.InvokeClientVersionReceived( new ClientVersionReceivedArgs( state, version ) );//todo
		}

		public static void MobileQuery(NetState state, PacketReader pvSrc)
		{
			Mobile from = state.Mobile;

			pvSrc.ReadInt32(); // 0xEDEDEDED
			int type = pvSrc.ReadByte();
			Mobile m = World.FindMobile(pvSrc.ReadInt32());

			if (m != null)
			{
				switch (type)
				{
					case 0x00: // Unknown, sent by godclient
						{
							if (VerifyGC(state))
							{
								Console.WriteLine("God Client: {0}: Query 0x{1:X2} on {2} '{3}'", state, type, m.Serial, m.Name);
							}

							break;
						}
					case 0x04: // Stats
						{
							m.OnStatsQuery(from);
							break;
						}
					case 0x05:
						{
							m.OnSkillsQuery(from);
							break;
						}
					default:
						{
							pvSrc.Trace(state);
							break;
						}
				}
			}
		}

		public delegate void PlayCharCallback(NetState state, bool val);

		public static PlayCharCallback ThirdPartyAuthCallback = null, ThirdPartyHackedCallback = null;

		private static readonly byte[] m_ThirdPartyAuthKey = new byte[]
		{0x9, 0x11, 0x83, (byte)'+', 0x4, 0x17, 0x83, 0x5, 0x24, 0x85, 0x7, 0x17, 0x87, 0x6, 0x19, 0x88,};

		private class LoginTimer : Timer
		{
			private readonly NetState m_State;
			private readonly Mobile m_Mobile;

			public LoginTimer(NetState state, Mobile m)
				: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
			{
				m_State = state;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				if (m_State == null)
				{
					Stop();
				}
				if (m_State.Version != null)
				{
					m_State.BlockAllPackets = false;
					DoLogin(m_State, m_Mobile);
					Stop();
				}
			}
		}

		public static void PlayCharacter(NetState state, PacketReader pvSrc)
		{
			pvSrc.ReadInt32(); // 0xEDEDEDED

			string name = pvSrc.ReadString(30);

			pvSrc.Seek(2, SeekOrigin.Current);
			int flags = pvSrc.ReadInt32();

			if (FeatureProtection.DisabledFeatures != 0 && ThirdPartyAuthCallback != null)
			{
				bool authOK = false;

				ulong razorFeatures = (((ulong)pvSrc.ReadUInt32())<<32) | ((ulong)pvSrc.ReadUInt32());

				if (razorFeatures == (ulong)FeatureProtection.DisabledFeatures)
				{
					bool match = true;
					for (int i = 0; match && i < m_ThirdPartyAuthKey.Length; i++)
					{
						match = match && pvSrc.ReadByte() == m_ThirdPartyAuthKey[i];
					}

					if (match)
					{
						authOK = true;
					}
				}
				else
				{
					pvSrc.Seek(16, SeekOrigin.Current);
				}

				ThirdPartyAuthCallback(state, authOK);
			}
			else
			{
				pvSrc.Seek(24, SeekOrigin.Current);
			}

			if (ThirdPartyHackedCallback != null)
			{
				pvSrc.Seek(-2, SeekOrigin.Current);
				if (pvSrc.ReadUInt16() == 0xDEAD)
				{
					ThirdPartyHackedCallback(state, true);
				}
			}

			if (!state.Running)
			{
				return;
			}

			int charSlot = pvSrc.ReadInt32();
			int clientIP = pvSrc.ReadInt32();

			IAccount a = state.Account;

			if (a == null || charSlot < 0 || charSlot >= a.Length)
			{
				state.Dispose();
			}
			else
			{
				Mobile m = a[charSlot];

				// Check if anyone is using this account
				for (int i = 0; i < a.Length; ++i)
				{
					Mobile check = a[i];

					if (check != null && check.Map != Map.Internal && check != m)
					{
						Utility.PushColor(ConsoleColor.Red);
						Console.WriteLine("Login: {0}: Account in use", state);
						Utility.PopColor();
						state.Send(new PopupMessage(PMMessage.CharInWorld));
						return;
					}
				}

				if (m == null)
				{
					state.Dispose();
				}
				else
				{
					if (m.NetState != null)
					{
						m.NetState.Dispose();
					}

					NetState.ProcessDisposedQueue();

					state.Send(new ClientVersionReq());

					state.BlockAllPackets = true;

					state.Flags = (ClientFlags)flags;

					state.Mobile = m;
					m.NetState = state;

					new LoginTimer(state, m).Start();
				}
			}
		}

		public static void DoLogin(NetState state, Mobile m)
		{
			state.Send(new LoginConfirm(m));

			if (m.Map != null)
			{
				state.Send(new MapChange(m));
			}

			state.Send(new MapPatches());

			state.Send(SeasonChange.Instantiate(m.GetSeason(), true));

			state.Send(SupportedFeatures.Instantiate(state));

			state.Sequence = 0;

			if (state.NewMobileIncoming)
			{
				state.Send(new MobileUpdate(m));
				state.Send(new MobileUpdate(m));

				m.CheckLightLevels(true);

				state.Send(new MobileUpdate(m));

				state.Send(new MobileIncoming(m, m));
				//state.Send( new MobileAttributes( m ) );
				state.Send(new MobileStatus(m, m));
				state.Send(Network.SetWarMode.Instantiate(m.Warmode));

				m.SendEverything();

				state.Send(SupportedFeatures.Instantiate(state));
				state.Send(new MobileUpdate(m));
				//state.Send( new MobileAttributes( m ) );
				state.Send(new MobileStatus(m, m));
				state.Send(Network.SetWarMode.Instantiate(m.Warmode));
				state.Send(new MobileIncoming(m, m));
			}
			else if (state.StygianAbyss)
			{
				state.Send(new MobileUpdate(m));
				state.Send(new MobileUpdate(m));

				m.CheckLightLevels(true);

				state.Send(new MobileUpdate(m));

				state.Send(new MobileIncomingSA(m, m));
				//state.Send( new MobileAttributes( m ) );
				state.Send(new MobileStatus(m, m));
				state.Send(Network.SetWarMode.Instantiate(m.Warmode));

				m.SendEverything();

				state.Send(SupportedFeatures.Instantiate(state));
				state.Send(new MobileUpdate(m));
				//state.Send( new MobileAttributes( m ) );
				state.Send(new MobileStatus(m, m));
				state.Send(Network.SetWarMode.Instantiate(m.Warmode));
				state.Send(new MobileIncomingSA(m, m));
			}
			else
			{
				state.Send(new MobileUpdateOld(m));
				state.Send(new MobileUpdateOld(m));

				m.CheckLightLevels(true);

				state.Send(new MobileUpdateOld(m));

				state.Send(new MobileIncomingOld(m, m));
				//state.Send( new MobileAttributes( m ) );
				state.Send(new MobileStatus(m, m));
				state.Send(Network.SetWarMode.Instantiate(m.Warmode));

				m.SendEverything();

				state.Send(SupportedFeatures.Instantiate(state));
				state.Send(new MobileUpdateOld(m));
				//state.Send( new MobileAttributes( m ) );
				state.Send(new MobileStatus(m, m));
				state.Send(Network.SetWarMode.Instantiate(m.Warmode));
				state.Send(new MobileIncomingOld(m, m));
			}

			state.Send(LoginComplete.Instance);
			state.Send(new CurrentTime());
			state.Send(SeasonChange.Instantiate(m.GetSeason(), true));
			state.Send(new MapChange(m));

			EventSink.InvokeLogin(new LoginEventArgs(m));

			m.ClearFastwalkStack();
		}

		public static void CreateCharacter(NetState state, PacketReader pvSrc)
		{
			int unk1 = pvSrc.ReadInt32();
			int unk2 = pvSrc.ReadInt32();
			int unk3 = pvSrc.ReadByte();
			string name = pvSrc.ReadString(30);

			pvSrc.Seek(2, SeekOrigin.Current);
			int flags = pvSrc.ReadInt32();
			pvSrc.Seek(8, SeekOrigin.Current);
			int prof = pvSrc.ReadByte();
			pvSrc.Seek(15, SeekOrigin.Current);

			//bool female = pvSrc.ReadBoolean();

			int genderRace = pvSrc.ReadByte();

			int str = pvSrc.ReadByte();
			int dex = pvSrc.ReadByte();
			int intl = pvSrc.ReadByte();
			int is1 = pvSrc.ReadByte();
			int vs1 = pvSrc.ReadByte();
			int is2 = pvSrc.ReadByte();
			int vs2 = pvSrc.ReadByte();
			int is3 = pvSrc.ReadByte();
			int vs3 = pvSrc.ReadByte();
			int hue = pvSrc.ReadUInt16();
			int hairVal = pvSrc.ReadInt16();
			int hairHue = pvSrc.ReadInt16();
			int hairValf = pvSrc.ReadInt16();
			int hairHuef = pvSrc.ReadInt16();
			pvSrc.ReadByte();
			int cityIndex = pvSrc.ReadByte();
			int charSlot = pvSrc.ReadInt32();
			int clientIP = pvSrc.ReadInt32();
			int shirtHue = pvSrc.ReadInt16();
			int pantsHue = pvSrc.ReadInt16();

			/*
			Pre-7.0.0.0:
			0x00, 0x01 -> Human Male, Human Female
			0x02, 0x03 -> Elf Male, Elf Female

			Post-7.0.0.0:
			0x00, 0x01
			0x02, 0x03 -> Human Male, Human Female
			0x04, 0x05 -> Elf Male, Elf Female
			0x05, 0x06 -> Gargoyle Male, Gargoyle Female
			*/

			bool female = ((genderRace % 2) != 0);

			Race race = null;

			if (state.StygianAbyss)
			{
				byte raceID = (byte)(genderRace < 4 ? 0 : ((genderRace / 2) - 1));
				race = Race.Races[raceID];
			}
			else
			{
				race = Race.Races[(byte)(genderRace / 2)];
			}

			if (race == null)
			{
				race = Race.DefaultRace;
			}

			var info = state.CityInfo;
			IAccount a = state.Account;

			if (info == null || a == null || cityIndex < 0 || cityIndex >= info.Length)
			{
				state.Dispose();
			}
			else
			{
				// Check if anyone is using this account
				for (int i = 0; i < a.Length; ++i)
				{
					Mobile check = a[i];

					if (check != null && check.Map != Map.Internal)
					{
						Utility.PushColor(ConsoleColor.Red);
						Console.WriteLine("Login: {0}: Account in use", state);
						Utility.PopColor();
						state.Send(new PopupMessage(PMMessage.CharInWorld));
						return;
					}
				}

				state.Flags = (ClientFlags)flags;

				CharacterCreatedEventArgs args = new CharacterCreatedEventArgs(
					state,
					a,
					name,
					female,
					hue,
					str,
					dex,
					intl,
					info[cityIndex],
					new SkillNameValue[3]
					{
						new SkillNameValue((SkillName)is1, vs1), new SkillNameValue((SkillName)is2, vs2),
						new SkillNameValue((SkillName)is3, vs3),
					},
					shirtHue,
					pantsHue,
					hairVal,
					hairHue,
					hairValf,
					hairHuef,
					prof,
					race);

				state.Send(new ClientVersionReq());

				state.BlockAllPackets = true;

				EventSink.InvokeCharacterCreated(args);

				Mobile m = args.Mobile;

				if (m != null)
				{
					state.Mobile = m;
					m.NetState = state;
					new LoginTimer(state, m).Start();
				}
				else
				{
					state.BlockAllPackets = false;
					state.Dispose();
				}
			}
		}

		public static void CreateCharacter70160(NetState state, PacketReader pvSrc)
		{
			int unk1 = pvSrc.ReadInt32();
			int unk2 = pvSrc.ReadInt32();
			int unk3 = pvSrc.ReadByte();
			string name = pvSrc.ReadString(30);

			pvSrc.Seek(2, SeekOrigin.Current);
			int flags = pvSrc.ReadInt32();
			pvSrc.Seek(8, SeekOrigin.Current);
			int prof = pvSrc.ReadByte();
			pvSrc.Seek(15, SeekOrigin.Current);

			int genderRace = pvSrc.ReadByte();

			int str = pvSrc.ReadByte();
			int dex = pvSrc.ReadByte();
			int intl = pvSrc.ReadByte();
			int is1 = pvSrc.ReadByte();
			int vs1 = pvSrc.ReadByte();
			int is2 = pvSrc.ReadByte();
			int vs2 = pvSrc.ReadByte();
			int is3 = pvSrc.ReadByte();
			int vs3 = pvSrc.ReadByte();
			int is4 = pvSrc.ReadByte();
			int vs4 = pvSrc.ReadByte();

			int hue = pvSrc.ReadUInt16();
			int hairVal = pvSrc.ReadInt16();
			int hairHue = pvSrc.ReadInt16();
			int hairValf = pvSrc.ReadInt16();
			int hairHuef = pvSrc.ReadInt16();
			pvSrc.ReadByte();
			int cityIndex = pvSrc.ReadByte();
			int charSlot = pvSrc.ReadInt32();
			int clientIP = pvSrc.ReadInt32();
			int shirtHue = pvSrc.ReadInt16();
			int pantsHue = pvSrc.ReadInt16();

			/*
			0x00, 0x01
			0x02, 0x03 -> Human Male, Human Female
			0x04, 0x05 -> Elf Male, Elf Female
			0x05, 0x06 -> Gargoyle Male, Gargoyle Female
			*/

			bool female = ((genderRace % 2) != 0);

			Race race = null;

			byte raceID = (byte)(genderRace < 4 ? 0 : ((genderRace / 2) - 1));
			race = Race.Races[raceID];

			if (race == null)
			{
				race = Race.DefaultRace;
			}

			var info = state.CityInfo;
			IAccount a = state.Account;

			if (info == null || a == null || cityIndex < 0 || cityIndex >= info.Length)
			{
				state.Dispose();
			}
			else
			{
				// Check if anyone is using this account
				for (int i = 0; i < a.Length; ++i)
				{
					Mobile check = a[i];

					if (check != null && check.Map != Map.Internal)
					{
						Utility.PushColor(ConsoleColor.Red);
						Console.WriteLine("Login: {0}: Account in use", state);
						Utility.PopColor();
						state.Send(new PopupMessage(PMMessage.CharInWorld));
						return;
					}
				}

				state.Flags = (ClientFlags)flags;

				CharacterCreatedEventArgs args = new CharacterCreatedEventArgs(
					state,
					a,
					name,
					female,
					hue,
					str,
					dex,
					intl,
					info[cityIndex],
					new SkillNameValue[4]
					{
						new SkillNameValue((SkillName)is1, vs1), new SkillNameValue((SkillName)is2, vs2),
						new SkillNameValue((SkillName)is3, vs3), new SkillNameValue((SkillName)is4, vs4),
					},
					shirtHue,
					pantsHue,
					hairVal,
					hairHue,
					hairValf,
					hairHuef,
					prof,
					race);

				state.Send(new ClientVersionReq());

				state.BlockAllPackets = true;

				EventSink.InvokeCharacterCreated(args);

				Mobile m = args.Mobile;

				if (m != null)
				{
					state.Mobile = m;
					m.NetState = state;
					new LoginTimer(state, m).Start();
				}
				else
				{
					state.BlockAllPackets = false;
					state.Dispose();
				}
			}
		}

		private static bool m_ClientVerification = true;

		public static bool ClientVerification { get { return m_ClientVerification; } set { m_ClientVerification = value; } }

		internal struct AuthIDPersistence
		{
			public DateTime Age;
			public ClientVersion Version;

			public AuthIDPersistence(ClientVersion v)
			{
				Age = DateTime.UtcNow;
				Version = v;
			}
		}

		private const int m_AuthIDWindowSize = 128;

		private static readonly Dictionary<int, AuthIDPersistence> m_AuthIDWindow =
			new Dictionary<int, AuthIDPersistence>(m_AuthIDWindowSize);

		private static int GenerateAuthID(NetState state)
		{
			if (m_AuthIDWindow.Count == m_AuthIDWindowSize)
			{
				int oldestID = 0;
				DateTime oldest = DateTime.MaxValue;

				foreach (var kvp in m_AuthIDWindow)
				{
					if (kvp.Value.Age < oldest)
					{
						oldestID = kvp.Key;
						oldest = kvp.Value.Age;
					}
				}

				m_AuthIDWindow.Remove(oldestID);
			}

			int authID;

			do
			{
				authID = Utility.Random(1, int.MaxValue - 1);

				if (Utility.RandomBool())
				{
					authID |= 1 << 31;
				}
			}
			while (m_AuthIDWindow.ContainsKey(authID));

			m_AuthIDWindow[authID] = new AuthIDPersistence(state.Version);

			return authID;
		}

		public static void GameLogin(NetState state, PacketReader pvSrc)
		{
			if (state.SentFirstPacket)
			{
				state.Dispose();
				return;
			}

			state.SentFirstPacket = true;

			int authID = pvSrc.ReadInt32();

			if (m_AuthIDWindow.ContainsKey(authID))
			{
				AuthIDPersistence ap = m_AuthIDWindow[authID];
				m_AuthIDWindow.Remove(authID);

				state.Version = ap.Version;
			}
			else if (m_ClientVerification)
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				Console.WriteLine("Login: {0}: Invalid client detected, disconnecting", state);
				Utility.PopColor();
				state.Dispose();
				return;
			}

			if (state.m_AuthID != 0 && authID != state.m_AuthID)
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				Console.WriteLine("Login: {0}: Invalid client detected, disconnecting", state);
				Utility.PopColor();
				state.Dispose();
				return;
			}
			else if (state.m_AuthID == 0 && authID != state.m_Seed)
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				Console.WriteLine("Login: {0}: Invalid client detected, disconnecting", state);
				Utility.PopColor();
				state.Dispose();
				return;
			}

			string username = pvSrc.ReadString(30);
			string password = pvSrc.ReadString(30);

			GameLoginEventArgs e = new GameLoginEventArgs(state, username, password);

			EventSink.InvokeGameLogin(e);

			if (e.Accepted)
			{
				state.CityInfo = e.CityInfo;
				state.CompressionEnabled = true;

				state.Send(SupportedFeatures.Instantiate(state));

				if (state.NewCharacterList)
				{
					state.Send(new CharacterList(state.Account, state.CityInfo));
				}
				else
				{
					state.Send(new CharacterListOld(state.Account, state.CityInfo));
				}
			}
			else
			{
				state.Dispose();
			}
		}

		public static void PlayServer(NetState state, PacketReader pvSrc)
		{
			int index = pvSrc.ReadInt16();
			var info = state.ServerInfo;
			IAccount a = state.Account;

			if (info == null || a == null || index < 0 || index >= info.Length)
			{
				state.Dispose();
			}
			else
			{
				ServerInfo si = info[index];

				state.m_AuthID = PlayServerAck.m_AuthID = GenerateAuthID(state);

				state.SentFirstPacket = false;
				state.Send(new PlayServerAck(si));
			}
		}

		public static void LoginServerSeed(NetState state, PacketReader pvSrc)
		{
			state.m_Seed = pvSrc.ReadInt32();
			state.Seeded = true;

			if (state.m_Seed == 0)
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				Console.WriteLine("Login: {0}: Invalid client detected, disconnecting", state);
				Utility.PopColor();
				state.Dispose();
				return;
			}

			int clientMaj = pvSrc.ReadInt32();
			int clientMin = pvSrc.ReadInt32();
			int clientRev = pvSrc.ReadInt32();
			int clientPat = pvSrc.ReadInt32();

			state.Version = new ClientVersion(clientMaj, clientMin, clientRev, clientPat);
		}

		public static void CrashReport(NetState state, PacketReader pvSrc)
		{
			byte clientMaj = pvSrc.ReadByte();
			byte clientMin = pvSrc.ReadByte();
			byte clientRev = pvSrc.ReadByte();
			byte clientPat = pvSrc.ReadByte();

			ushort x = pvSrc.ReadUInt16();
			ushort y = pvSrc.ReadUInt16();
			sbyte z = pvSrc.ReadSByte();
			byte map = pvSrc.ReadByte();

			string account = pvSrc.ReadString(32);
			string character = pvSrc.ReadString(32);
			string ip = pvSrc.ReadString(15);

			int unk1 = pvSrc.ReadInt32();
			int exception = pvSrc.ReadInt32();

			string process = pvSrc.ReadString(100);
			string report = pvSrc.ReadString(100);

			pvSrc.ReadByte(); // 0x00

			int offset = pvSrc.ReadInt32();

			int count = pvSrc.ReadByte();

			for (int i = 0; i < count; i++)
			{
				int address = pvSrc.ReadInt32();
			}
		}

		public static void AccountLogin(NetState state, PacketReader pvSrc)
		{
			if (state.SentFirstPacket)
			{
				state.Dispose();
				return;
			}

			state.SentFirstPacket = true;

			string username = pvSrc.ReadString(30);
			string password = pvSrc.ReadString(30);

			AccountLoginEventArgs e = new AccountLoginEventArgs(state, username, password);

			EventSink.InvokeAccountLogin(e);

			if (e.Accepted)
			{
				AccountLogin_ReplyAck(state);
			}
			else
			{
				AccountLogin_ReplyRej(state, e.RejectReason);
			}
		}

		public static void AccountLogin_ReplyAck(NetState state)
		{
			ServerListEventArgs e = new ServerListEventArgs(state, state.Account);

			EventSink.InvokeServerList(e);

			if (e.Rejected)
			{
				state.Account = null;
				state.Send(new AccountLoginRej(ALRReason.BadComm));
				state.Dispose();
			}
			else
			{
				var info = e.Servers.ToArray();

				state.ServerInfo = info;

				state.Send(new AccountLoginAck(info));
			}
		}

		public static void AccountLogin_ReplyRej(NetState state, ALRReason reason)
		{
			state.Send(new AccountLoginRej(reason));
			state.Dispose();
		}
	}
}
