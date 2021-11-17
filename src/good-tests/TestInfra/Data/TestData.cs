using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using good_tests.Party;
using good_tests.TestInfra.Helpers;

namespace good_tests.TestInfra.Data
{
    public class TestData
    {
        public PartyId PartyId = new PartyId(Guid.NewGuid());
        public PartyName PartyName = new PartyName(MyName());
        public Organizer Organizer = new(MyName());

        public Attendee AttendeePresentJohn = new(MyName());
        public Attendee AttendeePresentCatherine = new(MyName());
        public Attendee AttendeeAbsentAlice = new(MyName());
        public Attendee AttendeeAnnoyingAnton = new ( MyName());

        public IReadOnlyCollection<Attendee> PresentAttendees;

        public PartyDate PartyDate => new PartyDate(CurrentDateTime);

        public PartyLocation Location = new PartyLocation(new(MyName()));

        public TestData()
        {
            PresentAttendees = new[] { AttendeePresentJohn, AttendeePresentCatherine};
        }

        /// <summary>
        ///  Always control the current time in your unit
        /// tests. This way, you can easily time travel
        /// </summary>
        public DateTime CurrentDateTime =
            new DateTime(2000, 1, 2, 3, 4, 5);


        /// <summary>
        /// This method is a little trick. It returns the
        /// name of the calling property, but then in snake case
        /// and surrounded by hashtags. This makes it
        /// immediately clear when observing the data that the value
        /// actually IS the expected property, and no more and no less.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static string MyName(
            [CallerMemberName] string attribute = null)
        {
            return $"#{attribute.ToSnakeCase()}#";
        }
    }
}
