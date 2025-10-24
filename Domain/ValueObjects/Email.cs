using System.Text.RegularExpressions;

namespace SNI_Events.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para representar um Email seguindo os princípios de Domain-Driven Design
    /// </summary>
    public class Email : IEquatable<Email>
    {
        public string Address { get; }

        protected Email() { }

        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Email não pode estar vazio.", nameof(address));

            if (!IsValidEmail(address))
                throw new ArgumentException("Email possui formato inválido.", nameof(address));

            Address = address.ToLower().Trim();
        }

        private static bool IsValidEmail(string email)
        {
            const string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Email);
        }

        public bool Equals(Email? other)
        {
            return other is not null && Address == other.Address;
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override string ToString()
        {
            return Address;
        }

        public static implicit operator string(Email email)
        {
            return email.Address;
        }
    }
}
