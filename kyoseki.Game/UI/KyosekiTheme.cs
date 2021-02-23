using osu.Framework.Graphics.Sprites;

namespace kyoseki.Game.UI
{
    public class KyosekiTheme : kyoseki.UI.Components.Theming.KyosekiTheme
    {
        public override FontUsage DefaultFont => KyosekiFont.GetFont();
    }
}
