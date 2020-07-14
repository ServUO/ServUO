#region References
using Server.Engines.VvV;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
using Server.Targeting;
using Server.Multis;

using System;
using System.Collections;
#endregion

namespace Server.SkillHandlers
{
    public delegate void ItemStolenEventHandler(ItemStolenEventArgs e);

    public class Stealing
    {
        public static void Initialize()
        {
            SkillInfo.Table[33].Callback = OnUse;
        }

        public static event ItemStolenEventHandler ItemStolen;

        public static readonly bool ClassicMode = false;
        public static readonly bool SuspendOnMurder = false;

        public static bool IsInGuild(Mobile m)
        {
            return (m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild);
        }

        public static bool IsInnocentTo(Mobile from, Mobile to)
        {
            return (Notoriety.Compute(from, to) == Notoriety.Innocent);
        }

        private class StealingTarget : Target
        {
            private readonly Mobile m_Thief;

            public StealingTarget(Mobile thief)
                : base(1, false, TargetFlags.None)
            {
                m_Thief = thief;

                AllowNonlocal = true;
            }

            private Item TryStealItem(Item toSteal, ref bool caught)
            {
                Item stolen = null;

                object root = toSteal.RootParent;

                StealableArtifactsSpawner.StealableInstance si = null;

                if (toSteal.Parent == null || !toSteal.Movable)
                {
                    si = toSteal is AddonComponent ? StealableArtifactsSpawner.GetStealableInstance(((AddonComponent)toSteal).Addon) : StealableArtifactsSpawner.GetStealableInstance(toSteal);
                }

                if (!IsEmptyHanded(m_Thief))
                {
                    m_Thief.SendLocalizedMessage(1005584); // Both hands must be free to steal.
                }
                else if (root is Mobile && ((Mobile)root).Player && !IsInGuild(m_Thief))
                {
                    m_Thief.SendLocalizedMessage(1005596); // You must be in the thieves guild to steal from other players.
                }
                else if (SuspendOnMurder && root is Mobile && ((Mobile)root).Player && IsInGuild(m_Thief) && m_Thief.Kills > 0)
                {
                    m_Thief.SendLocalizedMessage(502706); // You are currently suspended from the thieves guild.
                }
                else if (root is BaseVendor && ((BaseVendor)root).IsInvulnerable)
                {
                    m_Thief.SendLocalizedMessage(1005598); // You can't steal from shopkeepers.
                }
                else if (root is PlayerVendor)
                {
                    m_Thief.SendLocalizedMessage(502709); // You can't steal from vendors.
                }
                else if (!m_Thief.CanSee(toSteal))
                {
                    m_Thief.SendLocalizedMessage(500237); // Target can not be seen.
                }
                else if (m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold(m_Thief, toSteal, false, true))
                {
                    m_Thief.SendLocalizedMessage(1048147); // Your backpack can't hold anything else.
                }
                else if (toSteal is VvVSigil && ViceVsVirtueSystem.Instance != null)
                {
                    VvVPlayerEntry entry = ViceVsVirtueSystem.Instance.GetPlayerEntry<VvVPlayerEntry>(m_Thief);

                    VvVSigil sig = (VvVSigil)toSteal;

                    if (!m_Thief.InRange(toSteal.GetWorldLocation(), 1))
                    {
                        m_Thief.SendLocalizedMessage(502703); // You must be standing next to an item to steal it.
                    }
                    else if (root != null) // not on the ground
                    {
                        m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                    }
                    else if (entry != null)
                    {
                        if (!m_Thief.CanBeginAction(typeof(IncognitoSpell)))
                        {
                            m_Thief.SendLocalizedMessage(1010581); //	You cannot steal the sigil when you are incognito
                        }
                        else if (DisguiseTimers.IsDisguised(m_Thief))
                        {
                            m_Thief.SendLocalizedMessage(1010583); //	You cannot steal the sigil while disguised
                        }
                        else if (!m_Thief.CanBeginAction(typeof(PolymorphSpell)))
                        {
                            m_Thief.SendLocalizedMessage(1010582); //	You cannot steal the sigil while polymorphed				
                        }
                        else if (TransformationSpellHelper.UnderTransformation(m_Thief))
                        {
                            m_Thief.SendLocalizedMessage(1061622); // You cannot steal the sigil while in that form.
                        }
                        else if (AnimalForm.UnderTransformation(m_Thief))
                        {
                            m_Thief.SendLocalizedMessage(1063222); // You cannot steal the sigil while mimicking an animal.
                        }
                        else if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, 100.0, 120.0))
                        {
                            if (m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold(m_Thief, sig, false, true))
                            {
                                m_Thief.SendLocalizedMessage(1010259); //	The sigil has gone home because your backpack is full
                            }
                            else
                            {
                                m_Thief.SendLocalizedMessage(1010586); // YOU STOLE THE SIGIL!!!   (woah, calm down now)

                                sig.OnStolen(entry);

                                return sig;
                            }
                        }
                        else
                        {
                            m_Thief.SendLocalizedMessage(1005594); //	You do not have enough skill to steal the sigil
                        }
                    }
                    else
                    {
                        m_Thief.SendLocalizedMessage(1155415); //	Only participants in Vice vs Virtue may use this item.
                    }
                }
                else if (si == null && (toSteal.Parent == null || !toSteal.Movable) && !ItemFlags.GetStealable(toSteal))
                {
                    m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                }
                else if ((toSteal.LootType == LootType.Newbied || toSteal.CheckBlessed(root)) && !ItemFlags.GetStealable(toSteal))
                {
                    m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                }
                else if (si == null && toSteal is Container && !ItemFlags.GetStealable(toSteal))
                {
                    m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                }
                else if (!m_Thief.InRange(toSteal.GetWorldLocation(), 1))
                {
                    m_Thief.SendLocalizedMessage(502703); // You must be standing next to an item to steal it.
                }
                else if (si != null && m_Thief.Skills[SkillName.Stealing].Value < 100.0)
                {
                    m_Thief.SendLocalizedMessage(1060025, "", 0x66D); // You're not skilled enough to attempt the theft of this item.
                }
                else if (toSteal.Parent is Mobile)
                {
                    m_Thief.SendLocalizedMessage(1005585); // You cannot steal items which are equiped.
                }
                else if (root == m_Thief)
                {
                    m_Thief.SendLocalizedMessage(502704); // You catch yourself red-handed.
                }
                else if (root is Mobile && ((Mobile)root).IsStaff())
                {
                    m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                }
                else if (root is Mobile && !m_Thief.CanBeHarmful((Mobile)root))
                { }
                else if (root is Corpse || !CheckHouse(toSteal, root))
                {
                    m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                }
                else
                {
                    double w = toSteal.Weight + toSteal.TotalWeight;

                    if (w > 10)
                    {
                        m_Thief.SendMessage("That is too heavy to steal.");
                    }
                    else
                    {
                        if (toSteal.Stackable && toSteal.Amount > 1)
                        {
                            int maxAmount = (int)((m_Thief.Skills[SkillName.Stealing].Value / 10.0) / toSteal.Weight);

                            if (maxAmount < 1)
                            {
                                maxAmount = 1;
                            }
                            else if (maxAmount > toSteal.Amount)
                            {
                                maxAmount = toSteal.Amount;
                            }

                            int amount = Utility.RandomMinMax(1, maxAmount);

                            if (amount >= toSteal.Amount)
                            {
                                int pileWeight = (int)Math.Ceiling(toSteal.Weight * toSteal.Amount);
                                pileWeight *= 10;

                                if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, pileWeight - 22.5, pileWeight + 27.5))
                                {
                                    stolen = toSteal;
                                }
                            }
                            else
                            {
                                int pileWeight = (int)Math.Ceiling(toSteal.Weight * amount);
                                pileWeight *= 10;

                                if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, pileWeight - 22.5, pileWeight + 27.5))
                                {
                                    stolen = Mobile.LiftItemDupe(toSteal, toSteal.Amount - amount);

                                    if (stolen == null)
                                    {
                                        stolen = toSteal;
                                    }
                                }
                            }
                        }
                        else
                        {
                            int iw = (int)Math.Ceiling(w);
                            iw *= 10;

                            if (m_Thief.CheckTargetSkill(SkillName.Stealing, toSteal, iw - 22.5, iw + 27.5))
                            {
                                stolen = toSteal;
                            }
                        }

                        // Non-movable stealable (not in fillable container) items cannot result in the stealer getting caught
                        if (stolen != null && (root is FillableContainer || stolen.Movable))
                        {
                            double skillValue = m_Thief.Skills[SkillName.Stealing].Value;

                            if (root is FillableContainer)
                            {
                                caught = (Utility.Random((int)(skillValue / 2.5)) == 0); // 1 of 48 chance at 120
                            }
                            else
                            {
                                caught = (skillValue < Utility.Random(150));
                            }
                        }
                        else
                        {
                            caught = false;
                        }

                        if (stolen != null)
                        {
                            m_Thief.SendLocalizedMessage(502724); // You succesfully steal the item.

                            ItemFlags.SetTaken(stolen, true);
                            ItemFlags.SetStealable(stolen, false);
                            stolen.Movable = true;

                            InvokeItemStoken(new ItemStolenEventArgs(stolen, m_Thief));

                            if (si != null)
                            {
                                toSteal.Movable = true;
                                si.Item = null;
                            }
                        }
                        else
                        {
                            m_Thief.SendLocalizedMessage(502723); // You fail to steal the item.
                        }
                    }
                }

                return stolen;
            }

            private bool CheckHouse(Item stolen, object root)
            {
                var house = BaseHouse.FindHouseAt(stolen);

                if (house != null)
                {
                    var rootItem = root as Item;

                    if (rootItem != null)
                    {
                        SecureInfo secure = house.GetSecureInfoFor(rootItem);

                        return secure != null && house.HasSecureAccess(m_Thief, secure);
                    }
                }

                return true;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                from.RevealingAction();

                Item stolen = null;
                object root = null;
                bool caught = false;

                if (target is Item)
                {
                    root = ((Item)target).RootParent;
                    stolen = TryStealItem((Item)target, ref caught);
                }
                else if (target is Mobile)
                {
                    var bc = target as BaseCreature;
                    root = target;

                    if (bc != null && from is PlayerMobile)
                    {
                        if (bc.Controlled || bc.Summoned)
                        {
                            from.SendLocalizedMessage(502708); //You can't steal from this.
                        }
                        if (bc.HasBeenStolen)
                        {
                            from.SendLocalizedMessage(1094948); //That creature has already been stolen from.  There is nothing left to steal.
                        }
                        else
                        {
                            from.SendLocalizedMessage(1010579); // You reach into the backpack... and try to take something.

                            Engines.CreatureStealing.StealingHandler.HandleSteal(bc, (PlayerMobile)from, ref stolen);

                            if (stolen == null)
                            {
                                if (!bc.StealPackGenerated)
                                {
                                    bc.GenerateLoot(LootStage.Stolen);
                                }

                                StealRandom((Mobile)target, ref caught, ref stolen);
                            }
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1010579); // You reach into the backpack... and try to take something.
                        StealRandom((Mobile)target, ref caught, ref stolen);
                    }
                }
                else
                {
                    m_Thief.SendLocalizedMessage(502710); // You can't steal that!
                }

                if (stolen != null)
                {
                    if (stolen is AddonComponent)
                    {
                        BaseAddon addon = ((AddonComponent)stolen).Addon;
                        from.AddToBackpack(addon.Deed);
                        addon.Delete();
                    }
                    else
                    {
                        from.AddToBackpack(stolen);
                    }

                    if (!(stolen is Container || stolen.Stackable))
                    {
                        // do not return stolen containers or stackable items
                        StolenItem.Add(stolen, m_Thief, root as Mobile);
                    }

                    if (target is BaseCreature)
                    {
                        ((BaseCreature)target).HasBeenStolen = true;
                    }
                }

                if (caught)
                {
                    if (root == null)
                    {
                        m_Thief.CriminalAction(false);
                    }
                    else if (root is Corpse && ((Corpse)root).IsCriminalAction(m_Thief))
                    {
                        m_Thief.CriminalAction(false);
                    }
                    else if (root is Mobile)
                    {
                        Mobile mobRoot = (Mobile)root;

                        if (!IsInGuild(mobRoot) && IsInnocentTo(m_Thief, mobRoot))
                        {
                            m_Thief.CriminalAction(false);
                        }

                        string message = String.Format("You notice {0} trying to steal from {1}.", m_Thief.Name, mobRoot.Name);

                        foreach (NetState ns in m_Thief.GetClientsInRange(8))
                        {
                            if (ns.Mobile != m_Thief)
                            {
                                ns.Mobile.SendMessage(message);
                            }
                        }
                    }
                }
                else if (root is Corpse && ((Corpse)root).IsCriminalAction(m_Thief))
                {
                    m_Thief.CriminalAction(false);
                }

                if (root is Mobile && ((Mobile)root).Player && m_Thief is PlayerMobile && IsInnocentTo(m_Thief, (Mobile)root) &&
                    !IsInGuild((Mobile)root))
                {
                    PlayerMobile pm = (PlayerMobile)m_Thief;

                    pm.PermaFlags.Add((Mobile)root);
                    pm.Delta(MobileDelta.Noto);
                }
            }

            private void StealRandom(Mobile target, ref bool caught, ref Item stolen)
            {
                Container pack = target.Backpack;

                if (pack != null && pack.Items.Count > 0)
                {
                    stolen = TryStealItem(pack.Items[Utility.Random(pack.Items.Count)], ref caught);
                }
                else
                {
                    m_Thief.SendLocalizedMessage(1010578); // You reach into the backpack... but find it's empty.
                }
            }
        }

        public static bool IsEmptyHanded(Mobile from)
        {
            if (from.FindItemOnLayer(Layer.OneHanded) != null)
            {
                return false;
            }

            if (from.FindItemOnLayer(Layer.TwoHanded) != null)
            {
                return false;
            }

            return true;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (!IsEmptyHanded(m))
            {
                m.SendLocalizedMessage(1005584); // Both hands must be free to steal.
            }
            else
            {
                m.Target = new StealingTarget(m);
                m.RevealingAction();

                m.SendLocalizedMessage(502698); // Which item do you want to steal?
            }

            return TimeSpan.FromSeconds(10.0);
        }

        public static void InvokeItemStoken(ItemStolenEventArgs e)
        {
            if (ItemStolen != null)
            {
                ItemStolen(e);
            }
        }
    }

    public class StolenItem
    {
        public static readonly TimeSpan StealTime = TimeSpan.FromMinutes(2.0);

        private readonly Item m_Stolen;
        private readonly Mobile m_Thief;
        private readonly Mobile m_Victim;
        private DateTime m_Expires;

        public Item Stolen => m_Stolen;
        public Mobile Thief => m_Thief;
        public Mobile Victim => m_Victim;
        public DateTime Expires => m_Expires;

        public bool IsExpired => (DateTime.UtcNow >= m_Expires);

        public StolenItem(Item stolen, Mobile thief, Mobile victim)
        {
            m_Stolen = stolen;
            m_Thief = thief;
            m_Victim = victim;

            m_Expires = DateTime.UtcNow + StealTime;
        }

        private static readonly Queue m_Queue = new Queue();

        public static void Add(Item item, Mobile thief, Mobile victim)
        {
            Clean();

            m_Queue.Enqueue(new StolenItem(item, thief, victim));
        }

        public static bool IsStolen(Item item)
        {
            Mobile victim = null;

            return IsStolen(item, ref victim);
        }

        public static bool IsStolen(Item item, ref Mobile victim)
        {
            Clean();

            foreach (StolenItem si in m_Queue)
            {
                if (si.m_Stolen == item && !si.IsExpired)
                {
                    victim = si.m_Victim;
                    return true;
                }
            }

            return false;
        }

        public static void ReturnOnDeath(Mobile killed, Container corpse)
        {
            Clean();

            foreach (StolenItem si in m_Queue)
            {
                if (si.m_Stolen.RootParent == corpse && si.m_Victim != null && !si.IsExpired)
                {
                    if (si.m_Victim.AddToBackpack(si.m_Stolen))
                    {
                        si.m_Victim.SendLocalizedMessage(1010464); // the item that was stolen is returned to you.
                    }
                    else
                    {
                        si.m_Victim.SendLocalizedMessage(1010463); // the item that was stolen from you falls to the ground.
                    }

                    si.m_Expires = DateTime.UtcNow; // such a hack
                }
            }
        }

        public static void Clean()
        {
            while (m_Queue.Count > 0)
            {
                StolenItem si = (StolenItem)m_Queue.Peek();

                if (si.IsExpired)
                {
                    m_Queue.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }
    }

    public class ItemStolenEventArgs : EventArgs
    {
        public Item Item { get; set; }
        public Mobile Mobile { get; set; }

        public ItemStolenEventArgs(Item item, Mobile thief)
        {
            Mobile = thief;
            Item = item;
        }
    }
}
