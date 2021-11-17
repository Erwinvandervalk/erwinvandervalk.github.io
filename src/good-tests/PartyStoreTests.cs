using System.Threading.Tasks;
using FluentAssertions;
using good_tests.Party;
using good_tests.TestInfra.Data;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace good_tests
{
    public class PartyStoreTests
    {
        private readonly ITestOutputHelper _output;
        private readonly TestData The = new TestData();
        private TestDataBuilder Some => new TestDataBuilder(The);
        private readonly PartyFixture Fixture;

        private PartyStore PartyStore => Fixture.PartyStore;

        public PartyStoreTests(ITestOutputHelper output)
        {
            _output = output;
            Fixture = new PartyFixture(output);
        }

        [Fact]
        public void One_person_does_not_make_a_fun_party()
        {
            var party = Some.Party()
                with
                {
                    Attendees = new[] {The.AttendeePresentJohn}
                };

            party.IsFun().Should().BeFalse();
        }

        [Fact]
        public async Task Can_save_new_party()
        {
            var party = Some.Party();
            await PartyStore.Save(party);

            var saved = await PartyStore.Get(The.PartyId);

            saved.Should().BeEquivalentTo(party);
        }

        [Fact]
        public async Task Can_modify_existing_party()
        {
            var party = await SaveNewParty();

            var differentName = new PartyName("different");
            var changed = party with
            {
                Name = differentName
            };

            await PartyStore.Save(changed);

            var saved = await PartyStore.Get(The.PartyId);
            saved.Name.Should().Be(differentName);
        }

        private async Task<Party.Party?> SaveNewParty()
        {
            var party = Some.Party();
            await PartyStore.Save(party);
            return party;
        }


        [Fact]
        public async Task Can_add_attendee_to_a_party()
        {
            var party = Some.Party();
            await PartyStore.Save(party);

            var partyWithAttendee = party.AddAttendee(The.AttendeeAbsentAlice);
            await PartyStore.Save(partyWithAttendee);
            var existingParty = await PartyStore.Get(The.PartyId);

            existingParty.ShouldNotBeNull();
            existingParty.Attendees.ShouldContain(The.AttendeeAbsentAlice);
        }
    }
}
