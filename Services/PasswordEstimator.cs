using System.Text.RegularExpressions;

namespace TP_Entropy_back.Services
{
    public class PasswordEstimatorResult
    {
        public int Score { get; set; } // From 0 to 4
        public string Comment { get; set; }
    }
    public static class PasswordEstimator
    {
        private static readonly string[] CommonPasswords =
        {
            "password", "123456", "123456789", "12345", "12345678", "qwerty", "letmein", "welcome", "admin", "iloveyou"
        };

        private static readonly Dictionary<char, char> Substitutions = new()
        {
            { '@', 'a' }, { '4', 'a' }, { '0', 'o' }, { '1', 'l' }, { '3', 'e' }, { '$', 's' }, { '!', 'i' }, { '7', 't' }
        };

        private static readonly string[] KeyboradPattern =
        {
            "azerty", "azeryuiop", "qsdfghjklm", "wxcvbn"
        };

        public static PasswordEstimatorResult Evaluate(string password)
        {
            if (string.IsNullOrEmpty(password))
                return new PasswordEstimatorResult { Score = 0, Comment = "Password is empty" };

            int score = 0;

            //Length check
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            //Character variety check
            if (Regex.IsMatch(password, "[a-z]")) score++;
            if (Regex.IsMatch(password, "[A-Z]")) score++;
            if (Regex.IsMatch(password, "[0-9]")) score++;
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) score++;

            //Common password check
            if (CommonPasswords.Contains(password.ToLower()))
                return new PasswordEstimatorResult { Score = 0, Comment = "Password is too common" };

            //Check substitutions
            if (CommonPasswords.Contains(NormalizeSubstitutions(password.ToLower())))
                return new PasswordEstimatorResult { Score = 0, Comment = "Password is too common with substitutions" };

            //Check repetitive patterns
            if (Regex.IsMatch(password, @"^(.)\1+$"))
                return new PasswordEstimatorResult { Score = 0, Comment = "Password has repetitive patterns" };

            //Check sequential characters
            if (IsSequential(password.ToLower()))
                return new PasswordEstimatorResult { Score = 0, Comment = "Password has sequential characters" };

            //Check keyboard patterns
            if (KeyboradPattern.Any(p => password.ToLower().Contains(p)))
                return new PasswordEstimatorResult { Score = 0, Comment = "Password is based on a keyboard patterns" };

            //Final score adjustment
            score = Math.Min(score, 4);

            string comment = score switch
            {
                0 => "Very Weak",
                1 => "Weak",
                2 => "Moderate",
                3 => "Strong",
                4 => "Very Strong",
                _ => "Unknown"
            };

            return new PasswordEstimatorResult { Score = score, Comment = comment };
        }

        private static string NormalizeSubstitutions(string input)
        {
            var chars = input.Select(c => Substitutions.ContainsKey(c) ? Substitutions[c] : c).ToArray();
            return new string(chars);
        }

        private static bool IsSequential(string input)
        {
            string sequences = "abcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < input.Length - 2; i++)
            {
                string chunk = input.Substring(i, 3);
                if (sequences.Contains(chunk))
                    return true;
            }
            return false;
        }
    }
}
