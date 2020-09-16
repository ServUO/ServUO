using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System;

namespace Server.Engines.Doom
{
    public class LampRoomBox : Item
    {
        private LeverPuzzleController m_Controller;
        private Mobile m_Wanderer;
        public LampRoomBox(LeverPuzzleController controller)
            : base(0xe80)
        {
            m_Controller = controller;
            ItemID = 0xe80;
            Movable = false;
        }

        public LampRoomBox(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), 3))
                return;
            if (m_Controller.Enabled)
                return;

            if ((m_Wanderer == null || !m_Wanderer.Alive))
            {
                m_Wanderer = new WandererOfTheVoid();
                m_Wanderer.MoveToWorld(LeverPuzzleController.lr_Enter, Map.Malas);
                m_Wanderer.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1060002, ""); // I am the guardian of...
                Timer.DelayCall(TimeSpan.FromSeconds(5.0), CallBackMessage);
            }
        }

        public void CallBackMessage()
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, 1060003, ""); // You try to pry the box open...
        }

        public override void OnAfterDelete()
        {
            if (m_Controller != null && !m_Controller.Deleted)
                m_Controller.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Controller = reader.ReadItem() as LeverPuzzleController;
        }
    }

    public class LeverPuzzleStatue : Item
    {
        private LeverPuzzleController m_Controller;
        public LeverPuzzleStatue(int[] dat, LeverPuzzleController controller)
            : base(dat[0])
        {
            m_Controller = controller;
            Hue = 0x44E;
            Movable = false;
        }

        public LeverPuzzleStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Controller != null && !m_Controller.Deleted)
                m_Controller.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Controller = reader.ReadItem() as LeverPuzzleController;
        }
    }

    public class LeverPuzzleLever : Item
    {
        private ushort m_Code;
        private LeverPuzzleController m_Controller;
        public LeverPuzzleLever(ushort code, LeverPuzzleController controller)
            : base(0x108E)
        {
            m_Controller = controller;
            m_Code = code;
            Hue = 0x66D;
            Movable = false;
        }

        public LeverPuzzleLever(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ushort Code => m_Code;
        public override void OnDoubleClick(Mobile m)
        {
            if (m != null && m_Controller.Enabled)
            {
                ItemID ^= 2;
                Effects.PlaySound(Location, Map, 0x3E8);
                m_Controller.LeverPulled(m_Code);
            }
            else
            {
                m.SendLocalizedMessage(1060001); // You throw the switch, but the mechanism cannot be engaged again so soon.
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Controller != null && !m_Controller.Deleted)
                m_Controller.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(m_Code);
            writer.Write(m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Code = reader.ReadUShort();
            m_Controller = reader.ReadItem() as LeverPuzzleController;
        }
    }

    [TypeAlias("Server.Engines.Doom.LampRoomTelePorter")]
    public class LampRoomTeleporter : Item
    {
        public LampRoomTeleporter(int[] dat)
        {
            Hue = dat[1];
            ItemID = dat[0];
            Movable = false;
        }

        public LampRoomTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement => true;
        public override bool OnMoveOver(Mobile m)
        {
            if (m != null && m is PlayerMobile)
            {
                if (SpellHelper.CheckCombat(m))
                {
                    m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??				
                }
                else
                {
                    BaseCreature.TeleportPets(m, LeverPuzzleController.lr_Exit, Map.Malas);
                    m.MoveToWorld(LeverPuzzleController.lr_Exit, Map.Malas);
                    return false;
                }
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
