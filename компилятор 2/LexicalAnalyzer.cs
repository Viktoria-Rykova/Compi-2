using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace компилятор_2
{
    internal class LexicalAnalyzer
    {
        public List<(int Code, string Type, string Lexeme, int StartPos, int EndPos)> AnalyzeText(string input)
        {
            var results = new List<(int, string, string, int, int)>();

            string dictPattern = @"\b([A-Za-zА-Яа-я_][A-Za-zА-Яа-я0-9_]*)\s*=\s*\{([^}]*)\}";
            Match dictMatch = Regex.Match(input, dictPattern);

            if (!dictMatch.Success)
            {
                Console.WriteLine("Словарь не найден в тексте.");
                return results;
            }

            string dictName = dictMatch.Groups[1].Value;
            int dictNameStart = dictMatch.Index + 1;
            int dictNameEnd = dictNameStart + dictName.Length - 1;

            if (Regex.IsMatch(dictName, @"[А-Яа-я]"))
            {
                results.Add((-1, "Ошибка (недопустимые символы в названии словаря)", dictName, dictNameStart, dictNameEnd));
            }
            else
            {
                results.Add((2, "Идентификатор (название словаря)", dictName, dictNameStart, dictNameEnd));
            }


            int assignPos = dictNameEnd + 2;
            results.Add((10, "Оператор присваивания", "=", assignPos, assignPos));

            int openBracePos = assignPos + 2; 
            results.Add((13, "Фигурная скобка открывающая", "{", openBracePos, openBracePos));

            string dictContent = dictMatch.Groups[2].Value;
            int dictContentStart = openBracePos + 2; 

            string tokenPattern = @"\s+|[{}=:,;]|'[^']*'|""[^""]*""|[A-Za-z_][A-Za-z0-9_]*|\d+|['""]";
            Regex regex = new Regex(tokenPattern);

            foreach (Match match in regex.Matches(dictContent))
            {
                int startPos = dictContentStart + match.Index;
                int endPos = startPos + match.Length - 1;

                if (Regex.IsMatch(match.Value, @"^\s+$"))
                {
                    continue;
                }

                if (Regex.IsMatch(match.Value, @"^\d+$"))
                {
                    results.Add((1, "Целое без знака", match.Value, startPos, endPos));
                    continue;
                }

                if (match.Value == ":")
                {
                    results.Add((18, "Оператор (двоеточие)", match.Value, startPos, endPos));
                    continue;
                }

                if (",;".Contains(match.Value))
                {
                    results.Add((12, "Разделитель", match.Value, startPos, endPos));
                    continue;
                }

                if (Regex.IsMatch(match.Value, @"^'[^']*'$") || Regex.IsMatch(match.Value, @"^""[^""]*""$"))
                {
                    results.Add((3, "Строка", match.Value, startPos, endPos));
                    continue;
                }

                if (Regex.IsMatch(match.Value, @"^[A-Za-z_][A-Za-z0-9_]*$"))
                {
                    results.Add((2, "Идентификатор", match.Value, startPos, endPos));
                    continue;
                }
                results.Add((-1, "Ошибка", match.Value, startPos, endPos));
            }

            int closeBracePos = dictMatch.Index + dictMatch.Length - 1;
            results.Add((14, "Фигурная скобка закрывающая", "}", closeBracePos, closeBracePos));

            return results;
        }
    }
}
