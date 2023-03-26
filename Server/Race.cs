#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server
{
	public interface IRacialEquipment
	{
		Race RequiredRace { get; }
	}

	[Parsable]
	public abstract class Race
	{
		public static Race[] Races { get; } = new Race[0x100];

		public static Race DefaultRace => Races[0];

		public static Race Human => Races[0];
		public static Race Elf => Races[1];
		public static Race Gargoyle => Races[2];

		public static List<Race> AllRaces { get; } = new List<Race>();

		private static string[] m_RaceNames;
		private static Race[] m_RaceValues;

		public static string[] GetRaceNames()
		{
			CheckNamesAndValues();

			return m_RaceNames;
		}

		public static Race[] GetRaceValues()
		{
			CheckNamesAndValues();

			return m_RaceValues;
		}

		public static Race Parse(string value)
		{
			CheckNamesAndValues();

			for (var i = 0; i < m_RaceNames.Length; ++i)
			{
				if (Insensitive.Equals(m_RaceNames[i], value))
				{
					return m_RaceValues[i];
				}
			}

			if (Int32.TryParse(value, out var index))
			{
				if (index >= 0 && index < Races.Length && Races[index] != null)
				{
					return Races[index];
				}
			}

			throw new ArgumentException("Invalid race name");
		}

		private static void CheckNamesAndValues()
		{
			if (m_RaceNames != null && m_RaceNames.Length == AllRaces.Count)
			{
				return;
			}

			m_RaceNames = new string[AllRaces.Count];
			m_RaceValues = new Race[AllRaces.Count];

			for (var i = 0; i < AllRaces.Count; ++i)
			{
				var race = AllRaces[i];

				m_RaceNames[i] = race.Name;
				m_RaceValues[i] = race;
			}
		}

		public int RaceID { get; }
		public int RaceIndex { get; }

		public abstract string Name { get; set; }
		public abstract string PluralName { get; set; }

		public abstract int MaleBody { get; set; }
		public abstract int FemaleBody { get; set; }

		public abstract int MaleGhostBody { get; set; }
		public abstract int FemaleGhostBody { get; set; }

		public abstract Expansion RequiredExpansion { get; set; }

		public abstract int[] SkinHues { get; set; }
		public abstract int[] HairHues { get; set; }

		public abstract int[][] HairTable { get; set; }
		public abstract int[][] BeardTable { get; set; }
		public abstract int[][] FaceTable { get; set; }

		public abstract int[] ExclusiveEquipment { get; set; }

		protected Race(int raceID, int raceIndex)
		{
			RaceID = raceID;
			RaceIndex = raceIndex;

			SortTables();
		}

		public override string ToString()
		{
			return Name;
		}

		public void SortTables()
		{
			Array.Sort(ExclusiveEquipment);

			Array.Sort(SkinHues);
			Array.Sort(HairHues);

			for (var i = 0; i <= 1; i++)
			{
				Array.Sort(HairTable[i]);
				Array.Sort(BeardTable[i]);
				Array.Sort(FaceTable[i]);
			}
		}

		protected int ClipRange(int[] table, int value)
		{
			if (!CheckRange(table, value, out var min, out var max))
				value = Math.Max(min, Math.Min(max, value));

			return value;
		}

		protected bool CheckRange(int[] table, int value)
		{
			return CheckRange(table, value, out _, out _);
		}

		protected bool CheckRange(int[] table, int value, out int min, out int max)
		{
			if (table == null || table.Length == 0)
			{
				min = max = 0;
				return value == 0;
			}

			min = table[0];
			max = table[table.Length - 1];

			if (table.Length > 1 && table[0] == 0)
				min = table[1];

			if (value == min || value == max)
				return true;

			if (value < min || value > max)
				return false;

			return Array.IndexOf(table, value) >= 0;
		}

		public bool IsExclusiveEquipment(Item equipment)
		{
			if (equipment is IRacialEquipment re && re.RequiredRace == this)
				return true;

			if (Array.IndexOf(ExclusiveEquipment, equipment.ItemID) != -1)
				return true;

			return false;
		}

		public virtual bool ValidateEquipment(Mobile from, Item equipment, bool message)
		{
			if (IsExclusiveEquipment(equipment))
				return true;

			foreach (var race in AllRaces)
			{
				if (race != null && race != this && Array.IndexOf(race.ExclusiveEquipment, equipment.ItemID) != -1)
				{
					if (message)
						from?.SendMessage("Only {0} may use this.", race.PluralName);

					return false;
				}
			}

			return true;
		}

		public virtual bool ValidateHair(Mobile m, int itemID)
		{
			return ValidateHair(m.Female, itemID);
		}

		public virtual bool ValidateHair(bool female, int itemID)
		{
			return CheckRange(HairTable[female ? 0 : 1], itemID);
		}

		public virtual int RandomHair(Mobile m)
		{
			return RandomHair(m.Female);
		}

		public virtual int RandomHair(bool female)
		{
			return Utility.RandomList(HairTable[female ? 0 : 1]);
		}

		public virtual bool ValidateFacialHair(Mobile m, int itemID)
		{
			return ValidateFacialHair(m.Female, itemID);
		}

		public virtual bool ValidateFacialHair(bool female, int itemID)
		{
			return CheckRange(BeardTable[female ? 0 : 1], itemID);
		}

		public virtual int RandomFacialHair(Mobile m)
		{
			return RandomFacialHair(m.Female);
		}

		public virtual int RandomFacialHair(bool female)
		{
			return Utility.RandomList(BeardTable[female ? 0 : 1]);
		}

		public virtual bool ValidateFace(Mobile m, int itemID)
		{
			return ValidateFace(m.Female, itemID);
		}

		public virtual bool ValidateFace(bool female, int itemID)
		{
			return CheckRange(FaceTable[female ? 0 : 1], itemID);
		}

		public virtual int RandomFace(Mobile m)
		{
			return RandomFace(m.Female);
		}

		public virtual int RandomFace(bool female)
		{
			return Utility.RandomList(FaceTable[female ? 0 : 1]);
		}

		public virtual int ClipSkinHue(int hue)
		{
			return ClipRange(SkinHues, hue);
		}

		public virtual int RandomSkinHue()
		{
			return Utility.RandomList(SkinHues);
		}

		public virtual int ClipHairHue(int hue)
		{
			return ClipRange(HairHues, hue);
		}

		public virtual int RandomHairHue()
		{
			return Utility.RandomList(HairHues);
		}

		public virtual int Body(Mobile m)
		{
			if (m.Alive)
			{
				return AliveBody(m.Female);
			}

			return GhostBody(m.Female);
		}

		public virtual int AliveBody(Mobile m)
		{
			return AliveBody(m.Female);
		}

		public virtual int AliveBody(bool female)
		{
			return female ? FemaleBody : MaleBody;
		}

		public virtual int GhostBody(Mobile m)
		{
			return GhostBody(m.Female);
		}

		public virtual int GhostBody(bool female)
		{
			return female ? FemaleGhostBody : MaleGhostBody;
		}
	}
}
