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
    /// Class NiVectorExtraData.
    /// </summary>
    public class NiVectorExtraData : NiExtraData
	{
        /// <summary>
        /// The data
        /// </summary>
        public Vector3 Data;

        /// <summary>
        /// The unkown float
        /// </summary>
        public float UnkownFloat;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiVectorExtraData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiVectorExtraData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Data = reader.ReadVector3();
			this.UnkownFloat = reader.ReadSingle();
		}
	}
}
