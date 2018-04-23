using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;
using Server.Engines.XmlSpawner2;
namespace Server.Items
{
    //item derived from BaseResourceKey
    public class AugmentKey : BaseStoreKey, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.Seer)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        //set the # of columns of entries to display on the gump.. default is 2
        public override int DisplayColumns { get { return 3; } }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

                entry.Add(new ResourceEntry(typeof(SocketHammer), "Socket Hammer", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(ExceptionalSocketHammer),"Exceptional Socket Hammer",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(HammerOfRecovery),"Hammer Of Recovery",0,25,-13,9));
              //  entry.Add(new ResourceEntry(typeof(SocketDeedPlusOne),"Socket Deed +1",0,25,-5,9));
              //  entry.Add(new ResourceEntry(typeof(SocketDeedPlusTwo), "Socket Deed +2", 0, 25, 7, 9));
               // entry.Add(new ResourceEntry(typeof(SocketDeedPlusThree), "Socket Deed +3", 0, 25, 7, 9));
               // entry.Add(new ResourceEntry(typeof(SocketDeedPlusFour), "Socket Deed +4", 0, 25, 6, 9));
              //  entry.Add(new ResourceEntry(typeof(SocketDeedPlusFive), "Socket Deed +5", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(MythicAmethyst), "Mythic Amethyst", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(LegendaryAmethyst), "Legendary Amethyst", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(AncientAmethyst), "Ancient Amethyst", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(MythicDiamond), "Mythic Diamond", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(LegendaryDiamond), "Legendary Diamond", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(AncientDiamond), "Ancient Diamond", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(MythicEmerald), "Mythic Emerald", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(LegendaryEmerald), "Legendary Emerald", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(AncientEmerald), "Ancient Emerald", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(FenCrystal), "Fen Crystal", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(RhoCrystal), "Rho Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(RysCrystal), "Rys Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(WyrCrystal), "Wyr Crystal", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(FreCrystal), "Fre Crystal", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(TorCrystal), "Tor Crystal", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(VelCrystal), "Vel Crystal", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(XenCrystal), "Xen Crystal", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(PolCrystal), "Pol Crystal", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(WolCrystal), "Wol Crystal", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(BalCrystal), "Bal Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(TalCrystal), "Tal Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(JalCrystal), "Jal Crystal", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(RalCrystal), "Ral Crystal", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(KalCrystal), "Kal Crystal", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(MythicEmerald), "Mythic Emerald", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(LegendaryEmerald), "Legendary Emerald", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(AncientEmerald), "Ancient Emerald", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(RadiantRhoCrystal), "Radiant Rho Crystal", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(RadiantRysCrystal), "Radiant Rys Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(RadiantWyrCrystal), "Radiant Wyr Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(RadiantFreCrystal), "Radiant Fre Crystal", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(RadiantTorCrystal), "Radiant Tor Crystal", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(RadiantVelCrystal), "Radiant Vel Crystal", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(RadiantXenCrystal), "Radiant Xen Crystal", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(RadiantPolCrystal), "Radiant Pol Crystal", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(RadiantWolCrystal), "Radiant Wol Crystal", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(RadiantBalCrystal), "Radiant Bal Crystal", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(RadiantTalCrystal), "Radiant Tal Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(RadiantJalCrystal), "Radiant Jal Crystal", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(RadiantRalCrystal), "Radiant Ral Crystal", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(RadiantKalCrystal), "Radiant Kal Crystal", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(MythicRuby), "Mythic Ruby", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(LegendaryRuby), "Legendary Ruby", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(AncientRuby), "Ancient Ruby", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(TyrRune), "Tyr Rune", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(AhmRune), "Ahm Rune", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(MorRune), "Mor Rune", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(MefRune), "Mef Rune", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(YlmRune), "Ylm Rune", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(KotRune), "Kot Rune", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(JorRune), "Jor Rune", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(MythicSapphire), "Mythic Sapphire", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(LegendarySapphire), "Legendary Sapphire", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(AncientSapphire), "Ancient Sapphire", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(MythicSkull), "Mythic Skull", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(AncientSkull), "Ancient Skull", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(LegendarySkull), "Legendary Skull", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(MythicTourmaline), "Mythic Tourmaline", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(LegendaryTourmaline), "Legendary Tourmaline", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(AncientTourmaline), "Ancient Tourmaline", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(MythicWood), "Mythic Wood", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(LegendaryWood), "Legendary Wood", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(AncientWood), "Ancient Wood", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringGranite), "Glimmering Granite", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(GlimmeringHeartstone), "Glimmering Heartstone", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringGypsum), "Glimmering Gypsum", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringIronOre), "Glimmering IronOre", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringOnyx), "Glimmering Onyx", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringMarble), "Glimmering Marble", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringPetrifiedWood), "Glimmering PetrifiedWood", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringLimestone), "Glimmering Limestone", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringBloodrock), "Glimmering Bloodrock", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringRockNite), "Glimmering RockNite", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(GlimmeringDiamond), "Glimmering Diamond", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringKimberlite), "Glimmering Kimberlite", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringEnderbite), "Glimmering Enderbite", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringEssexite), "Glimmering Essexite", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringGranophyre), "Glimmering Granophyre", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringDunite), "Glimmering Dunite", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringAluminum), "Glimmering Aluminum", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringBoxite), "Glimmering Boxite", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringCoesite), "Glimmering Coesite", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(GlimmeringKetite), "Glimmering Ketite", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringPhantomQuartz), "Glimmering PhantomQuartz", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringAmertrine), "Glimmering Amertrine", 0, 25, -13, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringCarnelian), "Glimmering Carnelian", 0, 25, -5, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringMacroCrystaline), "Glimmering MacroCrystaline", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringBlueChalcedonny), "Glimmering BlueChalcedonny", 0, 25, 7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringCryoprase), "Glimmering Cryoprase", 0, 25, 6, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringDendritic), "Glimmering Dendritic", 0, 25, -7, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringLemonQuartz), "Glimmering LemonQuartz", 0, 25, 11, 8));
                entry.Add(new ResourceEntry(typeof(GlimmeringDiorite), "Glimmering Diorite", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringHornblendite), "Glimmering Hornblendite", 0, 25, -11, 9));
                entry.Add(new ResourceEntry(typeof(GlimmeringRhydocite), "Glimmering Rhydocite", 0, 25, -13, 9));
               /*/ entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));
                /*/
                return entry;
            }
        }

        [Constructable]
        public AugmentKey() : base(0x0)      //hue 1154
        {
            Name = "Augment Key Storage";

            ItemID = 0x9BC8;
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Augment Key Storage ";

            store.Dynamic = false;
            store.OfferDeeds = false;

            return store;
        }

        //serial constructor
        public AugmentKey(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        //events

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write((bool)m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }
}