﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    public class SmartE : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the last section
            var lastSetionData = pex.GetSectionData(sections.Length);
            if (lastSetionData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // BITARTS
                    new ContentMatchSet(
                        new ContentMatch(
                            new byte?[] { 0x42, 0x49, 0x54, 0x41, 0x52, 0x54, 0x53 },
                            start: 18319,
                            end: 18320),
                        "SmartE"),
                };

                string match = MatchUtil.GetFirstMatch(file, lastSetionData, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch($"{Path.DirectorySeparatorChar}00001.TMP", useEndsWith: true),
                    new PathMatch($"{Path.DirectorySeparatorChar}00002.TMP", useEndsWith: true)
                 }, "SmartE"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00001.TMP", useEndsWith: true), "SmartE"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}00002.TMP", useEndsWith: true), "SmartE"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
