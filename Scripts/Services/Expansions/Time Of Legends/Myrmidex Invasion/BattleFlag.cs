using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Linq;
using System.Collections.Generic;

namespace Server.Engines.MyrmidexInvasion
{
    public class BattleFlag : Item
	{
        [Constructable]
        public BattleFlag(int itemid) : base(itemid)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(Location, 3))
            {
                BattleSpawner spawner = BattleSpawner.Instance;

                if (spawner != null)
                {
                    DisplayWaveInfo(spawner, m);
                }
            }
            else
                m.SendLocalizedMessage(500618); // That is too far away!
        }

        public static void DisplayWaveInfo(BattleSpawner spawner, Mobile m)
        {
            int delay = 0;
            ColUtility.ForEach(spawner.MyrmidexTeam, kvp =>
                {
                    if (kvp.Value.Count > 0)
                    {
                        int wave = kvp.Key + 1;
                        int count = kvp.Value.Where(bc => bc.Alive).Count();

                        Timer.DelayCall(TimeSpan.FromSeconds(delay), () =>
                            {
                                m.SendLocalizedMessage(1156606, String.Format("{0}\t{1}\t{2}", (BattleSpawner.WaveCount - count).ToString(), BattleSpawner.WaveCount.ToString(), wave.ToString())); // Myrmidex have lost ~1_VAL~ of ~2_VAL~ from wave ~3_VAL~ of their front line.	
                            });
                    }

                    delay++;
                });

            delay = 0;
            ColUtility.ForEach(spawner.TribeTeam, kvp =>
                {
                    if (kvp.Value.Count > 0)
                    {
                        int wave = kvp.Key + 1;
                        int count = kvp.Value.Where(bc => bc.Alive).Count();

                        Timer.DelayCall(TimeSpan.FromSeconds(delay), () =>
                        {
                            m.SendLocalizedMessage(1156607, String.Format("{0}\t{1}\t{2}", (BattleSpawner.WaveCount - count).ToString(), BattleSpawner.WaveCount.ToString(), wave.ToString())); // Myrmidex have lost ~1_VAL~ of ~2_VAL~ from wave ~3_VAL~ of their front line.	
                        });
                    }

                    delay++;
                });
        }

        public BattleFlag(Serial serial)
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