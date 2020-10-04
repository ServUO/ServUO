using Server.Mobiles;

namespace Server.Items
{
    public class DeathCertificate : Item
    {
        public override int LabelNumber => 1116716;

        private string m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Owner => m_Owner;

        public DeathCertificate(Mobile owner)
            : base(0x14F0)
        {
            if (owner is PirateCaptain)
            {
                PirateCaptain capt = (PirateCaptain)owner;

                if (capt.PirateName > 0)
                    m_Owner = string.Format("#{0}\t#{1}\t#{2}", capt.Adjective, capt.Noun, capt.PirateName);
                else
                    m_Owner = string.Format("#{0}\t#{1}\t{2}", capt.Adjective, capt.Noun, Name);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Owner != null)
            {
                list.Add(1116690, m_Owner);
            }
        }

        public DeathCertificate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Owner = reader.ReadString();
        }
    }
}