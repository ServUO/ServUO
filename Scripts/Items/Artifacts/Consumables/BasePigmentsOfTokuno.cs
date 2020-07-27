using System;

namespace Server.Items
{
    public abstract class BasePigmentsOfTokuno : Item, IUsesRemaining
    {
        public override int LabelNumber => 1070933;// Pigments of Tokuno

        private int m_UsesRemaining;
        private TextDefinition m_Label;

        protected TextDefinition Label
        {
            get
            {
                return m_Label;
            }
            set
            {
                m_Label = value;
                InvalidateProperties();
            }
        }

        #region Old Item Serialization Vars
        /* DO NOT USE! Only used in serialization of pigments that originally derived from Item */
        private bool m_InheritsItem;

        protected bool InheritsItem => m_InheritsItem;
        #endregion

        public BasePigmentsOfTokuno()
            : base(0xEFF)
        {
            Weight = 1.0;
            m_UsesRemaining = 1;
        }

        public BasePigmentsOfTokuno(int uses)
            : base(0xEFF)
        {
            Weight = 1.0;
            m_UsesRemaining = uses;
        }

        public BasePigmentsOfTokuno(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Label != null && m_Label > 0)
                TextDefinition.AddTo(list, m_Label);
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsAccessibleTo(from) && from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(1070929); // Select the artifact or enhanced magic item to dye.
                from.BeginTarget(3, false, Targeting.TargetFlags.None, new TargetStateCallback(InternalCallback), this);
            }
            else
                from.SendLocalizedMessage(502436); // That is not accessible.
        }

        private void InternalCallback(Mobile from, object targeted, object state)
        {
            BasePigmentsOfTokuno pigment = (BasePigmentsOfTokuno)state;

            if (pigment.Deleted || pigment.UsesRemaining <= 0 || !from.InRange(pigment.GetWorldLocation(), 3) || !pigment.IsAccessibleTo(from))
                return;

            Item i = targeted as Item;

            if (i == null)
                from.SendLocalizedMessage(1070931); // You can only dye artifacts and enhanced magic items with this tub.
            else if (!from.InRange(i.GetWorldLocation(), 3) || !IsAccessibleTo(from))
                from.SendLocalizedMessage(502436); // That is not accessible.
            else if (from.Items.Contains(i))
                from.SendLocalizedMessage(1070930); // Can't dye artifacts or enhanced magic items that are being worn.
            else if (i.IsLockedDown)
                from.SendLocalizedMessage(1070932); // You may not dye artifacts and enhanced magic items which are locked down.
            else if (i is MetalPigmentsOfTokuno || i is LesserPigmentsOfTokuno || i is PigmentsOfTokuno || i is CompassionPigment)
                from.SendLocalizedMessage(1042417); // You cannot dye that.
            else if (!IsValidItem(i))
                from.SendLocalizedMessage(1070931); // You can only dye artifacts and enhanced magic items with this tub.	//Yes, it says tub on OSI.  Don't ask me why ;p
            else
            {
                //Notes: on OSI there IS no hue check to see if it's already hued.  and no messages on successful hue either
                i.Hue = Hue;

                if (--pigment.UsesRemaining <= 0)
                    pigment.Delete();

                from.PlaySound(0x23E); // As per OSI TC1
            }
        }

        public static bool IsValidItem(Item i)
        {
            if (i is BasePigmentsOfTokuno)
                return false;

            Type t = i.GetType();

            CraftResource resource = CraftResource.None;

            if (i is BaseWeapon)
                resource = ((BaseWeapon)i).Resource;
            else if (i is BaseArmor)
                resource = ((BaseArmor)i).Resource;
            else if (i is BaseClothing)
                resource = ((BaseClothing)i).Resource;

            if (!CraftResources.IsStandard(resource))
                return true;

            if (i is MongbatDartboard || i is FelineBlessedStatue)
                return true;

            if (i.IsArtifact)
                return true;

            if (i is BaseAddonDeed && ((BaseAddonDeed)i).UseCraftResource && !((BaseAddonDeed)i).IsReDeed && ((BaseAddonDeed)i).Resource != CraftResource.None)
                return true;

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.WriteEncodedInt(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_UsesRemaining = reader.ReadEncodedInt();
                        break;
                    }
                case 0: // Old pigments that inherited from item
                    {
                        m_InheritsItem = true;

                        if (this is LesserPigmentsOfTokuno)
                            ((LesserPigmentsOfTokuno)this).Type = (LesserPigmentType)reader.ReadEncodedInt();
                        else if (this is PigmentsOfTokuno)
                            ((PigmentsOfTokuno)this).Type = (PigmentType)reader.ReadEncodedInt();
                        else if (this is MetalPigmentsOfTokuno)
                            reader.ReadEncodedInt();

                        m_UsesRemaining = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        #region IUsesRemaining Members

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }

        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        #endregion
    }
}
