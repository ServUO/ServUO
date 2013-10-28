#region Header
// **********
// ServUO - VirtueInfo.cs
// **********
#endregion

namespace Server
{
	[PropertyObject]
	public class VirtueInfo
	{
		private int[] m_Values;

		public int[] Values { get { return m_Values; } }

		public int GetValue(int index)
		{
			if (m_Values == null)
			{
				return 0;
			}
			else
			{
				return m_Values[index];
			}
		}

		public void SetValue(int index, int value)
		{
			if (m_Values == null)
			{
				m_Values = new int[8];
			}

			m_Values[index] = value;
		}

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Humility { get { return GetValue(0); } set { SetValue(0, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Sacrifice { get { return GetValue(1); } set { SetValue(1, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Compassion { get { return GetValue(2); } set { SetValue(2, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Spirituality { get { return GetValue(3); } set { SetValue(3, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Valor { get { return GetValue(4); } set { SetValue(4, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Honor { get { return GetValue(5); } set { SetValue(5, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Justice { get { return GetValue(6); } set { SetValue(6, value); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Honesty { get { return GetValue(7); } set { SetValue(7, value); } }

		public VirtueInfo()
		{ }

		public VirtueInfo(GenericReader reader)
		{
			int version = reader.ReadByte();

			switch (version)
			{
				case 1: //Changed the values throughout the virtue system
				case 0:
					{
						int mask = reader.ReadByte();

						if (mask != 0)
						{
							m_Values = new int[8];

							for (int i = 0; i < 8; ++i)
							{
								if ((mask & (1 << i)) != 0)
								{
									m_Values[i] = reader.ReadInt();
								}
							}
						}

						break;
					}
			}

			if (version == 0)
			{
				Compassion *= 200;
				Sacrifice *= 250; //Even though 40 (the max) only gives 10k, It's because it was formerly too easy

				//No direct conversion factor for Justice, this is just an approximation
				Justice *= 500;

				//All the other virtues haven't been defined at 'version 0' point in time in the scripts.
			}
		}

		public static void Serialize(GenericWriter writer, VirtueInfo info)
		{
			writer.Write((byte)1); // version

			if (info.m_Values == null)
			{
				writer.Write((byte)0);
			}
			else
			{
				int mask = 0;

				for (int i = 0; i < 8; ++i)
				{
					if (info.m_Values[i] != 0)
					{
						mask |= 1 << i;
					}
				}

				writer.Write((byte)mask);

				for (int i = 0; i < 8; ++i)
				{
					if (info.m_Values[i] != 0)
					{
						writer.Write(info.m_Values[i]);
					}
				}
			}
		}
	}
}