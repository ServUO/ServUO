using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a chief paroxysmus corpse")]
    public class ChiefParoxysmus : BasePeerless
    {
        [Constructable]
        public ChiefParoxysmus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a chief paroxysmus";
            Body = 0x100;

            SetStr(1232, 1400);
            SetDex(76, 82);
            SetInt(76, 85);

            SetHits(50000);

            SetDamage(27, 31);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Anatomy, 120.0);
            SetSkill(SkillName.Poisoning, 120.0);

            Timer.DelayCall(TimeSpan.FromSeconds(1), SpawnBulbous);  //BulbousPutrification

            Fame = 25000;
            Karma = -25000;

            SetAreaEffect(AreaEffect.PoisonBreath);
        }

        public ChiefParoxysmus(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
            AddLoot(LootPack.PeerlessResource, 8);
            AddLoot(LootPack.Talisman, 5);
            AddLoot(LootPack.LootItem<LardOfParoxysmus>());
            AddLoot(LootPack.RandomLootItem(new[] { typeof(ParoxysmusDinner), typeof(ParoxysmusCorrodedStein), typeof(StringOfPartsOfParoxysmusVictims) }));
            AddLoot(LootPack.LootItem<ParrotItem>(60.0));
            AddLoot(LootPack.LootItem<SweatOfParoxysmus>(50.0));
            AddLoot(LootPack.LootItem<ParoxysmusSwampDragonStatuette>(5.0));
            AddLoot(LootPack.LootItem<ScepterOfTheChief>(5.0));
        }

        public override int GetDeathSound()
        {
            return 0x56F;
        }

        public override int GetAttackSound()
        {
            return 0x570;
        }

        public override int GetIdleSound()
        {
            return 0x571;
        }

        public override int GetAngerSound()
        {
            return 0x572;
        }

        public override int GetHurtSound()
        {
            return 0x573;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // eats pet or summons
            if (from is BaseCreature)
            {
                BaseCreature creature = (BaseCreature)from;

                if (creature.Controlled || creature.Summoned)
                {
                    if (Hits < HitsMax)
                        Hits = HitsMax;

                    creature.Kill();

                    Effects.PlaySound(Location, Map, 0x574);
                }
            }

            // teleports player near
            if (from is PlayerMobile && !InRange(from.Location, 1))
            {
                Combatant = from;

                from.MoveToWorld(GetSpawnPosition(1), Map);
                from.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                from.PlaySound(0x1FE);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        public virtual void SpawnBulbous()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnHelper(new BulbousPutrification(), GetSpawnPosition(4));
            }
        }
    }
}
