using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Emit;

namespace chzzkbangonallramTEST
{

    public partial class Form2 : Form
    {
        private string SettingJsonDIrectory = "";
        private JObject settingjsonData;
        private string JsonDirectory = "";
        private string JsonfileDirectory = "";
        private string ThisDirectory = System.IO.Directory.GetCurrentDirectory();
        private Form1 form1 = Application.OpenForms["Form1"] as Form1;
        public Form2()
        {
            InitializeComponent();
            JsonDirectory = ThisDirectory + @"\Json";
            JsonfileDirectory = JsonDirectory + @"\stremerlist.json";
            SettingJsonDIrectory = JsonDirectory + @"\setting.json";
            string Settingjsondata = File.ReadAllText(SettingJsonDIrectory);
            settingjsonData = JObject.Parse(Settingjsondata);
            mainscrollobject();


        }
        private SButton liveimageshow;
        private TextBox getinputcoler;
        private System.Windows.Forms.Label label2;
        private void mainscrollobject()
        {
            var showliveimage = (bool)settingjsonData["setting"]["showliveimage"];
            var stremer_label_coler = (string)settingjsonData["setting"]["stremer_label_coler"]; // 문자열로 변환

            scrollPanel.AutoScroll = true;

            // 첫 번째 라벨과 버튼
            System.Windows.Forms.Label label = new System.Windows.Forms.Label
            {
                Width = 200,
                Height = 40,
                Text = "방송 미리보기 표시",
                Font = new Font("CookieRun Bold", 24),
                ForeColor = System.Drawing.Color.White,
                Margin = new Padding(0, 0, 0, 50)
            };
            scrollPanel.Controls.Add(label);

            liveimageshow = new SButton
            {
                IsOn = showliveimage,
                Location = new Point(350, 0), // 라벨의 오른쪽에 배치
                Width = 100,
                Height = 40,
                Margin = new Padding(0, 0, 0, 50)
            };
            scrollPanel.Controls.Add(liveimageshow);
            liveimageshow.MouseClick += Liveimageshow_MouseClick;

            // 두 번째 라벨과 텍스트 박스
            label2 = new System.Windows.Forms.Label
            {
                Width = 300,
                Height = 40,
                Text = "스트리머 이름 라벨 색",
                Font = new Font("CookieRun Bold", 24),
                ForeColor = System.Drawing.Color.White,
                Margin = new Padding(0, 0, 0, 50),
                Location = new Point(0, label.Height + 20) // 첫 번째 라벨 아래에 배치
            };

            // JSON에서 추출한 R, G, B, A 값을 사용하여 Color 객체 생성
            var label2_forecolor = ConvertToColor(stremer_label_coler);
            //MessageBox.Show(label2_forecolor.A.ToString());

            var a = label2_forecolor.A;
            var b = label2_forecolor.B;
            var g = label2_forecolor.G;
            var r = label2_forecolor.R;

            settingjsonData["setting"]["a"] = a;
            settingjsonData["setting"]["b"] = b;
            settingjsonData["setting"]["g"] = g;
            settingjsonData["setting"]["r"] = r;

            label2.ForeColor = label2_forecolor;

            scrollPanel.Controls.Add(label2);

            getinputcoler = new TextBox
            {
                Font = new Font("CookieRun Bold", 20),
                Width = 150,
                Height = 40,
                Margin = new Padding(0, 0, 0, 50),
                Location = new Point(label2.Location.X + label2.Width + 10, label2.Location.Y), // 라벨의 오른쪽에 배치
                Text = $"{label2_forecolor.R},{label2_forecolor.G},{label2_forecolor.B},{label2_forecolor.A}"
            };
            getinputcoler.TextChanged += Getinputcoler_TextChanged;
            scrollPanel.Controls.Add(getinputcoler);
            

            // JSON 파일에 컬러값을 업데이트하여 저장
            File.WriteAllText(SettingJsonDIrectory, settingjsonData.ToString());
        }

