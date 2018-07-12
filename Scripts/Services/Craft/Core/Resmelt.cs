using System;
using Server.Items;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public enum SmeltResult
    {
        Success,
        Invalid,
        NoSkill
    }

    public class Resmelt
    {
        public Resmelt()
        {
        }

        public static void Do(Mobile from, CraftSystem craftSystem, ITool tool)
        {
            int num = craftSystem.CanCraft(from, tool, null);

            if (num > 0 && num != 1044267)
            {
                from.SendGump(new CraftGump(from, craftSystem, tool, num));
            }
            else
            {
                from.Target = new InternalTarget(craftSystem, tool);
                from.SendLocalizedMessage(1044273); // Target an item to recycle.
            }
        }

        private class InternalTarget : Target
        {
            private readonly CraftSystem m_CraftSystem;
            private readonly ITool m_Tool;
            public InternalTarget(CraftSystem craftSystem, ITool tool)
                : base(2, false, TargetFlags.None)
            {
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int num = m_CraftSystem.CanCraft(from, m_Tool, null);

                if (num > 0)
                {
                    if (num == 1044267)
                    {
                        bool anvil, forge;
			
                        DefBlacksmithy.CheckAnvilAndForge(from, 2, out anvil, out forge);

                        if (!anvil)
                            num = 1044266; // You must be near an anvil
                        else if (!forge)
                            num = 1044265; // You must be near a forge.
                    }
					
                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, num));
                }
                else
                {
                    SmeltResult result = SmeltResult.Invalid;
                    bool isStoreBought = false;
                    int message;

                    if (targeted is BaseArmor)
                    {
                        result = Resmelt(from, (BaseArmor)targeted, ((BaseArmor)targeted).Resource);
                        isStoreBought = !((BaseArmor)targeted).PlayerConstructed;
                    }
                    else if (targeted is BaseWeapon)
                    {
                        result = Resmelt(from, (BaseWeapon)targeted, ((BaseWeapon)targeted).Resource);
                        isStoreBought = !((BaseWeapon)targeted).PlayerConstructed;
                    }
                    else if (targeted is DragonBardingDeed)
                    {
                        result = Resmelt(from, (DragonBardingDeed)targeted, ((DragonBardingDeed)targeted).Resource);
                        isStoreBought = false;
                    }

                    switch ( result )
                    {
                        default:
                        case SmeltResult.Invalid:
                            message = 1044272;
                            break; // You can't melt that down into ingots.
                        case SmeltResult.NoSkill:
                            message = 1044269;
                            break; // You have no idea how to work this metal.
                        case SmeltResult.Success:
                            message = isStoreBought ? 500418 : 1044270;
                            break; // You melt the item down into ingots.
                    }
					
                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, message));
                }
            }

            private SmeltResult Resmelt(Mobile from, Item item, CraftResource resource)
            {
                try
                {
                    if (Ethics.Ethic.IsImbued(item))
                        return SmeltResult.Invalid;

                    if (CraftResources.GetType(resource) != CraftResourceType.Metal)
                        return SmeltResult.Invalid;

                    CraftResourceInfo info = CraftResources.GetInfo(resource);

                    if (info == null || info.ResourceTypes.Length == 0)
                        return SmeltResult.Invalid;

                    CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(item.GetType());

                    if (craftItem == null || craftItem.Resources.Count == 0)
                        return SmeltResult.Invalid;

                    CraftRes craftResource = craftItem.Resources.GetAt(0);

                    if (craftResource.Amount < 2)
                        return SmeltResult.Invalid; // Not enough metal to resmelt

                    double difficulty = 0.0;

                    switch ( resource )
                    {
                        case CraftResource.DullCopper:
                            difficulty = 65.0;
                            break;
                        case CraftResource.ShadowIron:
                            difficulty = 70.0;
                            break;
                        case CraftResource.Copper:
                            difficulty = 75.0;
                            break;
                        case CraftResource.Bronze:
                            difficulty = 80.0;
                            break;
                        case CraftResource.Gold:
                            difficulty = 85.0;
                            break;
                        case CraftResource.Agapite:
                            difficulty = 90.0;
                            break;
                        case CraftResource.Verite:
                            difficulty = 95.0;
                            break;
                        case CraftResource.Valorite:
                            difficulty = 99.0;
                            break;
                    }

                    double skill = Math.Max(from.Skills[SkillName.Mining].Value, from.Skills[SkillName.Blacksmith].Value);

                    if (difficulty > skill)
                        return SmeltResult.NoSkill;

                    Type resourceType = info.ResourceTypes[0];
                    Item ingot = (Item)Activator.CreateInstance(resourceType);

                    if (item is DragonBardingDeed || (item is BaseArmor && ((BaseArmor)item).PlayerConstructed) || (item is BaseWeapon && ((BaseWeapon)item).PlayerConstructed) || (item is BaseClothing && ((BaseClothing)item).PlayerConstructed))
                        ingot.Amount = (int)((double)craftResource.Amount * .66);
                    else
                        ingot.Amount = 1;

                    item.Delete();
                    from.AddToBackpack(ingot);

                    from.PlaySound(0x2A);
                    from.PlaySound(0x240);
                    return SmeltResult.Success;
                }
                catch
                {
                }

                return SmeltResult.Invalid;
            }
        }
    }
}