using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class FrostDragon : BaseCreature, IAuraCreature
    {
        [Constructable]
        public FrostDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frost dragon";
            Body = Utility.RandomList(12, 59);
            BaseSoundID = 362;

            Hue = Utility.RandomMinMax(1319, 1327);

            SetStr(1300, 1400);
            SetDex(100, 125);
            SetInt(600, 700);

            SetHits(2050, 2250);

            SetDamage(24, 33);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 85, 92);
            SetResistance(ResistanceType.Fire, 55, 70);
            SetResistance(ResistanceType.Cold, 85, 95);
            SetResistance(ResistanceType.Poison, 65, 70);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 50, 60);
            SetSkill(SkillName.Magery, 120, 130);
            SetSkill(SkillName.MagicResist, 115, 135);
            SetSkill(SkillName.Tactics, 120, 135);
            SetSkill(SkillName.Wrestling, 120, 130);
            SetSkill(SkillName.Meditation, 1, 50);

            Fame = 25000;
            Karma = -25000;

            Tamable = true;
            ControlSlots = 5;
            MinTameSkill = 105.0;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.DragonBreath);
            SetAreaEffect(AreaEffect.AuraDamage);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.Gems, 8);
        }

        public override void OnAfterTame(Mobile tamer)
        {
            Title = null;

            base.OnAfterTame(tamer);
        }

        public override bool CanAngerOnTame => true;
        public override bool StatLossAfterTame => true;
        public override bool ReacquireOnMovement => !Controlled;
        public override bool AutoDispel => !Controlled;
        public override int TreasureMapLevel => 4;
        public override int Meat => 19;
        public override int Hides => 33;
        public override HideType HideType => HideType.Barbed;
        public override int DragonBlood => 8;
        public override FoodType FavoriteFood => FoodType.Meat;

        public void AuraEffect(Mobile m)
        {
            m.FixedParticles(0x374A, 10, 30, 5052, Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x5C6);

            m.SendLocalizedMessage(1008111); //  : The intense cold is damaging you!
        }

        public FrostDragon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
