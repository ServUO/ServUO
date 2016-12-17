using System;
using Server.Factions;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Server.Engines.VvV;

namespace Server.SkillHandlers
{
    public class DetectHidden
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile src)
        {
            src.SendLocalizedMessage(500819);//Where will you search?
            src.Target = new InternalTarget();

            return TimeSpan.FromSeconds(6.0);
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(12, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile src, object targ)
            {
                bool foundAnyone = false;

                Point3D p;
                if (targ is Mobile)
                    p = ((Mobile)targ).Location;
                else if (targ is Item)
                    p = ((Item)targ).Location;
                else if (targ is IPoint3D)
                    p = new Point3D((IPoint3D)targ);
                else 
                    p = src.Location;

                double srcSkill = src.Skills[SkillName.DetectHidden].Value;
                int range = (int)(srcSkill / 10.0);

                if (!src.CheckSkill(SkillName.DetectHidden, 0.0, 100.0))
                    range /= 2;

                BaseHouse house = BaseHouse.FindHouseAt(p, src.Map, 16);

                bool inHouse = (house != null && house.IsFriend(src));

                if (inHouse)
                    range = 22;

                if (range > 0)
                {
                    IPooledEnumerable inRange = src.Map.GetMobilesInRange(p, range);

                    foreach (Mobile trg in inRange)
                    {
                        if (trg.Hidden && src != trg)
                        {
                            double ss = srcSkill + Utility.Random(21) - 10;
                            double ts = trg.Skills[SkillName.Hiding].Value + Utility.Random(21) - 10;

                            if (src.AccessLevel >= trg.AccessLevel && (ss >= ts || (inHouse && house.IsInside(trg))))
                            {
                                if (trg is ShadowKnight && (trg.X != p.X || trg.Y != p.Y))
                                    continue;

                                trg.RevealingAction();
                                trg.SendLocalizedMessage(500814); // You have been revealed!
                                foundAnyone = true;
                            }
                        }
                    }

                    inRange.Free();

                    bool faction = Faction.Find(src) != null;
                    bool vvv = ViceVsVirtueSystem.IsVvV(src);

                    if (faction || vvv)
                    {
                        IPooledEnumerable itemsInRange = src.Map.GetItemsInRange(p, range);

                        foreach (Item item in itemsInRange)
                        {
                            if (faction && item is BaseFactionTrap)
                            {
                                BaseFactionTrap trap = (BaseFactionTrap)item;

                                if (src.CheckTargetSkill(SkillName.DetectHidden, trap, 80.0, 100.0))
                                {
                                    src.SendLocalizedMessage(1042712, true, " " + (trap.Faction == null ? "" : trap.Faction.Definition.FriendlyName)); // You reveal a trap placed by a faction:

                                    trap.Visible = true;
                                    trap.BeginConceal();

                                    foundAnyone = true;
                                }
                            }
                            else if (vvv && (item is VvVSigil || item is VvVTrap) && Utility.Random(100) <= srcSkill)
                            {
                                if (item is VvVTrap && item.ItemID == VvVTrap.HiddenID)
                                    ((VvVTrap)item).OnRevealed(src);
                                else if (!item.Visible)
                                    item.Visible = true;
                            }
                        }

                        itemsInRange.Free();
                    }
                }

                if (!foundAnyone)
                {
                    src.SendLocalizedMessage(500817); // You can see nothing hidden there.
                }
            }
        }

        public static void DoPassiveDetect(Mobile src)
        {
			if (src == null || src.Map == null || src.Location == Point3D.Zero || src.IsStaff())
				return;

            double ss = src.Skills[SkillName.DetectHidden].Value;

            if (ss <= 0)
                return;

            IPooledEnumerable eable = src.Map.GetMobilesInRange(src.Location, 4);

			if (eable == null)
				return;

            foreach (Mobile m in eable)
            {
                if (m == null || m is ShadowKnight)
                    continue;

                int noto = Notoriety.Compute(src, m);

                if (m != src && noto != Notoriety.Innocent && noto != Notoriety.Ally && noto != Notoriety.Invulnerable)
                {
                    double ts = (m.Skills[SkillName.Hiding].Value + m.Skills[SkillName.Stealth].Value) / 2;

                    if (src.Race == Race.Elf)
                        ss += 20;

                    if (src.AccessLevel >= m.AccessLevel && Utility.Random(1000) < (ss - ts) + 1)
                    {
                        m.RevealingAction();
                        m.SendLocalizedMessage(500814); // You have been revealed!
                    }
                }
            }

            eable.Free();
        }
    }
}