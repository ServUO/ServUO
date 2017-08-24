using System;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Items
{
    public class ItemIdentification
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile from)
        {
            from.SendLocalizedMessage(500343); // What do you wish to appraise and identify?
            from.Target = new InternalTarget();

            return TimeSpan.FromSeconds(1.0);
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
                this.AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                Item item = o as Item;
                Mobile m = o as Mobile;

                if (item == null && m == null)
                {
                    from.SendLocalizedMessage(500353); // You are not certain...
                    return;
                }
                
                if (!from.CheckTargetSkill(SkillName.ItemID, o, 0, 100))
                {
                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1041352, from.NetState); // You have no idea how much it might be worth.
                    return;
                }

                if (m != null)
                {
                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1041349, AffixType.Append, "  " + m.Name, "", from.NetState); // It appears to be:
                    return;
                }
                
                if (item.Name != null)
                {
                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, false, item.Name, from.NetState);
                    item.PrivateOverheadMessage(MessageType.Label, 0x3B2, false, item.Name, from.NetState);
                }
                else
                {
                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, item.LabelNumber, from.NetState);
                    item.PrivateOverheadMessage(MessageType.Label, 0x3B2, item.LabelNumber, from.NetState);
                }

                if (Core.AOS)
                {
                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1041351, AffixType.Append, "  " + GetPriceFor(item).ToString(), "", from.NetState); // You guess the value of that item at:

                    if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
                    {
                        if (Imbuing.TimesImbued(item) > 0)
                        {
                            from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1111877, from.NetState); // You conclude that item cannot be magically unraveled. The magic in that item has been weakened due to either low durability or the imbuing process.
                        }
                        else
                        {
                            int weight = Imbuing.GetTotalWeight(item);
                            string imbIngred = null;
                            double skill = from.Skills[SkillName.Imbuing].Base;
                            bool badSkill = false;

                            if (!Imbuing.CanUnravelItem(from, item, false))
                            {
                                weight = 0;
                            }

                            if (weight > 0 && weight <= 200)
                            {
                                imbIngred = "Magical Residue";
                            }
                            else if (weight > 200 && weight < 480)
                            {
                                imbIngred = "Enchanted Essence";

                                if (skill < 45.0)
                                    badSkill = true;
                            }
                            else if (weight >= 480)
                            {
                                imbIngred = "Relic Fragment";

                                if (skill < 95.0)
                                    badSkill = true;
                            }

                            if (imbIngred != null)
                            {
                                if (badSkill)
                                {
                                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1111875, from.NetState); // Your Imbuing skill is not high enough to identify the imbuing ingredient.
                                }
                                else
                                {
                                    from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1111874, imbIngred, from.NetState); //You conclude that item will magically unravel into: ~1_ingredient~
                                }
                            }
                            else  // Cannot be Unravelled
                            {
                                from.PrivateOverheadMessage(MessageType.Emote, 0x3B2, 1111876, from.NetState); //You conclude that item cannot be magically unraveled. It appears to possess little to no magic.
                            }
                        }
                    }
                    else
                    {
                        from.LocalOverheadMessage(MessageType.Emote, 0x3B2, 1111878); //You conclude that item cannot be magically unraveled.
                    }
                }
                else if (o is Item)
                {
                    if (from.CheckTargetSkill(SkillName.ItemID, o, 0, 100))
                    {
                        if (o is BaseWeapon)
                            ((BaseWeapon)o).Identified = true;
                        else if (o is BaseArmor)
                            ((BaseArmor)o).Identified = true;

                        if (!Core.AOS)
                            ((Item)o).OnSingleClick(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else if (o is Mobile)
                {
                    ((Mobile)o).OnSingleClick(from);
                }
                else
                {
                    from.SendLocalizedMessage(500353); // You are not certain...
                }

                Server.Engines.XmlSpawner2.XmlAttach.RevealAttachments(from, o);
            }

            public static int GetPriceFor(Item item)
            {
                Type type = item.GetType();

                if (GenericBuyInfo.BuyPrices.ContainsKey(type))
                {
                    return GenericBuyInfo.BuyPrices[item.GetType()] * item.Amount;
                }

                if (TypeCostCache == null)
                    TypeCostCache = new Dictionary<Type, int>();

                if (!TypeCostCache.ContainsKey(type))
                    TypeCostCache[type] = Utility.RandomMinMax(2, 7);

                return TypeCostCache[type];
            }

            public static Dictionary<Type, int> TypeCostCache { get; set; }
        }
    }
}