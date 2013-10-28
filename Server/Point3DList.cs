#region Header
// **********
// ServUO - Point3DList.cs
// **********
#endregion

namespace Server
{
	public class Point3DList
	{
		private Point3D[] m_List;
		private int m_Count;

		public Point3DList()
		{
			m_List = new Point3D[8];
			m_Count = 0;
		}

		public int Count { get { return m_Count; } }

		public void Clear()
		{
			m_Count = 0;
		}

		public Point3D Last { get { return m_List[m_Count - 1]; } }

		public Point3D this[int index] { get { return m_List[index]; } }

		public void Add(int x, int y, int z)
		{
			if ((m_Count + 1) > m_List.Length)
			{
				var old = m_List;
				m_List = new Point3D[old.Length * 2];

				for (int i = 0; i < old.Length; ++i)
				{
					m_List[i] = old[i];
				}
			}

			m_List[m_Count].m_X = x;
			m_List[m_Count].m_Y = y;
			m_List[m_Count].m_Z = z;
			++m_Count;
		}

		public void Add(Point3D p)
		{
			if ((m_Count + 1) > m_List.Length)
			{
				var old = m_List;
				m_List = new Point3D[old.Length * 2];

				for (int i = 0; i < old.Length; ++i)
				{
					m_List[i] = old[i];
				}
			}

			m_List[m_Count].m_X = p.m_X;
			m_List[m_Count].m_Y = p.m_Y;
			m_List[m_Count].m_Z = p.m_Z;
			++m_Count;
		}

		private static readonly Point3D[] m_EmptyList = new Point3D[0];

		public Point3D[] ToArray()
		{
			if (m_Count == 0)
			{
				return m_EmptyList;
			}

			var list = new Point3D[m_Count];

			for (int i = 0; i < m_Count; ++i)
			{
				list[i] = m_List[i];
			}

			m_Count = 0;

			return list;
		}
	}
}