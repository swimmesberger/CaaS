using System.Text.RegularExpressions;

namespace SwKo.Pay; 

public static class CreditCardValidityChecker {

    private static readonly Dictionary<string, string> RegexCardDictionary = new Dictionary<string, string>() {
        { "Master Card", "(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}" },
        { "Visa", "^4[0-9]{6,}$" },
        { "Diner's Club", "(^30[0-5][0-9]{11}$)|(^(36|38)[0-9]{12}$)" },
        { "American Express", "^[34|37][0-9]{14}$" },
        { "JCB", "(^3[0-9]{15}$)|(^(2131|1800)[0-9]{11}$)"},
        { "Discover", "^6011-?\\d{4}-?\\d{4}-?\\d{4}$"}
    };

    public static bool IsValid(string creditCardNumber) {
        return RegexCardDictionary.Any(entry => Regex.IsMatch(creditCardNumber, entry.Value));
    }
}