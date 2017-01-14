using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.Engines.Doom
{
    public class LampRoomBox : Item
    {
        private LeverPuzzleController m_Controller;
        private Mobile m_Wanderer;
        public LampRoomBox(LeverPuzzleController controller)
            : base(0xe80)
        {
            this.m_Controller = controller;
            this.ItemID = 0xe80;
            this.Movable = false;
        }

        public LampRoomBox(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(this.GetWorldLocation(), 3))
                return;
            if (this.m_Controller.Enabled)
                return;

            if ((this.m_Wanderer == null || !this.m_Wanderer.Alive))
            {
                this.m_Wanderer = new WandererOfTheVoid();
                this.m_Wanderer.MoveToWorld(LeverPuzzleController.lr_Enter, Map.Malas);
                this.m_Wanderer.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1060002, ""); // I am the guardian of...
                Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(CallBackMessage));
            }
        }

        public void CallBackMessage()
        {
            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1060003, ""); // You try to pry the box open...
        }

        public override void OnAfterDelete()
        {
            if (this.m_Controller != null && !this.m_Controller.Deleted)
                this.m_Controller.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(this.m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            this.m_Controller = reader.ReadItem() as LeverPuzzleController;
        }
    }

    public class LeverPuzzleStatue : Item
    {
        private LeverPuzzleController m_Controller;
        public LeverPuzzleStatue(int[] dat, LeverPuzzleController controller)
            : base(dat[0])
        {
            this.m_Controller = controller;
            this.Hue = 0x44E;
            this.Movable = false;
        }

        public LeverPuzzleStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (this.m_Controller != null && !this.m_Controller.Deleted)
                this.m_Controller.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(this.m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            this.m_Controller = reader.ReadItem() as LeverPuzzleController;
        }
    }

    public class LeverPuzzleLever : Item
    {
        private UInt16 m_Code;
        private LeverPuzzleController m_Controller;
        public LeverPuzzleLever(UInt16 code, LeverPuzzleController controller)
            : base(0x108E)
        {
            this.m_Controller = controller;
            this.m_Code = code;
            this.Hue = 0x66D;
            this.Movable = false;
        }

        public LeverPuzzleLever(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public UInt16 Code
        {
            get
            {
                return this.m_Code;
            }
        }
        public override void OnDoubleClick(Mobile m)
        {
            if (m != null && this.m_Controller.Enabled)
            {
                this.ItemID ^= 2;
                Effects.PlaySound(this.Location, this.Map, 0x3E8);
                this.m_Controller.LeverPulled(this.m_Code);
            }
            else
            {
                m.SendLocalizedMessage(1060001); // You throw the switch, but the mechanism cannot be engaged again so soon.
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_Controller != null && !this.m_Controller.Deleted)
                this.m_Controller.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write((ushort)this.m_Code);
            writer.Write(this.m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            this.m_Code = reader.ReadUShort();
            this.m_Controller = reader.ReadItem() as LeverPuzzleController;
        }
    }

    [TypeAlias("Server.Engines.Doom.LampRoomTelePorter")]
    public class LampRoomTeleporter : Item
    {
        public LampRoomTeleporter(int[] dat)
        {
            this.Hue = dat[1];
            this.ItemID = dat[0];
            this.Movable = false;
        }

        public LampRoomTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
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
                    Server.Mobiles.BaseCreature.TeleportPets(m, LeverPuzzleController.lr_Exit, Map.Malas);
                    m.MoveToWorld(LeverPuzzleController.lr_Exit, Map.Malas);
                    return false;
                }
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