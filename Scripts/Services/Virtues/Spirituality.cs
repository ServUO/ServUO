#region References
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
#endregion

namespace Server.Services.Virtues
{
    public class SpiritualityVirtue
    {
        public static Dictionary<Mobile, SpiritualityContext> ActiveTable { get; set; }

        public static void Initialize()
        {
            ActiveTable = new Dictionary<Mobile, SpiritualityContext>();

            VirtueGump.Register(111, OnVirtueUsed);
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (!from.Alive)
                return;

            if (VirtueHelper.GetLevel(from, VirtueName.Spirituality) < VirtueLevel.Seeker)
                from.SendLocalizedMessage(1155829); // You must be a Seeker of Spirituality to invoke this Virtue.
            else
            {
                from.SendLocalizedMessage(1155827); // Target whom you wish to embrace with your Spirituality

                from.BeginTarget(
                    10,
                    false,
                    TargetFlags.None,
                    (mobile, targeted) =>
                    {
                        if (targeted is Mobile)
                        {
                            Mobile m = (Mobile)targeted;

                            if (VirtueHelper.GetLevel(from, VirtueName.Spirituality) < VirtueLevel.Seeker)
                            {
                                from.SendLocalizedMessage(1155812); // You must be at least a Seeker of Humility to Invoke this ability.
                            }
                            else if (!m.Alive)
                            {
                                from.SendLocalizedMessage(1155828); // Thy target must be among the living.
                            }
                            else if (m is BaseCreature && !((BaseCreature)m).Controlled && !((BaseCreature)m).Summoned)
                            {
                                from.SendLocalizedMessage(1155837); // You can only embrace players and pets with Spirituality.
                            }
                            else if (IsEmbracee(m))
                            {
                                from.SendLocalizedMessage(1155836); // They are already embraced by Spirituality.
                            }
                            else if (m.MeleeDamageAbsorb > 0)
                            {
                                from.SendLocalizedMessage(
                                    1156039); // You may not use the Spirituality Virtue while the Attunement spell is active.
                            }
                            else if (m is BaseCreature || m is PlayerMobile)
                            {
                                SpiritualityContext context = new SpiritualityContext(from, m);

                                ActiveTable[from] = context;

                                m.SendLocalizedMessage(1155839); // Your spirit has been embraced! You feel more powerful!
                                from.SendLocalizedMessage(1155835); // You have lost some Spirituality.

                                BuffInfo.AddBuff(
                                    m,
                                    new BuffInfo(
                                        BuffIcon.Spirituality,
                                        1155824,
                                        1155825,
                                        string.Format(
                                            "{0}\t{1}",
                                            context.Reduction.ToString(),
                                            context.Pool.ToString()))); // ~1_VAL~% Reduction to Incoming Damage<br>~2_VAL~ Shield HP Remaining

                                VirtueHelper.Atrophy(from, VirtueName.Spirituality, 3200);

                                Timer.DelayCall(
                                    TimeSpan.FromMinutes(20),
                                    () =>
                                    {
                                        if (ActiveTable != null && ActiveTable.ContainsKey(from))
                                        {
                                            ActiveTable.Remove(from);

                                            m.SendLocalizedMessage(1155840); // Your spirit is no longer embraced. You feel less powerful.

                                            BuffInfo.RemoveBuff(m, BuffIcon.Spirituality);
                                        }
                                    });
                            }
                        }
                        else
                            from.SendLocalizedMessage(1155837); // You can only embrace players and pets with Spirituality.
                    });
            }
        }

        public static bool IsEmbracee(Mobile m)
        {
            if (ActiveTable == null)
                return false;

            return GetContext(m) != null;
        }

        public static bool IsEmbracer(Mobile m)
        {
            if (ActiveTable == null)
                return false;

            return ActiveTable.ContainsKey(m);
        }

        public static SpiritualityContext GetContext(Mobile m)
        {
            if (ActiveTable == null)
                return null;

            foreach (SpiritualityContext context in ActiveTable.Values)
            {
                if (context.Mobile == m)
                    return context;
            }

            return null;
        }

        public static void GetDamageReduction(Mobile victim, ref int damage)
        {
            if (ActiveTable == null)
                return;

            SpiritualityContext context = GetContext(victim);

            if (context != null)
            {
                double reduction = context.Reduction / 100.0;

                damage = (int)(damage - (damage * reduction));
                context.Pool -= damage;

                victim.FixedEffect(0x373A, 10, 16);

                BuffInfo.RemoveBuff(victim, BuffIcon.Spirituality);

                if (context.Pool <= 0)
                {
                    victim.SendLocalizedMessage(1155840); // Your spirit is no longer embraced. You feel less powerful.

                    if (ActiveTable.ContainsKey(victim) && ActiveTable[victim] == context)
                    {
                        ActiveTable.Remove(victim);
                    }
                }
                else
                    BuffInfo.AddBuff(
                        victim,
                        new BuffInfo(
                            BuffIcon.Spirituality,
                            1155824,
                            1155825,
                            string.Format(
                                "{0}\t{1}",
                                context.Reduction,
                                context.Pool))); // ~1_VAL~% Reduction to Incoming Damage<br>~2_VAL~ Shield HP Remaining
            }
        }

        public static void OnHeal(Mobile mobile, int amount)
        {
            int points = Math.Min(50, amount);
            bool gainedPath = false;

            if (VirtueHelper.Award(mobile, VirtueName.Spirituality, points, ref gainedPath))
            {
                if (gainedPath)
                    mobile.SendLocalizedMessage(1155833); // You have gained a path in Spirituality!
                else
                    mobile.SendLocalizedMessage(1155832); // You have gained in Spirituality.
            }
            else
                mobile.SendLocalizedMessage(1155831); // You cannot gain more Spirituality.
        }

        public class SpiritualityContext
        {
            public Mobile Mobile { get; set; }
            public Mobile Protector { get; set; }
            public int Pool { get; set; }
            public int Reduction { get; set; }

            public SpiritualityContext(Mobile protector, Mobile m)
            {
                Protector = protector;
                Mobile = m;
                Pool = GetPool(protector);
                Reduction = GetReduction(protector);
            }

            private static int GetPool(Mobile user)
            {
                if (VirtueHelper.IsKnight(user, VirtueName.Spirituality))
                    return 200;

                if (VirtueHelper.IsFollower(user, VirtueName.Spirituality))
                    return 100;

                if (VirtueHelper.IsSeeker(user, VirtueName.Spirituality))
                    return 50;

                return 0;
            }

            public static int GetReduction(Mobile m)
            {
                if (VirtueHelper.IsKnight(m, VirtueName.Spirituality))
                    return 20;

                if (VirtueHelper.IsFollower(m, VirtueName.Spirituality))
                    return 10;

                if (VirtueHelper.IsSeeker(m, VirtueName.Spirituality))
                    return 5;

                return 0;
            }
        }
    }
}