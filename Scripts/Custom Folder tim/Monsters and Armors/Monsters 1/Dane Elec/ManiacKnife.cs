// Created by Nept
using System;
using Server;

namespace Server.Items
{
    public class ManiacTailorKnife : SkinningKnife
    {
        public override int ArtifactRarity { get { return 23; } }
	
	public override int AosMinDamage{ get{ return 10; } }
	public override int OldMinDamage{ get{ return 10; } }
	public override int AosMaxDamage{ get{ return 30; } }
	public override int OldMaxDamage{ get{ return 30; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public ManiacTailorKnife()
        {
            Weight = 0.0;
            Name = "Knife of the Maniacal Tailor";
            Speed = Utility.Random( 50, 75 );


            Hue = 2075;
            Slayer = SlayerName.ElementalBan;
            Attributes.BonusStr = Utility.Random( 5, 35 );
            Attributes.BonusInt = Utility.Random( 5, 35 );
            Attributes.BonusDex = Utility.Random( 5, 35 );
            Attributes.BonusHits = Utility.Random( 5, 35 );
            Attributes.BonusStam = Utility.Random( 5, 35 );
            Attributes.BonusMana = Utility.Random( 5, 35 );
            Attributes.RegenHits = Utility.Random( 5, 35 );
            Attributes.RegenStam = Utility.Random( 5, 35 );
            WeaponAttributes.HitLeechHits = Utility.Random( 5, 100 );
            WeaponAttributes.HitLeechStam = Utility.Random( 5, 100 );
            WeaponAttributes.HitLeechMana = Utility.Random( 5, 100 );
            Attributes.AttackChance = Utility.Random( 5, 35 );
            Attributes.DefendChance = Utility.Random( 5, 35 );
            Attributes.WeaponDamage = Utility.Random( 5, 35 );
            Attributes.WeaponSpeed = Utility.Random( 5, 35 );
            Attributes.Luck = 100;
            Attributes.ReflectPhysical = Utility.Random( 5, 35 );
            Attributes.SpellDamage = Utility.Random( 5, 35 );
            WeaponAttributes.ResistPhysicalBonus = 20;
            WeaponAttributes.ResistColdBonus = 20;
            WeaponAttributes.ResistFireBonus = 20;
            WeaponAttributes.ResistEnergyBonus = 20;
            WeaponAttributes.ResistPoisonBonus = 20;
            WeaponAttributes.SelfRepair = 5;
            Attributes.CastSpeed = 5;
            Attributes.CastRecovery = 5;
            Attributes.LowerManaCost = Utility.Random( 5, 35 );
            Attributes.LowerRegCost = Utility.Random( 5, 35 );
            WeaponAttributes.HitLowerAttack = Utility.Random( 5, 35 );
            WeaponAttributes.HitLowerDefend = Utility.Random( 5, 35 );
            WeaponAttributes.HitHarm = Utility.Random( 5, 35 );
            WeaponAttributes.HitFireball = Utility.Random( 5, 35 );
            WeaponAttributes.HitLightning = Utility.Random( 5, 35 );
            WeaponAttributes.HitDispel = Utility.Random( 5, 35 );
        }

        public ManiacTailorKnife(Serial serial)
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