namespace SmartScheduling.Domain.ValueObjects;

public sealed class PhoneNumber
{
    public string Value { get; }
    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Numero de telefone nao pode ser vazio.", nameof(value));
        var digits = new string(value.Where(char.IsDigit).ToArray());
        if (digits.Length < 10 || digits.Length > 15)
            throw new ArgumentException("Numero de telefone invalido.", nameof(value));
        return new PhoneNumber(digits);
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is PhoneNumber other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
