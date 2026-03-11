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
    /// Class LODRange.
    /// </summary>
    public class LODRange
	{
        /// <summary>
        /// The near extent
        /// </summary>
        public float NearExtent;

        /// <summary>
        /// The far extent
        /// </summary>
        public float FarExtent;

        /// <summary>
        /// The unkown ints
        /// </summary>
        public uint[] UnkownInts;

        /// <summary>
        /// Initializes a new instance of the <see cref="LODRange"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public LODRange(NiFile file, BinaryReader reader)
		{
			this.NearExtent = reader.ReadSingle();
			this.FarExtent = reader.ReadSingle();
			if (file.Version <= eNifVersion.v3_1)
			{
				this.UnkownInts = new uint[]
				{
					reader.ReadUInt32(),
					reader.ReadUInt32(),
					reader.ReadUInt32()
				};
			}
		}
	}
}
