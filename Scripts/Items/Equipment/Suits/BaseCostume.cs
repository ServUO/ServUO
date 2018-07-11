using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
    [FlipableAttribute(0x19BC, 0x19BD)]
    public partial class BaseCostume : BaseShield, IDyable
    {
        public bool m_Transformed;
        private int m_Body = 0;
        private int m_Hue = -1;
        private int m_SaveHueMod = -1;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Transformed
        {
            get { return m_Transformed; }
            set { m_Transformed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CostumeBody
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CostumeHue
        {
            get { return m_Hue; }
            set { m_Hue = value; }
        }

        public BaseCostume()
            : base(0x19BC)
        {
            Resource = CraftResource.None;
            Attributes.SpellChanneling = 1;
            Layer = Layer.FirstValid;
            Weight = 3.0;
        }

        public BaseCostume(Serial serial)
            : base(serial)
        {

        }

        private bool EnMask(Mobile from)
        {
            if (from.Mounted || from.Flying) // You cannot use this while mounted or flying. 
            {
                from.SendLocalizedMessage(1010097);
            }
            else if (from.IsBodyMod || from.HueMod > -1)
            {
                from.SendLocalizedMessage(1158010); // You cannot use that item in this form.
            }
            else
            {
                from.BodyMod = m_Body;
                from.HueMod = m_Hue;
                Transformed = true;

                return true;
            }

            return false;
        }

        private void DeMask(Mobile from)
        {
            from.BodyMod = 0;
            from.HueMod = -1;
            Transformed = false;
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            else if (RootParent is Mobile && from != RootParent)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public override bool OnEquip(Mobile from)
        {
            if (!Transformed)
            {
                if (EnMask(from))
                    return true;

                return false;
            }

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile && Transformed)
            {
                DeMask((Mobile)parent);
            }

            base.OnRemoved(parent);
        }

        public static void OnDamaged(Mobile m)
        {
            BaseCostume costume = m.FindItemOnLayer(Layer.FirstValid) as BaseCostume;

            if (costume != null)
            {
                m.AddToBackpack(costume);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);
            writer.Write((int)m_Body);
            writer.Write((int)m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    m_Body = reader.ReadInt();
                    m_Hue = reader.ReadInt();
                    break;
                case 2:
                    m_Body = reader.ReadInt();
                    m_Hue = reader.ReadInt();
                    reader.ReadInt();
                    break;
                case 1:
                    m_Body = reader.ReadInt();
                    m_Hue = reader.ReadInt();
                    reader.ReadInt();
                    reader.ReadBool();

                    m_SaveHueMod = reader.ReadInt();
                    reader.ReadInt();
                    break;
            }

            if (RootParent is Mobile && ((Mobile)RootParent).Items.Contains(this))
            {
                EnMask((Mobile)RootParent);
            }
        }
    }
}