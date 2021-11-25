using System.Linq.Expressions;
// ReSharper disable MergeIntoPattern

namespace DotNetLiverpool.ExpressionTrees.Building
{
    internal class BuildExpressionTrees
    {
        // 5 < 9
        internal static void CreateSimple()
        {
            // We start by building the leaf nodes, since parent nodes must know about their children
            var five = Expression.Constant(5);
            var nine = Expression.Constant(9);

            // The 'Expression.LessThan' factory method requires two parameters; the left operand and the right operand
            // These are of type 'Expression', so they could even be other complex Expression structures!
            var fiveLessThanNine = Expression.LessThan(five, nine);
        }

        // i => i > 5 && i < 10
        internal static void CreateParameterised()
        {
            // Instead of using a constant, we can define i as a Parameter
            var i = Expression.Parameter(typeof(int), "i");

            // The left side of the expression tree evaluates if i is greater than five
            var five = Expression.Constant(5);
            var greaterThanFive = Expression.GreaterThan(i, five);

            // The right side of the expression tree evaluates if i is less than ten
            var ten = Expression.Constant(10);
            var lessThanTen = Expression.LessThan(i, ten);

            // Now we have built the parameters for the 'AND' part of the expression, we construct AndAlso using the factory method
            // NOTE: Expression.And is for the bitwise AND operation, not the conditional AND!
            var greaterThanFiveAndLessThanTen = Expression.AndAlso(greaterThanFive, lessThanTen);

            // This expression is useless on it's own, and we've not even done anything with the parameter
            // let's turn it into a function
            var function = Expression.Lambda<Func<int, bool>>(greaterThanFiveAndLessThanTen, i);
        }

        internal static void CreateCapturedLambdaExpression()
        {
            // Assigning a Lambda Expression to type Expression<TDelegate> tells the compiler to build an expression tree
            Expression<Func<int, bool>> greaterThanFiveAndLessThanTen
                = i => i > 5 && i < 10;

            // There are some limitations on what we can do here though!

            // Lambdas have two forms, 'Expression Lambdas' and 'Statement Lambdas'
            //  - In an expression lambda, the 'body' (right side of the =>) can only contain a single line expression
            //  - In a statement lambda, the body is enclosed in braces { ... } like a typical function definition, and can contain multiple statements
            // Expression Trees can only be generated at compile-time from the 'Expression Lambda' types

            // For example, The following lambda can be turned into an expression:
            Func<int, bool> expressionLambda = i => i > 5 && i < 10;

            // But the following lambda cannot be turned into an expression
            Func<int, bool> statementLambda = i =>
            {
                var gtFive = i > 5;
                var ltTen = i < 10;

                return gtFive && ltTen;
            };

            // Don't worry, we can construct block statement expressions with the API!
        }
    }
}
