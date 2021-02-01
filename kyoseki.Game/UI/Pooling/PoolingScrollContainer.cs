using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;

namespace kyoseki.Game.UI.Pooling
{
    public class PoolingScrollContainer<T> : KyosekiScrollContainer<T>
        where T : DrawablePoolableScrollItem, new()
    {
        private readonly List<PoolableScrollItem> items = new List<PoolableScrollItem>();

        private readonly List<PoolableScrollItem> visibleItems = new List<PoolableScrollItem>();

        private readonly DrawablePool<T> itemPool = new DrawablePool<T>(100);

        private readonly ScrollBoundsItem boundsItem = new ScrollBoundsItem();

        public PoolingScrollContainer()
        {
            ScrollContent.AutoSizeAxes = Axes.None;
            AddInternal(itemPool);
        }

        public void Add(PoolableScrollItem item) =>
            items.Add(item);

        public new void Clear()
        {
            items.Clear();

            foreach (var child in Children)
                child.FadeOut(100, Easing.OutQuint).Expire();
        }

        private const int render_distance = 500;

        protected override void Update()
        {
            base.Update();

            updateYPositions();

            if (visibleItems.Count < 1)
                return;

            var topBound = Current - render_distance;
            var bottomBound = Current + DrawHeight + render_distance;

            boundsItem.ScrollYPosition = topBound;
            int firstIndex = visibleItems.BinarySearch(boundsItem);
            if (firstIndex < 0) firstIndex = ~firstIndex;

            boundsItem.ScrollYPosition = bottomBound;
            int lastIndex = visibleItems.BinarySearch(boundsItem);
            if (lastIndex < 0) lastIndex = ~lastIndex;

            firstIndex = Math.Max(0, firstIndex - 1);
            lastIndex = Math.Clamp(lastIndex + 1, firstIndex, Math.Max(0, visibleItems.Count - 1));

            var toDisplay = visibleItems.GetRange(firstIndex, lastIndex - firstIndex + 1);

            float unloadDistance = maxItemHeight * 3;

            foreach (var child in Children)
            {
                if (toDisplay.Remove(child.Item))
                    continue;

                if (child.Y + child.DrawHeight < topBound - unloadDistance || child.Y > bottomBound + unloadDistance)
                {
                    child.Expire();
                }
            }

            foreach (var item in toDisplay)
            {
                var drawable = itemPool.Get(i => i.Item = item);

                drawable.Y = item.ScrollYPosition;
                drawable.Alpha = 1;

                Add(drawable);
            }
        }

        private float maxItemHeight;

        private void updateYPositions()
        {
            visibleItems.Clear();

            float currentY = 0;

            foreach (var item in items.Where(item => item.Visible))
            {
                if (item.Height > maxItemHeight)
                    maxItemHeight = item.Height;

                item.ScrollYPosition = currentY;

                currentY += item.Height;

                visibleItems.Add(item);
            }

            ScrollContent.Height = currentY;
        }

        private class ScrollBoundsItem : PoolableScrollItem
        {
            public override bool Visible => false;

            public override DrawablePoolableScrollItem CreateDrawable() => throw new NotImplementedException();
        }
    }
}
