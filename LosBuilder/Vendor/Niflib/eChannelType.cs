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
    /// Enum eChannelType
    /// </summary>
    public enum eChannelType : uint
    {
        /// <summary>
        /// Red
        /// </summary>
        RED,
        /// <summary>
        /// Green
        /// </summary>
        GREEN,
        /// <summary>
        /// Blue
        /// </summary>
        BLUE,
        /// <summary>
        /// Alpha
        /// </summary>
        ALPHA,
        /// <summary>
        /// Compressed
        /// </summary>
        COMPRESSED,
        /// <summary>
        /// Index
        /// </summary>
        INDEX = 16u,
        /// <summary>
        /// Empty
        /// </summary>
        EMPTY = 19u
    }
}
