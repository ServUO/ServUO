using System;
using System.Linq;

using Server.Items;

namespace Server.Misc
{
	public partial class RaceDefinitions
	{
		public static Type[] AllRaceTypes { get; set; } =
		{
			typeof(BootsOfBallast), typeof(DetectiveCredentials)
		};

		public static int[] AllRaceIDs { get; set; } =
		{
			0xA289, 0xA28A, 0xA28B, 0xA291, 0xA292, 0xA293, // Whips
            0x0E85, 0x0E86,                                 // Tools
            0x1F03, 0x1F04, 0x26AE,                         // Robe & Arcane Robe
            0x0E81,                                         // Crook
            0x1086, 0x108A, 0x1F06, 0x1F09                  // Rings/Bracelet
        };

		public static void Configure()
		{
			/* Here we configure all races. Some notes:
            * 
            * 1) The first 32 races are reserved for core use.
            * 2) Race 0x7F is reserved for core use.
            * 3) Race 0xFF is reserved for core use.
            * 4) Changing or removing any predefined races may cause server instability.
            */
			RegisterRace(new Human(0, 0));
			RegisterRace(new Elf(1, 1));
			RegisterRace(new Gargoyle(2, 2));
		}

		public static void RegisterRace(Race race)
		{
			Race.Races[race.RaceIndex] = race;
			Race.AllRaces.Add(race);
		}

		public static Race GetRequiredRace(Item item)
		{
			if (item is IRacialEquipment re && re.RequiredRace != null)
				return re.RequiredRace;

			foreach (var race in Race.AllRaces)
			{
				if (race.IsExclusiveEquipment(item))
					return race;
			}

			return null;
		}

		public static void AddRaceProperty(Item item, ObjectPropertyList list)
		{
			var race = GetRequiredRace(item);

			if (race == Race.Elf)
			{
				list.Add(1075086); // Elves Only
			}
			else if (race == Race.Gargoyle)
			{
				list.Add(1111709); // Gargoyles Only
			}
		}

		public static bool ValidateEquipment(Mobile from, Item equipment)
		{
			return ValidateEquipment(from, equipment, true);
		}

		public static bool ValidateEquipment(Mobile from, Item equipment, bool message)
		{
			if (from == null || from.Race == null)
				return false;

			var required = GetRequiredRace(equipment);

			if (required != null && from.Race != required)
			{
				if (required == Race.Elf && from.FindItemOnLayer(Layer.Earrings) is MorphEarrings)
					return true;

				if (message)
					from?.SendMessage("Only {0} may use this.", required.PluralName);

				return false;
			}

			if (Array.IndexOf(AllRaceTypes, equipment.GetType()) != -1)
				return true;

			if (Array.IndexOf(AllRaceIDs, equipment.ItemID) != -1)
				return true;

			return from.Race.ValidateEquipment(from, equipment, message);
		}
	}
}