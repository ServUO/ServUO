//BaleFire//

using System;
using Server;

namespace Server.Items
{
    public class Tundra : MetalShield, ITokunoDyable
    {
        //public override int BasePhysicalResistance { get { return 15; } }
        //public override int BasePoisonResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        //public override int BaseEnergyResistance { get { return 15; } }
        //public override int BaseFireResistance { get { return 15; } }
        public override int ArtifactRarity { get { return 13; } }
        public override int InitMinHits { get { return 300; } }
        public override int InitMaxHits { get { return 300; } }

        [Constructable]
        public Tundra()
        {
            Name = "-Tundra-";
            ItemID = 2597;
            Hue = Utility.RandomList(1150, 1151, 1152, 1153, 1154, 2066);
            StrRequirement = 15;
            //Attributes.BonusStr = 10;
            //Attributes.BonusInt = 10;
            Attributes.BonusDex = 10;
            Attributes.SpellChanneling = 1;
            //Attributes.NightSight = 1;
            Attributes.AttackChance = 5;
            //Attributes.DefendChance = 10;
            Attributes.ReflectPhysical = 35;
            Attributes.Luck = 150;
            ArmorAttributes.SelfRepair = 3;
            //SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0);
            Light = LightType.Circle300;

        }

        public Tundra(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            if (!IsChildOf(from.Backpack))

                switch (this.ItemID)
                {
                    case 0:
                        {
                            this.ItemID = 2597;
                            this.Name = "Tundra";
                            break;
                        }
                    case 1:
                        {
                            this.ItemID = 2597;
                            this.Name = "HeroFlame";
                            break;
                        }
                    case 2:
                        {
                            this.ItemID = 2597;
                            this.Name = "ToxicLight";
                            break;
                        }
                    case 3:
                        {
                            this.ItemID = 2597;
                            this.Name = "MageGuide";
                            break;
                        }
                    case 4:
                        {
                            this.ItemID = 2597;
                            this.Name = "NetherGuard";
                            break;
                        }
                }

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
    }
}