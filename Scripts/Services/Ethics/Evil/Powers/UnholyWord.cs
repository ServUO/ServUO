using System;

namespace Server.Ethics.Evil
{
    public sealed class UnholyWord : Power
    {
        public UnholyWord()
        {
            this.m_Definition = new PowerDefinition(
                100,
                "Unholy Word",
                "Velgo Oostrac",
                "");
        }

        public override void BeginInvoke(Player from)
        {
        }
    }
}