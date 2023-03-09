using System.Collections.Generic;
using System.IO;
using System.Text;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    public class ASPack : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .aspack section, if it exists
            bool aspackSection = pex.ContainsSection(".aspack", exact: true);
            if (aspackSection)
                return "ASPack 2.29";

            // TODO: Re-enable all Entry Point checks after implementing
            // Use the entry point data, if it exists
            // if (pex.EntryPointRaw != null)
            // {
            //     var matchers = GenerateMatchers();
            //     string match = MatchUtil.GetFirstMatch(file, pex.EntryPointRaw, matchers, includeDebug);
            //     if (!string.IsNullOrWhiteSpace(match))
            //         return match;
            // }

            // Get the .adata* section, if it exists
            var adataSection = pex.GetFirstSection(".adata", exact: false);
            if (adataSection != null)
            {
                var adataSectionRaw = pex.GetFirstSectionData(Encoding.UTF8.GetString(adataSection.Name));
                if (adataSectionRaw != null)
                {
                    var matchers = GenerateMatchers();
                    string match = MatchUtil.GetFirstMatch(file, adataSectionRaw, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            return null;
        }

        /// <summary>
        /// Generate the set of matchers used for each section
        /// </summary>
        /// <returns></returns>
        private List<ContentMatchSet> GenerateMatchers()
        {
            return new List<ContentMatchSet>
            {
                #region No Wildcards (Long)

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x92, 0x1A, 0x44, 0x00, 0xB8, 0x8C, 0x1A,
                    0x44, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0xCD, 0x1D,
                    0x44, 0x00, 0x89, 0x85, 0xD9, 0x1D, 0x44, 0x00,
                    0x80, 0xBD, 0xC4, 0x1D, 0x44,
                }, "ASPack 1.00b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0xD2, 0x2A, 0x44, 0x00, 0xB8, 0xCC, 0x2A,
                    0x44, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0xA5, 0x2E,
                    0x44, 0x00, 0x89, 0x85, 0xB1, 0x2E, 0x44, 0x00,
                    0x80, 0xBD, 0x9C, 0x2E, 0x44, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x9C, 0x2E, 0x44, 0x00, 0xE8,
                    0x1D, 0x00, 0x00, 0x00, 0xE8, 0xE4, 0x01, 0x00,
                    0x00, 0xE8, 0x7A, 0x02, 0x00, 0x00, 0x8B, 0x85,
                    0x9D, 0x2E, 0x44, 0x00, 0x03, 0x85, 0xB1, 0x2E,
                    0x44, 0x00, 0x89, 0x44, 0x24, 0x1C, 0x61, 0xFF
                }, "ASPack 1.01b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0xD2, 0x2A, 0x44, 0x00, 0xB8, 0xCC, 0x2A,
                    0x44, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0xA5, 0x2E,
                    0x44, 0x00, 0x89, 0x85, 0xB1, 0x2E, 0x44, 0x00,
                    0x80, 0xBD, 0x9C, 0x2E, 0x44
                }, "ASPack 1.01b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x96, 0x78, 0x43, 0x00, 0xB8, 0x90, 0x78,
                    0x43, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x7D, 0x7C,
                    0x43, 0x00, 0x89, 0x85, 0x89, 0x7C, 0x43, 0x00,
                    0x80, 0xBD, 0x74, 0x7C, 0x43, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x74, 0x7C, 0x43, 0x00, 0xE8,
                    0x1D, 0x00, 0x00, 0x00, 0xE8, 0xF7, 0x01, 0x00,
                    0x00, 0xE8, 0x8E, 0x02, 0x00, 0x00, 0x8B, 0x85,
                    0x75, 0x7C, 0x43, 0x00, 0x03, 0x85, 0x89, 0x7C,
                    0x43, 0x00, 0x89, 0x44, 0x24, 0x1C, 0x61, 0xFF
                }, "ASPack 1.02b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x96, 0x78, 0x43, 0x00, 0xB8, 0x90, 0x78,
                    0x43, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x7D, 0x7C,
                    0x43, 0x00, 0x89, 0x85, 0x89, 0x7C, 0x43, 0x00,
                    0x80, 0xBD, 0x74, 0x7C, 0x43, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x74, 0x7C, 0x43
                }, "ASPack 1.02b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x8A, 0x1C, 0x40, 0x00, 0xB9, 0x9E, 0x00,
                    0x00, 0x00, 0x8D, 0xBD, 0x4C, 0x23, 0x40, 0x00,
                    0x8B, 0xF7, 0x33
                }, "ASPack 1.02b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x96, 0x78, 0x43, 0x00, 0xB8, 0x90, 0x78,
                    0x43, 0x00, 0x03, 0xC5
                }, "ASPack 1.02b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0xAE, 0x98, 0x43, 0x00, 0xB8, 0xA8, 0x98,
                    0x43, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x18, 0x9D,
                    0x43, 0x00, 0x89, 0x85, 0x24, 0x9D, 0x43, 0x00,
                    0x80, 0xBD, 0x0E, 0x9D, 0x43
                }, "ASPack 1.03b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0xCE, 0x3A, 0x44, 0x00, 0xB8, 0xC8, 0x3A,
                    0x44, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0xB5, 0x3E,
                    0x44, 0x00, 0x89, 0x85, 0xC1, 0x3E, 0x44, 0x00,
                    0x80, 0xBD, 0xAC, 0x3E, 0x44
                }, "ASPack 1.05b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0xEA, 0xA8, 0x43, 0x00, 0xB8, 0xE4, 0xA8,
                    0x43, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x78, 0xAD,
                    0x43, 0x00, 0x89, 0x85, 0x84, 0xAD, 0x43, 0x00,
                    0x80, 0xBD, 0x6E, 0xAD, 0x43, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x6E, 0xAD, 0x43, 0x00, 0xE8,
                    0x1D, 0x00, 0x00, 0x00, 0xE8, 0x73, 0x02, 0x00,
                    0x00, 0xE8, 0x0A, 0x03, 0x00, 0x00, 0x8B, 0x85,
                    0x70, 0xAD, 0x43, 0x00, 0x03, 0x85, 0x84, 0xAD,
                    0x43, 0x00, 0x89, 0x44, 0x24, 0x1C, 0x61, 0xFF
                }, "ASPack 1.06.01b (DLL)"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0xEA, 0xA8, 0x43, 0x00, 0xB8, 0xE4, 0xA8,
                    0x43, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x78, 0xAD,
                    0x43, 0x00, 0x89, 0x85, 0x84, 0xAD, 0x43, 0x00,
                    0x80, 0xBD, 0x6E, 0xAD, 0x43, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x6E, 0xAD, 0x43
                }, "ASPack 1.06.01b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x3E, 0xD9, 0x43, 0x00, 0xB8, 0x38, 0xD9,
                    0x43, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x0B, 0xDE,
                    0x43, 0x00, 0x89, 0x85, 0x17, 0xDE, 0x43, 0x00,
                    0x80, 0xBD, 0x01, 0xDE, 0x43, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x01, 0xDE, 0x43, 0x00, 0xE8,
                    0x1D, 0x00, 0x00, 0x00, 0xE8, 0x79, 0x02, 0x00,
                    0x00, 0xE8, 0x12, 0x03, 0x00, 0x00, 0x8B, 0x85,
                    0x03, 0xDE, 0x43, 0x00, 0x03, 0x85, 0x17, 0xDE,
                    0x43, 0x00, 0x89, 0x44, 0x24, 0x1C, 0x61, 0xFF
                }, "ASPack 1.07b (DLL)"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xEB, 0x03, 0x5D, 0xFF, 0xE5, 0xE8, 0xF8,
                    0xFF, 0xFF, 0xFF, 0x81, 0xED, 0x1B, 0x6A, 0x44,
                    0x00, 0xBB, 0x10, 0x6A, 0x44, 0x00, 0x03, 0xDD,
                    0x2B, 0x9D, 0x2A
                }, "ASPack 1.08"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x0A, 0x4A, 0x44, 0x00, 0xBB, 0x04, 0x4A,
                    0x44, 0x00, 0x03, 0xDD, 0x2B, 0x9D, 0xB1, 0x50,
                    0x44, 0x00, 0x83, 0xBD, 0xAC, 0x50, 0x44, 0x00,
                    0x00, 0x89, 0x9D, 0xBB, 0x4E, 0x44, 0x00, 0x0F,
                    0x85, 0x17, 0x05, 0x00, 0x00, 0x8D, 0x85, 0xD1,
                    0x50, 0x44, 0x00, 0x50, 0xFF, 0x95, 0x94, 0x51,
                    0x44, 0x00, 0x89, 0x85, 0xCD, 0x50, 0x44, 0x00,
                    0x8B, 0xF8, 0x8D, 0x9D, 0xDE, 0x50, 0x44, 0x00,
                    0x53, 0x50, 0xFF, 0x95, 0x90, 0x51, 0x44, 0x00
                }, "ASPack 1.08.03"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x0A, 0x4A, 0x44, 0x00, 0xBB, 0x04, 0x4A,
                    0x44, 0x00, 0x03, 0xDD, 0x2B, 0x9D, 0xB1, 0x50,
                    0x44, 0x00, 0x83, 0xBD, 0xAC, 0x50, 0x44, 0x00,
                    0x00, 0x89, 0x9D, 0xBB, 0x4E
                }, "ASPack 1.08.03"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x0A, 0x4A, 0x44, 0x00, 0xBB, 0x04, 0x4A,
                    0x44, 0x00, 0x03, 0xDD
                }, "ASPack 1.08.03"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x72, 0x05, 0x00, 0x00, 0xEB, 0x33,
                    0x87, 0xDB, 0x90, 0x00
                }, "ASPack 2.00.01"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x03, 0x00, 0x00, 0x00, 0xE9, 0xEB,
                    0x04, 0x5D, 0x45, 0x55, 0xC3, 0xE8, 0x01, 0x00,
                    0x00, 0x00, 0xEB, 0x5D, 0xBB, 0xED, 0xFF, 0xFF,
                    0xFF, 0x03, 0xDD, 0x81, 0xEB
                }, "ASPack 2.1"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x02, 0x00, 0x00, 0x00, 0xEB, 0x09,
                    0x5D, 0x55, 0x81, 0xED, 0x39, 0x39, 0x44, 0x00,
                    0xC3, 0xE9, 0x3D, 0x04, 0x00, 0x00
                }, "ASPack 2.11b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x02, 0x00, 0x00, 0x00, 0xEB, 0x09,
                    0x5D, 0x55
                }, "ASPack 2.11b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x02, 0x00, 0x00, 0x00, 0xEB, 0x09,
                    0x5D, 0x55, 0x81, 0xED, 0x39, 0x39, 0x44, 0x00,
                    0xC3, 0xE9, 0x59, 0x04, 0x00, 0x00
                }, "ASPack 2.11c"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x02, 0x00, 0x00, 0x00, 0xCD, 0x20,
                    0xE8, 0x00, 0x00, 0x00, 0x00, 0x5E, 0x2B, 0xC9,
                    0x58, 0x74, 0x02
                }, "ASPack 2.11d"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x03, 0x00, 0x00, 0x00, 0xE9, 0xEB,
                    0x04, 0x5D, 0x45, 0x55, 0xC3, 0xE8, 0x01
                }, "ASPack 2.12"),

                #endregion

                #region Wildcards (Long)

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0x3E, 0xD9, 0x43, null, 0xB8, 0x38, null,
                    null, null, 0x03, 0xC5, 0x2B, 0x85, 0x0B, 0xDE,
                    0x43, null, 0x89, 0x85, 0x17, 0xDE, 0x43, null,
                    0x80, 0xBD, 0x01, 0xDE, 0x43, null, null, 0x75,
                    0x15, 0xFE, 0x85, 0x01, 0xDE, 0x43, null, 0xE8,
                    0x1D, null, null, null, 0xE8, 0x79, 0x02, null,
                    null, 0xE8, 0x12, 0x03, null, null, 0x8B, 0x85,
                    0x03, 0xDE, 0x43, null, 0x03, 0x85, 0x17, 0xDE,
                    0x43, null, 0x89, 0x44, 0x24, 0x1C, 0x61, 0xFF
                }, "ASPack 1.00b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0xD2, 0x2A, 0x44, null, 0xB8, 0xCC, 0x2A,
                    0x44, null, 0x03, 0xC5, 0x2B, 0x85, 0xA5, 0x2E,
                    0x44, null, 0x89, 0x85, 0xB1, 0x2E, 0x44, null,
                    0x80, 0xBD, 0x9C, 0x2E, 0x44
                }, "ASPack 1.01b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0xCE, 0x3A, 0x44, null, 0xB8, 0xC8, 0x3A,
                    0x44, null, 0x03, 0xC5, 0x2B, 0x85, 0xB5, 0x3E,
                    0x44, null, 0x89, 0x85, 0xC1, 0x3E, 0x44, null,
                    0x80, 0xBD, 0xAC, 0x3E, 0x44
                }, "ASPack 1.01b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x3E, 0xD9, 0x43, 0x00, 0xB8, 0x38, null,
                    null, 0x00, 0x03, 0xC5, 0x2B, 0x85, 0x0B, 0xDE,
                    0x43, 0x00, 0x89, 0x85, 0x17, 0xDE, 0x43, 0x00,
                    0x80, 0xBD, 0x01, 0xDE, 0x43, 0x00, 0x00, 0x75,
                    0x15, 0xFE, 0x85, 0x01, 0xDE, 0x43, 0x00, 0xE8,
                    0x1D, 0x00, 0x00, 0x00, 0xE8, 0x79, 0x02, 0x00,
                    0x00, 0xE8, 0x12, 0x03, 0x00, 0x00, 0x8B, 0x85,
                    0x03, 0xDE, 0x43, 0x00, 0x03, 0x85, 0x17, 0xDE,
                    0x43, 0x00, 0x89, 0x44, 0x24, 0x1C, 0x61, 0xFF
                }, "ASPack 1.02a -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0x06, null, null, null, 0x64, 0xA0, 0x23
                }, "ASPack 1.02a"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0x96, 0x78, 0x43, null, 0xB8, 0x90, 0x78,
                    0x43, null, 0x03, 0xC5, 0x2B, 0x85, 0x7D, 0x7C,
                    0x43, null, 0x89, 0x85, 0x89, 0x7C, 0x43, null,
                    0x80, 0xBD, 0x74, 0x7C, 0x43
                }, "ASPack 1.02b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0xAE, 0x98, 0x43, null, 0xB8, 0xA8, 0x98,
                    0x43, null, 0x03, 0xC5, 0x2B, 0x85, 0x18, 0x9D,
                    0x43, null, 0x89, 0x85, 0x24, 0x9D, 0x43, null,
                    0x80, 0xBD, 0x0E, 0x9D, 0x43
                }, "ASPack 1.03b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, null, null, null, null, 0xE8, 0x0D, null,
                    null, null, null, null, null, null, null, null,
                    null, null, null, null, null, null, null, 0x58
                }, "ASPack 1.03b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, null, null, null, 0x00, 0xB8, null, null,
                    null, 0x00, 0x03, 0xC5, 0x2B, 0x85, null, 0x12,
                    0x9D, null, 0x89, 0x85, 0x1E, 0x9D, null, 0x00,
                    0x80, 0xBD, 0x08, 0x9D, null, 0x00, 0x00
                }, "ASPack 1.04b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, null, null, null, null, 0xB8, null, null,
                    null, null, 0x03, 0xC5, 0x2B, 0x85, null, 0x12,
                    0x9D, null, 0x89, 0x85, 0x1E, 0x9D, null, null,
                    0x80, 0xBD, 0x08, 0x9D
                }, "ASPack 1.04b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, null, null, null, null, 0xB8, null, null,
                    null, null, 0x03, 0xC5, 0x2B, 0x85, null, 0x0B,
                    0xDE, null, 0x89, 0x85, 0x17, 0xDE, null, null,
                    0x80, 0xBD, 0x01, 0xDE
                }, "ASPack 1.04b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0xEA, 0xA8, 0x43, null, 0xB8, 0xE4, 0xA8,
                    0x43, null, 0x03, 0xC5, 0x2B, 0x85, 0x78, 0xAD,
                    0x43, null, 0x89, 0x85, 0x84, 0xAD, 0x43, null,
                    0x80, 0xBD, 0x6E, 0xAD, 0x43
                }, "ASPack 1.06.1b"),

                new ContentMatchSet(new byte?[]
                {
                    0x90, 0x61, 0xBE, null, null, null, null, 0x8D,
                    0xBE, null, null, null, null, 0x57, 0x83, 0xCD,
                    0xFF
                }, "ASPack 1.06.1b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, null,
                    null, null, null, null, null, 0xB8, null, null,
                    null, null, 0x03, 0xC5
                }, "ASPack 1.07b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, null, null, null, null, 0x60, 0xE8, 0x2B,
                    0x03, 0x00, 0x00
                }, "ASPack 1.07b"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xEB, 0x0A, 0x5D, 0xEB, 0x02, 0xFF, 0x25,
                    0x45, 0xFF, 0xE5, 0xE8, 0xE9, 0xE8, 0xF1, 0xFF,
                    0xFF, 0xFF, 0xE9, 0x81, null, null, null, 0x44,
                    0x00, 0xBB, 0x10, null, 0x44, 0x00, 0x03, 0xDD,
                    0x2B, 0x9D
                }, "ASPack 1.08.01"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xEB, 0x0A, 0x5D, 0xEB, 0x02, 0xFF, 0x25,
                    0x45, 0xFF, 0xE5, 0xE8, 0xE9, 0xE8, 0xF1, 0xFF,
                    0xFF, 0xFF, 0xE9, 0x81, null, null, null, 0x44,
                    null, 0xBB, 0x10, null, 0x44, null, 0x03, 0xDD,
                    0x2B, 0x9D
                }, "ASPack 1.08.01"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xEB, 0x0A, 0x5D, 0xEB, 0x02, 0xFF, 0x25,
                    0x45, 0xFF, 0xE5, 0xE8, 0xE9, 0xE8, 0xF1, 0xFF,
                    0xFF, 0xFF, 0xE9, 0x81, 0xED, 0x23, 0x6A, 0x44,
                    0x00, 0xBB, 0x10, null, 0x44, 0x00, 0x03, 0xDD,
                    0x2B, 0x9D, 0x72
                }, "ASPack 1.08.02"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, null,
                    null, null, null, null, null, 0xBB, null, null,
                    null, null, 0x03, 0xDD, 0x2B, 0x9D, 0xB1, 0x50,
                    0x44, 0x00, 0x83, 0xBD, 0xAC, 0x50, 0x44, 0x00,
                    0x00, 0x89, 0x9D, 0xBB, 0x4E
                }, "ASPack 1.08.03"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, null,
                    null, null, null, null, null, 0xBB, null, null,
                    null, null, 0x03, 0xDD
                }, "ASPack 1.08.03"),

                new ContentMatchSet(new byte?[]
                {
                    0x55, 0x57, 0x51, 0x53, 0xE8, null, null, null,
                    null, 0x5D, 0x8B, 0xC5, 0x81, 0xED, null, null,
                    null, null, 0x2B, 0x85, null, null, null, null,
                    0x83, 0xE8, 0x09, 0x89, 0x85, null, null, null,
                    null, 0x0F, 0xB6
                }, "ASPack 1.08.03"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE9, null, null, null, null, 0xEF, 0x40,
                    0x03, 0xA7, 0x07, 0x8F, 0x07, 0x1C, 0x37, 0x5D,
                    0x43, 0xA7, 0x04, 0xB9, 0x2C, 0x3A
                }, "ASPack 1.08.x"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x02, 0x00, 0x00, 0x00, 0xEB, 0x09,
                    0x5D, 0x55, 0x81, 0xED, 0x39, 0x39, 0x44, 0x00,
                    0xC3, 0xE9, null, 0x04, 0x00, 0x00
                }, "ASPack 2.11.x -> Alexey Solodovnikov"),

                new ContentMatchSet(new byte?[]
                {
                    null, 0xE8, 0x03, 0x00, 0x00, 0x00, 0xE9, 0xEB,
                    0x04, 0x5D, 0x45, 0x55, 0xC3, 0xE8, 0x01
                }, "ASPack 2.12 (without Poly) -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    null, 0x60, 0xE8, 0x03, 0x00, 0x00, 0x00, 0xE9,
                    0xEB, 0x04, 0x5D, 0x45, 0x55, 0xC3, 0xE8, 0x01,
                    0x00, 0x00, 0x00, 0xEB, 0x5D, 0xBB, 0xEC, 0xFF,
                    0xFF, 0xFF, 0x03, 0xDD, 0x81, 0xEB, 0x00, null,
                    null, 0x00, 0x83, 0xBD, 0x22, 0x04, 0x00, 0x00,
                    0x00, 0x89, 0x9D, 0x22, 0x04, 0x00, 0x00, 0x0F,
                    0x85, 0x65, 0x03, 0x00, 0x00, 0x8D, 0x85, 0x2E,
                    0x04, 0x00, 0x00, 0x50, 0xFF, 0x95, 0x4C, 0x0F,
                    0x00, 0x00, 0x89, 0x85, 0x26, 0x04, 0x00, 0x00,
                    0x8B, 0xF8, 0x8D, 0x5D, 0x5E, 0x53, 0x50, 0xFF,
                    0x95, 0x48, 0x0F, 0x00, 0x00, 0x89, 0x85, 0x4C,
                    0x05, 0x00, 0x00, 0x8D, 0x5D, 0x6B, 0x53, 0x57,
                    0xFF, 0x95, 0x48, 0x0F
                }, "ASPack 2.12b -> Solodovnikov Alexey"),

                new ContentMatchSet(new byte?[]
                {
                    0x60, 0xE8, 0x03, 0x00, 0x00, 0x00, 0xE9, 0xEB,
                    0x04, 0x5D, 0x45, 0x55, 0xC3, 0xE8, 0x01, 0x00,
                    0x00, 0x00, 0xEB, 0x5D, 0xBB, 0xED, 0xFF, 0xFF,
                    0xFF, 0x03, 0xDD, null, null, null, null, null,
                    null, 0x83, 0xBD, 0x7D, 0x04, 0x00, 0x00, 0x00,
                    0x89, 0x9D, 0x7D, 0x04, 0x00, 0x00, 0x0F, 0x85,
                    0xC0, 0x03, 0x00, 0x00, 0x8D, 0x85, 0x89, 0x04,
                    0x00, 0x00, 0x50, 0xFF, 0x95, 0x09, 0x0F, 0x00,
                    0x00, 0x89, 0x85, 0x81, 0x04, 0x00, 0x00, 0x8B,
                    0xF0, 0x8D, 0x7D, 0x51, 0x57, 0x56, 0xFF, 0x95,
                    0x05, 0x0F, 0x00, 0x00, 0xAB, 0xB0, 0x00, 0xAE,
                    0x75, 0xFD, 0x38, 0x07, 0x75, 0xEE, 0x8D, 0x45,
                    0x7A, 0xFF, 0xE0, 0x56, 0x69, 0x72, 0x74, 0x75,
                    0x61, 0x6C, 0x41, 0x6C, 0x6C, 0x6F, 0x63, 0x00,
                    0x56, 0x69, 0x72, 0x74, 0x75, 0x61, 0x6C, 0x46,
                    0x72, 0x65, 0x65, 0x00, 0x56, 0x69, 0x72, 0x74,
                    0x75, 0x61, 0x6C, 0x50, 0x72, 0x6F, 0x74, 0x65,
                    0x63, 0x74, 0x00, 0x00, 0x8B, 0x9D, 0x8D, 0x05,
                    0x00, 0x00, 0x0B, 0xDB, 0x74, 0x0A, 0x8B, 0x03,
                    0x87, 0x85, 0x91, 0x05, 0x00, 0x00, 0x89, 0x03,
                    0x8D, 0xB5, 0xBD, 0x05, 0x00, 0x00, 0x83, 0x3E,
                    0x00, 0x0F, 0x84, 0x15, 0x01, 0x00, 0x00, 0x6A,
                    0x04, 0x68, 0x00, 0x10, 0x00, 0x00, 0x68, 0x00,
                    0x18, 0x00, 0x00, 0x6A, 0x00, 0xFF, 0x55, 0x51,
                    0x89, 0x85, 0x53, 0x01, 0x00, 0x00, 0x8B, 0x46,
                    0x04, 0x05, 0x0E, 0x01, 0x00, 0x00, 0x6A, 0x04,
                    0x68, 0x00, 0x10, 0x00, 0x00, 0x50, 0x6A, 0x00,
                    0xFF, 0x55, 0x51, 0x89, 0x85, 0x4F, 0x01, 0x00,
                    0x00, 0x56, 0x8B, 0x1E, 0x03, 0x9D, 0x7D, 0x04,
                    0x00, 0x00, 0xFF, 0xB5, 0x53, 0x01, 0x00, 0x00,
                    0xFF, 0x76, 0x04, 0x50, 0x53, 0xE8, 0x2D, 0x05,
                    0x00, 0x00, 0xB3, 0x00, 0x80, 0xFB, 0x00, 0x75,
                    0x5E, 0xFE, 0x85, 0xE9, 0x00, 0x00, 0x00, 0x8B,
                    0x3E, 0x03, 0xBD, 0x7D, 0x04, 0x00, 0x00, 0xFF,
                    0x37, 0xC6, 0x07, 0xC3, 0xFF, 0xD7, 0x8F, 0x07,
                    0x50, 0x51, 0x56, 0x53, 0x8B, 0xC8, 0x83, 0xE9,
                    0x06, 0x8B, 0xB5, 0x4F, 0x01, 0x00, 0x00, 0x33,
                    0xDB, 0x0B, 0xC9, 0x74, 0x2E, 0x78, 0x2C, 0xAC,
                    0x3C, 0xE8, 0x74, 0x0A, 0xEB, 0x00, 0x3C, 0xE9,
                    0x74, 0x04, 0x43, 0x49, 0xEB, 0xEB, 0x8B, 0x06,
                    0xEB, 0x00, null, null, null, 0x75, 0xF3, 0x24,
                    0x00, 0xC1, 0xC0, 0x18, 0x2B, 0xC3, 0x89, 0x06,
                    0x83, 0xC3, 0x05, 0x83, 0xC6, 0x04, 0x83, 0xE9,
                    0x05, 0xEB, 0xCE, 0x5B, 0x5E, 0x59, 0x58, 0xEB,
                    0x08
                }, "ASPack 2.2 -> Alexey Solodovnikov & StarForce * 2009408"),

                new ContentMatchSet(new byte?[]
                {
                    null, 0x60, 0xE8, 0x03, 0x00, 0x00, 0x00, 0xE9,
                    0xEB, 0x04, 0x5D, 0x45, 0x55, 0xC3, 0xE8, 0x01,
                    0x00, 0x00, 0x00, 0xEB, 0x5D, 0xBB, 0xEC, 0xFF,
                    0xFF, 0xFF, 0x03, 0xDD, 0x81, 0xEB, 0x00, 0x40,
                    0x1C, 0x00
                }, "ASPack 2.x (without Poly) -> Solodovnikov Alexey"),

                #endregion

                #region 2.xx (Long)

                new ContentMatchSet(new byte?[]
                {
                    0xA8, 0x03, 0x00, 0x00, 0x61, 0x75, 0x08, 0xB8,
                    0x01, 0x00, 0x00, 0x00, 0xC2, 0x0C, 0x00, 0x68,
                    0x00, 0x00, 0x00, 0x00, 0xC3, 0x8B, 0x85, 0x26,
                    0x04, 0x00, 0x00, 0x8D, 0x8D, 0x3B, 0x04, 0x00,
                    0x00, 0x51, 0x50, 0xFF, 0x95
                }, "ASPack 2.xx"),

                new ContentMatchSet(new byte?[]
                {
                    0xA8, 0x03, null, null, 0x61, 0x75, 0x08, 0xB8,
                    0x01, null, null, null, 0xC2, 0x0C, null, 0x68,
                    null, null, null, null, 0xC3, 0x8B, 0x85, 0x26,
                    0x04, null, null, 0x8D, 0x8D, 0x3B, 0x04, null,
                    null, 0x51, 0x50, 0xFF, 0x95
                }, "ASPack 2.xx"),

                #endregion

                #region Short

                new ContentMatchSet(new byte?[] { 0x75, 0x00, 0xE9 }, "ASPack 1.05b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x90, 0x75, 0x00, 0xE9 }, "ASPack 1.06.1b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x75, 0x00, 0xE9 }, "ASPack 1.06.1b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x75, 0x00, 0xE9 }, "ASPack 1.06.1b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x90, 0x75, null, 0xE9 }, "ASPack 1.07b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x75, null, 0xE9 }, "ASPack 1.07b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x75, null, 0xE9 }, "ASPack 1.07b"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x90, 0x75, 0x01, 0x90, 0xE9 }, "ASPack 1.08"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x90, 0x75, 0x01, 0xFF, 0xE9 }, "ASPack 1.08"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x75, 0x01, 0xFF, 0xE9 }, "ASPack 1.08"),

                new ContentMatchSet(new byte?[] { 0x90, 0x75, 0x01, 0xFF, 0xE9 }, "ASPack 1.08"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x90, 0x75, null, 0x90, 0xE9 }, "ASPack 1.08.01"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x75, null, 0x90, 0xE9 }, "ASPack 1.08.01"),

                new ContentMatchSet(new byte?[] { 0x90, 0x75, null, 0x90, 0xE9 }, "ASPack 1.08.01"),

                new ContentMatchSet(new byte?[] { 0x90, 0x90, 0x75, 0x01, 0x90, 0xE9 }, "ASPack 1.08.02"),

                new ContentMatchSet(new byte?[] { 0x90, 0x75, 0x01, 0x90, 0xE9 }, "ASPack 1.08.02"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, 0x41, 0x06, 0x00, 0x00, 0xEB, 0x41 }, "ASPack 1.08.04"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, null, null, null, null, 0xEB }, "ASPack 1.08.04"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, 0x70, 0x05, 0x00, 0x00, 0xEB, 0x4C }, "ASPack 2.00.00"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, 0x48, 0x11, 0x00, 0x00, 0xC3, 0x83 }, "ASPack 2.00.00"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, 0x72, 0x05, 0x00, 0x00, 0xEB, 0x4C }, "ASPack 2.00.01"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, null, 0x05, 0x00, 0x00, 0xEB, 0x4C }, "ASPack 2.00.x -> Alexey Solodovnikov"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE9, 0x3D, 0x04, 0x00, 0x00 }, "ASPack 2.11"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, 0xF9, 0x11, 0x00, 0x00, 0xC3, 0x83 }, "ASPack 2.11"),

                new ContentMatchSet(new byte?[] { 0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81, 0xED }, "ASPack 1.02b/1.08.03"),

                #endregion
            };
        }
    }
}
