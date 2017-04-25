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
            this.Name = "a cold drake";
            this.Body = Utility.RandomList(60, 61);
            this.BaseSoundID = 362;

            this.Hue = Utility.RandomMinMax(1319, 1327);

            this.SetStr(610, 670);
            this.SetDex(130, 160);
            this.SetInt(150, 190);

            this.SetHits(450, 500);

            this.SetDamage(17, 20);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 50, 65);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 75, 90);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 95.0, 110.0);
            this.SetSkill(SkillName.Tactics, 115.0, 140.0);
            this.SetSkill(SkillName.Wrestling, 115.0, 126.0);
            this.SetSkill(SkillName.Parry, 70.0, 80.0);
            this.SetSkill(SkillName.DetectHidden, 40.0, 50.0);

            this.Fame = 12000;
            this.Karma = -12000;

            this.VirtualArmor = 60;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 96.0;

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
        public override bool ReacquireOnMovement { get { return !this.Controlled; } }
        public override int Meat { get { return 10; } }
        public override int Hides { get { return 22; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int DragonBlood { get { return 8; } }
        public override FoodType FavoriteFood { get { return FoodType.Fish; } }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathEffectHue { get { return 1264; } }

        public override bool HasAura { get { return !this.Controlled; } }
        public override int AuraRange { get { return 2; } }
        public override int AuraBaseDamage { get { return 20; } }
        public override int AuraFireDamage { get { return 0; } }
        public override int AuraColdDamage { get { return 100; } }

        public override void AuraEffect(Mobile m)
        {
            m.SendLocalizedMessage(1008111, false, this.Name); //  : The intense cold is damaging you!
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