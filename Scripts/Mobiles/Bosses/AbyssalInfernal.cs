using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("an abyssal infernal corpse")]
    public class AbyssalInfernal : BaseChampion
    {
        private DateTime m_NextAbility;
		
        [Constructable]
        public AbyssalInfernal() : base(AIType.AI_Mage)
        {
            Body = 713;
            Name = "The Abyssal Infernal";

            SetStr(1200, 1300);
            SetDex(100, 125);
            SetInt(600, 700);

            SetHits(50000);
            SetStam(203, 650);

            SetDamage(11, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Anatomy, 110.0, 120.0);
            SetSkill(SkillName.Magery, 130.0, 140.0);
            SetSkill(SkillName.EvalInt, 120.0, 140.0);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 110.0, 120.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 40;

            m_NextMeteor = DateTime.UtcNow;
            m_NextCondemn = DateTime.UtcNow;
            m_NextMark = DateTime.UtcNow;
            m_NextSpawn = DateTime.UtcNow;
        }

        public AbyssalInfernal(Serial serial) : base(serial)
        {
        }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.None; } }
        public override Type[] UniqueList { get { return new Type[] { typeof(TongueOfTheBeast), typeof(DeathsHead), typeof(WallOfHungryMouths), typeof(AbyssalBlade) }; } }
        public override Type[] SharedList { get { return new Type[] { typeof(RoyalGuardInvestigatorsCloak), typeof( DetectiveBoots ), typeof(JadeArmband) }; } }
        public override Type[] DecorativeList { get { return new Type[] { typeof(MagicalDoor) }; } }
        public override MonsterStatuetteType[] StatueTypes { get { return new MonsterStatuetteType[] { MonsterStatuetteType.AbyssalInfernal, MonsterStatuetteType.ArchDemon }; } }
        
		public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
		
        public override ScaleType ScaleType { get { return ScaleType.All; } }
        public override int Scales { get { return 20; } }
		
        public override int GetIdleSound()
        {
            return 0x5D7;
        }

        public override int GetAngerSound()
        {
            return 0x5D4;
        }

        public override int GetHurtSound()
        {
            return 0x5D6;
        }

        public override int GetDeathSound()
        {
            return 0x5D5;
        }
		
        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
        }
		
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomBool())
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new HornAbyssalInferno());
                        break;
                    case 1:
                        c.DropItem(new NetherCycloneScroll());
                        break;
                }
            }
        }

        private DateTime m_NextMeteor;
        private DateTime m_NextCondemn;
        private DateTime m_NextMark;
        private DateTime m_NextSpawn;

		public override void OnThink()
		{
			base.OnThink();
			
			if(Combatant == null)
				return;

            if (m_NextMeteor < DateTime.UtcNow)
            {
                DoMeteor();
                m_NextMeteor = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.Random(20, 50));
            }
            else if (m_NextCondemn < DateTime.UtcNow)
            {
                DoCondemn();
                m_NextCondemn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.Random(20, 50));
            }
            else if (m_NextSpawn < DateTime.UtcNow)
            {
                DoSpawn();
                m_NextSpawn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.Random(20, 50));
            }
		}
		
		private Mobile GetRandomTarget(int range, bool playersOnly)
		{
            List<Mobile> list = GetTargets(range, playersOnly);
            Mobile m = null;

            if (list != null && list.Count > 0)
            {
                m = list[Utility.Random(list.Count)];
                ColUtility.Free(list);
            }

			return m;
		}
		
		private List<Mobile> GetTargets(int range, bool playersOnly)
		{
			List<Mobile> targets = new List<Mobile>();

            IPooledEnumerable eable = GetMobilesInRange(range);
            foreach(Mobile m in eable)
			{
				if(m == this || !CanBeHarmful(m))
					continue;
					
                if (!playersOnly && m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    targets.Add(m);
                else if (m.Player)
                    targets.Add(m);
			}
            eable.Free();
			
			return targets;
		}

        public override bool DrainsLife { get { return true; } }
        public override double DrainsLifeChance { get { return .25; } }

		#region Meteor
		public void DoMeteor()
		{
			Map map = this.Map;
			
			if(map != null)
			{
				Mobile focus = GetRandomTarget(8, true);
				
				if(focus != null)
				{
					StygianDragon.CrimsonMeteor(this, focus, 75, 100);
				}
			}
		}
		#endregion
		
		#region Condemn
        public void DoCondemn()
        {
			Map map = this.Map;
			
            if (map != null)
            {
				Mobile toCondemn = GetRandomTarget(12, true);
				
				if(toCondemn != null)
				{
					Point3D loc;
					
					switch (Utility.Random(5))
					{
						default:
						case 0: loc = new Point3D(6949, 701, 32); break;
						case 1: loc = new Point3D(6941, 761, 32); break;
						case 2: loc = new Point3D(7015, 688, 32); break;
						case 3: loc = new Point3D(7043, 751, 32); break;
						case 4: loc = new Point3D(6999, 798, 32); break;
					}

					toCondemn.SendMessage("You are being burned by the heat from the Lava!");
                    DoHarmful(toCondemn);
					AOS.Damage(toCondemn, Utility.RandomMinMax(75, 85), 0, 100, 0, 0, 0);
					toCondemn.MoveToWorld(loc, map);
					toCondemn.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
					toCondemn.PlaySound(0x1FE);
				}
            }
        }
		#endregion

		#region Spawn
        public void DoSpawn()
        {
            Map map = this.Map;

            if (map != null && Utility.RandomBool())
			{
                this.PlaySound(0x218);//////////

                int newFollowers = Utility.RandomMinMax(2, 5);
				Mobile focus = GetRandomTarget(10, false);

				if(focus != null)
				{
					for (int i = 0; i < newFollowers; ++i)
					{
						BaseCreature spawn;

						switch (Utility.Random(5))
						{
							default:
							case 0: spawn = new WaterElemental(); break;
							case 1: spawn = new EarthElemental(); break;
							case 2: spawn = new FireSteed(); break;
							case 3: spawn = new FireElemental(); break;
							case 4: spawn = new AirElemental(); break;
						}

						spawn.Team = this.Team;

						bool validLocation = false;
						Point3D loc = focus.Location;

						for (int j = 0; !validLocation && j < 10; ++j)
						{
							int x = Utility.RandomMinMax(focus.X - 1, focus.X + 1);
							int y = Utility.RandomMinMax(focus.Y - 1, focus.Y + 1);
							int z = map.GetAverageZ(x, y);

							if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
								loc = new Point3D(x, y, this.Z);
							else if (validLocation = map.CanFit(x, y, z, 16, false, false))
								loc = new Point3D(x, y, z);
						}

						spawn.MoveToWorld(loc, map);
						spawn.Combatant = focus;
					}
				}
            }
        }
		#endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextMeteor = DateTime.UtcNow;
            m_NextCondemn = DateTime.UtcNow;
            m_NextMark = DateTime.UtcNow;
            m_NextSpawn = DateTime.UtcNow;
        }
    }
}