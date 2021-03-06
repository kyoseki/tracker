﻿using System;
using System.Threading.Tasks;
using kyoseki.Game.Configuration;
using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.Overlays.Skeleton;
using kyoseki.Game.Screens;
using kyoseki.Game.Serial;
using kyoseki.UI.Components.Theming;
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
            ThemeContainer themeContainer;

            Child = themeContainer = new ThemeContainer(new UI.KyosekiTheme())
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                    overlayContainer = new Container { RelativeSizeAxes = Axes.Both }
                }
            };

            dependencies.Cache(themeContainer);
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new SerialDependencies(base.CreateChildDependencies(parent));

        protected override void LoadComplete()
        {
            base.LoadComplete();

            dependencies.CacheAs(new KyosekiConfigManager(Host.Storage));

            AddInternal(dependencies.Get<ConnectionManager>());
            AddInternal(dependencies.Get<SkeletonLinkManager>());

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
                        var del = new ScheduledDelegate(() => task = LoadComponentAsync(component, loadCompleteAction));
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
