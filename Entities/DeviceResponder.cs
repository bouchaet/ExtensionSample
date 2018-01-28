using Entities;

internal class DeviceResponder : Port<IDevice>
{
    private IRouter _router;

    public DeviceResponder(IRouter router)
    {
        _router = router;
    }

    protected override void PostReceive(IDevice d)
    {
        var input = d.ReadLine();
        
        d.WriteLine(_router?
            .GetHandler(input)?
            .Map(input)?
            .ToString());
    }

    protected override void PostTransfer(IDevice d)
    {
    }

    protected override void PreReceive(IDevice d)
    {
    }

    protected override void PreTransfer(IDevice d)
    {
    }
}
