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
            //Name = "Generic Costume";
            Resource = CraftResource.None;
            Attributes.SpellChanneling = 1;
            Layer = Layer.FirstValid;
            Weight = 3.0;
        }

        public BaseCostume(Serial serial)
            : base(serial)
        {

        }

        private void EnMask(Mobile from)
        {
            from.SendMessage("You put on your spooky costume!");

            m_SaveHueMod = from.HueMod;
            from.BodyMod = m_Body;
            from.HueMod = m_Hue;
            Transformed = true;
        }

        private void DeMask(Mobile from)
        {
            from.SendMessage("You decide to quit being so spooky.");

            from.BodyMod = 0;
            from.HueMod = m_SaveHueMod;
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

        public override void OnDoubleClick(Mobile from)
        {

            if (Parent != from)
            {
                from.SendMessage("The costume must be equiped to be used.");
            }

            else if (from.Mounted == true)
            {
                from.SendMessage("You cannot be mounted while wearing your costume!");
            }

            else if (from.BodyMod != 0 && !Transformed)
            {
                from.SendMessage("You are already costumed!");
            }

            else if (Transformed == false)
            {
                EnMask(from);
            }
            else
            {
                DeMask(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2);
            writer.Write((int)m_Body);
            writer.Write((int)m_Hue);
            writer.Write((int)m_SaveHueMod);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    m_Body = reader.ReadInt();
                    m_Hue = reader.ReadInt();
                    m_SaveHueMod = reader.ReadInt();
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
        }
    }
}