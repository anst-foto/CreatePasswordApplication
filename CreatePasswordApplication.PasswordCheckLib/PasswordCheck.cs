namespace CreatePasswordApplication.PasswordCheckLib;

public class PasswordCheck
{
    public string Password { get; set; }
    
    public required int MinLength { get; init; }
    public required string MasterLetters { get; init; }
    public string? SecondLetters { get; init; }
    public string? Digits { get; init; }
    public string? Symbols { get; init; }

    public bool CheckMinLength => Password.Length >= MinLength;
    public bool CheckMasterLetters => MasterLetters.Any(Password.Contains);
    public bool? CheckSecondLetters => SecondLetters?.Any(Password.Contains);
    public bool? CheckDigits => Digits?.Any(Password.Contains);
    public bool? CheckSymbols => Symbols?.Any(Password.Contains);
}