using System;
using Server.Items;
using Server.Spells.SkillMasteries;
using Server.Spells;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an anchisaur corpse")]
    public class Anchisaur : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }
        private DateTime _NextMastery;

        [Constructable]
        public Anchisaur()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "an anchisaur";
            this.Body = 1292;
            this.BaseSoundID = 422;

            this.SetStr(441, 511);
            this.SetDex(166, 185);
            this.SetInt(362, 431);

            this.SetDamage(16, 19);

            this.SetHits(2663, 3718);

            this.SetResistance(ResistanceType.Physical, 3, 4);
            this.SetResistance(ResistanceType.Fire, 3, 4);
            this.SetResistance(ResistanceType.Cold, 1);
            this.SetResistance(ResistanceType.Poison, 2, 3);
            this.SetResistance(ResistanceType.Energy, 2, 3);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetSkill(SkillName.MagicResist, 105.0, 115.0);
            this.SetSkill(SkillName.Tactics, 95.0, 105.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 110.0);
            this.SetSkill(SkillName.Anatomy, 95.0, 105.0);
            this.SetSkill(SkillName.DetectHidden, 75.0, 85.0);
            this.SetSkill(SkillName.Parry, 75.0, 85.0);

            this.Fame = 8000;
            this.Karma = -8000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            if (Utility.RandomBool())
                return WeaponAbility.ParalyzingBlow;

            return WeaponAbility.Disarm;
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

        public override int DragonBlood { get { return 6; } }
        public override int Meat { get { return 6; } }
        public override int Hides { get { return 11; } }

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