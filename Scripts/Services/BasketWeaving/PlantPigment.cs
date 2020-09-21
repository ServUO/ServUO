using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class PlantPigment : Item, IPigmentHue
    {
        private PlantPigmentHue m_Hue;
        [Constructable]
        public PlantPigment()
            : this(PlantPigmentHue.None)
        {
        }

        [Constructable]
        public PlantPigment(PlantPigmentHue hue)
            : base(0x0F02)
        {
            Weight = 1.0;
            PigmentHue = hue;
        }

        [Constructable]
        public PlantPigment(PlantHue hue)
            : base(0x0F02)
        {
            Weight = 1.0;
            PigmentHue = PlantPigmentHueInfo.HueFromPlantHue(hue);
        }

        public PlantPigment(Serial serial)
            : base(serial)
        {
        }

        public bool RetainsColorFrom => true;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlantPigmentHue PigmentHue
        {
            get
            {
                return m_Hue;
            }
            set
            {
                m_Hue = value;
                //set any invalid pigment hue to Plain
                if (m_Hue != PlantPigmentHueInfo.GetInfo(m_Hue).PlantPigmentHue)
                    m_Hue = PlantPigmentHue.Plain;
                Hue = PlantPigmentHueInfo.GetInfo(m_Hue).Hue;
                InvalidateProperties();
            }
        }
        public override int LabelNumber => 1112132;// plant pigment
        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantPigmentHueInfo info = PlantPigmentHueInfo.GetInfo(m_Hue);
            int cliloc;

            if (Amount > 1)
            {
                cliloc = info.IsBright() ? 1113271 : 1113270;
                list.Add(cliloc, string.Format("{0}\t#{1}", Amount.ToString(), info.Name));
            }
            else
            {
                cliloc = info.IsBright() ? 1112134 : 1112133;
                list.Add(cliloc, string.Format("#{0}", info.Name));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write((int)m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Hue = (PlantPigmentHue)reader.ReadInt();
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!PlantPigmentHueInfo.IsMixable(PigmentHue))
                from.SendLocalizedMessage(1112125); // This pigment is saturated and cannot be mixed further.
            else
            {
                from.SendLocalizedMessage(1112123); // Which plant pigment do you wish to mix this with?

                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly PlantPigment m_Item;
            public InternalTarget(PlantPigment item)
                : base(2, false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Item.Deleted)
                    return;

                PlantPigment pigment = targeted as PlantPigment;
                if (null == pigment)
                {
                    from.SendLocalizedMessage(1112124); // You may only mix this with another non-saturated plant pigment.
                    return;
                }

                if (from.Skills[SkillName.Alchemy].Base < 75.0 && from.Skills[SkillName.Cooking].Base < 75.0)
                {
                    from.SendLocalizedMessage(1112214); // You lack the alchemy or cooking skills to mix plant pigments.
                }
                else if ((PlantPigmentHue.White == pigment.PigmentHue || PlantPigmentHue.Black == pigment.PigmentHue ||
                          PlantPigmentHue.White == m_Item.PigmentHue || PlantPigmentHue.Black == m_Item.PigmentHue) &&
                         from.Skills[SkillName.Alchemy].Base < 100.0 &&
                         from.Skills[SkillName.Cooking].Base < 100.0)
                {
                    from.SendLocalizedMessage(1112213); // You lack the alchemy or cooking skills to mix so unstable a pigment.
                }
                else if (m_Item.PigmentHue == pigment.PigmentHue)
                {
                    from.SendLocalizedMessage(1112242); // You decide not to waste pigments by mixing two identical colors.
                }
                else if ((m_Item.PigmentHue & ~(PlantPigmentHue.Bright | PlantPigmentHue.Dark | PlantPigmentHue.Ice)) ==
                         (pigment.PigmentHue & ~(PlantPigmentHue.Bright | PlantPigmentHue.Dark | PlantPigmentHue.Ice)))
                {
                    from.SendLocalizedMessage(1112243); // You decide not to waste pigments by mixing variations of the same hue.
                }
                else if ((PlantPigmentHue.White == m_Item.PigmentHue && PlantPigmentHueInfo.IsBright(pigment.PigmentHue)) ||
                         (PlantPigmentHue.White == pigment.PigmentHue && PlantPigmentHueInfo.IsBright(m_Item.PigmentHue)))
                {
                    from.SendLocalizedMessage(1112241); // This pigment is too diluted to be faded further.
                }
                else if (!PlantPigmentHueInfo.IsMixable(pigment.PigmentHue))
                    from.SendLocalizedMessage(1112125); // This pigment is saturated and cannot be mixed further.
                else
                {
                    PlantPigmentHue newHue = PlantPigmentHueInfo.Mix(m_Item.PigmentHue, pigment.PigmentHue);
                    if (PlantPigmentHue.None == newHue)
                        from.SendLocalizedMessage(1112125); // This pigment is saturated and cannot be mixed further.
                    else
                    {
                        pigment.PigmentHue = newHue;
                        Bottle bottle = new Bottle();
                        bottle.MoveToWorld(m_Item.Location, m_Item.Map);
                        m_Item.Delete();
                        from.PlaySound(0x240);
                    }
                }
            }
        }
    }
}
