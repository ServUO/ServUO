// Global Donation Box v3.0.0
// Author: Felladrin
// Started: 2010-03-07
// Updated: 2016-01-19

using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Felladrin.Items
{
    public static class GlobalDonationBox
    {
        public static void Initialize()
        {
            CommandSystem.Register("Donate", AccessLevel.Player, new CommandEventHandler(OnCommandDonate));
            CommandSystem.Register("DonationBox", AccessLevel.Administrator, new CommandEventHandler(OnCommandDonationBox));
        }

        [Usage("Donate")]
        [Description("Used to donate items from your backpack.")]
        static void OnCommandDonate(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            pm.Target = new DonateTarget();
            pm.SendMessage(1154, "Select the items you want to donate. Press ESC when you finish.");
        }

        [Usage("DonationBox")]
        [Description("Places the Donation Box at your current location.")]
        static void OnCommandDonationBox(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            Instance.MoveToWorld(pm.Location, pm.Map);
            Instance.Open(pm);
        }

        static DonationBox instance;

        public static DonationBox Instance
        {
            get
            {
                if (instance == null)
                {
                    bool exist = false;

                    foreach (Item item in World.Items.Values)
                    {
                        var donationBox = item as DonationBox;

                        if (donationBox != null)
                        {
                            instance = donationBox;
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        instance = new DonationBox();
                    }
                }

                return instance;
            }
        }

        static void Congratulate(Mobile m)
        {
            m.Emote("*donates an item*");
            m.PlaySound(m.Female ? 823 : 1097);
            m.PlaySound(1051);
            m.SendMessage(67, "Thanks for your donation!");
        }
            
        static void DeleteEmptyDonationBags()
        {
            foreach (DonationBag bag in Instance.FindItemsByType<DonationBag>(false))
            {
                if (bag.TotalItems == 0)
                    bag.Delete();
            }
        }

        static void CheckAllDonationBagsPositions()
        {
            foreach (DonationBag bag in Instance.FindItemsByType<DonationBag>(false))
                CheckDonationBagPosition(bag);
        }

        static void CheckDonationBagPosition(DonationBag donationBag)
        {
            var badPosition = true;

            for(int i = 0; i < 10 && badPosition; i++)
            {
                badPosition = donationBag.X < 15 || donationBag.X > 125 || donationBag.Y < 50 || donationBag.Y > 80;

                if (!badPosition)
                {
                    foreach (DonationBag bag in Instance.FindItemsByType<DonationBag>(false))
                    {
                        if (bag != donationBag && Math.Abs(donationBag.X - bag.X) < 10 && Math.Abs(donationBag.Y - bag.Y) < 10)
                        {
                            badPosition = true;
                            break;
                        }
                    }
                }

                if (badPosition)
                {
                    donationBag.X = Utility.RandomMinMax(15, 125);
                    donationBag.Y = Utility.RandomMinMax(50, 80);
                }
            }
        }

        static void Categorize(Item item)
        {
            DeleteEmptyDonationBags();

            if (item is DonationBag)
                return;

            var container = item as Container;

            if (container != null && container.Items.Count > 0)
            {
                foreach (Item subItem in new List<Item>(container.Items))
                    Categorize(subItem);
            }

            if (item.Stackable)
            {
                var similarItems = Instance.FindItemsByType(item.GetType());

                if (similarItems.Length > 1)
                {
                    foreach (var similarItem in similarItems)
                    {
                        if (similarItem != item)
                        {
                            item.Amount += similarItem.Amount;
                            similarItem.Delete();
                        }
                    }
                }
            }

            string name;

            if (item is BaseRanged)
                name = "Ranged Weapons";
            else if (item is BaseWand)
                name = "Wands";
            else if (item is BaseAxe)
                name = "Axes";
            else if (item is BaseKnife)
                name = "Knives";
            else if (item is BaseBashing)
                name = "Maces";
            else if (item is BasePoleArm)
                name = "PoleArms";
            else if (item is BaseSpear)
                name = "Spears and Forks";
            else if (item is BaseStaff)
                name = "Staves";
            else if (item is BaseSword)
                name = "Swords";
            else if (item is BaseWeapon || item is BaseMeleeWeapon)
                name = "Other Weapons";
            else if (item is BaseHat)
                name = "Hats";
            else if (item is BaseShield)
                name = "Shields";
            else if (item is BaseArmor)
                name = "Armors";
            else if (item is Gold || item is BankCheck)
                name = "Gold";
            else if (item is BaseClothing || item is Cloth)
                name = "Clothes";
            else if (item is BaseTool || item is Lockpick)
                name = "Tools";
            else if (item is Food || item is BaseBeverage)
                name = "Food";
            else if (item is Bolt || item is Arrow)
                name = "Ammunition";
            else if (item is BaseReagent)
                name = "Reagents";
            else if (item is BaseJewel)
                name = "Jewels";
            else if (item is SpellScroll || item is Spellbook)
                name = "Spells";
            else if (item is BaseOre || item is BaseScales || item is BaseIngot)
                name = "Blacksmithing Resources";
            else if (item is BaseHides || item is BaseLeather)
                name = "Tailor Resources";
            else if (item is BaseAddonDeed || item is BaseAddon)
                name = "Addons";
            else if (item is BaseBook)
                name = "Books";
            else if (item is BaseLight)
                name = "Lights";
            else if (item is BaseContainer)
                name = "Containers";
            else if (item is BasePotion)
                name = "Potions";
            else if (item is BaseInstrument)
                name = "Instruments";
            else if (item is BaseTalisman)
                name = "Talismans";
            else
                name = "Miscellaneous";

            var exist = false;

            var existingBags = Instance.FindItemsByType<DonationBag>(false);

            foreach (DonationBag bag in existingBags)
            {
                if (bag.Name == name)
                {
                    bag.DropItem(item);
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                var bag = new DonationBag();
                bag.Name = name;
                bag.DropItem(item);
                Instance.DropItem(bag);
                CheckAllDonationBagsPositions();
            }
        }

        class DonateTarget : Target
        {
            public DonateTarget() : base(-1, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                var item = targeted as Item;

                if (item != null)
                {
                    if (item.IsChildOf(from.Backpack) && item.Movable)
                    {
                        Instance.DropItem(item);
                        Congratulate(from);
                    }
                    else
                    {
                        from.SendMessage(67, "You can only donate items from your backpack.");
                    }
                }
                else
                {
                    from.SendMessage(33, "You can only donate items!");
                }

                from.Target = new DonateTarget();
            }
        }

        [Flipable(0x2811, 0x2812)]
        public class DonationBox : BaseContainer
        {
            public DonationBox() : base(0x2811)
            {
                Name = "The Donation Box";
                Movable = false;
                LiftOverride = true;
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                list.Add(Name);
                list.Add("Donations: {0}", TotalItems);
            }

            public override bool OnDragDrop(Mobile from, Item dropped)
            {
                if (base.OnDragDrop(from, dropped))
                {
                    Congratulate(from);
                    return true;
                }

                return false;
            }

            public override void OnItemAdded(Item item)
            {
                Categorize(item);
            }

            public override void OnSubItemAdded(Item item)
            {
                Categorize(item);
            }

            public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
            {
                var donationBag = item as DonationBag;

                if (donationBag != null)
                {
                    CheckDonationBagPosition(donationBag);
                    return false;
                }

                return base.CheckLift(from, item, ref reject);
            }

            public override int DefaultMaxWeight { get { return 0; } }

            public override int DefaultMaxItems { get { return 0; } }

            public override int DefaultGumpID { get { return 0x43; } }

            public override int DefaultDropSound { get { return 0x42; } }

            public override bool DisplayWeight { get { return false; } }

            public override bool Decays { get { return false; } }

            public DonationBox(Serial serial) : base(serial) { }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                reader.ReadInt();
            }
        }

        public class DonationBag : Bag
        {
            public DonationBag() { }

            public override void GetProperties(ObjectPropertyList list)
            {
                list.Add(Name);
                list.Add("Containing {0} Item{1}", TotalItems, (TotalItems > 1 ? "s" : ""));
            }

            public override int DefaultMaxWeight { get { return 0; } }

            public override int DefaultMaxItems { get { return 0; } }

            public override bool DisplayWeight { get { return false; } }

            public override bool Decays { get { return false; } }

            public DonationBag(Serial serial) : base(serial) { }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                reader.ReadInt();
            }
        }
    }
}
