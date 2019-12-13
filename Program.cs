using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace mpcGenerics
{
    [MessagePackObject]
    public class CustomGenericClass<T>
    {
        [Key(0)] public List<T> ListOfT;
    }

    [MessagePackObject]
    public class ClassToSerialize
    {
        [Key(0)] public CustomGenericClass<string> GenericField;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // This example will throw exception if you are using GeneratedResolver
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.BuiltinResolver.Instance
            );
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

            // But works if you use default resolvers
            // var options = MessagePackSerializerOptions.Standard;

            var classToSerialize = new ClassToSerialize()
            {
                GenericField = new CustomGenericClass<string>()
                {
                    ListOfT = new List<string>()
                    {
                        "foo", "boo", "john", "doe"
                    }
                }
            };

            var serializedValue = MessagePackSerializer.Serialize(classToSerialize, options);

            Console.WriteLine("Encoded hex: {0}", BitConverter.ToString(serializedValue));

            var deserializedValue = MessagePackSerializer.Deserialize<ClassToSerialize>(serializedValue, options);

            Console.WriteLine("Decoded list: {0}", deserializedValue.GenericField.ListOfT.Aggregate((a, b) => $"{a},{b}"));
        }
    }
}