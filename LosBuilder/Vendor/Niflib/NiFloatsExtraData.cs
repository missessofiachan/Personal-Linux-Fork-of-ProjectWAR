namespace Niflib
{
    using System.IO;

    /// <summary>
    /// Class NiFloatsExtraData.
    /// </summary>
    public class NiFloatsExtraData : NiExtraData
    {
        /// <summary>
        /// The float payload.
        /// </summary>
        public float[] FloatData;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiFloatsExtraData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiFloatsExtraData(NiFile file, BinaryReader reader)
            : base(file, reader)
        {
            uint count = reader.ReadUInt32();
            this.FloatData = new float[count];
            for (int i = 0; i < this.FloatData.Length; i++)
                this.FloatData[i] = reader.ReadSingle();
        }
    }
}
