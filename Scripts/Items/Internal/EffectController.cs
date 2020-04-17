using System;

namespace Server.Items
{
    public enum ECEffectType
    {
        None,
        Moving,
        Location,
        Target,
        Lightning
    }

    public enum EffectTriggerType
    {
        None,
        Sequenced,
        DoubleClick,
        InRange
    }

    public class EffectController : Item
    {
        private TimeSpan m_EffectDelay;
        private ECEffectType m_EffectType;
        private EffectTriggerType m_TriggerType;
        private IEntity m_Source;
        private IEntity m_Target;
        private TimeSpan m_TriggerDelay;
        private EffectController m_Trigger;
        private int m_ItemID;
        private int m_Hue;
        private int m_RenderMode;
        private int m_Speed;
        private int m_Duration;
        private bool m_FixedDirection;
        private bool m_Explodes;
        private int m_ParticleEffect;
        private int m_ExplodeParticleEffect;
        private int m_ExplodeSound;
        private EffectLayer m_EffectLayer;
        private int m_Unknown;
        private TimeSpan m_SoundDelay;
        private int m_SoundID;
        private bool m_PlaySoundAtTrigger;
        private int m_TriggerRange;
        [Constructable]
        public EffectController()
            : base(0x1B72)
        {
            Movable = false;
            Visible = false;
            m_TriggerType = EffectTriggerType.Sequenced;
            m_EffectLayer = (EffectLayer)255;
        }

