using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shimmering effusion corpse")]
    public class ShimmeringEffusion : BasePeerless
    {
        [Constructable]
        public ShimmeringEffusion()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shimmering effusion";
            Body = 0x105;			

            SetStr(500, 550);
            SetDex(350, 400);
            SetInt(1500, 1600);

            SetHits(20000);

            SetDamage(27, 31);
			
            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);
			
            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 60, 80);
            SetResistance(ResistanceType.Cold, 60, 80);
            SetResistance(ResistanceType.Poison, 60, 80);
            SetResistance(ResistanceType.Energy, 60, 80);

            SetSkill(SkillName.Wrestling, 100.0, 105.0);
            SetSkill(SkillName.Tactics, 100.0, 105.0);
            SetSkill(SkillName.MagicResist, 150);
            SetSkill(SkillName.Magery, 150.0);
            SetSkill(SkillName.EvalInt, 150.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Spellweaving, 120.0);

            Fame = 30000;
            Karma = -30000;
			
            PackResources(8);
            PackTalismans(5);

            for (int i = 0; i < Utility.RandomMinMax(1, 6); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }
		
        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
            AddLoot(LootPack.Parrot, 2);
            AddLoot(LootPack.HighScrolls, 3);
            AddLoot(LootPack.MedScrolls, 3);
        }
		
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new CapturedEssence());
            c.DropItem(new ShimmeringCrystals());			
			
            if (Utility.RandomDouble() < 0.05)
            {
                switch ( Utility.Random(4) )
                {
                    case 0:
                        c.DropItem(new ShimmeringEffusionStatuette());
                        break;
                    case 1:
                        c.DropItem(new CorporealBrumeStatuette());
                        break;
                    case 2:
                        c.DropItem(new MantraEffervescenceStatuette());
                        break;
                    case 3:
                        c.DropItem(new FetidEssenceStatuette());
                        break;
                }
            }

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new HumanFeyLeggings());
			
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new FerretImprisonedInCrystal());		
						
            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrystallineRing());	
					
            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());
				
            if (Utility.RandomDouble() < 0.05)
            {
                switch ( Utility.Random(4) )
                {
                    case 0:
                        c.DropItem(new MalekisHonor());
                        break;
                    case 1:
                        c.DropItem(new Feathernock());
                        break;
                    case 2:
                        c.DropItem(new Swiftflight());
                        break;
                    case 3:
                        c.DropItem(new HunterGloves());
                        break;
                }
            }
        }
			
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override bool HasFireRing
        {
            get
            {
                return true;
            }
        }
        public override double FireRingChance
        {
            get
            {
                return 0.1;
            }
        }

        public override int GetIdleSound()
        {
            return 0x1BF;
        }

        public override int GetAttackSound()
        {
            return 0x1C0;
        }

        public override int GetHurtSound()
        {
            return 0x1C1;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }
		
        #region Helpers
        public override bool CanSpawnHelpers
        {
            get
            {
                return true;
            }
        }
        public override int MaxHelpersWaves
        {
            get
            {
                return 4;
            }
        }
        public override double SpawnHelpersChance
        {
            get
            {
                return 0.1;
            }
        }
		
        public override void SpawnHelpers()
        {
            int amount = 1;
		
            if (Altar != null)
                amount = Altar.Fighters.Count;
				
            if (amount > 5)
                amount = 5;
			
            for (int i = 0; i < amount; i ++)
            { 
                switch ( Utility.Random(3) )
                {
                    case 0:
                        SpawnHelper(new MantraEffervescence(), 2);
                        break;
                    case 1:
                        SpawnHelper(new CorporealBrume(), 2);
                        break;
                    case 2:
                        SpawnHelper(new FetidEssence(), 2);
                        break;
                }
            }
        }

        #endregion

        public ShimmeringEffusion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}