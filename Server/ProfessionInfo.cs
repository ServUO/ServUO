using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server
{
	public enum Profession
	{
		Advanced = 0,
		Warrior = 1,
		Mage = 2,
		Blacksmith = 3,
		Necromancer = 4,
		Paladin = 5,
		Samurai = 6,
		Ninja = 7
	}

	public class ProfessionInfo
	{
		public static ProfessionInfo[] Professions { get; private set; }

		static ProfessionInfo()
		{
			Load();
		}

		public static void Configure()
		{
			Core.OnExpansionChanged += Load;
		}

		public static void Load()
		{
			var profs = new List<ProfessionInfo>
			{
				new ProfessionInfo
				{
					ID = Profession.Advanced,
					Name = "Advanced",
					TopLevel = false,
					GumpID = 5571,
					Skills = new SkillNameValue[4],
					Stats = new StatNameValue[3]
				}
			};

			var skillsBuffer = new List<SkillNameValue>();
			var statsBuffer = new List<StatNameValue>();

			var file = Core.FindDataFile("Prof.txt");

			if (String.IsNullOrWhiteSpace(file) || !File.Exists(file))
			{
				file = Path.Combine(Core.BaseDirectory, "Data", "Prof.txt");
			}

			if (!String.IsNullOrWhiteSpace(file) && File.Exists(file))
			{
				using (var s = File.OpenText(file))
				{
					ProfessionInfo prof;
					SkillInfo info;
					int skills, stats, valid;

					string line;
					string[] cols;

					while (!s.EndOfStream)
					{
						line = s.ReadLine();

						if (String.IsNullOrWhiteSpace(line))
						{
							continue;
						}

						line = line.Trim();

						if (!Insensitive.StartsWith(line, "Begin"))
						{
							continue;
						}

						prof = new ProfessionInfo();

						skills = stats = valid = 0;

						while (!s.EndOfStream)
						{
							line = s.ReadLine();

							if (String.IsNullOrWhiteSpace(line))
							{
								continue;
							}

							line = line.Trim();

							if (Insensitive.StartsWith(line, "End"))
							{
								if (valid >= 4)
								{
									profs.Add(prof);
								}

								break;
							}

							cols = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

							for (var i = 0; i < cols.Length; i++)
							{
								cols[i] = cols[i].Trim();
							}

							switch (cols[0].ToLower())
							{
								case "truename":
								{
									prof.Name = cols[1].Trim('"');

									++valid;
								}
								break;
								case "nameid":
									prof.NameID = Utility.ToInt32(cols[1]);
									break;
								case "descid":
									prof.DescID = Utility.ToInt32(cols[1]);
									break;
								case "desc":
								{
									if (Enum.TryParse(cols[1], out Profession id))
									{
										prof.ID = id;

										++valid;
									}
								}
								break;
								case "toplevel":
									prof.TopLevel = Utility.ToBoolean(cols[1]);
									break;
								case "gump":
									prof.GumpID = Utility.ToInt32(cols[1]);
									break;
								case "skill":
								{
									if (!Enum.TryParse(cols[1].Replace(" ", String.Empty), out SkillName skill))
									{
										if (Insensitive.Equals(cols[1], "Evaluate Intelligence"))
										{
											info = SkillInfo.Table[(int)SkillName.EvalInt];
										}
										else
										{
											info = SkillInfo.Table.FirstOrDefault(o => Insensitive.Contains(o.Name, cols[1]) || Insensitive.Contains(cols[1], o.Name));
										}

										if (info == null)
										{
											break;
										}

										skill = (SkillName)info.SkillID;
									}

									skillsBuffer.Add(new SkillNameValue(skill, Utility.ToInt32(cols[2])));

									if (++skills == 1)
									{
										++valid;
									}
								}
								break;
								case "stat":
								{
									if (!Enum.TryParse(cols[1], out StatType stat))
									{
										break;
									}

									statsBuffer.Add(new StatNameValue(stat, Utility.ToInt32(cols[2])));

									if (++stats == 1)
									{
										++valid;
									}
								}
								break;
							}
						}

						prof.Skills = skillsBuffer.ToArray();
						prof.Stats = statsBuffer.ToArray();

						skillsBuffer.Clear();
						statsBuffer.Clear();
					}
				}
			}

			Professions = new ProfessionInfo[1 + profs.Max(p => (int)p.ID)];

			foreach (var p in profs)
			{
				Professions[(int)p.ID] = p;
			}

			for (var i = 0; i < Professions.Length; i++)
			{
				if (Professions[i] == null)
				{
					Professions[i] = new ProfessionInfo
					{
						ID = (Profession)i,
						Name = $"Undefined-{i}",
						TopLevel = true
					};
				}
			}

			profs.Clear();
			profs.TrimExcess();
		}

		private ProfessionInfo()
		{
			Name = String.Empty;

			Skills = Array.Empty<SkillNameValue>();
			Stats = Array.Empty<StatNameValue>();
		}

		public Profession ID { get; private set; }

		public string Name { get; private set; }

		public int NameID { get; private set; }
		public int DescID { get; private set; }

		public bool TopLevel { get; private set; }

		public int GumpID { get; private set; }

		public SkillNameValue[] Skills { get; private set; }
		public StatNameValue[] Stats { get; private set; }
	}
}