        public EffectController(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ECEffectType EffectType
        {
            get
            {
                return m_EffectType;
            }
            set
            {
                m_EffectType = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public EffectTriggerType TriggerType
        {
            get
            {
                return m_TriggerType;
            }
            set
            {
                m_TriggerType = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public EffectLayer EffectLayer
        {
            get
            {
                return m_EffectLayer;
            }
            set
            {
                m_EffectLayer = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan EffectDelay
        {
            get
            {
                return m_EffectDelay;
            }
            set
            {
                m_EffectDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan TriggerDelay
        {
            get
            {
                return m_TriggerDelay;
            }
            set
            {
                m_TriggerDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SoundDelay
        {
            get
            {
                return m_SoundDelay;
            }
            set
            {
                m_SoundDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item SourceItem
        {
            get
            {
                return m_Source as Item;
            }
            set
            {
                m_Source = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SourceMobile
        {
            get
            {
                return m_Source as Mobile;
            }
            set
            {
                m_Source = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceNull
        {
            get
            {
                return (m_Source == null);
            }
            set
            {
                if (value)
                    m_Source = null;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item TargetItem
        {
            get
            {
                return m_Target as Item;
            }
            set
            {
                m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile TargetMobile
        {
            get
            {
                return m_Target as Mobile;
            }
            set
            {
                m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TargetNull
        {
            get
            {
                return (m_Target == null);
            }
            set
            {
                if (value)
                    m_Target = null;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public EffectController Sequence
        {
            get
            {
                return m_Trigger;
            }
            set
            {
                m_Trigger = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectItemID
        {
            get
            {
                return m_ItemID;
            }
            set
            {
                m_ItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectHue
        {
            get
            {
                return m_Hue;
            }
            set
            {
                m_Hue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RenderMode
        {
            get
            {
                return m_RenderMode;
            }
            set
            {
                m_RenderMode = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        {
            get
            {
                return m_Speed;
            }
            set
            {
                m_Speed = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                m_Duration = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ParticleEffect
        {
            get
            {
                return m_ParticleEffect;
            }
            set
            {
                m_ParticleEffect = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeParticleEffect
        {
            get
            {
                return m_ExplodeParticleEffect;
            }
            set
            {
                m_ExplodeParticleEffect = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeSound
        {
            get
            {
                return m_ExplodeSound;
            }
            set
            {
                m_ExplodeSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Unknown
        {
            get
            {
                return m_Unknown;
            }
            set
            {
                m_Unknown = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get
            {
                return m_SoundID;
            }
            set
            {
                m_SoundID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
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
        public override string DefaultName => "Effect Controller";
        public override bool HandlesOnMovement => (m_TriggerType == EffectTriggerType.InRange);
        [CommandProperty(AccessLevel.GameMaster)]
        private bool FixedDirection
        {
            get
            {
                return m_FixedDirection;
            }
            set
            {
                m_FixedDirection = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        private bool Explodes
        {
            get
            {
                return m_Explodes;
            }
            set
            {
                m_Explodes = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        private bool PlaySoundAtTrigger
        {
            get
            {
                return m_PlaySoundAtTrigger;
            }
            set
            {
                m_PlaySoundAtTrigger = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (m_TriggerType == EffectTriggerType.DoubleClick)
                DoEffect(from);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Location != oldLocation && m_TriggerType == EffectTriggerType.InRange && Utility.InRange(GetWorldLocation(), m.Location, m_TriggerRange) && !Utility.InRange(GetWorldLocation(), oldLocation, m_TriggerRange))
                DoEffect(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_EffectDelay);
            writer.Write(m_TriggerDelay);
            writer.Write(m_SoundDelay);

            if (m_Source is Item)
                writer.Write(m_Source as Item);
            else
                writer.Write(m_Source as Mobile);

            if (m_Target is Item)
                writer.Write(m_Target as Item);
            else
                writer.Write(m_Target as Mobile);

            writer.Write(m_Trigger as Item);

            writer.Write(m_FixedDirection);
            writer.Write(m_Explodes);
            writer.Write(m_PlaySoundAtTrigger);

            writer.WriteEncodedInt((int)m_EffectType);
            writer.WriteEncodedInt((int)m_EffectLayer);
            writer.WriteEncodedInt((int)m_TriggerType);

            writer.WriteEncodedInt(m_ItemID);
            writer.WriteEncodedInt(m_Hue);
            writer.WriteEncodedInt(m_RenderMode);
            writer.WriteEncodedInt(m_Speed);
            writer.WriteEncodedInt(m_Duration);
            writer.WriteEncodedInt(m_ParticleEffect);
            writer.WriteEncodedInt(m_ExplodeParticleEffect);
            writer.WriteEncodedInt(m_ExplodeSound);
            writer.WriteEncodedInt(m_Unknown);
            writer.WriteEncodedInt(m_SoundID);
            writer.WriteEncodedInt(m_TriggerRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_EffectDelay = reader.ReadTimeSpan();
                        m_TriggerDelay = reader.ReadTimeSpan();
                        m_SoundDelay = reader.ReadTimeSpan();

                        m_Source = ReadEntity(reader);
                        m_Target = ReadEntity(reader);
                        m_Trigger = reader.ReadItem() as EffectController;

                        m_FixedDirection = reader.ReadBool();
                        m_Explodes = reader.ReadBool();
                        m_PlaySoundAtTrigger = reader.ReadBool();

                        m_EffectType = (ECEffectType)reader.ReadEncodedInt();
                        m_EffectLayer = (EffectLayer)reader.ReadEncodedInt();
                        m_TriggerType = (EffectTriggerType)reader.ReadEncodedInt();

                        m_ItemID = reader.ReadEncodedInt();
                        m_Hue = reader.ReadEncodedInt();
                        m_RenderMode = reader.ReadEncodedInt();
                        m_Speed = reader.ReadEncodedInt();
                        m_Duration = reader.ReadEncodedInt();
                        m_ParticleEffect = reader.ReadEncodedInt();
                        m_ExplodeParticleEffect = reader.ReadEncodedInt();
                        m_ExplodeSound = reader.ReadEncodedInt();
                        m_Unknown = reader.ReadEncodedInt();
                        m_SoundID = reader.ReadEncodedInt();
                        m_TriggerRange = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public void PlaySound(object trigger)
        {
            IEntity ent = null;

            if (m_PlaySoundAtTrigger)
                ent = trigger as IEntity;

            if (ent == null)
                ent = this;

            Effects.PlaySound((ent is Item) ? ((Item)ent).GetWorldLocation() : ent.Location, ent.Map, m_SoundID);
        }

        public void DoEffect(object trigger)
        {
            if (Deleted || m_TriggerType == EffectTriggerType.None)
                return;

            if (trigger is Mobile && ((Mobile)trigger).Hidden && ((Mobile)trigger).IsStaff())
                return;

            if (m_SoundID > 0)
                Timer.DelayCall(m_SoundDelay, new TimerStateCallback(PlaySound), trigger);

            if (m_Trigger != null)
                Timer.DelayCall(m_TriggerDelay, new TimerStateCallback(m_Trigger.DoEffect), trigger);

            if (m_EffectType != ECEffectType.None)
                Timer.DelayCall(m_EffectDelay, new TimerStateCallback(InternalDoEffect), trigger);
        }

        public void InternalDoEffect(object trigger)
        {
            IEntity from = m_Source, to = m_Target;

            if (from == null)
                from = (IEntity)trigger;

            if (to == null)
                to = (IEntity)trigger;

            switch (m_EffectType)
            {
                case ECEffectType.Lightning:
                    {
                        Effects.SendBoltEffect(from, false, m_Hue);
                        break;
                    }
                case ECEffectType.Location:
                    {
                        Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), m_ItemID, m_Speed, m_Duration, m_Hue, m_RenderMode, m_ParticleEffect, m_Unknown);
                        break;
                    }
                case ECEffectType.Moving:
                    {
                        if (from == this)
                            from = EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration);

                        if (to == this)
                            to = EffectItem.Create(to.Location, to.Map, EffectItem.DefaultDuration);

                        Effects.SendMovingParticles(from, to, m_ItemID, m_Speed, m_Duration, m_FixedDirection, m_Explodes, m_Hue, m_RenderMode, m_ParticleEffect, m_ExplodeParticleEffect, m_ExplodeSound, m_EffectLayer, m_Unknown);
                        break;
                    }
                case ECEffectType.Target:
                    {
                        Effects.SendTargetParticles(from, m_ItemID, m_Speed, m_Duration, m_Hue, m_RenderMode, m_ParticleEffect, m_EffectLayer, m_Unknown);
                        break;
                    }
            }
        }

        private IEntity ReadEntity(GenericReader reader)
        {
            return World.FindEntity(reader.ReadInt());
        }
    }
}