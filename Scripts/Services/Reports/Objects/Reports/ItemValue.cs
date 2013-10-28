using System;

namespace Server.Engines.Reports
{
    public class ItemValue : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("iv", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new ItemValue();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private string m_Value;
        private string m_Format;

        public string Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }
        public string Format
        {
            get
            {
                return this.m_Format;
            }
            set
            {
                this.m_Format = value;
            }
        }

        private ItemValue()
        {
        }

        public ItemValue(string value)
            : this(value, null)
        {
        }

        public ItemValue(string value, string format)
        {
            this.m_Value = value;
            this.m_Format = format;
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            op.SetString("v", this.m_Value);
            op.SetString("f", this.m_Format);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            this.m_Value = ip.GetString("v");
            this.m_Format = Utility.Intern(ip.GetString("f"));

            if (this.m_Format == null)
                Utility.Intern(ref this.m_Value);
        }
    }
}