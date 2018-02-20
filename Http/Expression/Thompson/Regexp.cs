using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

internal class State
{
    public int c;
    public State output;
    public State output1;
    public int lastlist;

    public State(int c, State o, State o1)
    {
        this.c = c;
        output = o;
        output1 = o1;
        lastlist = 0;
    }
}

internal class Frag
{
    public State start;
    public IList<Action<State>> outputs;

    public Frag(State s, IList<Action<State>> o)
    {
        start = s;
        outputs = o;
    }
}

internal static class Regexp
{
    private static int SPLIT = 256;
    private static int MATCH = 257;
    internal static IList<Action<State>> List1(Action<State> accessor)
    {
        return new List<Action<State>> { x => accessor.Invoke(x) };
    }

    internal static IList<Action<State>> Append(IList<Action<State>> list1, IList<Action<State>> list2)
    {
        return list1.Concat(list2).ToList();
    }

    internal static void Patch(IList<Action<State>> l, State s)
    {
        foreach (var o in l)
            o.Invoke(s);
    }

    internal static State Post2Nfa(string postfix)
    {
        var stack = new Stack<Frag>();
        Frag e1, e2, e;

        foreach (var c in postfix.ToCharArray())
        {
            switch (c)
            {
                case '.': /* catenate */
                    {
                        e2 = stack.Pop();
                        e1 = stack.Pop();
                        Patch(e1.outputs, e2.start);
                        stack.Push(new Frag(e1.start, e2.outputs));
                        break;
                    }
                case '|': /* alternate */
                    {
                        e2 = stack.Pop();
                        e1 = stack.Pop();
                        var s = new State(SPLIT, e1.start, e2.start);
                        stack.Push(new Frag(s, Append(e1.outputs, e2.outputs)));
                        break;
                    }
                case '?': /* zero or one */
                    {
                        e = stack.Pop();
                        var s = new State(SPLIT, e.start, null);
                        stack.Push(new Frag(s, Append(e.outputs, List1(x => s.output1 = x))));
                        break;
                    }
                case '+': /* zero or more */
                    {
                        e = stack.Pop();
                        var s = new State(SPLIT, e.start, null);
                        Patch(e.outputs, s);
                        stack.Push(new Frag(s, List1(x => s.output1 = x)));
                        break;
                    }
                default:
                    {
                        var b = Convert.ToByte(c);
                        var s = new State(b, null, null);
                        stack.Push(new Frag(s, List1(x => s.output = x)));
                        break;
                    }
            }
        }
        e = stack.Pop();
        if (stack.Any()) return null;

        Patch(e.outputs, new State(MATCH, null, null));
        return e.start;
    }

    private static int ListId = 0;

    internal static IList<State> StartList(State start, IList<State> list)
    {
        ListId++;
        AddState(list, start);
        return list;
    }

    internal static void AddState(IList<State> list, State s)
    {
        if (s == null || s.lastlist == ListId)
            return;

        s.lastlist = ListId;
        if (s.c == SPLIT)
        {
            AddState(list, s.output);
            AddState(list, s.output1);
            return;
        }
        list.Add(s);
    }

    internal static void Step(IList<State> current, int c, IList<State> next)
    {
        ListId++;
        next.Clear();
        foreach (var s in current)
        {
            if (s.c == c)
                AddState(next, s.output);
        }
    }

    internal static int Match(State start, string text)
    {
        var current = StartList(start, new List<State>());
        IList<State> next = new List<State>();
        IList<State> temp = new List<State>();

        foreach (var c in text.ToCharArray())
        {
            Step(current, c, next);
            temp = current; current = next; next = temp;
        }

        return IsMatch(current);
    }

    internal static int IsMatch(IList<State> list)
    {
        return list.Any(s => s.c == MATCH) ? 1 : 0;
    }
}

public class RegexpTest
{
    [Fact]
    public void TestWithPostfix()
    {
        var start = Regexp.Post2Nfa("abb.+.a.");
        var match = Regexp.Match(start, "abbbba");

        Assert.Equal(1, match);
    }
}