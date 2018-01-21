using System;
using Entities;

internal class QueueProtocol : ListenerProtocol
{
    public override string Accept => string.Empty;

    public override string EOF => Environment.NewLine;

    public override string Exit => "quit";

    protected override string Greetings => "Welcome to Queue Server v0.1" + EOF;

    protected override string Goodbye => string.Empty;

    protected override string GetNext(string req) => string.Empty;
}