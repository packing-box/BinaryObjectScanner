﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibMSPackN;

namespace BurnOutSharp.FileType
{
    internal class MicrosoftCAB
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x4d, 0x53, 0x43, 0x46 }))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static List<string> Scan(Scanner parentScanner, string file, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the cab file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create a new scanner for the new temp path
                Scanner subScanner = new Scanner(tempPath, parentScanner.FileProgress)
                {
                    IncludePosition = parentScanner.IncludePosition,
                    ScanAllFiles = parentScanner.ScanAllFiles,
                    ScanArchives = parentScanner.ScanArchives,
                };

                using (MSCabinet cabfile = new MSCabinet(file))
                {
                    foreach (var sub in cabfile.GetFiles())
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, sub.Filename);
                            sub.ExtractTo(tempFile);
                        }
                        catch { }
                    }
                }

                // Collect and format all found protections
                var fileProtections = ProtectionFind.Scan(tempPath, includePosition);
                protections = fileProtections.Select(kvp => kvp.Key.Substring(tempPath.Length) + ": " + kvp.Value.TrimEnd()).ToList();

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }
            }
            catch { }

            return protections;
        }
    }
}
