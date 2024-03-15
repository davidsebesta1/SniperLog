namespace SniperLog.Extensions.Maui
{
    public static class InputValidatorHelper
    {
        public static bool ValidationCheck<T>(T input, Label errLabel, Dictionary<string, bool> dict, string name, Predicate<T> check)
        {
            bool res = check(input);

            errLabel.IsVisible = !res;
            dict[name] = res;
            return res;
        }
    }
}