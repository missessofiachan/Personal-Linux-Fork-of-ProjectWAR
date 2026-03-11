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
    /// Enum eTexType
    /// </summary>
    public enum eTexType : uint
	{
        /// <summary>
        /// Base map
        /// </summary>
        BASE_MAP,
        /// <summary>
        /// Dark map
        /// </summary>
        DARK_MAP,
        /// <summary>
        /// Detail map
        /// </summary>
        DETAIL_MAP,
        /// <summary>
        /// Gloss map
        /// </summary>
        GLOSS_MAP,
        /// <summary>
        /// Glow map
        /// </summary>
        GLOW_MAP,
        /// <summary>
        /// Bump map
        /// </summary>
        BUMP_MAP,
        /// <summary>
        /// Normal map
        /// </summary>
        NORMAL_MAP,
        /// <summary>
        /// Unknown 2 map
        /// </summary>
        UNKNOWN2_MAP,
        /// <summary>
        /// Decal map 0
        /// </summary>
        DECAL_0_MAP,
        /// <summary>
        /// Decal map 1
        /// </summary>
        DECAL_1_MAP,
        /// <summary>
        /// Decal map 2
        /// </summary>
        DECAL_2_MAP,
        /// <summary>
        /// Decal map 3
        /// </summary>
        DECAL_3_MAP
    }
}
