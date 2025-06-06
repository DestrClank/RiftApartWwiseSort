﻿// See https://aka.ms/new-console-template for more information

using System.Diagnostics.Contracts;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: RiftApartWwiseSort <extracted_soundbank_folder> <extracted_wem_folder> <output_directory>");
            return;
        }

        string extractedTocArg = args[0];
        string extractedWemArg = args[1];
        string outputDirArg = args[2];

        string extractedTocFolder = Path.GetFullPath(extractedTocArg);
        string extractedWemFolder = Path.GetFullPath(extractedWemArg);
        string outputDir = Path.GetFullPath(outputDirArg);

        if (extractedTocArg == null) {

            Console.WriteLine("Please provide the path to the extracted table of contents folder.");
            return;
        }

        if (extractedWemArg == null)
        {
            Console.WriteLine("Please provide the path to the extracted WEM folder.");
            return;
        }

        if (outputDirArg == null)
        {
            Console.WriteLine("Please provide the path to the output directory.");
            return;
        }

        if (!Directory.Exists(extractedTocFolder))
        {
            Console.WriteLine($"The extracted table of contents folder does not exist: {extractedTocFolder}");
            return;
        }
        if (!Directory.Exists(extractedWemFolder))
        {
            Console.WriteLine($"The extracted WEM folder does not exist: {extractedWemFolder}");
            return;
        }
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string[] folders = Directory.GetDirectories(extractedTocFolder);
        int folderCount = folders.Length;

        if (folderCount == 0)
        {
            Console.WriteLine($"No folders found in the extracted table of contents folder: {extractedTocFolder}");
            return;
        }

        var progress = 1;
        var parsed = 0;
        var skipped = 0;
        var errortxt = 0;
        var errorwemnotfound = 0;

        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            string[] txtFiles = Directory.GetFiles(folder, "*.txt");
            if (txtFiles.Length == 0)
            {
                Console.WriteLine($"({progress}/{folderCount}) No .txt files found in folder: {folder}");
                progress++;
                errortxt++;
                skipped++;
                continue;
            }
            string txtFile = txtFiles[0];
            string txtFileName = Path.GetFileNameWithoutExtension(txtFile);

            // Vérifier si le fichier WEM existe dans le dossier des WEM extraits
            string[] wemFiles = Directory.GetFiles(extractedWemFolder, $"{txtFileName}.wem");
            if (wemFiles.Length == 0)
            {
                Console.WriteLine($"({progress}/{folderCount}) Error: No WEM file found for {txtFileName} in {extractedWemFolder}.");
                progress++;
                skipped++;
                errorwemnotfound++;
                continue;
            }
            string wemFile = wemFiles[0];

            if (File.Exists(Path.Combine(outputDir, folderName + ".wem")))
            {
                Console.WriteLine($"({progress}/{folderCount}) Warning: {folderName}.wem already exists in {outputDir}. Skipping.");
                progress++;
                skipped++;
                continue;
            }

            File.Move(wemFile, Path.Combine(outputDir, folderName + ".wem"));

            Console.WriteLine($"({progress}/{folderCount}) Parsed {Path.GetFileName(wemFile)} to {folderName+".wem"}.");
            progress++;
            parsed++;
        }

        Console.WriteLine($"Parsed {parsed} files, skipped {skipped} files.");
        Console.WriteLine($"Errors: {errortxt} text files errors, {errorwemnotfound} WEM files not found.");

    }
}
