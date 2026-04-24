using CommunityToolkit.Maui.Views;
using Oidc;
using Oidc.Events;

namespace Visitz.Behaviors;

internal class AvatarSessionBehavior : Behavior<AvatarView>
{
    private AvatarView _avatarView;

    protected override async void OnAttachedTo(AvatarView bindable)
    {
        base.OnAttachedTo(bindable);
        _avatarView = bindable;

        await SetInitials();
        OidcSession.SessionChanged += VisitzSession_SessionChanged;
    }

    protected override void OnDetachingFrom(AvatarView bindable)
    {
        OidcSession.SessionChanged -= VisitzSession_SessionChanged;

        base.OnDetachingFrom(bindable);
    }

    private async void VisitzSession_SessionChanged(object? sender, SessionChangedEventArgs e)
    {
        await SetInitials();
    }

    private async Task SetInitials()
    {
        var info = await OidcSessionInfo.GetAsync();

        _avatarView.Text = info.UserInitials ?? "--";
    }
}
