using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Harvest
{
    public class HarvestTarget : Target
    {
        private readonly Item m_Tool;
        private readonly HarvestSystem m_System;

        public HarvestTarget(Item tool, HarvestSystem system)
            : base(-1, true, TargetFlags.None)
        {
            m_Tool = tool;
            m_System = system;

            DisallowMultis = true;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_System is Mining)
            {
                if (targeted is StaticTarget target)
                {
                    int itemID = target.ItemID;

                    // grave
                    if (itemID == 0xED3 || itemID == 0xEDF || itemID == 0xEE0 || itemID == 0xEE1 || itemID == 0xEE2 || itemID == 0xEE8)
                    {
                        PlayerMobile player = from as PlayerMobile;

                        if (player != null)
                        {
                            QuestSystem qs = player.Quest;

                            if (qs is WitchApprenticeQuest)
                            {
                                FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                                if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.Bones)
                                {
                                    player.SendLocalizedMessage(1055037); // You finish your grim work, finding some of the specific bones listed in the Hag's recipe.
                                    obj.Complete();

                                    return;
                                }
                            }
                        }
                    }
                }
                else if (targeted is LandTarget landTarget && landTarget.TileID >= 113 && landTarget.TileID <= 120)
                {
                    if (TheGreatVolcanoQuest.OnHarvest(from, m_Tool))
                        return;
                }
            }

            if (m_System is Lumberjacking && targeted is IChopable chopable)
            {
                chopable.OnChop(from);
            }
            else if (m_System is Lumberjacking && targeted is IAxe obj && m_Tool is BaseAxe axe)
            {
                Item item = (Item)obj;

                if (!item.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                else if (obj.Axe(from, axe))
                    from.PlaySound(0x13E);
            }
            else if (m_System is Lumberjacking && targeted is ICarvable carvable)
                carvable.Carve(from, m_Tool);
            else if (m_System is Lumberjacking && FurnitureAttribute.Check(targeted as Item))
                DestroyFurniture(from, (Item)targeted);
            else if (m_System is Mining && targeted is TreasureMap map)
                map.OnBeginDig(from);
            else if (m_System is Mining && targeted is NiterDeposit niter)
                niter.OnMine(from, m_Tool);
            else if (m_System is Lumberjacking && targeted is CrackedLavaRockEast lavaRockEast)
                lavaRockEast.OnCrack(from);
            else if (m_System is Lumberjacking && targeted is CrackedLavaRockSouth lavaRockSouth)
                lavaRockSouth.OnCrack(from);
            else
            {
                // If we got here and we're lumberjacking then we didn't target something that can be done from the pack
                if (m_System is Lumberjacking && m_Tool.Parent != from)
                {
                    from.SendLocalizedMessage(500487); // The axe must be equipped for any serious wood chopping.
                    return;
                }
                m_System.StartHarvesting(from, m_Tool, targeted);
            }
        }

        private void DestroyFurniture(Mobile from, Item item)
        {
            if (!from.InRange(item.GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (!item.IsChildOf(from.Backpack) && !item.Movable)
            {
                from.SendLocalizedMessage(500462); // You can't destroy that while it is here.
                return;
            }

            from.SendLocalizedMessage(500461); // You destroy the item.
            Effects.PlaySound(item.GetWorldLocation(), item.Map, 0x3B3);

            if (item is Container container)
            {
                if (container is TrapableContainer trapableContainer)
                    trapableContainer.ExecuteTrap(from);

                container.Destroy();
            }
            else
            {
                item.Delete();
            }
        }
    }
}
