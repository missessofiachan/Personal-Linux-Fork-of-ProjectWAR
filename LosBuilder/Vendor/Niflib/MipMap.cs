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
    /// Class MipMap.
    /// </summary>
    public class MipMap
	{
        /// <summary>
        /// The width
        /// </summary>
        public uint Width;

        /// <summary>
        /// The height
        /// </summary>
        public uint Height;

        /// <summary>
        /// The offset
        /// </summary>
        public uint Offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="MipMap"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public MipMap(NiFile file, BinaryReader reader)
		{
			this.Width = reader.ReadUInt32();
			this.Height = reader.ReadUInt32();
			this.Offset = reader.ReadUInt32();
		}
	}
}
