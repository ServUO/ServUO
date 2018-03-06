using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells.Spellweaving;

namespace Server.Spells.SkillMasteries
{
    public class SummonReaperSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Summon Reaper", "Lartarisstree",
                204,
				9061
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 0; } }
        public override int RequiredMana { get { return 50; } }
        public override bool PartyEffects { get { return false; } }

        public override SkillName CastSkill { get { return SkillName.Spellweaving; } }

        public SummonReaperSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 1371, 2);
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (Caster is PlayerMobile && !((PlayerMobile)Caster).Spellweaving)
            {
                Caster.SendLocalizedMessage(1073220); // You must have completed the epic arcanist quest to use this ability.
                return false;
            }
            else if (Caster.Followers + 5 > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this, 10, true, Server.Targeting.TargetFlags.None);
        }

        protected override void OnTarget(object o)
        {
            if (o is IPoint3D)
            {
                Map map = Caster.Map;
                IPoint3D p = o as IPoint3D;

                SpellHelper.GetSurfaceTop(ref p);

                if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
                {
                    this.Caster.SendLocalizedMessage(501942); // That location is blocked.
                }
                else if (SpellHelper.CheckTown(p, this.Caster) && this.CheckSequence())
                {
                    TimeSpan duration = TimeSpan.FromSeconds(((Caster.Skills[CastSkill].Value + (ArcanistSpell.GetFocusLevel(Caster) * 20)) / 240) * 75);
                    BaseCreature.Summon(new SummonedReaper(Caster, this), false, this.Caster, new Point3D(p), 442, duration);
                }
            }
        }
    }

    [CorpseName("a reapers corpse")]
    public class SummonedReaper : BaseCreature
    {
        private DateTime _StartTime;

        [Constructable]
        public SummonedReaper(Mobile caster, SummonReaperSpell spell)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a reaper";
            Body = 47;
            BaseSoundID = 442;

            double scale = 1.0 + ((caster.Skills[spell.CastSkill].Value + (double)(spell.GetMasteryLevel() * 40) + (double)(ArcanistSpell.GetFocusLevel(caster) * 20))) / 1000.0;

            SetStr((int)(450 * scale), (int)(500 * scale));
            SetDex((int)(130 * scale));
            SetInt((int)(247 * scale));

            SetHits((int)(450 * scale));

            SetDamage(16, 20);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 70);
            SetResistance(ResistanceType.Fire, 15);
            SetResistance(ResistanceType.Cold, 18);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 69);

            SetSkill(SkillName.Spellweaving, Math.Max(100, 75 * scale));
            SetSkill(SkillName.Anatomy, Math.Max(100, 75 * scale));
            SetSkill(SkillName.MagicResist, Math.Max(100, 75 * scale));
            SetSkill(SkillName.Tactics, Math.Max(100, 75 * scale));
            SetSkill(SkillName.Wrestling, Math.Max(100, 75 * scale));

            ControlSlots = 5;

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    ArcaneFocus casterFocus = ArcanistSpell.FindArcaneFocus(caster);

                    if (casterFocus != null)
                    {
                        ArcaneFocus f = new ArcaneFocus(casterFocus.LifeSpan, casterFocus.StrengthBonus);
                        f.CreationTime = casterFocus.CreationTime;
                        f.Movable = false;

                        PackItem(f);
                    }
                });

            _StartTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);

            SetWeaponAbility(WeaponAbility.WhirlwindAttack);
        }

        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool HasAura { get { return _StartTime < DateTime.UtcNow; } }
        public override TimeSpan AuraInterval { get { return TimeSpan.FromSeconds(2); } }
        public override int AuraBaseDamage { get { return Utility.RandomMinMax(10, 20); } }
        public override int AuraFireDamage { get { return 0; } }
        public override int AuraPoisonDamage { get { return 100; } }

        public override void AuraDamage()
        {
            Server.Misc.Geometry.Circle2D(Location, Map, AuraRange, (pnt, map) =>
            {
                Effects.SendLocationEffect(pnt, map, 0x3709, 0x14, 0x1, 0x8AF, 4);
            });

            Server.Misc.Geometry.Circle2D(this.Location, this.Map, AuraRange + 1, (pnt, map) =>
            {
                Effects.SendLocationEffect(pnt, map, 0x3709, 0x14, 0x1, 0x8AF, 4);
            });

            base.AuraDamage();
        }

        public SummonedReaper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}