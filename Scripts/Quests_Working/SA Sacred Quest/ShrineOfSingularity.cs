using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class ShrineOfSingularity : Item
    {
        [Constructable]
        public ShrineOfSingularity()
            : base(0x48A8)
        {
            this.Movable = false;
            this.Name = "ShrineOfSingularity";	
        }

        public ShrineOfSingularity(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }
        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(this.Location, 2))
            {
                PlayerMobile pm = e.Mobile as PlayerMobile;

                if (pm.AbyssEntry)
                {
                    e.Mobile.SendLocalizedMessage(1112697); 
                    e.Mobile.PlaySound(0xF9);
                }
                else 
                {
                    if (e.Speech.ToLower() == "unorus")
                        e.Handled = true;
                    e.Mobile.SendGump(new SQGump(e.Mobile));
                    e.Mobile.PlaySound(0xF9);
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
    }
}