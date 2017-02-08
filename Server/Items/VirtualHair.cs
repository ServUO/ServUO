#region Header
// **********
// ServUO - VirtualHair.cs
// **********
#endregion

#region References
using Server.Network;
#endregion

namespace Server
{
	public abstract class BaseHairInfo
	{
		private int m_ItemID;
		private int m_Hue;

		[CommandProperty(AccessLevel.GameMaster)]
		public int ItemID { get { return m_ItemID; } set { m_ItemID = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Hue { get { return m_Hue; } set { m_Hue = value; } }

		protected BaseHairInfo(int itemid)
			: this(itemid, 0)
		{ }

		protected BaseHairInfo(int itemid, int hue)
		{
			m_ItemID = itemid;
			m_Hue = hue;
		}

		protected BaseHairInfo(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_ItemID = reader.ReadInt();
						m_Hue = reader.ReadInt();
						break;
					}
			}
		}

		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(0); //version
			writer.Write(m_ItemID);
			writer.Write(m_Hue);
		}
	}

	public class HairInfo : BaseHairInfo
	{
		public HairInfo(int itemid)
			: base(itemid, 0)
		{ }

		public HairInfo(int itemid, int hue)
			: base(itemid, hue)
		{ }

		public HairInfo(GenericReader reader)
			: base(reader)
		{ }

		public static int FakeSerial(Mobile parent)
		{
			return (0x7FFFFFFF - 0x400 - (parent.Serial * 4));
		}
	}

	public class FacialHairInfo : BaseHairInfo
	{
		public FacialHairInfo(int itemid)
			: base(itemid, 0)
		{ }

		public FacialHairInfo(int itemid, int hue)
			: base(itemid, hue)
		{ }

		public FacialHairInfo(GenericReader reader)
			: base(reader)
		{ }

		public static int FakeSerial(Mobile parent)
		{
			return (0x7FFFFFFF - 0x400 - 1 - (parent.Serial * 4));
		}
	}

    public class FaceInfo : BaseHairInfo
    {
        public FaceInfo(int itemid)
            : base(itemid, 0)
        {
        }

        public FaceInfo(int itemid, int hue)
            : base(itemid, hue)
        {
        }

        public FaceInfo(GenericReader reader)
            : base(reader)
        {
        }

        public static int FakeSerial(Mobile parent)
        {
            return (0x7FFFFFFF - 0x400 - 2 - (parent.Serial * 4));
        }
    }

    public sealed class HairEquipUpdate : Packet
	{
		public HairEquipUpdate(Mobile parent)
			: base(0x2E, 15)
		{
			int hue = parent.HairHue;

			if (parent.SolidHueOverride >= 0)
			{
				hue = parent.SolidHueOverride;
			}

			int hairSerial = HairInfo.FakeSerial(parent);

			m_Stream.Write(hairSerial);
			m_Stream.Write((short)parent.HairItemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)Layer.Hair);
			m_Stream.Write(parent.Serial);
			m_Stream.Write((short)hue);
		}
	}

	public sealed class FacialHairEquipUpdate : Packet
	{
		public FacialHairEquipUpdate(Mobile parent)
			: base(0x2E, 15)
		{
			int hue = parent.FacialHairHue;

			if (parent.SolidHueOverride >= 0)
			{
				hue = parent.SolidHueOverride;
			}

			int hairSerial = FacialHairInfo.FakeSerial(parent);

			m_Stream.Write(hairSerial);
			m_Stream.Write((short)parent.FacialHairItemID);
			m_Stream.Write((byte)0);
			m_Stream.Write((byte)Layer.FacialHair);
			m_Stream.Write(parent.Serial);
			m_Stream.Write((short)hue);
		}
	}

    public sealed class FaceEquipUpdate : Packet
    {
        public FaceEquipUpdate(Mobile parent)
            : base(0x2E, 15)
        {
            int hue = parent.FaceHue;

            if (parent.SolidHueOverride >= 0)
            {
                hue = parent.SolidHueOverride;
            }

            int faceSerial = FaceInfo.FakeSerial(parent);

            m_Stream.Write((int)faceSerial);
            m_Stream.Write((short)parent.FaceItemID);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)Layer.Face);
            m_Stream.Write((int)parent.Serial);
            m_Stream.Write((short)hue);
        }
    }

    public sealed class RemoveHair : Packet
	{
		public RemoveHair(Mobile parent)
			: base(0x1D, 5)
		{
			m_Stream.Write(HairInfo.FakeSerial(parent));
		}
	}

	public sealed class RemoveFacialHair : Packet
	{
		public RemoveFacialHair(Mobile parent)
			: base(0x1D, 5)
		{
			m_Stream.Write(FacialHairInfo.FakeSerial(parent));
		}
	}

    public sealed class RemoveFace : Packet
    {
        public RemoveFace(Mobile parent)
            : base(0x1D, 5)
        {
            m_Stream.Write((int)FaceInfo.FakeSerial(parent));
        }
    }
}