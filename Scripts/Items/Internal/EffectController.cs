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
            this.Movable = false;
            this.Visible = false;
            this.m_TriggerType = EffectTriggerType.Sequenced;
            this.m_EffectLayer = (EffectLayer)255;
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
                return this.m_EffectType;
            }
            set
            {
                this.m_EffectType = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public EffectTriggerType TriggerType
        {
            get
            {
                return this.m_TriggerType;
            }
            set
            {
                this.m_TriggerType = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public EffectLayer EffectLayer
        {
            get
            {
                return this.m_EffectLayer;
            }
            set
            {
                this.m_EffectLayer = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan EffectDelay
        {
            get
            {
                return this.m_EffectDelay;
            }
            set
            {
                this.m_EffectDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan TriggerDelay
        {
            get
            {
                return this.m_TriggerDelay;
            }
            set
            {
                this.m_TriggerDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SoundDelay
        {
            get
            {
                return this.m_SoundDelay;
            }
            set
            {
                this.m_SoundDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item SourceItem
        {
            get
            {
                return this.m_Source as Item;
            }
            set
            {
                this.m_Source = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SourceMobile
        {
            get
            {
                return this.m_Source as Mobile;
            }
            set
            {
                this.m_Source = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceNull
        {
            get
            {
                return (this.m_Source == null);
            }
            set
            {
                if (value)
                    this.m_Source = null;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item TargetItem
        {
            get
            {
                return this.m_Target as Item;
            }
            set
            {
                this.m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile TargetMobile
        {
            get
            {
                return this.m_Target as Mobile;
            }
            set
            {
                this.m_Target = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TargetNull
        {
            get
            {
                return (this.m_Target == null);
            }
            set
            {
                if (value)
                    this.m_Target = null;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public EffectController Sequence
        {
            get
            {
                return this.m_Trigger;
            }
            set
            {
                this.m_Trigger = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectItemID
        {
            get
            {
                return this.m_ItemID;
            }
            set
            {
                this.m_ItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectHue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RenderMode
        {
            get
            {
                return this.m_RenderMode;
            }
            set
            {
                this.m_RenderMode = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        {
            get
            {
                return this.m_Speed;
            }
            set
            {
                this.m_Speed = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ParticleEffect
        {
            get
            {
                return this.m_ParticleEffect;
            }
            set
            {
                this.m_ParticleEffect = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeParticleEffect
        {
            get
            {
                return this.m_ExplodeParticleEffect;
            }
            set
            {
                this.m_ExplodeParticleEffect = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplodeSound
        {
            get
            {
                return this.m_ExplodeSound;
            }
            set
            {
                this.m_ExplodeSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Unknown
        {
            get
            {
                return this.m_Unknown;
            }
            set
            {
                this.m_Unknown = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get
            {
                return this.m_SoundID;
            }
            set
            {
                this.m_SoundID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
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
        public override string DefaultName
        {
            get
            {
                return "Effect Controller";
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return (this.m_TriggerType == EffectTriggerType.InRange);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        private bool FixedDirection
        {
            get
            {
                return this.m_FixedDirection;
            }
            set
            {
                this.m_FixedDirection = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        private bool Explodes
        {
            get
            {
                return this.m_Explodes;
            }
            set
            {
                this.m_Explodes = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        private bool PlaySoundAtTrigger
        {
            get
            {
                return this.m_PlaySoundAtTrigger;
            }
            set
            {
                this.m_PlaySoundAtTrigger = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_TriggerType == EffectTriggerType.DoubleClick)
                this.DoEffect(from);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Location != oldLocation && this.m_TriggerType == EffectTriggerType.InRange && Utility.InRange(this.GetWorldLocation(), m.Location, this.m_TriggerRange) && !Utility.InRange(this.GetWorldLocation(), oldLocation, this.m_TriggerRange))
                this.DoEffect(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_EffectDelay);
            writer.Write(this.m_TriggerDelay);
            writer.Write(this.m_SoundDelay);

            if (this.m_Source is Item)
                writer.Write(this.m_Source as Item);
            else
                writer.Write(this.m_Source as Mobile);

            if (this.m_Target is Item)
                writer.Write(this.m_Target as Item);
            else
                writer.Write(this.m_Target as Mobile);

            writer.Write(this.m_Trigger as Item);

            writer.Write(this.m_FixedDirection);
            writer.Write(this.m_Explodes);
            writer.Write(this.m_PlaySoundAtTrigger);

            writer.WriteEncodedInt((int)this.m_EffectType);
            writer.WriteEncodedInt((int)this.m_EffectLayer);
            writer.WriteEncodedInt((int)this.m_TriggerType);

            writer.WriteEncodedInt(this.m_ItemID);
            writer.WriteEncodedInt(this.m_Hue);
            writer.WriteEncodedInt(this.m_RenderMode);
            writer.WriteEncodedInt(this.m_Speed);
            writer.WriteEncodedInt(this.m_Duration);
            writer.WriteEncodedInt(this.m_ParticleEffect);
            writer.WriteEncodedInt(this.m_ExplodeParticleEffect);
            writer.WriteEncodedInt(this.m_ExplodeSound);
            writer.WriteEncodedInt(this.m_Unknown);
            writer.WriteEncodedInt(this.m_SoundID);
            writer.WriteEncodedInt(this.m_TriggerRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_EffectDelay = reader.ReadTimeSpan();
                        this.m_TriggerDelay = reader.ReadTimeSpan();
                        this.m_SoundDelay = reader.ReadTimeSpan();

                        this.m_Source = this.ReadEntity(reader);
                        this.m_Target = this.ReadEntity(reader);
                        this.m_Trigger = reader.ReadItem() as EffectController;

                        this.m_FixedDirection = reader.ReadBool();
                        this.m_Explodes = reader.ReadBool();
                        this.m_PlaySoundAtTrigger = reader.ReadBool();

                        this.m_EffectType = (ECEffectType)reader.ReadEncodedInt();
                        this.m_EffectLayer = (EffectLayer)reader.ReadEncodedInt();
                        this.m_TriggerType = (EffectTriggerType)reader.ReadEncodedInt();

                        this.m_ItemID = reader.ReadEncodedInt();
                        this.m_Hue = reader.ReadEncodedInt();
                        this.m_RenderMode = reader.ReadEncodedInt();
                        this.m_Speed = reader.ReadEncodedInt();
                        this.m_Duration = reader.ReadEncodedInt();
                        this.m_ParticleEffect = reader.ReadEncodedInt();
                        this.m_ExplodeParticleEffect = reader.ReadEncodedInt();
                        this.m_ExplodeSound = reader.ReadEncodedInt();
                        this.m_Unknown = reader.ReadEncodedInt();
                        this.m_SoundID = reader.ReadEncodedInt();
                        this.m_TriggerRange = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public void PlaySound(object trigger)
        {
            IEntity ent = null;

            if (this.m_PlaySoundAtTrigger)
                ent = trigger as IEntity;

            if (ent == null)
                ent = this;

            Effects.PlaySound((ent is Item) ? ((Item)ent).GetWorldLocation() : ent.Location, ent.Map, this.m_SoundID);
        }

        public void DoEffect(object trigger)
        {
            if (this.Deleted || this.m_TriggerType == EffectTriggerType.None)
                return;

            if (trigger is Mobile && ((Mobile)trigger).Hidden && ((Mobile)trigger).IsStaff())
                return;

            if (this.m_SoundID > 0)
                Timer.DelayCall(this.m_SoundDelay, new TimerStateCallback(PlaySound), trigger);

            if (this.m_Trigger != null)
                Timer.DelayCall(this.m_TriggerDelay, new TimerStateCallback(this.m_Trigger.DoEffect), trigger);

            if (this.m_EffectType != ECEffectType.None)
                Timer.DelayCall(this.m_EffectDelay, new TimerStateCallback(InternalDoEffect), trigger);
        }

        public void InternalDoEffect(object trigger)
        {
            IEntity from = this.m_Source, to = this.m_Target;
			
            if (from == null)
                from = (IEntity)trigger;

            if (to == null)
                to = (IEntity)trigger;

            switch ( this.m_EffectType )
            {
                case ECEffectType.Lightning:
                    {
                        Effects.SendBoltEffect(from, false, this.m_Hue);
                        break;
                    }
                case ECEffectType.Location:
                    {
                        Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), this.m_ItemID, this.m_Speed, this.m_Duration, this.m_Hue, this.m_RenderMode, this.m_ParticleEffect, this.m_Unknown);
                        break;
                    }
                case ECEffectType.Moving:
                    {
                        if (from == this)
                            from = EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration);

                        if (to == this)
                            to = EffectItem.Create(to.Location, to.Map, EffectItem.DefaultDuration);

                        Effects.SendMovingParticles(from, to, this.m_ItemID, this.m_Speed, this.m_Duration, this.m_FixedDirection, this.m_Explodes, this.m_Hue, this.m_RenderMode, this.m_ParticleEffect, this.m_ExplodeParticleEffect, this.m_ExplodeSound, this.m_EffectLayer, this.m_Unknown);
                        break;
                    }
                case ECEffectType.Target:
                    {
                        Effects.SendTargetParticles(from, this.m_ItemID, this.m_Speed, this.m_Duration, this.m_Hue, this.m_RenderMode, this.m_ParticleEffect, this.m_EffectLayer, this.m_Unknown);
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