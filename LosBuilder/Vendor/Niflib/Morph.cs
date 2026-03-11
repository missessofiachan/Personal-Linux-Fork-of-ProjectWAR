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
    /// Class Morph.
    /// </summary>
    public class Morph
	{
        /// <summary>
        /// The frame name
        /// </summary>
        public NiString FrameName;

        /// <summary>
        /// The number keys
        /// </summary>
        public uint NumKeys;

        /// <summary>
        /// The interpolation
        /// </summary>
        public uint Interpolation;

        /// <summary>
        /// The keys
        /// </summary>
        public KeyGroup<FloatKey> Keys;

        /// <summary>
        /// The unkown int
        /// </summary>
        public uint UnkownInt;

        /// <summary>
        /// The vectors
        /// </summary>
        public Vector3[] Vectors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Morph"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="numVertices">The number vertices.</param>
        public Morph(NiFile file, BinaryReader reader, uint numVertices)
		{
			if (file.Version >= eNifVersion.v10_1_0_106)
			{
				this.FrameName = new NiString(file, reader);
			}
			if (file.Version <= eNifVersion.v10_1_0_0)
			{
				this.Keys = new KeyGroup<FloatKey>(reader);
			}
			if (file.Version >= eNifVersion.v10_1_0_106 && file.Version <= eNifVersion.v10_2_0_0)
			{
				this.UnkownInt = reader.ReadUInt32();
			}
			if (file.Version >= eNifVersion.v20_0_0_4 && file.Version <= eNifVersion.v20_1_0_3)
			{
				this.UnkownInt = reader.ReadUInt32();
			}
			this.Vectors = new Vector3[numVertices];
			int num = 0;
			while ((long)num < (long)((ulong)numVertices))
			{
				this.Vectors[num] = reader.ReadVector3();
				num++;
			}
		}
	}
}
