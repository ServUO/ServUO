using System;

namespace Server.Items
{
    public class Pincer : BattleAxe
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154476; } } // Pincer

        [Constructable]
        public Pincer()
        {
            this.Hue = 2500;

            this.Attributes.BalancedWeapon = 1;
            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.SetSkillBonuses.SetValues(0, SkillName.Lumberjacking, 10);
            this.WeaponAttributes.HitLeechHits = 87;
            this.Attributes.RegenStam = 6;
            this.Attributes.RegenMana = 6;
            this.Attributes.WeaponDamage = 40;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 30; fire = 70;
            cold = nrgy = chaos = direct = pois = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public Pincer(Serial serial)
            : base(serial)
        {
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

    public class GargishPincer : GargishBattleAxe
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154476; } } // Pincer

        [Constructable]
        public GargishPincer()
        {
            this.Hue = 2500;

            this.Attributes.BalancedWeapon = 1;
            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.SetSkillBonuses.SetValues(0, SkillName.Lumberjacking, 10);
            this.WeaponAttributes.HitLeechHits = 87;
            this.Attributes.RegenStam = 6;
            this.Attributes.RegenMana = 6;
            this.Attributes.WeaponDamage = 40;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 30; fire = 70;
            cold = nrgy = chaos = direct = pois = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishPincer(Serial serial)
            : base(serial)
        {
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