using System;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseThrown : BaseRanged
    {
        private Mobile m_Thrower;
        private Mobile m_Target;
        private Point3D m_KillSave;
        public BaseThrown(int itemID)
            : base(itemID)
        {
        }

        public BaseThrown(Serial serial)
            : base(serial)
        {
        }

        public abstract int MinThrowRange { get; }
        public virtual int MaxThrowRange
        {
            get
            {
                return this.MinThrowRange + 4;
            }
        }
        public override int DefMaxRange
        {
            get
            {
                if (this.Parent is PlayerMobile)
                {
                    int range = ((PlayerMobile)this.Parent).Str / 10;

                    return Math.Min(range, 10);
                }

                return 10;
            }
        }
        public override int EffectID
        {
            get
            {
                return this.ItemID;
            }
        }
        public override Type AmmoType
        {
            get
            {
                return null;
            }
        }
        public override Item Ammo
        {
            get
            {
                return null;
            }
        }
        public override int DefHitSound
        {
            get
            {
                return 0x5D3;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x5D4;
            }
        }
        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Throwing;
            }
        }
        //public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Slash1H;
            }
        }
        public override SkillName AccuracySkill
        {
            get
            {
                return SkillName.Throwing;
            }
        }
        public override TimeSpan OnSwing(Mobile attacker, Mobile defender)
        {
            TimeSpan ts = base.OnSwing(attacker, defender);

            // time it takes to throw it around including mystic arc
            if (ts < TimeSpan.FromMilliseconds(1000))
                ts = TimeSpan.FromMilliseconds(1000);

            return ts;
        }

        public override bool OnFired(Mobile attacker, Mobile defender)
        {
            this.m_Thrower = attacker;

            if (!attacker.InRange(defender, 1))
            {
                //Internalize();
                this.Visible = false;
                attacker.MovingEffect(defender, this.EffectID, 18, 1, false, false);
            }

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            this.m_Target = defender;
            this.m_KillSave = defender.Location;

            if (!(WeaponAbility.GetCurrentAbility(attacker) is MysticArc))
                Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), new TimerCallback(ThrowBack));

            base.OnHit(attacker, defender, damageBonus);
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            this.m_Target = defender;

            if (!(WeaponAbility.GetCurrentAbility(attacker) is MysticArc))
                Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), new TimerCallback(ThrowBack));

            base.OnMiss(attacker, defender);
        }

        public virtual void ThrowBack()
        {
            if (this.m_Target != null)
                this.m_Target.MovingEffect(this.m_Thrower, this.EffectID, 18, 1, false, false);
            else if (this.m_Thrower != null)
                Effects.SendMovingParticles(new Entity(Serial.Zero, this.m_KillSave, this.m_Thrower.Map), this.m_Thrower, this.ItemID, 18, 0, false, false, this.Hue, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

            Timer.DelayCall(TimeSpan.FromMilliseconds(333.0), new TimerCallback(UnHide));
        }

        public virtual void UnHide()
        {
            if (this != null)
                this.Visible = true;
            //if ( m_Thrower != null )
            //if ( !m_Thrower.EquipItem( this ) )
            //MoveToWorld( m_Thrower.Location, m_Thrower.Map );
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            this.Visible = true;
        }
    }
}