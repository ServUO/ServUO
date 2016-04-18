using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public class Serado : BaseChampion
    {
        [Constructable]
        public Serado()
            : base(AIType.AI_Melee)
        {
            this.Name = "Serado";
            this.Title = "the awakened";

            this.Body = 249;
            this.Hue = 0x96C;

            this.SetStr(1000);
            this.SetDex(150);
            this.SetInt(300);

            this.SetHits(9000);
            this.SetMana(300);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 10);

            this.SetResistance(ResistanceType.Physical, 30);
            this.SetResistance(ResistanceType.Fire, 60);
            this.SetResistance(ResistanceType.Cold, 60);
            this.SetResistance(ResistanceType.Poison, 90);
            this.SetResistance(ResistanceType.Energy, 50);

            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0);
            this.SetSkill(SkillName.Wrestling, 70.0);
            this.SetSkill(SkillName.Poisoning, 150.0);

            this.Fame = 22500;
            this.Karma = -22500;

            this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
        }

        public Serado(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Power;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(Pacify) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[]
                {
                    typeof(BraveKnightOfTheBritannia),
                    typeof(DetectiveBoots),
                    typeof(EmbroideredOakLeafCloak),
                    typeof(LieutenantOfTheBritannianRoyalGuard)
                };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(Futon), typeof(SwampTile) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { };
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override double HitPoisonChance
        {
            get
            {
                return 0.8;
            }
        }
        public override int Feathers
        {
            get
            {
                return 30;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.DoubleStrike;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 4);
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Gems, 6);
        }

        // TODO: Hit Lightning Area
        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            this.ScaleResistances();
            this.DoCounter(attacker);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            this.ScaleResistances();
            this.DoCounter(attacker);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private void ScaleResistances()
        {
            double hitsLost = (this.HitsMax - this.Hits) / (double)this.HitsMax;

            this.SetResistance(ResistanceType.Physical, 30 + (int)(hitsLost * (95 - 30)));
            this.SetResistance(ResistanceType.Fire, 60 + (int)(hitsLost * (95 - 60)));
            this.SetResistance(ResistanceType.Cold, 60 + (int)(hitsLost * (95 - 60)));
            this.SetResistance(ResistanceType.Poison, 90 + (int)(hitsLost * (95 - 90)));
            this.SetResistance(ResistanceType.Energy, 50 + (int)(hitsLost * (95 - 50)));
        }

        private void DoCounter(Mobile attacker)
        {
            if (this.Map == null || (attacker is BaseCreature && ((BaseCreature)attacker).BardProvoked))
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

                if (target == null || !target.InRange(this, 25))
                    target = attacker;

                this.Animate(10, 4, 1, true, false, 0);

                ArrayList targets = new ArrayList();

                foreach (Mobile m in target.GetMobilesInRange(8))
                {
                    if (m == this || !this.CanBeHarmful(m))
                        continue;

                    if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                        targets.Add(m);
                    else if (m.Player)
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