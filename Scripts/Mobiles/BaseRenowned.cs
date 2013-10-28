using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public abstract class BaseRenowned : BaseCreature
    {
        Dictionary<Mobile, int> m_DamageEntries;
        public BaseRenowned(AIType aiType)
            : this(aiType, FightMode.Closest)
        {
        }

        public BaseRenowned(AIType aiType, FightMode mode)
            : base(aiType, mode, 18, 1, 0.1, 0.2)
        {
        }

        public BaseRenowned(Serial serial)
            : base(serial)
        {
        }

        public abstract Type[] UniqueSAList { get; }
        public abstract Type[] SharedSAList { get; }
        public virtual bool NoGoodies
        {
            get
            {
                return false;
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

        public virtual void RegisterDamageTo(Mobile m)
        {
            if (m == null)
                return;

            foreach (DamageEntry de in m.DamageEntries)
            {
                Mobile damager = de.Damager;

                Mobile master = damager.GetDamageMaster(m);

                if (master != null)
                    damager = master;

                this.RegisterDamage(damager, de.DamageGiven);
            }
        }

        public void RegisterDamage(Mobile from, int amount)
        {
            if (from == null || !from.Player)
                return;

            if (this.m_DamageEntries.ContainsKey(from))
                this.m_DamageEntries[from] += amount;
            else
                this.m_DamageEntries.Add(from, amount);
        }

        public void AwardArtifact(Item artifact)
        {
            if (artifact == null)
                return;

            int totalDamage = 0;

            Dictionary<Mobile, int> validEntries = new Dictionary<Mobile, int>();

            foreach (KeyValuePair<Mobile, int> kvp in this.m_DamageEntries)
            {
                if (this.IsEligible(kvp.Key, artifact))
                {
                    validEntries.Add(kvp.Key, kvp.Value);
                    totalDamage += kvp.Value;
                }
            }

            int randomDamage = Utility.RandomMinMax(1, totalDamage);

            totalDamage = 0;

            foreach (KeyValuePair<Mobile, int> kvp in this.m_DamageEntries)
            {
                totalDamage += kvp.Value;

                if (totalDamage > randomDamage)
                {
                    this.GiveArtifact(kvp.Key, artifact);
                    break;
                }
            }
        }

        public void GiveArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                artifact.Delete();
            else
                to.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
        }

        public bool IsEligible(Mobile m, Item Artifact)
        {
            return m.Player && m.Alive && m.InRange(this.Location, 32) && m.Backpack != null && m.Backpack.CheckHold(m, Artifact, false);
        }

        public Item GetArtifact()
        {
            double random = Utility.RandomDouble();
            if (0.05 >= random)
                return this.CreateArtifact(this.UniqueSAList);
            else if (0.15 >= random)
                return this.CreateArtifact(this.SharedSAList);

            return null;
        }

        public Item CreateArtifact(Type[] list)
        {
            if (list.Length == 0)
                return null;

            int random = Utility.Random(list.Length);

            Type type = list[random];

            Item artifact = Loot.Construct(type);

            return artifact;
        }

        public override bool OnBeforeDeath()
        {
            if (!this.NoKillAwards)
            {
                if (this.NoGoodies)
                    return base.OnBeforeDeath();

                Map map = this.Map;

                if (map != null)
                {
                    for (int x = -12; x <= 12; ++x)
                    {
                        for (int y = -12; y <= 12; ++y)
                        {
                            double dist = Math.Sqrt(x * x + y * y);

                            if (dist <= 12)
                                new GoodiesTimer(map, this.X + x, this.Y + y).Start();
                        }
                    }
                }
        
                this.m_DamageEntries = new Dictionary<Mobile, int>();

                this.RegisterDamageTo(this);
                this.AwardArtifact(this.GetArtifact());
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            if (this.Map == Map.Felucca || this.Map == Map.TerMur)
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
            }

            base.OnDeath(c);
        }

        private class GoodiesTimer : Timer
        {
            private readonly Map m_Map;
            private readonly int m_X;
            private readonly int m_Y;
            public GoodiesTimer(Map map, int x, int y)
                : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                this.m_Map = map;
                this.m_X = x;
                this.m_Y = y;
            }

            protected override void OnTick()
            {
                int z = this.m_Map.GetAverageZ(this.m_X, this.m_Y);
                bool canFit = this.m_Map.CanFit(this.m_X, this.m_Y, z, 6, false, false);

                for (int i = -3; !canFit && i <= 3; ++i)
                {
                    canFit = this.m_Map.CanFit(this.m_X, this.m_Y, z + i, 6, false, false);

                    if (canFit)
                        z += i;
                }

                if (!canFit)
                    return;

                Gold g = new Gold(500, 1000);
				
                g.MoveToWorld(new Point3D(this.m_X, this.m_Y, z), this.m_Map);

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