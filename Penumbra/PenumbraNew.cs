using System;
using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OtterGui.Classes;
using OtterGui.Log;
using Penumbra.Api;
using Penumbra.Collections;
using Penumbra.GameData;
using Penumbra.GameData.Data;
using Penumbra.Interop;
using Penumbra.Interop.Loader;
using Penumbra.Interop.Resolver;
using Penumbra.Interop.Services;
using Penumbra.Mods;
using Penumbra.Services;
using Penumbra.UI;
using Penumbra.UI.Classes;
using Penumbra.UI.AdvancedWindow;
using Penumbra.UI.ModsTab;
using Penumbra.UI.Tabs;
using Penumbra.Util;
using ModFileSystemSelector = Penumbra.UI.ModsTab.ModFileSystemSelector;

namespace Penumbra;

public class PenumbraNew
{
    public string Name
        => "Penumbra";

    public static readonly Logger          Log = new();
    public readonly        ServiceProvider Services;

    public PenumbraNew(Penumbra penumbra, DalamudPluginInterface pi)
    {
        var       startTimer = new StartTracker();
        using var time       = startTimer.Measure(StartTimeType.Total);

        var services = new ServiceCollection();
        // Add meta services.
        services.AddSingleton(Log)
            .AddSingleton(startTimer)
            .AddSingleton<ValidityChecker>()
            .AddSingleton<PerformanceTracker>()
            .AddSingleton<FilenameService>()
            .AddSingleton<BackupService>()
            .AddSingleton<CommunicatorService>()
            .AddSingleton<ChatService>()
            .AddSingleton<SaveService>();

        // Add Dalamud services
        var dalamud = new DalamudServices(pi);
        dalamud.AddServices(services);
        services.AddSingleton(penumbra);

        // Add Game Data
        services.AddSingleton<IGamePathParser, GamePathParser>()
            .AddSingleton<IdentifierService>()
            .AddSingleton<StainService>()
            .AddSingleton<ItemService>()
            .AddSingleton<ActorService>();

        // Add Game Services
        services.AddSingleton<GameEventManager>()
            .AddSingleton<FrameworkManager>()
            .AddSingleton<MetaFileManager>()
            .AddSingleton<CutsceneCharacters>()
            .AddSingleton<CharacterUtility>()
            .AddSingleton<ResourceManagerService>()
            .AddSingleton<ResourceService>()
            .AddSingleton<FileReadService>()
            .AddSingleton<TexMdlService>()
            .AddSingleton<CreateFileWHook>()
            .AddSingleton<ResidentResourceManager>()
            .AddSingleton<FontReloader>()
            .AddSingleton<RedrawService>();

        // Add Configuration
        services.AddTransient<ConfigMigrationService>()
            .AddSingleton<Configuration>();

        // Add Collection Services
        services.AddTransient<IndividualCollections>()
            .AddSingleton<TempCollectionManager>()
            .AddSingleton<ModCollection.Manager>();

        // Add Mod Services
        services.AddSingleton<TempModManager>()
            .AddSingleton<Mod.Manager>()
            .AddSingleton<ModFileSystem>();

        // Add main services
        services.AddSingleton<ResourceLoader>()
            .AddSingleton<PathResolver>()
            .AddSingleton<CharacterResolver>()
            .AddSingleton<ResourceWatcher>();

        // Add Interface
        services.AddSingleton<FileDialogService>()
            .AddSingleton<TutorialService>()
            .AddSingleton<PenumbraChangelog>()
            .AddSingleton<LaunchButton>()
            .AddSingleton<ConfigWindow>()
            .AddSingleton<PenumbraWindowSystem>()
            .AddSingleton<ModEditWindow>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<SettingsTab>()
            .AddSingleton<ModsTab>()
            .AddSingleton<ModPanel>()
            .AddSingleton<ModFileSystemSelector>()
            .AddSingleton<ModPanelDescriptionTab>()
            .AddSingleton<ModPanelSettingsTab>()
            .AddSingleton<ModPanelEditTab>()
            .AddSingleton<ModPanelChangedItemsTab>()
            .AddSingleton<ModPanelConflictsTab>()
            .AddSingleton<ModPanelTabBar>()
            .AddSingleton<ModFileSystemSelector>()
            .AddSingleton<CollectionsTab>()
            .AddSingleton<ChangedItemsTab>()
            .AddSingleton<EffectiveTab>()
            .AddSingleton<DebugTab>()
            .AddSingleton<ResourceTab>()
            .AddSingleton<ConfigTabBar>()
            .AddSingleton<ResourceWatcher>()
            .AddSingleton<ItemSwapTab>();

        // Add Mod Editor
        services.AddSingleton<ModFileCollection>()
            .AddSingleton<DuplicateManager>()
            .AddSingleton<MdlMaterialEditor>()
            .AddSingleton<ModFileEditor>()
            .AddSingleton<ModMetaEditor>()
            .AddSingleton<ModSwapEditor>()
            .AddSingleton<ModNormalizer>()
            .AddSingleton<ModEditor>();

        // Add API
        services.AddSingleton<PenumbraApi>()
            .AddSingleton<IPenumbraApi>(x => x.GetRequiredService<PenumbraApi>())
            .AddSingleton<PenumbraIpcProviders>()
            .AddSingleton<HttpApi>();

        Services = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
    }

    public void Dispose()
    {
        Services.Dispose();
    }
}
