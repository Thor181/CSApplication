﻿using CSLibrary.Stuff.Results;
using System.Linq.Expressions;

namespace CSLibrary.Stuff
{
    public class Validation
    {
        public static BaseResult StringsIsNullOrEmpty<T>(T instance, params Expression<Func<T, string>>[] expressions)
        {
            var result = new BaseResult();

            foreach (var expression in expressions)
            {
                var memberName = ((MemberExpression)expression.Body).Member.Name;
                var value = expression.Compile().Invoke(instance);
                if (string.IsNullOrWhiteSpace(value))
                {
                    result.IsSuccess = false;
                    result.MessageBuilder.AppendLine($"Значение поля {memberName} было пустым");
                }
            }

            return result;
        }

        public static BaseResult AllStringsEquals(int length, IEnumerable<string> strings)
        {
            var result = new BaseResult();

            foreach (var str in strings)
            {
                if (str.Length != length)
                {
                    result.IsSuccess = false;
                    result.MessageBuilder.AppendLine($"Длина строки \"{str}\" не равна {length}");
                }
            }

            return result;
        }

        public static bool StringsCountMoreThanOne(params string[] strings)
        {
            var oldCount = strings.Length;
            var newCount = strings.Distinct().Count();

            return oldCount != newCount;
        }
    }
}
