using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a chief paroxysmus corpse")]
    public class ChiefParoxysmus : BasePeerless
    {
        [Constructable]
        public ChiefParoxysmus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a chief paroxysmus";
            this.Body = 0x100;

            this.SetStr(1232, 1400);
            this.SetDex(76, 82);
            this.SetInt(76, 85);

            this.SetHits(50000);

            this.SetDamage(27, 31);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 75, 85);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 50, 60);
			
            this.SetSkill(SkillName.Wrestling, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0);
            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Anatomy, 120.0);
            this.SetSkill(SkillName.Poisoning, 120.0);
			
            this.SpawnBulbous();
			
            this.PackResources(8);
            this.PackTalismans(5);
            Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(SpawnBulbous));  //BulbousPutrification
        }

        public ChiefParoxysmus(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact
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
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override bool CanAreaPoison
        {
            get
            {
                return true;
            }
        }
        public override Poison HitAreaPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 8);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new LardOfParoxysmus());
			
            switch ( Utility.Random(3) )
            {
                case 0:
                    c.DropItem(new ParoxysmusDinner());
                    break;
                case 1:
                    c.DropItem(new ParoxysmusCorrodedStein());
                    break;
                case 2:
                    c.DropItem(new StringOfPartsOfParoxysmusVictims());
                    break;
            }
			
            if (Utility.RandomDouble() < 0.6)				
                c.DropItem(new ParrotItem());
			
            if (Utility.RandomBool())
                c.DropItem(new SweatOfParoxysmus());
				
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new ParoxysmusSwampDragonStatuette());
				
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new ScepterOfTheChief());
				
            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());
        }

        public override int GetDeathSound()
        {
            return 0x56F;
        }

        public override int GetAttackSound()
        {
            return 0x570;
        }

        public override int GetIdleSound()
        {
            return 0x571;
        }

        public override int GetAngerSound()
        {
            return 0x572;
        }

        public override int GetHurtSound()
        {
            return 0x573;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
			
            // eats pet or summons
            if (from is BaseCreature)
            {
                BaseCreature creature = (BaseCreature)from;
				
                if (creature.Controlled || creature.Summoned)
                {
                    this.Heal(creature.Hits);					
                    creature.Kill();				
					
                    Effects.PlaySound(this.Location, this.Map, 0x574);
                }
            }
			
            // teleports player near
            if (from is PlayerMobile && !this.InRange(from.Location, 1))
            {
                this.Combatant = from;
				
                from.MoveToWorld(this.GetSpawnPosition(1), this.Map);				
                from.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                from.PlaySound(0x1FE);
            }
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

        public virtual void SpawnBulbous()
        {
            for (int i = 0; i < 3; i++)
            {
                Mobile blobus = new BulbousPutrification();
                blobus.MoveToWorld(this.GetSpawnPosition(4), this.Map);
            }
        }
    }
}