using System.IO;
using System.Text;
using SabreTools.Models.BDPlus;

namespace BinaryObjectScanner.Wrappers
{
    public class BDPlusSVM : WrapperBase<SVM>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "BD+ SVM";

        #endregion

        #region Pass-Through Properties

        /// <inheritdoc cref="Models.BDPlus.SVM.Signature"/>
#if NET48
        public string Signature => _model.Signature;
#else
        public string? Signature => _model.Signature;
#endif

        /// <inheritdoc cref="Models.BDPlus.SVM.Unknown1"/>
#if NET48
        public byte[] Unknown1 => _model.Unknown1;
#else
        public byte[]? Unknown1 => _model.Unknown1;
#endif

        /// <inheritdoc cref="Models.BDPlus.SVM.Year"/>
        public ushort Year => _model.Year;

        /// <inheritdoc cref="Models.BDPlus.SVM.Month"/>
        public byte Month => _model.Month;

        /// <inheritdoc cref="Models.BDPlus.SVM.Day"/>
        public byte Day => _model.Day;

        /// <inheritdoc cref="Models.BDPlus.SVM.Unknown2"/>
#if NET48
        public byte[] Unknown2 => _model.Unknown2;
#else
        public byte[]? Unknown2 => _model.Unknown2;
#endif

        /// <inheritdoc cref="Models.BDPlus.SVM.Length"/>
        public uint Length => _model.Length;

        /// <inheritdoc cref="Models.BDPlus.SVM.Data"/>
#if NET48
        public byte[] Data => _model.Data;
#else
        public byte[]? Data => _model.Data;
#endif

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public BDPlusSVM(SVM model, byte[] data, int offset)
#else
        public BDPlusSVM(SVM? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public BDPlusSVM(SVM model, Stream data)
#else
        public BDPlusSVM(SVM? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BD+ SVM from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BD+ SVM wrapper on success, null on failure</returns>
#if NET48
        public static BDPlusSVM Create(byte[] data, int offset)
#else
        public static BDPlusSVM? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a BD+ SVM from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BD+ SVM wrapper on success, null on failure</returns>
#if NET48
        public static BDPlusSVM Create(Stream data)
#else
        public static BDPlusSVM? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var svm = new SabreTools.Serialization.Streams.BDPlus().Deserialize(data);
            if (svm == null)
                return null;

                try
            {
                return new BDPlusSVM(svm, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.BDPlusSVM.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}