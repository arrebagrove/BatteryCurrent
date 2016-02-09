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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BatteryCurrent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
       
        private DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();
            //Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.Start();
                timer_Tick(null, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in stopping timer : " + ex);
            }
            base.OnNavigatingFrom(e);
        }

        private void timer_Tick(object sender, object e)
        {
            try
            {
                RequestAggregateBatteryReport();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in requesting aggregate battery report : " + ex);
            }
        }
        private void RequestAggregateBatteryReport()
        {
            // Create aggregate battery object
            var aggBattery = Battery.AggregateBattery;

            // Get report
            var report = aggBattery.GetReport();

            // Update UI
            AddReportUI(report, aggBattery.DeviceId);
        }

        
        private void AddReportUI(BatteryReport report, string DeviceID)
        {
            // Create battery report UI
            TextBlock txt1 = new TextBlock { Text = "Device ID: " + DeviceID };
            txt1.FontSize = 15;
            txt1.Margin = new Thickness(0, 15, 0, 0);
            txt1.TextWrapping = TextWrapping.WrapWholeWords;
            if((report.Status.ToString()).ToLower() == "charging")
            {
                batteryStateText.Text = "Charging";
                batteryStatus.Text = "Charging";
                toWhatState.Text = "to charge";
                double diff = ((((double)report.FullChargeCapacityInMilliwattHours) / 3.8) - (((double)report.RemainingCapacityInMilliwattHours) / 3.8));
                if (diff == 0)
                {
                    batteryStateText.Text = "Charged";
                    batteryStatus.Text = "Charged";
                    toWhatState.Opacity = 0;
                    timeToCharge.Text = "";
                    timeToCharge.Opacity = 0;
                }
                else
                {
                    double time_to_charge = Math.Abs( ((diff)) / ((double)report.ChargeRateInMilliwatts / 3.8));
                    string hours = ((int)time_to_charge).ToString();
                    string minutes = ((int)(time_to_charge * 60)).ToString();
                    if (hours == "0")
                    {
                        timeToCharge.Text = minutes + " min ";
                        timeInWords.Text = minutes + " min ";
                    }
                    else
                    {
                        timeToCharge.Text = hours + " h " + minutes + " min ";
                        timeInWords.Text = hours = " h " + minutes + " min ";
                    }
                }
                
            }
            else
            {
                batteryStateText.Text = report.Status.ToString();
                batteryStatus.Text = report.Status.ToString();
                toWhatState.Text = "to discharge";
                double time_to_charge = Math.Abs((((double)report.RemainingCapacityInMilliwattHours)/3.8) / ((double)report.ChargeRateInMilliwatts / 3.8));
                Debug.WriteLine("Dischrge : Time to discharge : " + time_to_charge);
                string hours = ((int)time_to_charge).ToString();
                string minutes = ((int)((time_to_charge - (int)time_to_charge)* 60)).ToString();
                if (hours == "0")
                {
                    timeToCharge.Text = minutes + " min ";
                    timeInWords.Text = minutes + " min ";
                }
                else
                {
                    timeToCharge.Text = hours + " h " + minutes + " min ";
                    timeInWords.Text = hours + " h " + minutes + " min ";
                }

            }
            chargingpercentage.Text = ((int)((Convert.ToDouble(report.RemainingCapacityInMilliwattHours) / (Convert.ToDouble(report.FullChargeCapacityInMilliwattHours))) * 100)).ToString();

            chargingRatemA.Text = (report.ChargeRateInMilliwatts / 3.8).ToString();
            chargingRatemW.Text = (report.ChargeRateInMilliwatts).ToString();
            fullEnergyCapacitymAH.Text = ((report.FullChargeCapacityInMilliwattHours)/3.8).ToString();
            fullEnergyCapacitymWH.Text = ((report.FullChargeCapacityInMilliwattHours)).ToString();

            remainingEnergyCapacitymWH.Text = (report.RemainingCapacityInMilliwattHours).ToString();
            remainingEnergyCapacitymAH.Text = ((report.RemainingCapacityInMilliwattHours) / 3.8).ToString();
            batteryPercent.Text = ((int)((Convert.ToDouble(report.RemainingCapacityInMilliwattHours) / (Convert.ToDouble(report.FullChargeCapacityInMilliwattHours))) * 100)).ToString("F2") + "%";
            
        }

       

        private void basicBatteryInfoRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Storyboard myStoryBoard;
            myStoryBoard = (Storyboard)this.Resources["RedButtonTapped"];
            myStoryBoard.Begin();
        }

        private void basicBatteryInfoTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Storyboard myStoryBoard;
            myStoryBoard = (Storyboard)this.Resources["RedButtonTapped"];
            myStoryBoard.Begin();
        }

        private void batteryInfoRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Storyboard myStoryBoard;
            myStoryBoard = (Storyboard)this.Resources["RedButtonPressedAgain"];
            myStoryBoard.Begin();
            
        }

        private void batteryInfoTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Storyboard myStoryBoard;
            myStoryBoard = (Storyboard)this.Resources["RedButtonPressedAgain"];
            myStoryBoard.Begin();
        }
    }

}
