// By Nerun

namespace Server.Items
{
    public class StaffRing : BaseRing
    {
        [Constructable]
        public StaffRing()
            : base(0x108a)
        {
            Name = "The Staff Ring";
            Attributes.NightSight = 1;
            Attributes.AttackChance = 20;
            Attributes.LowerRegCost = 100;
            Attributes.LowerManaCost = 100;
            Attributes.RegenHits = 12;
            Attributes.RegenStam = 24;
            Attributes.RegenMana = 18;
            Attributes.SpellDamage = 30;
            Attributes.CastRecovery = 6;
            Attributes.CastSpeed = 4;
            LootType = LootType.Blessed;
        }

        public StaffRing(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.IsPlayer())
            {
                from.SendMessage("This item is to only be used by staff members.");
                Delete();
            }
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.IsPlayer())
            {
                from.SendMessage("This item is to only be used by staff members.");
                Delete();
            }
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}