using System;

namespace Server.Spells.Mystic
{
    public abstract class MysticTransformationSpell : MysticSpell, ITransformationSpell
    {
        public MysticTransformationSpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
            if (this.Body != caster.BodyValue)
                caster.Flying = false;
        }

        public abstract int Body { get; }
        public virtual int Hue
        {
            get
            {
                return 0;
            }
        }
        public virtual int PhysResistOffset
        {
            get
            {
                return 0;
            }
        }
        public virtual int FireResistOffset
        {
            get
            {
                return 0;
            }
        }
        public virtual int ColdResistOffset
        {
            get
            {
                return 0;
            }
        }
        public virtual int PoisResistOffset
        {
            get
            {
                return 0;
            }
        }
        public virtual int NrgyResistOffset
        {
            get
            {
                return 0;
            }
        }
        public override bool BlockedByHorrificBeast
        {
            get
            {
                return false;
            }
        }
        public virtual double TickRate
        {
            get
            {
                return 1.0;
            }
        }
        public override bool CheckCast()
        {
            if (!TransformationSpellHelper.CheckCast(this.Caster, this))
                return false;

            return base.CheckCast();
        }

        public override void OnCast()
        {
            TransformationSpellHelper.OnCast(this.Caster, this);

            this.FinishSequence();
        }

        public virtual void OnTick(Mobile m)
        {
        }

        public virtual void DoEffect(Mobile m)
        {
        }

        public virtual void RemoveEffect(Mobile m)
        {
        }
    }
}