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
    /// Enum eCoordGenType
    /// </summary>
    public enum eCoordGenType : uint
    {
        /// <summary>
        /// World parallel
        /// </summary>
        WORLD_PARALLEL,
        /// <summary>
        /// Worl perspective
        /// </summary>
        WORLD_PERSPECTIVE,
        /// <summary>
        /// Sphere map
        /// </summary>
        SPHERE_MAP,
        /// <summary>
        /// Specular cube map
        /// </summary>
        SPECULAR_CUBE_MAP,
        /// <summary>
        /// Diffuse cube map
        /// </summary>
        DIFFUSE_CUBE_MAP
    }
}
