using Server.Gumps;
using System.Linq;

namespace Server.Items
{
    public class Lute : BaseInstrument
    {
        [Constructable]
        public Lute()
            : base(0xEB3, 0x4C, 0x4D)
        {
            Weight = 5.0;
        }

        public Lute(Serial serial)
            : base(serial)
        {
        }

        public override void PlayInstrumentWell(Mobile from)
        {
            var smfs = from.Backpack.FindItemByType(typeof(SheetMusicForStones)) as SheetMusicForStones;

            if (smfs != null && !smfs.Active)
            {
                var box = from.Map.GetItemsInRange(from.Location, 3).FirstOrDefault(x => x.ItemID == 19724 && x.Hue == 1111);

                if (box != null)
                {
                    from.FixedParticles(0x376A, 1, 72, 0x13B5, EffectLayer.Waist);
                    from.AddToBackpack(new LuteTunedToStones());
                    from.PlaySound(1666);

                    smfs.Active = true;
                    Delete();

                    Gump g = new Gump(100, 100);
                    g.AddBackground(0, 0, 620, 290, 0x2454);
                    g.AddImage(0, 0, 0x9D3F);
                    g.AddHtmlLocalized(280, 14, 328, 18, 1114513, "#1159384", 0xC63, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                    g.AddHtmlLocalized(280, 51, 328, 229, 1159383, 0xC63, false, true); // Greetings, Britannian! Seems Hawkwind is having some fun with the timeline again? Britannia will always face threats, but it is brave adventurers such as yourself who forge the Britannian Legacy everyday. With your courage, you uphold the ideals of Virtue and the integrity of our realm against threats from monstrous evils and world thieves alike. Go now, visit the Well of Souls and free Hawkwind and restore Virtue to the realm!

                    from.SendGump(g);
                }

                return;
            }

            base.PlayInstrumentWell(from);
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
