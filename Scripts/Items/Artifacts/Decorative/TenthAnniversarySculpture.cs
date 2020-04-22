using Server.Accounting;
using Server.Engines.VeteranRewards;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x3BB3, 0x3BB4)]
    public class TenthAnniversarySculpture : Item
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1079532;  // 10th Anniversary Sculpture

        private static readonly Dictionary<Mobile, DateTime> m_LuckTable = new Dictionary<Mobile, DateTime>();
        private Dictionary<Mobile, DateTime> m_RewardCooldown;
        public Dictionary<Mobile, DateTime> RewardCooldown => m_RewardCooldown;
        private static readonly List<TenthAnniversarySculpture> m_sculptures = new List<TenthAnniversarySculpture>();

        private static Timer m_Timer;

        private static readonly int MaxLuckBonus = 1000;

        [Constructable]
        public TenthAnniversarySculpture() : base(15283)
        {
            Weight = 1.0;
            m_RewardCooldown = new Dictionary<Mobile, DateTime>();
            AddSculpture(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            DefragTables();

            if (!IsCoolingDown(from))
            {
                m_LuckTable[from] = DateTime.UtcNow + TimeSpan.FromMinutes(60);
                from.SendLocalizedMessage(1079551); // Your luck just improved!
                m_RewardCooldown[from] = DateTime.UtcNow + TimeSpan.FromHours(24);

                from.Delta(MobileDelta.Armor);
            }
        }

        public bool IsCoolingDown(Mobile from)
        {
            bool donemessage = false;

            if (m_LuckTable.ContainsKey(from))
            {
                from.SendLocalizedMessage(1079534); // You're still feeling lucky from the last time you touched the sculpture.
                donemessage = true;
            }

            foreach (TenthAnniversarySculpture sculpture in m_sculptures)
            {
                if (sculpture.RewardCooldown != null && sculpture.RewardCooldown.ContainsKey(from))
                {
                    if (!donemessage)
                    {
                        TimeSpan left = sculpture.RewardCooldown[from] - DateTime.UtcNow;

                        if (left.TotalHours > 1)
                            from.SendLocalizedMessage(1079550, ((int)left.TotalHours).ToString()); // You can improve your fortunes again in about ~1_TIME~ hours.
                        else if (left.TotalMinutes > 1)
                            from.SendLocalizedMessage(1079548, ((int)left.TotalMinutes).ToString()); // You can improve your fortunes in about ~1_TIME~ minutes.
                        else
                            from.SendLocalizedMessage(1079547); // Your fortunes are about to improve.
                    }

                    return true;
                }
            }

            return false;
        }

        public static void DefragTables()
        {
            foreach (TenthAnniversarySculpture sculpture in m_sculptures)
            {
                List<Mobile> list = new List<Mobile>(sculpture.RewardCooldown.Keys);

                foreach (Mobile m in list)
                {
                    if (sculpture.RewardCooldown.ContainsKey(m) && sculpture.RewardCooldown[m] < DateTime.UtcNow)
                        sculpture.RewardCooldown.Remove(m);
                }

                list.Clear();
            }

            List<Mobile> remove = new List<Mobile>();
            foreach (KeyValuePair<Mobile, DateTime> kvp in m_LuckTable)
            {
                if (kvp.Value < DateTime.UtcNow)
                    remove.Add(kvp.Key);
            }

            remove.ForEach(m =>
            {
                m_LuckTable.Remove(m);

                if (m.NetState != null)
                    m.SendLocalizedMessage(1079552); //Your luck just ran out.
            });

            remove.Clear();
        }

        public static int GetLuckBonus(Mobile from)
        {
            if (m_LuckTable.ContainsKey(from))
            {
                Account account = from.Account as Account;

                if (account != null)
                {
                    return Math.Min(MaxLuckBonus, 200 + (RewardSystem.GetRewardLevel(account)) * 50);
                }
            }

            return 0;
        }

        public override void Delete()
        {
            base.Delete();

            RemoveSculpture(this);

            if (m_RewardCooldown != null)
                m_RewardCooldown.Clear();
        }

        public static void AddSculpture(TenthAnniversarySculpture sculpture)
        {
            if (!m_sculptures.Contains(sculpture))
            {
                m_sculptures.Add(sculpture);
                StartTimer();
            }
        }

        public static void RemoveSculpture(TenthAnniversarySculpture sculpture)
        {
            if (m_sculptures.Contains(sculpture))
                m_sculptures.Remove(sculpture);

            if (m_sculptures.Count == 0 && m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public static void StartTimer()
        {
            if (m_Timer != null && m_Timer.Running)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), DefragTables);
            m_Timer.Start();
        }

        public TenthAnniversarySculpture(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_RewardCooldown = new Dictionary<Mobile, DateTime>();

            AddSculpture(this);

        }
    }
}
