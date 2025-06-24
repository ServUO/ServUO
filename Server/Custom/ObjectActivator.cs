namespace Server
{
    public delegate T ObjectActivator<out T>( params object[] args );
}