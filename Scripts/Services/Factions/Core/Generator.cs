using System;
using System.Collections.Generic;
using Server.Commands;
using System.Linq;

namespace Server.Factions
{
    public class Generator
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenerateFactions", AccessLevel.Administrator, new CommandEventHandler(GenerateFactions_OnCommand));
			CommandSystem.Register("DeleteFactions", AccessLevel.Administrator, new CommandEventHandler(DeleteFactions_OnCommand));
		}

        public static void RemoveFactions()
        {
            // Removes Items, ie monoliths, stones, etc
            WeakEntityCollection.Delete("factions");

            List<Sigil> sigils = new List<Sigil>(Sigil.Sigils);

            foreach (Sigil sig in sigils)
            {
                if (!sig.Deleted)
                    sig.Delete();
            }

            List<Item> items = new List<Item>(World.Items.Values.Where(i => i is StrongholdRune || i is ShrineGem || i is EnchantedBandage || i is PowderOfPerseverance));

            foreach (var item in items)
            {
                item.Delete();
            }

            ColUtility.Free(items);
        }

		public static void DeleteFactions_OnCommand(CommandEventArgs e)
		{
            RemoveFactions();
		}

        public static void GenerateFactions_OnCommand(CommandEventArgs e)
        {
            if (Settings.Enabled)
            {
                new FactionPersistence();

                List<Faction> factions = Faction.Factions;

                foreach (Faction faction in factions)
                    Generate(faction);

                List<Town> towns = Town.Towns;

                foreach (Town town in towns)
                    Generate(town);

                e.Mobile.SendMessage("Factions generated!");
            }
            else
            {
                e.Mobile.SendMessage("You must enable factions first by disabling VvV before you can generate.");
            }
        }

        public static void Generate(Town town)
        {
            Map facet = Faction.Facet;

            TownDefinition def = town.Definition;

            if (!CheckExistance(def.Monolith, facet, typeof(TownMonolith)))
            {
                TownMonolith mono = new TownMonolith(town);
                mono.MoveToWorld(def.Monolith, facet);
                mono.Sigil = new Sigil(town);
				WeakEntityCollection.Add("factions", mono);
				WeakEntityCollection.Add("factions", mono.Sigil);
			}

			if (!CheckExistance(def.TownStone, facet, typeof(TownStone)))
			{
				TownStone stone = new TownStone(town);
				WeakEntityCollection.Add("factions", stone);
				stone.MoveToWorld(def.TownStone, facet);
			}
        }

        public static void Generate(Faction faction)
        {
            Map facet = Faction.Facet;

            List<Town> towns = Town.Towns;

            StrongholdDefinition stronghold = faction.Definition.Stronghold;

			if (!CheckExistance(stronghold.JoinStone, facet, typeof(JoinStone)))
			{
				JoinStone join = new JoinStone(faction);
				WeakEntityCollection.Add("factions", join);
				join.MoveToWorld(stronghold.JoinStone, facet);
			}

			if (!CheckExistance(stronghold.FactionStone, facet, typeof(FactionStone)))
			{
				FactionStone stone = new FactionStone(faction);
				WeakEntityCollection.Add("factions", stone);
				stone.MoveToWorld(stronghold.FactionStone, facet);
			}

            for (int i = 0; i < stronghold.Monoliths.Length; ++i)
            {
                Point3D monolith = stronghold.Monoliths[i];

				if (!CheckExistance(monolith, facet, typeof(StrongholdMonolith)))
				{
					StrongholdMonolith mono = new StrongholdMonolith(towns[i], faction);
					WeakEntityCollection.Add("factions", mono);
					mono.MoveToWorld(monolith, facet);
				}
            }

            if (Core.ML && !CheckExistance(stronghold.FactionStone, facet, typeof(FactionCollectionBox)))
            {
                FactionCollectionBox box = new FactionCollectionBox(faction);
                WeakEntityCollection.Add("factions", box);
                box.MoveToWorld(stronghold.CollectionBox, facet);
            }
        }

        private static bool CheckExistance(Point3D loc, Map facet, Type type)
        {
            foreach (Item item in facet.GetItemsInRange(loc, 0))
            {
                if (type.IsAssignableFrom(item.GetType()))
                    return true;
            }

            return false;
        }
    }
}