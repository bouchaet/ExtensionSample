using System;
using System.Linq;

namespace Http
{
    public abstract class Pattern
    {
        private bool _eligible = true;
        abstract public int Length { get; }
        abstract protected bool Accept(byte item, int pos);

        public bool Try(byte item, int pos)
        {
            return CanAccept ? Eligible = Accept(item, pos) : false;
        }

        public bool CanAccept => _eligible;

        public virtual void Reset() =>
             _eligible = true;

        protected bool Eligible
        {
            get => _eligible;
            set => _eligible = value;
        }

        public static Pattern Any => new Any();

        public static Pattern In(char[] array) =>
            array.Select(d => Convert.ToByte(d))
                .Select(b => new One(b) as Pattern)
                .Aggregate((a, n) => a | n);

        public static Pattern operator +(Pattern lhs, Pattern rhs) =>
            new Concat(lhs, rhs);

        public static Pattern operator |(Pattern lhs, Pattern rhs) =>
            new Or(lhs, rhs);

        public static Pattern operator *(Pattern lhs, int times) =>
            new Repeat(lhs, times);

        public static implicit operator Pattern(char @char) =>
            new One(Convert.ToByte(@char));

        public static implicit operator Pattern(char[] sequence) =>
            sequence
                .Select(ch => Convert.ToByte(ch))
                .Select(b => new One(b) as Pattern)
                .Aggregate((a, n) => a + n);


        public static implicit operator Pattern(string sequence) =>
            sequence.ToCharArray();
    }
}