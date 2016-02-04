using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public abstract class BaseChampion : BaseCreature
    {
        public BaseChampion(AIType aiType)
            : this(aiType, FightMode.Closest)
        {
        }

        public BaseChampion(AIType aiType, FightMode mode)
            : base(aiType, mode, 18, 1, 0.1, 0.2)
        {
        }

        public BaseChampion(Serial serial)
            : base(serial)
        {
        }

        public abstract ChampionSkullType SkullType { get; }
        public abstract Type[] UniqueList { get; }
        public abstract Type[] SharedList { get; }
        public abstract Type[] DecorativeList { get; }
        public abstract MonsterStatuetteType[] StatueTypes { get; }
        public virtual bool NoGoodies
        {
            get
            {
                return false;
            }
        }
        public static void GivePowerScrollTo(Mobile m)
        {
            if (m == null)	//sanity
                return;

			PowerScroll ps = CreateRandomPowerScroll();

            m.SendLocalizedMessage(1049524); // You have received a scroll of power!

            if (!Core.SE || m.Alive)
                m.AddToBackpack(ps);
            else
            {
                if (m.Corpse != null && !m.Corpse.Deleted)
                    m.Corpse.DropItem(ps);
                else
                    m.AddToBackpack(ps);
            }

            if (m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                for (int j = 0; j < pm.JusticeProtectors.Count; ++j)
                {
                    Mobile prot = pm.JusticeProtectors[j];

                    if (prot.Map != m.Map || prot.Kills >= 5 || prot.Criminal || !JusticeVirtue.CheckMapRegion(m, prot))
                        continue;

                    int chance = 0;

                    switch( VirtueHelper.GetLevel(prot, VirtueName.Justice) )
                    {
                        case VirtueLevel.Seeker:
                            chance = 60;
                            break;
                        case VirtueLevel.Follower:
                            chance = 80;
                            break;
                        case VirtueLevel.Knight:
                            chance = 100;
                            break;
                    }

                    if (chance > Utility.Random(100))
                    {
						PowerScroll powerScroll = CreateRandomPowerScroll();

                        prot.SendLocalizedMessage(1049368); // You have been rewarded for your dedication to Justice!

                        if (!Core.SE || prot.Alive)
                            prot.AddToBackpack(powerScroll);
                        else
                        {
                            if (prot.Corpse != null && !prot.Corpse.Deleted)
                                prot.Corpse.DropItem(powerScroll);
                            else
                                prot.AddToBackpack(powerScroll);
                        }
                    }
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
        }

        public Item GetArtifact()
        {
            double random = Utility.RandomDouble();
            if (0.05 >= random)
                return this.CreateArtifact(this.UniqueList);
            else if (0.15 >= random)
                return this.CreateArtifact(this.SharedList);
            else if (0.30 >= random)
                return this.CreateArtifact(this.DecorativeList);
            return null;
        }

        public Item CreateArtifact(Type[] list)
        {
            if (list.Length == 0)
                return null;

            int random = Utility.Random(list.Length);
			
            Type type = list[random];

            Item artifact = Loot.Construct(type);

            if (artifact is MonsterStatuette && this.StatueTypes.Length > 0)
            {
                ((MonsterStatuette)artifact).Type = this.StatueTypes[Utility.Random(this.StatueTypes.Length)];
                ((MonsterStatuette)artifact).LootType = LootType.Regular;
            }

            return artifact;
        }

        public void GivePowerScrolls()
        {
            if (this.Map != Map.Felucca)
                return;

            List<Mobile> toGive = new List<Mobile>();
            List<DamageStore> rights = BaseCreature.GetLootingRights(this.DamageEntries, this.HitsMax);

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (ds.m_HasRight)
                    toGive.Add(ds.m_Mobile);
            }

            if (toGive.Count == 0)
                return;

            for (int i = 0; i < toGive.Count; i++)
            {
                Mobile m = toGive[i];

                if (!(m is PlayerMobile))
                    continue;

                bool gainedPath = false;

                int pointsToGain = 800;

                if (VirtueHelper.Award(m, VirtueName.Valor, pointsToGain, ref gainedPath))
                {
                    if (gainedPath)
                        m.SendLocalizedMessage(1054032); // You have gained a path in Valor!
                    else
                        m.SendLocalizedMessage(1054030); // You have gained in Valor!
                    //No delay on Valor gains
                }
            }

            // Randomize
            for (int i = 0; i < toGive.Count; ++i)
            {
                int rand = Utility.Random(toGive.Count);
                Mobile hold = toGive[i];
                toGive[i] = toGive[rand];
                toGive[rand] = hold;
            }

            for (int i = 0; i < ChampionSystem.PowerScrollAmount; ++i)
            {
                Mobile m = toGive[i % toGive.Count];

                GivePowerScrollTo(m);
            }
        }

        public override bool OnBeforeDeath()
        {
            if (!this.NoKillAwards)
            {
                this.GivePowerScrolls();

                if (this.NoGoodies)
                    return base.OnBeforeDeath();

				for(int i = 0; i < ChampionSystem.GoldShowerPiles; ++i)
				{
					new GoodiesTimer(Map, Location).Start();
				}
            }

            return base.OnBeforeDeath();
        }

		private static Point3D FindGoldLocation(Map map, Point3D center, int range)
		{
			int cx = center.X;
			int cy = center.Y;

			for(int i = 0; i < 20; ++i)
			{
				int x = cx + Utility.Random(range * 2) - range;
				int y = cy + Utility.Random(range * 2) - range;
				if ((cx - x) * (cx - x) + (cy - y) * (cy - y) > range * range)
					continue;

				int z = map.GetAverageZ(x, y);
				if (map.CanFit(x, y, z, 6, false, false))
					return new Point3D(x, y, z);

				for (int j = -3; j <= 3; ++j)
				{
					if (map.CanFit(x, y, z + j, 6, false, false))
						return new Point3D(x, y, z + j);
				}
			}
			return center;
		}

        public override void OnDeath(Container c)
        {
            if (this.Map == Map.Felucca)
            {
                //TODO: Confirm SE change or AoS one too?
                List<DamageStore> rights = BaseCreature.GetLootingRights(this.DamageEntries, this.HitsMax);
                List<Mobile> toGive = new List<Mobile>();

                for (int i = rights.Count - 1; i >= 0; --i)
                {
                    DamageStore ds = rights[i];

                    if (ds.m_HasRight)
                        toGive.Add(ds.m_Mobile);
                }

                if (toGive.Count > 0)
                    toGive[Utility.Random(toGive.Count)].AddToBackpack(new ChampionSkull(this.SkullType));
                else
                    c.DropItem(new ChampionSkull(this.SkullType));
            }

            base.OnDeath(c);
        }

        private static PowerScroll CreateRandomPowerScroll()
        {
            int level;
            double random = Utility.RandomDouble();

            if (0.05 >= random)
                level = 20;
            else if (0.4 >= random)
                level = 15;
            else
                level = 10;

            return PowerScroll.CreateRandomNoCraft(level, level);
        }

        private class GoodiesTimer : Timer
        {
            private readonly Map m_Map;
			private readonly Point3D m_Location;
            public GoodiesTimer(Map map, Point3D p)
                : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                m_Map = map;
				m_Location = p;
            }

            protected override void OnTick()
            {
				Point3D p = FindGoldLocation(m_Map, m_Location, 6);
				Gold g = new Gold(ChampionSystem.GoldShowerMinAmount, ChampionSystem.GoldShowerMaxAmount);
                g.MoveToWorld(p, this.m_Map);

                if (0.5 >= Utility.RandomDouble())
                {
                    switch ( Utility.Random(3) )
                    {
                        case 0: // Fire column
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                                Effects.PlaySound(g, g.Map, 0x208);

                                break;
                            }
                        case 1: // Explosion
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                                Effects.PlaySound(g, g.Map, 0x307);

                                break;
                            }
                        case 2: // Ball of fire
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);

                                break;
                            }
                    }
                }
            }
        }
    }
}