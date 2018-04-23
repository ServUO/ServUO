// Created by Neptune
using System;
using Server;

namespace Server.Items
{
    public class FTWatch : GoldBracelet
    {

        public override int ArtifactRarity { get { return 60; } }

        [Constructable]
        public FTWatch()
        {
            Name = "Watch of Father Time";
            Hue = 0x31D;

            Attributes.BonusInt = 10;
              		Attributes.BonusDex = 10;
			Attributes.BonusStr = 10;
             		Attributes.BonusHits = 10;
			Attributes.BonusMana = 10;
			Attributes.BonusStam = 10;
              		Attributes.CastRecovery = 1;
              		Attributes.CastSpeed = 1;
              		Attributes.Luck = 200;
			Attributes.RegenMana = 5;
            		Attributes.RegenHits = 5;
            		Attributes.RegenStam = 5;
              		Attributes.NightSight = 1;
              		Attributes.WeaponSpeed = 80;
              		Attributes.WeaponDamage = 130;


        }

        public FTWatch(Serial serial)
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