using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android;

namespace Com.Apacheta.BarcodeSample
{
    [Activity(Label = "BarcodeSample", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private Button btnFirstActivity;
        private Button btnSecondActivity;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            btnFirstActivity = FindViewById<Button>(Resource.Id.btnFirstActivity);
            btnSecondActivity = FindViewById<Button>(Resource.Id.btnSecondActivity);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
            btnFirstActivity.Click -= BtnFirstActivity_Click;
            btnSecondActivity.Click -= BtnSecondActivity_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            btnFirstActivity.Click += BtnFirstActivity_Click;
            btnSecondActivity.Click += BtnSecondActivity_Click;
        }

        private void BtnSecondActivity_Click(object sender, EventArgs e)
        {

            var secondActivityIntent = new Intent(this, typeof(SecondActivity));
            StartActivity(secondActivityIntent);
        }

        private void BtnFirstActivity_Click(object sender, EventArgs e)
        {
            var firstActivityIntent = new Intent(this, typeof(FirstActivity));
            StartActivity(firstActivityIntent);
        }
    }
}

