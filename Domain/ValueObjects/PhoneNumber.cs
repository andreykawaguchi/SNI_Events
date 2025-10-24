namespace SNI_Events.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para representar um Telefone
    /// </summary>
    public class PhoneNumber : IEquatable<PhoneNumber>
    {
        public string Number { get; }

        protected PhoneNumber() { }

        public PhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Telefone não pode estar vazio.", nameof(number));

            if (number.Length < 10 || number.Length > 15)
                throw new ArgumentException("Telefone deve conter entre 10 e 15 dígitos.", nameof(number));

            Number = number.Trim();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PhoneNumber);
        }

        public bool Equals(PhoneNumber? other)
        {
            return other is not null && Number == other.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override string ToString()
        {
            return Number;
        }

        public static implicit operator string(PhoneNumber phone)
        {
            return phone.Number;
        }
    }
}
