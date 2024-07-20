using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace MessagingToolkit.Barcode.Demo.CommandLineRunner
{
    class Program
    {
        private static readonly Regex COMMA = new Regex(",");

        private static void PrintUsage()
        {
            Console.WriteLine("Decode barcode images using the barcode library");
            Console.WriteLine();
            Console.WriteLine("usage: CommandLineRunner { file | dir | url } [ options ]");
            Console.WriteLine("  --try_harder: Use the TRY_HARDER option, default is normal (mobile) mode");
            Console.WriteLine("  --pure_barcode: Input image is a pure monochrome barcode image, not a photo");
            Console.WriteLine("  --products_only: Only decode the UPC and EAN families of barcodes");
            Console.WriteLine("  --dump_results: Write the decoded contents to input.txt");
            Console.WriteLine("  --dump_black_point: Compare black point algorithms as input.mono.png");
            Console.WriteLine("  --multi: Scans image for multiple barcodes");
            Console.WriteLine("  --brief: Only output one line per file, omitting the contents");
            Console.WriteLine("  --recursive: Descend into subdirectories");
            Console.WriteLine("  --crop=left,top,width,height: Only examine cropped region of input image(s)");
        }

        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }
            Config config = new Config();
            
            foreach (string arg in args)
            {
                if ("--try_harder".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.TryHarder = true;
                }
                else if ("--pure_barcode".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.PureBarcode = true;
                }
                else if ("--products_only".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.ProductsOnly = true;
                }
                else if ("--dump_results".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.DumpResults = true;
                }
                else if ("--dump_black_point".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.DumpBlackPoint = true;
                }
                else if ("--multi".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.Multi = true;
                }
                else if ("--brief".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.Brief = true;
                }
                else if ("--recursive".Equals(arg, StringComparison.OrdinalIgnoreCase))
                {
                    config.Recursive = true;
                }
                else if (arg.StartsWith("--crop", StringComparison.OrdinalIgnoreCase))
                {
                    int[] crop = new int[4];
                    String[] tokens = COMMA.Split(arg.Substring(7));
                    for (int i = 0; i < crop.Length; i++)
                    {
                        crop[i] = Convert.ToInt32(tokens[i]);
                    }
                    config.Crop = crop;
                }
                else if (arg.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Error.WriteLine("Unknown command line option " + arg);
                    PrintUsage();
                    return;
                }
            }
            config.DecodeOptions = BuildHints(config);
            Inputs inputs = new Inputs();
            foreach (String arg in args)
            {
                if (!arg.StartsWith("--", StringComparison.OrdinalIgnoreCase))
                {
                    AddArgumentToInputs(arg, config, inputs);
                }
            }

            int numThreads = Environment.ProcessorCount;
            Dictionary<DecodeThread, Thread> threads = new Dictionary<DecodeThread, Thread>(numThreads);
            for (int x = 0; x < numThreads; x++)
            {
                DecodeThread decodeThread = new DecodeThread(config, inputs);
                Thread thread = new Thread(decodeThread.Run);
                threads.Add(decodeThread, thread);
                thread.Start();
            }

            int successful = 0;
            foreach (DecodeThread decodeThread in threads.Keys)
            {
                Thread thread = threads[decodeThread];
                thread.Join();
                successful += decodeThread.IsSuccessful();
            }
            int total = inputs.GetInputCount();
            if (total > 1)
            {
                Console.WriteLine("\nDecoded " + successful + " files out of " + total +
                    " successfully (" + (successful * 100 / total) + "%)\n");
            }

            //Console.ReadLine();
        }

        // Build all the inputs up front into a single flat list, so the threads can atomically pull
        // paths/URLs off the queue.
        private static void AddArgumentToInputs(String argument, Config config, Inputs inputs)
        {
            if (Directory.Exists(argument))
            {
                foreach (string file in Directory.GetFiles(argument))
                {
                    string fileName = file.ToLowerInvariant();

                    // Skip hidden files and directories (e.g. svn stuff).
                    if (fileName.StartsWith("."))
                    {
                        continue;
                    }

                    // Skip text files and the results of dumping the black point.
                    if (fileName.EndsWith(".txt") || fileName.Contains(".mono.png"))
                    {
                        continue;
                    }
                    inputs.AddInput(file);
                }

                // Recurse on nested directories if requested, otherwise skip them.
                if (config.Recursive)
                {
                    foreach (string dir in Directory.GetDirectories(argument))
                    {
                        AddArgumentToInputs(dir, config, inputs);
                    }
                }
            }
            else
            {
                inputs.AddInput(argument);
            }
        }

        // Manually turn on all formats, even those not yet considered production quality.
        private static Dictionary<DecodeOptions, object> BuildHints(Config config)
        {
           
            List<BarcodeFormat> formats = new List<BarcodeFormat>(8);
            formats.Add(BarcodeFormat.UPCA);
            formats.Add(BarcodeFormat.UPCE);
            formats.Add(BarcodeFormat.EAN13);
            formats.Add(BarcodeFormat.EAN8);
            formats.Add(BarcodeFormat.RSS14);
            formats.Add(BarcodeFormat.RSSExpanded);
            formats.Add(BarcodeFormat.MSIMod10);
            if (!config.ProductsOnly)
            {
                formats.Add(BarcodeFormat.Code39);
                formats.Add(BarcodeFormat.Code93);
                formats.Add(BarcodeFormat.Code128);
                formats.Add(BarcodeFormat.ITF14);
                formats.Add(BarcodeFormat.QRCode);
                formats.Add(BarcodeFormat.DataMatrix);
                formats.Add(BarcodeFormat.Aztec);
                formats.Add(BarcodeFormat.PDF417);
                formats.Add(BarcodeFormat.Codabar);
                formats.Add(BarcodeFormat.MaxiCode);
            }
            Dictionary<DecodeOptions, object> hints = new Dictionary<DecodeOptions, object>(1);
            hints.Add(DecodeOptions.PossibleFormats, formats);
            if (config.TryHarder)
            {
                hints.Add(DecodeOptions.TryHarder, true);
            }
            if (config.PureBarcode)
            {
                hints.Add(DecodeOptions.PureBarcode, true);
            }
            return hints;
        }
    }
}
