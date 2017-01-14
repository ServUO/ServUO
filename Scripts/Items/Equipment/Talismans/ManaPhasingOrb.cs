using Server;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ManaPhasingOrb : BaseTalisman, Server.Engines.Craft.IRepairable
    {
        public override int LabelNumber { get { return 1116230; } }

        public Server.Engines.Craft.CraftSystem RepairSystem { get { return Server.Engines.Craft.DefTinkering.CraftSystem; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public ManaPhasingOrb() : base(4246)
        {
            MaxChargeTime = 30;
            Charges = 50;
            MaxCharges = Charges;
            Hue = 1165;

            Attributes.Brittle = 1;
            Attributes.LowerManaCost = 6;

            switch (Utility.Random(3))
            {
                case 0: Attributes.RegenHits = 1; break;
                case 1: Attributes.RegenMana = 1; break;
                case 2: Attributes.RegenStam = 1; break;
            }

            if (Utility.RandomBool())
                Attributes.DefendChance = 5;
            else
                Attributes.AttackChance = 5;

            switch (Utility.Random(3))
            {
                case 0: Attributes.LowerRegCost = 10; break;
                case 1: Attributes.WeaponDamage = 15; break;
                case 2: Attributes.SpellDamage = 5; break;
            }
        }

        public static bool IsInManaPhase(Mobile from)
        {
            return _ManaPhaseTable != null && _ManaPhaseTable.Contains(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile && IsInManaPhase((Mobile)parent))
                RemoveFromTable((Mobile)parent);

            base.OnRemoved(parent);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Talisman != this)
                from.SendLocalizedMessage(502641); // You must equip this item to use it.
            else
            {
                if (ChargeTime > 0)
                {
                    from.SendLocalizedMessage(1116163); //You must wait a few seconds before attempting to phase mana again.
                    return;
                }

                if (Charges > 0 && !IsInManaPhase(from))
                {
                    AddToTable(from);
                    OnAfterUse(from);

                    BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.ManaPhase, 1116158, 1153816));

                    return;
                }
            }

            base.OnDoubleClick(from);
        }

        private static void AddToTable(Mobile from)
        {
            if (_ManaPhaseTable == null)
                _ManaPhaseTable = new List<Mobile>();

            _ManaPhaseTable.Add(from);

            from.SendLocalizedMessage(1116164); //Your next use of magical energy will draw its power from the void.
        }

        public static void RemoveFromTable(Mobile from)
        {
            if (_ManaPhaseTable != null && _ManaPhaseTable.Contains(from))
                _ManaPhaseTable.Remove(from);

            BuffInfo.RemoveBuff(from, BuffIcon.ManaPhase);

            from.SendLocalizedMessage(1116165); //You will no longer attempt to draw magical energy from the void.
        }

        public static List<Mobile> _ManaPhaseTable { get; set; }

        public ManaPhasingOrb(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}