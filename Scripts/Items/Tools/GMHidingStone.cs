/*
Script Name: GMHidingStone.cs
Author: Marchenzio
Version: 1.0
Public Release: 01/01/09
Updated Release: 01/01/09
Purpose: A stone that allows for multiple hide/appear effects for GM and above.
*/                                                            
using System;
using CustomsFramework;

namespace Server.Items
{
    public enum StoneEffect
    {
        Gate,
        FlameStrike1,
        FlameStrike3,
        FlameStrikeLightningBolt,
        Sparkle1,
        Sparkle3, 
        Explosion,
        ExplosionLightningBolt,
        DefaultRunUO,
        Snow,
        Glow,
        PoisonField,
        Fireball, 
        FireStorm1,
        FireStorm2,
        RedSparkle,
        RedSparkle2,
        Marchenzio1,
        Hell
    }

    public class GMHidingStone : Item
    {
        private StoneEffect mAppearEffect;
        private StoneEffect mHideEffect;
        private int mAppearEffectHue;
        private int mHideEffectHue;
        private FireStormTimer m_Timer;
        [Constructable]
        public GMHidingStone()
            : base(0x1870)
        {
            this.Weight = 1.0;
            this.Hue = 0x0;
            this.Name = "GM hiding stone";
            this.LootType = LootType.Blessed;
            this.mAppearEffect = StoneEffect.DefaultRunUO;
            this.mAppearEffectHue = 0;
            this.mHideEffect = StoneEffect.DefaultRunUO;
            this.mHideEffectHue = 0;
        }

