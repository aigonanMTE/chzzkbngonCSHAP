using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Windows.Foundation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;


namespace chzzkbangonallramTEST
{
    public partial class firstsettingpage : UserControl
    {
        public int page = 0;
        public string jsondata;
        public firstsettingpage()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        public static string ConvertToJson(string rawData)
        {
            var headers = new Dictionary<string, string>();
            var lines = rawData.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i += 2)
            {
                string key = lines[i].Trim(':').Trim(); // ':' 제거 후 공백 제거
                string value = (i + 1 < lines.Length) ? lines[i + 1].Trim() : ""; // 값 가져오기
                headers[key] = value;
            }

            var jsonObject = new
            {
                user_headers = headers
            };

            if (!headers.ContainsKey("cookie"))
            {
                return "not cookie in value";
            }

            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }



        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (page)
            {
                case 0:
                    // 페이지 0 -> 1로 전환
                    page++;
                    label1.Text = "먼지 유저님의 브라우저 정보가 \n필요해요!\n사이트에 접속해 입력 방법을 \n알아보세요!";
                    button2.Visible = true;
                    textBox1.Visible = true;
                    break;

                case 1:
                    // 텍스트 입력 확인
                    if (string.IsNullOrWhiteSpace(textBox1.Text)) // 텍스트 박스가 비어있으면
                    {
                        MessageBox.Show("브라우저 정보를 입력 해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        try
                        {
                            if (ConvertToJson(textBox1.Text) != "not cookie in value")
                            {
                                jsondata = ConvertToJson(textBox1.Text);
                            }
                        }
                        catch (Exception ex) {
                            MessageBox.Show($"에러 {ex}", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                            
                        // 텍스트가 입력되면 다음 페이지로 전환
                        page++;
                        label1.Text = "기본 세팅이 완료 되었습니다!";
                        textBox1.Visible = false;
                        button2.Visible = false;
                    }
                    break;


                default:
                    MessageBox.Show("알 수 없는 상태입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string link = $"https://scratch-maple-882.notion.site/1459098ea1a480a7b568e82d65d95dcb";
            System.Diagnostics.Process.Start(link);
        }
    }
}
