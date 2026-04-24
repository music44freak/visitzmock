using Foundation;
using UIKit;

namespace Visitz.Device
{
    public partial class SoftKeyboardOpenHandler
    {
        private NSObject _keyboardWillShowToken;
        private NSObject _keyboardWillHideToken;

        partial void SubscribeToKeyboardEvents()
        {
            _keyboardWillShowToken = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
            _keyboardWillHideToken = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);
        }

        partial void UnsubscribeFromKeyboardEvents()
        {
            _keyboardWillShowToken.Dispose();
            _keyboardWillHideToken.Dispose();
        }

        private void OnKeyboardWillShow(object? sender, UIKeyboardEventArgs e)
        {
            OnKeyboardStateChanged(true);
        }

        private void OnKeyboardWillHide(object? sender, UIKeyboardEventArgs e)
        {
            OnKeyboardStateChanged(false);
        }
    }
}
