// By Nerun

using System;

namespace Server.Items
{
    public class StaffRing : BaseRing
    {
        [Constructable]
        public StaffRing()
            : base(0x108a)
        {
            this.Weight = 1.0;
            this.Name = "The Staff Ring";
            this.Attributes.NightSight = 1;
            this.Attributes.AttackChance = 20;
            this.Attributes.LowerRegCost = 100;
            this.Attributes.LowerManaCost = 100;
            this.Attributes.RegenHits = 12;
            this.Attributes.RegenStam = 24;
            this.Attributes.RegenMana = 18;
            this.Attributes.SpellDamage = 30;
            this.Attributes.CastRecovery = 6;
            this.Attributes.CastSpeed = 4;
            this.LootType = LootType.Blessed;
        }

        public StaffRing(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.IsPlayer())
            {
                from.SendMessage("Your not a Staff member, you may not wear this Item..."); 
                this.Delete();
            }
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.IsPlayer())
            {
                from.SendMessage("Your not a Staff member, you may not wear this Item..."); 
                this.Delete();
            }
            return true;
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