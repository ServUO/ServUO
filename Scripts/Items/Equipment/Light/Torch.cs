using System;

namespace Server.Items
{
    public class Torch : BaseEquipableLight
    {
        [Constructable]
        public Torch()
            : base(0xF6B)
        {
            if (Burnout)
                this.Duration = TimeSpan.FromMinutes(30);
            else
                this.Duration = TimeSpan.Zero;

            this.Burning = false;
            this.Light = LightType.Circle300;
            this.Weight = 1.0;
        }

        public Torch(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID
        {
            get
            {
                return 0xA12;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 0xF6B;
            }
        }
        public override int LitSound
        {
            get
            {
                return 0x54;
            }
        }
        public override int UnlitSound
        {
            get
            {
                return 0x4BB;
            }
        }
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile && this.Burning)
            {
                Mobiles.MeerMage.StopEffect((Mobile)parent, true);
                SwarmContext.CheckRemove((Mobile)parent);
            }
        }

        public override void Ignite()
        {
            base.Ignite();

            if (this.Parent is Mobile && this.Burning)
            {
                Mobiles.MeerMage.StopEffect((Mobile)this.Parent, true);
                SwarmContext.CheckRemove((Mobile)this.Parent);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (this.Weight == 2.0)
                this.Weight = 1.0;
        }
    }
}