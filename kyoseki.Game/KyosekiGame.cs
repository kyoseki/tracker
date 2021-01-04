using System;
using System.Threading.Tasks;
using kyoseki.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;

namespace kyoseki.Game
{
    public class KyosekiGame : KyosekiGameBase
    {
        private ScreenStack screenStack;
        private Container overlayContainer;

        private DependencyContainer dependencies;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                overlayContainer = new Container { RelativeSizeAxes = Axes.Both }
            };
        }
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected override void LoadComplete()
        {
            base.LoadComplete();

            loadComponentSingleFile(new SerialMonitorOverlay(), overlayContainer.Add, true);
            loadComponentSingleFile(new SkeletonOverlay(), overlayContainer.Add, true);

            screenStack.Push(new MainScreen());
        }

        private Task asyncLoadStream;

        private void loadComponentSingleFile<T>(T component, Action<T> loadCompleteAction, bool cache = false)
            where T : Drawable
        {
            if (cache)
                dependencies.CacheAs(component);

            Schedule(() =>
            {
                var previousLoadStream = asyncLoadStream;

                asyncLoadStream = Task.Run(async () =>
                {
                    if (previousLoadStream != null)
                        await previousLoadStream;

                    try
                    {
                        Logger.Log($"Loading {component}...", level: LogLevel.Debug);

                        Task task = null;
                        var del = new ScheduledDelegate(() => task = LoadComponentAsync(component, loadCompleteAction), 0, -1);
                        Scheduler.Add(del);

                        while (!IsDisposed && !del.Completed)
                            await Task.Delay(10);

                        if (IsDisposed)
                            return;

                        await task;

                        Logger.Log($"Loaded {component}!", level: LogLevel.Debug);
                    }
                    catch (OperationCanceledException) { }
                });
            });
        }
    }
}
