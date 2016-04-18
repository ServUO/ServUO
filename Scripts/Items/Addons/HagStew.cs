using System;

namespace Server.Items
{
    public class HagStew : BaseAddon
    {
        [Constructable]
        public HagStew()
        {
            AddonComponent stew;
            stew = new AddonComponent(2416);
            stew.Name = "stew";
            stew.Visible = true;
            this.AddComponent(stew, 0, 0, -7);      //stew
        }

        public HagStew(Serial serial)
            : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent stew, Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
                from.SendMessage("You are too far away.");
            else
            {
                {
                    stew.Visible = false;

                    BreadLoaf hagstew = new BreadLoaf();        //this decides your fillrate
                    hagstew.Eat(from);

                    Timer m_timer = new ShowStew(stew);
                    m_timer.Start();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public class ShowStew : Timer
        {
            private readonly AddonComponent stew;
            public ShowStew(AddonComponent ac)
                : base(TimeSpan.FromSeconds(30))
            {
                this.stew = ac;
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (this.stew.Visible == false)
                {
                    this.Stop();
                    this.stew.Visible = true;
                    return;
                }
            }
        }
    }
}