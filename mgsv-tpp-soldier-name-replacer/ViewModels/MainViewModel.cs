using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MgsvTppSoldierNameReplacer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using Avalonia.Platform;
using Avalonia;
using System.IO;
using System.Xml.Serialization;
using LangTool.Lang;
using System.Xml;
using System.Diagnostics;
using Avalonia.Platform.Storage;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Collections.ObjectModel;

namespace MgsvTppSoldierNameReplacer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public const int SOFT_MINIMUM_NUMBER_OF_NAMES = 61;
    public const int MAXIMUM_NUMBER_OF_NAMES = 296;
    private const string TPP_FOLDER_STRUCTURE = "\\Assets\\tpp\\pack\\ui\\lang\\lang_default_data_eng_fpk\\Assets\\tpp\\lang\\ui";

    [ObservableProperty] 
    public string _makeBiteFileLocation = "";
    [ObservableProperty]
    public string _saveFolderLocation = "";
    [ObservableProperty]
    public string _modName = "MyCustomNames";
    [ObservableProperty]
    public string _listOfNames = "";
    
    public string[] Names = [];

    [ObservableProperty]
    public ObservableCollection<string> _nameWarnings = new ObservableCollection<string>();
    [ObservableProperty]
    public ObservableCollection<string> _errorMessages = new ObservableCollection<string>();

    [RelayCommand]
    private async Task SelectMakeBite()
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>();
        if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

        string defaultMakeBiteLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SnakeBite");

        var file = await filesService.SelectFileAsync("Select MakeBite Executable", false, defaultMakeBiteLocation);
        if (file == null) return;
        
        MakeBiteFileLocation = file.Path.LocalPath;
    }

    [RelayCommand]
    private async Task SelectSaveFolder()
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>();
        if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

        var folder = await filesService.SelectFolderAsync("Select Save Folder", false, null);
        if (folder == null) return;

        SaveFolderLocation = folder.Path.LocalPath;
    }
    
    [RelayCommand]
    private async Task CreateMod()
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>();
        if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

        ErrorMessages.Clear();
        if(Names.Length == 1)
        {
            ErrorMessages.Add("No names provided in list.");
            return;
        }
        try
        {
            // It's just a couple small files, memory isn't an issue, right?
            string xml;
            string csv;
            using (var xmlFile = AssetLoader.Open(new Uri("avares://MgsvTppSoldierNameReplacer/Assets/motherBaseNames.xml")))
            using (var xmlReader = new StreamReader(xmlFile))
            {
                xml = await xmlReader.ReadToEndAsync();
            }
            using (var csvFile = AssetLoader.Open(new Uri("avares://MgsvTppSoldierNameReplacer/Assets/motherBaseNames.csv")))
            using (var csvReader = new StreamReader(csvFile))
            {
                csv = await csvReader.ReadToEndAsync();
            }

            bool incrementing = true;
            int j = 0;
            for (int i = 1; i <= MAXIMUM_NUMBER_OF_NAMES; i++)
            {
                string nameToReplace;

                // We read from the list forwards, then backwards, repeating until we have replaced all names in the template file. Since the tempalte files
                // have the name placeholders saved in order where $NAME1$ is the most common and $NAME296$ is the least, this distributes names across soldiers almost optimally
                if(incrementing)
                {
                    if(j == Names.Length)
                    {
                        incrementing = false;
                        j--; //Set it to and use the last name in the list
                        nameToReplace = Names[j];
                        j--;
                    } else {
                        nameToReplace = Names[j];
                        j++;
                    }
                } else
                {
                    if (j < 0)
                    {
                        incrementing = true;
                        j++; //Set it to and use the first name in the list
                        nameToReplace = Names[j];
                        j++;
                    }
                    else
                    {
                        nameToReplace = Names[j];
                        j--;
                    }
                }

                xml = xml.Replace($"$NAME{i}$", nameToReplace);
                csv = csv.Replace($"$NAME{i}$", nameToReplace.Replace("\"", "\"\"")); //Records will already be double quoted, so all we need to do is escpae any double quotes
            }



            //This is only ever going to be a desktop app anyway, so forgive me for using File
            //Save the lang file

            // We will use this temp path as teh project folder for MakeBite
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory + TPP_FOLDER_STRUCTURE);
            using (var xmlReader = new StringReader(xml))
            using (var outputLangFile = File.Create(tempDirectory + TPP_FOLDER_STRUCTURE + "\\mb_staff_name.eng.lng2"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LangFile));
                LangFile file = serializer.Deserialize(xmlReader) as LangFile;
                if (file == null)
                {
                    throw new Exception("Could not convert to Lang File.");
                }

                file.Write(outputLangFile);
            }

            //Save the metadata.xml
            using (var metadata = AssetLoader.Open(new Uri("avares://MgsvTppSoldierNameReplacer/Assets/metadata.xml")))
            using (var outputMetadata = File.Create(Path.Combine(tempDirectory, "metadata.xml")))
            {
                await metadata.CopyToAsync(outputMetadata);
            }

            //Create Mod using MakeBite
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = MakeBiteFileLocation,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"\"{tempDirectory}\""
            };

            using (Process makeBiteExe = Process.Start(startInfo))
            {
                makeBiteExe?.WaitForExit();
            }

            //Copy the mod to the save folder
            File.Copy(Path.Combine(tempDirectory, "mod.mgsv"), Path.Combine(SaveFolderLocation, $"{ModName}.mgsv"), true);

            //Save csv
            using (var outputCsv = File.Create(Path.Combine(SaveFolderLocation, $"{ModName}Mapping.csv")))
            using (var outputCsvStreamWriter = new StreamWriter(outputCsv))
            {
                outputCsvStreamWriter.Write(csv);
            }

            Directory.Delete(tempDirectory, true);

            var resultBox = MessageBoxManager
            .GetMessageBoxStandard("Success!", 
@$"A mod file has been created: 
    {ModName}.mgsv.
Additionally, file showing where you can find each soldier has been created: 
    {ModName}Mapping.csv",
                ButtonEnum.Ok);

            await resultBox.ShowAsync();
        }
        catch (Exception ex) {
            ErrorMessages.Add($"{ex.Message}");
        }
    }

    public void UpdateNames(object sender, RoutedEventArgs args)
    {
        Names = ListOfNames.Split('\n', StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

        //Create Warnings
        NameWarnings.Clear();

        if (Names.Count() < SOFT_MINIMUM_NUMBER_OF_NAMES) // The soft minimum is based on the number of soliders in the random names pool. Add a warning if the list is less than this amount as players will see many many repeats.
        {
            NameWarnings.Add($"List has less than {SOFT_MINIMUM_NUMBER_OF_NAMES} names. If more aren't added, there will be an excessive number of repeated names in game.");
        }

        if (Names.Count() > MAXIMUM_NUMBER_OF_NAMES) // This is the number of entries in the list that can be replaced. Add a warning if the list is more than this as it will ignore extras.
        {
            NameWarnings.Add($"List has less than {MAXIMUM_NUMBER_OF_NAMES} names. Only the first {MAXIMUM_NUMBER_OF_NAMES} will be included in the mod. Any after that will be ignored.");
        }

        foreach (var name in Names) //Lets do this type of warning last since there may be repeated entries.
        {
            if (!IsLatin1(name)) //If a name has characters outside Latin-1, they probably won't display, so give a warning.
            {
                NameWarnings.Add($"Entry {name} contains non-latin characters which may not display correctly in game.");
            }
        }
    }

    public static bool IsLatin1(string input)
    {
        byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
        string result = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
        return string.Equals(input, result);
    }
}
