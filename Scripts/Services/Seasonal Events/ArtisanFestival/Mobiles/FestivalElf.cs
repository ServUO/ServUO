using Server.Mobiles;
using Server.Items;
using Server.Engines.BulkOrders;

using System;
using System.Collections.Generic;

namespace Server.Engines.ArtisanFestival
{
    public class FestivalElf : BaseVendor
    {
        protected override List<SBInfo> SBInfos => new List<SBInfo>();
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArtisanFestivalEvent Festival { get { return ArtisanFestivalEvent.Instance; } set { } }

        private DateTime _NextTalk;

        public FestivalElf()
            : base("the elf")
        {
        }

        public FestivalElf(Serial serial)
            : base(serial)
        {
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 0x26);
            SetWearable(new LongPants(), 0x48);
            SetWearable(new BodySash(), 0x48);
            SetWearable(new Lantern(), 0x48);
            SetWearable(new Boots());
            SetWearable(new FloppyHat(), 0x26);
            SetWearable(new LeatherGloves(), 0x47E);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (from is PlayerMobile pm && dropped is IBOD bod)
            {
                if (!bod.Complete)
                {
                    SayTo(from, 1045131, 0x3B2); // You have not completed the order yet.
                    return false;
                }

                var festival = ArtisanFestivalEvent.Instance;

                if (festival.Running && !festival.ClaimPeriod)
                {
                    int points = 0;
                    double banked = 0.0;

                    if (bod is SmallBOD)
                        BulkOrderSystem.ComputePoints((SmallBOD)dropped, out points, out banked);
                    else
                        BulkOrderSystem.ComputePoints((LargeBOD)dropped, out points, out banked);

                    festival.OnBodTurnIn(pm, this, banked);
                    Say(1157204, pm.Name, 1150); // Ho! Ho! Thank ye ~1_PLAYER~ for giving me a Bulk Order Deed!

                    dropped.Delete();
                    return false;
                }
            }

            return false;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Player && ArtisanFestivalEvent.Instance.ClaimPeriod && _NextTalk < DateTime.UtcNow && !InRange(oldLocation, 8) && m.InRange(Location, 8))
            {
                Say(1157279, 1150); // Ho ho ho! Santa has delivered gifts to those lucky participants in this City!  Use the gift bag to claim your prize!
                _NextTalk = DateTime.UtcNow + TimeSpan.FromMinutes(3);
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
            reader.ReadInt();
        }
    }
}
