using Microsoft.Maui.Handlers;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

#nullable enable

public partial class EntityNotesView : IcmRecordContentView<EntityNotesViewModel>, IRequestedEntitySection
{
    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }

    public EntityNotesView()
        : base(ServiceProvider.GetService<EntityNotesViewModel>(), LocalizedStrings.Notes)
    {
        InitializeComponent();
        BindingContext = ViewModel;
        EditorHandler.Mapper.AppendToMapping(
            "NoBorder",
            (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif

#if IOS || MACCATALYST
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.Layer.CornerRadius = 0;
#endif

#if WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            }
        );
    }

    private async void NotesCollectionView_Loaded(object? sender, EventArgs e)
    {
        await ViewModel.notesLoadedTcs.Task;

        ScrollToItem(ViewModel.LastNoteItem, ViewModel.LastNoteItemGroup);
    }

    private void ScrollToItem(NoteItem? item, NoteItemGroup? noteItemGroup)
    {
        if (item != null && noteItemGroup != null)
            NotesCollectionView.ScrollTo(item, noteItemGroup, position: ScrollToPosition.End, animate: false);
    }
}
