#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
    [CorpseName("a fairy dragon corpse")]
    public class FairyDragon : BaseCreature
    {

        public override bool AutoDispel => !Controlled;
        public override int TreasureMapLevel => 3;
        public override int Meat => 9;
        public override Poison HitPoison => Poison.Greater;
        public override double HitPoisonChance => 0.75;
        public override FoodType FavoriteFood => FoodType.Meat;

        [Constructable]
        public FairyDragon()
            : base(AIType.AI_Mystic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "fairy dragon";
            Body = 718;
            BaseSoundID = 362;

            SetStr(512, 558);
            SetDex(95, 105);
            SetInt(455, 501);

            SetHits(398, 403);

            SetDamage(15, 18);

            SetDamageType(ResistanceType.Fire, 20, 25);
            SetDamageType(ResistanceType.Cold, 20, 25);
            SetDamageType(ResistanceType.Poison, 20, 25);
            SetDamageType(ResistanceType.Energy, 20, 25);

            SetResistance(ResistanceType.Physical, 16, 30);
            SetResistance(ResistanceType.Fire, 41, 44);
            SetResistance(ResistanceType.Cold, 40, 49);
            SetResistance(ResistanceType.Poison, 40, 49);
            SetResistance(ResistanceType.Energy, 45, 47);

            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 60.6, 68.2);
            SetSkill(SkillName.Wrestling, 90.1, 92.5);
            SetSkill(SkillName.Mysticism, 101.8, 108.3);

            Fame = 15000;
            Karma = -15000;
        }

        public FairyDragon(Serial serial)
            : base(serial)
        { }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {

            base.OnDeath(c);

            if (Utility.RandomDouble() <= 0.25)
            {
                c.DropItem(new FairyDragonWing());
            }

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new DraconicOrb());

            }
        }

        public override int GetAttackSound()
        {
            return 1513;
        }

        public override int GetAngerSound()
        {
            return 1558;
        }

        public override int GetDeathSound()
        {
            return 1514;
        }

        public override int GetHurtSound()
        {
            return 1515;
        }

        public override int GetIdleSound()
        {
            return 1516;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}