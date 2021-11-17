namespace good_tests.Party
{
    public record NonEmptyString
    {
        private readonly string _value;

        public NonEmptyString(string value)
        {
            _value = value;
        }

        public static implicit operator string(NonEmptyString value) => value?._value;
        public static implicit operator NonEmptyString(string value)
        {
            return new NonEmptyString(value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}