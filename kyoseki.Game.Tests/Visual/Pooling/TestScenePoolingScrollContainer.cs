using kyoseki.Game.UI;
using kyoseki.Game.UI.Pooling;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.Pooling
{
    public class TestScenePoolingScrollContainer : TestScene
    {
        private const int text_height = 100;

        private readonly PoolingScrollContainer<DrawableTestPoolableScrollItem> scroll;

        public TestScenePoolingScrollContainer()
        {
            Add(scroll = new PoolingScrollContainer<DrawableTestPoolableScrollItem>
            {
                RelativeSizeAxes = Axes.Both
            });
        }

        [Test]
        public void TestAddItems()
        {
            int items = 0;

            AddRepeatStep("Add items", () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    scroll.Add(new TestPoolableScrollItem
                    {
                        Text = { Value = $"hello there! {items}" }
                    });

                    items++;
                }
            }, 10);
        }

        private class TestPoolableScrollItem : PoolableScrollItem
        {
            public override float Height => text_height;

            public readonly Bindable<string> Text = new();

            public override DrawablePoolableScrollItem CreateDrawable() =>
                new DrawableTestPoolableScrollItem { Item = this };
        }

        private class DrawableTestPoolableScrollItem : DrawablePoolableScrollItem
        {
            private readonly Bindable<string> text = new();

            public DrawableTestPoolableScrollItem()
            {
                SpriteText spriteText;
                RelativeSizeAxes = Axes.X;

                InternalChild = spriteText = new SpriteText
                {
                    Font = KyosekiFont.GetFont(size: text_height),
                    RelativeSizeAxes = Axes.X
                };

                text.ValueChanged += e =>
                {
                    spriteText.Text = e.NewValue;
                };
            }

            protected override void UpdateItem()
            {
                text.UnbindBindings();
                text.BindTo(((TestPoolableScrollItem)Item).Text);
            }
        }
    }
}
