using System;
using Server.Engines.Harvest;
using Server.Targeting;

namespace Server.Items
{
    public class ProspectorsTool : BaseBashing
    {
        public override int LabelNumber { get { return 1049065; } } // prospector's tool

        [Constructable]
        public ProspectorsTool()
            : base(0xFB4)
        {
            Weight = 10.0;
            UsesRemaining = 50;
            ShowUsesRemaining = true;
        }

        public ProspectorsTool(Serial serial)
            : base(serial)
        {
        }        

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.CrushingBlow; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ShadowStrike; } }
        public override int AosStrengthReq { get { return 40; } }
        public override int AosMinDamage { get { return 13; } }
        public override int AosMaxDamage { get { return 15; } }
        public override int AosSpeed { get { return 33; } }
        public override float MlSpeed { get { return 3.25f; } }
        public override int OldStrengthReq { get { return 10; } }
        public override int OldMinDamage { get { return 6; } }
        public override int OldMaxDamage { get { return 8; } }
        public override int OldSpeed { get { return 33; } }
        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 60; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || Parent == from)
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public void Prospect(Mobile from, object toProspect)
        {
            if (!IsChildOf(from.Backpack) && Parent != from)
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            HarvestSystem system = Mining.System;

            int tileID;
            Map map;
            Point3D loc;

            if (!system.GetHarvestDetails(from, this, toProspect, out tileID, out map, out loc))
            {
                from.SendLocalizedMessage(1049048); // You cannot use your prospector tool on that.
                return;
            }

            HarvestDefinition def = system.GetDefinition(tileID);

            if (def == null || def.Veins.Length <= 1)
            {
                from.SendLocalizedMessage(1049048); // You cannot use your prospector tool on that.
                return;
            }

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null)
            {
                from.SendLocalizedMessage(1049048); // You cannot use your prospector tool on that.
                return;
            }

            HarvestVein vein = bank.Vein, defaultVein = bank.DefaultVein;

            if (vein == null || defaultVein == null)
            {
                from.SendLocalizedMessage(1049048); // You cannot use your prospector tool on that.
                return;
            }
            else if (vein != defaultVein)
            {
                from.SendLocalizedMessage(1049049); // That ore looks to be prospected already.
                return;
            }

            int veinIndex = Array.IndexOf(def.Veins, vein);

            if (veinIndex < 0)
            {
                from.SendLocalizedMessage(1049048); // You cannot use your prospector tool on that.
            }
            else if (veinIndex >= (def.Veins.Length - 1))
            {
                from.SendLocalizedMessage(1049061); // You cannot improve valorite ore through prospecting.
            }
            else
            {
                bank.Vein = def.Veins[veinIndex + 1];
                from.SendLocalizedMessage(1049050 + veinIndex);

                --UsesRemaining;

                if (UsesRemaining <= 0)
                {
                    from.SendLocalizedMessage(1049062); // You have used up your prospector's tool.
                    Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    break;
                case 1:
                    {
                        UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        private class InternalTarget : Target
        {
            private readonly ProspectorsTool m_Tool;
            public InternalTarget(ProspectorsTool tool)
                : base(2, true, TargetFlags.None)
            {
                m_Tool = tool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                m_Tool.Prospect(from, targeted);
            }
        }
    }
}