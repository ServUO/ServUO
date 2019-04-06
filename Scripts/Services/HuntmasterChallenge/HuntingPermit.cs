using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;
using Server.Engines.HuntsmasterChallenge;

namespace Server.Items
{
    public class HuntingPermit : Item
    {
        private static List<HuntingPermit> m_Permits = new List<HuntingPermit>();
        public static List<HuntingPermit> Permits { get { return m_Permits; } }

        private Mobile m_Owner;
        private bool m_ProducedTrophy;
        private bool m_HasSubmitted;
        private HuntingKillEntry m_KillEntry;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ProducedTrophy { get { return m_ProducedTrophy; } set { m_ProducedTrophy = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public HuntingKillEntry KillEntry { get { return m_KillEntry; } set { m_KillEntry = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasDocumentedKill { get { return m_KillEntry != null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanUseTaxidermyOn { get { return HasDocumentedKill && m_KillEntry.KillIndex < HuntingTrophyInfo.Infos.Count && !m_ProducedTrophy; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasSubmitted 
        { 
            get { return m_HasSubmitted; }
            set 
            {
                m_HasSubmitted = value; 

                if(m_HasSubmitted && m_Permits.Contains(this)) 
                    m_Permits.Remove(this);
            } 
        }

        public override int LabelNumber { get { return 1155704; } } // Hunting Permit

        [Constructable]
        public HuntingPermit(Mobile from)
            : base(5360)
        {
            m_Owner = from;
            m_Permits.Add(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Backpack != null && IsChildOf(from.Backpack))
            {
                if (!HasDocumentedKill)
                {
                    from.Target = new InternalTarget(this);
                    MessageHelper.SendLocalizedMessageTo(this, from, 1155705, 0x45); // Target the kill you wish to document
                }
                else
                    from.SendLocalizedMessage(1155712); // This hunting permit has already documented a kill.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_KillEntry == null || m_KillEntry.KillIndex < 0 || m_KillEntry.KillIndex >= HuntingTrophyInfo.Infos.Count)
                return;

            HuntingTrophyInfo info = HuntingTrophyInfo.Infos[m_KillEntry.KillIndex];

            if (m_Owner != null)
                list.Add(1155708, m_Owner.Name); // Hunter: ~1_NAME~

            if (m_KillEntry.DateKilled != DateTime.MinValue)
                list.Add(1155709, m_KillEntry.DateKilled.ToShortDateString()); // Date of Kill: ~1_DATE~

            if (m_KillEntry.Location != null)
                list.Add(1061114, m_KillEntry.Location); // Location: ~1_val~

            list.Add(1155718, info.Species.ToString());

            if (info.MeasuredBy == MeasuredBy.Length)
                list.Add(1155711, m_KillEntry.Measurement.ToString()); // Length: ~1_VAL~
            else if (info.MeasuredBy == MeasuredBy.Wingspan)
                list.Add(1155710, m_KillEntry.Measurement.ToString());	// Wingspan: ~1_VAL~
            else
                list.Add(1072789, m_KillEntry.Measurement.ToString()); // Weight: ~1_WEIGHT~
        }

        private class InternalTarget : Target
        {
            private HuntingPermit m_Permit;

            public InternalTarget(HuntingPermit Permit)
                : base(-1, false, TargetFlags.None)
            {
                m_Permit = Permit;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Corpse)
                {
                    Corpse c = targeted as Corpse;

                    if (!from.InRange(c.Location, 3))
                        from.SendLocalizedMessage(500446); // That is too far away.
                    if (c.VisitedByTaxidermist)
                        from.SendLocalizedMessage(1042596); // That corpse seems to have been visited by a taxidermist already.
                    else if (!m_Permit.IsChildOf(from.Backpack))
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    else if (c.Owner == null)
                        from.SendLocalizedMessage(1155706); // That is not a valid kill.
                    else if (!IsOnlyAttacker(from, c.Owner))
                        from.SendLocalizedMessage(1155707);	  // You cannot document someone else's kill.
                    else
                    {
                        Type t = c.Owner.GetType();

                        if (t == typeof(RagingGrizzlyBear)) // Bandaid Fix, we'll keep this until others arise
                            t = typeof(GrizzlyBear);

                        for (int i = 0; i < HuntingTrophyInfo.Infos.Count; i++)
                        {
                            HuntingTrophyInfo info = HuntingTrophyInfo.Infos[i];

                            if (t == info.CreatureType)
                            {
                                int v = 0;

                                if (HuntingSystem.Instance != null && HuntingSystem.Instance.IsPrimeHunt(from, c.Location))
                                {
                                    v = Utility.RandomMinMax(0, 100);
                                }
                                else
                                {
                                    v = Utility.RandomMinMax(0, 10000);
                                    v = (int)Math.Sqrt(v);
                                    v = 100 - v;
                                }

                                int measurement = info.MinMeasurement + (int)((double)(info.MaxMeasurement - info.MinMeasurement) * (double)((double)v / 100.0));
                                m_Permit.KillEntry = new HuntingKillEntry(m_Permit.Owner, measurement, DateTime.Now, i, WorldLocationInfo.GetLocationString(c.Location, c.Map));
                                c.VisitedByTaxidermist = true;

                                from.PlaySound(0x249);
                                from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x45, 1155713, from.NetState); // *You document your kill on the permit*
                                m_Permit.InvalidateProperties();
                                return;
                            }
                        }

                        from.SendLocalizedMessage(1155706); // That is not a valid kill.
                    }
                }
            }

            private bool IsOnlyAttacker(Mobile from, Mobile creature)
            {
                BaseCreature bc = creature as BaseCreature;

                if (bc != null)
                {
                    List<DamageStore> rights = bc.GetLootingRights();

                    return rights.Count > 0 && rights[0].m_Mobile == from && rights[0].m_Damage >= bc.HitsMax;
                }

                return false;
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Permits.Contains(this))
                m_Permits.Remove(this);
        }

        public static bool HasPermit(Mobile from)
        {
            foreach (HuntingPermit permit in m_Permits)
            {
                if (permit.Owner == from)
                    return true;
            }

            return false;
        }

        public HuntingPermit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Owner);
            writer.Write(m_ProducedTrophy);
            writer.Write(m_HasSubmitted);

            if (m_KillEntry != null)
            {
                writer.Write((int)1);
                m_KillEntry.Serialize(writer);
            }
            else
                writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_ProducedTrophy = reader.ReadBool();
            m_HasSubmitted = reader.ReadBool();

            if (reader.ReadInt() == 1)
                m_KillEntry = new HuntingKillEntry(reader);

            if (m_Owner != null && !m_HasSubmitted)
                m_Permits.Add(this);
        }
    }
}