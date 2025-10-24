namespace SNI_Events.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para representar uma Senha com hash
    /// </summary>
    public class Password : IEquatable<Password>
    {
        public string Hash { get; }

        protected Password() { }

        public Password(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Senha não pode estar vazia.", nameof(hash));

            if (hash.Length < 60) // BCrypt hash mínimo
                throw new ArgumentException("Senha possui formato inválido.", nameof(hash));

            Hash = hash;
        }

        public static Password Create(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Senha não pode estar vazia.", nameof(plainPassword));

            var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            return new Password(hash);
        }

        public bool Verify(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, Hash);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Password);
        }

        public bool Equals(Password? other)
        {
            return other is not null && Hash == other.Hash;
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public static implicit operator string(Password password)
        {
            return password.Hash;
        }
    }
}
