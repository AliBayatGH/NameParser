using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NameParser
{
    public static class StringExtension
    {
        /// <summary>
        /// Split full names into the following parts:
        /// - prefix / salutation  (Mr., Mrs., etc)
        /// - given name / first name
        /// - middle initials
        /// - surname / last name 
        /// - suffix (II, Phd, Jr, etc)
        /// </summary>
        /// <param name="fullName">Full name</param>
        /// <returns></returns>
        public static Name Parse(this string fullName)
        {
            fullName = fullName.Trim();

            var lastName = "";
            var firstName = "";
            var initials = "";
            List<string> nameParts;
            int numWords;
            SplitIntoWords(fullName, out nameParts, out numWords);

            // is the first word a title? (Mr. Mrs, etc)
            var salutation = IsSalutation(nameParts[0]);
            var suffix = IsSuffix(nameParts[numWords - 1]);
            // set the range for the middle part of the name (trim prefixes & suffixes)
            var start = !string.IsNullOrEmpty(salutation) ? 1 : 0;
            var end = !string.IsNullOrEmpty(suffix) ? numWords - 1 : numWords;

            var word = nameParts[start];
            // if we start off with an initial, we'll call it the first name
            if (IsInitial(word))
            {
                // if so, do a look-ahead to see if they go by their middle name 
                // for ex: "R. Jason Smith" => "Jason Smith" & "R." is stored as an initial
                // but "R. J. Smith" => "R. Smith" and "J." is stored as an initial
                if (IsInitial(nameParts[start + 1]))
                {
                    firstName += " " + word.ToUpper();
                }
                else
                {
                    initials += " " + word.ToUpper();
                }
            }
            else
            {
                firstName += " " + FixCase(word);
            }

            int i;
            // concat the first name
            for (i = start + 1; i < (end - 1); i++)
            {
                word = nameParts[i];
                // move on to parsing the last name if we find an indicator of a compound last name (Von, Van, etc)
                // we do not check earlier to allow for rare cases where an indicator is actually the first name (like "Von Fabella")
                if (IsCompoundLastName(word)) break;

                if (IsInitial(word))
                {
                    initials += " " + word.ToUpper();
                }
                else
                {
                    firstName += " " + FixCase(word);
                }
            }

            // check that we have more than 1 word in our string
            if ((end - start) > 1)
            {
                // concat the last name
                var j = 0;
                for (j = i; j < end; j++)
                {
                    lastName += " " + FixCase(nameParts[j]);
                }
            }

            // return the various parts in an array
            return new Name()
            {
                Salutation = salutation ?? "",
                FirstName = firstName.Trim(),
                MiddleInitials = initials.Trim(),
                LastName = lastName.Trim(),
                Suffix = suffix ?? ""
            };
        }

        /// <summary>
        /// Split into words
        /// Completely ignore any words in parentheses
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="nameParts"></param>
        /// <param name="numWords"></param>
        private static void SplitIntoWords(string fullName, out List<string> nameParts, out int numWords)
        {
            nameParts = fullName.Split(' ').Where(w => !w.StartsWith("(")).ToList();
            numWords = nameParts.Count();
        }

        /// <summary>
        /// Remove ignored chars
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string RemoveIgnoredChars(string word)
        {
            //ignore periods
            return word.Replace(".", "");
        }

        /// <summary>
        /// Detect and format standard salutations 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string IsSalutation(string word)
        {
            word = RemoveIgnoredChars(word).ToLower();
            return Normalize(word);
        }

        /// <summary>
        /// returns normalized word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string Normalize(string word)
        {
            switch (word)
            {
                case "mister":
                case "master":
                case "mr":
                    return "Mr.";
                case "mrs":
                    return "Mrs.";
                case "ms":
                case "miss":
                    return "Ms.";
                case "dr":
                    return "Dr.";
                case "rev":
                    return "Rev.";
                case "fr":
                    return "Fr.";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Detect and format common suffixes 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string IsSuffix(string word)
        {
            word = RemoveIgnoredChars(word).ToLower();
            var suffixArray = new[]
            {
                "I", "II", "III", "IV", "V", "Senior", "Junior", "Jr", "Sr", "PhD", "APR", "RPh", "PE", "MD", "MA", "DMD", "CME",
                "BVM", "CFRE", "CLU", "CPA", "CSC", "CSJ", "DC", "DD", "DDS", "DO", "DVM", "EdD", "Esq",
                "JD", "LLD", "OD", "OSB", "PC", "Ret", "RGS", "RN", "RNC", "SHCJ", "SJ", "SNJM", "SSMO",
                "USA", "USAF", "USAFR", "USAR", "USCG", "USMC", "USMCR", "USN", "USNR"
            };

            var suffix = suffixArray.FirstOrDefault(w => w.ToLower() == word);

            return suffix ?? "";
        }

        /// <summary>
        /// Detect compound last names like "Bayat Gholilaleh"
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static bool IsCompoundLastName(string word)
        {
            word = word.ToLower();

            // These are some common prefixes that identify a compound last names - what am I missing?
            var words = new[] { "vere", "von", "van", "de", "del", "della", "di", "da", "pietro", "vanden", "du", "st.", "st", "la", "lo", "ter" };

            return !string.IsNullOrEmpty(words.FirstOrDefault(w => w == word));
        }

        /// <summary>
        /// Single letter, possibly followed by a period
        /// </summary>
        /// <param name="word"></param>
        /// <returns>true if the value parameter length is 1; otherwise, false.</returns>
        private static bool IsInitial(string word)
        {
            word = RemoveIgnoredChars(word);

            return (word.Length == 1);
        }

        /// <summary>
        /// Detect mixed case words like "AliBayat"
        /// </summary>
        /// <param name="word"></param>
        /// <returns>false if the string is all one case</returns>
        private static bool IsCamelCase(string word)
        {
            var ucReg = new Regex("[A-Z]+");
            var lcReg = new Regex("[a-z]+");
            return (ucReg.IsMatch(word) && lcReg.IsMatch(word));
        }

        /// <summary>
        /// ucfirst words split by dashes or periods
        /// ucfirst all upper/lower strings, but leave camelcase words alone
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string FixCase(string word)
        {
            // uppercase words split by dashes, like "Ali-Bayat"
            word = SafeUcFirst('-', word);

            // uppercase words split by periods, like "A.B."
            word = SafeUcFirst('.', word);

            return word;
        }

        /// <summary>
        /// Helper for this.FixCase
        /// Uppercase words split by the seperator (ex. dashes or periods)
        /// </summary>
        /// <param name="seperator"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string SafeUcFirst(char seperator, string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return word;

            var words = word.Split(seperator);
            var newWord = new StringBuilder();
            foreach (var thisWord in words)
            {
                if (newWord.Length > 0)
                    newWord.Append(seperator);

                if (IsCamelCase(thisWord))
                {
                    newWord.Append(thisWord);
                }
                else
                {
                    newWord.Append(thisWord.Substring(0, 1).ToUpper() + thisWord.Substring(1).ToLower());
                }
            }

            return newWord.ToString();
        }
    }
}
