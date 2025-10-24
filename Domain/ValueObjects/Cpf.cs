using System.Text.RegularExpressions;

namespace SNI_Events.Domain.ValueObjects
{
    /// <summary>
    /// Value Object para representar um CPF seguindo os princípios de Domain-Driven Design
    /// </summary>
    public class Cpf : IEquatable<Cpf>
    {
        public string Number { get; }

        protected Cpf() { }

        public Cpf(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("CPF não pode estar vazio.", nameof(number));

            var cleanNumber = CleanCpf(number);

            if (!IsValidCpf(cleanNumber))
                throw new ArgumentException("CPF possui formato inválido.", nameof(number));

            Number = cleanNumber;
        }

        private static string CleanCpf(string cpf)
        {
            return Regex.Replace(cpf, @"\D", "");
        }

        private static bool IsValidCpf(string cpf)
        {
            // Remove caracteres não numéricos
            cpf = Regex.Replace(cpf, @"\D", "");

            // Verifica se tem 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação do primeiro dígito verificador
            var sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            var remainder = sum % 11;
            var firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != firstDigit)
                return false;

            // Validação do segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            var secondDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[10].ToString()) != secondDigit)
                return false;

            return true;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Cpf);
        }

        public bool Equals(Cpf? other)
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

        public static implicit operator string(Cpf cpf)
        {
            return cpf.Number;
        }
    }
}
