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
    /// Class NiKeyframeData.
    /// </summary>
    public class NiKeyframeData : NiObject
	{
        /// <summary>
        /// The key type
        /// </summary>
        public eKeyType KeyType;

        /// <summary>
        /// The quaternion keys
        /// </summary>
        public QuatKey[] QuaternionKeys;

        /// <summary>
        /// The unkown float
        /// </summary>
        public float UnkownFloat;

        /// <summary>
        /// The rotations
        /// </summary>
        public KeyGroup<FloatKey>[] Rotations;

        /// <summary>
        /// The translations
        /// </summary>
        public KeyGroup<VecKey> Translations;

        /// <summary>
        /// The scales
        /// </summary>
        public KeyGroup<FloatKey> Scales;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiKeyframeData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiKeyframeData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			uint num = reader.ReadUInt32();
			if (num != 0u)
			{
				this.KeyType = (eKeyType)reader.ReadUInt32();
			}
			if (this.KeyType != eKeyType.XYZ_ROTATION)
			{
				this.QuaternionKeys = new QuatKey[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					this.QuaternionKeys[num2] = new QuatKey(reader, this.KeyType);
					num2++;
				}
			}
			if (base.Version <= eNifVersion.v10_1_0_0 && this.KeyType == eKeyType.XYZ_ROTATION)
			{
				this.UnkownFloat = reader.ReadSingle();
			}
			if (this.KeyType == eKeyType.XYZ_ROTATION)
			{
				this.Rotations = new KeyGroup<FloatKey>[3];
				for (int i = 0; i < 3; i++)
				{
					this.Rotations[i] = new KeyGroup<FloatKey>(reader);
				}
			}
			this.Translations = new KeyGroup<VecKey>(reader);
			this.Scales = new KeyGroup<FloatKey>(reader);
		}
	}
}
