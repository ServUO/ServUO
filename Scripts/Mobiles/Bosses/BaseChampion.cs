using Server.Engines.CannedEvil;
using Server.Items;
using Server.Services.Virtues;
using System;
using System.Collections.Generic;

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
        public override bool CanBeParagon => false;
        public abstract ChampionSkullType SkullType { get; }
        public abstract Type[] UniqueList { get; }
        public abstract Type[] SharedList { get; }
        public abstract Type[] DecorativeList { get; }
        public abstract MonsterStatuetteType[] StatueTypes { get; }
        public virtual bool NoGoodies => false;

        public virtual bool CanGivePowerscrolls => true;
        public virtual bool RestrictedToFelucca => true;
        public virtual int PowerScrollAmount => ChampionSystem.PowerScrollAmount;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public virtual Item GetArtifact()
        {
            double random = Utility.RandomDouble();
            if (0.05 >= random)
                return CreateArtifact(UniqueList);
            else if (0.15 >= random)
                return CreateArtifact(SharedList);
            else if (0.30 >= random)
                return CreateArtifact(DecorativeList);
            return null;
        }

        public Item CreateArtifact(Type[] list)
        {
            if (list.Length == 0)
                return null;

            int random = Utility.Random(list.Length);

            Type type = list[random];

            Item artifact = Loot.Construct(type);

            if (artifact is MonsterStatuette && StatueTypes.Length > 0)
            {
                ((MonsterStatuette)artifact).Type = StatueTypes[Utility.Random(StatueTypes.Length)];
                ((MonsterStatuette)artifact).LootType = LootType.Regular;
            }

            return artifact;
        }

        public virtual void GivePowerScrolls()
        {
            if (Map == null || (RestrictedToFelucca && Map.Rules != MapRules.FeluccaRules))
                return;

            List<Mobile> toGive = new List<Mobile>();
            List<DamageStore> rights = GetLootingRights();

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (ds.m_HasRight && InRange(ds.m_Mobile, 100) && ds.m_Mobile.Map == Map)
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

            // Randomize - PowerScrolls
            for (int i = 0; i < toGive.Count; ++i)
            {
                int rand = Utility.Random(toGive.Count);
                Mobile hold = toGive[i];
                toGive[i] = toGive[rand];
                toGive[rand] = hold;
            }

            for (int i = 0; i < PowerScrollAmount; ++i)
            {
                Mobile m = toGive[i % toGive.Count];

                var ps = GetPowerScroll();
                GiveItemMessage(m, ps);

                GivePowerScrollTo(m, ps);
            }

            // Randomize - Primers
            for (int i = 0; i < toGive.Count; ++i)
            {
                int rand = Utility.Random(toGive.Count);
                Mobile hold = toGive[i];
                toGive[i] = toGive[rand];
                toGive[rand] = hold;
            }

            for (int i = 0; i < PowerScrollAmount; ++i)
            {
                Mobile m = toGive[i % toGive.Count];

                SkillMasteryPrimer p = CreateRandomPrimer();
                GiveItemMessage(m, p);

                GivePowerScrollTo(m, p);
            }

            ColUtility.Free(toGive);
        }

        public virtual void GiveItemMessage(Mobile m, Item item)
        {
            if (m == null)
                return;

            if (item is ScrollOfTranscendence)
            {
                m.SendLocalizedMessage(1094936); // You have received a Scroll of Transcendence!
            }
            else if (item is SkillMasteryPrimer)
            {
                m.SendLocalizedMessage(1156209); // You have received a mastery primer!
            }
            else
            {
                m.SendLocalizedMessage(1049524); // You have received a scroll of power!
            }
        }

        public virtual void GivePowerScrollTo(Mobile m, Item item)
        {
            if (m == null)	//sanity
                return;

            if (m.Alive)
                m.AddToBackpack(item);
            else
            {
                if (m.Corpse != null && !m.Corpse.Deleted)
                    m.Corpse.DropItem(item);
                else
                    m.AddToBackpack(item);
            }

            if (item is PowerScroll && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                for (int j = 0; j < pm.JusticeProtectors.Count; ++j)
                {
                    Mobile prot = pm.JusticeProtectors[j];

                    if (prot.Map != m.Map || prot.Murderer || prot.Criminal || !JusticeVirtue.CheckMapRegion(m, prot) || !prot.InRange(this, 100))
                        continue;

                    int chance = 0;

                    switch (VirtueHelper.GetLevel(prot, VirtueName.Justice))
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
                        var powerScroll = GetJusticePowerScroll();

                        prot.SendLocalizedMessage(1049368); // You have been rewarded for your dedication to Justice!

                        if (prot.Alive)
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

        public virtual Item GetPowerScroll()
        {
            return CreateRandomPowerScroll();
        }

        public virtual Item GetJusticePowerScroll()
        {
            return CreateRandomPowerScroll();
        }

        public virtual Item CreateRandomPowerScroll()
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

        public virtual SkillMasteryPrimer CreateRandomPrimer()
        {
            return SkillMasteryPrimer.GetRandom();
        }

        public virtual void OnChampPopped(ChampionSpawn spawn)
        {
        }

        public override bool OnBeforeDeath()
        {
            if (CanGivePowerscrolls && !NoKillAwards)
            {
                GivePowerScrolls();

                if (NoGoodies)
                    return base.OnBeforeDeath();

                GoldShower.DoForChamp(Location, Map);
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            if (Map == Map.Felucca)
            {
                List<DamageStore> rights = GetLootingRights();
                List<Mobile> toGive = new List<Mobile>();

                for (int i = rights.Count - 1; i >= 0; --i)
                {
                    DamageStore ds = rights[i];

                    if (ds.m_HasRight)
                        toGive.Add(ds.m_Mobile);
                }

                if (SkullType != ChampionSkullType.None)
                {
                    if (toGive.Count > 0)
                        toGive[Utility.Random(toGive.Count)].AddToBackpack(new ChampionSkull(SkullType));
                    else
                        c.DropItem(new ChampionSkull(SkullType));
                }

                RefinementComponent.Roll(c, 3, 0.10);
            }

            base.OnDeath(c);
        }
    }
}
