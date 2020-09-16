using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class HouseTeleporter : Item, ISecurable
    {
        private Item m_Target;
        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseHouse House => BaseHouse.FindHouseAt(this);

        [Constructable]
        public HouseTeleporter(int itemID)
            : this(itemID, null)
        {
        }

        public HouseTeleporter(int itemID, Item target)
            : base(itemID)
        {
            Movable = false;

            m_Level = SecureLevel.Anyone;

            m_Target = target;
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
                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }
        public virtual bool CheckAccess(Mobile m)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsCombatRestricted(m))
            {
                m.SendLocalizedMessage(1071514); // You cannot use this item during the heat of battle.
                return false;
            }

            if (house != null && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
            {
                m.SendLocalizedMessage(1115577); // You cannot teleport from here to the destination because you do not have the correct house permissions. 
                return false;
            }

            if (house == null || !house.HasSecureAccess(m, m_Level))
            {
                m.SendLocalizedMessage(1115577); // You cannot teleport from here to the destination because you do not have the correct house permissions.
                return false;
            }

            return true;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile && ((PlayerMobile)m).DesignContext != null)
            {
                return true;
            }

            if (m_Target != null && !m_Target.Deleted)
            {
                if (CheckAccess(m))
                {
                    if (!m.Hidden || m.IsPlayer())
                        new EffectTimer(Location, Map, 2023, 0x1F0, TimeSpan.FromSeconds(0.4)).Start();

                    new DelayTimer(this, m).Start();
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

            writer.Write(1); // version

            writer.Write((int)m_Level);

            writer.Write(m_Target);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Target = reader.ReadItem();

                        if (version < 0)
                            m_Level = SecureLevel.Anyone;

                        break;
                    }
            }
        }

        public virtual void OnAfterTeleport(Mobile m)
        {
        }

        public class EffectTimer : Timer
        {
            private readonly Point3D m_Location;
            private readonly Map m_Map;
            private readonly int m_EffectID;
            private readonly int m_SoundID;
            public EffectTimer(Point3D p, Map map, int effectID, int soundID, TimeSpan delay)
                : base(delay)
            {
                m_Location = p;
                m_Map = map;
                m_EffectID = effectID;
                m_SoundID = soundID;
            }

            protected override void OnTick()
            {
                Effects.SendLocationParticles(EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration), 0x3728, 10, 10, m_EffectID, 0);

                if (m_SoundID != -1)
                    Effects.PlaySound(m_Location, m_Map, m_SoundID);
            }
        }

        private class DelayTimer : Timer
        {
            private readonly HouseTeleporter m_Teleporter;
            private readonly Mobile m_Mobile;
            public DelayTimer(HouseTeleporter tp, Mobile m)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_Teleporter = tp;
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                Item target = m_Teleporter.m_Target;

                if (target != null && !target.Deleted)
                {
                    Mobile m = m_Mobile;

                    if (m.X == m_Teleporter.X && m.Y == m_Teleporter.Y && Math.Abs(m.Z - m_Teleporter.Z) <= 1 && m.Map == m_Teleporter.Map)
                    {
                        Point3D p = target.GetWorldTop();
                        Map map = target.Map;

                        BaseCreature.TeleportPets(m, p, map);

                        m.MoveToWorld(p, map);

                        if (!m.Hidden || m.IsPlayer())
                        {
                            Effects.PlaySound(target.Location, target.Map, 0x1FE);

                            Effects.SendLocationParticles(EffectItem.Create(m_Teleporter.Location, m_Teleporter.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023, 0);
                            Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023, 0);

                            new EffectTimer(target.Location, target.Map, 2023, -1, TimeSpan.FromSeconds(0.4)).Start();
                        }

                        m_Teleporter.OnAfterTeleport(m);
                    }
                }
            }
        }
    }
}
