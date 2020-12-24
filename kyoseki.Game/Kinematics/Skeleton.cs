using System.Numerics;

namespace kyoseki.Game.Kinematics
{
    public class Skeleton
    {
        public Bone Root { get; set; }

        public Bone GetBone(string name)
        {
            Bone result = null;

            Root.Traverse(bone =>
            {
                if (bone.Name == name)
                {
                    result = bone;
                }
            });

            return result;
        }

        private static Bone upper = new Bone
        {
            Name = "Collar",
            BaseOrientation = new Vector3(-5, 0, 0),
            Child = new Bone
            {
                Name = "Shoulder",
                BaseOrientation = new Vector3(-10, 0, 0),
                Child = new Bone
                {
                    Name = "Elbow",
                    BaseOrientation = new Vector3(-12, 0, 0),
                    Child = new Bone
                    {
                        Name = "Wrist",
                        BaseOrientation = new Vector3(-5, 0, 0)
                    }
                }
            }
        };

        private static Bone lower = new Bone
        {
            Name = "Hip",
            BaseOrientation = new Vector3(-5, 10, 0),
            Child = new Bone
            {
                Name = "Knee",
                BaseOrientation = new Vector3(0, 15, 0),
                Child = new Bone
                {
                    Name = "Ankle",
                    BaseOrientation = new Vector3(0, 0, -8)
                }
            }
        };

        public static Skeleton DEFAULT_SKELETON = new Skeleton
        {
            Root = new Bone
            {
                Name = "Hips",
                BaseOrientation = new Vector3(0, 4, 0),
                Children = new Bone[]
                {
                    new Bone
                    {
                        Name = "Chest",
                        Anchor = BoneAnchor.Root,
                        BaseOrientation = new Vector3(0, -20, 0),
                        Children = new Bone[]
                        {
                            new Bone
                            {
                                Name = "Neck",
                                BaseOrientation = new Vector3(0, -5, 0),
                                Children = new Bone[]
                                {
                                    new Bone
                                    {
                                        Name = "Head",
                                        BaseOrientation = new Vector3(0, -10, 0)
                                    }
                                }
                            },
                            upper.Clone().ApplyPrefix("Left"),
                            upper.Clone().Mirror(MirrorAxes.X).ApplyPrefix("Right")
                        }
                    },
                    lower.Clone().ApplyPrefix("Left"),
                    lower.Clone().Mirror(MirrorAxes.X).ApplyPrefix("Right")
                }
            }
        };
    }
}
