using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Factions;

namespace Server.Engines.Craft
{
    public interface IRepairable
    {
        CraftSystem RepairSystem { get; }
    }

    public class Repair
    {
        public Repair()
        {
        }

        public static void Do(Mobile from, CraftSystem craftSystem, ITool tool)
        {
            from.Target = new InternalTarget(craftSystem, tool);
            from.SendLocalizedMessage(1044276); // Target an item to repair.
        }

        public static void Do(Mobile from, CraftSystem craftSystem, RepairDeed deed)
        {
            from.Target = new InternalTarget(craftSystem, deed);
            from.SendLocalizedMessage(1044276); // Target an item to repair.
        }

        private class InternalTarget : Target
        {
            private readonly CraftSystem m_CraftSystem;
            private readonly ITool m_Tool;
            private readonly RepairDeed m_Deed;

            public InternalTarget(CraftSystem craftSystem, ITool tool)
                : base(10, false, TargetFlags.None)
            {
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            public InternalTarget(CraftSystem craftSystem, RepairDeed deed)
                : base(2, false, TargetFlags.None)
            {
                m_CraftSystem = craftSystem;
                m_Deed = deed;
            }

            private static void EndMobileRepair(object state)
            {
                ((Mobile)state).EndAction(typeof(IRepairableMobile));
            }

            private int GetWeakenChance(Mobile mob, SkillName skill, int curHits, int maxHits)
            {
                // 40% - (1% per hp lost) - (1% per 10 craft skill)
                return (40 + (maxHits - curHits)) - (int)(((m_Deed != null) ? m_Deed.SkillLevel : mob.Skills[skill].Value) / 10);
            }

            private bool CheckWeaken(Mobile mob, SkillName skill, int curHits, int maxHits)
            {
                return (GetWeakenChance(mob, skill, curHits, maxHits) > Utility.Random(100));
            }

            private int GetRepairDifficulty(int curHits, int maxHits)
            {
                return (((maxHits - curHits) * 1250) / Math.Max(maxHits, 1)) - 250;
            }

            private bool CheckRepairDifficulty(Mobile mob, SkillName skill, int curHits, int maxHits)
            {
                double difficulty = GetRepairDifficulty(curHits, maxHits) * 0.1;

                if (m_Deed != null)
                {
                    double value = m_Deed.SkillLevel;
                    double minSkill = difficulty - 25.0;
                    double maxSkill = difficulty + 25;

                    if (value < minSkill)
                        return false; // Too difficult
                    else if (value >= maxSkill)
                        return true; // No challenge

                    double chance = (value - minSkill) / (maxSkill - minSkill);

                    return (chance >= Utility.RandomDouble());
                }
                else
                {
                    SkillLock sl = mob.Skills[SkillName.Tinkering].Lock;
                    mob.Skills[SkillName.Tinkering].SetLockNoRelay(SkillLock.Locked);

                    bool check = mob.CheckSkill(skill, difficulty - 25.0, difficulty + 25.0);

                    mob.Skills[SkillName.Tinkering].SetLockNoRelay(sl);

                    return check;
                }
            }

            private bool CheckDeed(Mobile from)
            {
                if (m_Deed != null)
                {
                    return m_Deed.Check(from);
                }

                return true;
            }

            private bool CheckSpecial(Item item)
            {
                return item is IRepairable && ((IRepairable)item).RepairSystem == m_CraftSystem;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                bool usingDeed = (m_Deed != null);
                bool toDelete = false;
                int number;

                if (m_CraftSystem is DefTinkering && targeted is IRepairableMobile)
                {
                    if (TryRepairMobile(from, (IRepairableMobile)targeted, usingDeed, out toDelete))
                    {
                        number = 1044279; // You repair the item.
                    }
                    else
                    {
                        number = 500426; // You can't repair that.
                    }
                }
                else if (targeted is Item)
                {
                    if (from.InRange(((Item)targeted).GetWorldLocation(), 2))
                    {
                        if (!CheckDeed(from))
                            return;

                        if (!AllowsRepair(targeted, m_CraftSystem))
                        {
                            from.SendLocalizedMessage(500426); // You can't repair that.
                            return;
                        }

                        if (m_CraftSystem.CanCraft(from, m_Tool, targeted.GetType()) == 1044267)
                        {
                            number = 1044282; // You must be near a forge and and anvil to repair items. * Yes, there are two and's *
                        }
                        else if (!usingDeed && m_CraftSystem is DefTinkering && targeted is BrokenAutomatonHead)
                        {
                            if (((BrokenAutomatonHead)targeted).TryRepair(from))
                                number = 1044279; // You repair the item.
                            else
                                number = 1044280; // You fail to repair the item.
                        }
                        else if (targeted is BaseWeapon)
                        {
                            BaseWeapon weapon = (BaseWeapon)targeted;
                            SkillName skill = m_CraftSystem.MainSkill;
                            int toWeaken = 0;

                            if (Core.AOS)
                            {
                                toWeaken = 1;
                            }
                            else if (skill != SkillName.Tailoring)
                            {
                                double skillLevel = (usingDeed) ? m_Deed.SkillLevel : from.Skills[skill].Base;

                                if (skillLevel >= 90.0)
                                    toWeaken = 1;
                                else if (skillLevel >= 70.0)
                                    toWeaken = 2;
                                else
                                    toWeaken = 3;
                            }

                            if (m_CraftSystem.CraftItems.SearchForSubclass(weapon.GetType()) == null && !CheckSpecial(weapon))
                            {
                                number = (usingDeed) ? 1061136 : 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
                            }
                            else if (!weapon.IsChildOf(from.Backpack) && (!Core.ML || weapon.Parent != from))
                            {
                                number = 1044275; // The item must be in your backpack to repair it.
                            }
                            else if (!Core.AOS && weapon.PoisonCharges != 0)
                            {
                                number = 1005012; // You cannot repair an item while a caustic substance is on it.
                            }
                            else if (weapon.MaxHitPoints <= 0 || weapon.HitPoints == weapon.MaxHitPoints)
                            {
                                number = 1044281; // That item is in full repair
                            }
                            else if (weapon.MaxHitPoints <= toWeaken)
                            {
                                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
                            }
                            else if (weapon.NegativeAttributes.NoRepair > 0)
                            {
                                number = 1044277; // That item cannot be repaired.
                            }
                            else
                            {
                                if (CheckWeaken(from, skill, weapon.HitPoints, weapon.MaxHitPoints))
                                {
                                    weapon.MaxHitPoints -= toWeaken;
                                    weapon.HitPoints = Math.Max(0, weapon.HitPoints - toWeaken);
                                }

                                if (CheckRepairDifficulty(from, skill, weapon.HitPoints, weapon.MaxHitPoints))
                                {
                                    number = 1044279; // You repair the item.
                                    m_CraftSystem.PlayCraftEffect(from);
                                    weapon.HitPoints = weapon.MaxHitPoints;
                                }
                                else
                                {
                                    number = (usingDeed) ? 1061137 : 1044280; // You fail to repair the item. [And the contract is destroyed]
                                    m_CraftSystem.PlayCraftEffect(from);
                                }

                                toDelete = true;
                            }
                        }
                        else if (targeted is BaseArmor)
                        {
                            BaseArmor armor = (BaseArmor)targeted;
                            SkillName skill = m_CraftSystem.MainSkill;
                            int toWeaken = 0;

                            if (Core.AOS)
                            {
                                toWeaken = 1;
                            }
                            else if (skill != SkillName.Tailoring)
                            {
                                double skillLevel = (usingDeed) ? m_Deed.SkillLevel : from.Skills[skill].Base;

                                if (skillLevel >= 90.0)
                                    toWeaken = 1;
                                else if (skillLevel >= 70.0)
                                    toWeaken = 2;
                                else
                                    toWeaken = 3;
                            }

                            if (m_CraftSystem.CraftItems.SearchForSubclass(armor.GetType()) == null && !CheckSpecial(armor))
                            {
                                number = (usingDeed) ? 1061136 : 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
                            }
                            else if (!armor.IsChildOf(from.Backpack) && (!Core.ML || armor.Parent != from))
                            {
                                number = 1044275; // The item must be in your backpack to repair it.
                            }
                            else if (armor.MaxHitPoints <= 0 || armor.HitPoints == armor.MaxHitPoints)
                            {
                                number = 1044281; // That item is in full repair
                            }
                            else if (armor.MaxHitPoints <= toWeaken)
                            {
                                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
                            }
                            else if (armor.NegativeAttributes.NoRepair > 0)
                            {
                                number = 1044277; // That item cannot be repaired.
                            }
                            else
                            {
                                if (CheckWeaken(from, skill, armor.HitPoints, armor.MaxHitPoints))
                                {
                                    armor.MaxHitPoints -= toWeaken;
                                    armor.HitPoints = Math.Max(0, armor.HitPoints - toWeaken);
                                }

                                if (CheckRepairDifficulty(from, skill, armor.HitPoints, armor.MaxHitPoints))
                                {
                                    number = 1044279; // You repair the item.
                                    m_CraftSystem.PlayCraftEffect(from);
                                    armor.HitPoints = armor.MaxHitPoints;
                                }
                                else
                                {
                                    number = (usingDeed) ? 1061137 : 1044280; // You fail to repair the item. [And the contract is destroyed]
                                    m_CraftSystem.PlayCraftEffect(from);
                                }

                                toDelete = true;
                            }
                        }
                        else if (targeted is BaseJewel)
                        {
                            BaseJewel jewel = (BaseJewel)targeted;
                            SkillName skill = m_CraftSystem.MainSkill;
                            int toWeaken = 0;

                            if (Core.AOS)
                            {
                                toWeaken = 1;
                            }
                            else if (skill != SkillName.Tailoring)
                            {
                                double skillLevel = (usingDeed) ? m_Deed.SkillLevel : from.Skills[skill].Base;

                                if (skillLevel >= 90.0)
                                    toWeaken = 1;
                                else if (skillLevel >= 70.0)
                                    toWeaken = 2;
                                else
                                    toWeaken = 3;
                            }

                            if (m_CraftSystem.CraftItems.SearchForSubclass(jewel.GetType()) == null && !CheckSpecial(jewel))
                            {
                                number = (usingDeed) ? 1061136 : 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
                            }
                            else if (!jewel.IsChildOf(from.Backpack))
                            {
                                number = 1044275; // The item must be in your backpack to repair it.
                            }
                            else if (jewel.MaxHitPoints <= 0 || jewel.HitPoints == jewel.MaxHitPoints)
                            {
                                number = 1044281; // That item is in full repair
                            }
                            else if (jewel.MaxHitPoints <= toWeaken)
                            {
                                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
                            }
                            else if (jewel.NegativeAttributes.NoRepair > 0)
                            {
                                number = 1044277; // That item cannot be repaired.
                            }
                            else
                            {
                                if (CheckWeaken(from, skill, jewel.HitPoints, jewel.MaxHitPoints))
                                {
                                    jewel.MaxHitPoints -= toWeaken;
                                    jewel.HitPoints = Math.Max(0, jewel.HitPoints - toWeaken);
                                }

                                if (CheckRepairDifficulty(from, skill, jewel.HitPoints, jewel.MaxHitPoints))
                                {
                                    number = 1044279; // You repair the item.
                                    m_CraftSystem.PlayCraftEffect(from);
                                    jewel.HitPoints = jewel.MaxHitPoints;
                                }
                                else
                                {
                                    number = (usingDeed) ? 1061137 : 1044280; // You fail to repair the item. [And the contract is destroyed]
                                    m_CraftSystem.PlayCraftEffect(from);
                                }

                                toDelete = true;
                            }
                        }
                        else if (targeted is BaseClothing)
                        {
                            BaseClothing clothing = (BaseClothing)targeted;
                            SkillName skill = m_CraftSystem.MainSkill;
                            int toWeaken = 0;

                            if (Core.AOS)
                            {
                                toWeaken = 1;
                            }
                            else if (skill != SkillName.Tailoring)
                            {
                                double skillLevel = (usingDeed) ? m_Deed.SkillLevel : from.Skills[skill].Base;

                                if (skillLevel >= 90.0)
                                    toWeaken = 1;
                                else if (skillLevel >= 70.0)
                                    toWeaken = 2;
                                else
                                    toWeaken = 3;
                            }

                            if (m_CraftSystem.CraftItems.SearchForSubclass(clothing.GetType()) == null && !CheckSpecial(clothing))
                            {
                                number = (usingDeed) ? 1061136 : 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
                            }
                            else if (!clothing.IsChildOf(from.Backpack) && (!Core.ML || clothing.Parent != from))
                            {
                                number = 1044275; // The item must be in your backpack to repair it.
                            }
                            else if (clothing.MaxHitPoints <= 0 || clothing.HitPoints == clothing.MaxHitPoints)
                            {
                                number = 1044281; // That item is in full repair
                            }
                            else if (clothing.MaxHitPoints <= toWeaken)
                            {
                                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
                            }
                            else if (clothing.NegativeAttributes.NoRepair > 0)// quick fix
                            {
                                number = 1044277; // That item cannot be repaired.
                            }
                            else
                            {
                                if (CheckWeaken(from, skill, clothing.HitPoints, clothing.MaxHitPoints))
                                {
                                    clothing.MaxHitPoints -= toWeaken;
                                    clothing.HitPoints = Math.Max(0, clothing.HitPoints - toWeaken);
                                }

                                if (CheckRepairDifficulty(from, skill, clothing.HitPoints, clothing.MaxHitPoints))
                                {
                                    number = 1044279; // You repair the item.
                                    m_CraftSystem.PlayCraftEffect(from);
                                    clothing.HitPoints = clothing.MaxHitPoints;
                                }
                                else
                                {
                                    number = (usingDeed) ? 1061137 : 1044280; // You fail to repair the item. [And the contract is destroyed]
                                    m_CraftSystem.PlayCraftEffect(from);
                                }

                                toDelete = true;
                            }
                        }
                        else if (targeted is BaseTalisman)
                        {
                            BaseTalisman talisman = (BaseTalisman)targeted;
                            SkillName skill = m_CraftSystem.MainSkill;
                            int toWeaken = 0;

                            if (Core.AOS)
                            {
                                toWeaken = 1;
                            }
                            else if (skill != SkillName.Tailoring)
                            {
                                double skillLevel = (usingDeed) ? m_Deed.SkillLevel : from.Skills[skill].Base;

                                if (skillLevel >= 90.0)
                                    toWeaken = 1;
                                else if (skillLevel >= 70.0)
                                    toWeaken = 2;
                                else
                                    toWeaken = 3;
                            }

                            if (!(m_CraftSystem is DefTinkering))
                            {
                                number = (usingDeed) ? 1061136 : 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
                            }
                            else if (!talisman.IsChildOf(from.Backpack) && (!Core.ML || talisman.Parent != from))
                            {
                                number = 1044275; // The item must be in your backpack to repair it.
                            }
                            else if (talisman.MaxHitPoints <= 0 || talisman.HitPoints == talisman.MaxHitPoints)
                            {
                                number = 1044281; // That item is in full repair
                            }
                            else if (talisman.MaxHitPoints <= toWeaken)
                            {
                                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
                            }
                            else if (!talisman.CanRepair)// quick fix
                            {
                                number = 1044277; // That item cannot be repaired.
                            }
                            else
                            {
                                if (CheckWeaken(from, skill, talisman.HitPoints, talisman.MaxHitPoints))
                                {
                                    talisman.MaxHitPoints -= toWeaken;
                                    talisman.HitPoints = Math.Max(0, talisman.HitPoints - toWeaken);
                                }

                                if (CheckRepairDifficulty(from, skill, talisman.HitPoints, talisman.MaxHitPoints))
                                {
                                    number = 1044279; // You repair the item.
                                    m_CraftSystem.PlayCraftEffect(from);
                                    talisman.HitPoints = talisman.MaxHitPoints;
                                }
                                else
                                {
                                    number = (usingDeed) ? 1061137 : 1044280; // You fail to repair the item. [And the contract is destroyed]
                                    m_CraftSystem.PlayCraftEffect(from);
                                }

                                toDelete = true;
                            }
                        }
                        else if (!usingDeed && targeted is BlankScroll)
                        {
                            SkillName skill = m_CraftSystem.MainSkill;

                            if (from.Skills[skill].Value >= 50.0)
                            {
                                ((BlankScroll)targeted).Consume(1);
                                RepairDeed deed = new RepairDeed(RepairDeed.GetTypeFor(m_CraftSystem), from.Skills[skill].Value, from);
                                from.AddToBackpack(deed);

                                number = 500442; // You create the item and put it in your backpack.
                            }
                            else
                                number = 1047005; // You must be at least apprentice level to create a repair service contract.
                        }
                        else
                        {
                            number = 500426; // You can't repair that.
                        }
                    }
                    else
                    {
                        number = 500446; // That is too far away.
                    }
                }
                else
                {
                    number = 500426; // You can't repair that.
                }

                if (!usingDeed)
                {
                    CraftContext context = m_CraftSystem.GetContext(from);
                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, number));
                }
                else
                {
                    from.SendLocalizedMessage(number);

                    if (toDelete)
                        m_Deed.Delete();
                }
            }

