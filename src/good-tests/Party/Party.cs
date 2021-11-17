using System.Collections.Generic;
using System.Linq;

namespace good_tests.Party
{
    public record Party(PartyId Id, PartyName Name, PartyLocation Location, PartyDate Date, IReadOnlyCollection<Attendee> Attendees)
    {
        public Party AddAttendee(Attendee attendee)
        {
            return this
                with
                {
                    Attendees = this.Attendees.Append(attendee).ToArray()
                };
        }

        public bool IsFun()
        {
            // A party is fun if it has
            return Attendees.Count >= 2

                   // 
                   && !Attendees.Any(x => x.Name.ToString().Contains("annoying"));
        }
    }
}