using System;
using Entities;
internal class AdminProtocol : ListenerProtocol
{
    public override string Accept => "accept";

    public override string EOF => Environment.NewLine;

    public override string Exit => "quit";

    protected override string Greetings => "Enter 'quit' to exit." + EOF;

    protected override string Goodbye => "Bye!";

    protected override string GetNext(string req)
    {
        return string.Empty;
    }
}
