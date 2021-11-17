namespace good_tests.TestInfra.Data
{
    /// <summary>
    /// The test data creates new instances of classes to test with. 
    /// </summary>
    public class TestDataBuilder
    {
        public readonly TestData The;

        public TestDataBuilder(TestData the)
        {
            The = the;
        }

        /// <summary>
        /// Create a VALID instance of a party. It's easy to turn
        /// a valid instance in an invalid one but most tests actually work
        /// with valid data. So, start from a VALID instance and work
        /// from there. 
        /// </summary>
        /// <returns></returns>
        public Party.Party Party() => new Party.Party(
            Id: The.PartyId,
            Name: The.PartyName,
            Location: The.Location,
            Date: The.PartyDate,
            Attendees: The.PresentAttendees);
    }
}
