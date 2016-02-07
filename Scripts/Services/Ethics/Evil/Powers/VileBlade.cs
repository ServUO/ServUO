using System;

namespace Server.Ethics.Evil
{
    public sealed class VileBlade : Power
    {
        public VileBlade()
        {
            this.m_Definition = new PowerDefinition(
                10,
                "Vile Blade",
                "Velgo Reyam",
                "");
        }

        public override void BeginInvoke(Player from)
        {
        }
    }
}