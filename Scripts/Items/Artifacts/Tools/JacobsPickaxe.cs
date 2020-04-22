using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class JacobsPickaxe : Pickaxe
    {
        private static readonly List<JacobsPickaxe> _Instances = new List<JacobsPickaxe>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), Tick_Callback);
        }

        private static void Tick_Callback()
        {
            foreach (JacobsPickaxe pickaxe in _Instances.Where(p => p != null && !p.Deleted))
            {
                int charge = pickaxe.UsesRemaining + 10 > 20 ? 20 - pickaxe.UsesRemaining : 10;

                if (charge > 0)
                    pickaxe.UsesRemaining += charge;

                pickaxe.InvalidateProperties();
            }
        }

        public override int LabelNumber => 1077758;  // Jacob's Pickaxe

        [Constructable]
        public JacobsPickaxe()
            : base()
        {
            LootType = LootType.Blessed;
            SkillBonuses.SetValues(0, SkillName.Mining, 10.0);
            UsesRemaining = 20;

            _Instances.Add(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (HarvestSystem == null)
                return;

            if (IsChildOf(from.Backpack) || Parent == from)
            {
                if (UsesRemaining > 0)
                    HarvestSystem.BeginHarvesting(from, this);
                else
                    from.SendLocalizedMessage(1072306); // You must wait a moment for it to recharge.
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void Delete()
        {
            base.Delete();

            _Instances.Remove(this);
        }

        public JacobsPickaxe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
            _Instances.Add(this);
        }
    }
}
