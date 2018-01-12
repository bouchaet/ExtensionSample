namespace Entities
{
    public class BfmEvent
    {
        public string Id { get; }
        public string SubId { get; }

        public BfmEvent(string id, string subid)
        {
            Id = id;
            SubId = subid;
        }
    }
}
