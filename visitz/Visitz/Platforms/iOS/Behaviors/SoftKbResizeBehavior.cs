// Adapted from https://developer.apple.com/documentation/uikit/uiresponder/1621578-keyboardframeenduserinfokey

using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Visitz.Behaviors;

public partial class SoftKbResizeBehavior
{
    UIView UIView { get; set; }

    NSObject ObserveWillChangeFrameToken { get; set; }

    partial void Attach()
    {
        UIView = (View.Handler as ViewHandler).PlatformView;

        ObserveWillChangeFrameToken = UIKeyboard.Notifications.ObserveWillChangeFrame(OnKeyboardWillChangeFrame);
    }

    partial void Detach()
    {
        ObserveWillChangeFrameToken.Dispose();
    }

    void OnKeyboardWillChangeFrame(object? sender, UIKeyboardEventArgs e)
    {
        var intersection = CGRect.Intersect(UIView.Frame, e.FrameEnd);

        if (intersection.IsEmpty)
        {
            View.HeightRequest = -1;
            View.VerticalOptions = LayoutOptions.Fill;
        }
        else
        {
            var uiViewFrameInWindowCoordinateSpace = UIView.CoordinateSpace.ConvertRectToCoordinateSpace(
                UIView.Frame,
                UIView.Window.CoordinateSpace
            );

            var viewMaxY = uiViewFrameInWindowCoordinateSpace.GetMaxY();
            var intMinY = intersection.GetMinY();

            var offset = viewMaxY - intMinY;

            View.HeightRequest = View.Height - offset;
            View.VerticalOptions = LayoutOptions.Start;
        }
    }
}
