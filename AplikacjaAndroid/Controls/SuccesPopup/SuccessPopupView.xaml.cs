using Mopups.Pages;
using Mopups.Services;
using SkiaSharp.Extended.UI.Controls;

namespace AplikacjaAndroid;
public partial class SuccessPopupView : PopupPage
{
	private AnimationType _animationType;
	public string AnimationName { get; set; }
	public SuccessPopupView(AnimationType animationType)
	{
		_animationType = animationType;
        LoadAnimationName();

		InitializeComponent();

        Animation.Source = (SKLottieImageSource)SKLottieImageSource.FromFile(AnimationName);
        Animation.AnimationCompleted += async (s, e) =>
		{
            await Task.Delay(500);
            await MopupService.Instance.PopAllAsync();
        };
    }

	private void LoadAnimationName() 
	{
		if (_animationType == AnimationType.Check)
		{
			AnimationName = "check.json";
		}
		else if (_animationType == AnimationType.Loading)
		{
			AnimationName = "Loading.json";
        }
    }
}