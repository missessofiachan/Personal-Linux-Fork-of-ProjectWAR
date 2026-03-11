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
    /// Class NiDynamicEffect.
    /// </summary>
    public class NiDynamicEffect : NiAVObject
	{
        /// <summary>
        /// The switch state
        /// </summary>
        public bool SwitchState;

        /// <summary>
        /// The affected nodes
        /// </summary>
        public NiRef<NiAVObject>[] AffectedNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiDynamicEffect"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiDynamicEffect(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.SwitchState = true;
			if (base.Version >= eNifVersion.v10_1_0_106)
			{
				this.SwitchState = reader.ReadBoolean(Version);
			}
			if (base.Version <= eNifVersion.v4_0_0_2 || base.Version >= eNifVersion.v10_0_1_0)
			{
				this.AffectedNodes = new NiRef<NiAVObject>[reader.ReadUInt32()];
				for (int i = 0; i < this.AffectedNodes.Length; i++)
				{
					this.AffectedNodes[i] = new NiRef<NiAVObject>(reader);
				}
			}
		}
	}
}
