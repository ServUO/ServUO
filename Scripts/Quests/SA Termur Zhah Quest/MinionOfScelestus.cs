using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a minion of Scelestus corpse")]
    public class MinionOfScelestus : BaseCreature
    {
        [Constructable]
        public MinionOfScelestus()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a minion of Scelestus";
            this.Body = 40;
            this.BaseSoundID = 357;
            this.Hue = 0;

            this.SetStr(377, 405);
            this.SetDex(176, 195);
            this.SetInt(201, 225);

            this.SetHits(30,000);

            this.SetDamage(19, 21);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 50);
            this.SetDamageType(ResistanceType.Energy, 10);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 50);
            this.SetResistance(ResistanceType.Cold, 100);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 50);

            this.SetSkill(SkillName.Poisoning, 120.0);
            this.SetSkill(SkillName.EvalInt, 100.3, 109.4);
            this.SetSkill(SkillName.Magery, 115.3, 125.0);
            //this.SetSkill(SkillName.Meditation, 25.1, 50.0);
            this.SetSkill(SkillName.MagicResist, 130.8, 139.9);
            this.SetSkill(SkillName.Tactics, 110.2, 119.5);
            this.SetSkill(SkillName.Wrestling, 101.4, 119.8);

            this.Fame = 12500;
            this.Karma = -12500;

            this.VirtualArmor = 90;

            this.PackItem(new Longsword());

        }

        public MinionOfScelestus(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }

        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
       /* public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }*/
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
            AddLoot(LootPack.Meager);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

          
           // if (0.25 > Utility.RandomDouble())
           // {
                switch (Utility.Random(10))
                {
                    case 0: c.DropItem(new TheChallengeRite()); break;
                    case 1: c.DropItem(new AthenaeumDecree()); break;
                    case 2: c.DropItem(new ALetterFromTheKing()); break;
                    case 3: c.DropItem(new OnTheVoid()); break;
                    case 4: c.DropItem(new ShilaxrinarsMemorial()); break;
                    case 5: c.DropItem(new ToTheHighScholar()); break;
                    case 6: c.DropItem(new ToTheHighBroodmother()); break;
                    case 7: c.DropItem(new ReplyToTheHighScholar()); break;
                    case 8: c.DropItem(new AccessToTheIsle()); break;
                    case 9: c.DropItem(new InMemory()); break;
                }
            }
     //   }

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