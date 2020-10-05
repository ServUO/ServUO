using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a kappa corpse")]
    public class Kappa : BaseCreature
    {
        [Constructable]
        public Kappa()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a kappa";
            Body = 240;

            SetStr(186, 230);
            SetDex(51, 75);
            SetInt(41, 55);

            SetMana(30);

            SetHits(151, 180);

            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 50);
            SetResistance(ResistanceType.Fire, 35, 50);
            SetResistance(ResistanceType.Cold, 25, 50);
            SetResistance(ResistanceType.Poison, 35, 50);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 60.1, 70.0);
            SetSkill(SkillName.Tactics, 79.1, 89.0);
            SetSkill(SkillName.Wrestling, 60.1, 70.0);

            Fame = 1700;
            Karma = -1700;

            SetSpecialAbility(SpecialAbility.LifeLeech);
        }

        public Kappa(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<RawFishSteak>(3, true));
            AddLoot(LootPack.RandomLootItem(new [] { typeof(Gears), typeof(Hinge), typeof(Axle) }, 50.0, 1));
            AddLoot(LootPack.PeculiarSeed2);
        }

        public override int GetAngerSound()
        {
            return 0x50B;
        }

        public override int GetIdleSound()
        {
            return 0x50A;
        }

        public override int GetAttackSound()
        {
            return 0x509;
        }

        public override int GetHurtSound()
        {
            return 0x50C;
        }

        public override int GetDeathSound()
        {
            return 0x508;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && from.Map != null)
            {
                int amt = 0;
                Mobile target = this;
                int rand = Utility.Random(1, 100);
                if (willKill)
                {
                    amt = (((rand % 5) >> 2) + 3);
                }
                if ((Hits < 100) && (rand < 21))
                {
                    target = (rand % 2) < 1 ? this : from;
                    amt++;
                }
                if (amt > 0)
                {
                    SpillAcid(target, amt);
                    from.SendLocalizedMessage(1070820);

                    if (Mana > 14)
                        Mana -= 15;
                }
            }
            base.OnDamage(amount, from, willKill);
        }

        public override Item NewHarmfulItem()
        {
            return new AcidSlime(TimeSpan.FromSeconds(10), 5, 10);
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
