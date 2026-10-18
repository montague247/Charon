using Figgle.Fonts;

namespace Charon.Generator
{
    public static class Banner
    {
        public static string Create(string text)
        {
            return FiggleFonts.Doom.Render(text);
        }
    }
}
