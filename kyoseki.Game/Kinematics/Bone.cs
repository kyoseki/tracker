using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using osuTK;

namespace kyoseki.Game.Kinematics
{
    [Serializable]
    public class Bone
    {
        public string Name { get; set; }

        /// <summary>
        /// This bone's parent. Only gets set upon adding a bone as a child to another.
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
            get => rotation * (Parent?.Rotation ?? Quaternion.Identity);
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
        /// No adjustments are reversed if the child is removed.
        /// </summary>
        public Bone[] Children
        {
            get => children;
            set
            {
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
                if (Children.Length == 1)
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
        /// https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net
        /// Create a deep clone of this Bone
        /// </summary>
        public Bone Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Bone)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Mirror the values of the provided axis.
        /// Does not mirror ACROSS the axis - inverts the values instead
        /// </summary>
        /// <param name="axis">The value to mirror</param>
        public Bone Mirror(MirrorAxes axis)
        {
            Action<Bone> action = bone =>
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
            };

            Traverse(action);
            return this;
        }

        private void traverse(Action<Bone> action, Bone current)
        {
            if (current.HasChildren)
            {
                foreach (var child in current.Children)
                {
                    child.Traverse(action);
                }
            }

            action.Invoke(this);
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
