using System;
using Server.Items;
using Server.Spells;

namespace Server.Engines.Craft
{
    public class DefInscription : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Inscribe;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044009;
            }// <CENTER>INSCRIPTION MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefInscription();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefInscription()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type typeItem)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            if (typeItem != null)
            {
                object o = Activator.CreateInstance(typeItem);

                if (o is SpellScroll)
                {
                    SpellScroll scroll = (SpellScroll)o;
                    Spellbook book = Spellbook.Find(from, scroll.SpellID);

                    bool hasSpell = (book != null && book.HasSpell(scroll.SpellID));

                    scroll.Delete();

                    return (hasSpell ? 0 : 1042404); // null : You don't have that spell!
                }
                else if (o is Item)
                {
                    ((Item)o).Delete();
                }
            }

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x249);
        }

        private static readonly Type typeofSpellScroll = typeof(SpellScroll);

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (!typeofSpellScroll.IsAssignableFrom(item.ItemType)) //  not a scroll
            {
                if (failed)
                {
                    if (lostMaterial)
                        return 1044043; // You failed to create the item, and some of your materials are lost.
                    else
                        return 1044157; // You failed to create the item, but no materials were lost.
                }
                else
                {
                    if (quality == 0)
                        return 502785; // You were barely able to make this item.  It's quality is below average.
                    else if (makersMark && quality == 2)
                        return 1044156; // You create an exceptional quality item and affix your maker's mark.
                    else if (quality == 2)
                        return 1044155; // You create an exceptional quality item.
                    else
                        return 1044154; // You create the item.
                }
            }
            else
            {
                if (failed)
                    return 501630; // You fail to inscribe the scroll, and the scroll is ruined.
                else
                    return 501629; // You inscribe the spell and put the scroll in your backpack.
            }
        }

        private int m_Circle, m_Mana;

        private enum Reg
        {
            BlackPearl,
            Bloodmoss,
            Garlic,
            Ginseng,
            MandrakeRoot,
            Nightshade,
            SulfurousAsh,
            SpidersSilk,
            DaemonBone,
            FertileDirt,
            DragonBlood,
            Bone
        }

        private readonly Type[] m_RegTypes = new Type[]
        {
            typeof(BlackPearl),
            typeof(Bloodmoss),
            typeof(Garlic),
            typeof(Ginseng),
            typeof(MandrakeRoot),
            typeof(Nightshade),
            typeof(SulfurousAsh),
            typeof(SpidersSilk),
            //Mysticism
            typeof(DaemonBone),
            typeof(FertileDirt),
            typeof(DragonBlood),
            typeof(Bone)
            //END				
        };

        private int m_Index;

        private void AddSpell(Type type, params Reg[] regs)
        {
            double minSkill, maxSkill;

            switch (this.m_Circle)
            {
                default:
                case 0:
                    minSkill = -25.0;
                    maxSkill = 25.0;
                    break;
                case 1:
                    minSkill = -10.8;
                    maxSkill = 39.2;
                    break;
                case 2:
                    minSkill = 03.5;
                    maxSkill = 53.5;
                    break;
                case 3:
                    minSkill = 17.8;
                    maxSkill = 67.8;
                    break;
                case 4:
                    minSkill = 32.1;
                    maxSkill = 82.1;
                    break;
                case 5:
                    minSkill = 46.4;
                    maxSkill = 96.4;
                    break;
                case 6:
                    minSkill = 60.7;
                    maxSkill = 110.7;
                    break;
                case 7:
                    minSkill = 75.0;
                    maxSkill = 125.0;
                    break;
            }

            int index = this.AddCraft(type, 1044369 + this.m_Circle, 1044381 + this.m_Index++, minSkill, maxSkill, this.m_RegTypes[(int)regs[0]], 1044353 + (int)regs[0], 1, 1044361 + (int)regs[0]);

            for (int i = 1; i < regs.Length; ++i)
                this.AddRes(index, this.m_RegTypes[(int)regs[i]], 1044353 + (int)regs[i], 1, 1044361 + (int)regs[i]);

            this.AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            this.SetManaReq(index, this.m_Mana);
        }

        private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            int id = CraftItem.ItemIDOf(regs[0]);

            int index = this.AddCraft(type, 1061677, 1060509 + spell, minSkill, minSkill + 1.0, regs[0], id < 0x4000 ? 1020000 + id : 1078872 + id, 1, 501627);	//Yes, on OSI it's only 1.0 skill diff'.  Don't blame me, blame OSI.

            for (int i = 1; i < regs.Length; ++i)
            {
                id = CraftItem.ItemIDOf(regs[i]);
                this.AddRes(index, regs[i], id < 0x4000 ? 1020000 + id : 1078872 + id, 1, 501627);
            }

            this.AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            this.SetManaReq(index, mana);
        }
		
        private void AddMysticSpell(string spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            int index = this.AddCraft(type, "Spells of Mysticism", spell, minSkill, minSkill + 1.0, regs[0], 1020000 + CraftItem.ItemIDOf(regs[0]), 1, 501627);	//Yes, on OSI it's only 1.0 skill diff'.  Don't blame me, blame OSI.

            for (int i = 1; i < regs.Length; ++i)
                this.AddRes(index, regs[i], 1020000 + CraftItem.ItemIDOf(regs[i]), 1, 501627);

            this.AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            this.SetManaReq(index, mana);
        }

        public override void InitCraftList()
        {
            this.m_Circle = 0;
            this.m_Mana = 4;

            this.AddSpell(typeof(ReactiveArmorScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(ClumsyScroll), Reg.Bloodmoss, Reg.Nightshade);
            this.AddSpell(typeof(CreateFoodScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
            this.AddSpell(typeof(FeeblemindScroll), Reg.Nightshade, Reg.Ginseng);
            this.AddSpell(typeof(HealScroll), Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk);
            this.AddSpell(typeof(MagicArrowScroll), Reg.SulfurousAsh);
            this.AddSpell(typeof(NightSightScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(WeakenScroll), Reg.Garlic, Reg.Nightshade);

            this.m_Circle = 1;
            this.m_Mana = 6;

            this.AddSpell(typeof(AgilityScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            this.AddSpell(typeof(CunningScroll), Reg.Nightshade, Reg.MandrakeRoot);
            this.AddSpell(typeof(CureScroll), Reg.Garlic, Reg.Ginseng);
            this.AddSpell(typeof(HarmScroll), Reg.Nightshade, Reg.SpidersSilk);
            this.AddSpell(typeof(MagicTrapScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(MagicUnTrapScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
            this.AddSpell(typeof(ProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.SulfurousAsh);
            this.AddSpell(typeof(StrengthScroll), Reg.Nightshade, Reg.MandrakeRoot);

            this.m_Circle = 2;
            this.m_Mana = 9;

            this.AddSpell(typeof(BlessScroll), Reg.Garlic, Reg.MandrakeRoot);
            this.AddSpell(typeof(FireballScroll), Reg.BlackPearl);
            this.AddSpell(typeof(MagicLockScroll), Reg.Bloodmoss, Reg.Garlic, Reg.SulfurousAsh);
            this.AddSpell(typeof(PoisonScroll), Reg.Nightshade);
            this.AddSpell(typeof(TelekinisisScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            this.AddSpell(typeof(TeleportScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            this.AddSpell(typeof(UnlockScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
            this.AddSpell(typeof(WallOfStoneScroll), Reg.Bloodmoss, Reg.Garlic);

            this.m_Circle = 3;
            this.m_Mana = 11;

            this.AddSpell(typeof(ArchCureScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
            this.AddSpell(typeof(ArchProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot, Reg.SulfurousAsh);
            this.AddSpell(typeof(CurseScroll), Reg.Garlic, Reg.Nightshade, Reg.SulfurousAsh);
            this.AddSpell(typeof(FireFieldScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(GreaterHealScroll), Reg.Garlic, Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Ginseng);
            this.AddSpell(typeof(LightningScroll), Reg.MandrakeRoot, Reg.SulfurousAsh);
            this.AddSpell(typeof(ManaDrainScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.MandrakeRoot);
            this.AddSpell(typeof(RecallScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot);

            this.m_Circle = 4;
            this.m_Mana = 14;

            this.AddSpell(typeof(BladeSpiritsScroll), Reg.BlackPearl, Reg.Nightshade, Reg.MandrakeRoot);
            this.AddSpell(typeof(DispelFieldScroll), Reg.BlackPearl, Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(IncognitoScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Nightshade);
            this.AddSpell(typeof(MagicReflectScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
            this.AddSpell(typeof(MindBlastScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
            this.AddSpell(typeof(ParalyzeScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
            this.AddSpell(typeof(PoisonFieldScroll), Reg.BlackPearl, Reg.Nightshade, Reg.SpidersSilk);
            this.AddSpell(typeof(SummonCreatureScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            this.m_Circle = 5;
            this.m_Mana = 20;

            this.AddSpell(typeof(DispelScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            this.AddSpell(typeof(EnergyBoltScroll), Reg.BlackPearl, Reg.Nightshade);
            this.AddSpell(typeof(ExplosionScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            this.AddSpell(typeof(InvisibilityScroll), Reg.Bloodmoss, Reg.Nightshade);
            this.AddSpell(typeof(MarkScroll), Reg.Bloodmoss, Reg.BlackPearl, Reg.MandrakeRoot);
            this.AddSpell(typeof(MassCurseScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
            this.AddSpell(typeof(ParalyzeFieldScroll), Reg.BlackPearl, Reg.Ginseng, Reg.SpidersSilk);
            this.AddSpell(typeof(RevealScroll), Reg.Bloodmoss, Reg.SulfurousAsh);

            this.m_Circle = 6;
            this.m_Mana = 40;

            this.AddSpell(typeof(ChainLightningScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh);
            this.AddSpell(typeof(EnergyFieldScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(FlamestrikeScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(GateTravelScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SulfurousAsh);
            this.AddSpell(typeof(ManaVampireScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            this.AddSpell(typeof(MassDispelScroll), Reg.BlackPearl, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            this.AddSpell(typeof(MeteorSwarmScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh, Reg.SpidersSilk);
            this.AddSpell(typeof(PolymorphScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            this.m_Circle = 7;
            this.m_Mana = 50;

            this.AddSpell(typeof(EarthquakeScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Ginseng, Reg.SulfurousAsh);
            this.AddSpell(typeof(EnergyVortexScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Nightshade);
            this.AddSpell(typeof(ResurrectionScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Ginseng);
            this.AddSpell(typeof(SummonAirElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            this.AddSpell(typeof(SummonDaemonScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(SummonEarthElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            this.AddSpell(typeof(SummonFireElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            this.AddSpell(typeof(SummonWaterElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            if (Core.SE)
            {
                this.AddNecroSpell(0, 23, 39.6, typeof(AnimateDeadScroll), Reagent.GraveDust, Reagent.DaemonBlood);
                this.AddNecroSpell(1, 13, 19.6, typeof(BloodOathScroll), Reagent.DaemonBlood);
                this.AddNecroSpell(2, 11, 19.6, typeof(CorpseSkinScroll), Reagent.BatWing, Reagent.GraveDust);
                this.AddNecroSpell(3, 7, 19.6, typeof(CurseWeaponScroll), Reagent.PigIron);
                this.AddNecroSpell(4, 11, 19.6, typeof(EvilOmenScroll), Reagent.BatWing, Reagent.NoxCrystal);
                this.AddNecroSpell(5, 11, 39.6, typeof(HorrificBeastScroll), Reagent.BatWing, Reagent.DaemonBlood);
                this.AddNecroSpell(6, 23, 69.6, typeof(LichFormScroll), Reagent.GraveDust, Reagent.DaemonBlood, Reagent.NoxCrystal);
                this.AddNecroSpell(7, 17, 29.6, typeof(MindRotScroll), Reagent.BatWing, Reagent.DaemonBlood, Reagent.PigIron);
                this.AddNecroSpell(8, 5, 19.6, typeof(PainSpikeScroll), Reagent.GraveDust, Reagent.PigIron);
                this.AddNecroSpell(9, 17, 49.6, typeof(PoisonStrikeScroll), Reagent.NoxCrystal);
                this.AddNecroSpell(10, 29, 64.6, typeof(StrangleScroll), Reagent.DaemonBlood, Reagent.NoxCrystal);
                this.AddNecroSpell(11, 17, 29.6, typeof(SummonFamiliarScroll), Reagent.BatWing, Reagent.GraveDust, Reagent.DaemonBlood);
                this.AddNecroSpell(12, 23, 98.6, typeof(VampiricEmbraceScroll), Reagent.BatWing, Reagent.NoxCrystal, Reagent.PigIron);
                this.AddNecroSpell(13, 41, 79.6, typeof(VengefulSpiritScroll), Reagent.BatWing, Reagent.GraveDust, Reagent.PigIron);
                this.AddNecroSpell(14, 23, 59.6, typeof(WitherScroll), Reagent.GraveDust, Reagent.NoxCrystal, Reagent.PigIron);
                this.AddNecroSpell(15, 17, 79.6, typeof(WraithFormScroll), Reagent.NoxCrystal, Reagent.PigIron);
                this.AddNecroSpell(16, 40, 79.6, typeof(ExorcismScroll), Reagent.NoxCrystal, Reagent.GraveDust);
            }

            int index;
			
            if (Core.ML)
            {
                index = this.AddCraft(typeof(EnchantedSwitch), 1044294, 1072893, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
                this.AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
                this.AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
                this.AddRes(index, typeof(SwitchItem), 1073464, 1, 1044253);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
				
                index = this.AddCraft(typeof(RunedPrism), 1044294, 1073465, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
                this.AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
                this.AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
                this.AddRes(index, typeof(HollowPrism), 1072895, 1, 1044253);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
				
                //Mysticism Scrolls
                this.AddMysticSpell("Nether Bolt", 4, 0.0, typeof(NetherBoltScroll), Reagent.SulfurousAsh, Reagent.BlackPearl);
                this.AddMysticSpell("Healing Stone", 4, 0.0, typeof(HealingStoneScroll), Reagent.Bone, Reagent.Garlic, Reagent.Ginseng, Reagent.SpidersSilk);
                this.AddMysticSpell("Purge Magic", 6, 0.0, typeof(PurgeMagicScroll), Reagent.FertileDirt, Reagent.Garlic, Reagent.MandrakeRoot, Reagent.SulfurousAsh);
                this.AddMysticSpell("Enchant", 6, 0.0, typeof(EnchantScroll), Reagent.SpidersSilk, Reagent.MandrakeRoot, Reagent.SulfurousAsh);
                this.AddMysticSpell("Sleep", 9, 3.5, typeof(SleepScroll), Reagent.SpidersSilk, Reagent.BlackPearl, Reagent.Nightshade);
                this.AddMysticSpell("Eagle Strike", 9, 3.5, typeof(EagleStrikeScroll), Reagent.SpidersSilk, Reagent.Bloodmoss, Reagent.MandrakeRoot, Reagent.Bone);
                this.AddMysticSpell("Animated Weapon", 11, 17.8, typeof(AnimatedWeaponScroll), Reagent.Bone, Reagent.BlackPearl, Reagent.MandrakeRoot, Reagent.Nightshade);
                this.AddMysticSpell("Stone Form", 11, 17.8, typeof(StoneFormScroll), Reagent.Bloodmoss, Reagent.FertileDirt, Reagent.Garlic);
                this.AddMysticSpell("Spell Trigger", 14, 32.1, typeof(SpellTriggerScroll), Reagent.SpidersSilk, Reagent.MandrakeRoot, Reagent.Garlic, Reagent.DragonBlood);
                this.AddMysticSpell("Mass Sleep", 14, 32.1, typeof(MassSleepScroll), Reagent.SpidersSilk, Reagent.Nightshade, Reagent.Ginseng);
                this.AddMysticSpell("Cleansing Winds", 20, 46.4, typeof(CleansingWindsScroll), Reagent.Ginseng, Reagent.Garlic, Reagent.DragonBlood, Reagent.MandrakeRoot);
                this.AddMysticSpell("Bombard", 20, 46.4, typeof(BombardScroll), Reagent.Garlic, Reagent.DragonBlood, Reagent.SulfurousAsh, Reagent.Bloodmoss);
                this.AddMysticSpell("Spell Plague", 40, 60.7, typeof(SpellPlagueScroll), Reagent.Garlic, Reagent.DragonBlood, Reagent.MandrakeRoot, Reagent.Nightshade, Reagent.SulfurousAsh, Reagent.DaemonBone);
                this.AddMysticSpell("Hail Storm", 40, 60.7, typeof(HailStormScroll), Reagent.DragonBlood, Reagent.BlackPearl, Reagent.MandrakeRoot, Reagent.Bloodmoss);
                this.AddMysticSpell("Nether Cyclone", 50, 75.0, typeof(NetherCycloneScroll), Reagent.Bloodmoss, Reagent.Nightshade, Reagent.SulfurousAsh, Reagent.MandrakeRoot);
                this.AddMysticSpell("Rising Colossus", 50, 75.0, typeof(RisingColossusScroll), Reagent.DaemonBone, Reagent.FertileDirt, Reagent.DragonBlood, Reagent.Nightshade, Reagent.MandrakeRoot);
                //END
            }
			
            // Runebook
            index = this.AddCraft(typeof(Runebook), 1044294, 1041267, 45.0, 95.0, typeof(BlankScroll), 1044377, 8, 1044378);
            this.AddRes(index, typeof(RecallScroll), 1044445, 1, 1044253);
            this.AddRes(index, typeof(GateTravelScroll), 1044446, 1, 1044253);

            if (Core.AOS)
            {
                this.AddCraft(typeof(Engines.BulkOrders.BulkOrderBook), 1044294, 1028793, 65.0, 115.0, typeof(BlankScroll), 1044377, 10, 1044378);
            }

            if (Core.SE)
            {
                this.AddCraft(typeof(Spellbook), 1044294, 1023834, 50.0, 126, typeof(BlankScroll), 1044377, 10, 1044378);
            }
			
            #region Mondain's Legacy	
            if (Core.ML)
            {
                index = this.AddCraft(typeof(ScrappersCompendium), 1044294, 1072940, 75.0, 125.0, typeof(BlankScroll), 1044377, 100, 1044378);
                this.AddRes(index, typeof(DreadHornMane), 1032682, 1, 1044253);
                this.AddRes(index, typeof(Taint), 1032679, 10, 1044253);
                this.AddRes(index, typeof(Corruption), 1032676, 10, 1044253);
                this.AddRecipe(index, (int)TinkerRecipes.ScrappersCompendium);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
				
                index = this.AddCraft(typeof(SpellbookEngraver), 1044294, 1072151, 75.0, 100.0, typeof(Feather), 1044562, 1, 1044563);
                this.AddRes(index, typeof(BlackPearl), 1015001, 7, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);
				
                this.AddCraft(typeof(NecromancerSpellbook), 1044294, "Necromancer spellbook", 50.0, 100.0, typeof(BlankScroll), 1044377, 10, 1044378);
                //	AddCraft(typeof(SpellweavingBook), 1044294, "Spellweaving book", 50.0, 100.0, typeof(BlankScroll), 1044377, 10, 1044378);
                this.AddCraft(typeof(MysticBook), 1044294, "Mysticism spellbook", 50.0, 100.0, typeof(BlankScroll), 1044377, 10, 1044378);
            }
            #endregion

            #region OS-Edit for SA items
            if (Core.SA)
            {
                index = this.AddCraft(typeof(GargoyleBook100), 1044294, 1113290, 60.0, 100.0, typeof(BlankScroll), 1044377, 40, 1044378);
                this.AddRes(index, typeof(Beeswax), 1025154, 2, "You do not have enough beeswax.");
				
                index = this.AddCraft(typeof(GargoyleBook200), 1044294, 1113291, 72.0, 100.0, typeof(BlankScroll), 1044377, 40, 1044378);
                this.AddRes(index, typeof(Beeswax), 1025154, 4, "You do not have enough beeswax.");

                index = AddCraft(typeof(ScrollBinderDeed), 1044294, ("Scroll Binder"), 75.0, 100.0, typeof(WoodPulp), ("Wood Pulp"), 1, ("You do not have enough Wood Pulp")); //Todo check Clilocs
            }
            #endregion

            this.MarkOption = true;
        }
    }
}