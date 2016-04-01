using System;

//using System.Collections.Generic;
namespace Server.Items.Holiday
{
    [TypeAlias("Server.Items.ClownMask", "Server.Items.DaemonMask", "Server.Items.PlagueMask")]
    public class BasePaintedMask : Item
    {
        private static readonly string[] m_Staffers =
        {
            "Ryan",
            "Mark",
            "Krrios",
            "Zippy",
            "Athena",
            "Eos",
            "Xavier"
        };
        private string m_Staffer;
        public BasePaintedMask(int itemid)
            : this(m_Staffers[Utility.Random(m_Staffers.Length)], itemid)
        {
        }

        public BasePaintedMask(string staffer, int itemid)
            : base(itemid + Utility.Random(2))
        {
            this.m_Staffer = staffer;

            Utility.Intern(this.m_Staffer);
        }

        public BasePaintedMask(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                if (this.m_Staffer != null)
                {
                    return String.Format("{0} hand painted by {1}", this.MaskName, this.m_Staffer);
                }

                return this.MaskName;
            }
        }
        public virtual string MaskName
        {
            get
            {
                return "A Mask";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write((string)this.m_Staffer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 1)
            {
                this.m_Staffer = Utility.Intern(reader.ReadString());
            }
        }
    }
}