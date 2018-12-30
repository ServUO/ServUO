using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a drake corpse")]
    public class ColdDrake : BaseCreature
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

            VirtualArmor = 60;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 96.0;

            PackReg(3);

            for (int i = 0; i <= 1; i++)
            {
                Item item;

                if (Utility.RandomBool())
                    item = Loot.RandomScroll(0, Loot.NecromancyScrollTypes.Length, SpellbookType.Necromancer);
                else
                    item = Loot.RandomScroll(0, Loot.RegularScrollTypes.Length, SpellbookType.Regular);

                PackItem(item);
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override int Meat { get { return 10; } }
        public override int Hides { get { return 22; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int DragonBlood { get { return 8; } }
        public override FoodType FavoriteFood { get { return FoodType.Fish; } }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathEffectHue { get { return 1264; } }

        public override bool HasAura { get { return !Controlled; } }
        public override int AuraRange { get { return 2; } }
        public override int AuraBaseDamage { get { return 20; } }
        public override int AuraFireDamage { get { return 0; } }
        public override int AuraColdDamage { get { return 100; } }

        public override void AuraEffect(Mobile m)
        {
            m.SendLocalizedMessage(1008111, false, Name); //  : The intense cold is damaging you!
        }

        public ColdDrake(Serial serial) : base(serial)
        {
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