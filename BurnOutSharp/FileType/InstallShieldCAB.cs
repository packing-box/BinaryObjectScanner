﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using UnshieldSharp.Cabinet;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// InstallShield cabinet file
    /// </summary>
    public class InstallShieldCAB : IExtractable
    {
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
            // Get the name of the first cabinet file or header
            string directory = Path.GetDirectoryName(file);
            string noExtension = Path.GetFileNameWithoutExtension(file);
            string filenamePattern = Path.Combine(directory, noExtension);
            filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);

            bool cabinetHeaderExists = File.Exists(Path.Combine(directory, filenamePattern + "1.hdr"));
            bool shouldScanCabinet = cabinetHeaderExists
                ? file.Equals(Path.Combine(directory, filenamePattern + "1.hdr"), StringComparison.OrdinalIgnoreCase)
                : file.Equals(Path.Combine(directory, filenamePattern + "1.cab"), StringComparison.OrdinalIgnoreCase);

            // If we have anything but the first file
            if (!shouldScanCabinet)
                return null;

            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                InstallShieldCabinet cabfile = InstallShieldCabinet.Open(file);
                for (int i = 0; i < cabfile.FileCount; i++)
                {
                    try
                    {
                        // Check if the file is valid first
                        if (!cabfile.FileIsValid(i))
                            continue;

                        string tempFile;
                        try
                        {
                            string filename = cabfile.FileName(i);
                            tempFile = Path.Combine(tempPath, filename);
                        }
                        catch
                        {
                            tempFile = Path.Combine(tempPath, $"BAD_FILENAME{i}");
                        }

                        cabfile.FileSave(i, tempFile);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }
    }
}
