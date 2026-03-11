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
    /// Enum eNifVersion
    /// </summary>
    public enum eNifVersion : uint
    {
        /// <summary>
        /// Version 2.3
        /// </summary>
        v2_3 = 33751040u,
        /// <summary>
        /// Version 3.0
        /// </summary>
        v3_0 = 50331648u,
        /// <summary>
        /// Version 3.03
        /// </summary>
        v3_03 = 50332416u,
        /// <summary>
        /// Version 3.1
        /// </summary>
        v3_1 = 50397184u,
        /// <summary>
        /// Version 3.3.0.13
        /// </summary>
        v3_3_0_13 = 50528269u,
        /// <summary>
        /// Version 4.0.0.0
        /// </summary>
        v4_0_0_0 = 67108864u,
        /// <summary>
        /// Version 4.0.0.2
        /// </summary>
        v4_0_0_2 = 67108866u,
        /// <summary>
        /// Version 4.1.0.12
        /// </summary>
        v4_1_0_1 = 0x04010001,
        /// <summary>
        /// Version 4.1.0.12
        /// </summary>
        v4_1_0_12 = 67174412u,
        /// <summary>
        /// Version 4.2.0.2
        /// </summary>
        v4_2_0_2 = 67239938u,
        /// <summary>
        /// Version 4.2.1.0
        /// </summary>
        v4_2_1_0 = 67240192u,
        /// <summary>
        /// Version 4.2.2.0
        /// </summary>
        v4_2_2_0 = 67240448u,
        /// <summary>
        /// Version 5.0.0.1
        /// </summary>
        v5_0_0_1 = 83886081u,
        /// <summary>
        /// Version 5.0.0.6
        /// </summary>
        v5_0_0_6 = 83886086u,
        /// <summary>
        /// Version 10.0.1.0
        /// </summary>
        v10_0_1_0 = 167772416u,
        /// <summary>
        /// Version 10.0.1.2
        /// </summary>
        v10_0_1_2 = 167772418u,
        /// <summary>
        /// Version 10.0.1.3
        /// </summary>
        v10_0_1_3 = 167772419u,
        /// <summary>
        /// Version 10.1.0.0
        /// </summary>
        v10_1_0_0 = 167837696u,
        /// <summary>
        /// Version 10.1.0.101
        /// </summary>
        v10_1_0_101 = 167837797u,
        /// <summary>
        /// Version 10.1.0.106
        /// </summary>
        v10_1_0_106 = 167837802u,
        /// <summary>
        /// Version 10.1.0.114
        /// </summary>
        v10_1_0_114 = 167837810u,
        /// <summary>
        /// Version 10.2.0.0
        /// </summary>
        v10_2_0_0 = 167903232u,
        /// <summary>
        /// Version 10.4.0.1
        /// </summary>
        v10_4_0_1 = 168034305u,
        /// <summary>
        /// Version 20.0.0.3
        /// </summary>
        v20_0_0_3 = 335544323u,
        /// <summary>
        /// Version 20.0.0.4
        /// </summary>
        v20_0_0_4 = 335544324u,
        /// <summary>
        /// Version 20.0.0.5
        /// </summary>
        v20_0_0_5 = 335544325u,
        /// <summary>
        /// Version 20.1.0.1
        /// </summary>
        v20_1_0_1 = 335609857u,
        /// <summary>
        /// Version 20.1.0.2
        /// </summary>
        v20_1_0_2 = 335609858u,
        /// <summary>
        /// Version 20.1.0.3
        /// </summary>
        v20_1_0_3 = 335609859u,
        /// <summary>
        /// Version 20.2.0.5
        /// </summary>
        v20_2_0_5 = 335675397u,
        /// <summary>
        /// Version 20.2.0.7
        /// </summary>
        v20_2_0_7 = 335675399u,
        /// <summary>
        /// Version 20.2.0.8
        /// </summary>
        v20_2_0_8 = 335675400u,
        /// <summary>
        /// Version 20.3.0.1
        /// </summary>
        v20_3_0_1 = 335740929u,
        /// <summary>
        /// Version 20.3.0.2
        /// </summary>
        v20_3_0_2 = 335740930u,
        /// <summary>
        /// Version 20.3.0.3
        /// </summary>
        v20_3_0_3 = 335740931u,
        /// <summary>
        /// Version 20.3.0.6
        /// </summary>
        v20_3_0_6 = 335740934u,
        /// <summary>
        /// Version 20.3.0.9
        /// </summary>
        v20_3_0_9 = 335740937u,
        /// <summary>
        /// Version Unsupported
        /// </summary>
        UNSUPPORTED = 4294967295u,
        /// <summary>
        /// Version Invalid
        /// </summary>
        INVALID = 4294967294u
    }
}
