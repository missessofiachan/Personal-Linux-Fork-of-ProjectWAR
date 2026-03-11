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
    /// Class NiTextKeyExtraData.
    /// </summary>
    public class NiTextKeyExtraData : NiExtraData
	{
        /// <summary>
        /// The number text keys
        /// </summary>
        public uint NumTextKeys;

        /// <summary>
        /// The unkown int1
        /// </summary>
        public uint UnkownInt1;

        /// <summary>
        /// The text keys
        /// </summary>
        public StringKey[] TextKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTextKeyExtraData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTextKeyExtraData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (base.Version <= eNifVersion.v4_2_2_0)
			{
				this.UnkownInt1 = reader.ReadUInt32();
			}
			this.NumTextKeys = reader.ReadUInt32();
			this.TextKeys = new StringKey[this.NumTextKeys];
			int num = 0;
			while ((long)num < (long)((ulong)this.NumTextKeys))
			{
				this.TextKeys[num] = new StringKey(reader, eKeyType.LINEAR);
				num++;
			}
		}
	}
}
