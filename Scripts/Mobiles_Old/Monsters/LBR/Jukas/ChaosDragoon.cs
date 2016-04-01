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
            this.Name = "a chaos dragoon";
            this.Body = 0x190;
            this.Hue = Utility.RandomSkinHue();

            this.SetStr(176, 225);
            this.SetDex(81, 95);
            this.SetInt(61, 85);

            this.SetHits(176, 225);

            this.SetDamage(24, 26);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Cold, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            //SetResistance( ResistanceType.Physical, 25, 38 );
            //SetResistance( ResistanceType.Fire, 25, 38 );
            //SetResistance( ResistanceType.Cold, 25, 38 );
            //SetResistance( ResistanceType.Poison, 25, 38 );
            //SetResistance( ResistanceType.Energy, 25, 38 );

            this.SetSkill(SkillName.Fencing, 77.6, 92.5);
            this.SetSkill(SkillName.Healing, 60.3, 90.0);
            this.SetSkill(SkillName.Macing, 77.6, 92.5);
            this.SetSkill(SkillName.Anatomy, 77.6, 87.5);
            this.SetSkill(SkillName.MagicResist, 77.6, 97.5);
            this.SetSkill(SkillName.Swords, 77.6, 92.5);
            this.SetSkill(SkillName.Tactics, 77.6, 87.5);

            this.Fame = 5000;
            this.Karma = -5000;

            CraftResource res = CraftResource.None;

            switch (Utility.Random(6))
            {
                case 0:
                    res = CraftResource.BlackScales;
                    break;
                case 1:
                    res = CraftResource.RedScales;
                    break;
                case 2:
                    res = CraftResource.BlueScales;
                    break;
                case 3:
                    res = CraftResource.YellowScales;
                    break;
                case 4:
                    res = CraftResource.GreenScales;
                    break;
                case 5:
                    res = CraftResource.WhiteScales;
                    break;
            }

            BaseWeapon melee = null;

            switch (Utility.Random(3))
            {
                case 0:
                    melee = new Kryss();
                    break;
                case 1:
                    melee = new Broadsword();
                    break;
                case 2:
                    melee = new Katana();
                    break;
            }

            melee.Movable = false;
            this.AddItem(melee);

            DragonHelm helm = new DragonHelm();
            helm.Resource = res;
            helm.Movable = false;
            this.AddItem(helm);

            DragonChest chest = new DragonChest();
            chest.Resource = res;
            chest.Movable = false;
            this.AddItem(chest);

            DragonArms arms = new DragonArms();
            arms.Resource = res;
            arms.Movable = false;
            this.AddItem(arms);

            DragonGloves gloves = new DragonGloves();
            gloves.Resource = res;
            gloves.Movable = false;
            this.AddItem(gloves);

            DragonLegs legs = new DragonLegs();
            legs.Resource = res;
            legs.Movable = false;
            this.AddItem(legs);

            ChaosShield shield = new ChaosShield();
            shield.Movable = false;
            this.AddItem(shield);

            this.AddItem(new Shirt());
            this.AddItem(new Boots());

            int amount = Utility.RandomMinMax(1, 3);

            switch ( res )
            {
                case CraftResource.BlackScales:
                    this.AddItem(new BlackScales(amount));
                    break;
                case CraftResource.RedScales:
                    this.AddItem(new RedScales(amount));
                    break;
                case CraftResource.BlueScales:
                    this.AddItem(new BlueScales(amount));
                    break;
                case CraftResource.YellowScales:
                    this.AddItem(new YellowScales(amount));
                    break;
                case CraftResource.GreenScales:
                    this.AddItem(new GreenScales(amount));
                    break;
                case CraftResource.WhiteScales:
                    this.AddItem(new WhiteScales(amount));
                    break;
            }

            new SwampDragon().Rider = this;
        }

        public ChaosDragoon(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
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
            this.AddLoot(LootPack.Rich);
            //AddLoot( LootPack.Gems );	
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}