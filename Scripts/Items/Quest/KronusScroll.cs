using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Necro
{
    public class KronusScroll : QuestItem
    {
        private static readonly Rectangle2D m_WellOfTearsArea = new Rectangle2D(2080, 1346, 10, 10);
        private static readonly Map m_WellOfTearsMap = Map.Malas;
        [Constructable]
        public KronusScroll()
            : base(0x227A)
        {
            this.Weight = 1.0;
            this.Hue = 0x44E;
        }

        public KronusScroll(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060149;
            }
        }// Calling of Kronus
        public override bool CanDrop(PlayerMobile player)
        {
            DarkTidesQuest qs = player.Quest as DarkTidesQuest;

            if (qs == null)
                return true;

            /*return !( qs.IsObjectiveInProgress( typeof( FindCallingScrollObjective ) )
            || qs.IsObjectiveInProgress( typeof( FindMardothAboutKronusObjective ) )
            || qs.IsObjectiveInProgress( typeof( FindWellOfTearsObjective ) )
            || qs.IsObjectiveInProgress( typeof( UseCallingScrollObjective ) ) );*/
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from))
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (pm != null)
            {
                QuestSystem qs = pm.Quest;

                if (qs is DarkTidesQuest)
                {
                    if (qs.IsObjectiveInProgress(typeof(FindMardothAboutKronusObjective)))
                    {
                        pm.SendLocalizedMessage(1060151, "", 0x41); // You read the scroll, but decide against performing the calling until you are instructed to do so by Mardoth.
                    }
                    else if (qs.IsObjectiveInProgress(typeof(FindWellOfTearsObjective)))
                    {
                        pm.SendLocalizedMessage(1060152, "", 0x41); // You must be at the Well of Tears in the city of Necromancers to use this scroll.
                    }
                    else if (qs.IsObjectiveInProgress(typeof(UseCallingScrollObjective)))
                    {
                        if (pm.Map == m_WellOfTearsMap && m_WellOfTearsArea.Contains(pm))
                        {
                            QuestObjective obj = qs.FindObjective(typeof(UseCallingScrollObjective));

                            if (obj != null && !obj.Completed)
                                obj.Complete();

                            this.Delete();
                            new CallingTimer(pm).Start();
                        }
                        else
                        {
                            pm.SendLocalizedMessage(1060152, "", 0x41); // You must be at the Well of Tears in the city of Necromancers to use this scroll.
                        }
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1060150, "", 0x41); // A strange terror grips your heart as you attempt to read the scroll.  You decide it would be a bad idea to read it out loud.
                    }
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

        private class CallingTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private int m_Step;
            public CallingTimer(PlayerMobile player)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), 6)
            {
                this.Priority = TimerPriority.TwentyFiveMS;

                this.m_Player = player;
                this.m_Step = 0;
            }

            protected override void OnTick()
            {
                if (this.m_Player.Deleted)
                {
                    this.Stop();
                    return;
                }

                if (!this.m_Player.Mounted)
                    this.m_Player.Animate(Utility.RandomBool() ? 16 : 17, 7, 1, true, false, 0);

                if (this.m_Step == 4)
                {
                    int baseX = KronusScroll.m_WellOfTearsArea.X;
                    int baseY = KronusScroll.m_WellOfTearsArea.Y;
                    int width = KronusScroll.m_WellOfTearsArea.Width;
                    int height = KronusScroll.m_WellOfTearsArea.Height;
                    Map map = KronusScroll.m_WellOfTearsMap;

                    Effects.SendLocationParticles(EffectItem.Create(this.m_Player.Location, this.m_Player.Map, TimeSpan.FromSeconds(1.0)), 0, 0, 0, 0x13C4);
                    Effects.PlaySound(this.m_Player.Location, this.m_Player.Map, 0x243);

                    for (int i = 0; i < 15; i++)
                    {
                        int x = baseX + Utility.Random(width);
                        int y = baseY + Utility.Random(height);
                        int z = map.GetAverageZ(x, y);

                        Point3D from = new Point3D(x, y, z + Utility.RandomMinMax(5, 20));
                        Point3D to = new Point3D(x, y, z);

                        int hue = Utility.RandomList(0x481, 0x482, 0x489, 0x497, 0x66D);

                        Effects.SendPacket(from, map, new HuedEffect(EffectType.Moving, Serial.Zero, Serial.Zero, 0x36D4, from, to, 0, 0, false, true, hue, 0));
                    }
                }

                if (this.m_Step < 5)
                {
                    this.m_Player.Frozen = true;
                }
                else // Cast completed
                {
                    this.m_Player.Frozen = false;

                    SummonedPaladin.BeginSummon(this.m_Player);
                }

                this.m_Step++;
            }
        }
    }
}