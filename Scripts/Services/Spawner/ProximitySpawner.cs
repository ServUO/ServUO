using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class ProximitySpawner : Spawner
    {
        private int m_TriggerRange;
        private TextDefinition m_SpawnMessage;
        private bool m_InstantFlag;
        [Constructable]
        public ProximitySpawner()
        {
        }

        [Constructable]
        public ProximitySpawner(string spawnName)
            : base(spawnName)
        {
        }

        [Constructable]
        public ProximitySpawner(int amount, int minDelay, int maxDelay, int team, int homeRange, string spawnName)
            : base(amount, minDelay, maxDelay, team, homeRange, spawnName)
        {
        }

        [Constructable]
        public ProximitySpawner(int amount, int minDelay, int maxDelay, int team, int homeRange, string spawnName, int triggerRange, string spawnMessage, bool instantFlag)
            : base(amount, minDelay, maxDelay, team, homeRange, spawnName)
        {
            m_TriggerRange = triggerRange;
            m_SpawnMessage = TextDefinition.Parse(spawnMessage);
            m_InstantFlag = instantFlag;
        }

        public ProximitySpawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, List<string> spawnNames)
            : base(amount, minDelay, maxDelay, team, homeRange, spawnNames)
        {
        }

        public ProximitySpawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, List<string> spawnNames, int triggerRange, TextDefinition spawnMessage, bool instantFlag)
            : base(amount, minDelay, maxDelay, team, homeRange, spawnNames)
        {
            m_TriggerRange = triggerRange;
            m_SpawnMessage = spawnMessage;
            m_InstantFlag = instantFlag;
        }

        public ProximitySpawner(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public int TriggerRange
        {
            get
            {
                return m_TriggerRange;
            }
            set
            {
                m_TriggerRange = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TextDefinition SpawnMessage
        {
            get
            {
                return m_SpawnMessage;
            }
            set
            {
                m_SpawnMessage = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool InstantFlag
        {
            get
            {
                return m_InstantFlag;
            }
            set
            {
                m_InstantFlag = value;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "Proximity Spawner";
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void DoTimer(TimeSpan delay)
        {
            if (!Running)
                return;

            End = DateTime.UtcNow + delay;
        }

        public override void Respawn()
        {
            RemoveSpawned();

            End = DateTime.UtcNow;
        }

        public override void Spawn()
        {
            for (int i = 0; i < SpawnObjectCount; ++i)
            {
                for (int j = 0; j < MaxCount; ++j)
                    Spawn(SpawnObjects[i]);
            }
        }

        public override bool CheckSpawnerFull()
        {
            return false;
        }

        public virtual bool ValidTrigger(Mobile m)
        {
            if (m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                if (!bc.Controlled && !bc.Summoned)
                    return false;
            }
            else if (!m.Player)
            {
                return false;
            }

            return (m.Alive && !m.IsDeadBondedPet && m.CanBeDamaged() && !m.Hidden);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!Running)
                return;

            if (IsEmpty && End <= DateTime.UtcNow && m.InRange(GetWorldLocation(), m_TriggerRange) && m.Location != oldLocation && ValidTrigger(m))
            {
                TextDefinition.SendMessageTo(m, m_SpawnMessage);

                DoTimer();
                Spawn();

                if (m_InstantFlag)
                {
                    foreach (ISpawnable spawned in GetSpawn())
                    {
                        if (spawned is Mobile)
                            ((Mobile)spawned).Combatant = m;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_TriggerRange);
            TextDefinition.Serialize(writer, m_SpawnMessage);
            writer.Write(m_InstantFlag);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_TriggerRange = reader.ReadInt();
            m_SpawnMessage = TextDefinition.Deserialize(reader);
            m_InstantFlag = reader.ReadBool();
        }
    }
}