using System;
using System.Text;
using Entities;

public class QueueServer : RouteServer
{
    private readonly IQueueManager _mgr;
    private const string BAD_REQUEST = "BAD_REQUEST";
    private const string OK = "OK";
    private const string SERVER_ERROR = "SERVER_ERROR";

    public QueueServer(IListener<IDevice> listener, IQueueManager mgr)
    : base(listener)
    {
        listener.Configure("port", 10865);
        _mgr = mgr;
        Add("*", ForwardToMgr); // set the default route
    }

    // todo: route on action using an interface
    private string ForwardToMgr(string message)
    {
        if (_mgr == null) return SERVER_ERROR;

        var req = Parse(message);

        if (req.action.ToLower() == "post")
            return Post(req.qname, req.data);

        return  BadRequest(req.action);
    }

    private string Post(string queuename, string data)
    {
        _mgr.Receive(queuename, Encoding.ASCII.GetBytes(data));
        return OK;
    }

    private string BadRequest(string action)
    {
        Logger.WriteInfo($"Action '{action}' is not suppoted.");
        return BAD_REQUEST;
    }

    private (string action, string qname, string data) Parse(string input)
    {
        // expecting "{action} {queuename} {data}"

        var whitesp = ' ';
        var action = string.Empty;
        var queuename = string.Empty;

        int ws = 0;
        var sb = new StringBuilder();
        foreach (var c in input.TrimStart())
        {
            if (c != whitesp)
            {
                sb.Append(c);
                continue;
            }

            if (c == whitesp)
            {
                if (ws == 0)
                    action = sb.ToString();
                if (ws == 1)
                    queuename = sb.ToString();
                if (ws < 2)
                    sb.Clear();
                else
                    sb.Append(c);

                ++ws;
            }
        }
        var data = sb.ToString();

        Debug.Write($"Queue server received {data.ToCharArray().Length * 2}" +
            $" bytes of data. Action: {action}."+
            $" Destination queue: '{queuename}'. Data: {data}");

        return (action, queuename, data);
    }
}