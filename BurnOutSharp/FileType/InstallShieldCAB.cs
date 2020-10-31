﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static List<string> Scan(Scanner parentScanner, string file, bool includePosition = false)
        {
            List<string> protections = new List<string>();

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

                    // Create a new scanner for the new temp path
                    Scanner subScanner = new Scanner(tempPath, parentScanner.FileProgress)
                    {
                        IncludePosition = parentScanner.IncludePosition,
                        ScanAllFiles = parentScanner.ScanAllFiles,
                        ScanArchives = parentScanner.ScanArchives,
                    };

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
            }

            return protections;
        }
    }
}
