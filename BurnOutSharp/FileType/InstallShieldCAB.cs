﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnshieldSharp;

namespace BurnOutSharp.FileType
{
    internal class InstallShieldCAB
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x49, 0x53, 0x63 }))
                return true;

            return false;
        }

        // TODO: Add stream opening support
        public static Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            // Get the name of the first cabinet file or header
            string directory = Path.GetDirectoryName(file);
            string noExtension = Path.GetFileNameWithoutExtension(file);
            string filenamePattern = Path.Combine(directory, noExtension);
            filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);

            bool cabinetHeaderExists = File.Exists(Path.Combine(directory, filenamePattern + "1.hdr"));
            bool shouldScanCabinet = cabinetHeaderExists
                ? file.Equals(Path.Combine(directory, filenamePattern + "1.hdr"), StringComparison.OrdinalIgnoreCase)
                : file.Equals(Path.Combine(directory, filenamePattern + "1.cab"), StringComparison.OrdinalIgnoreCase);

            // If we have the first file
            if (shouldScanCabinet)
            {
                // If the cab file itself fails
                try
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);

                    UnshieldCabinet cabfile = UnshieldCabinet.Open(file);
                    for (int i = 0; i < cabfile.FileCount; i++)
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, cabfile.FileName(i));
                            cabfile.FileSave(i, tempFile);
                        }
                        catch { }
                    }

                    // Collect and format all found protections
                    var protections = scanner.GetProtections(tempPath);

                    // If temp directory cleanup fails
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { }

                    return protections;
                }
                catch { }
            }

            return null;
        }
    }
}
