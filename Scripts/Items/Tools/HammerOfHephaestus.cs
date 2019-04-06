using System;
using Server.Engines.Craft;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class HammerOfHephaestus : AncientSmithyHammer
    {
        private static List<HammerOfHephaestus> _Instances = new List<HammerOfHephaestus>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), new TimerCallback(Tick_Callback));
        }

        private static void Tick_Callback()
        {
            foreach (var hammer in _Instances.Where(h => h != null && !h.Deleted && h.UsesRemaining < 20))
            {
                hammer.UsesRemaining++;
                hammer.InvalidateProperties();
            }
        }

        [Constructable]
        public HammerOfHephaestus()
            : base(10, 20)
        {
            LootType = LootType.Blessed;
            Hue = 0x0;

            _Instances.Add(this);
        }

        public HammerOfHephaestus(Serial serial)
            : base(serial)
        {
        }

        public override void Delete()
        {
            base.Delete();

            _Instances.Remove(this);
        }

        public override int LabelNumber
        {
            get
            {
                return 1077740;
            }
        }// Hammer of Hephaestus

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || Parent == from)
            {
                if (UsesRemaining > 0)
                {
                    CraftSystem system = CraftSystem;
	
                    int num = system.CanCraft(from, this, null);
	
                    if (num > 0)
                    {
                        from.SendLocalizedMessage(num);
                    }
                    else
                    {
                        CraftContext context = system.GetContext(from);
	
                        from.SendGump(new CraftGump(from, system, this, null));
                    }
                }
                else
                    from.SendLocalizedMessage(1072306); // You must wait a moment for it to recharge.
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override bool CanEquip(Mobile from)
        {
            if (UsesRemaining > 0)
                return base.CanEquip(from);

            from.SendLocalizedMessage(1072306); // You must wait a moment for it to recharge.
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && Hue == 0x482)
                Hue = 0x0;

            _Instances.Add(this);
        }
    }
}