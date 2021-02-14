using Server.Engines.Harvest;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Targets
{
    public class BladedItemTarget : Target
    {
        private readonly Item m_Item;
        public BladedItemTarget(Item item)
            : base(2, false, TargetFlags.None)
        {
            m_Item = item;
        }

        protected override void OnTargetOutOfRange(Mobile from, object targeted)
        {
            if (targeted is UnholyBone bone && from.InRange(bone, 12))
            {
                if (bone.Carve(from, m_Item) && Siege.SiegeShard)
                {
                    Siege.CheckUsesRemaining(from, m_Item);
                }
            }
            else
                base.OnTargetOutOfRange(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Item.Deleted)
                return;

            if (targeted is ICarvable carvable)
            {
                if (carvable is Item item && (item.IsLockedDown || item.RootParent is Container container && (!item.Movable || !container.LiftOverride)))
                {
                    from.SendLocalizedMessage(500494); // You can't use a bladed item on that!
                    return;
                }

                if (carvable.Carve(from, m_Item) && Siege.SiegeShard)
                {
                    Siege.CheckUsesRemaining(from, m_Item);
                }
            }
            else if (targeted is SwampDragon pet && pet.HasBarding)
            {
                if (!pet.Controlled || pet.ControlMaster != from)
                    from.SendLocalizedMessage(1053022); // You cannot remove barding from a swamp dragon you do not own.
                else
                {
                    pet.HasBarding = false;

                    if (Siege.SiegeShard && m_Item is IUsesRemaining)
                    {
                        Siege.CheckUsesRemaining(from, m_Item);
                    }
                }
            }
            else
            {
                if (targeted is Mobile mobile)
                {
                    mobile.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500450, from.NetState); // You can only skin dead creatures.
                    return;
                }

                if (targeted is StaticTarget sTarget)
                {
                    int itemID = sTarget.ItemID;

                    if (itemID == 0xD15 || itemID == 0xD16) // red mushroom
                    {
                        PlayerMobile player = from as PlayerMobile;

                        if (player != null)
                        {
                            QuestSystem qs = player.Quest;

                            if (qs is WitchApprenticeQuest)
                            {
                                FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                                if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.RedMushrooms)
                                {
                                    player.SendLocalizedMessage(1055036); // You slice a red cap mushroom from its stem.
                                    obj.Complete();

                                    if (Siege.SiegeShard && m_Item is IUsesRemaining)
                                    {
                                        Siege.CheckUsesRemaining(from, m_Item);
                                    }

                                    return;
                                }
                            }
                        }
                    }
                }

                HarvestSystem system = Lumberjacking.System;
                HarvestDefinition def = Lumberjacking.System.Definition;

                int tileID;
                Map map;
                Point3D loc;

                if (!system.GetHarvestDetails(from, m_Item, targeted, out tileID, out map, out loc))
                {
                    from.SendLocalizedMessage(500494); // You can't use a bladed item on that!
                }
                else if (!def.Validate(tileID))
                {
                    from.SendLocalizedMessage(500494); // You can't use a bladed item on that!
                }
                else
                {
                    HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

                    if (bank == null)
                        return;

                    if (bank.Current < 5)
                    {
                        from.SendLocalizedMessage(500493); // There's not enough wood here to harvest.
                    }
                    else
                    {
                        bank.Consume(5, from);

                        Item item = new Kindling();

                        if (from.PlaceInBackpack(item))
                        {
                            from.SendLocalizedMessage(500491); // You put some kindling into your backpack.
                            from.SendLocalizedMessage(500492); // An axe would probably get you more wood.
                        }
                        else
                        {
                            from.SendLocalizedMessage(500490); // You can't place any kindling into your backpack!

                            item.Delete();
                        }

                        if (Siege.SiegeShard && m_Item is IUsesRemaining)
                        {
                            Siege.CheckUsesRemaining(from, m_Item);
                        }
                    }
                }
            }
        }
    }
}
