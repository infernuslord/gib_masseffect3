﻿/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.MassEffect3.FileFormats;
using NDesk.Options;
using Newtonsoft.Json;
using Coalesced = Gibbed.MassEffect3.FileFormats.Coalesced;

namespace Gibbed.MassEffect3.Coalesce
{
    internal class Program
    {
        private static string GetExecutablePath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void Main(string[] args)
        {
            //var serializer = new DataContractSerializer(typeof(TType));

            var mode = Mode.Unknown;
            var showHelp = false;

            var options = new OptionSet()
            {
                {
                    "b|json2bin",
                    "convert json to bin",
                    v => mode = v != null ? Mode.ToBIN : mode
                },
                {
                    "j|bin2json",
                    "convert bin to json",
                    v => mode = v != null ? Mode.ToJSON : mode
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            // try to figure out what they want to do
            if (mode == Mode.Unknown &&
                extras.Count >= 1)
            {
                var testPath = extras[0];

                if (Directory.Exists(testPath) == true)
                {
                    mode = Mode.ToBIN;
                }
                else if (File.Exists(testPath) == true)
                {
                    mode = Mode.ToJSON;
                }
            }

            if (extras.Count < 1 || extras.Count > 2 ||
                showHelp == true || mode == Mode.Unknown)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ -j input_bin [output_dir]", GetExecutableName());
                Console.WriteLine("       {0} [OPTIONS]+ -b input_dir [output_bin]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            if (mode == Mode.ToJSON)
            {
                var inputPath = extras[0];
                var outputPath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, null);

                using (var input = File.OpenRead(inputPath))
                {
                    var coal = new CoalescedFile();
                    coal.Deserialize(input);

                    var padding = coal.Files.Count.ToString().Length;

                    var setup = new Setup();
                    setup.Endian = coal.Endian;
                    setup.Version = coal.Version;

                    var counter = 0;
                    foreach (var file in coal.Files)//.OrderBy(f => f.Name))
                    {
                        var iniPath = string.Format("{1}_{0}",
                            Path.GetFileNameWithoutExtension(file.Name),
                            counter.ToString().PadLeft(padding, '0'));
                        iniPath = Path.Combine(outputPath, Path.ChangeExtension(iniPath, ".json"));
                        counter++;

                        setup.Files.Add(Path.GetFileName(iniPath));

                        Directory.CreateDirectory(Path.GetDirectoryName(iniPath));
                        using (var output = File.Create(iniPath))
                        {
                            var writer = new StreamWriter(output);
                            writer.Write(JsonConvert.SerializeObject(
                                file, Newtonsoft.Json.Formatting.Indented));
                            writer.Flush();
                        }
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    using (var output = File.Create(Path.Combine(outputPath, "@coalesced.json")))
                    {
                        var writer = new StreamWriter(output);
                        writer.Write(JsonConvert.SerializeObject(
                            setup, Newtonsoft.Json.Formatting.Indented));
                        writer.Flush();
                    }
                }
            }
            else
            {
                var inputPath = extras[0];
                var outputPath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, ".bin");

                Setup setup;

                using (var input = File.OpenRead(Path.Combine(inputPath, "@coalesced.json")))
                {
                    var reader = new StreamReader(input);
                    var text = reader.ReadToEnd();
                    setup = JsonConvert.DeserializeObject<Setup>(text);
                }

                var coal = new CoalescedFile();
                coal.Endian = setup.Endian;
                coal.Version = setup.Version;
                
                foreach (var iniName in setup.Files)
                {
                    string iniPath = Path.IsPathRooted(iniName) == false ?
                        Path.Combine(inputPath, iniName) : iniName;

                    using (var input = File.OpenRead(iniPath))
                    {
                        var reader = new StreamReader(input);
                        var text = reader.ReadToEnd();
                        var file = JsonConvert.DeserializeObject<Coalesced.File>(text);
                        coal.Files.Add(file);
                    }
                }

                using (var output = File.Create(outputPath))
                {
                    coal.Serialize(output);
                }
            }
        }
    }
}
