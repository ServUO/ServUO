using Server.Engines.VeteranRewards;
using Server.Items;
using Server.SkillHandlers;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Engines.Craft
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AlterableAttribute : Attribute
    {
        public Type CraftSystem { get; private set; }
        public Type AlteredType { get; private set; }
        public bool Inherit { get; private set; }

        public AlterableAttribute(Type craftSystem, Type alteredType, bool inherit = false)
        {
            CraftSystem = craftSystem;
            AlteredType = alteredType;
            Inherit = inherit;
        }

        /// <summary>
        /// this enables any craftable item where their parent class can be altered, can be altered too.
        /// This is mainly for the ML craftable artifacts.
        /// </summary>
        /// <returns></returns>
        public bool CheckInherit(Type original)
        {
            if (Inherit)
                return true;

            CraftSystem system = CraftContext.Systems.FirstOrDefault(sys => sys.GetType() == CraftSystem);

            if (system != null)
            {
                return system.CraftItems.SearchFor(original) != null;
            }

            return false;
        }
    }

    public class AlterItem
    {
        public static void BeginTarget(Mobile from, CraftSystem system, ITool tool)
        {
            from.Target = new AlterItemTarget(system, tool);
            from.SendLocalizedMessage(1094730); //Target the item to altar
        }

        public static void BeginTarget(Mobile from, CraftSystem system, Item contract)
        {
            from.Target = new AlterItemTarget(system, contract);
            from.SendLocalizedMessage(1094730); //Target the item to altar
        }
    }

    public class AlterItemTarget : Target
    {
        private readonly CraftSystem m_System;
        private readonly ITool m_Tool;
        private readonly Item m_Contract;

        public AlterItemTarget(CraftSystem system, Item contract)
                : base(2, false, TargetFlags.None)
        {
            m_System = system;
            m_Contract = contract;
        }

        public AlterItemTarget(CraftSystem system, ITool tool)
            : base(1, false, TargetFlags.None)
        {
            m_System = system;
            m_Tool = tool;
        }

        private static AlterableAttribute GetAlterableAttribute(object o, bool inherit)
        {
            Type t = o.GetType();

            object[] attrs = t.GetCustomAttributes(typeof(AlterableAttribute), inherit);

            if (attrs != null && attrs.Length > 0)
            {
                AlterableAttribute attr = attrs[0] as AlterableAttribute;

                if (attr != null && (!inherit || attr.CheckInherit(t)))
                    return attr;
            }

            return null;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            int number = -1;

            Item origItem = o as Item;
            SkillName skill = m_System.MainSkill;
            double value = from.Skills[skill].Value;

            AlterableAttribute alterInfo = GetAlterableAttribute(o, false);

            if (alterInfo == null)
            {
                alterInfo = GetAlterableAttribute(o, true);
            }

            if (origItem == null || !origItem.IsChildOf(from.Backpack))
            {
                number = 1094729; // The item must be in your backpack for you to alter it.
            }
            else if (origItem is BlankScroll)
            {
                if (m_Contract == null)
                {
                    if (value >= 100.0)
                    {
                        Item contract = null;

                        if (skill == SkillName.Blacksmith)
                            contract = new AlterContract(RepairSkillType.Smithing, from);
                        else if (skill == SkillName.Carpentry)
                            contract = new AlterContract(RepairSkillType.Carpentry, from);
                        else if (skill == SkillName.Tailoring)
                            contract = new AlterContract(RepairSkillType.Tailoring, from);
                        else if (skill == SkillName.Tinkering)
                            contract = new AlterContract(RepairSkillType.Tinkering, from);

                        if (contract != null)
                        {
                            from.AddToBackpack(contract);

                            number = 1044154; // You create the item.

                            // Consume a blank scroll
                            origItem.Consume();
                        }
                    }
                    else
                    {
                        number = 1111869; // You must be at least grandmaster level to create an alter service contract.
                    }
                }
                else
                {
                    number = 1094728; // You may not alter that item.
                }
            }
            else if (alterInfo == null)
            {
                number = 1094728; // You may not alter that item.
            }
            else if (!IsAlterable(origItem))
            {
                number = 1094728; // You may not alter that item.
            }
            else if (alterInfo.CraftSystem != m_System.GetType())
            {
                if (m_Tool != null)
                {
                    // You may not alter that item.
                    number = 1094728;
                }
                else
                {
                    // You cannot alter that item with this type of alter contract.
                    number = 1094793;
                }
            }
            else if (!Imbuing.CheckSoulForge(from, 2, false, false))
            {
                number = 1111867; // You must be near a soulforge to alter an item.
            }
            else if (m_Contract == null && value < 100.0)
            {
                number = 1111870; // You must be at least grandmaster level to alter an item.
            }
            else if (origItem is BaseWeapon && ((BaseWeapon)origItem).EnchantedWeilder != null)
            {
                number = 1111849; // You cannot alter an item that is currently enchanted.
            }
            else if (origItem.HasSocket<SlayerSocket>())
            {
                SlayerSocket socket = origItem.GetSocket<SlayerSocket>();

                if (socket.Slayer == SlayerName.Silver)
                {
                    number = 1155681; // You cannot alter an item that has been treated with Tincture of Silver.
                }
                else
                {
                    number = 1111849; // You cannot alter an item that is currently enchanted.
                }
            }
            else
            {
                Item newitem = Activator.CreateInstance(alterInfo.AlteredType) as Item;

                if (newitem == null)
                    return;

                if (origItem is BaseWeapon && newitem is BaseWeapon)
                {
                    BaseWeapon oldweapon = (BaseWeapon)origItem;
                    BaseWeapon newweapon = (BaseWeapon)newitem;

                    newweapon.Slayer = oldweapon.Slayer;
                    newweapon.Slayer2 = oldweapon.Slayer2;
                    newweapon.Slayer3 = oldweapon.Slayer3;
                    newweapon.Resource = oldweapon.Resource;

                    if (oldweapon.PlayerConstructed)
                    {
                        newweapon.PlayerConstructed = true;
                        newweapon.Crafter = oldweapon.Crafter;
                        newweapon.Quality = oldweapon.Quality;
                    }

                    newweapon.Altered = true;
                }
                else if (origItem is BaseArmor && newitem is BaseArmor)
                {
                    BaseArmor oldarmor = (BaseArmor)origItem;
                    BaseArmor newarmor = (BaseArmor)newitem;

                    if (oldarmor.PlayerConstructed)
                    {
                        newarmor.PlayerConstructed = true;
                        newarmor.Crafter = oldarmor.Crafter;
                        newarmor.Quality = oldarmor.Quality;
                    }

                    newarmor.Resource = oldarmor.Resource;

                    newarmor.PhysicalBonus = oldarmor.PhysicalBonus;
                    newarmor.FireBonus = oldarmor.FireBonus;
                    newarmor.ColdBonus = oldarmor.ColdBonus;
                    newarmor.PoisonBonus = oldarmor.PoisonBonus;
                    newarmor.EnergyBonus = oldarmor.EnergyBonus;

                    newarmor.Altered = true;
                }
                else if (origItem is BaseClothing && newitem is BaseClothing)
                {
                    BaseClothing oldcloth = (BaseClothing)origItem;
                    BaseClothing newcloth = (BaseClothing)newitem;

                    if (oldcloth.PlayerConstructed)
                    {
                        newcloth.PlayerConstructed = true;
                        newcloth.Crafter = oldcloth.Crafter;
                        newcloth.Quality = oldcloth.Quality;
                    }

                    newcloth.Altered = true;
                }
                else if (origItem is BaseClothing && newitem is BaseArmor)
                {
                    BaseClothing oldcloth = (BaseClothing)origItem;
                    BaseArmor newarmor = (BaseArmor)newitem;

                    if (oldcloth.PlayerConstructed)
                    {
                        int qual = (int)oldcloth.Quality;

                        newarmor.PlayerConstructed = true;
                        newarmor.Crafter = oldcloth.Crafter;
                        newarmor.Quality = (ItemQuality)qual;
                    }

                    newarmor.Altered = true;
                }
                else if (origItem is BaseQuiver && newitem is BaseArmor)
                {
                    ((BaseArmor)newitem).Altered = true;
                }
                else
                {
                    return;
                }

                if (origItem.Name != null)
                {
                    newitem.Name = origItem.Name;
                }
                else if (VendorSearching.VendorSearch.StringList != null)
                {
                    if (origItem.LabelNumber > 0 && RetainsName(origItem))
                        newitem.Name = VendorSearching.VendorSearch.StringList.GetString(origItem.LabelNumber);
                }

                AlterResists(newitem, origItem);

                newitem.Hue = origItem.Hue;
                newitem.LootType = origItem.LootType;
                newitem.Insured = origItem.Insured;

                origItem.OnAfterDuped(newitem);
                newitem.Parent = null;

                if (origItem is IDurability && newitem is IDurability)
                {
                    ((IDurability)newitem).MaxHitPoints = ((IDurability)origItem).MaxHitPoints;
                    ((IDurability)newitem).HitPoints = ((IDurability)origItem).HitPoints;
                }

                if (from.Backpack == null)
                    newitem.MoveToWorld(from.Location, from.Map);
                else
                    from.Backpack.DropItem(newitem);

                newitem.InvalidateProperties();

                if (m_Contract != null)
                    m_Contract.Delete();

                origItem.Delete();

                EventSink.InvokeAlterItem(new AlterItemEventArgs(from, m_Tool is Item ? (Item)m_Tool : m_Contract, origItem, newitem));

                number = 1094727; // You have altered the item.
            }

            if (m_Tool != null)
                from.SendGump(new CraftGump(from, m_System, m_Tool, number));
            else
                from.SendLocalizedMessage(number);
        }

        private void AlterResists(Item newItem, Item oldItem)
        {
            if (newItem is BaseArmor || newItem is BaseClothing)
            {
                int[] newResists = Imbuing.GetBaseResists(newItem);
                int[] oldResists = Imbuing.GetBaseResists(oldItem);

                for (int i = 0; i < newResists.Length; i++)
                {
                    if (oldResists[i] > newResists[i])
                    {
                        Imbuing.SetProperty(newItem, 51 + i, oldResists[i] - newResists[i]);
                    }
                }
            }
        }

        private bool RetainsName(Item item)
        {
            if (item is Glasses || item is ElvenGlasses || item.IsArtifact)
                return true;

            if (item is IArtifact && ((IArtifact)item).ArtifactRarity > 0)
                return true;

            return (item.LabelNumber >= 1073505 && item.LabelNumber <= 1073552) || (item.LabelNumber >= 1073111 && item.LabelNumber <= 1075040);
        }

        private static readonly Type[] ArmorType =
        {
            typeof(RingmailGloves),    typeof(RingmailGlovesOfMining),
            typeof(PlateGloves),   typeof(LeatherGloves)
        };

        private static bool IsAlterable(Item item)
        {
            if (item is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)item;

                if (weapon.SetID != SetItem.None || !weapon.CanAlter || weapon.NegativeAttributes.Antique != 0)
                    return false;

                if ((Race.Gargoyle.ValidateEquipment(weapon) && !weapon.IsArtifact))
                    return false;
            }

            if (item is BaseArmor)
            {
                BaseArmor armor = (BaseArmor)item;

                if (armor.SetID != SetItem.None || !armor.CanAlter || armor.NegativeAttributes.Antique != 0)
                    return false;

                if ((Race.Gargoyle.ValidateEquipment(armor) && !armor.IsArtifact))
                    return false;

                if (ArmorType.Any(t => t == armor.GetType()) && armor.Resource > CraftResource.Iron)
                    return false;
            }

            if (item is BaseClothing)
            {
                BaseClothing cloth = (BaseClothing)item;

                if (cloth.SetID != SetItem.None || !cloth.CanAlter || cloth.NegativeAttributes.Antique != 0)
                    return false;

                if ((Race.Gargoyle.ValidateEquipment(cloth) && !cloth.IsArtifact))
                    return false;
            }

            if (item is BaseQuiver)
            {
                BaseQuiver quiver = (BaseQuiver)item;

                if (quiver.SetID != SetItem.None || !quiver.CanAlter)
                    return false;
            }

            if (item is IVvVItem && ((IVvVItem)item).IsVvVItem)
                return false;

            if (item is IRewardItem)
                return false;

            return true;
        }
    }
}
