// Created by Aremihca Modifyed by Neshoba
using System;
using Server;
using Server.Items;
namespace Server.Items
{
    public class TurtleFeet : ThighBoots
    {

	  public override int ArtifactRarity { get { return 15; } }

	  [Constructable]
        public TurtleFeet()
            : base(11012)
        {
            Hue = 2871;
            Name = "Turtle Feet";
            Attributes.BonusStr = 15;
            Attributes.BonusInt = 15;
            Attributes.BonusDex = 15;
            Attributes.BonusHits = 15;
            Attributes.BonusStam = 15;
            Attributes.BonusMana = 5;
            Attributes.RegenHits = 5;
            Attributes.RegenStam = 5;
            Attributes.AttackChance = 12;
            Attributes.DefendChance = 10;
            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 15;
            Attributes.LowerManaCost = 15;
            Attributes.LowerRegCost = 15;
            Attributes.Luck = 200;
            LootType = LootType.Blessed;
        }
        public TurtleFeet(Serial serial)
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
