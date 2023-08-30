using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Configuration;
using System.Security.Cryptography;
using System.Data;
using BatSo.BUS;
using System.Windows;

namespace BatSo
{
    class checkThongTinBHYT
    {
        public static dynamic thongTinThe2018(string traCuuThe, string userName, string passWord, string maThe, string hoTen, string namSinh)
        {
            string iKetQua = "", iMessage = "";

            userName = userName.ToUpper();

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://egw.baohiemxahoi.gov.vn");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string username = userName;
                string password = passWord;

                ApiToken input = new ApiToken { username = username, password = password };

                //ApiToken input = new ApiToken { username = username, password = encryptDataMD5(passWord) };
                var values = new Dictionary<string, string>
                {
                    { "username", username },
                    { "password", password }
                };
                var content = new FormUrlEncodedContent(values);
                HttpResponseMessage response = client.PostAsync("api/token/take", content).Result;
                if (response.IsSuccessStatusCode)
                {

                    KQPhienLamViec plv = response.Content.ReadAsAsync<KQPhienLamViec>().Result;
                    var key = plv.APIKey;
                    try
                    {
                        string data2 = string.Format("token={0}&id_token={1}&username={2}&password={3}", key.access_token, key.id_token, username, password);


                        //tra cuu The: api/egw/NhanLichSuKCB2018?" 
                        HttpResponseMessage response2 = client.PostAsJsonAsync(traCuuThe + data2,
                        new ApiTheBHYT2018 { maThe = maThe, hoTen = hoTen, ngaySinh = namSinh }).Result;

                        if (response2.IsSuccessStatusCode)
                        {
                            string result = response2.Content.ReadAsStringAsync().Result;
                            try
                            {

                                var kqua = (KQLichSuKCBBS)JsonConvert.DeserializeObject<KQLichSuKCBBS>(result);

                                string ngaySinh = kqua.ngaySinh;
                                string gioiTinh = kqua.gioiTinh;
                                string maCSKCB = kqua.maDKBD;
                                string ngayBD = kqua.gtTheTuMoi;
                                string ngayKT = kqua.gtTheDenMoi;

                                if (ngaySinh == null)
                                    ngaySinh = "1900";

                                HttpResponseMessage response200 = client.PostAsJsonAsync(traCuuThe + data2,
                                new ApiTheBHYT2018 { maThe = maThe, hoTen = hoTen, ngaySinh = ngaySinh, gioiTinh = gioiTinh, maCSKCB = maCSKCB, ngayBD = ngayBD, ngayKT = ngayKT }).Result;


                                if (response200.IsSuccessStatusCode)
                                {
                                    string result200 = response200.Content.ReadAsStringAsync().Result;
                                    try
                                    {
                                        var kqua200 = (KQLichSuKCBBS)JsonConvert.DeserializeObject<KQLichSuKCBBS>(result200);

                                        string maKQ = kqua200.maKetQua;
                                        switch (maKQ)
                                        {
                                            case "000":
                                                iKetQua = "Thông tin thẻ chính xác";
                                                break;
                                            case "001":
                                                iKetQua = "Thẻ do BHXH Bộ Quốc Phòng quản lý, đề nghị kiểm tra thẻ và thông tin giấy tờ tùy thân";
                                                break;
                                            case "002":
                                                iKetQua = "Thẻ do BHXH Bộ Công An quản lý, đề nghị kiểm tra thẻ và thông tin giấy tờ tùy thân";
                                                break;
                                            case "003":
                                                iKetQua = "Thẻ cũ hết giá trị sử dụng nhưng đã được cấp thẻ mới";
                                                break;
                                            case "004":
                                                iKetQua = "Thẻ cũ còn giá trị sử dụng nhưng đã được cấp thẻ mới";
                                                break;
                                            case "010":
                                                iKetQua = "Thẻ hết giá trị sử dụng";
                                                break;
                                            case "051":
                                                iKetQua = "Mã thẻ không đúng";
                                                break;
                                            case "052":
                                                iKetQua = "Mã tỉnh cấp thẻ (kí tự thứ 4,5 của mã thẻ) không đúng";
                                                break;
                                            case "053":
                                                iKetQua = "Mã quyền lợi thẻ (kí tự thứ 3 của mã thẻ) không đúng";
                                                break;
                                            case "050":
                                                iKetQua = "Không thấy thông tin thẻ BHYT";
                                                break;
                                            case "060":
                                                iKetQua = "Thẻ sai họ tên";
                                                break;
                                            case "061":
                                                iKetQua = "Thẻ sai họ tên (đúng kí tự đầu)";
                                                break;
                                            case "070":
                                                iKetQua = "Thẻ sai ngày sinh";
                                                break;
                                            case "100":
                                                iKetQua = "Lỗi khi lấy dữ liệu sổ thẻ";
                                                break;
                                            case "101":
                                                iKetQua = "Lỗi server";
                                                break;
                                            case "110":
                                                iKetQua = "Thẻ đã thu hồi";
                                                break;
                                            case "120":
                                                iKetQua = "Thẻ đã báo giảm";
                                                break;
                                            case "121":
                                                iKetQua = "Thẻ đã báo giảm. Giảm chuyển ngoại tỉnh";
                                                break;
                                            case "122":
                                                iKetQua = "Thẻ đã báo giảm. Giảm chuyển nội tỉnh";
                                                break;
                                            case "123":
                                                iKetQua = "Thẻ đã báo giảm. Thu hồi do tăng lại đơn vị";
                                                break;
                                            case "124":
                                                iKetQua = "Thẻ đã báo giảm. Ngừng tham gia";
                                                break;
                                            case "130":
                                                iKetQua = "Trẻ em không xuất trình thẻ";
                                                break;
                                            case "205":
                                                iKetQua = "Lỗi sai định dạng tham số truyền vào";
                                                break;
                                            case "401":
                                                iKetQua = "Lỗi xác thực tài khoản";
                                                break;
                                        }

                                        string tenDKBD = "";
                                        iMessage = maKQ + "|" + iKetQua + "|" + kqua200.maThe + "|" + kqua200.maSoBHXH + "|" + kqua200.hoTen + "|" + kqua200.ngaySinh + "|" + kqua200.gioiTinh + "|" + kqua200.diaChi + "|" + kqua200.maDKBD + "|" + tenDKBD + "|" + kqua200.cqBHXH + "|" + kqua200.gtTheTu + "|" + kqua200.gtTheDen + "|" + kqua200.maKV + "|" + kqua200.ngayDu5Nam + "|" + kqua200.maTheMoi + "|" + kqua200.gtTheTuMoi + "|" + kqua200.gtTheDenMoi + "|" + kqua200.maTheCu + "|" + kqua200.ghiChu;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Có lỗi xảy ra khi kiểm tra thẻ!" + System.Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Có lỗi xảy ra khi kiểm tra thẻ!" + System.Environment.NewLine + "Thông tin lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch
                    {
                        //if (key == null)
                        //    XtraMessageBox.Show("Lỗi xác thực tài khoản HIRA với tài khoản cổng TN!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return iMessage;

        }
    }
}
