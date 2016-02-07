using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class ItemIdentification
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile from)
        {
            from.SendLocalizedMessage(500343); // What do you wish to appraise and identify?
            from.Target = new InternalTarget();

            return TimeSpan.FromSeconds(1.0);
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
                this.AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Item)
                {
                    if (from.CheckTargetSkill(SkillName.ItemID, o, 0, 100))
                    {
                        if (o is BaseWeapon)
                            ((BaseWeapon)o).Identified = true;
                        else if (o is BaseArmor)
                            ((BaseArmor)o).Identified = true;

                        if (!Core.AOS)
                            ((Item)o).OnSingleClick(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else if (o is Mobile)
                {
                    ((Mobile)o).OnSingleClick(from);
                }
                else
                {
                    from.SendLocalizedMessage(500353); // You are not certain...
                }
                Server.Engines.XmlSpawner2.XmlAttach.RevealAttachments(from, o);
            }
        }
    }
}