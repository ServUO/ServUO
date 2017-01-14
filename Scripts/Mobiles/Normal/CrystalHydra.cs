using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal hydra corpse")]
    public class CrystalHydra : BaseCreature
    {
        [Constructable]
        public CrystalHydra()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a crystal hydra";
            this.Body = 0x109;
            this.Hue = 0x47E;
            this.BaseSoundID = 0x16A;

            this.SetStr(800, 830);
            this.SetDex(100, 120);
            this.SetInt(100, 120);

            this.SetHits(1450, 1500);

            this.SetDamage(21, 26);

            this.SetDamageType(ResistanceType.Physical, 5);
            this.SetDamageType(ResistanceType.Fire, 5);
            this.SetDamageType(ResistanceType.Cold, 80);
            this.SetDamageType(ResistanceType.Poison, 5);
            this.SetDamageType(ResistanceType.Energy, 5);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 80, 100);
            this.SetResistance(ResistanceType.Poison, 35, 45);
            this.SetResistance(ResistanceType.Energy, 80, 100);

            this.SetSkill(SkillName.Wrestling, 100.0, 120.0);
            this.SetSkill(SkillName.Tactics, 100.0, 110.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 100.0);
            this.SetSkill(SkillName.Anatomy, 70.0, 80.0);
			
            this.Fame = 17000;
            this.Karma = -17000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }
		
        public CrystalHydra(Serial serial)
            : base(serial)
        {
        }
		
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
            this.AddLoot(LootPack.HighScrolls);
            this.AddLoot(LootPack.Parrot);
        }
		
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            if (Utility.RandomDouble() < 0.25)
                c.DropItem(new ShatteredCrystals());
				
            c.DropItem(new CrystallineFragments());
        }
		
        #region Breath
        public override double BreathDamageScalar
        {
            get
            {
                return 0.13;
            }
        }
        public override int BreathRange
        {
            get
            {
                return 5;
            }
        }
        public override int BreathFireDamage
        {
            get
            {
                return 0;
            }
        }
        public override int BreathColdDamage
        {
            get
            {
                return 100;
            }
        }
        public override int BreathEffectHue
        {
            get
            {
                return 0x47E;
            }
        }
        public override int BreathEffectSound
        {
            get
            {
                return 0x56D;
            }
        }
        public override double BreathMinDelay
        {
            get
            {
                return 5.0;
            }
        }
        public override double BreathMaxDelay
        {
            get
            {
                return 7.0;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }
		
        public override void BreathStart(IDamageable target)
        { 
            this.BreathStallMovement();
            this.BreathPlayAngerSound();
            this.BreathPlayAngerAnimation();
						
            this.Direction = this.GetDirectionTo(target);
			
            int count = 0;

            IPooledEnumerable eable = this.GetMobilesInRange(this.BreathRange);

            foreach (Mobile m in eable)
            {
                if (count++ > 3)
                    break;
					
                if (m != null && m != target && m.Alive && !m.IsDeadBondedPet && this.CanBeHarmful(m) && m.Map == this.Map && !this.IsDeadBondedPet && m.InRange(this, this.BreathRange) && this.InLOS(m) && !this.BardPacified)
                    Timer.DelayCall(TimeSpan.FromSeconds(this.BreathEffectDelay), new TimerStateCallback(BreathEffect_Callback), m);
            }

            eable.Free();

            Timer.DelayCall(TimeSpan.FromSeconds(this.BreathEffectDelay), new TimerStateCallback(BreathEffect_Callback), target);
        }

        #endregion
		
        public override int Hides
        {
            get
            {
                return 40;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
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
    }
}