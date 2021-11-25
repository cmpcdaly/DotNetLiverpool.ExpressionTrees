using System.Linq.Expressions;
using System.Text;

#pragma warning disable CS8602

namespace DotNetLiverpool.ExpressionTrees.Parsing
{
    internal class ComplexExpressionParsing
    {
        public static void PrintSimpleBinaryExpressionTree()
        {
            // For this, we need a Binary expression
            // Each node in a binary expression tree contains zero, one or two operands
            // The binary expression we're going to traverse is "5 + 7"
            var addExpression = Expression.Add(Expression.Constant(7), Expression.Constant(10));

            // The visit will 'visit' the root node first (the 'AndAlso' Expression)
            // From there, it will visit the children of this root node along with printing something out for the node itself
            var visitor = new SimpleInOrderVisitor();
            visitor.Visit(addExpression);

            Console.WriteLine(visitor.Result);
        }

        public static void PrintComplexBinaryExpressionTree()
        {
            // Let's create a more complex binary expression than the one we created above
            // We're first going to create an addition (7 + 10)
            var addExpression = Expression.Add(Expression.Constant(7), Expression.Constant(10));

            // Then we're going to create a subtraction (5 + 2)
            var minusExpression = Expression.Subtract(Expression.Constant(5), Expression.Constant(2));

            // And we're going to multiple the results of those two expression together (7 + 10) x (5 + 2)
            var multiplicationExpression = Expression.Multiply(addExpression, minusExpression);

            var visitor = new ComplexInOrderVisitor();
            visitor.Visit(multiplicationExpression);

            Console.WriteLine(visitor.Result);
        }
    }

    public class SimpleInOrderVisitor : ExpressionVisitor
    {
        public StringBuilder Result { get; } = new StringBuilder();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Result.Append('(');

            // First visit the left expression
            Visit(node.Left);

            // Then print the expression itself (Surrounded by spaces)
            Result.Append(' ');

            if (node.NodeType == ExpressionType.Add)
            {
                Result.Append('+');
            }
            else
            {
                throw new NotImplementedException();
            }

            Result.Append(' ');

            // Then visit the right expression
            Visit(node.Right);

            Result.Append(')');

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Result.Append(node.Value);
            return base.VisitConstant(node);
        }
    }

    public class ComplexInOrderVisitor : ExpressionVisitor
    {
        public StringBuilder Result { get; } = new StringBuilder();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Result.Append('(');

            // First visit the left expression
            Visit(node.Left);

            // Then print the expression itself (Surrounded by spaces)
            Result.Append(' ');

            if (node.NodeType == ExpressionType.Add)
            {
                Result.Append('+');
            }
            else if (node.NodeType == ExpressionType.Subtract)
            {
                Result.Append('-');
            }
            else if (node.NodeType == ExpressionType.Multiply)
            {
                Result.Append('x');
            }
            else
            {
                throw new NotImplementedException();
            }

            Result.Append(' ');

            // Then visit the right expression
            Visit(node.Right);

            Result.Append(')');

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Result.Append(node.Value);
            return base.VisitConstant(node);
        }
    }
}
