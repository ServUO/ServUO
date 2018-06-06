using System;
using Server.Factions;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Server.Engines.VvV;
using Server.Items;
using System.Collections.Generic;
using System.Linq;
using Server.Spells;

namespace Server.Items
{
    public interface IRevealableItem
    {
        bool CheckReveal(Mobile m);
        bool CheckPassiveDetect(Mobile m);
        void OnRevealed(Mobile m);
    }
}

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

            return TimeSpan.FromSeconds(10.0);
        }

        public class InternalTarget : Target
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

                bool inHouse = house != null && house.IsFriend(src);

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
                            double shadow = Server.Spells.SkillMasteries.ShadowSpell.GetDifficultyFactor(trg);
                            bool houseCheck = inHouse && house.IsInside(trg);

                            if (src.AccessLevel >= trg.AccessLevel && (ss >= ts || houseCheck) && Utility.RandomDouble() > shadow)
                            {
                                if ((trg is ShadowKnight && (trg.X != p.X || trg.Y != p.Y)) ||
                                    (!houseCheck && !CanDetect(src, trg)))
                                    continue;

                                trg.RevealingAction();
                                trg.SendLocalizedMessage(500814); // You have been revealed!
                                foundAnyone = true;
                            }
                        }
                    }

                    inRange.Free();

                    IPooledEnumerable itemsInRange = src.Map.GetItemsInRange(p, range);

                    foreach (Item item in itemsInRange)
                    {
                        if (item.Visible)
                            continue;

                        IRevealableItem dItem = item as IRevealableItem;

                        if (dItem != null && dItem.CheckReveal(src))
                        {
                            dItem.OnRevealed(src);

                            foundAnyone = true;
                        }
                    }

                    itemsInRange.Free();
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
                if (m == null || m == src || m is ShadowKnight || !CanDetect(src, m))
                    continue;

                double ts = (m.Skills[SkillName.Hiding].Value + m.Skills[SkillName.Stealth].Value) / 2;

                if (src.Race == Race.Elf)
                    ss += 20;

                if (src.AccessLevel >= m.AccessLevel && Utility.Random(1000) < (ss - ts) + 1)
                {
                    m.RevealingAction();
                    m.SendLocalizedMessage(500814); // You have been revealed!
                }
            }

            eable.Free();

            eable = src.Map.GetItemsInRange(src.Location, 8);

            foreach (Item item in eable)
            {
                if (!item.Visible && item is IRevealableItem && ((IRevealableItem)item).CheckPassiveDetect(src))
                {
                    src.SendLocalizedMessage(1153493); // Your keen senses detect something hidden in the area...
                }
            }

            eable.Free();
        }

        private static bool CanDetect(Mobile src, Mobile target)
        {
            if (src.Map == null || src.Blessed || (src is BaseCreature && ((BaseCreature)src).IsInvulnerable))
                return false;

            if (target.Blessed || (target is BaseCreature && ((BaseCreature)target).IsInvulnerable))
                return false;

            return src.CanBeHarmful(target, false) && (src.Map.Rules != MapRules.FeluccaRules || SpellHelper.ValidIndirectTarget(src, target));
        }
    }
}