using Android.App;
using Android.OS;
using Symbol.XamarinEMDK.Barcode;
using System.Collections.Generic;
using Android.Widget;
using System.Linq;
using Symbol.XamarinEMDK;

namespace Com.Apacheta.BarcodeSample
{
    [Activity(Label = "FirstActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FirstActivity : Activity, EMDKManager.IEMDKListener
    {
        private ScannerEmdk scannerUtil;
        private TextView tvScannerStatus;
        private TextView tvScannedBarcodes;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FirstActivity);
            scannerUtil = new ScannerEmdk(this);

            tvScannedBarcodes = FindViewById<TextView>(Resource.Id.tvBarcodes);
            tvScannerStatus = FindViewById<TextView>(Resource.Id.tvScannerStatus);


            tvScannerStatus.Text = "Scanner Status: Scanner Initialized";
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            scannerUtil?.Destroy();
            scannerUtil = null;
        }
        protected override void OnPause()
        {
            base.OnPause();

            if (scannerUtil != null)
            {
                scannerUtil.ScanDataReceived -= ScanDataReceived;
            }

            scannerUtil?.Pause();

            tvScannerStatus.Text = "Scanner Status: Scanner disabled";
        }

        protected override void OnResume()
        {
            base.OnResume();

            scannerUtil?.InitializeScanner();

            if (scannerUtil != null)
            {
                scannerUtil.ScanDataReceived += ScanDataReceived;
            }
            tvScannerStatus.Text = "Scanner Status: Ready to scan";
        }

        private void ScanDataReceived(object sender, IList<ScanDataCollection.ScanData> scanDataList)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "Scanner Status: Scanned Barcode(s) successfully", ToastLength.Short).Show();
                tvScannedBarcodes.Text += scanDataList.Select(d => d.Data)?.Aggregate((first, next) => first + ", " + next) + System.Environment.NewLine;
            });
        }

        #region IEMDKListener
        public void OnClosed()
        {
            tvScannerStatus.Text = "Scanner Status: Scanner connection closed";
            scannerUtil.Destroy();
        }

        public void OnOpened(EMDKManager emdkManager)
        {

            tvScannerStatus.Text = "Scanner Status: Scanner connection opened";
            scannerUtil.EmdkManager = emdkManager;
            scannerUtil.InitializeScanner();
        }
        #endregion

    }
}