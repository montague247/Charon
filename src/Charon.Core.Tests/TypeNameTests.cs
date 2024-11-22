namespace Charon.Core.Tests
{
    public sealed class TypeNameTests
    {
        [Fact]
        public void SimpleTypes()
        {
            Assert.Equal("string", TypeName.Instance.GetName(typeof(string)));
            Assert.Equal("string[]", TypeName.Instance.GetName(typeof(string[])));
            Assert.Equal("int", TypeName.Instance.GetName(typeof(int)));
            Assert.Equal("int?", TypeName.Instance.GetName(typeof(int?)));
            Assert.Equal("int[]", TypeName.Instance.GetName(typeof(int[])));
            Assert.Equal("int?[]", TypeName.Instance.GetName(typeof(int?[])));
            Assert.Equal("long", TypeName.Instance.GetName(typeof(long)));
            Assert.Equal("long?", TypeName.Instance.GetName(typeof(long?)));
            Assert.Equal("long[]", TypeName.Instance.GetName(typeof(long[])));
            Assert.Equal("long?[]", TypeName.Instance.GetName(typeof(long?[])));
            Assert.Equal("decimal", TypeName.Instance.GetName(typeof(decimal)));
            Assert.Equal("decimal?", TypeName.Instance.GetName(typeof(decimal?)));
            Assert.Equal("decimal[]", TypeName.Instance.GetName(typeof(decimal[])));
            Assert.Equal("decimal?[]", TypeName.Instance.GetName(typeof(decimal?[])));
            Assert.Equal("bool", TypeName.Instance.GetName(typeof(bool)));
            Assert.Equal("bool?", TypeName.Instance.GetName(typeof(bool?)));
            Assert.Equal("bool[]", TypeName.Instance.GetName(typeof(bool[])));
            Assert.Equal("bool?[]", TypeName.Instance.GetName(typeof(bool?[])));
            Assert.Equal("byte", TypeName.Instance.GetName(typeof(byte)));
            Assert.Equal("byte?", TypeName.Instance.GetName(typeof(byte?)));
            Assert.Equal("byte[]", TypeName.Instance.GetName(typeof(byte[])));
            Assert.Equal("byte?[]", TypeName.Instance.GetName(typeof(byte?[])));

            // let's test the cache
            Assert.Equal("string", TypeName.Instance.GetName(typeof(string)));
            Assert.Equal("string[]", TypeName.Instance.GetName(typeof(string[])));
            Assert.Equal("int", TypeName.Instance.GetName(typeof(int)));
            Assert.Equal("int?", TypeName.Instance.GetName(typeof(int?)));
            Assert.Equal("int[]", TypeName.Instance.GetName(typeof(int[])));
            Assert.Equal("long", TypeName.Instance.GetName(typeof(long)));
            Assert.Equal("long?", TypeName.Instance.GetName(typeof(long?)));
            Assert.Equal("long[]", TypeName.Instance.GetName(typeof(long[])));
            Assert.Equal("decimal", TypeName.Instance.GetName(typeof(decimal)));
            Assert.Equal("decimal?", TypeName.Instance.GetName(typeof(decimal?)));
            Assert.Equal("decimal[]", TypeName.Instance.GetName(typeof(decimal[])));
            Assert.Equal("bool", TypeName.Instance.GetName(typeof(bool)));
            Assert.Equal("bool?", TypeName.Instance.GetName(typeof(bool?)));
            Assert.Equal("bool[]", TypeName.Instance.GetName(typeof(bool[])));
        }

        [Fact]
        public void IsSimple()
        {
            Assert.True(TypeName.IsSimple<string>());
            Assert.True(TypeName.IsSimple<string[]>());
            Assert.True(TypeName.IsSimple<int>());
            Assert.True(TypeName.IsSimple<int?>());
            Assert.True(TypeName.IsSimple<int[]>());
            Assert.True(TypeName.IsSimple<int?[]>());
            Assert.True(TypeName.IsSimple<long>());
            Assert.True(TypeName.IsSimple<long?>());
            Assert.True(TypeName.IsSimple<long[]>());
            Assert.True(TypeName.IsSimple<long?[]>());
            Assert.True(TypeName.IsSimple<decimal>());
            Assert.True(TypeName.IsSimple<decimal?>());
            Assert.True(TypeName.IsSimple<decimal[]>());
            Assert.True(TypeName.IsSimple<decimal?[]>());
            Assert.True(TypeName.IsSimple<bool>());
            Assert.True(TypeName.IsSimple<bool?>());
            Assert.True(TypeName.IsSimple<bool[]>());
            Assert.True(TypeName.IsSimple<bool?[]>());
            Assert.False(TypeName.IsSimple<TypeNameTests>());
            Assert.False(TypeName.IsSimple<TypeNameTests?>());
            Assert.False(TypeName.IsSimple<TypeNameTests[]>());
            Assert.False(TypeName.IsSimple<TypeNameTests?[]>());
        }

        [Fact]
        public void InitEmpty()
        {
            var actual = new TypeName();

            Assert.NotNull(actual);
        }

        [Fact]
        public void InitMultiple()
        {
            var namespaces = new TypeName("System.Linq", "System", typeof(TypeName).Namespace!, typeof(int).Namespace!).Namespaces.ToArray();

            Assert.NotNull(namespaces);
            Assert.Equal(3, namespaces.Length);
            Assert.Equal("Charon", namespaces[0]);
            Assert.Equal("System", namespaces[1]);
            Assert.Equal("System.Linq", namespaces[2]);
        }

        [Fact]
        public void EnumGetNameNull()
        {
            ConsoleColor? e = null;
            var actual = new TypeName("System").GetName(e);
            Assert.Null(actual);
        }

        [Fact]
        public void GenericTypeGetName()
        {
            var actual = new TypeName("System").GetName<AppDomain>();
            Assert.Equal("AppDomain", actual);
        }

        [Fact]
        public void TypeGetNameNull()
        {
            Type? t = null;
            var actual = new TypeName("System").GetName(t);
            Assert.Null(actual);
        }

        [Fact]
        public void TypeGetNameNullable()
        {
            var actual = new TypeName().GetName(typeof(char?));
            Assert.Equal("System.Char?", actual);
        }

        [Fact]
        public void TypeGetNameGenericListSimple()
        {
            var actual = new TypeName().GetName<List<int>>();
            Assert.Equal("System.Collections.Generic.List<int>", actual);
        }

        [Fact]
        public void TypeGetNameGenericList()
        {
            var actual = new TypeName().GetName<List<Types.AppDomain>>();
            Assert.Equal("System.Collections.Generic.List<Charon.Core.Tests.Types.AppDomain>", actual);
        }

        [Fact]
        public void TypeGetNameGenericList2()
        {
            var actual = new TypeName("System.Collections.Generic").GetName<Types.List<Types.AppDomain>>();
            Assert.Equal("Charon.Core.Tests.Types.List<Charon.Core.Tests.Types.AppDomain>", actual);
        }

        [Fact]
        public void TypeGetNameGenericList3()
        {
            var actual = new TypeName(typeof(Types.List<Types.AppDomain>).Namespace!).GetName<Types.List<Types.AppDomain>>();
            Assert.Equal("List<AppDomain>", actual);
        }

        [Fact]
        public void TypeGetNameGenericList4()
        {
            var actual = new TypeName("System.Collections.Generic", typeof(Types.List<Types.AppDomain>).Namespace!).GetName<Types.List<Types.AppDomain>>();
            Assert.Equal("Charon.Core.Tests.Types.List<AppDomain>", actual);
        }

        [Fact]
        public void TypeGetNameGenericDictionary()
        {
            var actual = new TypeName().GetName<Dictionary<int, Types.AppDomain>>();
            Assert.Equal("System.Collections.Generic.Dictionary<int, Charon.Core.Tests.Types.AppDomain>", actual);
        }

        [Fact]
        public void TypeGetNameGenericDictionaryWithNamespace()
        {
            var actual = new TypeName("System.Collections.Generic", typeof(Types.AppDomain).Namespace!).GetName<Dictionary<int, Types.AppDomain>>();
            Assert.Equal("Dictionary<int, AppDomain>", actual);
        }

        [Fact]
        public void TypeGetNameDuplicateInOtherNamespace()
        {
            var tn = new TypeName("System");
            var actual = tn.GetName(typeof(Types.AppDomain));
            Assert.Equal("Charon.Core.Tests.Types.AppDomain", actual);

            actual = tn.GetName(typeof(AppDomain));
            Assert.Equal("AppDomain", actual);
        }

        [Fact]
        public void TypeGetNameDuplicateInMultipleNamespaces()
        {
            var tn = new TypeName("System", typeof(Types.AppDomain).Namespace!);
            var actual = tn.GetName(typeof(Types.AppDomain));
            Assert.Equal("Charon.Core.Tests.Types.AppDomain", actual);

            actual = tn.GetName(typeof(AppDomain));
            Assert.Equal("System.AppDomain", actual);
        }
    }
}
