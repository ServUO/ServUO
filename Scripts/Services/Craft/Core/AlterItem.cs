using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Items;
using Server.Targeting;
using Server.Engines.VeteranRewards;

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
    }

    public class AlterItem
    {
        public static void BeginTarget(Mobile from, CraftSystem system, BaseTool tool)
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
        private readonly BaseTool m_Tool;
        private Item m_Contract;

        public AlterItemTarget(CraftSystem system, Item contract)
                : base(2, false, TargetFlags.None)
        {
            m_System = system;
            m_Contract = contract;
        }

        public AlterItemTarget(CraftSystem system, BaseTool tool)
            : base(1, false, TargetFlags.None)
        {
            this.m_System = system;
            this.m_Tool = tool;
        }

        private static AlterableAttribute GetAlterableAttribute(object o, bool inherit = false)
        {
            object[] attrs = o.GetType().GetCustomAttributes(typeof(AlterableAttribute), inherit);

            if (attrs != null && attrs.Length > 0)
            {
                AlterableAttribute attr = attrs[0] as AlterableAttribute;

                if (attr != null && (!inherit || attr.Inherit))
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

            var alterInfo = GetAlterableAttribute(o, false);

            if (alterInfo == null)
                alterInfo = GetAlterableAttribute(o, true);

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
                            contract = new AlterContract(RepairDeed.RepairSkillType.Smithing, from);
                        else if (skill == SkillName.Carpentry)
                            contract = new AlterContract(RepairDeed.RepairSkillType.Carpentry, from);
                        else if (skill == SkillName.Tailoring)
                            contract = new AlterContract(RepairDeed.RepairSkillType.Tailoring, from);
                        else if (skill == SkillName.Tinkering)
                            contract = new AlterContract(RepairDeed.RepairSkillType.Tinkering, from);

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
                // You may not alter that item.
                number = 1094728;
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
            else if (!Server.SkillHandlers.Imbuing.CheckSoulForge(from, 2, false))
            {
                number = 1111867; // You must be near a soulforge to alter an item.
            }
            else if (!Server.SkillHandlers.Imbuing.CheckQueen(from))
            {
                number = 1113736; // You must rise to the rank of noble in the eyes of the Gargoyle Queen before her majesty will allow you to use this soulforge.
            }
            else if (m_Contract == null && value < 100.0)
            {
                number = 1111870; // You must be at least grandmaster level to alter an item.
            }
            else if (origItem is BaseWeapon && ((BaseWeapon)origItem).EnchantedWeilder != null)
            {
                number = 1111849; // You cannot alter an item that is currently enchanted.
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

                    newweapon.Attributes = new AosAttributes(oldweapon, newweapon.Attributes);
                    //newweapon.ElementDamages = new AosElementAttributes( oldweapon, newweapon.ElementDamages ); To Do
                    newweapon.SkillBonuses = new AosSkillBonuses(oldweapon, newweapon.SkillBonuses);
                    newweapon.WeaponAttributes = new AosWeaponAttributes(oldweapon, newweapon.WeaponAttributes);
                    newweapon.AbsorptionAttributes = new SAAbsorptionAttributes(oldweapon, newweapon.AbsorptionAttributes);
                    newweapon.Altered = true;
                }
                else if (origItem is BaseArmor && newitem is BaseArmor)
                {
                    BaseArmor oldarmor = (BaseArmor)origItem;
                    BaseArmor newarmor = (BaseArmor)newitem;

                    newarmor.Attributes = new AosAttributes(oldarmor, newarmor.Attributes);
                    newarmor.ArmorAttributes = new AosArmorAttributes(oldarmor, newarmor.ArmorAttributes);
                    newarmor.SkillBonuses = new AosSkillBonuses(oldarmor, newarmor.SkillBonuses);
                    newarmor.AbsorptionAttributes = new SAAbsorptionAttributes(oldarmor, newarmor.AbsorptionAttributes);
                    newarmor.Altered = true;
                }
                else if (origItem is BaseClothing && newitem is BaseClothing)
                {
                    BaseClothing oldcloth = (BaseClothing)origItem;
                    BaseClothing newcloth = (BaseClothing)newitem;

                    newcloth.Attributes = new AosAttributes(oldcloth, newcloth.Attributes);
                    newcloth.SkillBonuses = new AosSkillBonuses(oldcloth, newcloth.SkillBonuses);
                    newcloth.SAAbsorptionAttributes = new SAAbsorptionAttributes(oldcloth, newcloth.SAAbsorptionAttributes);
                    newcloth.Altered = true;
                }
                else if (origItem is BaseClothing && newitem is BaseArmor)
                {
                    BaseClothing oldcloth = (BaseClothing)origItem;
                    BaseArmor newarmor = (BaseArmor)newitem;

                    newarmor.Attributes = new AosAttributes(oldcloth, newarmor.Attributes);
                    newarmor.ArmorAttributes = new AosArmorAttributes(oldcloth, newarmor.ArmorAttributes);
                    newarmor.SkillBonuses = new AosSkillBonuses(oldcloth, newarmor.SkillBonuses);
                    newarmor.AbsorptionAttributes = new SAAbsorptionAttributes(oldcloth, newarmor.AbsorptionAttributes);
                    newarmor.Altered = true;
                }
                else if (origItem is BaseQuiver && newitem is BaseArmor)
                {
                    BaseQuiver oldquiver = (BaseQuiver)origItem;
                    BaseArmor newarmor = (BaseArmor)newitem;

                    newarmor.Attributes = new AosAttributes(oldquiver, newarmor.Attributes);
                    newarmor.ArmorAttributes = new AosArmorAttributes(oldquiver, newarmor.ArmorAttributes);
                    newarmor.SkillBonuses = new AosSkillBonuses(oldquiver, newarmor.SkillBonuses);
                    newarmor.AbsorptionAttributes = new SAAbsorptionAttributes(oldquiver, newarmor.AbsorptionAttributes);
                    newarmor.Altered = true;
                }
                else
                {
                    return;
                }

                newitem.Name = origItem.Name;
                newitem.Hue = origItem.Hue;

                newitem.LootType = origItem.LootType;

                origItem.Delete();
                origItem.OnAfterDuped(newitem);
                newitem.Parent = null;

                if (from.Backpack == null)
                    newitem.MoveToWorld(from.Location, from.Map);
                else
                    from.Backpack.DropItem(newitem);

                newitem.InvalidateProperties();

                if (m_Contract != null)
                    m_Contract.Delete();

                number = 1094727; // You have altered the item.
            }

            if (m_Tool != null)
                from.SendGump(new CraftGump(from, m_System, m_Tool, number));
            else
                from.SendLocalizedMessage(number);
        }

        private static bool IsAlterable(Item item)
        {
            if (item is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)item;

                if (weapon.SetID != SetItem.None || !weapon.CanAlter)
                    return false;

                if ((weapon.RequiredRace != null && weapon.RequiredRace == Race.Gargoyle && !weapon.IsArtifact))
                    return false;
            }

            if (item is BaseArmor)
            {
                BaseArmor armor = (BaseArmor)item;

                if (armor.SetID != SetItem.None || !armor.CanAlter)
                    return false;

                if ((armor.RequiredRace != null && armor.RequiredRace == Race.Gargoyle && !armor.IsArtifact))
                    return false;
            }

            if (item is BaseClothing)
            {
                BaseClothing cloth = (BaseClothing)item;

                if (cloth.SetID != SetItem.None || !cloth.CanAlter)
                    return false;

                if ((cloth.RequiredRace != null && cloth.RequiredRace == Race.Gargoyle && !cloth.IsArtifact))
                    return false;
            }

	        if (item is BaseQuiver)
	        {
		        BaseQuiver quiver = (BaseQuiver) item;

		        if (quiver.SetID != SetItem.None || !quiver.CanAlter)
			        return false;
	        }

            if (item is IRewardItem)
                return false;

            return true;
        }
    }
}