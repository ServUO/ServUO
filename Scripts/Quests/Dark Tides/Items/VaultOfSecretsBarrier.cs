using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class VaultOfSecretsBarrier : Item
    {
        [Constructable]
        public VaultOfSecretsBarrier()
            : base(0x49E)
        {
            this.Movable = false;
            this.Visible = false;
        }

        public VaultOfSecretsBarrier(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.IsStaff())
                return true;

            PlayerMobile pm = m as PlayerMobile;

            if (pm != null && pm.Profession == 4)
            {
                m.SendLocalizedMessage(1060188, "", 0x24); // The wicked may not enter!
                return false;
            }

            return base.OnMoveOver(m);
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