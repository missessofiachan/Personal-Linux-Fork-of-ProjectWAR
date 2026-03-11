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
    /// Enum ePixelLayout
    /// </summary>
    public enum ePixelLayout : uint
	{
        /// <summary>
        /// Palettised
        /// </summary>
        PALETTISED,
        /// <summary>
        /// High color 16
        /// </summary>
        HIGH_COLOR_16,
        /// <summary>
        /// True color 32
        /// </summary>
        TRUE_COLOR_32,
        /// <summary>
        /// Compressed
        /// </summary>
        COMPRESSED,
        /// <summary>
        /// Bumpmap
        /// </summary>
        BUMPMAP,
        /// <summary>
        /// Palettised 4
        /// </summary>
        PALETTISED_4,
        /// <summary>
        /// Default
        /// </summary>
        DEFAULT
    }
}
