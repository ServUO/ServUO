using System;

namespace Server.Mobiles
{
    public interface IFreezable
    {
        void OnRequestedAnimation(Mobile from);
    }
}