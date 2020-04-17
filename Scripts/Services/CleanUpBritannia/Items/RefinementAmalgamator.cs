using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class RefinementAmalgamator : Item
    {
        public List<RefinementComponent> ToCombine { get; set; }
        public RefinementComponent ToUpgrade { get; set; }

        [Constructable]
        public RefinementAmalgamator()
            : base(0x9966)
        {
            Hue = 1152;
            Weight = 1;
        }

        public RefinementAmalgamator(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154340;// Refinement Amalgamator

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                // The item must be in your backpack to use it.
                from.SendLocalizedMessage(1060640);
            }
            else
            {
                ToCombine = null;
                ToUpgrade = null;

                from.SendLocalizedMessage(1154351); // Target the refinement you wish to combine.
                from.Target = new InternalTarget(from, this);
            }
        }

        public void CheckCombine(Mobile m, RefinementComponent component)
        {
            if (ToUpgrade == null)
            {
                if (component.ModType != ModType.Invulnerability)
                {
                    ToCombine = new List<RefinementComponent>();
                    ToUpgrade = component;

                    m.SendLocalizedMessage(1154351); // Target the refinement you wish to combine.
                    m.Target = new InternalTarget(m, this);
                }
                else
                {
                    m.SendLocalizedMessage(1154353); // You can't upgrade this refinement.
                }
            }
            else
            {
                if (ToUpgrade.RefinementType != component.RefinementType
                            || ToUpgrade.CraftType != component.CraftType
                            || ToUpgrade.SubCraftType != component.SubCraftType
                            || ToUpgrade.ModType != component.ModType)
                {
                    m.SendLocalizedMessage(1154354); // This refinement does not match the type currently being combined.
                }
                else
                {
                    ToCombine.Add(component);
                    ValidateList(m);

                    if (ToCombine.Count >= GetCombineTotal(component.ModType) - 1) // -1 because we're counting ToUpgrade
                    {
                        foreach (RefinementComponent comp in ToCombine)
                            comp.Delete();

                        ToUpgrade.ModType++;

                        m.SendLocalizedMessage(1154352); // You've completed the amalgamation and received an upgraded version of your refinement.
                    }
                    else
                    {
                        m.SendLocalizedMessage(1154360); // You place the refinement into the amalgamator.

                        m.SendLocalizedMessage(1154351); // Target the refinement you wish to combine.
                        m.Target = new InternalTarget(m, this);
                    }
                }
            }
        }

        private void ValidateList(Mobile m)
        {
            if (ToCombine == null)
                return;

            List<RefinementComponent> copy = new List<RefinementComponent>(ToCombine);

            foreach (RefinementComponent comp in copy)
            {
                if (comp == null || comp.Deleted || !comp.IsChildOf(m.Backpack))
                {
                    ToCombine.Remove(comp);
                }
            }

            ColUtility.Free(copy);
        }

        private int GetCombineTotal(ModType type)
        {
            switch (type)
            {
                default:
                case ModType.Defense: return 2;
                case ModType.Protection: return 3;
                case ModType.Hardening: return 4;
                case ModType.Fortification: return 5;
                case ModType.Invulnerability: return -1;
            }
        }

        private class InternalTarget : Target
        {
            private readonly Mobile m_Mobile;
            private readonly RefinementAmalgamator m_Amalgamator;

            public InternalTarget(Mobile m, RefinementAmalgamator amalgamator)
                : base(-1, true, TargetFlags.None)
            {
                m_Mobile = m;
                m_Amalgamator = amalgamator;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                RefinementComponent item = targeted as RefinementComponent;

                if (item == null)
                {
                    from.SendLocalizedMessage(1154457); // This is not a refinement.
                }
                else if (!item.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                }
                else
                {
                    if (m_Amalgamator != null && !m_Amalgamator.Deleted)
                    {
                        m_Amalgamator.CheckCombine(from, item);
                    }
                    /*RefinementComponent comp = (RefinementComponent)targeted;

                    if (ToCombine == null)
                    {
                        if (comp.ModType == ModType.Invulnerability)
                        {
                            from.SendLocalizedMessage(1154353); // You can't upgrade this refinement.
                        }
                        else
                        {
                            from.SendLocalizedMessage(1154351); // Target the refinement you wish to combine.
                            from.Target = new InternalTarget(from, comp);
                        }
                    }
                    else
                    {
                        if (m_First.RefinementType != comp.RefinementType
                            || m_First.CraftType != comp.CraftType
                            || m_First.SubCraftType != comp.SubCraftType
                            || m_First.ModType != comp.ModType)
                        {
                            from.SendLocalizedMessage(1154354); // This refinement does not match the type currently being combined.
                        }
                        else
                        {
                            comp.Delete();
                            m_First.ModType++;

                            from.SendLocalizedMessage(1154352); // You've completed the amalgamation and received an upgraded version of your refinement.
                        }
                    }*/
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}