using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.Power;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BatteryCurrent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool reportRequested = false;
        public MainPage()
        {
            this.InitializeComponent();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
        }


        private void GetBatteryReport(object sender, RoutedEventArgs e)
        {
            // Clear UI
            BatteryReportPanel.Children.Clear();


            if (AggregateButton.IsChecked == true)
            {
                // Request aggregate battery report
                RequestAggregateBatteryReport();
            }
            else
            {
                // Request individual battery report
                RequestIndividualBatteryReports();
            }

            // Note request
            reportRequested = true;
        }

        private void RequestAggregateBatteryReport()
        {
            // Create aggregate battery object
            var aggBattery = Battery.AggregateBattery;

            // Get report
            var report = aggBattery.GetReport();

            // Update UI
            AddReportUI(BatteryReportPanel, report, aggBattery.DeviceId);
        }

        async private void RequestIndividualBatteryReports()
        {
            // Find batteries 
            var deviceInfo = await DeviceInformation.FindAllAsync(Battery.GetDeviceSelector());
            foreach (DeviceInformation device in deviceInfo)
            {
                try
                {
                    // Create battery object
                    var battery = await Battery.FromIdAsync(device.Id);

                    // Get report
                    var report = battery.GetReport();

                    // Update UI
                    AddReportUI(BatteryReportPanel, report, battery.DeviceId);
                }
                catch { /* Add error handling, as applicable */ }
            }
        }


        private void AddReportUI(StackPanel sp, BatteryReport report, string DeviceID)
        {
            // Create battery report UI
            TextBlock txt1 = new TextBlock { Text = "Device ID: " + DeviceID };
            txt1.FontSize = 15;
            txt1.Margin = new Thickness(0, 15, 0, 0);
            txt1.TextWrapping = TextWrapping.WrapWholeWords;

           
           
            batteryStatus.Text = report.Status.ToString();
            chargingRatemA.Text = (report.ChargeRateInMilliwatts / 3.8).ToString();
            chargingRatemW.Text = (report.ChargeRateInMilliwatts).ToString();
            fullEnergyCapacitymAH.Text = ((report.FullChargeCapacityInMilliwattHours)/3.8).ToString();
            fullEnergyCapacitymWH.Text = ((report.FullChargeCapacityInMilliwattHours)).ToString();

            remainingEnergyCapacitymWH.Text = (report.RemainingCapacityInMilliwattHours).ToString();
            remainingEnergyCapacitymAH.Text = ((report.RemainingCapacityInMilliwattHours) / 3.8).ToString();

            batteryPercent.Text = ((Convert.ToDouble(report.RemainingCapacityInMilliwattHours) / (Convert.ToDouble(report.FullChargeCapacityInMilliwattHours))) * 100).ToString("F2") + "%";
            timeToCharge.Text = ((((report.FullChargeCapacityInMilliwattHours) / 3.8) - ((report.RemainingCapacityInMilliwattHours) / 3.8))/(report.ChargeRateInMilliwatts/3.8)).ToString();
            // Create energy capacity progress bar & labels
            //TextBlock pbLabel = new TextBlock { Text = "Percent remaining energy capacity" };
            //pbLabel.Margin = new Thickness(0, 10, 0, 5);
            //pbLabel.FontFamily = new FontFamily("Segoe UI");
            //pbLabel.FontSize = 11;

            //ProgressBar pb = new ProgressBar();
            //pb.Margin = new Thickness(0, 5, 0, 0);
            //pb.Width = 200;
            //pb.Height = 10;
            //pb.IsIndeterminate = false;
            //pb.HorizontalAlignment = HorizontalAlignment.Left;

            //TextBlock pbPercent = new TextBlock();
            //pbPercent.Margin = new Thickness(0, 5, 0, 10);
            //pbPercent.FontFamily = new FontFamily("Segoe UI");
            //pbLabel.FontSize = 11;

            //// Disable progress bar if values are null
            //if ((report.FullChargeCapacityInMilliwattHours == null) ||
            //    (report.RemainingCapacityInMilliwattHours == null))
            //{
            //    pb.IsEnabled = false;
            //    pbPercent.Text = "N/A";
            //}
            //else
            //{
            //    pb.IsEnabled = true;
            //    pb.Maximum = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
            //    pb.Value = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
            //    pbPercent.Text = ((pb.Value / pb.Maximum) * 100).ToString("F2") + "%";
            //}

            //// Add controls to stackpanel
            
            //sp.Children.Add(pbLabel);
            //sp.Children.Add(pb);
            //sp.Children.Add(pbPercent);
        }

        async private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            if (reportRequested)
            {

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Clear UI
                    BatteryReportPanel.Children.Clear();


                    if (AggregateButton.IsChecked == true)
                    {
                        // Request aggregate battery report
                        RequestAggregateBatteryReport();
                    }
                    else
                    {
                        // Request individual battery report
                        RequestIndividualBatteryReports();
                    }
                });
            }
        }

        private void basicBatteryInfoRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            basicBatteryInfoGrid.Visibility = Visibility.Collapsed;
            batteryInfoGrid.Visibility = Visibility.Visible;
        }

        private void basicBatteryInfoTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            basicBatteryInfoGrid.Visibility = Visibility.Collapsed;
            batteryInfoGrid.Visibility = Visibility.Visible;
        }

        private void batteryInfoRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            basicBatteryInfoGrid.Visibility = Visibility.Visible;
            batteryInfoGrid.Visibility = Visibility.Collapsed;
        }

        private void batteryInfoTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            basicBatteryInfoGrid.Visibility = Visibility.Visible;
            batteryInfoGrid.Visibility = Visibility.Collapsed;
        }
    }

}
