using System;
using Server;

namespace Server.Items
{
   
    public class BunnySlippers: Shoes
    {
        public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

        [Constructable]
        public BunnySlippers()
        {
            Name = "Bunny Slippers";
            Hue = 902;

            Attributes.AttackChance = 45;
            Attributes.BonusDex = 25;
            Attributes.BonusHits = 20;
            Attributes.BonusInt = 35;
            Attributes.BonusMana = 35;
            Attributes.BonusStam = 37;
            Attributes.CastRecovery = 10;
            Attributes.CastSpeed = 10;
            Attributes.DefendChance = 54;
            //Attributes.EnhancePotions = 30;
            Attributes.LowerManaCost = 50;
            Attributes.LowerRegCost = 50;
            Attributes.Luck = 620;
            Attributes.NightSight = 1;
            Attributes.ReflectPhysical = 55;
            Attributes.RegenHits = 50;
            Attributes.RegenMana = 53;
            Attributes.RegenStam = 54;
            Attributes.SpellChanneling = 1;
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 15.0);
            SkillBonuses.SetValues(1, SkillName.Stealth, 10.0);
            SkillBonuses.SetValues(2, SkillName.Swords, 12.0);
            SkillBonuses.SetValues(3, SkillName.Hiding, 15.0);
            SkillBonuses.SetValues(4, SkillName.AnimalLore, 15.0);
        }

       

        public BunnySlippers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
  
        }
    }
}