using System.Linq.Expressions;

#pragma warning disable CS8321
#pragma warning disable CS8604
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8618
namespace DotNetLiverpool.ExpressionTrees.Compiling
{
    internal class CompiledPropertyAccessors
    {
        internal static void Reflection()
        {
            // Member access at compile time is easy, it's as simple as defining an instance
            var instance = new SomeEntity
            {
                SomeValue = "Test"
            };

            // And accessing the member by name
            var directResult = instance.SomeValue;

            Console.WriteLine($"directResult: {directResult}");

            // Relative to direct member access (at compile time), using reflection to access instance members is slow!
            // We have to first look-up the PropertyInfo
            var propertyInfo = typeof(SomeEntity).GetProperty(nameof(SomeEntity.SomeValue));

            // And then invoke the GetValue method dynamically
            // A lot of things have to happen here: https://github.com/microsoft/referencesource/blob/master/mscorlib/system/reflection/propertyinfo.cs#L102
            // Which will eventually call MethodInfo.Invoke
            var methodInfoInvokeResult = (string)propertyInfo.GetValue(instance);

            Console.WriteLine($"methodInfoInvokeResult: {methodInfoInvokeResult}");

            // There are some performance improvements we can make here without expression trees.
            // The first (and simplest) is that we can cache the call to 'PropertyInfo' against a key of the type and the property we're accessing
            var delegateMethod =
                (Func<SomeEntity, string>)Delegate.CreateDelegate(typeof(Func<SomeEntity, string>),
                    propertyInfo.GetGetMethod());

            // Wew can now invoke this delegate directly, which provides performance close to the direct compile-time accessor
            var resultFromDelegate = delegateMethod.Invoke(instance);
            Console.WriteLine($"resultFromDelegate: {resultFromDelegate}");

            // There are some limitations with this approach though!
            // To create this delegate, we need to know the type of the delegate at compile-time (the parameter passed to Delegate.CreateDelegate)
            // For simple problems, this might not be an issue at all...
            // But what if we wanted to build a collection of Func<object, object> capable of retrieving members for different type?
            // We can't do this...
            try
            {
                var polymorphicDelegateMethod =
                    (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>),
                        propertyInfo.GetGetMethod());
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"polymorphicDelegateMethod failed: {e.Message}");
            }

            // There is a really clever way to achieve this (funnily enough, using reflection!) which you can read about here:
            // https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
            // But for the purposes of this tutorial, this is where Expression Trees come in B)
        }

        public static void CompiledExpressionTrees()
        {
            var instance = new SomeEntity
            {
                SomeValue = "Test"
            };

            // What we're essentially going to build is a function that looks like this:
            // String GetSomeValue(SomeEntity e) => e.SomeValue

            // It will allow us to pass any instance of SomeEntity and return the result of the SomeValue property

            // The first thing that we need to do is define a parameter, of type SomeEntity
            var param = Expression.Parameter(typeof(SomeEntity), "e");

            // Then, we need to create an Expression which represents a "Method Call"
            // The method, in this case, being the 'Get' method of the 'SomeEntity.SomeValue' property

            // The Expression.Call factory method has a few overloads
            // The one we're interested in takes an 'Expression' (instance) parameter, and a 'MethodInfo' (method) parameter
            // We already have our instance, which is the parameter defined above - now we just have to get the MethodInfo
            var methodInfo = typeof(SomeEntity).GetProperty(nameof(SomeEntity.SomeValue)).GetGetMethod();

            var call = Expression.Call(param, methodInfo);

            // Next, like we did previously in the CompiledSimple example, we just have to create the delegate to bring it all together
            var someValueExpression = Expression.Lambda<Func<SomeEntity, string>>(call, param);

            // And finally, we can compile this method into executable code!
            var getSomeValue = someValueExpression.Compile();

            var someValueFromCompiledExpression = getSomeValue(instance);
            Console.WriteLine("someValueFromCompiledExpression: " + someValueFromCompiledExpression);

            // Let's take a step back though, what have we done here?
            // So the method call itself is going to be faster* than the reflection based propertyInfo.GetValue approach
            //      (*I'm a scientific man, so take a look at the attached benchmarking project for some numbers)
            // But what about the two functions, start to finish, what have we gained?

            // Accessing the PropertyInfo and building the expression tree don't come free.
            // And worst of all, compiling the expression tree is going to carry much of the same overhead as the PropertyInfo.GetValue approach
            // So what have we gained here, except maybe complexity?

            // The value in this approach is that we start to see performance improvements when these compiled expressions are executed multiple times
            // Infrequent access for a short-lived process is probably not a good use case
            // For example, if your application runs on some serverless architecture that is transient in nature
            // (i.e. it gets invoked once and then likely disappears)
            // Then using compiled expressions for reflection is probably not a good idea
        }

        public static void PolymorphicExpressionTrees()
        {
            // So, why is this approach any better than Delegate.CreateDelegate?
            // There is actually an Expression we can use to cast another Expression (e.g. a parameter)
            // So we can make a polymorphic Func<object, object> which could work for both properties in the SomeEntity class

            // What we're essentially looking to build is something like this at run-time
            object GetValue(object o)
            {
                return ((SomeEntity)o).SomeValue;
            }

            // So that we can have another implementation with the same signature, that reads
            object GetAnotherValue(object o)
            {
                return ((AnotherEntity)o).AnotherValue;
            }

            // Let's take a look at how we could do this
            // The implementation here is going to look like the expression we compiled above but with two differences:
            //  Firstly, the parameter type of the Func is going to be object
            //  Secondly, since the parameter type is of object, we need to cast it back to the SomeEntity type

            // This is the parameter that we'll accept
            var param = Expression.Parameter(typeof(object), "e");
            // And this is the parameter converted to a SomeEntity type
            var convertedParam = Expression.Convert(param, typeof(SomeEntity));
            var methodInfo = typeof(SomeEntity).GetProperty(nameof(SomeEntity.SomeValue)).GetGetMethod();

            var call = Expression.Call(convertedParam, methodInfo);

            // Finally, we can define our Func as <object, object> instead and compile it
            var someValueExpression = Expression.Lambda<Func<object, object>>(call, param);

            var getSomeValue = someValueExpression.Compile();

            // Let's do the same for a completely different type (AnotherEntity) and property (AnotherValue)
            var param2 = Expression.Parameter(typeof(object), "e");
            var convertedParam2 = Expression.Convert(param, typeof(AnotherEntity));
            var methodInfo2 = typeof(AnotherEntity).GetProperty(nameof(AnotherEntity.AnotherValue)).GetGetMethod();
            var call2 = Expression.Call(convertedParam, methodInfo);
            var anotherValueExpression = Expression.Lambda<Func<object, object>>(call, param);

            var getAnotherValue = anotherValueExpression.Compile();

            // We've now got two Func<object, object> which we could store in a lookup somewhere
            // With a key potentially made up of the type name and member name
            // All we have to do is provide it a correct instance type and the Func will give us the result!

            // If performance is important here, be aware that value types returned by this expression will be boxed
        }
    }
}

internal class SomeEntity
{
    public string SomeValue { get; set; }
}

internal class AnotherEntity
{
    public int AnotherValue { get; set; }
}
