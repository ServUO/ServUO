using Server.Mobiles;

namespace Server.Engines.ArtisanFestival
{
    public class SantasGiftBag : Item
    {
        public override bool ForceShowProperties => true;
        public override int LabelNumber => 1157280;  // Santa's Gift Bag

        public SantasGiftBag()
            : base(0x9DB5)
        {
            Movable = false;
        }

        public SantasGiftBag(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm && from.InRange(GetWorldLocation(), 3))
            {
                var festival = ArtisanFestivalEvent.Instance;

                if (festival.Running && festival.ClaimPeriod)
                {
                    if (festival.Winners != null && festival.Winners.ContainsKey(pm))
                    {
                        if (festival.Winners[pm])
                        {
                            pm.SendLocalizedMessage(1157282); // You have already claimed your Artisan Festival reward.
                        }
                        else
                        {
                            var reward = new FestivalGiftBox();

                            if (pm.Backpack == null || !pm.Backpack.TryDropItem(pm, reward, false))
                            {
                                pm.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                                reward.Delete();
                            }
                            else
                            {
                                pm.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                                festival.Winners[pm] = true;
                            }
                        }
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1157281); // Santa thanks you for your efforts in the Artisan festival, but you did not qualify for a reward.  Try again in the next city!
                    }
                }
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
