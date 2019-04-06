using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.IO;
using System.Reflection;

namespace Server.Commands
{
	public static class GenerateGameDocs
	{
		private static CsvFile csv;

		private delegate void ProcessObject(object obj);

		public static void Initialize()
		{
			CommandSystem.Register("GenGameDocs", AccessLevel.GameMaster, new CommandEventHandler(GenGameDocs_OnCommand));
		}

		private static void GenGameDocs_OnCommand(CommandEventArgs e)
		{
			csv = new CsvFile();
			AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(t => t.GetTypes())
				.Where(t => t.IsClass && t.Namespace == "Server.Mobiles" && typeof(Mobiles.BaseCreature).IsAssignableFrom(t))
				.ToList()
				.ForEach(t => ConsumeType(t, HandleBaseCreature));
			csv.Write("Creatures.csv");

			csv = new CsvFile();
			AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(t => t.GetTypes())
				.Where(t => t.IsClass && t.Namespace == "Server.Items" && typeof(Items.BaseWeapon).IsAssignableFrom(t))
				.ToList()
				.ForEach(t => ConsumeType(t, HandleBaseWeapon));
			csv.Write("Weapons.csv");

			csv = new CsvFile();
			AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(t => t.GetTypes())
				.Where(t => t.IsClass && t.Namespace == "Server.Items" && typeof(Items.BaseArmor).IsAssignableFrom(t))
				.ToList()
				.ForEach(t => ConsumeType(t, HandleBaseArmor));
			csv.Write("Armor.csv");
		}

		private static void HandleBaseArmor(object obj)
		{
			Items.BaseArmor arm = obj as Items.BaseArmor;
			if (arm == null)
				return;

			csv.AddRow();
			csv.AddValue("Type", arm.GetType().Name);
			csv.AddValue("Name", arm.Name);
			csv.AddValue("Is Artifact?", arm.IsArtifact);
			csv.AddValue("Pysical Resist", arm.PhysicalResistance);
			csv.AddValue("Fire Resist", arm.FireResistance);
			csv.AddValue("Cold Resist", arm.ColdResistance);
			csv.AddValue("Poison Resist", arm.PoisonResistance);
			csv.AddValue("Energy Resist", arm.EnergyResistance);
			csv.AddValue("DCI", arm.Attributes.DefendChance);

		}

		private static void HandleBaseWeapon(object obj)
		{
			Items.BaseWeapon wep = obj as Items.BaseWeapon;
			if (wep == null)
				return;

			csv.AddRow();
			csv.AddValue("Type", wep.GetType().Name);
			csv.AddValue("Name", wep.Name);
			csv.AddValue("Is Artifact?", wep.IsArtifact);
			csv.AddValue("Str Requirement", wep.StrRequirement);
			csv.AddValue("Dex Requirement", wep.DexRequirement);
			csv.AddValue("Int Requirement", wep.IntRequirement);
			csv.AddValue("Skill", wep.Skill);
			csv.AddValue("Race", wep.RequiredRace);
			csv.AddValue("Speed", wep.MlSpeed);
			csv.AddValue("Min Damage", wep.MinDamage);
			csv.AddValue("Max Damage", wep.MaxDamage);
		}

		private static void HandleBaseCreature(object obj)
		{
			Mobiles.BaseCreature creature = obj as Mobiles.BaseCreature;
			if (creature == null)
				return;

			Server.Items.BambooFlute flute = new Items.BambooFlute();

			csv.AddRow();
			csv.AddValue("Type", creature.GetType().Name);
			csv.AddValue("Name", creature.Name);
			csv.AddValue("Str", creature.Str);
			csv.AddValue("Dex", creature.Dex);
			csv.AddValue("Int", creature.Int);
			csv.AddValue("Hits", creature.HitsMax);
			csv.AddValue("Stam", creature.StamMax);
			csv.AddValue("Mana", creature.ManaMax);
			csv.AddValue("Physical Resist", creature.PhysicalResistance);
			csv.AddValue("Fire Resist", creature.FireResistance);
			csv.AddValue("Cold Resist", creature.ColdResistance);
			csv.AddValue("Poison Resist", creature.PoisonResistance);
			csv.AddValue("Energy Resist", creature.EnergyResistance);
			csv.AddValue("Physical Damage", creature.PhysicalDamage);
			csv.AddValue("Fire Damage", creature.FireDamage);
			csv.AddValue("Cold Damage", creature.ColdDamage);
			csv.AddValue("Poison Damage", creature.PoisonDamage);
			csv.AddValue("Energy Damage", creature.EnergyDamage);
			csv.AddValue("Taming Difficulty", creature.CurrentTameSkill);
			csv.AddValue("Barding Difficulty", flute.GetDifficultyFor(creature));
			csv.AddValue("TMap Level", creature.TreasureMapLevel);
			csv.AddValue("Wrestling Skill", creature.Skills.Wrestling.Base);
		}

		private static void ConsumeType(Type t, ProcessObject proc)
		{
			ConstructorInfo ctor = t.GetConstructor(new Type[] { });
			if (ctor == null)
				return;

			object obj;
			try
			{
				obj = ctor.Invoke(new object[] { });
			}
			catch (Exception)
			{
				return;
			}

			if (obj == null)
				return;

			proc(obj);
		}

		private class CsvFile
		{
			private List<Dictionary<String, String>> rows = new List<Dictionary<string, string>>();
			private Dictionary<String, String> currentRow = null;
			private HashSet<String> headerSet = new HashSet<string>();
			private List<String> allHeaders = new List<string>();

			public CsvFile()
			{
			}

			public void AddRow()
			{
				currentRow = new Dictionary<String, String>();
				rows.Add(currentRow);
			}

			public void AddValue(String name, object value)
			{
				if (name == null)
					return;

				String v = "";
				if (value != null)
					v = value.ToString();

				currentRow.Add(name, v.ToString());
				if (headerSet.Contains(name))
					return;
				headerSet.Add(name);
				allHeaders.Add(name);
			}

			public void Write(String path)
			{
				StreamWriter outf = new StreamWriter(path);
				bool first;

				first = true;
				foreach(String header in allHeaders)
				{
					if (first)
					{
						outf.Write(String.Format("\"{0}\"", header));
						first = false;
					}
					else
					{
						outf.Write(String.Format(",\"{0}\"", header));
					}
				}
				outf.WriteLine("");

				foreach(Dictionary<String, String> row in rows)
				{
					first = true;
					foreach (String header in allHeaders)
					{
						String value = "";
						row.TryGetValue(header, out value);
						if (first)
						{
							outf.Write(String.Format("\"{0}\"", value));
							first = false;
						}
						else
						{
							outf.Write(String.Format(",\"{0}\"", value));
						}
					}
					outf.WriteLine("");
				}

				outf.Close();
			}
		}
	}
}
