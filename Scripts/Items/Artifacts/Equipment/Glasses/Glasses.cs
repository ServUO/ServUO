using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishGlasses), true)]
    public class Glasses : BaseArmor, IRepairable
	{
        public CraftSystem RepairSystem { get { return DefTinkering.CraftSystem; } }

        [Constructable]
        public Glasses()
            : base(0x2FB8)
        {
            Weight = 2.0;			
        }

        public Glasses(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 45; } }
        public override int OldStrReq { get { return 40; } }
        public override int ArmorBase { get { return 30; } }

        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Leather;
            }
        }

        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        public override ArmorMeditationAllowance DefMedAllowance
        {
            get
            {
                return ArmorMeditationAllowance.All;
            }
        }

        public override bool CanEquip(Mobile m)
        {
            if (m.NetState != null && !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072791); // You must upgrade to Mondain's Legacy in order to use that item.
				
                return false;
            }
			
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);			
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                xWeaponAttributesDeserializeHelper(reader, this);
            }
        }
    }
}
