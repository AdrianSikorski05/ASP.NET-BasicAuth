﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace AplikacjaAndroid
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Przed base.OnCreate
            System.Environment.SetEnvironmentVariable("MONO_GC_PARAMS", "nursery-size=64m,major=marksweep-conc");

            base.OnCreate(savedInstanceState);
        }
    }
}
