using BfmEventDS = Entities.BfmEvent;

namespace BfmEvent.Details
{
    public class TokenDeserializer : IDeserializer<BfmEventDS>
    {
        public BfmEventDS Deserialize(string s)
        {
            var tokens = s.Split('#');
            return tokens.Length > 1
                ? new BfmEventDS(tokens[0], tokens[1])
                : new BfmEventDS(s, s);
        }
    }
}