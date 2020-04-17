namespace Server.Items
{
    public class SmilingMoonBlade : CrescentBlade
    {
        public override int LabelNumber => 1116628;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public SmilingMoonBlade()
        {
            Hue = 2567;
            WeaponAttributes.HitManaDrain = 10;
            WeaponAttributes.HitFireball = 45;
            WeaponAttributes.HitLowerDefend = 40;
            WeaponAttributes.BattleLust = 1;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 45;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = chaos = nrgy = pois = direct = 0;
            cold = 100;
        }

        public SmilingMoonBlade(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}