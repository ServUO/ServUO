// Created by Neptune
using System;
using Server;

namespace Server.Items
{
    public class FTGWatch : GoldBracelet
    {


        [Constructable]
        public FTGWatch()
        {
            Name = "Gold Watch of Father Time";
            

            Attributes.BonusInt = 30;
              		Attributes.BonusDex = 30;
			Attributes.BonusStr = 30;
             		Attributes.BonusHits = 30;
			Attributes.BonusMana = 30;
			Attributes.BonusStam = 30;
              		Attributes.CastRecovery = 3;
              		Attributes.CastSpeed = 3;
              		Attributes.Luck = 600;
			Attributes.RegenMana = 15;
            		Attributes.RegenHits = 15;
            		Attributes.RegenStam = 15;
              		Attributes.NightSight = 1;
              		Attributes.WeaponSpeed = 80;
              		Attributes.WeaponDamage = 130;


        }

        public FTGWatch(Serial serial)
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