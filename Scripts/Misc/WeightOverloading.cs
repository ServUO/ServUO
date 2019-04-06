using System;
using System.Collections.Generic;

using Server.Mobiles;
using Server.Items;

namespace Server.Misc
{
    public class WeightOverloading
    {
        public const int OverloadAllowance = 4;// We can be four stones overweight without getting fatigued

        public static void Initialize()
        {
            EventSink.Movement += new MovementEventHandler(EventSink_Movement);
            Mobile.FatigueHandler = FatigueOnDamage;
        }

        public static void FatigueOnDamage(Mobile m, int damage, DFAlgorithm df)
        {
            double fatigue = 0.0;

            switch (m.DFA)
            {
                case DFAlgorithm.Standard:
                    {
                        if (Core.SA && m.Player)
                        {
                            fatigue = damage + Utility.RandomMinMax(-3, 3);
                        }
                        else
                        {
                            fatigue = (damage * (100.0 / m.Hits) * ((double)m.Stam / 100)) - 5.0;
                        }
                        break;
                    }
                case DFAlgorithm.PainSpike:
                    {
                        if (Core.SA && m.Player)
                        {
                            fatigue = (damage * 2) + Utility.RandomMinMax(-3, 3);
                        }
                        else
                        {
                            fatigue = (damage * ((100.0 / m.Hits) + ((50.0 + m.Stam) / 100) - 1.0)) - 5.0;
                        }
                        break;
                    }
            }

            var reduction = BaseArmor.GetInherentStaminaLossReduction(m) + 1;

            if (reduction > 1)
            {
                fatigue = fatigue / reduction;
            }

            if (fatigue > 0)
            {
                if (m.Stam - fatigue <= 0)
                {
                    m.Stam = Utility.RandomMinMax(0, Math.Min(3, m.Stam));
                }
                else
                {
                    m.Stam -= (int)fatigue;
                }
            }
        }

        public static void EventSink_Movement(MovementEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!from.Alive || from.IsStaff())
                return;

            if (!from.Player)
            {
                return;
            }

            int maxWeight = from.MaxWeight + OverloadAllowance;
            int overWeight = (Mobile.BodyWeight + from.TotalWeight) - maxWeight;

            if (overWeight > 0)
            {
                from.Stam -= GetStamLoss(from, overWeight, (e.Direction & Direction.Running) != 0);

                if (from.Stam == 0)
                {
                    from.SendLocalizedMessage(500109); // You are too fatigued to move, because you are carrying too much weight!
                    e.Blocked = true;
                    return;
                }
            }

            if (!Core.SA && ((from.Stam * 100) / Math.Max(from.StamMax, 1)) < 10)
            {
                --from.Stam;
            }

            if (from.Stam == 0)
            {
                from.SendLocalizedMessage(from.Mounted ? 500108 : 500110); // Your mount is too fatigued to move. : You are too fatigued to move.
                e.Blocked = true;
                return;
            }

            var pm = from as PlayerMobile;

            if (pm != null)
            {
                int amt = Core.SA ? 10 : (from.Mounted ? 48 : 16);

                if ((++pm.StepsTaken % amt) == 0)
                    --from.Stam;
            }
        }

        public static int GetStamLoss(Mobile from, int overWeight, bool running)
        {
            int loss = 5 + (overWeight / 25);

            if (from.Mounted)
                loss /= 3;

            if (running)
                loss *= 2;

            return loss;
        }

        public static bool IsOverloaded(Mobile m)
        {
            if (!m.Player || !m.Alive || m.IsStaff())
                return false;

            return ((Mobile.BodyWeight + m.TotalWeight) > (m.MaxWeight + OverloadAllowance));
        }
    }
}