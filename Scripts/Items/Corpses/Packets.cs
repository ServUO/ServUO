#region References
using System.IO;

using Server.Items;
#endregion

namespace Server.Network
{
	public sealed class CorpseEquip : Packet
	{
		public CorpseEquip(Mobile beholder, Corpse beheld)
			: base(0x89)
		{
			var list = beheld.EquipItems;

			int count = list.Count;
			if (beheld.Hair != null && beheld.Hair.ItemID > 0)
			{
				count++;
			}
			if (beheld.FacialHair != null && beheld.FacialHair.ItemID > 0)
			{
				count++;
			}

			EnsureCapacity(8 + (count * 5));

			m_Stream.Write(beheld.Serial);

			for (int i = 0; i < list.Count; ++i)
			{
				Item item = list[i];

				if (!item.Deleted && beholder.CanSee(item) && item.Parent == beheld)
				{
					m_Stream.Write((byte)(item.Layer + 1));
					m_Stream.Write(item.Serial);
				}
			}

			if (beheld.Hair != null && beheld.Hair.ItemID > 0)
			{
				m_Stream.Write((byte)(Layer.Hair + 1));
				m_Stream.Write(HairInfo.FakeSerial(beheld.Owner) - 2);
			}

			if (beheld.FacialHair != null && beheld.FacialHair.ItemID > 0)
			{
				m_Stream.Write((byte)(Layer.FacialHair + 1));
				m_Stream.Write(FacialHairInfo.FakeSerial(beheld.Owner) - 2);
			}

			m_Stream.Write((byte)Layer.Invalid);
		}
	}

	public sealed class CorpseContent : Packet
	{
		public CorpseContent(Mobile beholder, Corpse beheld)
			: base(0x3C)
		{
			var items = beheld.EquipItems;
			int count = items.Count;

			if (beheld.Hair != null && beheld.Hair.ItemID > 0)
			{
				count++;
			}
			if (beheld.FacialHair != null && beheld.FacialHair.ItemID > 0)
			{
				count++;
			}

			EnsureCapacity(5 + (count * 19));

			long pos = m_Stream.Position;

			int written = 0;

			m_Stream.Write((ushort)0);

			for (int i = 0; i < items.Count; ++i)
			{
				Item child = items[i];

				if (!child.Deleted && child.Parent == beheld && beholder.CanSee(child))
				{
					m_Stream.Write(child.Serial);
					m_Stream.Write((ushort)child.ItemID);
					m_Stream.Write((byte)0); // signed, itemID offset
					m_Stream.Write((ushort)child.Amount);
					m_Stream.Write((short)child.X);
					m_Stream.Write((short)child.Y);
					m_Stream.Write(beheld.Serial);
					m_Stream.Write((ushort)child.Hue);

					++written;
				}
			}

			if (beheld.Hair != null && beheld.Hair.ItemID > 0)
			{
				m_Stream.Write(HairInfo.FakeSerial(beheld.Owner) - 2);
				m_Stream.Write((ushort)beheld.Hair.ItemID);
				m_Stream.Write((byte)0); // signed, itemID offset
				m_Stream.Write((ushort)1);
				m_Stream.Write((short)0);
				m_Stream.Write((short)0);
				m_Stream.Write(beheld.Serial);
				m_Stream.Write((ushort)beheld.Hair.Hue);

				++written;
			}

			if (beheld.FacialHair != null && beheld.FacialHair.ItemID > 0)
			{
				m_Stream.Write(FacialHairInfo.FakeSerial(beheld.Owner) - 2);
				m_Stream.Write((ushort)beheld.FacialHair.ItemID);
				m_Stream.Write((byte)0); // signed, itemID offset
				m_Stream.Write((ushort)1);
				m_Stream.Write((short)0);
				m_Stream.Write((short)0);
				m_Stream.Write(beheld.Serial);
				m_Stream.Write((ushort)beheld.FacialHair.Hue);

				++written;
			}

			m_Stream.Seek(pos, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}

	public sealed class CorpseContent6017 : Packet
	{
		public CorpseContent6017(Mobile beholder, Corpse beheld)
			: base(0x3C)
		{
			var items = beheld.EquipItems;
			int count = items.Count;

			if (beheld.Hair != null && beheld.Hair.ItemID > 0)
			{
				count++;
			}
			if (beheld.FacialHair != null && beheld.FacialHair.ItemID > 0)
			{
				count++;
			}

			EnsureCapacity(5 + (count * 20));

			long pos = m_Stream.Position;

			int written = 0;

			m_Stream.Write((ushort)0);

			for (int i = 0; i < items.Count; ++i)
			{
				Item child = items[i];

				if (!child.Deleted && child.Parent == beheld && beholder.CanSee(child))
				{
					m_Stream.Write(child.Serial);
					m_Stream.Write((ushort)child.ItemID);
					m_Stream.Write((byte)0); // signed, itemID offset
					m_Stream.Write((ushort)child.Amount);
					m_Stream.Write((short)child.X);
					m_Stream.Write((short)child.Y);
					m_Stream.Write((byte)0); // Grid Location?
					m_Stream.Write(beheld.Serial);
					m_Stream.Write((ushort)child.Hue);

					++written;
				}
			}

			if (beheld.Hair != null && beheld.Hair.ItemID > 0)
			{
				m_Stream.Write(HairInfo.FakeSerial(beheld.Owner) - 2);
				m_Stream.Write((ushort)beheld.Hair.ItemID);
				m_Stream.Write((byte)0); // signed, itemID offset
				m_Stream.Write((ushort)1);
				m_Stream.Write((short)0);
				m_Stream.Write((short)0);
				m_Stream.Write((byte)0); // Grid Location?
				m_Stream.Write(beheld.Serial);
				m_Stream.Write((ushort)beheld.Hair.Hue);

				++written;
			}

			if (beheld.FacialHair != null && beheld.FacialHair.ItemID > 0)
			{
				m_Stream.Write(FacialHairInfo.FakeSerial(beheld.Owner) - 2);
				m_Stream.Write((ushort)beheld.FacialHair.ItemID);
				m_Stream.Write((byte)0); // signed, itemID offset
				m_Stream.Write((ushort)1);
				m_Stream.Write((short)0);
				m_Stream.Write((short)0);
				m_Stream.Write((byte)0); // Grid Location?
				m_Stream.Write(beheld.Serial);
				m_Stream.Write((ushort)beheld.FacialHair.Hue);

				++written;
			}

			m_Stream.Seek(pos, SeekOrigin.Begin);
			m_Stream.Write((ushort)written);
		}
	}
}