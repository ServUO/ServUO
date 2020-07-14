using Server.Items;
using Server.Spells;
using Server.Spells.SkillMasteries;
using System;

namespace Server.Mobiles
{
    [CorpseName("an anchisaur corpse")]
    public class Anchisaur : BaseCreature
    {
        public override bool AttacksFocus => true;
        private DateTime _NextMastery;

        [Constructable]
        public Anchisaur()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "an anchisaur";
            Body = 1292;
            BaseSoundID = 422;

            SetStr(441, 511);
            SetDex(166, 185);
            SetInt(362, 431);

            SetDamage(16, 19);

            SetHits(2663, 3718);

            SetResistance(ResistanceType.Physical, 3, 4);
            SetResistance(ResistanceType.Fire, 3, 4);
            SetResistance(ResistanceType.Cold, 1);
            SetResistance(ResistanceType.Poison, 2, 3);
            SetResistance(ResistanceType.Energy, 2, 3);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 105.0, 115.0);
            SetSkill(SkillName.Tactics, 95.0, 105.0);
            SetSkill(SkillName.Wrestling, 100.0, 110.0);
            SetSkill(SkillName.Anatomy, 95.0, 105.0);
            SetSkill(SkillName.DetectHidden, 75.0, 85.0);
            SetSkill(SkillName.Parry, 75.0, 85.0);

            Fame = 8000;
            Karma = -8000;

            SetWeaponAbility(WeaponAbility.Disarm);
            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (_NextMastery < DateTime.UtcNow)
            {
                if (SkillMasterySpell.HasSpell(this, typeof(RampageSpell)) || Utility.RandomDouble() > 0.5)
                {
                    SpecialMove.SetCurrentMove(this, SpellRegistry.GetSpecialMove(740));
                }
                else
                {
                    SkillMasterySpell spell = new RampageSpell(this, null);
                    spell.Cast();
                }

                _NextMastery = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 70));
            }
        }

        public override int DragonBlood => 6;
        public override int Meat => 6;
        public override MeatType MeatType => MeatType.DinoRibs;
        public override int Hides => 11;
        public override int TreasureMapLevel => 1;

        public Anchisaur(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
