using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using MgsvTppSoldierNameReplacer.ViewModels;
using MgsvTppSoldierNameReplacer.Views;
using System;
using Microsoft.Extensions.DependencyInjection;
using MgsvTppSoldierNameReplacer.Services;

namespace MgsvTppSoldierNameReplacer;

public partial class App : Application
{

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFilesService>(x => new FilesService(desktop.MainWindow));

            Services = serviceCollection.BuildServiceProvider();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public new static App? Current => Application.Current as App;
}
