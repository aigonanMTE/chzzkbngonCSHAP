using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace chzzkbangonallramTEST
{
    public partial class firstsettingpage : UserControl
    {
        public int page = 0;
        public string jsondata;
        private string ThisDirectory = System.IO.Directory.GetCurrentDirectory();
        private string JsonDirectory = "";
        private string SettingJsonDIrectory = "";
        private JObject settingjsonData;

        public firstsettingpage()
        {
            InitializeComponent();
            JsonDirectory = ThisDirectory + @"\Json";
            SettingJsonDIrectory = JsonDirectory + @"\setting.json";

            // JSON 파일 로드
            string settingjsondata = File.ReadAllText(SettingJsonDIrectory);
            settingjsonData = JObject.Parse(settingjsondata);
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        public static string ConvertToJson(string rawData, JObject existingData = null)
        {
            var headers = new Dictionary<string, string>();
            var lines = rawData.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i += 2)
            {
                string key = lines[i].Trim(':').Trim(); // ':' 제거 후 공백 제거
                string value = (i + 1 < lines.Length) ? lines[i + 1].Trim() : ""; // 값 가져오기
                headers[key] = value;
            }

            // 기존 데이터에 user_headers 병합
            if (existingData == null)
            {
                existingData = new JObject
                {
                    ["setting"] = new JObject
                    {
                        ["firstrun"] = true,
                        ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    ["user_headers"] = JObject.FromObject(headers)
                };
            }
            else
            {
                if (!existingData.ContainsKey("user_headers"))
                {
                    existingData["user_headers"] = new JObject();
                }

                foreach (var header in headers)
                {
                    ((JObject)existingData["user_headers"])[header.Key] = header.Value;
                }
            }

            if (!headers.ContainsKey("cookie"))
            {
                return "not cookie in value";
            }

            return JsonConvert.SerializeObject(existingData, Formatting.Indented);
        }

        private void SaveJsonToFile(string jsonData)
        {
            try
            {
                File.WriteAllText(SettingJsonDIrectory, jsonData, Encoding.UTF8); // JSON 파일 저장
                MessageBox.Show($"JSON 데이터가 {SettingJsonDIrectory} 파일에 저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JSON 파일 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (page)
            {
                case 0:
                    page++;
                    label1.Text = "먼지 유저님의 브라우저 정보가 \n필요해요!\n사이트에 접속해 입력 방법을 \n알아보세요!";
                    button2.Visible = true;
                    textBox1.Visible = true;
                    break;

                case 1:
                    if (string.IsNullOrWhiteSpace(textBox1.Text)) // 텍스트 박스가 비어있으면
                    {
                        MessageBox.Show("브라우저 정보를 입력 해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        try
                        {
                            string existingJson = File.ReadAllText(SettingJsonDIrectory);
                            JObject existingData = JObject.Parse(existingJson);
                            string convertedJson = ConvertToJson(textBox1.Text, existingData);

                            if (convertedJson != "not cookie in value")
                            {
                                jsondata = convertedJson;
                                MessageBox.Show(jsondata, "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                SaveJsonToFile(jsondata); // JSON 데이터를 파일에 저장
                            }
                            else
                            {
                                MessageBox.Show($"브라우저 정보 중 cookie 값이 빠졌습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"에러 발생: {ex.Message}", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }

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
