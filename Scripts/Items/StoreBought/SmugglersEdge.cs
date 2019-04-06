using System;
using Server;
using Server.Mobiles;
using Server.Engines.CreatureStealing;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishSmugglersEdge))]
    public class SmugglersEdge : ButcherKnife
    {
        public override int LabelNumber { get { return 1071499; } } // Smuggler's Edge
        public override bool CanFortify { get { return false; } }
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SmugglersEdge()
        {
            Hue = 1461;

            WeaponAttributes.UseBestSkill = 1;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 30;

            if (!Siege.SiegeShard)
                LootType = LootType.Blessed;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        public override int AosMinDamage { get { return 9; } }
        public override int AosMaxDamage { get { return 11; } }

        public SmugglersEdge(Serial serial)
            : base(serial)
        {
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            if (damageable is BaseCreature)
            {
                if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
                {
                    attacker.SendLocalizedMessage(1071501); // Your left hand must be free to steal an item from the creature.
                }
                else if (attacker is PlayerMobile)
                {
                    StealingHandler.HandleSmugglersEdgeSteal((BaseCreature)damageable, (PlayerMobile)attacker);
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1071500); // Monster Stealing
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
                Attributes.WeaponSpeed = 30;
        }
    }

    public class GargishSmugglersEdge : GargishButcherKnife
    {
        public override int LabelNumber { get { return 1071499; } } // Smuggler's Edge
        public override bool CanFortify { get { return false; } }
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishSmugglersEdge()
        {
            Hue = 1461;

            WeaponAttributes.UseBestSkill = 1;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 30;

            if (!Siege.SiegeShard)
                LootType = LootType.Blessed;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        public override int AosMinDamage { get { return 9; } }
        public override int AosMaxDamage { get { return 11; } }

        public GargishSmugglersEdge(Serial serial)
            : base(serial)
        {
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            if (damageable is BaseCreature)
            {
                if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
                {
                    attacker.SendLocalizedMessage(1071501); // Your left hand must be free to steal an item from the creature.
                }
                else if (attacker is PlayerMobile)
                {
                    StealingHandler.HandleSmugglersEdgeSteal((BaseCreature)damageable, (PlayerMobile)attacker);
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1071500); // Monster Stealing
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
                Attributes.WeaponSpeed = 30;
        }
    }
}
