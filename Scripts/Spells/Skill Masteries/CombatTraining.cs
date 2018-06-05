using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;
using System.Linq;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public enum TrainingType
    {
        Empowerment,
        Berserk,
        ConsumeDamage,
        AsOne
    }

    public class CombatTrainingSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Combat Training", "",
                -1,
                9002
            );

        public override double UpKeep
        {
            get
            {
                double taming = Caster.Skills[CastSkill].Base;
                double lore = Caster.Skills[SkillName.AnimalLore].Base;
                bool asone = SpellType == TrainingType.AsOne;

                if (taming >= 120.0)
                {
                    if (lore >= 120)
                    {
                        return asone ? 12 : 6;
                    }

                    if (lore >= 115)
                    {
                        return asone ? 16 : 8;
                    }
                }

                return asone ? 20 : 10;
            }
        }

        public override double RequiredSkill { get { return 90; } }
        public override int RequiredMana { get { return 30; } }
        public override bool PartyEffects { get { return false; } }
        public override SkillName CastSkill { get { return SkillName.AnimalTaming; } }

        public TrainingType SpellType { get; set; }

        private int _Phase;
        private int _DamageTaken;
        private bool _Expired;

        public int Phase { get { return _Phase; } set { _Phase = value; } }
        public int DamageTaken { get { return _DamageTaken; } set { _DamageTaken = value; } }

        public CombatTrainingSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            CombatTrainingSpell spell = GetSpell(Caster, typeof(CombatTrainingSpell)) as CombatTrainingSpell;

            if (spell != null)
            {
                spell.Expire();
                return false;
            }

            if (Caster is PlayerMobile && ((PlayerMobile)Caster).AllFollowers == null || ((PlayerMobile)Caster).AllFollowers.Count == 0)
            {
                Caster.SendLocalizedMessage(1156112); // This ability requires you to have pets.
                return false;
            }

            return base.CheckCast();
        }

        public override void SendCastEffect()
        {
            base.SendCastEffect();

            Caster.PrivateOverheadMessage(MessageType.Regular, 0x35, false, "You ready your pet for combat, increasing its battle effectiveness!", Caster.NetState);
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void OnSelected(TrainingType type, Mobile target)
        {
            if (type == TrainingType.AsOne && Caster is PlayerMobile && ((PlayerMobile)Caster).AllFollowers.Where(mob => mob != target).Count() == 0)
            {
                FinishSequence();
                return;
            }

            SpellType = type;
            Target = target;

            _Phase = 0;

            BeginTimer();

            Target.FixedParticles(0x373A, 10, 80, 5018, 0, 0, EffectLayer.Waist);

            BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.CombatTraining, 1155933, 1156107, String.Format("{0}\t{1}\t{2}", SpellType.ToString(), Target.Name, ((int)ScaleUpkeep()).ToString())));
            //You train ~2_NAME~ to use ~1_SKILLNAME~.<br>Mana Upkeep: ~3_COST~

            FinishSequence();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.CombatTraining);
            Caster.SendSound(0x1ED);

            _Expired = true;
        }

        protected override void DoEffects()
        {
            Caster.FixedParticles(0x376A, 10, 30, 5052, 1261, 0, EffectLayer.LeftFoot, 0);
            Caster.DisruptiveAction();
        }

        public override bool OnTick()
        {
            if (Target == null || Target.IsDeadBondedPet || Target.Map != Caster.Map)
            {
                Expire();
                return false;
            }

            return base.OnTick();
        }

        public double DamageMod
        {
            get
            {
                if (Target == null || SpellType == TrainingType.AsOne)
                    return 0.0;

                double dam = (double)_DamageTaken / ((double)Target.HitsMax * .66);

                if (dam > 1.0) dam = 1.0;

                return dam;
            }
        }

        private void EndPhase1()
        {
            if (_Expired)
                return;

            _Phase = 2;

            Server.Timer.DelayCall(TimeSpan.FromSeconds(SpellType == TrainingType.Berserk ? 8 : 10), EndPhase2);
        }

        private void EndPhase2()
        {
            if (_Expired)
                return;

            _DamageTaken = 0;
            _Phase = 0;

            if (SpellType == TrainingType.Berserk)
            {
                AddRageCooldown(Target);
            }
        }

        public static void CheckDamage(Mobile attacker, Mobile defender, DamageType type, ref int damage)
        {
            if (defender is BaseCreature && (((BaseCreature)defender).Controlled || ((BaseCreature)defender).Summoned))
            {
                CombatTrainingSpell spell = GetSpell<CombatTrainingSpell>(sp => sp.Target == defender);

                if (spell != null)
                {
                    int storedDamage = damage;

                    switch (spell.SpellType)
                    {
                        case TrainingType.Empowerment:
                            break;
                        case TrainingType.Berserk:
                            if (InRageCooldown(defender))
                            {
                                return;
                            }

                            if (spell.Phase > 1)
                            {
                                damage = damage - (int)((double)damage * spell.DamageMod);
                                defender.FixedParticles(0x376A, 10, 30, 5052, 1261, 7, EffectLayer.LeftFoot, 0);
                            }
                            break;
                        case TrainingType.ConsumeDamage:
                            if (spell.Phase < 2)
                            {
                                defender.SendDamagePacket(attacker, damage);
                                damage = 0;
                            }
                            break;
                        case TrainingType.AsOne:
                            if (((BaseCreature)defender).GetMaster() is PlayerMobile)
                            {
                                var pm = ((BaseCreature)defender).GetMaster() as PlayerMobile;
                                var list = pm.AllFollowers.Where(m => (m == defender || m.InRange(defender.Location, 3)) && m.CanBeHarmful(attacker)).ToList();

                                if (list.Count > 0)
                                {
                                    damage = damage / list.Count;

                                    foreach (var m in list.Where(mob => mob != defender))
                                    {
                                        m.Damage(damage, attacker, true, false);
                                    }
                                }

                                ColUtility.Free(list);
                            }
                            return;
                    }


                    if (spell.Phase < 2)
                    {
                        if (spell.Phase != 1)
                        {
                            spell.Phase = 1;

                            if (spell.SpellType != TrainingType.AsOne && (spell.SpellType != TrainingType.Berserk || !InRageCooldown(defender)))
                            {
                                Server.Timer.DelayCall(TimeSpan.FromSeconds(5), spell.EndPhase1);
                            }
                        }

                        if (spell.DamageTaken == 0)
                            defender.FixedEffect(0x3779, 10, 30, 1743, 0);

                        spell.DamageTaken += storedDamage;
                    }
                }
            }
            else if (attacker is BaseCreature && (((BaseCreature)attacker).Controlled || ((BaseCreature)attacker).Summoned))
            {
                CombatTrainingSpell spell = GetSpell<CombatTrainingSpell>(sp => sp.Target == attacker);

                if (spell != null)
                {
                    switch (spell.SpellType)
                    {
                        case TrainingType.Empowerment:
                            if (spell.Phase > 1)
                            {
                                damage = damage + (int)((double)damage * spell.DamageMod);
                                attacker.FixedParticles(0x376A, 10, 30, 5052, 1261, 7, EffectLayer.LeftFoot, 0);
                            }
                            break;
                        case TrainingType.Berserk:
                        case TrainingType.ConsumeDamage:
                        case TrainingType.AsOne:
                            break;
                    }
                }
            }
        }

        public static void OnCreatureHit(Mobile attacker, Mobile defender, ref int damage)
        {
            if (attacker is BaseCreature && (((BaseCreature)attacker).Controlled || ((BaseCreature)attacker).Summoned))
            {
                CombatTrainingSpell spell = GetSpell<CombatTrainingSpell>(sp => sp.Target == attacker);

                if (spell != null)
                {
                    switch (spell.SpellType)
                    {
                        case TrainingType.Empowerment:
                            break;
                        case TrainingType.Berserk:
                            if (spell.Phase > 1)
                            {
                                damage = damage + (int)((double)damage * spell.DamageMod);
                                attacker.FixedParticles(0x376A, 10, 30, 5052, 1261, 7, EffectLayer.LeftFoot, 0);
                            }
                            break;
                        case TrainingType.ConsumeDamage:
                        case TrainingType.AsOne:
                            break;
                    }
                }
            }
        }

        public static int RegenBonus(Mobile m)
        {
            if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned))
            {
                CombatTrainingSpell spell = GetSpell<CombatTrainingSpell>(sp => sp.Target == m);

                if (spell != null && spell.SpellType == TrainingType.ConsumeDamage && spell.Phase > 1)
                {
                    return (int)(30.0 * spell.DamageMod);
                }
            }

            return 0;
        }

        public static int GetHitChanceBonus(Mobile m)
        {
            if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned))
            {
                CombatTrainingSpell spell = GetSpell<CombatTrainingSpell>(sp => sp.Target == m);

                if (spell != null && spell.SpellType == TrainingType.ConsumeDamage && spell.Phase > 1)
                {
                    return (int)(45 * spell.DamageMod);
                }
            }

            return 0;
        }

        public class InternalTarget : Target
        {
            public CombatTrainingSpell Spell { get; set; }

            public InternalTarget(CombatTrainingSpell spell)
                : base(8, false, TargetFlags.None)
            {
                Spell = spell;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature && ((BaseCreature)targeted).GetMaster() == from && from.Spell == Spell)
                {
                    Spell.Caster.FixedEffect(0x3779, 10, 20, 1270, 0);
                    Spell.Caster.SendSound(0x64E);

                    int taming = (int)from.Skills[SkillName.AnimalTaming].Value;
                    int lore = (int)from.Skills[SkillName.AnimalLore].Value;

                    from.CheckTargetSkill(SkillName.AnimalTaming, (BaseCreature)targeted, taming - 25, taming + 25);
                    from.CheckTargetSkill(SkillName.AnimalLore, (BaseCreature)targeted, lore - 25, lore + 25);

                    from.CloseGump(typeof(ChooseTrainingGump));
                    from.SendGump(new ChooseTrainingGump(from, (BaseCreature)targeted, Spell));
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.SendLocalizedMessage(1156110); // Your ability was canceled.
                Spell.FinishSequence();
            }
        }

        public static void AddRageCooldown(Mobile m)
        {
            if (_RageCooldown == null)
                _RageCooldown = new Dictionary<Mobile, Timer>();

            _RageCooldown[m] = Server.Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(60), EndRageCooldown, m);
        }

        public static bool InRageCooldown(Mobile m)
        {
            return _RageCooldown != null && _RageCooldown.ContainsKey(m);
        }

        public static void EndRageCooldown(Mobile m)
        {
            if (_RageCooldown != null && _RageCooldown.ContainsKey(m))
            {
                _RageCooldown.Remove(m);
            }
        }

        public static Dictionary<Mobile, Timer> _RageCooldown;
    }

    public class ChooseTrainingGump : Gump
    {
        public CombatTrainingSpell Spell { get; private set; }
        public Mobile Caster { get; private set; }
        public BaseCreature Target { get; private set; }

        public const int Hue = 0x07FF;

        public ChooseTrainingGump(Mobile caster, BaseCreature target, CombatTrainingSpell spell) : base(100, 100)
        {
            Spell = spell;
            Caster = caster;
            Target = target;

            AddBackground(0, 0, 260, 187, 3600);
            AddAlphaRegion(10, 10, 240, 167);

            AddImageTiled(220, 15, 30, 162, 10464);

            AddHtmlLocalized(20, 20, 150, 16, 1156113, Hue, false, false); // Select Training

            AddButton(20, 40, 9762, 9763, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(43, 40, 150, 16, 1156109, Hue, false, false); // Empowerment

            AddButton(20, 60, 9762, 9763, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(43, 60, 150, 16, 1153271, Hue, false, false); // Berserk

            AddButton(20, 80, 9762, 9763, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(43, 80, 150, 16, 1156108, Hue, false, false); // Consume Damage

            AddButton(20, 100, 9762, 9763, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(43, 100, 150, 16, 1157544, Hue, false, false); // As One
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                Spell.FinishSequence();
                state.Mobile.SendLocalizedMessage(1156110); // Your ability was canceled. 
                return;
            }

            Spell.OnSelected((TrainingType)info.ButtonID - 1, Target);
        }
    }
}
