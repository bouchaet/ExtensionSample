using Administration;
using Entities;
internal class AdministrationServer : RouteServer
{
    public AdministrationServer(IListener<IDevice> listener)
        : base(listener)
    {
    }

}