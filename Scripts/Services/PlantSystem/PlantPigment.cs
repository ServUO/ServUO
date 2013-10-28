using System;
using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class PlantPigment : Item
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
            this.Weight = 1.0;
            this.PigmentHue = hue;
        }

        [Constructable]
        public PlantPigment(PlantHue hue)
            : base(0x0F02)
        {
            this.Weight = 1.0;
            this.PigmentHue = PlantPigmentHueInfo.HueFromPlantHue(hue);
        }

        public PlantPigment(Serial serial)
            : base(serial)
        {
        }

        public bool RetainsColorFrom
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PlantPigmentHue PigmentHue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
                //set any invalid pigment hue to Plain
                if (this.m_Hue != PlantPigmentHueInfo.GetInfo(this.m_Hue).PlantPigmentHue)
                    this.m_Hue = PlantPigmentHue.Plain;
                this.Hue = PlantPigmentHueInfo.GetInfo(this.m_Hue).Hue;
                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1112132;
            }
        }// plant pigment
        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantPigmentHueInfo hueInfo = PlantPigmentHueInfo.GetInfo(this.m_Hue);

            if (this.Amount > 1)
                list.Add(1113273, "{0}\t{1}", this.Amount, "#" + hueInfo.Name);  // ~1_COLOR~ dry reeds
            else 
                list.Add(1112133, "#" + hueInfo.Name);  // ~1_COLOR~ plant pigment
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    this.m_Hue = (PlantPigmentHue)reader.ReadInt();
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!PlantPigmentHueInfo.IsMixable(this.PigmentHue))
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
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
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
                          PlantPigmentHue.White == this.m_Item.PigmentHue || PlantPigmentHue.Black == this.m_Item.PigmentHue) &&
                         from.Skills[SkillName.Alchemy].Base < 100.0 &&
                         from.Skills[SkillName.Cooking].Base < 100.0)
                {
                    from.SendLocalizedMessage(1112213); // You lack the alchemy or cooking skills to mix so unstable a pigment.
                }
                else if (this.m_Item.PigmentHue == pigment.PigmentHue)
                {
                    from.SendLocalizedMessage(1112242); // You decide not to waste pigments by mixing two identical colors.
                }
                else if ((this.m_Item.PigmentHue & ~(PlantPigmentHue.Bright | PlantPigmentHue.Dark | PlantPigmentHue.Ice)) ==
                         (pigment.PigmentHue & ~(PlantPigmentHue.Bright | PlantPigmentHue.Dark | PlantPigmentHue.Ice)))
                {
                    from.SendLocalizedMessage(1112243); // You decide not to waste pigments by mixing variations of the same hue.
                }
                else if ((PlantPigmentHue.White == this.m_Item.PigmentHue && PlantPigmentHueInfo.IsBright(pigment.PigmentHue)) ||
                         (PlantPigmentHue.White == pigment.PigmentHue && PlantPigmentHueInfo.IsBright(this.m_Item.PigmentHue)))
                {
                    from.SendLocalizedMessage(1112241); // This pigment is too diluted to be faded further.
                }
                else if (!PlantPigmentHueInfo.IsMixable(pigment.PigmentHue))
                    from.SendLocalizedMessage(1112125); // This pigment is saturated and cannot be mixed further.
                else
                {
                    PlantPigmentHue newHue = PlantPigmentHueInfo.Mix(this.m_Item.PigmentHue, pigment.PigmentHue);
                    if (PlantPigmentHue.None == newHue)
                        from.SendLocalizedMessage(1112125); // This pigment is saturated and cannot be mixed further.
                    else
                    {
                        pigment.PigmentHue = newHue;
                        Bottle bottle = new Bottle();
                        bottle.MoveToWorld(this.m_Item.Location, this.m_Item.Map);
                        this.m_Item.Delete();
                        from.PlaySound(0x240);
                    }
                }
            }
        }
    }
}