        private void Getinputcoler_TextChanged(object sender, EventArgs e)
        {
            settingjsonData["setting"]["stremer_label_coler"] = getinputcoler.Text.ToString();
            var label2_forecolor = ConvertToColor(getinputcoler.Text.ToString());

            var a = label2_forecolor.A;
            var b = label2_forecolor.B;
            var g = label2_forecolor.G;
            var r = label2_forecolor.R;

            settingjsonData["setting"]["a"] = a;
            settingjsonData["setting"]["b"] = b;
            settingjsonData["setting"]["g"] = g;
            settingjsonData["setting"]["r"] = r;

            //MessageBox.Show(settingjsonData["setting"].ToString());
            File.WriteAllText(SettingJsonDIrectory, settingjsonData.ToString());
            label2.ForeColor = label2_forecolor;
        }

        static System.Drawing.Color ConvertToColor(string input)
        {
            // HEX 형식 처리
            if (input.StartsWith("#"))
            {
                string hex = input.TrimStart('#');
                if (hex.Length == 6 || hex.Length == 8)
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    if (hex.Length == 8) // AARRGGBB 형식에서 알파값은 무시
                    {
                        // int a = Convert.ToInt32(hex.Substring(6, 2), 16); // 알파 값은 필요하면 사용 가능
                    }
                    return System.Drawing.Color.FromArgb(r, g, b);
                }
            }

            // RGB 형식 처리
            if (input.StartsWith("rgb(") && input.EndsWith(")"))
            {
                string rgb = input.Substring(4, input.Length - 5); // "rgb("와 ")" 제거
                string[] rgbValues = rgb.Split(',');
                if (rgbValues.Length == 3)
                {
                    int r = int.Parse(rgbValues[0].Trim());
                    int g = int.Parse(rgbValues[1].Trim());
                    int b = int.Parse(rgbValues[2].Trim());
                    return System.Drawing.Color.FromArgb(r, g, b);
                }
            }

            // RGBA 형식 처리
            if (input.StartsWith("rgba(") && input.EndsWith(")"))
            {
                string rgba = input.Substring(5, input.Length - 6); // "rgba("와 ")" 제거
                string[] rgbaValues = rgba.Split(',');
                if (rgbaValues.Length == 4)
                {
                    int r = int.Parse(rgbaValues[0].Trim());
                    int g = int.Parse(rgbaValues[1].Trim());
                    int b = int.Parse(rgbaValues[2].Trim());
                    // 알파 값은 사용하지 않음
                    return System.Drawing.Color.FromArgb(r, g, b);
                }
            }

            // HTML 색상 이름 처리
            string[] htmlColors = { "red", "blue", "green", "yellow", "black", "white", "gray", "purple", "pink", "orange" };
            var htmlColorValues = new System.Collections.Generic.Dictionary<string, System.Drawing.Color>(StringComparer.OrdinalIgnoreCase)
    {
        { "red", System.Drawing.Color.Red },
        { "blue", System.Drawing.Color.Blue },
        { "green", System.Drawing.Color.Green },
        { "yellow", System.Drawing.Color.Yellow },
        { "black", System.Drawing.Color.Black },
        { "white", System.Drawing.Color.White },
        { "gray", System.Drawing.Color.Gray },
        { "purple", System.Drawing.Color.Purple },
        { "pink", System.Drawing.Color.Pink },
        { "orange", System.Drawing.Color.Orange }
    };

            if (htmlColorValues.ContainsKey(input))
            {
                return htmlColorValues[input];
            }

            return System.Drawing.Color.White;
        }





        private void Liveimageshow_MouseClick(object sender, MouseEventArgs e)
        {
            bool ison = liveimageshow.IsOn;
            settingjsonData["setting"]["showliveimage"] = ison;
            File.WriteAllText(SettingJsonDIrectory, settingjsonData.ToString());
            form1?.api_update_labe(false);
        }

    }
}
