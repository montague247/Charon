using Charon.Dojo.Code;

namespace Charon.Dojo.Tests.Code
{
    public static class Extensions
    {
        public static string BuildPartial(this IBuilder builder)
        {
            var writer = new IndentedWriter();

            builder.Build(writer);

            return writer.Text;
        }
    }
}
