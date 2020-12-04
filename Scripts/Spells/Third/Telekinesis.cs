using Server.Items;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class TelekinesisSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Telekinesis", "Ort Por Ylem",
            203,
            9031,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);
        public TelekinesisSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Third;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(ITelekinesisable obj)
        {
            if (CheckSequence())
            {
                SpellHelper.Turn(Caster, obj);

                obj.OnTelekinesis(Caster);
            }

            FinishSequence();
        }

        public void Target(Container item)
        {
            if (CheckSequence())
            {
                SpellHelper.Turn(Caster, item);

                object root = item.RootParent;

                if (!item.IsAccessibleTo(Caster))
                {
                    item.OnDoubleClickNotAccessible(Caster);
                }
                else if (!item.CheckItemUse(Caster, item))
                {
                }
                else if (root != null && root is Mobile && root != Caster)
                {
                    item.OnSnoop(Caster);
                }
                else if (item is Corpse && !((Corpse)item).CheckLoot(Caster))
                {
                }
                else if (Caster.Region.OnDoubleClick(Caster, item))
                {
                    Effects.SendLocationParticles(EffectItem.Create(item.Location, item.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
                    Effects.PlaySound(item.Location, item.Map, 0x1F5);

                    item.OnItemUsed(Caster, item);
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly TelekinesisSpell m_Owner;
            public InternalTarget(TelekinesisSpell owner)
                : base(10, false, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is ITelekinesisable && (!(o is Container) || !Siege.SiegeShard))
                    m_Owner.Target((ITelekinesisable)o);
                else if (o is Container && !Siege.SiegeShard)
                    m_Owner.Target((Container)o);
                else
                    from.SendLocalizedMessage(501857); // This spell won't work on that!
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}

namespace Server
{
    public interface ITelekinesisable : IPoint3D
    {
        void OnTelekinesis(Mobile from);
    }
}
