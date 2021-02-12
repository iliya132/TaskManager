using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManager_redesign.Controls
{
    /// <summary>
    /// Interaction logic for StructuresSelector.xaml
    /// </summary>
    public partial class StructuresSelector : UserControl
    {
        const string DROP_DOWN_CLOSED = @"..\res\dropdown2.png";
        const string DROP_DOWN_OPENED = @"..\res\dropdown.png";
        private bool isDFMExpanded = false;
        private bool isUKKOExpanded = false;
        private bool isUOKIOExpanded = false;
        private bool isUKPBPExpanded = false;
        private bool isUMKExpanded = false;
        private bool isUSPxpanded = false;
        public StructuresSelector()
        {
            InitializeComponent();
        }

        private void DFMBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isDFMExpanded)
            {
                dfmDropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                DFM_DropDown.Visibility = Visibility.Collapsed;
                isDFMExpanded = false;
            }
            else
            {

                dfmDropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));
                DFM_DropDown.Visibility = Visibility.Visible;
                isDFMExpanded = true;
            }
        }

        private void UKKOBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isUKKOExpanded)
            {
                UkkoDropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                UKKO_DropDown.Visibility = Visibility.Collapsed;
                isUKKOExpanded = false;
            }
            else
            {

                UkkoDropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));
                UKKO_DropDown.Visibility = Visibility.Visible;
                isUKKOExpanded = true;
            }
        }

        private void UOKIOBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isUOKIOExpanded)
            {
                UOKIOImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                UOKIO_DropDown.Visibility = Visibility.Collapsed;
                isUOKIOExpanded = false;
            }
            else
            {

                UOKIOImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));
                UOKIO_DropDown.Visibility = Visibility.Visible;
                isUOKIOExpanded = true;
            }
        }

        private void UKPBPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isUKPBPExpanded)
            {
                UKPBPImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                UKPBP_DropDown.Visibility = Visibility.Collapsed;
                isUKPBPExpanded = false;
            }
            else
            {

                UKPBPImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));
                UKPBP_DropDown.Visibility = Visibility.Visible;
                isUKPBPExpanded = true;
            }
        }

        private void UMKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isUMKExpanded)
            {
                UMKImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                UMK_DropDown.Visibility = Visibility.Collapsed;
                isUMKExpanded = false;
            }
            else
            {

                UMKImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));
                UMK_DropDown.Visibility = Visibility.Visible;
                isUMKExpanded = true;
            }
        }

        private void USPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isUSPxpanded)
            {
                USPImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                USP_DropDown.Visibility = Visibility.Collapsed;
                isUSPxpanded = false;
            }
            else
            {

                USPImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));
                USP_DropDown.Visibility = Visibility.Visible;
                isUSPxpanded = true;
            }
        }

        private void SelectorBtnClick(object sender, RoutedEventArgs e)
        {
            if(!(sender is Button btnSender))
            {
                return;
            }
            if (btnSender.Tag != null && btnSender.Tag.Equals("Pressed"))
            {
                btnSender.Tag = string.Empty;
            }
            else
            {
                btnSender.Tag = "Pressed";
            }
        }
    }
}
