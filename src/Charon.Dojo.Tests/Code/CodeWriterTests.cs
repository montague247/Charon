using System.Diagnostics;
using Charon.Dojo.Code;

namespace Charon.Dojo.Tests.Code
{
    public sealed class CodeWriterTests
    {
        [Fact]
        public void IsNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new CodeWriter(s => { }).Build());
        }

        [Fact]
        public void NoClass()
        {
            var actual = new CodeWriter(s => s.Using("System").Namespace("Foo.Bar"))
                .Build();

            Assert.Equal("using System;\n\nnamespace Foo.Bar\n{\n}\n", actual);
        }

        [Fact]
        public void NoClassNoUsing()
        {
            var actual = new CodeWriter(s => s.Using("Foo.Bar").Namespace("Foo.Bar"))
                .Build();

            Assert.Equal("namespace Foo.Bar\n{\n}\n", actual);
        }

        [Fact]
        public void EmptyClass()
        {
            var actual = new CodeWriter(s => s.Namespace("Foo.Bar"))
                .Class("TestClass", s => { })
                .Build();

            Assert.Equal("namespace Foo.Bar\n{\n    private class TestClass\n    {\n    }\n}\n", actual);
        }

        [Fact]
        public void ClassWithInheritance()
        {
            var actual = new CodeWriter(s => s.Namespace(GetType().Namespace!))
                .Class("TestClass", s => s
                    .Accessibility(Accessibility.Public)
                    .Inheritance(Inheritance.Sealed)
                    .Partial(true)
                    .Inherit<CodeWriterTests>())
                .Build();

            Assert.Equal("namespace Charon.Dojo.Tests.Code\n{\n    public sealed partial class TestClass : CodeWriterTests\n    {\n    }\n}\n", actual);
        }

        [Fact]
        public void Methods()
        {
            var actual = new CodeWriter(s => s.Namespace(GetType().Namespace!))
                .Class("TestClass", s => s
                    .Accessibility(Accessibility.Public)
                    .Inheritance(Inheritance.Sealed)
                    .Partial(true))
                .Method("PublicMethod", s => s.Accessibility(Accessibility.Public),
                    code =>
                    {
                        code.WriteLine("// Hello World");
                        code.NewLine();
                        code.WriteLine("var ", "coded", " = ", "true", ';');
                    })
                .Method("PrivateMethod", s => s.ReturnType<Accessibility>(),
                    code =>
                    {
                        code.WriteLine("// Some types:")
                            .Indent()
                            .WriteLine("// ", code.GetName<Accessibility>())
                            .WriteLine("// ", code.GetName(typeof(Accessibility)))
                            .WriteLine("// ", code.GetName(Accessibility.Public))
                            .Unindent();
                        code.WriteLine("// Some more types:")
                            .Indent(indentedCode =>
                            {
                                indentedCode.WriteLine("// ", code.GetName(typeof(DojoRunner)));
                                indentedCode.WriteLine("// ", code.GetName<TypeName>());
                                indentedCode.WriteLine("// ", code.GetName<Json.ObjectListJsonConverter<int>>());
                            });
                    })
                .Build();

            Debug.WriteLine(actual.SingleLine());

            Assert.Equal("using Charon;\nusing Charon.Dojo;\nusing Charon.Dojo.Code;\nusing Charon.Json;\n\nnamespace Charon.Dojo.Tests.Code\n{\n    public sealed partial class TestClass\n    {\n        public void PublicMethod()\n        {\n            // Hello World\n\n            var coded = true;\n        }\n\n        private Accessibility PrivateMethod()\n        {\n            // Some types:\n            // Accessibility\n            // Accessibility\n            // Accessibility.Public\n            // Some more types:\n            // DojoRunner\n            // TypeName\n            // ObjectListJsonConverter<int>\n        }\n    }\n}\n", actual);
        }
    }
}
