using kyoseki.Game;
using osu.Framework;
using osu.Framework.Platform;

namespace kyoseki.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"kyoseki"))
            using (osu.Framework.Game game = new KyosekiGame())
                host.Run(game);
        }
    }
}
