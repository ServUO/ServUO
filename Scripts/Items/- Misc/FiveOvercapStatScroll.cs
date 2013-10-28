using System;

namespace Server.Items
{
    public class FiveOvercapStatScroll : Item
    {
        [Constructable]
        public FiveOvercapStatScroll()
            : base(0x14F0)
        {
            this.Name = "+5 Overcap StatScroll";
            this.Weight = 1.0;
            this.LootType = LootType.Cursed;
            this.Hue = 0x481;
        }

        public FiveOvercapStatScroll(Serial serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                if (from.StatCap == 250)
                {
                    from.StatCap += 5;

                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x243);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);  

                    from.SendLocalizedMessage(1049512); // You feel a surge of magic as the scroll enhances your powers!
                    this.Delete();           
                }
                else
                {
                    from.SendMessage("You can use this only if you have 250 Stat Total on your Character !!");
                }
            }
        }
    }
}