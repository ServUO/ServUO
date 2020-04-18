using Server.Mobiles;
using Server.Spells;
using System;

namespace Server.Items
{
    public class TribalPaint : Item
    {
        [Constructable]
        public TribalPaint()
            : base(0x9EC)
        {
            Hue = 2101;
            Weight = 2.0;
            Stackable = true;
        }

        public TribalPaint(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1040000;// savage kin paint
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (!from.CanBeginAction(typeof(Spells.Fifth.IncognitoSpell)))
                {
                    from.SendLocalizedMessage(501698); // You cannot disguise yourself while incognitoed.
                }
                else if (!from.CanBeginAction(typeof(Spells.Seventh.PolymorphSpell)))
                {
                    from.SendLocalizedMessage(501699); // You cannot disguise yourself while polymorphed.
                }
                else if (TransformationSpellHelper.UnderTransformation(from))
                {
                    from.SendLocalizedMessage(501699); // You cannot disguise yourself while polymorphed.
                }
                else if (Spells.Ninjitsu.AnimalForm.UnderTransformation(from))
                {
                    from.SendLocalizedMessage(1061634); // You cannot disguise yourself while in that form.
                }
                else if (from.IsBodyMod || from.FindItemOnLayer(Layer.Helm) is OrcishKinMask)
                {
                    from.SendLocalizedMessage(501605); // You are already disguised.
                }
                else
                {
                    from.BodyMod = (from.Female ? 184 : 183);
                    from.HueMod = 0;

                    if (from is PlayerMobile)
                        ((PlayerMobile)from).SavagePaintExpiration = TimeSpan.FromDays(7.0);

                    from.SendLocalizedMessage(1042537); // You now bear the markings of the savage tribe.  Your body paint will last about a week or you can remove it with an oil cloth.

                    Consume();
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
