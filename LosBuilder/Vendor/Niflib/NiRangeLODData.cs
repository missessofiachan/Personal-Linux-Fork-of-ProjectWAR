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
    /// Class NiRangeLODData.
    /// </summary>
    public class NiRangeLODData : NiLODData
	{
        /// <summary>
        /// The lod center
        /// </summary>
        public Vector3 LODCenter;

        /// <summary>
        /// The lod levels
        /// </summary>
        public LODRange[] LODLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiRangeLODData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiRangeLODData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.LODCenter = reader.ReadVector3();
			uint num = reader.ReadUInt32();
			this.LODLevels = new LODRange[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				this.LODLevels[num2] = new LODRange(file, reader);
				num2++;
			}
		}
	}
}
