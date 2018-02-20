using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using StateSetterList = System.Collections.Generic.IList<System.Action<State>>;

internal class State
{
    public int c { get; set; }
    public State Output { get; private set; }
    public State Output1 { get; private set; }
    public int LastList { get; set; }

    public State(int c, State o, State o1)
    {
        this.c = c;
        Output = o;
        Output1 = o1;
        LastList = 0;
    }

    public void SetOutput(State s) => Output = s;
    public void SetOutput1(State s) => Output1 = s;

}

internal class Frag
{
    public State Start { get; }
    public StateSetterList Outputs { get; }

    public Frag(State s, StateSetterList o)
    {
        Start = s;
        Outputs = o;
    }
}

public class Expr
{
    private State _start;

    internal Expr(State start) 
        => _start = start;
    
    public int Match(string s) 
        => _start == null? 0 : Regexp.Match(_start, s);
}

internal static class Regexp
{
    private static int SPLIT = 256;
    private static int MATCH = 257;
    internal static StateSetterList List1(Action<State> accessor)
    {
        return new List<Action<State>> { accessor };
    }

    private static StateSetterList Append(StateSetterList list1, StateSetterList list2)
        => list1.Concat(list2).ToList();


    private static void Patch(StateSetterList l, State s)
    {
        foreach (var setter in l)
            setter.Invoke(s);
    }

    public static Expr CompilePostfix(string postfix)
        => new Expr(Post2Nfa(postfix));

    private static State Post2Nfa(string postfix)
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
                        Patch(e1.Outputs, e2.Start);
                        stack.Push(new Frag(e1.Start, e2.Outputs));
                        break;
                    }
                case '|': /* alternate */
                    {
                        e2 = stack.Pop();
                        e1 = stack.Pop();
                        var s = new State(SPLIT, e1.Start, e2.Start);
                        stack.Push(new Frag(s, Append(e1.Outputs, e2.Outputs)));
                        break;
                    }
                case '?': /* zero or one */
                    {
                        e = stack.Pop();
                        var s = new State(SPLIT, e.Start, null);
                        stack.Push(new Frag(s, Append(e.Outputs, List1(s.SetOutput1))));
                        break;
                    }
                case '+': /* zero or more */
                    {
                        e = stack.Pop();
                        var s = new State(SPLIT, e.Start, null);
                        Patch(e.Outputs, s);
                        stack.Push(new Frag(s, List1(s.SetOutput1)));
                        break;
                    }
                default:
                    {
                        var b = Convert.ToByte(c);
                        var s = new State(b, null, null);
                        stack.Push(new Frag(s, List1(s.SetOutput)));
                        break;
                    }
            }
        }
        e = stack.Pop();
        if (stack.Any())
            return null;

        Patch(e.Outputs, new State(MATCH, null, null));
        return e.Start;
    }

    private static IList<State> StartList(State start, IList<State> list, ref int ListId)
    {
        ListId++;
        AddState(list, start, ref ListId);
        return list;
    }

    private static void AddState(IList<State> list, State s, ref int ListId)
    {
        if (s == null || s.LastList == ListId)
            return;

        s.LastList = ListId;
        if (s.c == SPLIT)
        {
            AddState(list, s.Output, ref ListId);
            AddState(list, s.Output1, ref ListId);
            return;
        }
        list.Add(s);
    }

    private static void Step(IList<State> current, int c, IList<State> next, ref int ListId)
    {
        ListId++;
        next.Clear();
        foreach (var state in current)
        {
            if (state.c == c)
                AddState(next, state.Output, ref ListId);
        }
    }

    internal static int Match(State start, string text)
    {
        var ListId = 0;
        var current = StartList(start, new List<State>(), ref ListId);
        IList<State> next = new List<State>();
        IList<State> temp = new List<State>();

        foreach (var c in text)
        {
            Step(current, c, next, ref ListId);
            temp = current; current = next; next = temp;
        }

        return IsMatch(current);
    }

    private static int IsMatch(IList<State> list)
        => list.Any(s => s.c == MATCH) ? 1 : 0;
}

public class RegexpTest
{
    [Fact]
    public void TestWithPostfix()
    {
        var expr = Regexp.CompilePostfix("abb.+.a.");
        var match = expr.Match("abbbba");

        Assert.True(match == 1);
        Assert.False(expr.Match("abbba") == 1);
    }
}