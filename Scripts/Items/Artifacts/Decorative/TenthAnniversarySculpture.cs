using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
	[Flipable( 0x3BB3, 0x3BB4 )]
	public class TenthAnniversarySculpture : Item
	{
        public override bool IsArtifact { get { return true; } }

        private static Dictionary<Mobile, DateTime> m_LuckTable = new Dictionary<Mobile, DateTime>();
        private Dictionary<Mobile, DateTime> m_RewardCooldown;
        public Dictionary<Mobile, DateTime> RewardCooldown { get { return m_RewardCooldown; } }
        private static List<TenthAnniversarySculpture> m_sculptures = new List<TenthAnniversarySculpture>();

        private static Timer m_Timer;

        private static readonly int LuckBonus = 200;

        [Constructable]
		public TenthAnniversarySculpture() : base( 15283 )
		{
			Name = "10th Anniversary Sculpture";
			Weight = 1.0;
            m_RewardCooldown = new Dictionary<Mobile, DateTime>();
            AddSculpture(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack) && from is PlayerMobile)
                from.SendLocalizedMessage(1042001);

            DefragTables();

            if (!IsCoolingDown(from))
            {            
                m_LuckTable[from] = DateTime.UtcNow + TimeSpan.FromMinutes(60);
                from.SendMessage("You Feel Your luck changing");
                m_RewardCooldown[from] = DateTime.UtcNow + TimeSpan.FromHours(24);
            }
        }

        public bool IsCoolingDown(Mobile from)
        {
            foreach (TenthAnniversarySculpture sculpture in m_sculptures)
            {
                if (sculpture.RewardCooldown != null && sculpture.RewardCooldown.ContainsKey(from))
                    return true;
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
                return LuckBonus;

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

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(DefragTables));
            m_Timer.Start();
        }

        public TenthAnniversarySculpture(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
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