using System.Linq.Expressions;
using System.Text;

#pragma warning disable CS8602

namespace DotNetLiverpool.ExpressionTrees.Parsing
{
    internal class ExplicitParsing
    {
        public static void ParseSimple()
        {
            // Let's build a simple lambda expression
            Expression expression = Expression.Add(Expression.Constant(7), Expression.Constant(10));

            // From this, we want to print a representation of this in code form, which would look something like this:
            // '7 + 10'

            // How do we write code to do this? Given that all nodes on the tree are of type 'Expression',
            // we don't have immediate access to members of the sub-types, e.g.:
            //  The 'Left' and 'Right' properties of the Add expression
            //  The 'Value' of the Constant expressions

            // To parse this explicitly, we first have to cast the type of the root expression
            var addExpression = (BinaryExpression) expression;

            // Now we can look at the operands, left and right
            // We know they are Constant expressions, so lets cast them
            var left = (ConstantExpression) addExpression.Left;
            var right = (ConstantExpression) addExpression.Right;

            // So lets build a result! The expression should look like:
            // (left operand) (operator) (right operand)
            var result = new StringBuilder();
            
            // First append the left operand
            result.Append(left.Value);
            
            // Now since we know it's a binary expression, let's add the operator in the middle
            result.Append($" {addExpression.NodeType} ");

            // And finally, add the right operand
            result.Append(right.Value);

            Console.WriteLine(result);
        }
    }
}
