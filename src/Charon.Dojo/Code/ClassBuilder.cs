using System.Text;

namespace Charon.Dojo.Code
{
    public sealed class ClassBuilder(CodeWriter writer, CodeFileArguments codeFileArguments, ClassArguments arguments) : CommonCodeBuilder(writer, codeFileArguments)
    {
        private readonly ClassArguments _arguments = arguments;
        private bool _empty;
        private List<AttributeBuilder>? _attributeBuilders;
        private List<PropertyBuilder>? _propertyBuilders;
        private List<MethodBuilder>? _methodBuilders;

        public ClassBuilder Attribute<T>(Action<AttributeDescriptor>? descriptor = null)
            where T : Attribute
        {
            var attributeDescriptor = new AttributeDescriptor(Arguments, out AttributeArguments attributeArguments).Type<T>();

            return Attribute(descriptor, attributeDescriptor, attributeArguments);
        }

        public ClassBuilder Property<T>(string name, Action<PropertyDescriptor> descriptor)
        {
            var propertyDescriptor = new PropertyDescriptor(name, Arguments, out PropertyArguments attributeArguments).ReturnType<T>();

            return Property(descriptor, propertyDescriptor, attributeArguments);
        }

        public ClassBuilder Method(string name, Action<MethodDescriptor> descriptor, Action<CodeBuilder> code)
        {
            var methodDescriptor = new MethodDescriptor(name, Arguments, out MethodArguments attributeArguments);

            return Method(descriptor, methodDescriptor, code, attributeArguments);
        }

        public ClassBuilder Method<T>(string name, Action<MethodDescriptor> descriptor, Action<CodeBuilder> code)
        {
            var methodDescriptor = new MethodDescriptor(name, Arguments, out MethodArguments attributeArguments).ReturnType<T>();

            return Method(descriptor, methodDescriptor, code, attributeArguments);
        }

        public override void Build(IndentedWriter writer)
        {
            BuildAttributes(writer);
            writer.WriteLine(BuildClass());
            writer.WriteLine("{");
            writer.Indent();

            _empty = true;

            BuildProperties(writer);
            BuildMethods(writer);

            writer.Unindent();
            writer.WriteLine("}");
        }

        private ClassBuilder Attribute(Action<AttributeDescriptor>? descriptor, AttributeDescriptor defaultDescriptor, AttributeArguments arguments)
        {
            descriptor?.Invoke(defaultDescriptor);

            _attributeBuilders ??= [];
            _attributeBuilders.Add(new AttributeBuilder(this, arguments));

            return this;
        }

        private ClassBuilder Property(Action<PropertyDescriptor> descriptor, PropertyDescriptor defaultDescriptor, PropertyArguments arguments)
        {
            descriptor(defaultDescriptor);

            _propertyBuilders ??= [];
            _propertyBuilders.Add(new PropertyBuilder(this, arguments));

            return this;
        }

        private ClassBuilder Method(Action<MethodDescriptor> descriptor, MethodDescriptor defaultDescriptor, Action<CodeBuilder> code, MethodArguments arguments)
        {
            descriptor(defaultDescriptor);

            var codeBuilder = new CodeBuilder(this);
            code(codeBuilder);

            _methodBuilders ??= [];
            _methodBuilders.Add(new MethodBuilder(this, arguments, codeBuilder));

            return this;
        }

        private void BuildProperties(IndentedWriter writer)
        {
            if (_propertyBuilders == null ||
                _propertyBuilders.Count == 0)
                return;

            foreach (var builder in _propertyBuilders)
            {
                if (_empty)
                    _empty = false;
                else
                    writer.NewLine();

                builder.Build(writer);
            }
        }

        private void BuildMethods(IndentedWriter writer)
        {
            if (_methodBuilders == null ||
                _methodBuilders.Count == 0)
                return;

            foreach (var builder in _methodBuilders)
            {
                if (_empty)
                    _empty = false;
                else
                    writer.NewLine();

                builder.Build(writer);
            }
        }

        private string BuildClass()
        {
            var sb = new StringBuilder(_arguments.Accessibility.Value());

            if (_arguments.Inheritance != Inheritance.Default)
                sb.Append(' ').Append(_arguments.Inheritance.Value());

            if (_arguments.Partial)
                sb.Append(" partial");

            sb.Append(" class ").Append(_arguments.Name);

            if (_arguments.Inherits != null)
                BuildInherit(sb);

            return sb.ToString();
        }

        private void BuildAttributes(IndentedWriter writer)
        {
            if (_attributeBuilders == null ||
                _attributeBuilders.Count == 0)
                return;

            foreach (var builder in _attributeBuilders)
            {
                builder.Build(writer);
            }
        }

        private void BuildInherit(StringBuilder sb)
        {
            var first = true;

            foreach (var inherit in _arguments.Inherits!)
            {
                if (first)
                {
                    first = false;
                    sb.Append(" : ");
                }
                else
                    sb.Append(", ");

                sb.Append(inherit.ToString());
            }
        }
    }
}
