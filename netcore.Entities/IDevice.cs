namespace Entities
{
    public interface IDevice
    {
        void Open();
        string ReadLine();
        void Write(char[] s, int index, int count);
        void WriteLine(string s);
        void Seek(int pos);
        void Close();
    }
}