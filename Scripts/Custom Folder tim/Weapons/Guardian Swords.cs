// Created with UO Weapon Generator
// Created On: 9/11/2013 3:30:37 PM
// By: Tim

using System;
using Server;

namespace Server.Items
{
    public class GuardianSwords : Daisho
    {
        public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
        public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
        public override int ArtifactRarity{ get{ return 10; } }
        public override int InitMinHits{ get{ return 750; } }
        public override int InitMaxHits{ get{ return 1000; } }

        [Constructable]
        public GuardianSwords()
        {
            Name = "Guardian Swords";
            Hue = 1174;
            LootType = LootType.Blessed;
            Slayer = SlayerName.Exorcism;
            Attributes.SpellChanneling = 1;
            Attributes.NightSight = 1;
            Attributes.BonusStr = 100;
            Attributes.BonusInt = 100;
            Attributes.BonusDex = 100;
            Attributes.RegenHits = 100;
            Attributes.RegenStam = 100;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.HitLeechHits = 100;
            WeaponAttributes.HitLeechStam = 100;
            WeaponAttributes.HitLeechMana = 100;
            Attributes.AttackChance = 100;
            Attributes.DefendChance = 100;
            Attributes.WeaponDamage = 450;
            Attributes.WeaponSpeed = 1000;
            Attributes.Luck = 1000;
            Attributes.ReflectPhysical = 100;
            Attributes.EnhancePotions = 100;
            Attributes.SpellDamage = 100;
            WeaponAttributes.HitPhysicalArea = 100;
            WeaponAttributes.HitColdArea = 100;
            WeaponAttributes.HitFireArea = 100;
            WeaponAttributes.HitEnergyArea = 100;
            WeaponAttributes.HitPoisonArea = 100;
            WeaponAttributes.ResistPhysicalBonus = 100;
            WeaponAttributes.ResistColdBonus = 100;
            WeaponAttributes.ResistFireBonus = 100;
            WeaponAttributes.ResistEnergyBonus = 100;
            WeaponAttributes.ResistPoisonBonus = 100;
            WeaponAttributes.DurabilityBonus = 1000;
            WeaponAttributes.SelfRepair = 100;
            Attributes.CastSpeed = 100;
            Attributes.CastRecovery = 100;
            Attributes.LowerManaCost = 100;
            Attributes.LowerRegCost = 100;
            WeaponAttributes.HitLowerAttack = 100;
            WeaponAttributes.HitLowerDefend = 100;
            WeaponAttributes.HitHarm = 100;
            WeaponAttributes.HitFireball = 100;
            WeaponAttributes.HitLightning = 100;
            WeaponAttributes.HitDispel = 100;
        }

        public GuardianSwords(Serial serial) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    } // End Class
} // End Namespace
