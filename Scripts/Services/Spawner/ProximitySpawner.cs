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
            this.m_TriggerRange = triggerRange;
            this.m_SpawnMessage = TextDefinition.Parse(spawnMessage);
            this.m_InstantFlag = instantFlag;
        }

        public ProximitySpawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, List<string> spawnNames)
            : base(amount, minDelay, maxDelay, team, homeRange, spawnNames)
        {
        }

        public ProximitySpawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, List<string> spawnNames, int triggerRange, TextDefinition spawnMessage, bool instantFlag)
            : base(amount, minDelay, maxDelay, team, homeRange, spawnNames)
        {
            this.m_TriggerRange = triggerRange;
            this.m_SpawnMessage = spawnMessage;
            this.m_InstantFlag = instantFlag;
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
                return this.m_TriggerRange;
            }
            set
            {
                this.m_TriggerRange = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TextDefinition SpawnMessage
        {
            get
            {
                return this.m_SpawnMessage;
            }
            set
            {
                this.m_SpawnMessage = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool InstantFlag
        {
            get
            {
                return this.m_InstantFlag;
            }
            set
            {
                this.m_InstantFlag = value;
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
            if (!this.Running)
                return;

            this.End = DateTime.UtcNow + delay;
        }

        public override void Respawn()
        {
            this.RemoveSpawned();

            this.End = DateTime.UtcNow;
        }

        public override void Spawn()
        {
            for (int i = 0; i < this.SpawnNamesCount; ++i)
            {
                for (int j = 0; j < this.Count; ++j)
                    this.Spawn(i);
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
            if (!this.Running)
                return;

            if (this.IsEmpty && this.End <= DateTime.UtcNow && m.InRange(this.GetWorldLocation(), this.m_TriggerRange) && m.Location != oldLocation && this.ValidTrigger(m))
            {
                TextDefinition.SendMessageTo(m, this.m_SpawnMessage);

                this.DoTimer();
                this.Spawn();

                if (this.m_InstantFlag)
                {
                    foreach (ISpawnable spawned in this.Spawned)
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

            writer.Write(this.m_TriggerRange);
            TextDefinition.Serialize(writer, this.m_SpawnMessage);
            writer.Write(this.m_InstantFlag);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_TriggerRange = reader.ReadInt();
            this.m_SpawnMessage = TextDefinition.Deserialize(reader);
            this.m_InstantFlag = reader.ReadBool();
        }
    }
}