using System.Linq.Expressions;
using DotNetLiverpool.ExpressionTrees.Building;
// ReSharper disable MergeIntoPattern

namespace DotNetLiverpool.ExpressionTrees.Compiling
{
    internal class CompileSimple
    {
        /// <see cref="BuildExpressionTrees.CreateParameterised" />
        public static void CompileGreaterThanFiveAndLessThanTen()
        {
            // First, we have to build the expression tree
            var param = Expression.Parameter(typeof(int), "i");

            var five = Expression.Constant(5);
            var greaterThanFive = Expression.GreaterThan(param, five);

            var ten = Expression.Constant(10);
            var lessThanTen = Expression.LessThan(param, ten);

            var greaterThanFiveAndLessThanTen = Expression.AndAlso(greaterThanFive, lessThanTen);

            var expressionLambda = Expression.Lambda<Func<int, bool>>(greaterThanFiveAndLessThanTen, param);

            // Then, we can compile the function to emit IL!
            // Since we specified the TDelegate type when creating the Expression.Lambda, we know what type this delegate will be!
            // It will be a Func<int, bool>
            // i.e., it will have a single 'int' type parameter and return a 'bool'
            var function = expressionLambda.Compile();

            // Let's try it out
            var isSevenWithinRange = function(7);
            Console.WriteLine($"isSevenWithinRange = {isSevenWithinRange}");

            var isElevenWithinRange = function(11);
            Console.WriteLine($"isElevenWithinRange = {isElevenWithinRange}");

            // What we're doing is essentially the same (for the purposes of this tutorial) as writing this code at compile time, like this:
            var isSevenWithinRangeCompileTime = GreaterThanFiveAndLessThanTen(7);
            Console.WriteLine($"isSevenWithinRangeCompileTime = {isSevenWithinRangeCompileTime}");
        }

        private static bool GreaterThanFiveAndLessThanTen(int i)
        {
            return i > 5 && i < 10;
        }
    }
}
