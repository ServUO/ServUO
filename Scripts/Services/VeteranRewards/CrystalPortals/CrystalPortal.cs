#region References
using System;
using System.Collections.Generic;

using Server.Factions;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Multis;
using Server.ContextMenus;
#endregion

namespace Server.Items
{
	public class CrystalPortal : Item, ISecurable
	{
        public override int LabelNumber { get { return 1113945; } } // Crystal Portal

		private SecureLevel m_Level;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level 
		{
			get { return m_Level; }
			set { m_Level = value; }
		}
		
		public override bool HandlesOnSpeech { get { return true; } }

		[Constructable]
		public CrystalPortal()
            : base(0x468B)
		{
			Weight = 5.0;
			Movable = true;
			LootType = LootType.Blessed;
		}

		public CrystalPortal(Serial serial)
			: base(serial)
		{ }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

		public virtual bool ValidateUse(Mobile m, bool message)
		{
            BaseHouse house = BaseHouse.FindHouseAt(this);

			if (house == null || !IsLockedDown)
			{
				if (message)
				{
					m.SendMessage("This must be locked down in a house to use!");
				}

				return false;
			}

            if (!house.HasSecureAccess(m, m_Level))
            {
                if (message)
                {
                    m.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                }

                return false;
            }

			if (Sigil.ExistsOn(m))
			{
				if (message)
				{
					m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
				}

				return false;
			}

			if (WeightOverloading.IsOverloaded(m))
			{
				if (message)
				{
					m.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
				}

				return false;
			}

			if (m.Criminal)
			{
				if (message)
				{
					m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
				}

				return false;
			}

			if (SpellHelper.CheckCombat(m))
			{
				if (message)
				{
					m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				}

				return false;
			}

			if (m.Spell != null)
			{
				if (message)
				{
					m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
				}

				return false;
			}

			return true;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!m.InRange(Location, 3))
			{
				m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			if (ValidateUse(m, true))
			{
				m.SendGump(new CrystalPortalGump(m));
			}
		}

		public virtual void OnTeleport(Mobile m, Point3D loc, Map map)
		{
			if (m == null || loc == Point3D.Zero || map == null || map == Map.Internal)
			{
				return;
			}

			Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
			Effects.PlaySound(m.Location, m.Map, 0x1FE);

			BaseCreature.TeleportPets(m, loc, map);
			m.MoveToWorld(loc, map);

			Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
			Effects.PlaySound(m.Location, m.Map, 0x1FE);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (e.Handled || e.Blocked || !e.Mobile.InRange(Location, 2))
			{
				return;
			}

			Point3D loc = Point3D.Zero;
			Map map = null;

			ResolveDest(e.Speech.Trim(), ref loc, ref map);

			if (loc == Point3D.Zero || map == null || map == Map.Internal || (Siege.SiegeShard && map == Map.Trammel))
			{
				return;
			}

			e.Handled = true;

			if (ValidateUse(e.Mobile, true))
			{
				OnTeleport(e.Mobile, loc, map);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version			
			writer.WriteEncodedInt((int)m_Level);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Level = (SecureLevel)reader.ReadEncodedInt();

            if (version < 1)
            {
                ItemID = 0x468B;
                Hue = 0;
                Weight = 5.0;
            }
        }

		public static void ResolveDest(string name, ref Point3D loc, ref Map map)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				return;
			}

