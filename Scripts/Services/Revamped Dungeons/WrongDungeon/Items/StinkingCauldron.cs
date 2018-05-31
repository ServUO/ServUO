using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class StinkingCauldron : Item
    {
        public override int LabelNumber { get { return 1152163; } } // stinking cauldron

        [Constructable]
        public StinkingCauldron()
            : base(0x142A)
        {
            Movable = false;
            Hue = 363;
            Weight = 0.0;
            Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(GooeyMaggotsSpawn));
        }

        private void GooeyMaggotsSpawn()
        {
            if (Map != Map.Internal)
            {
                int amount = Utility.RandomMinMax(1, 2);

                for (int i = 0; i < amount; ++i)
                {
                    BaseCreature creature = new GooeyMaggots();

                    creature.GetSpawnPosition(2);
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                }
            }

            Delete();
        }

        public StinkingCauldron(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(GooeyMaggotsSpawn));
        }
    }
}
