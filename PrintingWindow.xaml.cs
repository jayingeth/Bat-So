using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatSo
{
    public partial class PrintingWindow : Window
    {
        private readonly MainWindow _mainWindow;
        public PrintingWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NgayGiohienTai.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                SoThuTu.Text = _mainWindow.SoThuTu.Text;
                if (_mainWindow.ThongTinThe.Text.Equals("ƯU TIÊN") || _mainWindow.ThongTinThe.Text.Equals("KHÔNG ƯU TIÊN"))
                {
                    ThongTinThe.FontSize = 24;
                } else
                {
                    ThongTinThe.FontSize = 10;
                }
                ThongTinThe.Text = _mainWindow.ThongTinThe.Text;
                TenBenhNhan.Text = _mainWindow.TenBenhNhan.Text;
                //SoThe.Text = _mainWindow.SoThe.Text;
                SoThe.Text = "";
                UuTien.Text = _mainWindow.textUuTien.Text;

                await Task.Delay(100);
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintVisual(PrintArea, "STT");
                
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
