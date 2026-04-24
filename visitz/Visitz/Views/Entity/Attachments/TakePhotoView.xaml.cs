using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Visitz.Animations;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Attachments;

public partial class TakePhotoView : IcmRecordContentView<TakePhotoViewModel>
{
    readonly VisibilityAnimation SnapshotFade = new(showView: false);

    public TakePhotoView()
        : base(ServiceProvider.GetService<TakePhotoViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override Task InitAsync()
    {
        Task init = base.InitAsync();

        Unloaded += TakePhotoView_Unloaded;
        Camera.MediaCaptured += Camera_MediaCaptured;
        Camera.MediaCaptureFailed += Camera_MediaCaptureFailed;

        return init;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Camera.StopCameraPreview();
            Camera.Handler.DisconnectHandler();

            Camera.MediaCaptured -= Camera_MediaCaptured;
            Camera.MediaCaptureFailed -= Camera_MediaCaptureFailed;

            ViewModel.Dispose();

            Unloaded -= TakePhotoView_Unloaded;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void TakePhotoView_Unloaded(object? sender, EventArgs e)
    {
        Dispose();
    }

    private async Task AnimateSnapshotAsync()
    {
        SnapshotLayer.IsVisible = true;
        await Task.Delay(150);
        await SnapshotFade.Animate(SnapshotLayer, CancellationToken.None);
    }

    private async void Camera_MediaCaptured(object? sender, MediaCapturedEventArgs e)
    {
        try
        {
            await ViewModel.SavePicture(e.Media);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            Logger.LogError(ex, ex.Message);
        }
    }

    private void Camera_MediaCaptureFailed(object? sender, MediaCaptureFailedEventArgs e)
    {
        Logger.LogError($"{nameof(Camera_MediaCaptureFailed)} " + e.FailureReason);
        // TODO: Show error when info is added to MediaCaptureFailedEventArgs
        // await Navigator.CurrentOpenPage.DisplayErrorAlert(e...);
    }

    private async void CameraRollButton_Clicked(object? sender, EventArgs e)
    {
        await Navigator.Navigation.PopModalAsync();
    }

    public static async Task TryOpenWithPermissionsAsync(IBusinessObject businessObject)
    {
        var status = await DevicePermissions.PromptEnsureCameraAsync();

        if (status == PermissionStatus.Granted)
        {
            TakePhotoView photoView = new() { BusinessObject = businessObject };
            await Navigator.Navigation.PushModalAsync(photoView, ViewModalSize.Fullscreen);
        }
        else
        {
#if WINDOWS
            SnackbarHandler.ShowTextWithDetails(
                LocalizedStrings.NoCameraMicrophonePermissionsPrompt,
                LocalizedStrings.NoCameraMicrophonePermissionsPrompt,
                LocalizedStrings.PhotoPermissionsErrorDesc
            );
#else
            SnackbarHandler.ShowTextWithDetails(
                LocalizedStrings.NoCameraPermissionsPrompt,
                LocalizedStrings.NoCameraPermissionsPrompt,
                LocalizedStrings.NoCameraPermissionsDetailMessage
            );
#endif
        }
    }

    [RelayCommand]
    public async Task TakePicture()
    {
        try
        {
            CameraRollButton.IsEnabled = false;
            CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));

            await Task.WhenAll(AnimateSnapshotAsync(), Camera.CaptureImage(cts.Token));
        }
        catch (TaskCanceledException)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(
                "Took too long to save picture and process was canceled."
            );
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            Logger.LogError(ex, ex.Message);
        }
        finally
        {
            CameraRollButton.IsEnabled = true;
        }
    }
}
