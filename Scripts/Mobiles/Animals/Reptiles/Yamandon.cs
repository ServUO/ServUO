using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.Yamadon")]
    [CorpseName("a yamandon corpse")]
    public class Yamandon : BaseCreature
    {
        [Constructable]
        public Yamandon()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a yamandon";
            this.Body = 249;

            this.SetStr(786, 930);
            this.SetDex(251, 365);
            this.SetInt(101, 115);

            this.SetHits(1601, 1800);

            this.SetDamage(19, 35);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 10);

            this.SetResistance(ResistanceType.Physical, 65, 85);
            this.SetResistance(ResistanceType.Fire, 70, 90);
            this.SetResistance(ResistanceType.Cold, 50, 70);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 50, 70);

            this.SetSkill(SkillName.Anatomy, 115.1, 130.0);
            this.SetSkill(SkillName.MagicResist, 117.6, 132.5);
            this.SetSkill(SkillName.Poisoning, 120.1, 140.0);
            this.SetSkill(SkillName.Tactics, 117.1, 132.0);
            this.SetSkill(SkillName.Wrestling, 112.6, 132.5);

            this.Fame = 22000;
            this.Karma = -22000;

            if (Utility.RandomDouble() < .50)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());

            this.PackItem(new Eggs(2));
        }

        public Yamandon(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Utility.RandomBool() ? Poison.Deadly : Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.DoubleStrike;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich);
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.Gems, 6);
        }

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            this.DoCounter(attacker);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            this.DoCounter(attacker);
        }

        public override int GetAttackSound()
        {
            return 1260;
        }

        public override int GetAngerSound()
        {
            return 1262;
        }

        public override int GetDeathSound()
        {
            return 1259; //Other Death sound is 1258... One for Yamadon, one for Serado?
        }

        public override int GetHurtSound()
        {
            return 1263;
        }

        public override int GetIdleSound()
        {
            return 1261;
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

        private void DoCounter(Mobile attacker)
        {
            if (this.Map == null)
                return;

            if (attacker is BaseCreature && ((BaseCreature)attacker).BardProvoked)
                return;

            if (0.2 > Utility.RandomDouble())
            {
                /* Counterattack with Hit Poison Area
                * 20-25 damage, unresistable
                * Lethal poison, 100% of the time
                * Particle effect: Type: "2" From: "0x4061A107" To: "0x0" ItemId: "0x36BD" ItemIdName: "explosion" FromLocation: "(296 615, 17)" ToLocation: "(296 615, 17)" Speed: "1" Duration: "10" FixedDirection: "True" Explode: "False" Hue: "0xA6" RenderMode: "0x0" Effect: "0x1F78" ExplodeEffect: "0x1" ExplodeSound: "0x0" Serial: "0x4061A107" Layer: "255" Unknown: "0x0"
                * Doesn't work on provoked monsters
                */
                Mobile target = null;

                if (attacker is BaseCreature)
                {
                    Mobile m = ((BaseCreature)attacker).GetMaster();
					
                    if (m != null)
                        target = m;
                }

                if (target == null || !target.InRange(this, 18))
                    target = attacker;

                this.Animate(10, 4, 1, true, false, 0);

                ArrayList targets = new ArrayList();

                foreach (Mobile m in target.GetMobilesInRange(8))
                {
                    if (m == this || !this.CanBeHarmful(m))
                        continue;

                    if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                        targets.Add(m);
                    else if (m.Player && m.Alive)
                        targets.Add(m);
                }

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = (Mobile)targets[i];

                    this.DoHarmful(m);

                    AOS.Damage(m, this, Utility.RandomMinMax(20, 25), true, 0, 0, 0, 100, 0);

                    m.FixedParticles(0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer)255);
                    m.ApplyPoison(this, Poison.Lethal);
                }
            }
        }
    }
}