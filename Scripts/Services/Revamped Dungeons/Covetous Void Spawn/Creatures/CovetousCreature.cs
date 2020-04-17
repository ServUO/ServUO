using Server.Items;
using System;

namespace Server.Mobiles
{
    public class CovetousCreature : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool VoidSpawn { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level { get; set; }

        public virtual int Stage => Math.Max(1, Level / 5);
        public virtual int MaxStage => 15;

        public virtual int StatRatio => Utility.RandomMinMax(35, 60);

        public virtual double SkillStart => Utility.RandomMinMax(35, 50);
        public virtual double SkillMax => 160.0;

        public virtual int StrStart => Utility.RandomMinMax(91, 100);
        public virtual int DexStart => Utility.RandomMinMax(91, 100);
        public virtual int IntStart => IsMagical ? Utility.RandomMinMax(91, 100) : 1;

        public virtual int StrMax => 410;
        public virtual int DexMax => 422;
        public virtual int IntMax => 250;

        public virtual int MaxHits => 2400;
        public virtual int MaxStam => 3000;
        public virtual int MaxMana => IsMagical ? 8500 : 1500;

        public virtual int MinDamMax => 5;
        public virtual int MaxDamMax => 12;

        public virtual int MinDamStart => 5;
        public virtual int MaxDamStart => 15;

        public virtual int HitsStart => StrStart + (int)(StrStart * (StatRatio / 100.0));
        public virtual int StamStart => DexStart + (int)(DexStart * (StatRatio / 100.0));
        public virtual int ManaStart => IntStart + (int)(IntStart * (StatRatio / 100.0));

        public virtual bool RaiseDamage => true;
        public virtual double RaiseDamageFactor => 0.33;

        public virtual int ResistStart => 25;
        public virtual int ResistMax => 95;

        public virtual bool IsMagical => AIObject is MageAI;

        public override bool GivesFameAndKarmaAward => false;
        public override bool PlayerRangeSensitive => false;
        public override bool CanDestroyObstacles => true;

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
                    MoveToWorld(CurrentWayPoint.Location, Map);
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

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 10);

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
                eable = Map.GetItemsInRange(Location, 8);

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
            if (!VoidSpawn)
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