        public GMHidingStone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Counselor)]
        public StoneEffect AppearEffect
        {
            get
            {
                return this.mAppearEffect;
            }
            set
            {
                this.mAppearEffect = value;
            }
        }
        [Hue, CommandProperty(AccessLevel.Counselor)]
        public int AppearEffectHue
        {
            get
            {
                return this.mAppearEffectHue;
            }
            set
            {
                this.mAppearEffectHue = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor)]
        public StoneEffect HideEffect
        {
            get
            {
                return this.mHideEffect;
            }
            set
            {
                this.mHideEffect = value;
            }
        }
        [Hue, CommandProperty(AccessLevel.Counselor)]
        public int HideEffectHue
        {
            get
            {
                return this.mHideEffectHue;
            }
            set
            {
                this.mHideEffectHue = value;
            }
        }
        public override void OnDoubleClick(Mobile m)
        {
            if (Utilities.IsStaff(m))
            {
                if (m.Hidden)
                {
                    this.ToggleHidden(m, this.mAppearEffect);
                    this.SendStoneEffects(this.mAppearEffect, this.mAppearEffectHue, m);
                }
                else
                {
                    this.SendStoneEffects(this.mHideEffect, this.mHideEffectHue, m);
                    this.ToggleHidden(m, this.mHideEffect);
                }
            }
            else
            {
                m.SendMessage("You are unable to use that!");
            }
        }

        public void SendStoneEffects(StoneEffect mStoneEffect, int effHue, Mobile m)
        {
            if (effHue > 0)
                effHue--; //Adjust the friggin hue to match true effect color
            switch (mStoneEffect)
            {
                //[s7]
                case StoneEffect.Gate:
                    Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x1FCB, 10, 14, effHue, 0, 0x1FCB, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x20E);
                    m.Frozen = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(0.65), new TimerStateCallback(InternalShowGate), new object[] { m, effHue });
                    Timer.DelayCall(TimeSpan.FromSeconds(1.5), new TimerStateCallback(InternalHideGate), new object[] { m, effHue });
                    break;
                    //[/s7]
                case StoneEffect.FlameStrike1:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    break;
                case StoneEffect.FlameStrike3:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    break;
                case StoneEffect.Snow:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x376A, 15, effHue, 0); //0x47D );
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 492);
                    break;
                case StoneEffect.FlameStrikeLightningBolt:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendBoltEffect(m, true, 0);
                    break;
                case StoneEffect.Sparkle1:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x375A, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x213);
                    break;
                case StoneEffect.Sparkle3:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x373A, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x373A, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z - 1), m.Map, 0x373A, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x213);
                    break;
                case StoneEffect.Explosion:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
                    break;
                case StoneEffect.ExplosionLightningBolt:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendBoltEffect(m, true, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
                    break;
                case StoneEffect.DefaultRunUO:
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z - 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z - 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 7), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 3), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3728, 13, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x228);
                    break;
                case StoneEffect.Glow:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x37C4, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1E2);
                    break;
                case StoneEffect.PoisonField:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x3915, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x231);
                    break;
                case StoneEffect.Fireball:
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 8), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 8), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 10), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x15E);
                    break;
                case StoneEffect.FireStorm1: //Added By Nitewender (further modifed by me to carry color effect to timer
                    m.PlaySound(520);
                    m.PlaySound(525);
                    m.Hidden = !m.Hidden;
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X - 1, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y - 1, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    this.m_Timer = new FireStormTimer(DateTime.UtcNow, m, effHue, 0, 1);
                    this.m_Timer.Start();
                    break;
                case StoneEffect.FireStorm2: //CEO Using above idea, this one does the firestorm outside->in
                    m.PlaySound(520);
                    m.PlaySound(525);
                    Effects.SendLocationEffect(new Point3D(m.X + 5, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X - 5, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y - 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 5, m.Y - 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X - 5, m.Y + 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    this.m_Timer = new FireStormTimer(DateTime.UtcNow, m, effHue, 5, -1);
                    this.m_Timer.Start();
                    break;
                case StoneEffect.RedSparkle:
                    Effects.SendLocationEffect(new Point3D(m.X , m.Y + 1, m.Z), m.Map, 0x374A, 15);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1f7);
                    break;
                case StoneEffect.RedSparkle2:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x374A, 15);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x374A, 15);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x374A, 15);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x374A, 15);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1E0);
                    break;
                case StoneEffect.Marchenzio1:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x376A, 15, effHue, 0); //0x47D );
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 492);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendBoltEffect(m, true, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x375A, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x213);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x373A, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x373A, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z - 1), m.Map, 0x373A, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x213);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendBoltEffect(m, true, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z - 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z - 4), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 7), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 3), m.Map, 0x3728, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3728, 13, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x228);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x37C4, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1E2);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 8), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 8), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 10), m.Map, 0x36D4, 13, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x15E);
                    m.PlaySound(520);
                    m.PlaySound(525);
                    Effects.SendLocationEffect(new Point3D(m.X + 5, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X - 5, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y - 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 5, m.Y - 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X - 5, m.Y + 5, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    this.m_Timer = new FireStormTimer(DateTime.UtcNow, m, effHue, 5, -1);
                    this.m_Timer.Start();
                    Effects.SendLocationEffect(new Point3D(m.X , m.Y + 1, m.Z), m.Map, 0x374A, 15);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1f7);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x374A, 15);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x374A, 15);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x374A, 15);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x374A, 15);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x1E0);
                    m.Hidden = !m.Hidden;
                    break;
                case StoneEffect.Hell:
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 6), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x3709, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x208);
                    Effects.SendBoltEffect(m, true, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z + 1), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x36BD, 15, effHue, 0);
                    Effects.SendBoltEffect(m, true, 0);
                    Effects.PlaySound(new Point3D(m.X, m.Y, m.Z), m.Map, 0x307);
                    m.PlaySound(520);
                    m.PlaySound(525);
                    Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X - 1, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y - 1, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    Effects.SendLocationEffect(new Point3D(m.X, m.Y, m.Z), m.Map, 0x3709, 17, effHue, 0);
                    this.m_Timer = new FireStormTimer(DateTime.UtcNow, m, effHue, 0, 1);
                    this.m_Timer.Start();
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version 
            writer.Write((int)this.mAppearEffect);
            writer.Write((int)this.mHideEffect);
            writer.Write(this.mAppearEffectHue);
            writer.Write(this.mHideEffectHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        this.mAppearEffect = (StoneEffect)reader.ReadInt();
                        this.mHideEffect = (StoneEffect)reader.ReadInt();
                        break;
                    }
                case 2:
                    {
                        this.mAppearEffect = (StoneEffect)reader.ReadInt();
                        this.mHideEffect = (StoneEffect)reader.ReadInt();
                        this.mAppearEffectHue = reader.ReadInt();
                        this.mHideEffectHue = reader.ReadInt();
                        break;
                    }
            }
        }

        private void ToggleHidden(Mobile m, StoneEffect heffect)
        {
            switch (heffect)
            {
                case StoneEffect.Gate:
                    break;
                case StoneEffect.FireStorm1:
                    break;
                case StoneEffect.FireStorm2:
                    break;
                default:
                    m.Hidden = !m.Hidden;
                    break;
            }
        }

        //[s7] gate!
        private void InternalHideGate(object arg)
        {
            object[] args = arg as object[];
            Mobile m = args[0] as Mobile;
            int hue = (int)args[1];
            if (m != null)
            {
                m.Hidden = !m.Hidden;
                Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x376A, 9, 20, hue, 0, 5042, 0);
                Effects.PlaySound(m.Location, m.Map, 0x201);
                m.Frozen = false;
            }
        }

        private void InternalShowGate(object arg)
        {
            object[] args = arg as object[];
            Mobile m = args[0] as Mobile;
            int hue = (int)args[1];
            if (m is Mobile)
                Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 8148, 9, 20, hue, 0, 8149, 0);
        }

        //[/s7]
        public class FireStormTimer : Timer
        {
            public Mobile m;
            public int inc;
            public int ehue;
            public int fstart;
            public int fdir;
            public FireStormTimer(DateTime time, Mobile from, int hue, int start, int dir)
                : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1))
            {
                this.Priority = TimerPriority.FiftyMS;
                this.m = from;
                this.ehue = hue;
                this.fstart = start;
                this.fdir = dir;
                this.inc = start;
            }

            protected override void OnTick()
            {
                this.inc = this.inc + this.fdir;

                Effects.SendLocationEffect(new Point3D(this.m.X + this.inc, this.m.Y, this.m.Z), this.m.Map, 0x3709, 17, this.ehue, 0);
                Effects.SendLocationEffect(new Point3D(this.m.X - this.inc, this.m.Y, this.m.Z), this.m.Map, 0x3709, 17, this.ehue, 0);
                Effects.SendLocationEffect(new Point3D(this.m.X, this.m.Y + this.inc, this.m.Z), this.m.Map, 0x3709, 17, this.ehue, 0);
                Effects.SendLocationEffect(new Point3D(this.m.X, this.m.Y - this.inc, this.m.Z), this.m.Map, 0x3709, 17, this.ehue, 0);
                Effects.SendLocationEffect(new Point3D(this.m.X + this.inc, this.m.Y - this.inc, this.m.Z), this.m.Map, 0x3709, 17, this.ehue, 0);
                Effects.SendLocationEffect(new Point3D(this.m.X - this.inc, this.m.Y + this.inc, this.m.Z), this.m.Map, 0x3709, 17, this.ehue, 0);

                if ((this.fdir == 1 && this.inc >= (this.fstart + 5)) || (this.fdir == -1 && this.inc < 0))
                {
                    if (this.fdir == -1)
                    {
                        this.m.Hidden = !this.m.Hidden;
                    }
                    this.Stop();
                }
            }
        }
    }
}