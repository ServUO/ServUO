using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells.Spellweaving;

/*"Summon Reaper" with 120 Spellweaving/120 Meditation summons a stationary Reaper that has 603 Health/180 Stam/200 Mana, 
 * 650 STR/180 DEX/200 INT, Resists on this one are 70/15/18/100/69, it deals 80% Physical/20% Poison Damage with 16-20 
 * Base Damage and GM Wrest/GM Tact/GM Resist/GM Anat, no Magery skills. It does cast some Spellweaving spells though,
 * and it has a damage aura, plus it can do Dismount. It did 20-25 damage per hit to a player with max Resists. Arcane 
 * Focus Level does not affect it. 
 
 The spellweaver summons a stationary reaper for a duration based on spellweaving skill, and arcane focus.  The strength 
 * of the summoned reaper is determined by spellweaving skill, arcane focus, and mastery level.*/

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
            if (CheckSequence())
            {
                TimeSpan duration = TimeSpan.FromSeconds((2 * (Caster.Skills[CastSkill].Fixed) + (ArcanistSpell.GetFocusLevel(Caster) * 200)) / 5);
                var reaper = new SummonedReaper(Caster, this);

                Point3D p = Caster.Location;

                if (SpellHelper.FindValidSpawnLocation(Caster.Map, ref p, true))
                {
                    BaseCreature.Summon(reaper, false, Caster, p, 442, duration);
                }
                else
                    reaper.Delete();
            }

            FinishSequence();
        }
    }

    [CorpseName("a reapers corpse")]
    public class SummonedReaper : BaseCreature
    {
        [Constructable]
        public SummonedReaper(Mobile caster, SummonReaperSpell spell)
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
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
        }

        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override bool DisallowAllMoves { get { return true; } }

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