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
    /// Class NiVisData.
    /// </summary>
    public class NiVisData : NiObject
	{
        /// <summary>
        /// The keys
        /// </summary>
        public ByteKey[] Keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiVisData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiVisData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			uint num = reader.ReadUInt32();
			this.Keys = new ByteKey[num];
			for (int i = 0; i < this.Keys.Length; i++)
			{
				this.Keys[i] = new ByteKey(reader, eKeyType.LINEAR);
			}
		}
	}
}
