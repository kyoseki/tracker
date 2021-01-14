using System.Collections.Generic;
using osu.Framework.Input.Bindings;

namespace kyoseki.Game.Input.Bindings
{
    public class GlobalActionContainer : KeyBindingContainer<GlobalAction>
    {
        public GlobalActionContainer()
            : base(matchingMode: KeyCombinationMatchingMode.Exact)
        {
        }

        public override IEnumerable<KeyBinding> DefaultKeyBindings => new[]
        {
            new KeyBinding(new[] { InputKey.Escape }, GlobalAction.Back)
        };
    }

    public enum GlobalAction
    {
        Back
    }
}
