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

        public override string ToString() => 
            $"{{'Id': '{Id}', 'SubId': '{SubId}'}}";
    }
}