using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;  // JSON 파싱을 위한 라이브러리 추가
using WMPLib;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace chzzkbangonallramTEST
{
    public partial class Form1 : Form
    {
        private NotifyIcon trayIcon;  // 클래스 수준에서 trayIcon을 선언합니다.
        private ContextMenuStrip trayMenu;

        // Assuming targetURL is defined as follows:
        private string targetURL = "notset"; // Replace with your actual URL
        private string chnnale_url = "";

        private string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private string ThisDirectory = System.IO.Directory.GetCurrentDirectory();
        private string JsonDirectory = "";
        private string JsonfileDirectory = "";
        private JObject jsonData;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            JsonDirectory = ThisDirectory + @"\Json";
            JsonfileDirectory = JsonDirectory + @"\stremerlist.json";
            string jsondata = File.ReadAllText(JsonfileDirectory);
            jsonData = JObject.Parse(jsondata);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Point mouseLocation;  // 클래스 수준에서 선언

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(-mouseLocation.X, -mouseLocation.Y);
                this.Location = mousePos;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/iwantmunsang/chzzkbangonpy");
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(e.X, e.Y);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(-mouseLocation.X, -mouseLocation.Y);
                this.Location = mousePos;
            }
        }

        public string read_json(string file_path)
        {
            string JsonfileDirectory = file_path;

                // 파일 내용 읽기
            try
            {
                if (File.Exists(JsonfileDirectory))
                {
                    string jsonContent = File.ReadAllText(JsonfileDirectory);

                    // JSON 파일이 비어있지 않은지 확인하고 파싱
                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        JObject jsonObject = JObject.Parse(jsonContent);
                        return jsonObject.ToString();
                        
                    }                    
                    else                    
                    {                   
                        return "JSON 파일이 비어 있습니다.";                       
                    }                   
                }                
                else               
                {               
                    return "JSON 파일을 찾을 수 없습니다.";                    
                }               
            }            
            catch (JsonReaderException jex)            
            {           
                return $"JSON 파일의 형식이 잘못되었습니다: {jex.Message}";                
            }            
            catch (Exception ex)
            {
                return $"파일을 읽는 중 오류 발생: {ex.Message}";
            }
        }


        public bool api_get_runing = false;

        public async Task api_get()
        {
            api_get_runing = true;
            JArray usersArray = (JArray)jsonData["users"];

            for (int i = 0; i < usersArray.Count; i++)
            {
                string Targetid = usersArray[i]["channlid"].ToString(); // "channlid" 값 추출

                targetURL = "https://api.chzzk.naver.com/service/v1/channels/" + Targetid;
                api_get_runing = true;
                string result = string.Empty;
                try
                {
                    WebClient client = new WebClient();

                    // Add specific headers
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 12.4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.6335.223 Safari/537.36");
                    client.Headers.Add("Accept-Language", "ko-KR,ko;q=0.9,en");
                    client.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    client.Headers.Add("Referer", "https://www.google.com/");
                    client.Headers.Add("Upgrade-Insecure-Requests", "1");
                    client.Headers.Add("Cookie", "NNB=5Z6ILX3ZKMLGO; nid_inf=490044677; NID_AUT=rFmati7co9izzzJzdUlQQurkpqn3DReAtXKyDo8MSPUrfuHeV6eEBP6xri5Mb6FY; NID_JKL=LChhy6zSGaeMi+xf/3QLtuuBUgNo0t7wUo2by5VgLmk=; loungesRecentlyVisited=chzzk; lastSeenQuestPopupTime=1730551445280; recentKeyword=%5B%5D; recentKeywordInLounge=%5B%5D; BUC=68tGDuP97gonv2Tan7GEzoTZFHI5s5Gxnt6KlEDxh8o=; NID_SES=AAABnfhe0qmsEnuN5XoJEr4K3k7NK61vIGMUf7gaBI60il63SeYw+5DQ+mHJ5LNPrm4ulKVx7IlMUMHW4z3KgtHJxv8rP0K52qvk06JvDbRUmolgpgrvGlt6/3qJ52YJ08ymfrBt12EEdpXDlkyxTfKZtnuPjQ7vgUZTdFD8eRtUobkmvDIqb7kd/52g00DTuel1fUlWxS5zmZP01T0Mec9wK2assSix9R7lVK7ETn57FeZZhAtbyn6JBzrwHOdrL0CVNbiv9fiS2dSwskLlMokOztU6oJreA2g26cEpfOBI7VnXctUstOFBgnjjuEnV25G0ropigkh8MOkPRPQdLJGvWyjf7qCSK93XNDMelI3+B7+iS4fxL24MMeWYact/YXIbiRRUeAq3F8M8qUeT10SKV+P+RswzOdlNuTvQSJdnrKGiAS6xSB6up4L3nivJF/qqzBjmL2dJC3LV3lXrOhZkQpHxDliFrrs5/S+TfJAReBfRKyK44jw628WugqMgZ7rT2RgSLg89BnMIIcE9JGR3u2qDueeVt8KYVbdxZKuzj/vP; ba.uuid=0");

                    // Open the stream for the URL and read the content asynchronously
                    using (Stream data = await client.OpenReadTaskAsync(targetURL))
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            result = await reader.ReadToEndAsync();
                        }
                    }

                    // Parse JSON to extract the 'channelName' from 'content'
                    dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                    // Extract the details from the response
                    string channelName = jsonResponse.content.channelName;
                    bool openlive = jsonResponse.content.openLive;
                    int followerCount = jsonResponse.content.followerCount;
                    string livetitle = "not streming";
                    if (openlive)
                    {
                        targetURL = "https://api.chzzk.naver.com/service/v1/channels/" + Targetid + "/live-detail";
                        result = string.Empty;
                        using (Stream data = await client.OpenReadTaskAsync(targetURL))
                        {
                            using (StreamReader reader = new StreamReader(data))
                            {
                                result = await reader.ReadToEndAsync();
                            }
                        }
                        jsonResponse = JsonConvert.DeserializeObject(result);

                        livetitle = jsonResponse.content.liveTitle;
                    }
                    else
                    {
                        livetitle = "not streming";

                    }

                    // Update the "users" array with the new information
                    usersArray[i]["channelName"] = channelName;
                    usersArray[i]["openlive"] = openlive;
                    usersArray[i]["followerCount"] = followerCount;
                    usersArray[i]["livetitle"] = livetitle;


                    // Save the updated JSON data back to the file
                    File.WriteAllText(JsonfileDirectory, jsonData.ToString());

                    // Optionally, print the updated JSON to the console
                }
                catch (Exception ex)
                {
                    // Handle communication failures
                    MessageBox.Show($"Error: 채널 링크의 값을 확인 해주세요 \n\n{ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    api_get_runing = false;
                    targetURL = "notset";
                }
            }
        }


        private void Print(string message)
        {
            MessageBox.Show($"{message}", "디버깅", MessageBoxButtons.OK, MessageBoxIcon.Information);
        } 

        private void printerror(string message)
        {
            MessageBox.Show($"{message}", "디버깅", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void reload_button_click(object sender, EventArgs e)
        {
            
            if (!api_get_runing)
            {
                await api_get();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
            chnnale_url = textBox2.Text;

            if (!string.IsNullOrEmpty(chnnale_url))
            {
                if (chnnale_url.Contains("chzzk.naver.com/"))
                {
                    try
                    {
                        string url = chnnale_url;

                        // Uri 객체 생성 및 유효성 검사
                        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                        {
                            throw new UriFormatException("URL 형식이 올바르지 않습니다.");
                        }

                        
                        string lastPart = uri.Segments[uri.Segments.Length - 1];

                        // JSON 파일이 존재하는지 확인
                        if (!File.Exists(JsonfileDirectory))
                        {
                            // 파일이 없으면 빈 JSON 배열 생성
                            File.WriteAllText(JsonfileDirectory, "{\"users\":[]}");
                        }

                        // JSON 파일 내용 읽기
                        string jsonContent = File.ReadAllText(JsonfileDirectory);

                        // JSON 데이터를 dynamic 형식으로 파싱
                        JObject data = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

                        // "users" 배열 생성 또는 가져오기
                        JArray usersArray = data["users"] as JArray ?? new JArray();

                        // 새 사용자 데이터 생성
                        JObject newUser = new JObject
                        {

                            ["channelName"] = "noname",
                            ["openlive"] = false,
                            ["followerCount"] = 0,
                            ["livetitle"] = "notitle",
                            ["channlid"] = lastPart
                        };

                        // "users" 배열에 새 데이터 추가
                        usersArray.Add(newUser);
                        data["users"] = usersArray;

                        // 변경된 JSON 데이터를 파일에 다시 저장
                        File.WriteAllText(JsonfileDirectory, data.ToString());

                        Print(data.ToString());

                        label2.Text = "추가 되었습니다";
                        label2.ForeColor = System.Drawing.Color.Lime;
                    }
                    catch (UriFormatException uriEx)
                    {
                        label2.Text = "추가가 취소 되었습니다";
                        label2.ForeColor = System.Drawing.Color.Red;
                        printerror("URL 형식이 잘못되었습니다.\n\n" + uriEx.Message);
                    }
                    catch (JsonException jsonEx)
                    {
                        label2.Text = "추가가 취소 되었습니다";
                        label2.ForeColor = System.Drawing.Color.Red;
                        printerror("JSON 처리 중 오류가 발생했습니다.\n\n" + jsonEx.Message);
                    }
                    catch (Exception ex)
                    {
                        label2.Text = "추가가 취소 되었습니다";
                        label2.ForeColor = System.Drawing.Color.Red;
                        printerror("오류가 발생했습니다.\n\n" + ex.Message);
                    }
                    finally
                    {
                        textBox2.Text = "";
                    }
                }
                else
                {
                    textBox2.Text = "";
                    label2.Text = "추가가 취소 되었습니다";
                    label2.ForeColor = System.Drawing.Color.Red;
                    printerror("url이 정상적이지 않습니다.");
                }
            }
            else
            {
                textBox2.Text = "";
                label2.Text = "추가가 취소 되었습니다";
                label2.ForeColor = System.Drawing.Color.Red;
                printerror("url이 입력되지 않았습니다.");
            }
        }



        private int label4_Click_value = 0;
        private bool click_label4 = false;

        private void label4_Click(object sender, MouseEventArgs e)
        {
            if (!click_label4)
            {
                // 클릭 횟수가 10 이상일 때만 색상 변경을 허용
                if (label4_Click_value >= 10)
                {
                    // 오른쪽 버튼 클릭 시 색상 변경
                    if (e.Button == MouseButtons.Right)
                    {
                        // 기본 색상 설정
                        label1.ForeColor = System.Drawing.Color.White;
                        label2.ForeColor = System.Drawing.Color.White;
                        label4.ForeColor = System.Drawing.Color.White;
                        button1.ForeColor = System.Drawing.Color.White;
                        button2.ForeColor = System.Drawing.Color.White;
                        reload_button.ForeColor = System.Drawing.Color.Black;
                        button4.ForeColor = System.Drawing.Color.Black;
                        textBox2.ForeColor = System.Drawing.Color.Black;
                    }
                    else if (e.Button == MouseButtons.Left)
                    {

                        // 랜덤 색상 설정
                        Random randomobj = new Random();
                        label1.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        label2.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        label4.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        button1.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        button2.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        reload_button.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        button4.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                        textBox2.ForeColor = Color.FromArgb(randomobj.Next(256), randomobj.Next(256), randomobj.Next(256));
                    }
                }
                else
                {
                    // 클릭 횟수 증가
                    WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                    wplayer.URL = currentPath + @"\Sound\sound.mp3";
                    wplayer.controls.play(); // 'controls' 속성을 통해 'play()' 메서드 호출
                    label4_Click_value++;
                }
            }
        }


        private List<Control> FindScrollTaggedControls(Control parentControl)
        {
            List<Control> scrollTaggedControls = new List<Control>();

            foreach (Control control in parentControl.Controls)
            {
                // Tag가 "scroll"이고, 버튼 또는 라벨일 때만 추가
                if (control.Tag?.ToString() == "scroll" && (control is Button || control is Label))
                {
                    scrollTaggedControls.Add(control);
                }

                // 하위 컨트롤도 재귀적으로 검색
                if (control.HasChildren)
                {
                    scrollTaggedControls.AddRange(FindScrollTaggedControls(control));
                }
            }

            return scrollTaggedControls;
        }

    }
}

