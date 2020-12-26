using System;
using System.Linq;
using System.Numerics;

namespace kyoseki.Game.Kinematics
{
    /// <summary>
    /// A class representing basic information for joints in a skeleton.
    /// 2D coordinates are provided using osuTK for compatability with osu!framework.
    /// 3D vectors and quaternions are provided using System.Numerics as osuTK Quaternions are broken.
    /// </summary>
    public class Bone
    {
        public string Name { get; set; }

        /// <summary>
        /// This bone's parent.
        /// </summary>
        public Bone Parent { get; private set; }

        public bool IsRoot => Parent == null;

        /// <summary>
        /// The fallback to the root position of this bone,
        /// used if there are no parents to base position off of.
        /// </summary>
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);

        /// <summary>
        /// The root point of this bone.
        /// </summary>
        public Vector3 RootPoint
        {
            get
            {
                if (Parent == null)
                    return Position;

                return Anchor == BoneAnchor.Root ? Parent.RootPoint : Parent.EndPoint;
            }
        }

        /// <summary>
        /// The end point of this bone.
        /// </summary>
        public Vector3 EndPoint => RootPoint + Vector3.Transform(BaseOrientation, Rotation);

        public osuTK.Vector2 Root2D => new osuTK.Vector2(RootPoint.X, RootPoint.Y);

        public osuTK.Vector2 End2D => new osuTK.Vector2(EndPoint.X, EndPoint.Y);

        /// <summary>
        /// The "base orientation" of this bone -
        /// a vector (including magnitude) representing the orientation of
        /// this bone with zero rotation
        /// </summary>
        public Vector3 BaseOrientation { get; set; }

        private Quaternion rotation = Quaternion.Identity;

        /// <summary>
        /// The rotation of this bone, around its root point.
        /// </summary>
        public Quaternion Rotation
        {
            get => (Parent?.Rotation ?? Quaternion.Identity) * rotation;
            set => rotation = value;
        }

        /// <summary>
        /// Where this bone should be anchored, relative to its parent.
        /// </summary>
        public BoneAnchor Anchor { get; set; } = BoneAnchor.End;

        public bool HasChildren => Children?.Length > 0;

        private Bone[] children;

        /// <summary>
        /// List of children to this bone.
        /// All bones listed as children will have their <see cref="Parent"/> value set to this bone.
        /// </summary>
        public Bone[] Children
        {
            get => children;
            set
            {
                if (HasChildren)
                {
                    foreach (var child in children)
                    {
                        child.Parent = null;
                    }
                }

                if (value == null)
                {
                    children = null;
                    return;
                }

                foreach (var child in value)
                {
                    if (!child.IsRoot)
                    {
                        throw new InvalidOperationException($"Bone {child.Name} already has a parent, and cannot be the child of multiple bones");
                    }
                }

                children = value;

                foreach (var child in children)
                {
                    child.Parent = this;
                }
            }
        }

        /// <summary>
        /// get: Return this bone's only child (if applicable)
        /// set: Set the provided bone as this bone's only child
        /// </summary>
        public Bone Child
        {
            get
            {
                if (Children?.Length == 1)
                {
                    return Children[0];
                }

                return null;
            }
            set => Children = new Bone[]
            {
                value
            };
        }

        /// <summary>
        /// Appends a string to the beginning of this bone and all children.
        /// </summary>
        public Bone ApplyPrefix(string prefix)
        {
            Traverse(bone =>
            {
                bone.Name = prefix + bone.Name;
            });

            return this;
        }

        /// <summary>
        /// Create a clone of this Bone
        /// </summary>
        public Bone Clone()
        {
            return new Bone
            {
                Name = Name,
                BaseOrientation = BaseOrientation,
                Children = Children?.Select(c => c.Clone()).ToArray()
            };
        }

        /// <summary>
        /// Mirror the values of the provided axis.
        /// Does not mirror ACROSS the axis - inverts the values instead
        /// </summary>
        /// <param name="axis">The value to mirror</param>
        public Bone Mirror(MirrorAxes axis)
        {
            Traverse(bone =>
            {
                float x = bone.BaseOrientation.X;
                float y = bone.BaseOrientation.Y;
                float z = bone.BaseOrientation.Z;

                switch (axis)
                {
                    case MirrorAxes.X:
                        x *= -1;
                        break;
                    case MirrorAxes.Y:
                        y *= -1;
                        break;
                    case MirrorAxes.Z:
                        z *= -1;
                        break;
                }

                bone.BaseOrientation = new Vector3(x, y, z);
            });

            return this;
        }

        private void traverse(Action<Bone> action, Bone current)
        {
            action.Invoke(this);

            if (current.HasChildren)
            {
                foreach (var child in current.Children)
                {
                    child.Traverse(action);
                }
            }
        }

        /// <summary>
        /// Execute an action on this bone and all children recursively
        /// </summary>
        /// <param name="action">The action to perform on each bone individually</param>
        public void Traverse(Action<Bone> action) => traverse(action, this);
    }

    public enum BoneAnchor
    {
        Root,
        End
    }

    public enum MirrorAxes
    {
        X,
        Y,
        Z
    }
}
