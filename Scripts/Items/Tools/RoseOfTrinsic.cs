using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0x234C, 0x234D)]
    public class RoseOfTrinsic : Item, ISecurable
    {
        private static readonly TimeSpan m_SpawnTime = TimeSpan.FromHours(4.0);
        private int m_Petals;
        private DateTime m_NextSpawnTime;
        private SpawnTimer m_SpawnTimer;
        private SecureLevel m_Level;
        [Constructable]
        public RoseOfTrinsic()
            : base(0x234D)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;

            this.m_Petals = 0;
            this.StartSpawnTimer(TimeSpan.FromMinutes(1.0));
        }

        public RoseOfTrinsic(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062913;
            }
        }// Rose of Trinsic
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Petals
        {
            get
            {
                return this.m_Petals;
            }
            set
            {
                if (value >= 10)
                {
                    this.m_Petals = 10;

                    this.StopSpawnTimer();
                }
                else
                {
                    if (value <= 0)
                        this.m_Petals = 0;
                    else
                        this.m_Petals = value;

                    this.StartSpawnTimer(m_SpawnTime);
                }

                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1062925, this.Petals.ToString()); // Petals:  ~1_COUNT~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (this.Petals > 0)
            {
                from.AddToBackpack(new RoseOfTrinsicPetal(this.Petals));
                this.Petals = 0;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_Petals);
            writer.WriteDeltaTime((DateTime)this.m_NextSpawnTime);
            writer.WriteEncodedInt((int)this.m_Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Petals = reader.ReadEncodedInt();
            this.m_NextSpawnTime = reader.ReadDeltaTime();
            this.m_Level = (SecureLevel)reader.ReadEncodedInt();

            if (this.m_Petals < 10)
                this.StartSpawnTimer(this.m_NextSpawnTime - DateTime.UtcNow);
        }

        private void StartSpawnTimer(TimeSpan delay)
        {
            this.StopSpawnTimer();

            this.m_SpawnTimer = new SpawnTimer(this, delay);
            this.m_SpawnTimer.Start();

            this.m_NextSpawnTime = DateTime.UtcNow + delay;
        }

        private void StopSpawnTimer()
        {
            if (this.m_SpawnTimer != null)
            {
                this.m_SpawnTimer.Stop();
                this.m_SpawnTimer = null;
            }
        }

        private class SpawnTimer : Timer
        {
            private readonly RoseOfTrinsic m_Rose;
            public SpawnTimer(RoseOfTrinsic rose, TimeSpan delay)
                : base(delay)
            {
                this.m_Rose = rose;

                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (this.m_Rose.Deleted)
                    return;

                this.m_Rose.m_SpawnTimer = null;
                this.m_Rose.Petals++;
            }
        }
    }

    public class RoseOfTrinsicPetal : Item
    {
        [Constructable]
        public RoseOfTrinsicPetal()
            : this(1)
        {
        }

        [Constructable]
        public RoseOfTrinsicPetal(int amount)
            : base(0x1021)
        {
            this.Stackable = true;
            this.Amount = amount;

            this.Weight = 1.0;
            this.Hue = 0xE;
        }

        public RoseOfTrinsicPetal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062926;
            }
        }// Petal of the Rose of Trinsic
        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
            }
            else if (from.GetStatMod("RoseOfTrinsicPetal") != null)
            {
                from.SendLocalizedMessage(1062927); // You have eaten one of these recently and eating another would provide no benefit.
            }
            else
            {
                from.PlaySound(0x1EE);
                from.AddStatMod(new StatMod(StatType.Str, "RoseOfTrinsicPetal", 5, TimeSpan.FromMinutes(5.0)));

                this.Consume();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}