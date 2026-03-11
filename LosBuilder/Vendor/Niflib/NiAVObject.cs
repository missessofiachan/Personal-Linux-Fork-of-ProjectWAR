namespace Niflib
{
    using System;
    using System.IO;
#if OpenTK
	using OpenTK;
	using OpenTK.Graphics;
	using Matrix = OpenTK.Matrix4;
	using Color3 = OpenTK.Graphics.Color4;
#elif SharpDX
	using SharpDX;
#elif MonoGame
	using Microsoft.Xna.Framework;
	using Color3 = Microsoft.Xna.Framework.Color;
	using Color4 = Microsoft.Xna.Framework.Color;
#else
    using System.Numerics;
    using Matrix = System.Numerics.Matrix4x4;
    using Color3 = System.Numerics.Vector3;
    using Color4 = System.Numerics.Vector4;
#endif

    /// <summary>
    /// Class NiAVObject.
    /// </summary>
    public class NiAVObject : NiObjectNET
    {
        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The unkown short1
        /// </summary>
        public ushort UnkownShort1;

        /// <summary>
        /// The translation
        /// </summary>
        public Vector3 Translation;

        /// <summary>
        /// The rotation
        /// </summary>
        public Matrix Rotation;

        /// <summary>
        /// The scale
        /// </summary>
        public float Scale;

        /// <summary>
        /// The velocity
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// The properties
        /// </summary>
        public NiRef<NiProperty>[] Properties;

        /// <summary>
        /// The unkown ints1
        /// </summary>
        public uint[] UnkownInts1;

        /// <summary>
        /// The unkown byte
        /// </summary>
        public byte UnkownByte;

        /// <summary>
        /// The has bounding box
        /// </summary>
        public bool HasBoundingBox;

        /// <summary>
        /// The collision object
        /// </summary>
        public NiRef<NiCollisionObject> CollisionObject;

        /// <summary>
        /// The parent
        /// </summary>
        public NiNode Parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiAVObject" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">Cannot read BoundingBoxes yet</exception>
        public NiAVObject(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if (this.File.Header.Version >= eNifVersion.v3_0)
            {
                this.Flags = reader.ReadUInt16();
            }
            if (this.File.Header.Version >= eNifVersion.v20_2_0_7 && this.File.Header.UserVersion == 11u && this.File.Header.UserVersion2 > 26u)
            {
                this.UnkownShort1 = reader.ReadUInt16();
            }
            this.Translation = reader.ReadVector3();
            this.Rotation = reader.ReadMatrix33();
            this.Scale = reader.ReadSingle();
            if (this.File.Header.Version <= eNifVersion.v4_2_2_0)
            {
                this.Velocity = reader.ReadVector3();
            }
            if (this.File.Header.Version <= eNifVersion.v20_2_0_7 || this.File.Header.UserVersion <= 11u)
            {
                uint num = reader.ReadUInt32();
                this.Properties = new NiRef<NiProperty>[num];
                int num2 = 0;
                while ((long)num2 < (long)((ulong)num))
                {
                    this.Properties[num2] = new NiRef<NiProperty>(reader.ReadUInt32());
                    num2++;
                }
            }
            if (this.File.Header.Version <= eNifVersion.v2_3)
            {
                this.UnkownInts1 = new uint[4];
                for (int i = 0; i < 4; i++)
                {
                    this.UnkownInts1[i] = reader.ReadUInt32();
                }
                this.UnkownByte = reader.ReadByte();
            }
            if (this.File.Header.Version >= eNifVersion.v3_0 && this.File.Header.Version <= eNifVersion.v4_2_2_0)
            {
                this.HasBoundingBox = reader.ReadBoolean(Version);
                if (this.HasBoundingBox)
                {
                    throw new Exception("Cannot read BoundingBoxes yet");
                }
            }
            if (this.File.Header.Version >= eNifVersion.v10_0_1_0)
            {
                this.CollisionObject = new NiRef<NiCollisionObject>(reader.ReadUInt32());
            }
        }
    }
}
