// Created by Nept
using System;
using Server;

namespace Server.Items
{
    public class VirtueBlade : Longsword
    {
        public override int ArtifactRarity { get { return 23; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public VirtueBlade()
        {
            Weight = 0.0;
            Name = "Blade of Virtue";
            Speed = Utility.Random( 30, 40 );


            Hue = 2075;
            Slayer = SlayerName.ElementalBan;
            Attributes.BonusStr = Utility.Random( 1, 25 );
            Attributes.BonusInt = Utility.Random( 1, 25 );
            Attributes.BonusDex = Utility.Random( 1, 25 );
            Attributes.BonusHits = Utility.Random( 1, 25 );
            Attributes.BonusStam = Utility.Random( 1, 25 );
            Attributes.BonusMana = Utility.Random( 1, 25 );
            Attributes.RegenHits = Utility.Random( 1, 25 );
            Attributes.RegenStam = Utility.Random( 1, 25 );
            WeaponAttributes.HitLeechHits = Utility.Random( 1, 100 );
            WeaponAttributes.HitLeechStam = Utility.Random( 1, 100 );
            WeaponAttributes.HitLeechMana = Utility.Random( 1, 100 );
            Attributes.AttackChance = Utility.Random( 1, 25 );
            Attributes.DefendChance = Utility.Random( 1, 25 );
            Attributes.WeaponDamage = Utility.Random( 1, 25 );
            Attributes.WeaponSpeed = Utility.Random( 1, 25 );
            Attributes.Luck = 100;
            Attributes.ReflectPhysical = Utility.Random( 1, 25 );
            Attributes.SpellDamage = Utility.Random( 1, 25 );
            WeaponAttributes.ResistPhysicalBonus = 20;
            WeaponAttributes.ResistColdBonus = 20;
            WeaponAttributes.ResistFireBonus = 20;
            WeaponAttributes.ResistEnergyBonus = 20;
            WeaponAttributes.ResistPoisonBonus = 20;
            WeaponAttributes.SelfRepair = 5;
            Attributes.CastSpeed = 5;
            Attributes.CastRecovery = 5;
            Attributes.LowerManaCost = Utility.Random( 1, 25 );
            Attributes.LowerRegCost = Utility.Random( 1, 25 );
            WeaponAttributes.HitLowerAttack = Utility.Random( 1, 25 );
            WeaponAttributes.HitLowerDefend = Utility.Random( 1, 25 );
            WeaponAttributes.HitHarm = Utility.Random( 1, 25 );
            WeaponAttributes.HitFireball = Utility.Random( 1, 25 );
            WeaponAttributes.HitLightning = Utility.Random( 1, 25 );
            WeaponAttributes.HitDispel = Utility.Random( 1, 25 );
        }

        public VirtueBlade(Serial serial)
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