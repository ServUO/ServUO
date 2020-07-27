using Server.Items;
using System;

namespace Server.Engines.Craft
{
    public enum InscriptionRecipes
    {
        RunicAtlas = 800
    }

    public class DefInscription : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Inscribe;

        public override int GumpTitleNumber => 1044009;

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

        public override int CanCraft(Mobile from, ITool tool, Type typeItem)
        {
            int num = 0;

            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!
            else if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.

            if (typeItem != null && typeItem.IsSubclassOf(typeof(SpellScroll)))
            {
                if (!_Buffer.ContainsKey(typeItem))
                {
                    object o = Activator.CreateInstance(typeItem);

                    if (o is SpellScroll)
                    {
                        SpellScroll scroll = (SpellScroll)o;
                        _Buffer[typeItem] = scroll.SpellID;
                        scroll.Delete();
                    }
                    else if (o is IEntity)
                    {
                        ((IEntity)o).Delete();
                        return 1042404; // You don't have that spell!
                    }
                }

                int id = _Buffer[typeItem];
                Spellbook book = Spellbook.Find(from, id);

                if (book == null || !book.HasSpell(id))
                    return 1042404; // You don't have that spell!
            }

            return 0;
        }

        private readonly System.Collections.Generic.Dictionary<Type, int> _Buffer = new System.Collections.Generic.Dictionary<Type, int>();

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

        private enum Reg { BlackPearl, Bloodmoss, Garlic, Ginseng, MandrakeRoot, Nightshade, SulfurousAsh, SpidersSilk, BatWing, GraveDust, DaemonBlood, NoxCrystal, PigIron, Bone, DragonBlood, FertileDirt, DaemonBone }

        private readonly Type[] m_RegTypes = new Type[]
        {
            typeof( BlackPearl ),
            typeof( Bloodmoss ),
            typeof( Garlic ),
            typeof( Ginseng ),
            typeof( MandrakeRoot ),
            typeof( Nightshade ),
            typeof( SulfurousAsh ),
            typeof( SpidersSilk ),
            typeof( BatWing ),
            typeof( GraveDust ),
            typeof( DaemonBlood ),
            typeof( NoxCrystal ),
            typeof( PigIron ),
            typeof( Bone ),
            typeof( DragonBlood ),
            typeof( FertileDirt ),
            typeof( DaemonBone )
        };

        private int m_Index;

