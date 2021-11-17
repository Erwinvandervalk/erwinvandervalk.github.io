using System;

namespace good_tests.Party
{
    public record PartyDocument(Guid Id, string Name, string Location, DateTimeOffset Date, string[] Attendees);
}