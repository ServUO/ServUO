//////////////////////////////////////////////////////////////////
////////Made by Oberon shard owner of Solus Realm :///////////////
/////////////////////////////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class AdamantiumGorget : PlateGorget
    {
        [Constructable]
        public AdamantiumGorget()
        {
            this.Weight = 2;
            this.Hue = 2803;
            this.Name = "Adamantium Gorget";

            this.Attributes.AttackChance = 10;
            this.Attributes.BonusDex = 30;
            //this.Attributes.BonusHits = 54;
            this.Attributes.BonusInt = 20;
            //this.Attributes.BonusMana = 50;
            //this.Attributes.BonusStam = 48;
            //this.Attributes.CastRecovery = 100;
            this.Attributes.CastSpeed = 60;
            this.Attributes.DefendChance = 20;
            //this.Attributes.EnhancePotions = 5;
            this.Attributes.LowerManaCost = 66;
            this.Attributes.LowerRegCost = 60;
            //this.Attributes.Luck = 100;
            this.Attributes.NightSight = 1;
            //this.Attributes.ReflectPhysical = 90;
            this.Attributes.RegenHits = 70;
            this.Attributes.RegenMana = 40;
            this.Attributes.RegenStam = 44;
            this.Attributes.SpellChanneling = 1;
            //this.Attributes.SpellDamage = 30;
            this.Attributes.WeaponDamage = 10;

            this.ArmorAttributes.DurabilityBonus = 40;
            this.ArmorAttributes.LowerStatReq = 38;
            this.ArmorAttributes.SelfRepair = 1;

            this.ColdBonus = 74;
            this.EnergyBonus = 40;
            this.FireBonus = 20;
            this.PhysicalBonus = 50;
            this.PoisonBonus = 10;
            this.StrBonus = 55;

            this.LootType = LootType.Blessed;
        }

        public AdamantiumGorget( Serial serial ) : base( serial )
        {
        }

		public override int ArtifactRarity{ get{ return 95; } }
             
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
    }
}