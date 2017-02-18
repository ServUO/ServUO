#region References
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

namespace Ultima
{
	public sealed class Skills
	{
	    private Files _files;
	    /// <summary>
	    /// Inizializza una nuova istanza della classe <see cref="T:System.Object"/>.
	    /// </summary>
	    public Skills(Verdata verdata, Files files)
	    {
	        _files = files;
	        m_FileIndex = new FileIndex("skills.idx", "skills.mul", 16, verdata, _files);
            m_SkillEntries = new List<SkillInfo>();
            for (int i = 0; i < m_FileIndex.Index.Length; ++i)
            {
                SkillInfo info = GetSkill(i);
                if (info == null)
                {
                    break;
                }
                m_SkillEntries.Add(info);
            }
        }

	    private  FileIndex m_FileIndex;

		private  List<SkillInfo> m_SkillEntries;

		public  List<SkillInfo> SkillEntries
		{
			get
			{
				if (m_SkillEntries == null)
				{
					m_SkillEntries = new List<SkillInfo>();
					for (int i = 0; i < m_FileIndex.Index.Length; ++i)
					{
						SkillInfo info = GetSkill(i);
						if (info == null)
						{
							break;
						}
						m_SkillEntries.Add(info);
					}
				}
				return m_SkillEntries;
			}
			set { m_SkillEntries = value; }
		}

		/// <summary>
		///     ReReads skills.mul
		/// </summary>
		public  void Reload()
		{
			
		}

		/// <summary>
		///     Returns <see cref="SkillInfo" /> of index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public  SkillInfo GetSkill(int index)
		{
			int length, extra;
			bool patched;

			Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);
			if (stream == null)
			{
				return null;
			}
			if (length == 0)
			{
				return null;
			}

			using (var bin = new BinaryReader(stream))
			{
				bool action = bin.ReadBoolean();
				string name = ReadNameString(bin, length - 1);
				return new SkillInfo(index, name, action, extra);
			}
		}

		private  readonly byte[] m_StringBuffer = new byte[1024];

		private  string ReadNameString(BinaryReader bin, int length)
		{
			bin.Read(m_StringBuffer, 0, length);
			int count;
			for (count = 0; count < length && m_StringBuffer[count] != 0; ++count)
			{
				;
			}

			return Encoding.Default.GetString(m_StringBuffer, 0, count);
		}

		public  void Save(string path)
		{
			string idx = Path.Combine(path, "skills.idx");
			string mul = Path.Combine(path, "skills.mul");
			using (
				FileStream fsidx = new FileStream(idx, FileMode.Create, FileAccess.Write, FileShare.Write),
						   fsmul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				using (BinaryWriter binidx = new BinaryWriter(fsidx), binmul = new BinaryWriter(fsmul))
				{
					for (int i = 0; i < m_FileIndex.Index.Length; ++i)
					{
						SkillInfo skill = (i < m_SkillEntries.Count) ? m_SkillEntries[i] : null;
						if (skill == null)
						{
							binidx.Write(-1); // lookup
							binidx.Write(0); // length
							binidx.Write(0); // extra
						}
						else
						{
							binidx.Write((int)fsmul.Position); //lookup
							var length = (int)fsmul.Position;
							binmul.Write(skill.IsAction);

							byte[] namebytes = Encoding.Default.GetBytes(skill.Name);
							binmul.Write(namebytes);
							binmul.Write((byte)0); //nullterminated

							length = (int)fsmul.Position - length;
							binidx.Write(length);
							binidx.Write(skill.Extra);
						}
					}
				}
			}
		}
	}

	public sealed class SkillInfo
	{
		private string m_Name;

		public int Index { get; set; }
		public bool IsAction { get; set; }

		public string Name
		{
			get { return m_Name; }
			set
			{
				if (value == null)
				{
					m_Name = "";
				}
				else
				{
					m_Name = value;
				}
			}
		}

		public int Extra { get; private set; }

		public SkillInfo(int nr, string name, bool action, int extra)
		{
			Index = nr;
			m_Name = name;
			IsAction = action;
			Extra = extra;
		}
	}
}