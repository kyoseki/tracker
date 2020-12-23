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

        public Bone Parent { get; set; }

        public bool IsRoot => Parent == null;

        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);

        public Vector3 RootPoint
        {
            get
            {
                if (Parent == null)
                    return Position;

                return Anchor == BoneAnchor.Root ? Parent.RootPoint : Parent.EndPoint;
            }
        }

        public Vector3 EndPoint => RootPoint + EndOffset;

        public bool IsEnd => Children == null || Children.Length == 0;

        public Vector3 BaseOrientation { get; set; }

        private Quaternion rotation = Quaternion.Identity;

        public Quaternion Rotation
        {
            get => rotation * (Parent?.Rotation ?? Quaternion.Identity);
            set => rotation = value;
        }

        public BoneAnchor Anchor { get; set; } = BoneAnchor.End;

        public Vector3 EndOffset => Vector3.Transform(BaseOrientation, Rotation);

        public bool HasChildren => Children?.Length > 0;

        private Bone[] children;

        public Bone[] Children
        {
            get => children;
            set
            {
                children = value;
                foreach (var child in children)
                {
                    child.Parent = this;
                    child.Rotation *= Rotation;
                }
            }
        }

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
