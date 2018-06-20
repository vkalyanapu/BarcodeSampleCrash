using System;
using System.Collections.Generic;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using System.Linq;
using Android.Util;

namespace Com.Apacheta.BarcodeSample
{
    public class ScannerEmdk
    {
        public EMDKManager EmdkManager;
        public EventHandler<IList<ScanDataCollection.ScanData>> ScanDataReceived;

        private BarcodeManager barcodeManager = null;
        private Scanner scanner = null;
        private bool scannerEventsWired = false;

        private string TAG = "ScannerUtil";


        public ScannerEmdk(EMDKManager.IEMDKListener listener)
        {
            Log.Debug(TAG, "scannerUtil is initialized");
            //The EMDKManager object will be created asynchronously, and returned in OnOpen callback.
            EMDKResults results = EMDKManager.GetEMDKManager(Android.App.Application.Context, listener);
            if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
                throw new Exception("Barcode scanner could not be initialized.");
        }


        private void Scanner_Status(object sender, Scanner.StatusEventArgs statusData)
        {
            try
            {
              Log.Debug(TAG, $"Scanner status - {statusData.P0.State}");
                if (statusData.P0.State == StatusData.ScannerStates.Idle)
                {
                    if (scanner.IsEnabled && !scanner.IsReadPending)
                    {
                        scanner.Read();
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Debug(TAG, "Scanner_Status Exception", ex);
            }
        }

        private void Scanner_Data(object sender, Scanner.DataEventArgs data)
        {
            Log.Debug(TAG, $"Scanned Barcode(s): {data.P0.GetScanData()?.Select(d => d.Data)?.Aggregate((first, next) => first +", " + next)}");
            ScanDataReceived?.Invoke(sender, data.P0.GetScanData());
        }

        public void InitializeScanner()
        {
            if (scanner == null && EmdkManager != null)
            {
                try
                {
                    barcodeManager = (BarcodeManager)EmdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);
                    scanner = barcodeManager?.GetDevice(BarcodeManager.DeviceIdentifier.Default);
                }
                catch (Exception ex)
                {
                    scanner = null;
                    Log.Debug(TAG, "Scanner Initialize Exception", ex);
                }
            }

            if (scanner != null && !scannerEventsWired)
            {
                try
                {
                    scanner.Status += Scanner_Status;
                    scanner.Data += Scanner_Data;
                    scanner.Enable();
                    scannerEventsWired = true;
                    Log.Debug(TAG, "Scanner events are wired successfully.");
                }
                catch(Exception ex)
                {
                    Log.Debug(TAG, "Scanner Initialize Exception", ex);
                }
            }
        }

        public void UninitializeScanner()
        {
            if (scanner != null)
            {
                try
                {
                    if (scannerEventsWired)
                    {
                        scanner.CancelRead();
                        scanner.Disable();

                        scanner.Data -= Scanner_Data;
                        scanner.Status -= Scanner_Status;
                        scannerEventsWired = false;
                        Log.Debug(TAG, "Scanner events are unwired successfully.");
                    }

                }
                catch (Exception ex)
                {
                    Log.Debug(TAG, "Scanner Uninitialize Exception", ex);
                }
            }
        }

        public void Pause()
        {
            try
            {
                UninitializeScanner();
            }
            catch(Exception ex)
            {
                Log.Debug(TAG, "Scanner crash on Pause", ex);
            }
        }

        public void Destroy()
        {
            try
            {
                Log.Debug(TAG, "Disposing scanning objects.");
                UninitializeScanner();

                scanner?.Release();
                scanner?.Dispose();
                scanner = null;

                ScanDataReceived = null;
                barcodeManager = null;
                EmdkManager?.Release();
                EmdkManager = null;
            }
            catch(Exception ex)
            {
                Log.Debug(TAG, "Scanner crash on Destroy", ex);
            }
        }
    }
}