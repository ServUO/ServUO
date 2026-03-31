using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Services.Virtues;

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
		public override bool CanBeParagon { get { return false; } }
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

        public virtual bool CanGivePowerscrolls { get { return true; } }

        public static void GivePowerScrollTo(Mobile m, Item item, BaseChampion champ)
        {
            if (m == null)	//sanity
                return;

            if (!Core.SE || m.Alive)
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

                    if (prot.Map != m.Map || prot.Murderer || prot.Criminal || !JusticeVirtue.CheckMapRegion(m, prot) || !prot.InRange(champ, 100))
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

        public virtual Item GetArtifact()
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

        public virtual void GivePowerScrolls()
        {
            if (this.Map != Map.Felucca)
                return;

            List<Mobile> toGive = new List<Mobile>();
            List<DamageStore> rights = GetLootingRights();

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (ds.m_HasRight && InRange(ds.m_Mobile, 100) && ds.m_Mobile.Map == this.Map)
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

            for (int i = 0; i < ChampionSystem.PowerScrollAmount; ++i)
            {
                Mobile m = toGive[i % toGive.Count];

                PowerScroll ps = CreateRandomPowerScroll();
                m.SendLocalizedMessage(1049524); // You have received a scroll of power!

                GivePowerScrollTo(m, ps, this);
            }

            if (Core.TOL)
            {
                // Randomize - Primers
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

                    SkillMasteryPrimer p = CreateRandomPrimer();
                    m.SendLocalizedMessage(1156209); // You have received a mastery primer!

                    GivePowerScrollTo(m, p, this);
                }
            }

            ColUtility.Free(toGive);
        }

        public virtual void OnChampPopped(ChampionSpawn spawn)
        {
        }

        public override bool OnBeforeDeath()
        {
            if (CanGivePowerscrolls && !NoKillAwards)
            {
                this.GivePowerScrolls();

                if (this.NoGoodies)
                    return base.OnBeforeDeath();

				GoldShower.DoForChamp(Location, Map);
            }

            return base.OnBeforeDeath();
        }

        #region Custom Artifact Drops
        private static readonly Type[] m_CustomArtifacts = new Type[]
        {
            // Armor
            typeof(GlovesOfTheHolyWarrior), typeof(GargishKiltOfTheHolyWarrior),
            typeof(SentinelsMempo), typeof(SentinelsNecklace),
            typeof(ShugenjasRaiment), typeof(GargishShugenjasRaiment),
            typeof(HexweaversVisage), typeof(GargishHexweaversVisage),
            typeof(UmbrascaleChampionsAegis), typeof(GargishUmbrascaleChampionsAegis),
            typeof(CorruptedPaladinVambraces), typeof(GargishCorruptedPaladinVambraces),
            typeof(GlovesOfTheArchlich), typeof(GargishKiltOfTheArchlich),
            typeof(BalronBoneArmor), typeof(GargishBalronBoneArmor),
            // Khal Ankur
            typeof(MaskOfKhalAnkur), typeof(PendantOfKhalAnkur),
            // Clothing
            typeof(ScabbardOfJuonar), typeof(GargishScabbardOfJuonar),
            typeof(GeneralLethesEpaulettes), typeof(GargishGeneralLethesEpaulettes),
            typeof(LordMorphiusEpaulettes), typeof(GargishLordMorphiusEpaulettes),
            typeof(ShadowbaneEpaulettes), typeof(GargishShadowbaneEpaulettes),
            typeof(MantleOfTheArchlich),
            typeof(FeudalCloakOfElements), typeof(WingArmorOfElements),
            typeof(FeudalGhostwalkers), typeof(GargishFeudalGhostwalkers),
            typeof(MushroomApron), typeof(GargishMushroomApron),
            typeof(SerpentSkinQuiver), typeof(GargishSerpentSkinWingArmor),
            typeof(RangersCloakOfAugmentation), typeof(WardensArmorOfAugmentation),
            // Weapons - Unique
            typeof(ExporMalasFlamus), typeof(GargishExporMalasFlamus),
            typeof(ShugenjasWand),
            // Slayer Weapons - Reptile
            typeof(ReptileLeafblade), typeof(ReptileWarAxe), typeof(ReptileBroadsword),
            typeof(ReptileDoubleAxe), typeof(ReptileWarHammer), typeof(ReptileMagicalShortbow),
            typeof(ReptileCompositeBow), typeof(ReptileSoulGlaive), typeof(ReptileBoomerang),
            typeof(ReptileGargishTalwar), typeof(ReptileGargishKatana), typeof(ReptileLajatang),
            // Slayer Weapons - Repond
            typeof(RepondLeafblade), typeof(RepondWarAxe), typeof(RepondBroadsword),
            typeof(RepondDoubleAxe), typeof(RepondWarHammer), typeof(RepondMagicalShortbow),
            typeof(RepondCompositeBow), typeof(RepondSoulGlaive), typeof(RepondBoomerang),
            typeof(RepondGargishTalwar), typeof(RepondGargishKatana), typeof(RepondLajatang),
            // Slayer Weapons - Arachnid
            typeof(ArachnidLeafblade), typeof(ArachnidWarAxe), typeof(ArachnidBroadsword),
            typeof(ArachnidDoubleAxe), typeof(ArachnidWarHammer), typeof(ArachnidMagicalShortbow),
            typeof(ArachnidCompositeBow), typeof(ArachnidSoulGlaive), typeof(ArachnidBoomerang),
            typeof(ArachnidGargishTalwar), typeof(ArachnidGargishKatana), typeof(ArachnidLajatang),
            // Slayer Weapons - Undead
            typeof(UndeadLeafblade), typeof(UndeadWarAxe), typeof(UndeadBroadsword),
            typeof(UndeadDoubleAxe), typeof(UndeadWarHammer), typeof(UndeadMagicalShortbow),
            typeof(UndeadCompositeBow), typeof(UndeadSoulGlaive), typeof(UndeadBoomerang),
            typeof(UndeadGargishTalwar), typeof(UndeadGargishKatana), typeof(UndeadLajatang),
            // Slayer Weapons - Demon
            typeof(DemonLeafblade), typeof(DemonWarAxe), typeof(DemonBroadsword),
            typeof(DemonDoubleAxe), typeof(DemonWarHammer), typeof(DemonMagicalShortbow),
            typeof(DemonCompositeBow), typeof(DemonSoulGlaive), typeof(DemonBoomerang),
            typeof(DemonGargishTalwar), typeof(DemonGargishKatana), typeof(DemonLajatang),
            // Slayer Weapons - Fey
            typeof(FeyLeafblade), typeof(FeyWarAxe), typeof(FeyBroadsword),
            typeof(FeyDoubleAxe), typeof(FeyWarHammer), typeof(FeyMagicalShortbow),
            typeof(FeyCompositeBow), typeof(FeySoulGlaive), typeof(FeyBoomerang),
            typeof(FeyGargishTalwar), typeof(FeyGargishKatana), typeof(FeyLajatang),
            // Slayer Weapons - Elemental
            typeof(ElementalLeafblade), typeof(ElementalWarAxe), typeof(ElementalBroadsword),
            typeof(ElementalDoubleAxe), typeof(ElementalWarHammer), typeof(ElementalMagicalShortbow),
            typeof(ElementalCompositeBow), typeof(ElementalSoulGlaive), typeof(ElementalBoomerang),
            typeof(ElementalGargishTalwar), typeof(ElementalGargishKatana), typeof(ElementalLajatang),
            // Spellbooks
            typeof(ReptilianDeathSpellbook), typeof(RepondSpellbook),
            typeof(UndeadSpellbook), typeof(DemonSpellbook),
            typeof(FeySpellbook), typeof(ArachnidDoomSpellbook),
            typeof(ElementalBanSpellbook),
            // Talisman
            typeof(CarvedBoneRelicFromHolmes),
            typeof(ShadowMastersTalisman),
            // Jewelry
            typeof(SolariasSecretPoisons), typeof(GargishSolariasSecretPoisons),
            // New Armor
            typeof(DeathwardensGreaves), typeof(GargishDeathwardensGreaves),
            typeof(AzaroksLegplates), typeof(GargishAzaroksLegplates),
            // New Clothing
            typeof(KaelvoksCincture), typeof(GargishKaelvoksCincture),
            // Existing Updated
            typeof(MarkOfTravesty),
        };

        public static void GiveCustomArtifact(Mobile m)
        {
            Type type = m_CustomArtifacts[Utility.Random(m_CustomArtifacts.Length)];
            Item artifact = Loot.Construct(type);

            if (artifact != null)
            {
                m.AddToBackpack(artifact);
                m.SendMessage(0x22, "You have received a custom artifact!");
            }
        }
        #endregion

        public override void OnDeath(Container c)
        {
            if (this.Map == Map.Felucca)
            {
                //TODO: Confirm SE change or AoS one too?
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
                        toGive[Utility.Random(toGive.Count)].AddToBackpack(new ChampionSkull(this.SkullType));
                    else
                        c.DropItem(new ChampionSkull(this.SkullType));
                }

                if(Core.SA)
                    RefinementComponent.Roll(c, 3, 0.10);

                // Custom artifact drops - 5% chance per eligible player
                foreach (Mobile m in toGive)
                {
                    if (m is PlayerMobile && 0.10 > Utility.RandomDouble())
                    {
                        GiveCustomArtifact(m);
                    }
                }
            }

            base.OnDeath(c);
        }

        private static PowerScroll CreateRandomPowerScroll()
        {
            int level;
            double random = Utility.RandomDouble();

            if (0.15 >= random)
                level = 20;
            else if (0.5 >= random)
                level = 15;
            else
                level = 10;

            return PowerScroll.CreateRandomNoCraft(level, level);
        }

        private static SkillMasteryPrimer CreateRandomPrimer()
        {
            return SkillMasteryPrimer.GetRandom();
        }
    }
}