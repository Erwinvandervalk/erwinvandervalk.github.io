using System;

namespace good_tests.Party
{
    public record PartyId(Guid Value)
    {
        public override string ToString()
        {
            return "p:" + Value.ToString();
        }
    }
}