
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class VollemHeldInCrystal : BaseImprisonedMobile
    {
        public override int LabelNumber => 1113629;  // A Vollem Held in Crystal

        [Constructable]
        public VollemHeldInCrystal()
            : base(0x1f19)
        {
            Hue = 1154;
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override BaseCreature Summon => new VollemHeld();

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.SendGump(new ConfirmBreakCrystalGump(this));
            else
                from.SendLocalizedMessage(1010095); // This must be on your person to use.
        }

        public VollemHeldInCrystal(Serial serial)
            : base(serial)
        {
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

    public class VollemHeld : Vollem
    {
        public override bool IsMechanical => false;

        [Constructable]
        public VollemHeld()
            : base()
        {
        }

        public VollemHeld(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                PetTrainingHelper.GetAbilityProfile(this, true).TokunoTame = true;
            }
        }
    }
}
