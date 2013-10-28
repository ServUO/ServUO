using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a deathwatchbeetle corpse")]
    [TypeAlias("Server.Mobiles.DeathWatchBeetle")]
    public class DeathwatchBeetle : BaseCreature
    {
        [Constructable]
        public DeathwatchBeetle()
            : base(AIType.AI_Melee, Core.ML ? FightMode.Aggressor : FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a deathwatch beetle";
            this.Body = 242;

            this.SetStr(136, 160);
            this.SetDex(41, 52);
            this.SetInt(31, 40);

            this.SetHits(121, 145);
            this.SetMana(20);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 15, 30);
            this.SetResistance(ResistanceType.Cold, 15, 30);
            this.SetResistance(ResistanceType.Poison, 50, 80);
            this.SetResistance(ResistanceType.Energy, 20, 35);

            this.SetSkill(SkillName.MagicResist, 50.1, 58.0);
            this.SetSkill(SkillName.Tactics, 67.1, 77.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 60.0);
            this.SetSkill(SkillName.Anatomy, 30.1, 34.0);

            this.Fame = 1400;
            this.Karma = -1400;

            switch ( Utility.Random(12) )
            {
                case 0:
                    this.PackItem(new LeatherGorget());
                    break;
                case 1:
                    this.PackItem(new LeatherGloves());
                    break;
                case 2:
                    this.PackItem(new LeatherArms());
                    break;
                case 3:
                    this.PackItem(new LeatherLegs());
                    break;
                case 4:
                    this.PackItem(new LeatherCap());
                    break;
                case 5:
                    this.PackItem(new LeatherChest());
                    break;
            }

            if (Utility.RandomDouble() < .5)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());

            this.Tamable = true;
            this.MinTameSkill = 41.1;
            this.ControlSlots = 1;
        }

        public DeathwatchBeetle(Serial serial)
            : base(serial)
        {
        }

        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
        }

        public override int GetAngerSound()
        {
            return 0x4F3;
        }

        public override int GetIdleSound()
        {
            return 0x4F2;
        }

        public override int GetAttackSound()
        {
            return 0x4F1;
        }

        public override int GetHurtSound()
        {
            return 0x4F4;
        }

        public override int GetDeathSound()
        {
            return 0x4F0;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.LowScrolls, 1);
            this.AddLoot(LootPack.Potions, 1);
        }

        public override void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            if (Utility.RandomBool() && (this.Mana > 14) && to != null)
            {
                damage = (damage + (damage / 2));
                to.SendLocalizedMessage(1060091); // You take extra damage from the crushing attack!
                to.PlaySound(0x1E1);
                to.FixedParticles(0x377A, 1, 32, 0x26da, 0, 0, 0);
                this.Mana -= 15;
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            Mobile combatant = this.Combatant;

            if (combatant == null || combatant.Deleted || combatant.Map != this.Map || !this.InRange(combatant, 12) || !this.CanBeHarmful(combatant) || !this.InLOS(combatant))
                return;

            if (Utility.Random(10) == 0)
                this.PoisonAttack(combatant);

            base.OnDamage(amount, from, willKill);
        }

        public void PoisonAttack(Mobile m)
        {
            this.DoHarmful(m);
            this.MovingParticles(m, 0x36D4, 1, 0, false, false, 0x3F, 0, 0x1F73, 1, 0, (EffectLayer)255, 0x100);
            m.ApplyPoison(this, Poison.Regular);
            m.SendLocalizedMessage(1070821, this.Name); // %s spits a poisonous substance at you!
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