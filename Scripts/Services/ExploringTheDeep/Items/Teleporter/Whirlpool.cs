using System;
using Server.Mobiles;
using Server.Spells;
using System.Linq;
using Server.Engines.Quests;

namespace Server.Items
{
    public class Whirlpool : Item
    {
        public override int LabelNumber { get { return 1033459; } } // whirlpool

        private bool m_Active, m_Creatures, m_CombatCheck, m_CriminalCheck;
        private Point3D m_PointDest;
        private Map m_MapDest;
        private bool m_SourceEffect;
        private bool m_DestEffect;
        private int m_SoundID;
        private TimeSpan m_Delay;
        
        [Constructable]
        public Whirlpool(Point3D pointDest, Map mapDest)
            : this(pointDest, mapDest, false)
        {
        }

        [Constructable]
        public Whirlpool(Point3D pointDest, Map mapDest, bool creatures)
            : base(0x3789)
        {
            Movable = false;
            Hue = 2592;

            m_Active = true;
            m_PointDest = pointDest;
            m_MapDest = mapDest;
            m_Creatures = creatures;

            m_CombatCheck = false;
            m_CriminalCheck = false;
        }

        public Whirlpool(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceEffect
        {
            get { return m_SourceEffect; }
            set
            {
                m_SourceEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DestEffect
        {
            get { return m_DestEffect; }
            set
            {
                m_DestEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get { return m_SoundID; }
            set
            {
                m_SoundID = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay
        {
            get { return m_Delay; }
            set
            {
                m_Delay = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                m_Active = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get { return m_PointDest; }
            set
            {
                m_PointDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get { return m_MapDest; }
            set
            {
                m_MapDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Creatures
        {
            get { return m_Creatures; }
            set
            {
                m_Creatures = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CombatCheck
        {
            get { return m_CombatCheck; }
            set
            {
                m_CombatCheck = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CriminalCheck
        {
            get { return m_CriminalCheck; }
            set
            {
                m_CriminalCheck = value;
                InvalidateProperties();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Holding != null)
            {
                from.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return;
            }
            PlayerMobile mobile = from as PlayerMobile;

            if (mobile == null)
                return;

            if (mobile.IsStaff())
            {
                StartTeleport(mobile);
                return;
            }

            if (!mobile.Alive || !mobile.IsPlayer())
                return;

            else if (m_Active && CanTeleport(from))
            {
                int equipment = mobile.Items.Where(i => (i is CanvassRobe || i is BootsOfBallast || i is NictitatingLens || i is AquaPendant || i is GargishNictitatingLens) && (i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i)).Count();

                if (equipment < 4)
                {
                    mobile.Kill();
                    return;
                }
                else
                {
                    StartTeleport(from);
                    return;
                }
            }
        }

        public virtual bool CanTeleport(Mobile m)
        {
            if (!m_Creatures && !m.Player)
            {
                return false;
            }
            else if (m_CriminalCheck && m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (m_CombatCheck && SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }

            return true;
        }

        public virtual void StartTeleport(Mobile m)
        {
            if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return;
            }
            else if (m_Delay == TimeSpan.Zero)
            {
                DoTeleport(m);
            }
            else
            {
                Timer.DelayCall(m_Delay, DoTeleport, m);
            }
        }

        public virtual void DoTeleport(Mobile m)
        {
            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
            {
                map = m.Map;
            }

            Point3D p = m_PointDest;

            if (p == Point3D.Zero)
            {
                p = m.Location;
            }

            BaseCreature.TeleportPets(m, p, map);

            if (m is PlayerMobile)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
                    {
                        var spell = QuestHelper.GetQuest<ExploringTheDeepQuest>((PlayerMobile)m);

                        if (spell != null)
                        {
                            spell.CompleteQuest();
                        }
                    });
            }

            bool sendEffect = (!m.Hidden || m.IsPlayer());

            if (m_SourceEffect && sendEffect)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            }

            m.MoveToWorld(p, map);

            if (m_DestEffect && sendEffect)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            }

            if (m_SoundID > 0 && sendEffect)
            {
                Effects.PlaySound(m.Location, m.Map, m_SoundID);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write(m_CriminalCheck);
            writer.Write(m_CombatCheck);

            writer.Write(m_SourceEffect);
            writer.Write(m_DestEffect);
            writer.Write(m_Delay);
            writer.WriteEncodedInt(m_SoundID);

            writer.Write(m_Creatures);

            writer.Write(m_Active);
            writer.Write(m_PointDest);
            writer.Write(m_MapDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        m_CriminalCheck = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        m_CombatCheck = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        m_SourceEffect = reader.ReadBool();
                        m_DestEffect = reader.ReadBool();
                        m_Delay = reader.ReadTimeSpan();
                        m_SoundID = reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        m_Creatures = reader.ReadBool();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Active = reader.ReadBool();
                        m_PointDest = reader.ReadPoint3D();
                        m_MapDest = reader.ReadMap();

                        break;
                    }
            }
        }
    }
}
