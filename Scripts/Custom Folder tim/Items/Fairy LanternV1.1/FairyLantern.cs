using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items {
    public class FairyLantern : Item {

        #region Constructors
        public FairyLantern (Serial serial)
            : base(serial) {
        }
        [Constructable]
        public FairyLantern ()
            : base(0xa25) {
            Name = "a Fairy Lantern";
            Weight = 2.0;
            Layer = Layer.OneHanded;
			LootType = LootType.Blessed;
        }
        #endregion

        public override void OnDoubleClick (Mobile from) {
            if (!Movable)
                return;
            else if (from.InRange(this.GetWorldLocation(), 2) == false) {
                from.SendLocalizedMessage(500486);	//That is too far away.
                return;
            }

            Container pack = from.Backpack;

            if (!(Parent == from || (pack != null && Parent == pack))) //If not in pack.
			{
                from.SendLocalizedMessage(1042001);	//That must be in your pack to use it.
                return;
            }
            from.Target = new FairyLanternTarget(this);
            from.SendMessage("Target the pet you wish to shrink");
        }


        private class FairyLanternTarget : Target {
            private FairyLantern m_Potion;

            public FairyLanternTarget (Item i)
                : base(3, false, TargetFlags.None) {
                m_Potion = (FairyLantern)i;
            }

            protected override void OnTarget (Mobile from, object targ) {
                if (!(m_Potion.Deleted)) {
                    if (FairyShrinkFunctions.FairyShrink(from, targ)) {
                        m_Potion.Delete();
                    }
                }

                return;
            }
        }


        #region Serialization
        public override void Serialize (GenericWriter writer) {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize (GenericReader reader) {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        #endregion
    }
}