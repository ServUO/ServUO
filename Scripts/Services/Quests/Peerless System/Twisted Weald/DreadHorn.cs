using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a dread horns corpse")]	
    public class DreadHorn : BasePeerless
    { 
        private static Dictionary<Mobile, bool> m_Affected;
        private DateTime m_Change;
        private DateTime m_Stomp;
        [Constructable]
        public DreadHorn()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Dread Horn";
            this.Body = 257;
            this.BaseSoundID = 0xA8;

            this.SetStr(812, 999);
            this.SetDex(507, 669);
            this.SetInt(1206, 1389);

            this.SetHits(50000);
            this.SetStam(507, 669);
            this.SetMana(1206, 1389);

            this.SetDamage(27, 31);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Poison, 60);

            this.SetResistance(ResistanceType.Physical, 40, 53);
            this.SetResistance(ResistanceType.Fire, 50, 63);
            this.SetResistance(ResistanceType.Cold, 50, 62);
            this.SetResistance(ResistanceType.Poison, 67, 73);
            this.SetResistance(ResistanceType.Energy, 60, 73);

            this.SetSkill(SkillName.Wrestling, 90.0);
            this.SetSkill(SkillName.Tactics, 90.0);
            this.SetSkill(SkillName.MagicResist, 110.0);
            this.SetSkill(SkillName.Poisoning, 120.0);
            this.SetSkill(SkillName.Magery, 110.0);
            this.SetSkill(SkillName.EvalInt, 110.0);
            this.SetSkill(SkillName.Meditation, 110.0);

            this.Fame = 15000;  //Guessing here
            this.Karma = -15000;  //Guessing here
            
            this.PackArcaneScroll(1, 3);
			
            this.PackResources(8);
            this.PackTalismans(5);	
			
            this.m_Change = DateTime.UtcNow;
            this.m_Stomp = DateTime.UtcNow;
        }

        public DreadHorn(Serial serial)
            : base(serial)
        {
        }

        public virtual int StrikingRange
        {
            get
            {
                return 12;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Regular;
            }
        }
        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Ribs;
            }
        }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
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
                return Poison.Deadly;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public static bool IsUnderInfluence(Mobile mobile)
        {
            if (m_Affected != null)
            {
                if (m_Affected.ContainsKey(mobile))
                    return true;
            }
			
            return false;
        }

        public static bool Affect(Mobile mobile)
        {
            if (m_Affected == null)
                m_Affected = new Dictionary<Mobile, bool>();
				
            if (!m_Affected.ContainsKey(mobile))
            {
                m_Affected.Add(mobile, true);
                return true;
            }
			
            return false;
        }

        public static void StopAffecting()
        {
            if (m_Affected != null)
                m_Affected.Clear();
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 8);
        }

        public override void OnThink()
        {
            base.OnThink();
			
            if (this.Combatant != null)
            {
                if (this.m_Change < DateTime.UtcNow && Utility.RandomDouble() < 0.2)
                    this.ChangeOpponent();					
				
                if (this.m_Stomp < DateTime.UtcNow && Utility.RandomDouble() < 0.1)
                    this.HoofStomp();
            }
            // exit ilsh 1313, 936, 32
        }

        public override void Damage(int amount, Mobile from)
        {
            base.Damage(amount, from);
						
            if (this.Combatant == null || this.Hits > this.HitsMax * 0.05 || Utility.RandomDouble() > 0.1)
                return;	
							
            new InvisibilitySpell(this, null).Cast();
			
            Target target = this.Target;
			
            if (target != null)
                target.Invoke(this, this);
				
            Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerCallback(Teleport));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new DreadHornMane());	
			
            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new TaintedMushroom());
			
            if (Utility.RandomDouble() < 0.6)				
                c.DropItem(new ParrotItem());
				
            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new MangledHeadOfDreadhorn());
				
            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new HornOfTheDreadhorn());
				
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new PristineDreadHorn());
				
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new DreadFlute());
				
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new DreadsRevenge());
				
            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());
				
            if (Utility.RandomDouble() < 0.05)
            {
                switch ( Utility.Random(4) )
                {
                    case 0:
                        c.DropItem(new LeafweaveLegs());
                        break;
                    case 1:
                        c.DropItem(new DeathChest());
                        break;
                    case 2:
                        c.DropItem(new AssassinLegs());
                        break;
                    case 3:
                        c.DropItem(new Feathernock());
                        break;
                }
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
			
            this.m_Change = DateTime.UtcNow;
            this.m_Stomp = DateTime.UtcNow;
        }

        public void Teleport()
        { 
            // 20 tries to teleport
            for (int tries = 0; tries < 20; tries ++)
            {
                int x = Utility.RandomMinMax(5, 7); 
                int y = Utility.RandomMinMax(5, 7);
				
                if (Utility.RandomBool())
                    x *= -1;
					
                if (Utility.RandomBool())
                    y *= -1;
				
                Point3D p = new Point3D(this.X + x, this.Y + y, 0);
                IPoint3D po = new LandTarget(p, this.Map)as IPoint3D;
				
                if (po == null)
                    continue;
					
                SpellHelper.GetSurfaceTop(ref po);

                if (this.InRange(p, 12) && this.InLOS(p) && this.Map.CanSpawnMobile(po.X, po.Y, po.Z))
                { 
                    Point3D from = this.Location;
                    Point3D to = new Point3D(po);
	
                    this.Location = to;
                    this.ProcessDelta();
					
                    this.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                    this.PlaySound(0x1FE);
										
                    break;					
                }
            }
			
            this.RevealingAction();
        }

        public void ChangeOpponent()
        {
            Mobile agro, best = null;
            double distance, random = Utility.RandomDouble();
			
            if (random < 0.75)
            { 
                // find random target relatively close
                for (int i = 0; i < this.Aggressors.Count && best == null; i ++)
                {
                    agro = this.Validate(this.Aggressors[i].Attacker);
					
                    if (agro == null)
                        continue;				
				
                    distance = this.StrikingRange - this.GetDistanceToSqrt(agro);
					
                    if (distance > 0 && distance < this.StrikingRange - 2 && this.InLOS(agro.Location))
                    {
                        distance /= this.StrikingRange;
						
                        if (random < distance)
                            best = agro;
                    }
                }
            }
            else
            {
                int damage = 0;
				
                // find a player who dealt most damage
                for (int i = 0; i < this.DamageEntries.Count; i ++)
                {
                    agro = this.Validate(this.DamageEntries[i].Damager);
					
                    if (agro == null)
                        continue;
					
                    distance = this.GetDistanceToSqrt(agro);
						
                    if (distance < this.StrikingRange && this.DamageEntries[i].DamageGiven > damage && this.InLOS(agro.Location))
                    {
                        best = agro;
                        damage = this.DamageEntries[i].DamageGiven;
                    }
                }
            }
			
            if (best != null)
            {
                // teleport
                best.Location = BasePeerless.GetSpawnPosition(this.Location, this.Map, 1);
                best.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                best.PlaySound(0x1FE);
				
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(delegate()
                {
                    // poison
                    best.ApplyPoison(this, this.HitPoison);
                    best.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                    best.PlaySound(0x474);
                }));
				
                this.m_Change = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));
            }
        }

        public void HoofStomp()
        { 
            foreach (Mobile m in this.GetMobilesInRange(this.StrikingRange))
            {
                Mobile valid = this.Validate(m);
				
                if (valid != null && Affect(valid))
                    valid.SendLocalizedMessage(1075081); // *Dreadhorn’s eyes light up, his mouth almost a grin, as he slams one hoof to the ground!*
            }
			
            // earthquake
            this.PlaySound(0x2F3);
				
            Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerCallback(delegate { StopAffecting(); }));
						
            this.m_Stomp = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(40, 50));
        }

        public Mobile Validate(Mobile m)
        { 
            Mobile agro;
					
            if (m is BaseCreature)
                agro = ((BaseCreature)m).ControlMaster;
            else
                agro = m;
			
            if (!this.CanBeHarmful(agro, false) || !agro.Player || this.Combatant == agro)
                return null;	
			
            return agro;
        }
    }
}