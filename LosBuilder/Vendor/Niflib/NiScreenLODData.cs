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
    /// Class NiScreenLODData.
    /// </summary>
    public class NiScreenLODData : NiLODData
	{
        /// <summary>
        /// The bound center
        /// </summary>
        public Vector3 BoundCenter;

        /// <summary>
        /// The bound radius
        /// </summary>
        public float BoundRadius;

        /// <summary>
        /// The world center
        /// </summary>
        public Vector3 WorldCenter;

        /// <summary>
        /// The world radius
        /// </summary>
        public float WorldRadius;

        /// <summary>
        /// The proportion levels
        /// </summary>
        public float[] ProportionLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiScreenLODData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiScreenLODData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.BoundCenter = reader.ReadVector3();
			this.BoundRadius = reader.ReadSingle();
			this.WorldCenter = reader.ReadVector3();
			this.WorldRadius = reader.ReadSingle();
			uint num = reader.ReadUInt32();
			this.ProportionLevels = new float[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				this.ProportionLevels[num2] = reader.ReadSingle();
				num2++;
			}
		}
	}
}
