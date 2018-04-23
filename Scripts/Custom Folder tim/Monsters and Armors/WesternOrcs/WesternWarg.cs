////////////////////////////////////////////////////////////////////////////////////////////////
//********************************************************************************************//
//* Created by Seraph035 : This script is free to use, edit, abuse, or otherwise tamper with.*//
//* Please retain this header when copying or modifying in order to assign blame (or credit) *//
//********************************************************************************************//
////////////////////////////////////////////////////////////////////////////////////////////////
using System;using System.Collections;using Server.Items;using Server.Targeting;using 
Server.Misc;namespace Server.Mobiles{[CorpseName( "a warg corpse" )]public class WesternWarg : 
BaseMount{[Constructable]public WesternWarg() : this( "a western warg" ){}public override 
TimeSpan MountAbilityDelay { get { return TimeSpan.FromHours( .003 ); } }public override bool 
DoMountAbility( int damage, Mobile attacker ){if( Rider == null || attacker == null )return 
false;if( Rider.Map == attacker.Map && Rider.InRange( attacker, 1 ) &&  .7 > Utility.RandomDouble() 
){attacker.Paralyze( TimeSpan.FromSeconds( 6 ) );{if ( attacker.Body.IsHuman && !attacker.Mounted )
{attacker.Animate( 22, 7, 1, true, true, 2 );return true;}}}return false;}[Constructable]public 
WesternWarg( string name ) : base( name, 277, 0x3E91, AIType.AI_Animal, FightMode.Weakest, 10, 1, 
0.2, 0.4 ){Hue = Utility.RandomAnimalHue();SetStr( 600, 612 );SetDex( 76, 85 );SetInt( 126, 141 )
;SetDamage( 21, 28 );SetDamageType( ResistanceType.Physical, 100 );SetResistance
( ResistanceType.Physical, 55, 65 );SetResistance( ResistanceType.Fire, 30, 40 );
SetResistance( ResistanceType.Cold, 70, 85 );SetResistance( ResistanceType.Poison, 70, 85 );
SetResistance( ResistanceType.Energy, 40, 55 );SetSkill( SkillName.Wrestling, 90.1, 96.8 );
SetSkill( SkillName.Tactics, 90.3, 99.3 );SetSkill( SkillName.MagicResist, 75.3, 90.0 );
SetSkill( SkillName.Anatomy, 65.5, 69.4 );Fame = 7000;Karma = -7000;Tamable = true;ControlSlots 
= 2;MinTameSkill = 110.1;}public override int Meat{ get{ return 1; } }public override int Hides{ 
get{ return 12; } }public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }public 
override WeaponAbility GetWeaponAbility(){switch (Utility.Random(2)){default:case 0: return 
WeaponAbility.BleedAttack;case 1: return WeaponAbility.Dismount;}}public override void 
OnGaveMeleeAttack( Mobile defender ){if( .7 > Utility.RandomDouble() )defender.Paralyze( TimeSpan
.FromSeconds( 6 ) );{if ( defender.Alive && defender.Body.IsHuman && !defender.Mounted ){defender
.Animate( 22, 7, 1, true, true, 2 );}}base.OnGaveMeleeAttack( defender );}public WesternWarg( 
Serial serial ) : base( serial ){}public override int GetIdleSound() { return 0x577; }public 
override int GetAttackSound() { return 0x576; }public override int GetAngerSound() { return 0x578
; }public override int GetHurtSound(){ return 0x576; }public override int GetDeathSound() { 
return 0x579; }public override void Serialize( GenericWriter writer ){base.Serialize( writer );
writer.Write( (int) 0 );}public override void Deserialize( GenericReader reader ){base.
Deserialize( reader );int version = reader.ReadInt();}}}//Final version born on 3.18.2013 --}@ //