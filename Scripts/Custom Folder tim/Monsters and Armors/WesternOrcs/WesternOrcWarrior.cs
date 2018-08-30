////////////////////////////////////////////////////////////////////////////////////////////////
//********************************************************************************************//
//* Created by Seraph035 : This script is free to use, edit, abuse, or otherwise tamper with.*//
//* Please retain this header when copying or modifying in order to assign blame (or credit) *//
//********************************************************************************************//
////////////////////////////////////////////////////////////////////////////////////////////////
using System;using System.Collections;using Server.Items;using Server.ContextMenus;
using Server.Misc;using Server.Network;namespace Server.Mobiles{ [CorpseName( "an orc corpse" )]
public class WesternOrcWarrior : BaseCreature
{public override bool AlwaysAttackable{ get{ return true; } }
public override bool BleedImmune{ get{ return true; } }
public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
public override bool ShowFameTitle{ get { return false; } }
public override bool Uncalmable{ get{ return true; } } 
public override bool CanRummageCorpses{ get{ return true; } }
public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
public override WeaponAbility GetWeaponAbility() { switch (Utility.Random(2)) { default:
case 0: return WeaponAbility.BleedAttack; case 1: return WeaponAbility.MortalStrike; }}
private DateTime m_NextFastHeal;[Constructable] 
public WesternOrcWarrior() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
{SpeechHue = Utility.RandomDyedHue();Title = "the orc";Hue = 2413;HairItemID = 5147;
HairHue = 2413;BaseSoundID = 0x45A;Body = 0x190;Name = NameList.RandomName( "orc" );Fame = 5000;
Karma = -5000;SetStr( 386, 400 );SetDex( 151, 165 );SetInt( 161, 175 );SetDamage( 15, 17 );
VirtualArmor = 40;SetDamageType( ResistanceType.Physical, 100 );
SetResistance( ResistanceType.Physical, 50, 55 );SetResistance( ResistanceType.Fire, 40, 45 );
SetResistance( ResistanceType.Cold, 40, 45 );SetResistance( ResistanceType.Poison, 50, 55 );
SetResistance( ResistanceType.Energy, 40, 45 );SetSkill( SkillName.MagicResist, 84.5, 97.5 );
SetSkill( SkillName.Swords, 97.5,119.5 );SetSkill( SkillName.Tactics, 97.5,119.5 );
AddItem( new StuddedGorget());AddItem( new StuddedGloves());AddItem( new StuddedArms());
AddItem( new StuddedChest());AddItem( new StuddedLegs());AddItem( new ExecutionersAxe());
AddItem( new ThighBoots());}public override void GenerateLoot(){AddLoot( LootPack.FilthyRich );}
public override void OnThink(){if (this.m_NextFastHeal < DateTime.Now){
if (this.Hits < this.HitsMax){this.Hits += (this.HitsMax / 6);this.m_NextFastHeal = 
DateTime.Now + TimeSpan.FromSeconds(10);}}}public WesternOrcWarrior( Serial serial ) : base
( serial ){}public override void Serialize( GenericWriter writer ){ base.Serialize( writer ); 
writer.Write( (int) 0 );}public override void Deserialize( GenericReader reader )
{base.Deserialize( reader );int version = reader.ReadInt();}}}
//Final version born on 3.18.2013 --}@ // 
