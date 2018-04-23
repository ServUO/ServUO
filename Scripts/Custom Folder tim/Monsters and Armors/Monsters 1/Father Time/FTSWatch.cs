// Created by Neptune
using System;
using Server;

namespace Server.Items
{
    public class FTSWatch : GoldBracelet
    {

        

        [Constructable]
        public FTSWatch()
        {
            Name = "Silver Watch of Father Time";
            Hue = 2040;

            Attributes.BonusInt = 20;
              		Attributes.BonusDex = 20;
			Attributes.BonusStr = 20;
             		Attributes.BonusHits = 20;
			Attributes.BonusMana = 20;
			Attributes.BonusStam = 20;
              		Attributes.CastRecovery = 2;
              		Attributes.CastSpeed = 2;
              		Attributes.Luck = 400;
			Attributes.RegenMana = 10;
            		Attributes.RegenHits = 10;
            		Attributes.RegenStam = 10;
              		Attributes.NightSight = 1;
              		Attributes.WeaponSpeed = 80;
              		Attributes.WeaponDamage = 130;


        }

        public FTSWatch(Serial serial)
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