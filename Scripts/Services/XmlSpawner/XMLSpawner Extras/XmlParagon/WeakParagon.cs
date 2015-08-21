using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class WeakParagon : XmlParagon
    {
        // string that is displayed on the xmlspawner when this is attached
        public override string OnIdentify(Mobile from)
        {
            return String.Format("Weak {0}", base.OnIdentify(from));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }

        #region constructors
        public WeakParagon(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public WeakParagon()
            : base()
        {
            // reduced buff modifiers
            HitsBuff = 4.0;
            StrBuff = 1.05;
            IntBuff = 1.10;
            DexBuff = 1.10;
            SkillsBuff = 1.20;
            SpeedBuff = 1.20;
            FameBuff = 1.40;
            KarmaBuff = 1.40;
            DamageBuff = 4;
        }
        #endregion
    }
}