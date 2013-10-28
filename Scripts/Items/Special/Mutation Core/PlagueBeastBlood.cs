using System;
using Server.Network;

namespace Server.Items
{
    public class PlagueBeastBlood : PlagueBeastComponent
    {
        private readonly Timer m_Timer;
        public PlagueBeastBlood()
            : base(0x122C, 0)
        {
            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5), 3, new TimerCallback(Hemorrhage));
        }

        public PlagueBeastBlood(Serial serial)
            : base(serial)
        {
        }

        public bool Patched
        {
            get
            {
                return this.ItemID == 0x1765;
            }
        }
        public bool Starting
        {
            get
            {
                return this.ItemID == 0x122C;
            }
        }
        public override void OnAfterDelete()
        {
            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
        }

        public override bool OnBandage(Mobile from)
        {
            if (this.IsAccessibleTo(from) && !this.Patched)
            {
                if (this.m_Timer != null && this.m_Timer.Running)
                    this.m_Timer.Stop();

                if (this.Starting)
                {
                    this.X += 2;
                    this.Y -= 9;

                    if (this.Organ is PlagueBeastRubbleOrgan)
                        this.Y -= 5;
                    else if (this.Organ is PlagueBeastBackupOrgan)
                        this.X += 7;
                }
                else
                {
                    this.X -= 4;
                    this.Y -= 2;
                }

                this.ItemID = 0x1765;

                if (this.Owner != null)
                {
                    Container pack = this.Owner.Backpack;

                    if (pack != null)
                    {
                        for (int i = 0; i < pack.Items.Count; i++)
                        {
                            PlagueBeastMainOrgan main = pack.Items[i] as PlagueBeastMainOrgan;

                            if (main != null && main.Complete)
                                main.FinishOpening(from);
                        }
                    }
                }

                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071916); // * You patch up the wound with a bandage *

                return true;
            }

            return false;
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
        }

        private void Hemorrhage()
        {
            if (this.Patched)
                return;

            if (this.Owner != null)
                this.Owner.PlaySound(0x25);

            if (this.ItemID == 0x122A)
            {
                if (this.Owner != null)
                {
                    this.Owner.Unfreeze();
                    this.Owner.Kill();
                }
            }
            else
            {
                if (this.Starting)
                {
                    this.X += 8;
                    this.Y -= 10;
                }

                this.ItemID--;
            }
        }
    }
}