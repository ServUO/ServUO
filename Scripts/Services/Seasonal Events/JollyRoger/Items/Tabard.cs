using Server.Engines.Craft;
using Server.Engines.JollyRoger;

namespace Server.Items
{
    public class Tabard : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem => Converted ? DefTailoring.CraftSystem : null;

        public override bool IsArtifact => true;

        public Shrine _Shrine { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Converted { get; set; }

        [Constructable]
        public Tabard(Shrine shrine)
            : base(0xA412)
        {
            _Shrine = shrine;

            Weight = 3;
            Hue = JollyRogerData.GetShrineHue(shrine);
            StrRequirement = 0;
        }

        public Tabard(Serial serial)
            : base(serial)
        {
        }

        public void CovertRobe()
        {
            if (!Converted)
            {
                Attributes.RegenMana = 2;
                Attributes.SpellDamage = 5;
                Attributes.LowerManaCost = 10;
                Attributes.LowerRegCost = 10;
                MaxHitPoints = 255;
                HitPoints = 255;

                Converted = true;
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1159369, string.Format("{0}", _Shrine.ToString())); // Tabard of ~1_VIRTUE~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)_Shrine);
            writer.Write(Converted);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Shrine = (Shrine)reader.ReadInt();
            Converted = reader.ReadBool();
        }
    }
}

