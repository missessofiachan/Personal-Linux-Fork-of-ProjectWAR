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
    /// Class NiRotatingParticlesData.
    /// </summary>
    public class NiRotatingParticlesData : NiParticlesData
	{
        /// <summary>
        /// The has rotations2
        /// </summary>
        public bool HasRotations2;

        /// <summary>
        /// The rotations2
        /// </summary>
        public Vector4[] Rotations2;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiRotatingParticlesData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiRotatingParticlesData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (base.Version <= eNifVersion.v4_2_2_0)
			{
				this.HasRotations2 = reader.ReadBoolean(Version);
				this.Rotations2 = new Vector4[this.NumVertices];
				int num = 0;
				while ((long)num < (long)((ulong)this.NumVertices))
				{
					this.Rotations2[num] = reader.ReadVector4();
					num++;
				}
			}
		}
	}
}
