using Bogus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            string str = RemoveDiacritics(phrase);

            str = str.ToLowerInvariant();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Replace(" ", "-"); 

            str = Regex.Replace(str, @"-+", "-");

            return str;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static string GenerateVietnamPhoneNumber(Faker f)
        {
            // Chọn một trong các đầu số di động tại Việt Nam
            var prefixes = new[] { "091", "092", "093", "094", "095", "096", "097", "098", "099" };
            var prefix = f.PickRandom(prefixes);

            // Tạo 7 ký tự ngẫu nhiên còn lại
            var number = f.Random.Number(1000000, 9999999).ToString();

            return $"{prefix}{number}";
        }
    }
}