			switch (name.Trim().ToLower())
			{
				// tram banks
				case "britain mint":
				{
					loc = new Point3D(1434, 1699, 2);
					map = Map.Trammel;
				}
					break;
				case "bucs mint":
				{
					loc = new Point3D(2724, 2192, 0);
					map = Map.Trammel;
				}
					break;
				case "cove mint":
				{
					loc = new Point3D(2238, 1195, 0);
					map = Map.Trammel;
				}
					break;
				case "delucia mint":
				{
					loc = new Point3D(5274, 3991, 37);
					map = Map.Trammel;
				}
					break;
				//osi lists new haven listed as simply 'haven'. probably because there's no bank in 'old haven'
				case "haven mint":
				{
					loc = new Point3D(3500, 2571, 14);
					map = Map.Trammel;
				}
					break;
				case "jhelom mint":
				{
					loc = new Point3D(1417, 3821, 0);
					map = Map.Trammel;
				}
					break;
				case "magincia mint":
				{
					loc = new Point3D(3728, 2164, 20);
					map = Map.Trammel;
				}
					break;
				case "minoc mint":
				{
					loc = new Point3D(2498, 561, 0);
					map = Map.Trammel;
				}
					break;
				case "moonglow mint":
				{
					loc = new Point3D(4471, 1177, 0);
					map = Map.Trammel;
				}
					break;
				case "nujelm mint":
				{
					loc = new Point3D(3770, 1308, 0);
					map = Map.Trammel;
				}
					break;
				case "papua mint":
				{
					loc = new Point3D(5675, 3144, 12);
					map = Map.Trammel;
				}
					break;
				case "serpent mint":
				{
					loc = new Point3D(2895, 3479, 15);
					map = Map.Trammel;
				}
					break;
				case "skara mint":
				{
					loc = new Point3D(596, 2138, 0);
					map = Map.Trammel;
				}
					break;
				case "trinsic mint":
				{
					loc = new Point3D(1823, 2821, 0);
					map = Map.Trammel;
				}
					break;
				case "vesper mint":
				{
					loc = new Point3D(2899, 676, 0);
					map = Map.Trammel;
				}
					break;
				case "wind mint":
				{
					loc = new Point3D(5345, 93, 15);
					map = Map.Trammel;
				}
					break;
				case "luna mint":
				{
					loc = new Point3D(1015, 527, -65);
					map = Map.Malas;
				}
					break;
				case "zento mint":
				{
					loc = new Point3D(741, 1261, 30);
					map = Map.Tokuno;
				}
					break;
				case "ilshenar mint":
				{
					loc = new Point3D(1232, 557, -19);
					map = Map.Ilshenar;
				}
					break;

                case "yew mint":
                    {
                        loc = new Point3D(643, 858, 0);
                        map = Map.Trammel;
                    }
                    break;

				// fel banks

				case "fel britain mint":
				{
					loc = new Point3D(1434, 1699, 2);
					map = Map.Felucca;
				}
					break;
				case "fel bucs mint":
				{
					loc = new Point3D(2724, 2192, 0);
					map = Map.Felucca;
				}
					break;
				case "fel cove mint":
				{
					loc = new Point3D(2238, 1195, 0);
					map = Map.Felucca;
				}
					break;
				case "fel ocllo mint":
				{
					loc = new Point3D(3687, 2523, 0);
					map = Map.Felucca;
				}
					break;

				case "fel jhelom mint":
				{
					loc = new Point3D(1417, 3821, 0);
					map = Map.Felucca;
				}
					break;
				case "fel magincia mint":
				{
					loc = new Point3D(3728, 2164, 20);
					map = Map.Felucca;
				}
					break;
				case "fel minoc mint":
				{
					loc = new Point3D(2498, 561, 0);
					map = Map.Felucca;
				}
					break;
				case "fel moonglow mint":
				{
					loc = new Point3D(4471, 1177, 0);
					map = Map.Felucca;
				}
					break;
				case "fel nujelm mint":
				{
					loc = new Point3D(3770, 1308, 0);
					map = Map.Felucca;
				}
					break;

				case "fel serpent mint":
				{
					loc = new Point3D(2895, 3479, 15);
					map = Map.Felucca;
				}
					break;
				case "fel skara mint":
				{
					loc = new Point3D(596, 2138, 0);
					map = Map.Felucca;
				}
					break;
				case "fel trinsic mint":
				{
					loc = new Point3D(1823, 2821, 0);
					map = Map.Felucca;
				}
					break;
				case "fel vesper mint":
				{
					loc = new Point3D(2899, 676, 0);
					map = Map.Felucca;
				}
					break;
				case "fel wind mint":
				{
					loc = new Point3D(1361, 895, 0);
					map = Map.Felucca;
				}
					break;

                case "fel yew mint":
                    {
                        loc = new Point3D(643, 858, 0);
                        map = Map.Felucca;
                    }
                    break;

				// tram moongates

				case "britain moongate":
				{
					loc = new Point3D(1336, 1997, 5);
					map = Map.Trammel;
				}
					break;
				case "haven moongate":
				{
					loc = new Point3D(3763, 2771, 50);
					map = Map.Trammel;
				}
					break;
				case "jhelom moongate":
				{
					loc = new Point3D(1495, 3773, 0);
					map = Map.Trammel;
				}
					break;
				case "magincia moongate":
				{
					loc = new Point3D(3563, 2139, 34);
					map = Map.Trammel;
				}
					break;
				case "minoc moongate":
				{
					loc = new Point3D(2701, 692, 5);
					map = Map.Trammel;
				}
					break;
				case "moonglow moongate":
				{
					loc = new Point3D(4467, 1283, 5);
					map = Map.Trammel;
				}
					break;
				case "skara moongate":
				{
					loc = new Point3D(643, 2067, 5);
					map = Map.Trammel;
				}
					break;
				case "trinsic moongate":
				{
					loc = new Point3D(1828, 2948, -20);
					map = Map.Trammel;
				}
					break;
				// vesper doesn't have it's own moongate, it shares one with minoc. 
				// but osi has an entry for one, clilocs confirm this.		
				case "vesper moongate":
				{
					loc = new Point3D(2701, 692, 5);
					map = Map.Trammel;
				}
					break;
				// yew moongate not included in osi, but i'm adding it.
				case "yew moongate":
				{
					loc = new Point3D(771, 752, 5);
					map = Map.Trammel;
				}
					break;

				case "compassion moongate":
				{
					loc = new Point3D(1215, 467, -13);
					map = Map.Ilshenar;
				}
					break;
				case "honesty moongate":
				{
					loc = new Point3D(722, 1366, -60);
					map = Map.Ilshenar;
				}
					break;
				case "honor moongate":
				{
					loc = new Point3D(744, 724, -28);
					map = Map.Ilshenar;
				}
					break;
				case "humility moongate":
				{
					loc = new Point3D(281, 1016, 0);
					map = Map.Ilshenar;
				}
					break;
				case "justice moongate":
				{
					loc = new Point3D(987, 1011, -32);
					map = Map.Ilshenar;
				}
					break;
				case "sacrifice moongate":
				{
					loc = new Point3D(1174, 1286, -30);
					map = Map.Ilshenar;
				}
					break;
				case "spirituality moongate":
				{
					loc = new Point3D(1532, 1340, -3);
					map = Map.Ilshenar;
				}
					break;
				case "valor moongate":
				{
					loc = new Point3D(528, 216, -45);
					map = Map.Ilshenar;
				}
					break;
				case "chaos moongate":
				{
					loc = new Point3D(1721, 218, 96);
					map = Map.Ilshenar;
				}
					break;

				case "luna moongate":
				{
					loc = new Point3D(1015, 527, -65);
					map = Map.Malas;
				}
					break;
				case "umbra moongate":
				{
					loc = new Point3D(1997, 1386, -85);
					map = Map.Malas;
				}
					break;
				case "termur moongate":
				{
					loc = new Point3D(851, 3526, 0);
					map = Map.TerMur;
				}
					break;
				case "isamu moongate":
				{
					loc = new Point3D(1169, 998, 41);
					map = Map.Tokuno;
				}
					break;
				case "makoto moongate":
				{
					loc = new Point3D(802, 1204, 25);
					map = Map.Tokuno;
				}
					break;
				case "homare moongate":
				{
					loc = new Point3D(270, 628, 15);
					map = Map.Tokuno;
				}
					break;

				// fel moongates

				case "fel britain moongate":
				{
					loc = new Point3D(1336, 1997, 5);
					map = Map.Felucca;
				}
					break;
				case "fel bucs moongate":
				{
					loc = new Point3D(2711, 2234, 0);
					map = Map.Felucca;
				}
					break;
				case "fel jhelom moongate":
				{
					loc = new Point3D(1495, 3773, 0);
					map = Map.Felucca;
				}
					break;
				case "fel magincia moongate":
				{
					loc = new Point3D(3563, 2139, 34);
					map = Map.Felucca;
				}
					break;
				case "fel minoc moongate":
				{
					loc = new Point3D(2701, 692, 5);
					map = Map.Felucca;
				}
					break;
				case "fel moonglow moongate":
				{
					loc = new Point3D(4467, 1283, 5);
					map = Map.Felucca;
				}
					break;
				case "fel skara moongate":
				{
					loc = new Point3D(643, 2067, 5);
					map = Map.Felucca;
				}
					break;
				case "fel trinsic moongate":
				{
					loc = new Point3D(1828, 2948, -20);
					map = Map.Felucca;
				}
					break;
				case "fel yew moongate":
				{
					loc = new Point3D(771, 752, 5);
					map = Map.Felucca;
				}
					break;
				case "fel vesper moongate":
				{
					loc = new Point3D(2701, 692, 5);
					map = Map.Felucca;
				}
					break;
			}
		}
	}
}
