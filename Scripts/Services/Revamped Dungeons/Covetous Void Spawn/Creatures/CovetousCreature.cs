using Server;
using System;
using Server.Items;

 namespace Server.Mobiles
 {
	public class CovetousCreature : BaseCreature
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool VoidSpawn { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Level { get; set; }
	
		public virtual int Stage { get { return Math.Max(1, Level / 5); } }
		public virtual int MaxStage { get { return 15; } }
		
		public virtual int StatRatio { get { return Utility.RandomMinMax(35, 60); } }

        public virtual double SkillStart { get { return Utility.RandomMinMax(35, 50); } }
        public virtual double SkillMax { get { return 160.0; } }

        public virtual int StrStart { get { return Utility.RandomMinMax(91, 100); } }
        public virtual int DexStart { get { return Utility.RandomMinMax(91, 100); } }
        public virtual int IntStart { get { return IsMagical ? Utility.RandomMinMax(91, 100) : 1; } }

        public virtual int StrMax { get { return 410; } }
        public virtual int DexMax { get { return 422; } }
        public virtual int IntMax { get { return 250; } }

        public virtual int MaxHits { get { return 2400; } }
        public virtual int MaxStam { get { return 3000; } }
        public virtual int MaxMana { get { return IsMagical ? 8500 : 1500; } }

        public virtual int MinDamMax { get { return 5; } }
        public virtual int MaxDamMax { get { return 12; } }

        public virtual int MinDamStart { get { return 5; } }
        public virtual int MaxDamStart { get { return 15; } }

        public virtual int HitsStart { get { return StrStart + (int)((double)StrStart * ((double)StatRatio / 100.0)); } }
        public virtual int StamStart { get { return DexStart + (int)((double)DexStart * ((double)StatRatio / 100.0)); } }
        public virtual int ManaStart { get { return IntStart + (int)((double)IntStart * ((double)StatRatio / 100.0)); } }

        public virtual bool RaiseDamage { get { return true; } }
        public virtual double RaiseDamageFactor { get { return 0.33; } }

        public virtual int ResistStart { get { return 25; } }
        public virtual int ResistMax { get { return 95; } }

        public virtual bool IsMagical { get { return AIObject is MageAI; } }

        public override bool GivesFameAndKarmaAward { get { return false; } }
        public override bool PlayerRangeSensitive { get { return false; } }
        public override bool CanDestroyObstacles { get { return true; } }

        private Tuple<WayPoint, DateTime> TimeOnWayPoint;

        public CovetousCreature(AIType ai, int level = 60, bool voidspawn = false)
            : base(ai, FightMode.Closest, 10, 1, 0.2, 0.1)
        {
            Level = level;
            VoidSpawn = voidspawn;

            SetSkill(SkillName.MagicResist, SkillStart);
            SetSkill(SkillName.Tactics, SkillStart);
            SetSkill(SkillName.Wrestling, SkillStart);
            SetSkill(SkillName.Anatomy, SkillStart);

            switch (ai)
            {
                default: break;
                case AIType.AI_Mage:
                    SetSkill(SkillName.Magery, SkillStart);
                    SetSkill(SkillName.EvalInt, SkillStart);
                    SetSkill(SkillName.Meditation, SkillStart);
                    break;
            }

            SetStr(StrStart);
            SetDex(DexStart);
            SetInt(IntStart);

            SetHits(HitsStart);
            SetStam(StamStart);
            SetMana(ManaStart);

            SetDamage(MinDamStart, MaxDamStart);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, ResistStart - 5, ResistStart + 5);
            SetResistance(ResistanceType.Fire, ResistStart - 5, ResistStart + 5);
            SetResistance(ResistanceType.Cold, ResistStart - 5, ResistStart + 5);
            SetResistance(ResistanceType.Poison, ResistStart - 5, ResistStart + 5);
            SetResistance(ResistanceType.Energy, ResistStart - 5, ResistStart + 5); 

            if (Stage > 1)
                Timer.DelayCall(TimeSpan.FromSeconds(.5), SetPower);

            Fame = Math.Min(8500, Level * 142);
            Karma = Math.Min(8500, Level * 142) * -1;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!Alive)
                return;

            if (TimeOnWayPoint == null && CurrentWayPoint != null)
            {
                TimeOnWayPoint = new Tuple<WayPoint, DateTime>(CurrentWayPoint, DateTime.UtcNow + TimeSpan.FromMinutes(2));
            }
            else if (TimeOnWayPoint != null && TimeOnWayPoint.Item1 == CurrentWayPoint && TimeOnWayPoint.Item2 < DateTime.UtcNow)
            {
                if (CheckCanTeleport())
                    MoveToWorld(CurrentWayPoint.Location, this.Map);
            }
            else if (TimeOnWayPoint != null && TimeOnWayPoint.Item1 != CurrentWayPoint)
            {
                TimeOnWayPoint = new Tuple<WayPoint, DateTime>(CurrentWayPoint, DateTime.UtcNow + TimeSpan.FromMinutes(2));
            }
        }

        protected virtual bool CheckCanTeleport()
        {
            if (CurrentWayPoint == null || Frozen || Paralyzed || (Combatant is Mobile && ((Mobile)Combatant).InLOS(this)))
                return false;

            bool canTeleport = true;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 10);

            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile && m.AccessLevel == AccessLevel.Player)
                {
                    canTeleport = false;
                    break;
                }
            }

            if (canTeleport)
            {
                eable = this.Map.GetItemsInRange(this.Location, 8);

                foreach (Item item in eable)
                {
                    int id = item.ItemID;

                    if (id == 0x82 || id == 0x3946 || id == 0x3956 || id == 0x3967 || id == 0x3979)
                    {
                        canTeleport = false;
                        break;
                    }
                }
            }

            eable.Free();

            return canTeleport;
        }

		public override void GenerateLoot()
        {
			if(!VoidSpawn)
				AddLoot(LootPack.Rich, Math.Max(1, Stage / 2));
        }
		
		public virtual void SetPower()
        {
            foreach (Skill skill in Skills)
            {
                if (skill != null && skill.Base > 0 && skill.Base < SkillMax)
                {
                    double toRaise = ((SkillMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5);

                    if (toRaise > skill.Base)
                        skill.Base = Math.Min(SkillMax, toRaise);
                }
            }

            SetResistance(ResistanceType.Physical, ((ResistMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5));
            SetResistance(ResistanceType.Fire, ((ResistMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5));
            SetResistance(ResistanceType.Cold, ((ResistMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5));
            SetResistance(ResistanceType.Poison, ((ResistMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5));
            SetResistance(ResistanceType.Energy, ((ResistMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5));

            int strRaise = ((StrMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5);
            int dexRaise = ((DexMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5);
            int intRaise = ((IntMax / MaxStage) * Stage) + Utility.RandomMinMax(-5, 5);

            if (strRaise > RawStr)
                SetStr(Math.Min(StrMax, strRaise));

            if (dexRaise > RawDex)
                SetDex(Math.Min(DexMax, dexRaise));

            if (intRaise > RawInt)
                SetInt(Math.Min(IntMax, intRaise));

            int hitsRaise = ((MaxHits / 60) * Level) + Utility.RandomMinMax(-5, 5);
            int stamRaise = ((MaxStam / 60) * Level) + Utility.RandomMinMax(-5, 5);
            int manaRaise = ((MaxMana / 60) * Level) + Utility.RandomMinMax(-5, 5);

            if (hitsRaise > HitsMax)
                SetHits(Math.Min(MaxHits, hitsRaise));

            if (stamRaise > StamMax)
                SetStam(Math.Min(MaxStam, stamRaise));

            if (manaRaise > ManaMax)
                SetMana(Math.Min(MaxMana, manaRaise));

            if (RaiseDamage && Utility.RandomDouble() < RaiseDamageFactor)
            {
                DamageMin = Math.Min(MinDamMax, DamageMin + 1);
                DamageMax = Math.Min(MaxDamMax, DamageMax + 1);
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }
		
		public CovetousCreature(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write(Level);
			writer.Write(VoidSpawn);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			Level = reader.ReadInt();
			VoidSpawn = reader.ReadBool();
		}
	}
 }