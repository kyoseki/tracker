using System;
using osu.Framework.Graphics.Pooling;

namespace kyoseki.Game.UI.Pooling
{
    public abstract class PoolableScrollItem : IComparable<PoolableScrollItem>
    {
        public float ScrollYPosition;

        public virtual bool Visible => true;

        public virtual float Height => 5;

        public abstract DrawablePoolableScrollItem CreateDrawable();

        public int CompareTo(PoolableScrollItem other) => ScrollYPosition.CompareTo(other.ScrollYPosition);
    }

    public abstract class DrawablePoolableScrollItem : PoolableDrawable
    {
        private PoolableScrollItem item;

        public PoolableScrollItem Item
        {
            get => item;
            set
            {
                item = value;

                if (IsPresent)
                    UpdateItem();
            }
        }

        protected abstract void UpdateItem();
    }
}
