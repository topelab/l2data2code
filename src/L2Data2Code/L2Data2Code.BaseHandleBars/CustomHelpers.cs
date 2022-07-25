using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Attributes;
using HandlebarsDotNet.Helpers.Enums;
using HandlebarsDotNet.Helpers.Helpers;
using L2Data2Code.SharedLib.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public string FormatCurrency(string textNumber)
        {
            return decimal.TryParse(textNumber, out var number) ? number.ToString("C") : string.Empty;
        }

        [HandlebarsWriter(WriterType.Value)]
        public bool Or(string left, string right)
        {
            return left.IsTrue() || right.IsTrue();
        }

        [HandlebarsWriter(WriterType.Value)]
        public bool And(string left, string right)
        {
            return left.IsTrue() && right.IsTrue();
        }

        [HandlebarsWriter(WriterType.Value)]
        public bool Equals(string left, string right)
        {
            return left == right;
        }

    }
}
