using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a chaos dragoon corpse")]
    public class ChaosDragoon : BaseCreature
    {
        [Constructable]
        public ChaosDragoon()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4)
        {
            Name = "a chaos dragoon";
            Body = 0x190;
            Hue = Utility.RandomSkinHue();

            SetStr(176, 225);
            SetDex(81, 95);
            SetInt(61, 85);

            SetHits(176, 225);

            SetDamage(24, 26);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 25);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetSkill(SkillName.Fencing, 77.6, 92.5);
            SetSkill(SkillName.Healing, 60.3, 90.0);
            SetSkill(SkillName.Macing, 77.6, 92.5);
            SetSkill(SkillName.Anatomy, 77.6, 87.5);
            SetSkill(SkillName.MagicResist, 77.6, 97.5);
            SetSkill(SkillName.Swords, 77.6, 92.5);
            SetSkill(SkillName.Tactics, 77.6, 87.5);

            Fame = 5000;
            Karma = -5000;

			CraftResource res = (CraftResource)Utility.RandomMinMax(201, 206); // All normal scales

            SetWearable((Item)Activator.CreateInstance(Utility.RandomList(_WeaponsList)));
			SetWearable(new DragonHelm() { Resource = res });
			SetWearable(new DragonChest() { Resource = res });
			SetWearable(new DragonArms() { Resource = res });
			SetWearable(new DragonGloves() { Resource = res });
			SetWearable(new DragonLegs() { Resource = res });
			SetWearable(new ChaosShield());
			SetWearable(new Shirt(), dropChance: 1);
			SetWearable(new Boots(), dropChance: 1);

            int amount = Utility.RandomMinMax(1, 3);

            switch (res)
            {
                case CraftResource.BlackScales:
					AddItem(new BlackScales(amount));
                    break;
                case CraftResource.RedScales:
                    AddItem(new RedScales(amount));
                    break;
                case CraftResource.BlueScales:
                    AddItem(new BlueScales(amount));
                    break;
                case CraftResource.YellowScales:
                    AddItem(new YellowScales(amount));
                    break;
                case CraftResource.GreenScales:
                    AddItem(new GreenScales(amount));
                    break;
                case CraftResource.WhiteScales:
                    AddItem(new WhiteScales(amount));
                    break;
            }

            new SwampDragon().Rider = this;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public ChaosDragoon(Serial serial)
            : base(serial)
        {
        }

		private static readonly Type[] _WeaponsList = new Type[]
		{
			typeof(Kryss), typeof(Broadsword), typeof(Katana)
		};

        public override bool AutoDispel => true;
        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;
        public override bool ShowFameTitle => false;
        public override int GetIdleSound()
        {
            return 0x2CE;
        }

        public override int GetDeathSound()
        {
            return 0x2CC;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override int GetAttackSound()
        {
            return 0x2C8;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if (mount != null)
                mount.Rider = null;

            return base.OnBeforeDeath();
        }

        public override void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            if (to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Hiryu || to is LesserHiryu || to is Daemon)
                damage *= 3;
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
