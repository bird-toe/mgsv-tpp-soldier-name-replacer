using Avalonia.Controls;
using MgsvTppSoldierNameReplacer.ViewModels;
using System;

namespace MgsvTppSoldierNameReplacer.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public void UpdateNames(object sender, TextChangedEventArgs args)
    {
        var context = DataContext as MainViewModel;
        
        context.Names = context.ListOfNames.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

        //Create Warnings
        context.NameWarnings.Clear();

        if (context.Names.Length < MainViewModel.SOFT_MINIMUM_NUMBER_OF_NAMES) // The soft minimum is based on the number of soliders in the random names pool. Add a warning if the list is less than this amount as players will see many many repeats.
        {
            context.NameWarnings.Add($"List has less than {MainViewModel.SOFT_MINIMUM_NUMBER_OF_NAMES} names. If more aren't added, there will be an excessive number of repeated names in game.");
        }

        if (context.Names.Length > MainViewModel.MAXIMUM_NUMBER_OF_NAMES) // This is the number of entries in the list that can be replaced. Add a warning if the list is more than this as it will ignore extras.
        {
            context.NameWarnings.Add($"List has less than {MainViewModel.MAXIMUM_NUMBER_OF_NAMES} names. Only the first {MainViewModel.MAXIMUM_NUMBER_OF_NAMES} will be included in the mod. Any after that will be ignored.");
        }

        foreach (var name in context.Names) //Lets do this type of warning last since there may be repeated entries.
        {
            if (!MainViewModel.IsLatin1(name)) //If a name has characters outside Latin-1, they probably won't display, so give a warning.
            {
                context.NameWarnings.Add($"Entry {name} contains non-latin characters which may not display correctly in game.");
            }
        }
    }
}
