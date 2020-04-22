using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.InstancedPeerless
{
    public abstract class PeerlessPlatform : BaseAddon
    {
        public abstract Type KeyType { get; }
        public abstract Type BossType { get; }

        public virtual int OfferGumpTitle => 1113737; // Monster's Lair
        public virtual int OfferGumpDesc => 1113738; // Your party has gained entrance to a monster's lair. You may choose to join the fight or stay away.

        private readonly List<PeerlessKeyBrazier> m_Braziers = new List<PeerlessKeyBrazier>();
        private readonly List<PeerlessInstance> m_Instances = new List<PeerlessInstance>();
        private readonly Queue<List<Mobile>> m_PartyQueue = new Queue<List<Mobile>>();

        private Point3D m_ExitLocation;
        private Mobile m_Summoner;

        private Timer m_KeyExpireTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Summoner { get { return m_Summoner; } set { m_Summoner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ExitLocation { get { return m_ExitLocation; } set { m_ExitLocation = value; } }

        public PeerlessPlatform()
        {
            AddInstances();
            AddBraziers();
        }

        public abstract void AddInstances();
        public abstract void AddBraziers();

        public override bool ShareHue => false;

        public void Validate()
        {
            if (m_KeyExpireTimer != null)
                m_KeyExpireTimer.Stop();

            foreach (PeerlessKeyBrazier brazier in m_Braziers)
            {
                if (!Validate(brazier))
                {
                    m_KeyExpireTimer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), delegate { Clear(false); });
                    return;
                }
            }

            List<Mobile> party = GetParty(m_Summoner);
            List<Mobile> invalid;

            if (!ValidateMembers(party, out invalid))
            {
                m_Summoner.SendLocalizedMessage(1113574); // Your party cannot join the queue because the following members have already registered with another group:

                foreach (Mobile m in invalid)
                    m_Summoner.SendMessage(m.Name);

                Clear(true);
            }
            else
            {
                PeerlessInstance instance = AcquireEmptyInstance();

                if (instance == null)
                {
                    m_PartyQueue.Enqueue(party);

                    m_Summoner.SendLocalizedMessage(1113575); // Your party has been successfully added to the queue for this instance.
                }
                else
                {
                    instance.Activate();

                    new InstanceEnterGate(instance, party).MoveToWorld(new Point3D(X, Y, Z + 5), Map);
                }

                Clear(false);
            }
        }

        private void Clear(bool returnKeys)
        {
            foreach (PeerlessKeyBrazier brazier in m_Braziers)
            {
                Item key = brazier.Key;

                if (key == null)
                    continue;

                if (returnKeys)
                {
                    key.Movable = true;
                    m_Summoner.PlaceInBackpack(key);
                }
                else
                    key.Delete();

                brazier.Key = null;
            }

            if (returnKeys)
                m_Summoner.SendLocalizedMessage(1113576); // Your sacrificial keys have been returned to you.

            m_Summoner = null;
        }

        private bool Validate(PeerlessKeyBrazier brazier)
        {
            return brazier.Key != null && !brazier.Key.Deleted;
        }

        private bool ValidateMembers(List<Mobile> party, out List<Mobile> invalid)
        {
            invalid = new List<Mobile>();

            foreach (List<Mobile> otherParty in m_PartyQueue)
            {
                foreach (Mobile m in otherParty)
                {
                    if (party.Contains(m))
                        invalid.Add(m);
                }
            }

            return (invalid.Count == 0);
        }

        private List<Mobile> GetParty(Mobile from)
        {
            List<Mobile> list = new List<Mobile>();
            Party p = from.Party as Party;

            if (p != null)
            {
                foreach (PartyMemberInfo pmInfo in p.Members)
                    list.Add(pmInfo.Mobile);
            }
            else
            {
                list.Add(from);
            }

            return list;
        }

        private PeerlessInstance AcquireEmptyInstance()
        {
            foreach (PeerlessInstance instance in m_Instances)
            {
                if (instance.State == InstanceState.Available)
                    return instance;
            }

            return null;
        }

        protected void AddInstance(int x, int y, int z, Map map, Point3D entranceLoc, Point3D bossSpawnLoc, Rectangle2D regionBounds)
        {
            AddonComponent light = new AddonComponent(0x1ECF);

            m_Instances.Add(new PeerlessInstance(this, map, light, entranceLoc, bossSpawnLoc, regionBounds));

            AddComponent(light, x, y, z);
        }

        protected void AddBrazier(int x, int y, int z)
        {
            PeerlessKeyBrazier brazier = new PeerlessKeyBrazier(this);

            m_Braziers.Add(brazier);

            AddComponent(brazier, x, y, z);
        }

        public void OnFreeInstance(PeerlessInstance instance)
        {
            if (m_PartyQueue.Count > 0)
            {
                List<Mobile> party = m_PartyQueue.Dequeue();

                instance.Activate();

                foreach (Mobile member in party)
                {
                    if (member.Region.IsPartOf("Stygian Abyss"))
                    {
                        member.SendGump(new RejoinInstanceGump(instance, OfferGumpTitle, OfferGumpDesc));

                        Timer.DelayCall(TimeSpan.FromMinutes(1.0), delegate { member.CloseGump(typeof(RejoinInstanceGump)); });
                    }
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Clear(false);

            if (m_KeyExpireTimer != null)
                m_KeyExpireTimer.Stop();

            foreach (PeerlessInstance instance in m_Instances)
                instance.OnDelete();
        }

        public PeerlessPlatform(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_ExitLocation);

            writer.Write(m_Braziers.Count);

            for (int i = 0; i < m_Braziers.Count; i++)
                writer.WriteItem(m_Braziers[i]);

            writer.Write(m_Instances.Count);

            for (int i = 0; i < m_Instances.Count; i++)
                m_Instances[i].Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_ExitLocation = reader.ReadPoint3D();

                        int braziers = reader.ReadInt();

                        for (int i = 0; i < braziers; i++)
                            m_Braziers.Add(reader.ReadItem<PeerlessKeyBrazier>());

                        int instances = reader.ReadInt();

                        for (int i = 0; i < instances; i++)
                            m_Instances.Add(new PeerlessInstance(reader));

                        break;
                    }
            }
        }
    }
}
