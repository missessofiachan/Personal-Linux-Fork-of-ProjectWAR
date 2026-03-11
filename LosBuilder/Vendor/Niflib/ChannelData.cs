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
    /// Class ChannelData.
    /// </summary>
    public class ChannelData
    {
        /// <summary>
        /// The type
        /// </summary>
        public eChannelType Type;

        /// <summary>
        /// The convention
        /// </summary>
        public eChannelConvention Convention;

        /// <summary>
        /// The bits per channel
        /// </summary>
        public byte BitsPerChannel;

        /// <summary>
        /// The unkown byte
        /// </summary>
        public byte UnkownByte;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public ChannelData(NiFile file, BinaryReader reader)
        {
            Type = (eChannelType)reader.ReadUInt32();
            Convention = (eChannelConvention)reader.ReadUInt32();
            BitsPerChannel = reader.ReadByte();
            UnkownByte = reader.ReadByte();
        }
    }
}
