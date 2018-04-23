// Created by Nept
using System;
using Server;

namespace Server.Items
{
    public class SinBlade : BoneHarvester
    {
        public override int ArtifactRarity { get { return 23; } }
	public override int AosMinDamage{ get{ return 30; } }
	public override int OldMinDamage{ get{ return 30; } }
	public override int AosMaxDamage{ get{ return 30; } }
	public override int OldMaxDamage{ get{ return 30; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public SinBlade()
        {
            Weight = 0.0;
            Name = "Blade of Wrath";
            Speed = Utility.Random( 36, 50 );


            Hue = 2075;
            Slayer = SlayerName.ElementalBan;
            Attributes.BonusStr = Utility.Random( 1, 20 );
            Attributes.BonusInt = Utility.Random( 1, 20 );
            Attributes.BonusDex = Utility.Random( 1, 20 );
            Attributes.BonusHits = Utility.Random( 1, 20 );
            Attributes.BonusStam = Utility.Random( 1, 20 );
            Attributes.BonusMana = Utility.Random( 1, 20 );
            WeaponAttributes.HitLeechHits = Utility.Random( 1, 120 );
            WeaponAttributes.HitLeechStam = Utility.Random( 1, 120 );
            WeaponAttributes.HitLeechMana = Utility.Random( 1, 120 );
            Attributes.AttackChance = Utility.Random( 1, 35 );
            Attributes.DefendChance = Utility.Random( 1, 35 );
            Attributes.WeaponDamage = Utility.Random( 1, 120 );
            Attributes.WeaponSpeed = Utility.Random( 1, 75 );
            Attributes.SpellChanneling = 1;
            Attributes.ReflectPhysical = Utility.Random( 1, 35 );
            Attributes.SpellDamage = Utility.Random( 1, 75 );
            WeaponAttributes.ResistPhysicalBonus = 20;
            WeaponAttributes.ResistColdBonus = 20;
            WeaponAttributes.ResistFireBonus = 20;
            WeaponAttributes.ResistEnergyBonus = 20;
            WeaponAttributes.ResistPoisonBonus = 20;
            WeaponAttributes.SelfRepair = 5;
            WeaponAttributes.HitLowerAttack = Utility.Random( 1, 120 );
            WeaponAttributes.HitLowerDefend = Utility.Random( 1, 120 );
            WeaponAttributes.HitHarm = Utility.Random( 1, 120 );
            WeaponAttributes.HitFireball = Utility.Random( 1, 120 );
            WeaponAttributes.HitLightning = Utility.Random( 1, 120 );
            WeaponAttributes.HitDispel = Utility.Random( 1, 120 );
        }

        public SinBlade(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}