/*
 * Organize Me by Tresdni
 * www.uofreedom.com
 * Instantly organize your backpack with a simple command.
 */

#region References

using System.Collections.Generic;
using System.Linq;
using Server.Factions;
using Server.Items;

#endregion

namespace Server.Commands
{
    public class OrganizeMeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("OrganizeMe", AccessLevel.Player, OrganizeMe_OnCommand);
        }

        //This command will not move spellbooks, runebooks, blessed, or insured items.
        [Usage("OrganizeMe")]
        [Description("Organize the items in your backpack into pouches.")]
        private static void OrganizeMe_OnCommand(CommandEventArgs arg)
        {
            Mobile from = arg.Mobile;
            Backpack bp = from.Backpack as Backpack;

            if (@from == null || bp == null)
            {
                return;
            }

            var backpackitems = new List<Item>(bp.Items);
            var subcontaineritems = new List<Item>();
            var oldpouches = new List<OrganizePouch>();

            foreach (BaseContainer item in backpackitems.OfType<BaseContainer>())
            {
                subcontaineritems.AddRange(item.Items);
            }

            backpackitems.AddRange(subcontaineritems);

            OrganizePouch weaponpouch = new OrganizePouch {Name = "Weapons", Hue = Utility.RandomMetalHue()};
            OrganizePouch armorpouch = new OrganizePouch {Name = "Armor", Hue = Utility.RandomMetalHue()};
            OrganizePouch clothingpouch = new OrganizePouch {Name = "Clothing", Hue = Utility.RandomBrightHue()};
            OrganizePouch jewelpouch = new OrganizePouch {Name = "Jewelry", Hue = Utility.RandomPinkHue()};
            OrganizePouch potionpouch = new OrganizePouch {Name = "Potions", Hue = Utility.RandomOrangeHue()};
            OrganizePouch currencypouch = new OrganizePouch {Name = "Currency", Hue = Utility.RandomYellowHue()};
            OrganizePouch resourcepouch = new OrganizePouch {Name = "Resources", Hue = Utility.RandomNondyedHue()};
            OrganizePouch toolpouch = new OrganizePouch { Name = "Tools", Hue = Utility.RandomMetalHue() };
            OrganizePouch regspouch = new OrganizePouch {Name = "Reagents", Hue = Utility.RandomGreenHue()};
            OrganizePouch miscpouch = new OrganizePouch {Name = "Misc"};

            var pouches = new List<OrganizePouch>
            {
                weaponpouch,
                armorpouch,
                clothingpouch,
                jewelpouch,
                potionpouch,
                currencypouch,
                resourcepouch,
                toolpouch,
                regspouch,
                miscpouch
            };

            foreach (
                Item item in
                    backpackitems.Where(
                        item =>
                            item.LootType != LootType.Blessed && !item.Insured && !(item is Runebook) &&
                            !(item is Spellbook) && item.Movable))
            {
                if (item is BaseWeapon)
                {
                    weaponpouch.DropItem(item);
                }
                else if (item is BaseArmor)
                {
                    armorpouch.DropItem(item);
                }
                else if (item is BaseClothing)
                {
                    clothingpouch.DropItem(item);
                }
                else if (item is BaseJewel)
                {
                    jewelpouch.DropItem(item);
                }
                else if (item is BasePotion)
                {
                    potionpouch.DropItem(item);
                }
                else if (item is Gold || item is Silver)
                {
                    currencypouch.DropItem(item);
                }
                else if (item is BaseIngot || item is BaseOre || item is Feather || item is BaseBoard || item is Log || item is BaseLeather ||
                         item is Sand || item is BaseGranite)
                {
                    resourcepouch.DropItem(item);
                }
                else if (item is BaseTool)
                {
                    toolpouch.DropItem(item);
                }
                else if (item is BaseReagent)
                {
                    regspouch.DropItem(item);
                }
                else if (item is OrganizePouch)
                {
                    oldpouches.Add((OrganizePouch) item);
                }
                else
                {
                    miscpouch.DropItem(item);
                }
            }

            int x = 45;

            foreach (OrganizePouch pouch in pouches)
            {
                from.AddToBackpack(pouch);
                pouch.X = x;
                pouch.Y = 65;

                x += 10;
            }

            var todelete =
                @from.Backpack.Items.OfType<OrganizePouch>()
                    .Select(item => item)
                    .Where(emptypouch => emptypouch.Items.Count <= 0)
                    .ToList();

            foreach (OrganizePouch oldpouch in oldpouches)
            {
                oldpouch.Delete();
            }

            foreach (OrganizePouch packtodelete in todelete)
            {
                packtodelete.Delete();
            }
        }
    }
}