        private void AddSpell(Type type, params Reg[] regs)
        {
            double minSkill, maxSkill;
            int cliloc;

            switch (m_Circle)
            {
                default:
                case 0: minSkill = -25.0; maxSkill = 25.0; cliloc = 1111691; break;
                case 1: minSkill = -10.8; maxSkill = 39.2; cliloc = 1111691; break;
                case 2: minSkill = 03.5; maxSkill = 53.5; cliloc = 1111692; break;
                case 3: minSkill = 17.8; maxSkill = 67.8; cliloc = 1111692; break;
                case 4: minSkill = 32.1; maxSkill = 82.1; cliloc = 1111693; break;
                case 5: minSkill = 46.4; maxSkill = 96.4; cliloc = 1111693; break;
                case 6: minSkill = 60.7; maxSkill = 110.7; cliloc = 1111694; break;
                case 7: minSkill = 75.0; maxSkill = 125.0; cliloc = 1111694; break;
            }

            int index = AddCraft(type, cliloc, 1044381 + m_Index++, minSkill, maxSkill, m_RegTypes[(int)regs[0]], 1044353 + (int)regs[0], 1, 1044361 + (int)regs[0]);

            for (int i = 1; i < regs.Length; ++i)
                AddRes(index, m_RegTypes[(int)regs[i]], 1044353 + (int)regs[i], 1, 1044361 + (int)regs[i]);

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, m_Mana);
        }

        private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Reg[] regs)
        {
            int id = GetRegLocalization(regs[0]);
            int index = AddCraft(type, 1061677, 1060509 + spell, minSkill, minSkill + 1.0, m_RegTypes[(int)regs[0]], id, 1, 501627);

            for (int i = 1; i < regs.Length; ++i)
            {
                id = GetRegLocalization(regs[i]);
                AddRes(index, m_RegTypes[(int)regs[i]], id, 1, 501627);
            }

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, mana);
        }

        private void AddMysticSpell(int id, int mana, double minSkill, Type type, params Reg[] regs)
        {
            int index = AddCraft(type, 1111671, id, minSkill, minSkill + 1.0, m_RegTypes[(int)regs[0]], GetRegLocalization(regs[0]), 1, 501627);	//Yes, on OSI it's only 1.0 skill diff'.  Don't blame me, blame OSI.

            for (int i = 1; i < regs.Length; ++i)
                AddRes(index, m_RegTypes[(int)regs[i]], GetRegLocalization(regs[i]), 1, 501627);

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, mana);
        }

        private int GetRegLocalization(Reg reg)
        {
            int loc = 0;

            switch (reg)
            {
                case Reg.BatWing: loc = 1023960; break;
                case Reg.GraveDust: loc = 1023983; break;
                case Reg.DaemonBlood: loc = 1023965; break;
                case Reg.NoxCrystal: loc = 1023982; break;
                case Reg.PigIron: loc = 1023978; break;
                case Reg.Bone: loc = 1023966; break;
                case Reg.DragonBlood: loc = 1023970; break;
                case Reg.FertileDirt: loc = 1023969; break;
                case Reg.DaemonBone: loc = 1023968; break;
            }

            if (loc == 0)
                loc = 1044353 + (int)reg;

            return loc;
        }

        public override void InitCraftList()
        {
            m_Circle = 0;
            m_Mana = 4;

            AddSpell(typeof(ReactiveArmorScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(ClumsyScroll), Reg.Bloodmoss, Reg.Nightshade);
            AddSpell(typeof(CreateFoodScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
            AddSpell(typeof(FeeblemindScroll), Reg.Nightshade, Reg.Ginseng);
            AddSpell(typeof(HealScroll), Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk);
            AddSpell(typeof(MagicArrowScroll), Reg.SulfurousAsh);
            AddSpell(typeof(NightSightScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(WeakenScroll), Reg.Garlic, Reg.Nightshade);

            m_Circle = 1;
            m_Mana = 6;

            AddSpell(typeof(AgilityScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(CunningScroll), Reg.Nightshade, Reg.MandrakeRoot);
            AddSpell(typeof(CureScroll), Reg.Garlic, Reg.Ginseng);
            AddSpell(typeof(HarmScroll), Reg.Nightshade, Reg.SpidersSilk);
            AddSpell(typeof(MagicTrapScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(MagicUnTrapScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
            AddSpell(typeof(ProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.SulfurousAsh);
            AddSpell(typeof(StrengthScroll), Reg.Nightshade, Reg.MandrakeRoot);

            m_Circle = 2;
            m_Mana = 9;

            AddSpell(typeof(BlessScroll), Reg.Garlic, Reg.MandrakeRoot);
            AddSpell(typeof(FireballScroll), Reg.BlackPearl);
            AddSpell(typeof(MagicLockScroll), Reg.Bloodmoss, Reg.Garlic, Reg.SulfurousAsh);
            AddSpell(typeof(PoisonScroll), Reg.Nightshade);
            AddSpell(typeof(TelekinisisScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(TeleportScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(UnlockScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
            AddSpell(typeof(WallOfStoneScroll), Reg.Bloodmoss, Reg.Garlic);

            m_Circle = 3;
            m_Mana = 11;

            AddSpell(typeof(ArchCureScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
            AddSpell(typeof(ArchProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(CurseScroll), Reg.Garlic, Reg.Nightshade, Reg.SulfurousAsh);
            AddSpell(typeof(FireFieldScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(GreaterHealScroll), Reg.Garlic, Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Ginseng);
            AddSpell(typeof(LightningScroll), Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(ManaDrainScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.MandrakeRoot);
            AddSpell(typeof(RecallScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot);

            m_Circle = 4;
            m_Mana = 14;

            AddSpell(typeof(BladeSpiritsScroll), Reg.BlackPearl, Reg.Nightshade, Reg.MandrakeRoot);
            AddSpell(typeof(DispelFieldScroll), Reg.BlackPearl, Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(IncognitoScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Nightshade);
            AddSpell(typeof(MagicReflectScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(MindBlastScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
            AddSpell(typeof(ParalyzeScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(PoisonFieldScroll), Reg.BlackPearl, Reg.Nightshade, Reg.SpidersSilk);
            AddSpell(typeof(SummonCreatureScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            m_Circle = 5;
            m_Mana = 20;

            AddSpell(typeof(DispelScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(EnergyBoltScroll), Reg.BlackPearl, Reg.Nightshade);
            AddSpell(typeof(ExplosionScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(InvisibilityScroll), Reg.Bloodmoss, Reg.Nightshade);
            AddSpell(typeof(MarkScroll), Reg.Bloodmoss, Reg.BlackPearl, Reg.MandrakeRoot);
            AddSpell(typeof(MassCurseScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
            AddSpell(typeof(ParalyzeFieldScroll), Reg.BlackPearl, Reg.Ginseng, Reg.SpidersSilk);
            AddSpell(typeof(RevealScroll), Reg.Bloodmoss, Reg.SulfurousAsh);

            m_Circle = 6;
            m_Mana = 40;

            AddSpell(typeof(ChainLightningScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(EnergyFieldScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(FlamestrikeScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(GateTravelScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(ManaVampireScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(MassDispelScroll), Reg.BlackPearl, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(MeteorSwarmScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh, Reg.SpidersSilk);
            AddSpell(typeof(PolymorphScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            m_Circle = 7;
            m_Mana = 50;

            AddSpell(typeof(EarthquakeScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Ginseng, Reg.SulfurousAsh);
            AddSpell(typeof(EnergyVortexScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Nightshade);
            AddSpell(typeof(ResurrectionScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Ginseng);
            AddSpell(typeof(SummonAirElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(SummonDaemonScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(SummonEarthElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(SummonFireElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(SummonWaterElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            AddNecroSpell(0, 23, 39.6, typeof(AnimateDeadScroll), Reg.GraveDust, Reg.DaemonBlood);
            AddNecroSpell(1, 13, 19.6, typeof(BloodOathScroll), Reg.DaemonBlood);
            AddNecroSpell(2, 11, 19.6, typeof(CorpseSkinScroll), Reg.BatWing, Reg.GraveDust);
            AddNecroSpell(3, 7, 19.6, typeof(CurseWeaponScroll), Reg.PigIron);
            AddNecroSpell(4, 11, 19.6, typeof(EvilOmenScroll), Reg.BatWing, Reg.NoxCrystal);
            AddNecroSpell(5, 11, 39.6, typeof(HorrificBeastScroll), Reg.BatWing, Reg.DaemonBlood);
            AddNecroSpell(6, 23, 69.6, typeof(LichFormScroll), Reg.GraveDust, Reg.DaemonBlood, Reg.NoxCrystal);
            AddNecroSpell(7, 17, 29.6, typeof(MindRotScroll), Reg.BatWing, Reg.DaemonBlood, Reg.PigIron);
            AddNecroSpell(8, 5, 19.6, typeof(PainSpikeScroll), Reg.GraveDust, Reg.PigIron);
            AddNecroSpell(9, 17, 49.6, typeof(PoisonStrikeScroll), Reg.NoxCrystal);
            AddNecroSpell(10, 29, 64.6, typeof(StrangleScroll), Reg.DaemonBlood, Reg.NoxCrystal);
            AddNecroSpell(11, 17, 29.6, typeof(SummonFamiliarScroll), Reg.BatWing, Reg.GraveDust, Reg.DaemonBlood);
            AddNecroSpell(12, 23, 98.6, typeof(VampiricEmbraceScroll), Reg.BatWing, Reg.NoxCrystal, Reg.PigIron);
            AddNecroSpell(13, 41, 79.6, typeof(VengefulSpiritScroll), Reg.BatWing, Reg.GraveDust, Reg.PigIron);
            AddNecroSpell(14, 23, 59.6, typeof(WitherScroll), Reg.GraveDust, Reg.NoxCrystal, Reg.PigIron);
            AddNecroSpell(15, 17, 19.6, typeof(WraithFormScroll), Reg.NoxCrystal, Reg.PigIron);
            AddNecroSpell(16, 40, 79.6, typeof(ExorcismScroll), Reg.NoxCrystal, Reg.GraveDust);

            int index;

            index = AddCraft(typeof(EnchantedSwitch), 1044294, 1072893, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
            AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
            AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
            AddRes(index, typeof(SwitchItem), 1073464, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(RunedPrism), 1044294, 1073465, 45.0, 95.0, typeof(BlankScroll), 1044377, 1, 1044378);
            AddRes(index, typeof(SpidersSilk), 1044360, 1, 1044253);
            AddRes(index, typeof(BlackPearl), 1044353, 1, 1044253);
            AddRes(index, typeof(HollowPrism), 1072895, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(Runebook), 1044294, 1041267, 45.0, 95.0, typeof(BlankScroll), 1044377, 8, 1044378);
            AddRes(index, typeof(RecallScroll), 1044445, 1, 1044253);
            AddRes(index, typeof(GateTravelScroll), 1044446, 1, 1044253);            

            AddCraft(typeof(BulkOrders.BulkOrderBook), 1044294, 1028793, 65.0, 115.0, typeof(BlankScroll), 1044377, 10, 1044378);

            AddCraft(typeof(Spellbook), 1044294, 1023834, 50.0, 126, typeof(BlankScroll), 1044377, 10, 1044378);

            index = AddCraft(typeof(ScrappersCompendium), 1044294, 1072940, 75.0, 125.0, typeof(BlankScroll), 1044377, 100, 1044378);
            AddRes(index, typeof(DreadHornMane), 1032682, 1, 1044253);
            AddRes(index, typeof(Taint), 1032679, 10, 1044253);
            AddRes(index, typeof(Corruption), 1032676, 10, 1044253);
            AddRecipe(index, (int)TinkerRecipes.ScrappersCompendium);
            ForceNonExceptional(index);

            index = AddCraft(typeof(SpellbookEngraver), 1044294, 1072151, 75.0, 100.0, typeof(Feather), 1044562, 1, 1044563);
            AddRes(index, typeof(BlackPearl), 1015001, 7, 1044253);
                        
            AddCraft(typeof(MysticBook), 1044294, 1031677, 50.0, 100.0, typeof(BlankScroll), 1044377, 10, 1044378);

            AddCraft(typeof(NecromancerSpellbook), 1044294, 1074909, 50.0, 100.0, typeof(BlankScroll), 1044377, 10, 1044378);

            index = AddCraft(typeof(ExodusSummoningRite), 1044294, 1153498, 95.0, 120.0, typeof(DaemonBlood), 1023965, 5, 1044253);
            AddRes(index, typeof(Taint), 1032679, 1, 1044253);
            AddRes(index, typeof(DaemonBone), 1017412, 5, 1044253);
            AddRes(index, typeof(SummonDaemonScroll), 1016017, 1, 1044253);

            index = AddCraft(typeof(PropheticManuscript), 1044294, 1155631, 90.0, 115.0, typeof(AncientParchment), 1155627, 10, 1044253);
            AddRes(index, typeof(AntiqueDocumentsKit), 1155630, 1, 1044253);
            AddRes(index, typeof(WoodPulp), 1113136, 10, 1113289);
            AddRes(index, typeof(Beeswax), 1025154, 5, 1044253);

            AddCraft(typeof(BlankScroll), 1044294, 1023636, 50.0, 100.0, typeof(WoodPulp), 1113136, 1, 1044378);

            index = AddCraft(typeof(ScrollBinderDeed), 1044294, 1113135, 75.0, 125.0, typeof(WoodPulp), 1113136, 1, 1044253);
            SetItemHue(index, 1641);

            index = AddCraft(typeof(GargoyleBook100), 1044294, 1113290, 60.0, 100.0, typeof(BlankScroll), 1044377, 40, 1044378);
            AddRes(index, typeof(Beeswax), 1025154, 2, 1053098);

            index = AddCraft(typeof(GargoyleBook200), 1044294, 1113291, 72.0, 100.0, typeof(BlankScroll), 1044377, 40, 1044378);
            AddRes(index, typeof(Beeswax), 1025154, 4, 1053098);

            index = AddCraft(typeof(RunicAtlas), 1044294, 1156443, 45.0, 95.0, typeof(BlankScroll), 1044377, 24, 1044378);
            AddRes(index, typeof(RecallRune), 1044447, 3, 1044253);
            AddRes(index, typeof(RecallScroll), 1044445, 3, 1044253);
            AddRes(index, typeof(GateTravelScroll), 1044446, 3, 1044253);
            AddRecipe(index, (int)InscriptionRecipes.RunicAtlas);

            AddMysticSpell(1031678, 4, 0.0, typeof(NetherBoltScroll), Reg.SulfurousAsh, Reg.BlackPearl);
            AddMysticSpell(1031679, 4, 0.0, typeof(HealingStoneScroll), Reg.Bone, Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk);
            AddMysticSpell(1031680, 6, 0.0, typeof(PurgeMagicScroll), Reg.FertileDirt, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddMysticSpell(1031681, 6, 0.0, typeof(EnchantScroll), Reg.SpidersSilk, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddMysticSpell(1031682, 9, 3.5, typeof(SleepScroll), Reg.SpidersSilk, Reg.BlackPearl, Reg.Nightshade);
            AddMysticSpell(1031683, 9, 3.5, typeof(EagleStrikeScroll), Reg.SpidersSilk, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Bone);
            AddMysticSpell(1031684, 11, 17.8, typeof(AnimatedWeaponScroll), Reg.Bone, Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade);
            AddMysticSpell(1031685, 11, 17.8, typeof(StoneFormScroll), Reg.Bloodmoss, Reg.FertileDirt, Reg.Garlic);
            AddMysticSpell(1031686, 14, 32.1, typeof(SpellTriggerScroll), Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Garlic, Reg.DragonBlood);
            AddMysticSpell(1031687, 14, 32.1, typeof(MassSleepScroll), Reg.SpidersSilk, Reg.Nightshade, Reg.Ginseng);
            AddMysticSpell(1031688, 20, 46.4, typeof(CleansingWindsScroll), Reg.Ginseng, Reg.Garlic, Reg.DragonBlood, Reg.MandrakeRoot);
            AddMysticSpell(1031689, 20, 46.4, typeof(BombardScroll), Reg.Garlic, Reg.DragonBlood, Reg.SulfurousAsh, Reg.Bloodmoss);
            AddMysticSpell(1031690, 40, 60.7, typeof(SpellPlagueScroll), Reg.DaemonBone, Reg.DragonBlood, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh, Reg.DaemonBone);
            AddMysticSpell(1031691, 40, 60.7, typeof(HailStormScroll), Reg.DragonBlood, Reg.BlackPearl, Reg.MandrakeRoot, Reg.Bloodmoss);
            AddMysticSpell(1031692, 50, 75.0, typeof(NetherCycloneScroll), Reg.Bloodmoss, Reg.Nightshade, Reg.SulfurousAsh, Reg.MandrakeRoot);
            AddMysticSpell(1031693, 50, 75.0, typeof(RisingColossusScroll), Reg.DaemonBone, Reg.FertileDirt, Reg.DragonBlood, Reg.Nightshade, Reg.MandrakeRoot);

            MarkOption = true;
        }
    }
}