            public bool TryRepairMobile(Mobile from, IRepairableMobile m, bool usingDeed, out bool toDelete)
            {
                int damage = m.HitsMax - m.Hits;
                BaseCreature bc = m as BaseCreature;
                toDelete = false;

                string name = bc != null ? bc.Name : "the creature";

                if (!from.InRange(m.Location, 2))
                {
                    from.SendLocalizedMessage(1113612, name); // You must move closer to attempt to repair ~1_CREATURE~.
                }
                else if (bc != null && bc.IsDeadBondedPet)
                {
                    from.SendLocalizedMessage(500426); // You can't repair that.
                }
                else if (damage <= 0)
                {
                    from.SendLocalizedMessage(1113613, name); // ~1_CREATURE~ doesn't appear to be damaged.
                }
                else
                {
                    double skillValue = (usingDeed) ? m_Deed.SkillLevel : from.Skills[SkillName.Tinkering].Value;
                    double required = m is KotlAutomaton ? 80.0 : 0.1;

                    if (skillValue < required)
                    {
                        if (required == 80.0)
                            from.SendLocalizedMessage(1157049, name); // You must have at least 80 tinkering skill to attempt to repair ~1_CREATURE~.
                        else
                            from.SendLocalizedMessage(1113614, name); // You must have some tinkering skills to attempt to repair a ~1_CREATURE~.
                    }
                    else if (!from.CanBeginAction(typeof(IRepairableMobile)))
                    {
                        from.SendLocalizedMessage(1113611, name); // You must wait a moment before attempting to repair ~1_CREATURE~ again.
                    }
                    else if (bc != null && bc.GetMaster() != null && bc.GetMaster() != from && !bc.GetMaster().InRange(from.Location, 10))
                    {
                        from.SendLocalizedMessage(1157045); // The pet's owner must be nearby to attempt repair.
                    }
                    else if (!from.CanBeBeneficial(bc, false, false))
                    {
                        from.SendLocalizedMessage(1001017); // You cannot perform beneficial acts on your target.
                    }
                    else
                    {
                        if (damage > (int)(skillValue * 0.6))
                            damage = (int)(skillValue * 0.6);

                        SkillLock sl = from.Skills[SkillName.Tinkering].Lock;
                        from.Skills[SkillName.Tinkering].SetLockNoRelay(SkillLock.Locked);

                        if (!from.CheckSkill(SkillName.Tinkering, 0.0, 100.0))
                            damage /= 6;

                        from.Skills[SkillName.Tinkering].SetLockNoRelay(sl);

                        Container pack = from.Backpack;

                        if (pack != null)
                        {
                            int v = pack.ConsumeUpTo(m.RepairResource, (damage + 4) / 5);

                            if (v <= 0 && m is Golem)
                                v = pack.ConsumeUpTo(typeof(BronzeIngot), (damage + 4) / 5);

                            if (v > 0)
                            {
                                m.Hits += damage;

                                if (damage > 1)
                                    from.SendLocalizedMessage(1113616, name); // You repair ~1_CREATURE~.
                                else
                                    from.SendLocalizedMessage(1157030, name); // You repair ~1_CREATURE~, but it barely helps.

                                toDelete = true;
                                double delay = 10 - (skillValue / 16.65);

                                from.BeginAction(typeof(IRepairableMobile));
                                Timer.DelayCall(TimeSpan.FromSeconds(delay), new TimerStateCallback(EndMobileRepair), from);

                                return true;
                            }
                            else if (m is Golem)
                            {
                                from.SendLocalizedMessage(1113615, name); // You need some iron or bronze ingots to repair the ~1_CREATURE~.
                            }
                            else
                            {
                                from.SendLocalizedMessage(1044037); // You do not have sufficient metal to make that.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1044037); // You do not have sufficient metal to make that.
                        }
                    }
                }

                return false;
            }
        }

        public static bool AllowsRepair(object targeted, CraftSystem system)
        {
            if (targeted is IFactionItem && ((IFactionItem)targeted).FactionItemState != null)
                return false;

            if (targeted is BrokenAutomatonHead || targeted is IRepairableMobile)
                return true;

            return (targeted is BlankScroll ||
                    (targeted is BaseArmor && ((BaseArmor)targeted).CanRepair) ||
                    (targeted is BaseWeapon && ((BaseWeapon)targeted).CanRepair) ||
                    (targeted is BaseClothing && ((BaseClothing)targeted).CanRepair) ||
                    (targeted is BaseJewel && ((BaseJewel)targeted).CanRepair)) ||
                    (targeted is BaseTalisman && ((BaseTalisman)targeted).CanRepair);
        }
    }
}