using System;
using Entities;

namespace Server.Details.Devices
{
    internal class ConsoleDevice : IDevice
    {
        public void Open()
        {
            Console.WriteLine("Hello");
        }

        public int Read(char[] buffer, int pos, int count)
        {
            int c;
            int total = 0;
            while((c = Console.Read()) > -1 && total++ < count)
                if(total > pos)
                    buffer[total-1] = Convert.ToChar(c);

            return total > count? count: total;
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