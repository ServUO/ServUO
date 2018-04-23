// Created by Neptune
using System;
using Server;

namespace Server.Items
{
    public class FTRWatch : GoldBracelet
    {

        

        [Constructable]
        public FTRWatch()
        {
            Name = "Rolex Watch of Father Time";
            

            Attributes.BonusInt = 40;
              		Attributes.BonusDex = 40;
			Attributes.BonusStr = 40;
             		Attributes.BonusHits = 40;
			Attributes.BonusMana = 40;
			Attributes.BonusStam = 40;
              		Attributes.CastRecovery = 4;
              		Attributes.CastSpeed = 4;
              		Attributes.Luck = 800;
			Attributes.RegenMana = 20;
            		Attributes.RegenHits = 20;
            		Attributes.RegenStam = 20;
              		Attributes.NightSight = 1;
              		Attributes.WeaponSpeed = 80;
              		Attributes.WeaponDamage = 130;


        }

        public FTRWatch(Serial serial)
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