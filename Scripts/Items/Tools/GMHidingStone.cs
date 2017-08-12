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

        [Constructable]
        public GMHidingStone()
            : base(0x1870)
        {
            Weight = 1.0;
            Hue = 0x0;
            Name = "GM hiding stone";
            LootType = LootType.Blessed;
            mAppearEffect = StoneEffect.DefaultRunUO;
            mAppearEffectHue = 0;
            mHideEffect = StoneEffect.DefaultRunUO;
            mHideEffectHue = 0;
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
                return mAppearEffect;
            }
            set
            {
                mAppearEffect = value;
            }
        }
        [Hue, CommandProperty(AccessLevel.Counselor)]
        public int AppearEffectHue
        {
            get
            {
                return mAppearEffectHue;
            }
            set
            {
                mAppearEffectHue = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor)]
        public StoneEffect HideEffect
        {
            get
            {
                return mHideEffect;
            }
            set
            {
                mHideEffect = value;
            }
        }
        [Hue, CommandProperty(AccessLevel.Counselor)]
        public int HideEffectHue
        {
            get
            {
                return mHideEffectHue;
            }
            set
            {
                mHideEffectHue = value;
            }
        }
        public override void OnDoubleClick(Mobile m)
        {
            if (Utilities.IsStaff(m))
            {
                if (m.Hidden)
                {
                    ToggleHidden(m, mAppearEffect);
                    SendStoneEffects(mAppearEffect, mAppearEffectHue, m);
                }
                else
                {
                    SendStoneEffects(mHideEffect, mHideEffectHue, m);
                    ToggleHidden(m, mHideEffect);
                }
            }
            else
            {
                m.SendMessage("You are unable to use that!");
            }
        }

        public static void SendStoneEffects(StoneEffect mStoneEffect, int effHue, Mobile m)
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
                    Timer t = new FireStormTimer(DateTime.UtcNow, m, effHue, 0, 1);
                    t.Start();
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
                    Timer t1 = new FireStormTimer(DateTime.UtcNow, m, effHue, 5, -1);
                    t1.Start();
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
                    Timer t2 = new FireStormTimer(DateTime.UtcNow, m, effHue, 5, -1);
                    t2.Start();
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
                    Timer t3 = new FireStormTimer(DateTime.UtcNow, m, effHue, 0, 1);
                    t3.Start();
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version 
            writer.Write((int)mAppearEffect);
            writer.Write((int)mHideEffect);
            writer.Write(mAppearEffectHue);
            writer.Write(mHideEffectHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        mAppearEffect = (StoneEffect)reader.ReadInt();
                        mHideEffect = (StoneEffect)reader.ReadInt();
                        break;
                    }
                case 2:
                    {
                        mAppearEffect = (StoneEffect)reader.ReadInt();
                        mHideEffect = (StoneEffect)reader.ReadInt();
                        mAppearEffectHue = reader.ReadInt();
                        mHideEffectHue = reader.ReadInt();
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
        private static void InternalHideGate(object arg)
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

        private static void InternalShowGate(object arg)
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
                Priority = TimerPriority.FiftyMS;
                m = from;
                ehue = hue;
                fstart = start;
                fdir = dir;
                inc = start;
            }

            protected override void OnTick()
            {
                inc = inc + fdir;

                Effects.SendLocationEffect(new Point3D(m.X + inc, m.Y, m.Z), m.Map, 0x3709, 17, ehue, 0);
                Effects.SendLocationEffect(new Point3D(m.X - inc, m.Y, m.Z), m.Map, 0x3709, 17, ehue, 0);
                Effects.SendLocationEffect(new Point3D(m.X, m.Y + inc, m.Z), m.Map, 0x3709, 17, ehue, 0);
                Effects.SendLocationEffect(new Point3D(m.X, m.Y - inc, m.Z), m.Map, 0x3709, 17, ehue, 0);
                Effects.SendLocationEffect(new Point3D(m.X + inc, m.Y - inc, m.Z), m.Map, 0x3709, 17, ehue, 0);
                Effects.SendLocationEffect(new Point3D(m.X - inc, m.Y + inc, m.Z), m.Map, 0x3709, 17, ehue, 0);

                if ((fdir == 1 && inc >= (fstart + 5)) || (fdir == -1 && inc < 0))
                {
                    if (fdir == -1)
                    {
                        m.Hidden = !m.Hidden;
                    }
                    Stop();
                }
            }
        }
    }
}