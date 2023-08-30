using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Printing;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using BatSo.BUS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BatSo
{
    public partial class MainWindow : Window
    {
        //Khai báo các tham số cần thiến lấy từ App.config
        string MaDonVi = "";
        string URL = "";
        string CheckBHYT_Username = "";
        string CheckBHYT_Password = "";
        int LogoSize = 0;
        int LogoMarginTop = 0;
        int LogoMarginLeft = 0;
        int TenBenhVienSize = 0;
        int TenHeThongSize = 0;

        string DuLieuHienTai = "";

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            textBox.Focus();

            var appSettings = ConfigurationManager.AppSettings;
            if (appSettings != null)
            {
                MaDonVi = appSettings["MaDonVi"];
                URL = appSettings["ApiLuuThongTinBatSo"];
                CheckBHYT_Username = appSettings["CheckBHYT_Username"];
                CheckBHYT_Password = appSettings["CheckBHYT_Password"];
                LogoSize = Convert.ToInt32(appSettings["LogoSize"]);
                LogoMarginTop = Convert.ToInt32(appSettings["LogoMarginTop"]);
                LogoMarginLeft = Convert.ToInt32(appSettings["LogoMarginLeft"]);
                TenBenhVienSize = Convert.ToInt32(appSettings["TenBenhVienSize"]);
                TenHeThongSize = Convert.ToInt32(appSettings["TenHeThongSize"]);
            }

            textBox.SelectAll();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textBox.Focus();
            textBox.SelectAll();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        //Sự kiện của Window
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(6);
                TenBenhVien.FontSize = TenBenhVienSize;
                TenHeThong.FontSize = TenHeThongSize;
                

                Thickness margin = LogoBV.Margin;
                margin.Top = LogoMarginTop;
                margin.Left = LogoMarginLeft;
                LogoBV.Margin = margin;

                LogoBV.Height = LogoSize;
                LogoBV.Width = LogoSize;
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
                TenBenhVien.FontSize = 42;
                TenHeThong.FontSize = 36;
                

                Thickness margin = LogoBV.Margin;
                margin.Top = 0;
                LogoBV.Margin = margin;

                LogoBV.Height = 140;
                LogoBV.Width = 140;
            } 
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn thoát ứng dụng không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        //In STT
        private void PrintSTT()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                printDialog.PrintVisual(PrintContent, "STT");
            }
            catch (Exception)
            {
                MessageBox.Show("Có lỗi xảy ra khi in số thứ tự!", "Thống báo", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }

        private void PrintSTTV2(int loai)
        {
            if (loai == 0)
            {
                try
                {
                    PrintingWindow2 printingWindow2 = new PrintingWindow2(this);
                    printingWindow2.Show();

                    WindowInteropHelper helper = new WindowInteropHelper(printingWindow2);
                    IntPtr handle = helper.Handle;
                    NativeMethods.SetWindowPos(handle, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                }
                catch (Exception)
                {
                    MessageBox.Show("Có lỗi xảy ra khi in số thứ tự!", "Thống báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else if (loai == 1)
            {
                try
                {
                    PrintingWindow printingWindow = new PrintingWindow(this);
                    printingWindow.Show();

                    WindowInteropHelper helper = new WindowInteropHelper(printingWindow);
                    IntPtr handle = helper.Handle;
                    NativeMethods.SetWindowPos(handle, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                }
                catch (Exception)
                {
                    MessageBox.Show("Có lỗi xảy ra khi in số thứ tự!", "Thống báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        //Sự kiện chính
        private async void textBox_KeyDown(object sender, KeyEventArgs e)
        {   
            //Bắt sự kiện khi máy quét quét thẻ
            if (e.Key == Key.Enter)
            {
                //TextBox tbx = (TextBox)sender;
                string hienTai = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                if (textBox.Text.Length > 0)
                {
                    if (textBox.Text.Trim() == "1200") //Thẻ chứa chuỗi "1200": Không ưu tiên
                    {
                        try
                        {
                            textBox.Clear();
                            //Gửi API
                            var client = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Post, URL);
                            var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi + "\",\r\n    \"hotenbenhnhan\":\"\",\r\n    \"ngaysinh\": \"\",\r\n    \"gioitinh\":1,\r\n    \"diachi\":\"\",\r\n    \"matheBHYT\": \"\",\r\n    \"uutien\": 0\r\n}", null, "application/json");
                            request.Content = content;
                            var response = await client.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                            var jsonData = JToken.Parse(result.ToString());

                            string STT = jsonData[0]["sothutu"].ToString();

                            //Gán thông tin hiển thị
                            NgayGioHienTai.Text = hienTai;
                            SoThuTu.Text = STT;
                            ThongTinThe.Text = "KHÔNG ƯU TIÊN";
                            ThongTinThe.FontSize = 32;
                            textUuTien.Text = "";
                            TenBenhNhan.Text = "";
                            SoThe.Text = "";
                            TenBenhNhan.Visibility = Visibility.Hidden;
                            SoThe.Visibility = Visibility.Hidden;

                            textBox.Clear();
                            textBox.Focus();
                            textBox.SelectAll();

                            //In phiếu thứ tự
                            await Task.Delay(300);
                            PrintSTTV2(0);
                        }
                        catch (Exception ex)
                        {
                            textBox.Clear();
                            MessageBox.Show("Có lỗi xảy ra khi tạo số thường!" + Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (textBox.Text.Trim() == "1300") //Thẻ chứa chuỗi "1300": Ưu tiên
                    {
                        try
                        {
                            textBox.Clear();
                            //Gửi API
                            var client = new HttpClient();
                            var request = new HttpRequestMessage(HttpMethod.Post, URL);
                            var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi + "\",\r\n    \"hotenbenhnhan\":\"\",\r\n    \"ngaysinh\": \"\",\r\n    \"gioitinh\":1,\r\n    \"diachi\":\"\",\r\n    \"matheBHYT\": \"\",\r\n    \"uutien\": 1\r\n}", null, "application/json");
                            request.Content = content;
                            var response = await client.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                            var jsonData = JToken.Parse(result.ToString());

                            string STT = jsonData[0]["sothutu"].ToString();

                            //Gán thông tin hiển thị
                            NgayGioHienTai.Text = hienTai;
                            SoThuTu.Text = STT;
                            ThongTinThe.Text = "ƯU TIÊN";
                            ThongTinThe.FontSize = 48;
                            textUuTien.Text = "";
                            TenBenhNhan.Text = "";
                            SoThe.Text = "";
                            TenBenhNhan.Visibility = Visibility.Hidden;
                            SoThe.Visibility = Visibility.Hidden;
                            textBox.Clear();
                            textBox.Focus();
                            textBox.SelectAll();

                            //In phiếu thứ tự
                            await Task.Delay(300);
                            PrintSTTV2(1);
                        }
                        catch (Exception ex)
                        {
                            textBox.Clear();
                            MessageBox.Show("Có lỗi xảy ra khi tạo số ưu tiên!" + Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (textBox.Text.Trim() != "1200" && textBox.Text.Trim() != "1300")
                    {
                        //Check nếu quét 1 thẻ liên tục
                        string Input = textBox.Text.Trim();
                        int InputLength = Input.Split('$').Length;

                        if (!Input.Equals(DuLieuHienTai)) {
                            string noiDung = textBox.Text.Trim();
                            string[] arr = noiDung.Split('|');
                            string giaTriDau = arr[0];
                            string giaTriCuoi = arr[arr.Length - 1];
                            string tempName = null;

                            if (arr != null && arr.Length > 2)
                                tempName = arr[2];

                            if (giaTriCuoi != "$" && arr.Length == 7) //Thẻ CCCD
                            {
                                try
                                {
                                    int uutien = 0;
                                    string CCCD = arr[0];
                                    string ngaysinh = arr[3].Substring(0, 2) + "/" + arr[3].Substring(2, 2) + "/" + arr[3].Substring(4, 4);
                                    DateTime ngasinhFormat = DateTime.ParseExact(ngaysinh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    string gioitinh = arr[4];
                                    int gioitinhFormat = 1;
                                    if (gioitinh != "Nam")
                                        gioitinhFormat = 0;
                                    string diachi = arr[5];
                                    int tuoi = CalculateAge(ngasinhFormat);
                                    if (tuoi < 6 || tuoi >= 80)
                                        uutien = 1;
                                    DuLieuHienTai = "";
                                    DuLieuHienTai = textBox.Text;
                                    textBox.Clear();
                                    //Gửi API
                                    var client = new HttpClient();
                                    var request = new HttpRequestMessage(HttpMethod.Post, URL);
                                    //var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi  + "\",\r\n    \"hotenbenhnhan\":\"" + tempName + "\",\r\n    \"ngaysinh\": \"" + ngaysinh + "\",\r\n    \"gioitinh\":1,\r\n    \"diachi\":\"\",\r\n    \"matheBHYT\": \"" + CCCD + "\",\r\n    \"uutien\": " + uutien + "\r\n}", null, "application/json");
                                    var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi + "\",\r\n    \"hotenbenhnhan\":\"" + tempName + "\",\r\n    \"ngaysinh\": \"" + ngaysinh + "\",\r\n    \"gioitinh\":" + gioitinhFormat + ",\r\n    \"diachi\":\"" + diachi + "\",\r\n    \"cccd\":\"" + CCCD + "\",\r\n    \"matheBHYT\": \"\",\r\n    \"uutien\": \"" + uutien + "\"\r\n}", null, "application/json");
                                    request.Content = content;
                                    var response = await client.SendAsync(request);
                                    response.EnsureSuccessStatusCode();

                                    var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                                    var jsonData = JToken.Parse(result.ToString());

                                    string STT = jsonData[0]["sothutu"].ToString();

                                    //Gán thông tin hiển thị
                                    NgayGioHienTai.Text = hienTai;
                                    SoThuTu.Text = STT;
                                    if (uutien == 1)
                                    {
                                        ThongTinThe.Text = "ƯU TIÊN";
                                        ThongTinThe.FontSize = 48;
                                    }
                                    else
                                    {
                                        ThongTinThe.Text = "KHÔNG ƯU TIÊN";
                                        ThongTinThe.FontSize = 32;
                                    }

                                    textUuTien.Text = "";
                                    TenBenhNhan.Text = "Họ tên: " + tempName;
                                    SoThe.Text = "CCCD: " + giaTriDau;
                                    TenBenhNhan.Visibility = Visibility.Visible;
                                    SoThe.Visibility = Visibility.Hidden;
                                    textBox.Clear();
                                    textBox.Focus();
                                    textBox.SelectAll();

                                    //In phiếu thứ tự
                                    await Task.Delay(300);
                                    if (uutien == 1)
                                        PrintSTTV2(1);
                                    else
                                        PrintSTTV2(0);

                                }
                                catch (Exception ex)
                                {
                                    textBox.Clear();
                                    MessageBox.Show("Có lỗi xảy ra khi quét mã CCCD!" + Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else if (giaTriCuoi == "$" && InputLength == 2) //Thẻ BHYT
                            {
                                string ngaySinh = "";
                                DateTime ngaySinhFormat = DateTime.Now;
                                try
                                {
                                    //Kiểm tra thẻ BHYT
                                    string hoTenPost = ConvertHexStrToUnicode(arr[1]);
                                    ngaySinh = arr[2];
                                    if (ngaySinh.Length < 8)
                                        ngaySinh = "01/01/" + ngaySinh;

                                    ngaySinhFormat = DateTime.ParseExact(ngaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    int tuoi = CalculateAge(Convert.ToDateTime(ngaySinh));
                                    //string[] ketqua = checkHanThe(giaTriDau, hoTenPost, ngaySinh, CheckBHYT_Username, MD5Hash(CheckBHYT_Password));
                                    string[] ketqua = checkHanThe(giaTriDau, hoTenPost, ngaySinh, CheckBHYT_Username, CheckBHYT_Password);

                                    string HoTen = ketqua[4];
                                    string NgaySinh = ketqua[5];
                                    if (NgaySinh.Length < 8)
                                        NgaySinh = "01/01/" + NgaySinh;
                                    string DiaChi = ketqua[7];
                                    string SoTheBHYT = ketqua[2];
                                    string HanThe = ketqua[12];
                                    string ChinhXac = ketqua[1];
                                    int GioiTinh = 0;
                                    string GioiTinhTemp = ketqua[6];
                                    if (GioiTinhTemp == "Nam")
                                        GioiTinh = 1;

                                    string KyHieu = SoTheBHYT.Substring(0, 3);

                                    int UuTien = 0;

                                    if (
                                        KyHieu == "CK2" || KyHieu == "CB2" || KyHieu == "KC2" || KyHieu == "KC4" || KyHieu == "TE1"
                                        || KyHieu == "BT2" || KyHieu == "CT2" || KyHieu == "CC1" || KyHieu == "HT1" || tuoi < 6 || tuoi >= 80
                                    )
                                    {
                                        UuTien = 1;
                                    }

                                    DuLieuHienTai = "";
                                    DuLieuHienTai = textBox.Text;
                                    textBox.Clear();
                                    //Gửi API
                                    var client = new HttpClient();
                                    var request = new HttpRequestMessage(HttpMethod.Post, URL);
                                    //var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi  + "\",\r\n    \"hotenbenhnhan\":\"" + HoTen + "\",\r\n    \"ngaysinh\": \"" + NgaySinh + "\",\r\n    \"gioitinh\":" + GioiTinh + ",\r\n    \"diachi\": \"" + DiaChi + "\",\r\n    \"matheBHYT\": \"" + SoTheBHYT + "\",\r\n    \"uutien\": " + UuTien + "\r\n}", null, "application/json");
                                    var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi + "\",\r\n    \"hotenbenhnhan\":\"" + HoTen + "\",\r\n    \"ngaysinh\": \"" + NgaySinh + "\",\r\n    \"gioitinh\":" + GioiTinh + ",\r\n    \"diachi\":\"" + DiaChi + "\",\r\n    \"cccd\":\"\",\r\n    \"matheBHYT\": \"" + SoTheBHYT + "\",\r\n    \"uutien\":" + UuTien + "\r\n}", null, "application/json");
                                    request.Content = content;
                                    var response = await client.SendAsync(request);
                                    response.EnsureSuccessStatusCode();

                                    var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                                    var jsonData = JToken.Parse(result.ToString());

                                    string STT = jsonData[0]["sothutu"].ToString();

                                    //Gán thông tin hiển thị
                                    string UuTienString = UuTien == 1 ? "ƯU TIÊN" : "";

                                    NgayGioHienTai.Text = hienTai;
                                    SoThuTu.Text = STT;
                                    ThongTinThe.Text = "Hạn thẻ: " + HanThe + System.Environment.NewLine + ChinhXac;
                                    ThongTinThe.FontSize = 24;
                                    textUuTien.Text = UuTienString;
                                    TenBenhNhan.Text = "Họ tên: " + HoTen;
                                    SoThe.Text = "Số thẻ: " + SoTheBHYT;
                                    TenBenhNhan.Visibility = Visibility.Visible;
                                    SoThe.Visibility = Visibility.Hidden;
                                    textBox.Clear();
                                    textBox.Focus();
                                    textBox.SelectAll();

                                    //In phiếu thứ tự
                                    await Task.Delay(300);
                                    PrintSTTV2(1);
                                }
                                catch (Exception)
                                {
                                    //Lỗi thì xuất thẻ thường
                                    try
                                    {
                                        if (textBox.Text.Trim() != DuLieuHienTai)
                                        {
                                            int tuoi = CalculateAge(ngaySinhFormat);
                                            int uutien = 0;
                                            if (tuoi < 6 || tuoi >= 80)
                                                uutien = 1;

                                            DuLieuHienTai = "";
                                            DuLieuHienTai = textBox.Text;
                                            textBox.Clear();
                                            //Gửi API
                                            var client = new HttpClient();
                                            var request = new HttpRequestMessage(HttpMethod.Post, URL);
                                            var content = new StringContent("{\r\n    \"donvi\": \"" + MaDonVi + "\",\r\n    \"hotenbenhnhan\":\"\",\r\n    \"ngaysinh\": \"\",\r\n    \"gioitinh\":1,\r\n    \"diachi\":\"\",\r\n    \"matheBHYT\": \"\",\r\n    \"uutien\": " + uutien + "\r\n}", null, "application/json");
                                            request.Content = content;
                                            var response = await client.SendAsync(request);
                                            response.EnsureSuccessStatusCode();

                                            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                                            var jsonData = JToken.Parse(result.ToString());

                                            string STT = jsonData[0]["sothutu"].ToString();

                                            NgayGioHienTai.Text = hienTai;
                                            SoThuTu.Text = STT;
                                            if (uutien == 0)
                                            {
                                                ThongTinThe.Text = "KHÔNG ƯU TIÊN";
                                                ThongTinThe.FontSize = 32;
                                                textUuTien.Text = "";
                                            }
                                            else
                                            {
                                                ThongTinThe.Text = "ƯU TIÊN";
                                                ThongTinThe.FontSize = 48;
                                                textUuTien.Text = "";
                                            }

                                            TenBenhNhan.Text = "";
                                            SoThe.Text = "";
                                            textBox.Clear();
                                            textBox.Focus();
                                            textBox.SelectAll();

                                            await Task.Delay(300);
                                            if (uutien == 1)
                                                PrintSTTV2(1);
                                            else
                                                PrintSTTV2(0);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        textBox.Clear();
                                        MessageBox.Show("Có lỗi xảy ra khi tạo số thường!" + Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }

                                    //MessageBox.Show("Có lỗi xảy ra khi quét thẻ BHYT!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                        }
                    }
                }
            }
        }

        //Các hàm phụ
        public string[] checkHanThe(string theBHYT, string tenBN, string namSinh, string taiKhoan, string matKhau)
        {
            try
            {
                string handung = checkThongTinBHYT.thongTinThe2018("api/egw/KQNhanLichSuKCB2019?", taiKhoan, matKhau, theBHYT, tenBN, namSinh);
                string[] arrcheckthe = handung.Split(new Char[] { '|' });
                if (Convert.ToInt32(arrcheckthe[0]) == 401 || Convert.ToInt32(arrcheckthe[0]) == 205 || Convert.ToInt32(arrcheckthe[0]) == 0
                    || Convert.ToInt32(arrcheckthe[0]) == 100 || Convert.ToInt32(arrcheckthe[0]) == 101
                    || Convert.ToInt32(arrcheckthe[0]) == 130 || arrcheckthe[12] == "null"
                    || Convert.ToInt32(arrcheckthe[0]) == 1 || Convert.ToInt32(arrcheckthe[0]) == 2 || Convert.ToInt32(arrcheckthe[0]) == 4)
                {
                    return arrcheckthe;
                }
                return new string[20] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
            }
            catch
            {
                return new string[20] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
            }
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        private string ConvertHexStrToUnicode(String hexString)
        {
            try
            {
                int length = hexString.Length;
                byte[] bytes = new byte[length / 2];
                for (int i = 0; i < length; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
                }
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            try
            {
                age = DateTime.Now.Year - dateOfBirth.Year;
                if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                    age = age - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tính tuổi bệnh nhân!" + Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return age;
        }

        public class NativeMethods
        {
            public const int HWND_BOTTOM = 1;
            public const int SWP_NOMOVE = 0x0002;
            public const int SWP_NOSIZE = 0x0001;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        }

        private void focusButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.Focus();
            textBox.SelectAll();
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focusIcon.Foreground = Brushes.Green;
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            focusIcon.Foreground = Brushes.Red;
        }
    }
}
