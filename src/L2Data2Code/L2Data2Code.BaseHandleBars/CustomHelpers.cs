using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Attributes;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers.Helpers;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace L2Data2Code.BaseHandleBars
{
    internal class CustomHelpers : BaseHelpers, IHelpers
    {
        private readonly IDictionary<string, object> values;

        public CustomHelpers(IHandlebars context, IDictionary<string, object> values) : base(context)
        {
            this.values = values;
        }

        [HandlebarsWriter(WriterType.String)]
        public static string Join(IEnumerable values, string separator = null)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            return string.Join(separator ?? string.Empty, values.Cast<object>());
        }


        [HandlebarsWriter(WriterType.String)]
        public static string JoinWithHeader(IEnumerable values, string separator, string header)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            return string.Concat(header, string.Join(separator ?? string.Empty, values.Cast<object>()));
        }

        [HandlebarsWriter(WriterType.String)]
        public static string JoinWithHeaderFooter(IEnumerable values, string separator, string header, string footer)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            return string.Concat(header, string.Join(separator ?? string.Empty, values.Cast<object>()), footer);
        }

        [HandlebarsWriter(WriterType.Value)]
        public object GetVar(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return values.TryGetValue(key, out var value) ? value : null;
        }

        [HandlebarsWriter(WriterType.String)]
        public static string FormatCurrency(object textNumber)
        {
            return decimal.TryParse(textNumber.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var number) ? number.ToString("C") : string.Empty;
        }

        [HandlebarsWriter(WriterType.Value)]
        public static bool Or(string left, string right)
        {
            return left.IsTrue() || right.IsTrue();
        }

        [HandlebarsWriter(WriterType.Value)]
        public static bool And(string left, string right)
        {
            return left.IsTrue() && right.IsTrue();
        }

        [HandlebarsWriter(WriterType.Value)]
        public static bool Equals(string left, string right)
        {
            return left == right;
        }

        [HandlebarsWriter(WriterType.String)]
        public string IncreaseVersion(int increment = 0, string versionValue = null)
        {
            increment = increment == 0 ? 1 : increment;
            if (increment > 100 || increment < -100)
            {
                throw new ArgumentException("Increment must be between -100 and 100");
            }
            versionValue ??= (values.TryGetValue("Version", out var value) && value is string version ? version : string.Empty);
            var result = versionValue;

            var parts = versionValue.Split('.');

            if (parts.Length >= 3)
            {
                int[] numbers = [int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])];

                numbers[2] += increment;
                CheckLessThan0(numbers);
                CheckGreaterThan99(numbers);

                parts[2] = (int.Parse(parts[2]) + increment).ToString();
                result = string.Join(".", numbers.Select(n => n.ToString()));
            }

            return result;
        }

        private static void CheckLessThan0(int[] numbers)
        {
            if (numbers[2] < 0)
            {
                numbers[2] += 100;
                numbers[1] -= 1;
            }

            if (numbers[1] < 0)
            {
                numbers[1] += 100;
                numbers[0] -= 1;
            }
        }

        private static void CheckGreaterThan99(int[] numbers)
        {
            if (numbers[2] > 99)
            {
                numbers[2] -= 100;
                numbers[1] += 1;
            }

            if (numbers[1] > 99)
            {
                numbers[1] -= 100;
                numbers[0] += 1;
            }
        }

        public Category Category => (Category)999;
    }
}
