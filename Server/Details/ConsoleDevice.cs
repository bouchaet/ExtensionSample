using System;
using Entities;

namespace Server.Details
{
    internal class ConsoleDevice : IDevice
    {
        public void Open()
        {
            Console.WriteLine("Hello");
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(char[] s, int index, int count)
        {
            Console.Write(s, index, count);
        }   

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }

        public void Seek(int pos)
        {

        }

        public void Close()
        {
            Console.WriteLine("goodbye");
        }

    }
}