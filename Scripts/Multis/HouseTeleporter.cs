using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
    public class HouseTeleporter : Item, ISecurable
    {
        private Item m_Target;
        private SecureLevel m_Level;
        [Constructable]
        public HouseTeleporter(int itemID)
            : this(itemID, null)
        {
        }

        public HouseTeleporter(int itemID, Item target)
            : base(itemID)
        {
            this.Movable = false;

            this.m_Level = SecureLevel.Anyone;

            this.m_Target = target;
        }

        public HouseTeleporter(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item Target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
            }
        }
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
        public bool CheckAccess(Mobile m)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
                return false;

            return (house != null && house.HasSecureAccess(m, this.m_Level));
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (this.m_Target != null && !this.m_Target.Deleted)
            {
                if (this.CheckAccess(m))
                {
                    if (!m.Hidden || m.IsPlayer())
                        new EffectTimer(this.Location, this.Map, 2023, 0x1F0, TimeSpan.FromSeconds(0.4)).Start();

                    new DelayTimer(this, m).Start();
                }
                else
                {
                    m.SendLocalizedMessage(1061637); // You are not allowed to access this.
                }
            }

            return true;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Level);

            writer.Write((Item)this.m_Target);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Target = reader.ReadItem();

                        if (version < 0)
                            this.m_Level = SecureLevel.Anyone;

                        break;
                    }
            }
        }

        private class EffectTimer : Timer
        {
            private readonly Point3D m_Location;
            private readonly Map m_Map;
            private readonly int m_EffectID;
            private readonly int m_SoundID;
            public EffectTimer(Point3D p, Map map, int effectID, int soundID, TimeSpan delay)
                : base(delay)
            {
                this.m_Location = p;
                this.m_Map = map;
                this.m_EffectID = effectID;
                this.m_SoundID = soundID;
            }

            protected override void OnTick()
            {
                Effects.SendLocationParticles(EffectItem.Create(this.m_Location, this.m_Map, EffectItem.DefaultDuration), 0x3728, 10, 10, this.m_EffectID, 0);

                if (this.m_SoundID != -1)
                    Effects.PlaySound(this.m_Location, this.m_Map, this.m_SoundID);
            }
        }

        private class DelayTimer : Timer
        {
            private readonly HouseTeleporter m_Teleporter;
            private readonly Mobile m_Mobile;
            public DelayTimer(HouseTeleporter tp, Mobile m)
                : base(TimeSpan.FromSeconds(1.0))
            {
                this.m_Teleporter = tp;
                this.m_Mobile = m;
            }

            protected override void OnTick()
            {
                Item target = this.m_Teleporter.m_Target;

                if (target != null && !target.Deleted)
                {
                    Mobile m = this.m_Mobile;

                    if (m.X == this.m_Teleporter.X && m.Y == this.m_Teleporter.Y && Math.Abs(m.Z - this.m_Teleporter.Z) <= 1 && m.Map == this.m_Teleporter.Map)
                    {
                        Point3D p = target.GetWorldTop();
                        Map map = target.Map;

                        Server.Mobiles.BaseCreature.TeleportPets(m, p, map);

                        m.MoveToWorld(p, map);

                        if (!m.Hidden || m.IsPlayer())
                        {
                            Effects.PlaySound(target.Location, target.Map, 0x1FE);

                            Effects.SendLocationParticles(EffectItem.Create(this.m_Teleporter.Location, this.m_Teleporter.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023, 0);
                            Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023, 0);

                            new EffectTimer(target.Location, target.Map, 2023, -1, TimeSpan.FromSeconds(0.4)).Start();
                        }
                    }
                }
            }
        }
    }
}