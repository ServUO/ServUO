namespace Server.Spells.Mysticism
{
    public abstract class MysticTransformationSpell : MysticSpell, ITransformationSpell
    {
        public abstract int Body { get; }
        public virtual int Hue => 0;

        public virtual int PhysResistOffset => 0;
        public virtual int FireResistOffset => 0;
        public virtual int ColdResistOffset => 0;
        public virtual int PoisResistOffset => 0;
        public virtual int NrgyResistOffset => 0;

        public MysticTransformationSpell(Mobile caster, Item scroll, SpellInfo info) : base(caster, scroll, info)
        {
        }

        public override bool BlockedByHorrificBeast => false;

        public override bool CheckCast()
        {
            if (!TransformationSpellHelper.CheckCast(Caster, this))
                return false;

            return base.CheckCast();
        }

        public override void OnCast()
        {
            TransformationSpellHelper.OnCast(Caster, this);

            FinishSequence();
        }

        public virtual double TickRate => 1.0;

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