using System;
using System.Collections.Generic;

namespace Uaeglp.Utilities
{
    public static class AssessmentCalculations
    {
        public static int GetPercentile(List<Decimal> Scores, Decimal RequiredNumber)
        {
            int num1 = 0;
            int num2 = 0;
            int count = Scores.Count;
            foreach (Decimal score in Scores)
            {
                if (score < RequiredNumber)
                    ++num1;
                if (score == RequiredNumber)
                    ++num2;
            }
            return int.Parse(Math.Floor(((double)num1 + 0.5 * (double)num2) / (double)count * 100.0).ToString());
        }

        public static Decimal PercentRank(List<Decimal> matrix, Decimal value)
        {
            matrix.Sort();
            for (int index = 0; index < matrix.Count; ++index)
            {
                if (matrix[index] == value)
                    return (Decimal)index / (Decimal)(matrix.Count - 1);
            }
            for (int index = 0; index < matrix.Count - 1; ++index)
            {
                if (matrix[index] < value && value < matrix[index + 1])
                {
                    Decimal num1 = matrix[index];
                    Decimal num2 = matrix[index + 1];
                    Decimal num3 = AssessmentCalculations.PercentRank(matrix, num1);
                    Decimal num4 = AssessmentCalculations.PercentRank(matrix, num2);
                    return ((num2 - value) * num3 + (value - num1) * num4) / (num2 - num1);
                }
            }
            throw new Exception("Out of bounds");
        }
    }
}
