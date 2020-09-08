namespace Server.Mobiles
{
    [CorpseName("a drake corpse")]
    public class ColdDrake : BaseCreature, IAuraCreature
    {
        [Constructable]
        public ColdDrake() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a cold drake";
            Body = Utility.RandomList(60, 61);
            BaseSoundID = 362;

            Hue = Utility.RandomMinMax(1319, 1327);

            SetStr(610, 670);
            SetDex(130, 160);
            SetInt(150, 190);

            SetHits(450, 500);

            SetDamage(17, 20);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 50, 65);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 75, 90);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 95.0, 110.0);
            SetSkill(SkillName.Tactics, 115.0, 140.0);
            SetSkill(SkillName.Wrestling, 115.0, 126.0);
            SetSkill(SkillName.Parry, 70.0, 80.0);
            SetSkill(SkillName.DetectHidden, 40.0, 50.0);

            Fame = 12000;
            Karma = -12000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 96.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
            SetAreaEffect(AreaEffect.AuraDamage);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.MageryRegs, 3);

            if (Utility.RandomBool())
            {
                AddLoot(LootPack.NecroScrolls, 2);
            }
            else
            {
                AddLoot(LootPack.MageryScrolls, 2);
            }
        }

        public override bool CanAngerOnTame => true;
        public override bool ReacquireOnMovement => !Controlled;
        public override int TreasureMapLevel => 3;
        public override int Meat => 10;
        public override int Hides => 22;
        public override HideType HideType => HideType.Horned;
        public override int DragonBlood => 8;
        public override FoodType FavoriteFood => FoodType.Fish;

        public virtual void AuraEffect(Mobile m)
        {
            m.FixedParticles(0x374A, 10, 30, 5052, Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x5C6);

            m.SendLocalizedMessage(1008111, false, Name); //  : The intense cold is damaging you!
        }

        public ColdDrake(Serial serial) : base(serial)
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
