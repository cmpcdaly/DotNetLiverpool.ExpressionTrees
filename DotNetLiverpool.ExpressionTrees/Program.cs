// See https://aka.ms/new-console-template for more information

using DotNetLiverpool.ExpressionTrees.Building;
using DotNetLiverpool.ExpressionTrees.Compiling;
using DotNetLiverpool.ExpressionTrees.Parsing;

{
    // 1. Creating Expressions

    //      1.1 Run-Time Expressions
    BuildExpressionTrees.CreateSimple();

    BuildExpressionTrees.CreateParameterised();

    //      1.2 Compile-Time Expressions
    BuildExpressionTrees.CreateCapturedLambdaExpression();

    // 2. Compiling Expressions

    //      2.1 Compiling the simple example (i > 5 && i < 10)
    CompileSimple.CompileGreaterThanFiveAndLessThanTen();

    //      2.2 Compiling Property Accessors

    //          2.2.1 Using Reflection
    CompiledPropertyAccessors.Reflection();

    //          2.2.2 Using Expression Trees
    CompiledPropertyAccessors.CompiledExpressionTrees();

    //          2.2.3 Using Polymorphic Funcs
    CompiledPropertyAccessors.PolymorphicExpressionTrees();

    // 3. Evaluating Expressions

    //      3.1 Explicit Parsing
    ExplicitParsing.ParseSimple();

    //      3.2 Visitor Parsing
    ComplexExpressionParsing.PrintSimpleBinaryExpressionTree();

    ComplexExpressionParsing.PrintComplexBinaryExpressionTree();

    Console.ReadKey();
}