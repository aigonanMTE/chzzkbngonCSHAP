﻿using System;
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
using System.Net.Http;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.Authentication.ExtendedProtection;
using System.Windows.Forms.VisualStyles;

namespace chzzkbangonallramTEST
{


    public partial class Form1 : Form
    {

        // Assuming targetURL is defined as follows:
        private string targetURL = "notset"; // Replace with your actual URL
        private string chnnale_url = "";

        private string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private string ThisDirectory = System.IO.Directory.GetCurrentDirectory();
        private string JsonDirectory = "";
        private string JsonfileDirectory = "";
        private JObject jsonData;
        private int thispage = 1;
        private int mexpage;
        public int stremerid { get; set; }
        private string SettingJsonDIrectory = "";
        private JObject settingjsonData;
        private string imagedirectory;

        public Form1()
        {
            InitializeComponent();
        }

        private void Print(string message)
        {
            MessageBox.Show($"{message}", "디버깅", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void printerror(string message)
        {
            MessageBox.Show($"{message}", "디버깅", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private NotifyIcon notifyIcon;
        private async void Form1_Load(object sender, EventArgs e)
        {
            

            howtousebtn.FlatStyle = FlatStyle.Flat;
            howtousebtn.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            JsonDirectory = ThisDirectory + @"\Json";
            JsonfileDirectory = JsonDirectory + @"\stremerlist.json";
            string jsondata = File.ReadAllText(JsonfileDirectory);
            jsonData = JObject.Parse(jsondata);
            SettingJsonDIrectory = JsonDirectory + @"\setting.json";
            string settingjsondata = File.ReadAllText(SettingJsonDIrectory);
            settingjsonData = JObject.Parse(settingjsondata);
            button5.FlatStyle = FlatStyle.Flat;
            button5.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.FlatAppearance.BorderSize = 0;
            opensetting.FlatStyle = FlatStyle.Flat;
            opensetting.FlatAppearance.BorderSize = 0;
            no_open_live.Visible = false;
            no_open_live.Location = new Point(66, 34);
            imagedirectory = ThisDirectory + @"\images";
            //File.WriteAllText(SettingJsonDIrectory, settingjsonData.ToString());

            set_label_color();

            // NotifyIcon 설정
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // 아이콘 설정
                Visible = true,
                Text = "치지직뱅온알람"
            };

            // ContextMenuStrip 초기화
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("열기", null, (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            });
            contextMenu.Items.Add("닫기", null, (s, e) =>
            {
                notifyIcon.Visible = false; // 트레이 아이콘 숨기기
                Application.Exit(); // 애플리케이션 종료
            });

            notifyIcon.ContextMenuStrip = contextMenu;



            // NotifyIcon 더블 클릭 이벤트
            notifyIcon.DoubleClick += (s, e) =>
            {
                // 트레이에서 더블 클릭 시 창 보이기
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };
            notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    notifyIcon.ContextMenuStrip?.Show();
                }
            };

            

            await StartApiUpdateLoopAsync();
        }

        public void set_label_color()
        {
            string settingjsondata = File.ReadAllText(SettingJsonDIrectory);
            settingjsonData = JObject.Parse(settingjsondata);
            var a = (int)settingjsonData["setting"]["a"];
            var b = (int)settingjsonData["setting"]["b"];
            var g = (int)settingjsonData["setting"]["g"];
            var r = (int)settingjsonData["setting"]["r"];

            stremer_name_label1.ForeColor = Color.FromArgb(a, r, g, b);
            stremer_name_label2.ForeColor = Color.FromArgb(a, r, g, b);
            stremer_name_label3.ForeColor = Color.FromArgb(a, r, g, b);

            streming_title_label1.ForeColor = Color.FromArgb(a, r, g, b);
            streming_title_label2.ForeColor = Color.FromArgb(a, r, g, b);
            streming_title_label3.ForeColor = Color.FromArgb(a, r, g, b);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SettingJsonDIrectory = JsonDirectory + @"\setting.json";
            string settingjsondata = File.ReadAllText(SettingJsonDIrectory);
            var backgroundrun = (bool)settingjsonData["setting"]["backgroundrun"];
            if (backgroundrun) 
            {
                this.Hide();
            }
            else 
            {
                Application.Exit();
            }
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


        public string GetdataByName(string channelName, string dataname, bool fullname)
        {
            // "users" 배열을 가져와서 각 스트리머의 정보를 확인합니다.
            JArray users = (JArray)jsonData["users"];

            foreach (var user in users)
            {
                if ((string)user["channelName"] == channelName)
                {
                    if (!fullname)
                    {
                        return (string)user[$"{dataname}"];
                    }
                    else
                    {
                        string dataValue = (string)user[$"{dataname}"];
                        if (!string.IsNullOrEmpty(dataValue) && dataValue.Length > 11)
                        {
                            return dataValue.Substring(0, 11) + ".."; // 11자 이후를 ".."으로 대체
                        }
                        return dataValue; // 11자 이하이면 그대로 반환
                    }
                }
            }

            return null; // channelName에 해당하는 channlid가 없을 경우 null 반환
        }


        //다운로드 함수
        public async Task DownloadImageAsync(string link, string name)
        {
            try
            {
                // 경로에 디렉토리가 없다면 생성
                string savePath = $@"{ThisDirectory}\images";
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                // 파일 경로 설정
                string filePath = Path.Combine(savePath, name);

                // 시간 체크 시작
                var start = DateTime.Now;

                // 이미지 다운로드
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(link);
                    response.EnsureSuccessStatusCode();

                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(filePath, imageBytes);
                }

                // 다운로드 시간 계산
                var elapsed = DateTime.Now - start;
            }
            catch (Exception e)
            {
                printerror($"이미지 다운로드 오류: \nlink : {link}\n \n{e.Message}");
            }
        }


        //알람 보내기 함수

        private void ShowNotification(string title, string text , string chid)
        {
            // NotifyIcon 객체 생성 및 설정
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true,
                BalloonTipTitle = title,
                BalloonTipText = text
            };

            // BalloonTip 클릭 이벤트 등록
            notifyIcon.BalloonTipClicked += (sender, e) =>
            {
                // 클릭 시 실행할 코드
                
                // notifyIcon을 더 이상 사용하지 않을 경우 Dispose로 정리
                notifyIcon.Dispose();
                string link = $"https://chzzk.naver.com/{chid}";
                System.Diagnostics.Process.Start(link);
            };

            // 3초 동안 알림 표시
            notifyIcon.ShowBalloonTip(3000);
        }
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> SendGetRequest(string url)
        {
            // 헤더 설정 (Postman에서 설정한 것과 동일하게)
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_ACCESS_TOKEN"); // 필요시 인증 토큰 추가
            client.DefaultRequestHeaders.Add("Accept", "application/json");  // 응답 형식 JSON 요청
            client.DefaultRequestHeaders.Add("User-Agent", "YourApp/1.0"); // 필요시 User-Agent 설정

            try
            {
                // GET 요청 보내기
                HttpResponseMessage response = await client.GetAsync(url);

                // 응답 상태 코드 확인
                if (response.IsSuccessStatusCode)
                {
                    // 성공적인 응답 처리
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody; // JSON 형태로 반환
                }
                else
                {
                    // 실패 시 처리
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task Api_get()
        {
            // JSON 데이터를 파일에서 읽음
            string jsonContent = File.ReadAllText(JsonfileDirectory);

            // JSON 데이터를 JObject로 파싱
            JObject data1 = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

            // users 배열 가져오기
            JArray usersArray = (JArray)data1["users"];

            for (int i = 0; i < usersArray.Count; i++)
            {
                try
                {
                    // 채널 데이터 가져오기
                    string response = await SendGetRequest("https://api.chzzk.naver.com/service/v1/channels/" + usersArray[i]["channlid"]);
                    if (response != null)
                    {
                        string liveTitle;
                        // JSON 파싱
                        JObject channle_basic_data = JObject.Parse(response);
                        string channelName = channle_basic_data["content"]["channelName"].ToString();
                        int followerCount = (int)channle_basic_data["content"]["followerCount"];
                        bool openLive = (bool)channle_basic_data["content"]["openLive"];
                        string channelImageUrl = channle_basic_data["content"]["channelImageUrl"].ToString();
                        string channelImagename;

                        // 정규식을 사용하여 이미지 URL에서 파일명 추출
                        string fileName = $"{usersArray[i]["channlid"]}.png";

                        if (fileName != null)
                        {
                            channelImagename = fileName;
                            //Print("Extracted part: " + extractedPart);
                        }
                        else
                        {
                            channelImagename = "ERROR-none-file.png";
                        }

                        //Print($"체널 이름 : {channelName}\n팔로워 카운트 : {followerCount}\n방송 여부 : {openLive}\n체널 이미지 url : {channelImageUrl}\n체널 이미지 이름 : {channelImagename}");

                        if (openLive)
                        {
                            response = await SendGetRequest("https://api.chzzk.naver.com/service/v1/channels/" + usersArray[i]["channlid"] + "/data?fields=banners,topExposedVideos,missionDonationChannelHomeExposure");
                            if (response != null)
                            {
                                JObject channel_live_detail = JObject.Parse(response);
                                liveTitle = channel_live_detail["content"]?["topExposedVideos"]?["openLive"]?["liveTitle"]?.ToString();
                                usersArray[i]["livetitle"] = liveTitle;
                                string templateUrl = channel_live_detail["content"]?["topExposedVideos"]?["openLive"]?["liveImageUrl"].ToString(); // liveImageUrl
                                int type = 480;

                                string liveImageUrl = templateUrl.Replace("{type}", type.ToString());
                                usersArray[i]["liveImageUrl"] = liveImageUrl;
                                //// 새로운 함수 호출
                                //await LoadLiveImageAsync(channelName, liveImageUrl);
                                if (!(bool)usersArray[i]["bangonallrm"])
                                {
                                    ShowNotification(channelName, $"{channelName}님이 방송 중입니다!\n방송 제목: {liveTitle}", usersArray[i]["channlid"].ToString());
                                    usersArray[i]["bangonallrm"] = true;
                                }
                            }
                        }
                        else
                        {
                            liveTitle = "방송중 아님";
                            usersArray[i]["livetitle"] = liveTitle;
                            usersArray[i]["bangonallrm"] = false;
                        }


                        usersArray[i]["channelName"] = channelName;
                        usersArray[i]["followerCount"] = followerCount;
                        usersArray[i]["openlive"] = openLive;
                        usersArray[i]["imageurl"] = channelImageUrl;
                        usersArray[i]["imagename"] = channelImagename;

                        if (!File.Exists($@"{imagedirectory}\{channelImagename}") & channelImagename != "ERROR-none-file.png")
                        {
                            usersArray[i]["imagedownload"] = false;
                            await dloadImageAsync(channelImageUrl, channelImagename);
                        }
                        else if (File.Exists($@"{imagedirectory}\{channelImagename}"))
                        {
                            usersArray[i]["imagedownload"] = true;
                        }


                    }
                }
                catch (Exception ex)
                {
                    printerror($"Api_get 함수에서 오류 발생 \n {ex}");
                }
                File.WriteAllText(JsonfileDirectory, data1.ToString());
                jsonData = data1;
            }
        }

        private async Task LoadLiveImageAsync(string channelName, string liveImageUrl)
        {
            try
            {
                if (channelName == stremer_name_label1.Text)
                {
                    await LoadImageAsync(liveImageUrl, 1);
                }
                else if (channelName == stremer_name_label2.Text)
                {
                    await LoadImageAsync(liveImageUrl, 2);
                }
                else if (channelName == stremer_name_label3.Text)
                {
                    await LoadImageAsync(liveImageUrl, 3);
                }
            }
            catch (Exception ex)
            {
                printerror($"LoadLiveImageAsync 함수에서 오류 발생 \n {ex}");
            }
        }





        // 이미지 다운로드 처리
        private async Task dloadImageAsync(string url, string filename)
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

            string filePath = Path.Combine("Images", filename);
            File.WriteAllBytes(filePath, imageBytes);
        }

        private async Task LoadImageAsync(string url, int page)
        {
            using HttpClient client = new HttpClient();
            try
            {
                byte[] imageData = await client.GetByteArrayAsync(url);
                using MemoryStream ms = new MemoryStream(imageData);
                Image image = Image.FromStream(ms);

                if (page == 1)
                {
                    // `BackgroundImage`에 이미지를 설정
                    back_ground_pannel1.BackgroundImage = image;
                    back_ground_pannel1.BackgroundImageLayout = ImageLayout.Stretch; // 이미지 레이아웃 설정 (Stretch, Tile 등)
                }
                if (page == 2)
                {
                    back_ground_pannel2.BackgroundImage = image;
                    back_ground_pannel2.BackgroundImageLayout = ImageLayout.Stretch; // 이미지 레이아웃 설정 (Stretch, Tile 등)
                }
                if(page == 3)
                {
                    back_ground_pannel3.BackgroundImage = image;
                    back_ground_pannel3.BackgroundImageLayout = ImageLayout.Stretch; // 이미지 레이아웃 설정 (Stretch, Tile 등)
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지를 가져오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        public List<string> openlivestremername = new List<string>();

        public List<string> pageforstremersname = new List<string>(); // 페이지별 스트리머 이름을 저장할 리스트
        public string GetUserName(int pageNumber, int userIndex, bool fullname)
        {
            // pageforstremersname 리스트에서 해당 페이지 번호를 찾아 출력
            if (pageforstremersname.Count >= pageNumber && pageNumber > 0)
            {
                // 해당 페이지 데이터 가져오기 (예: "Page 1: {user1, user2, user3}")
                string pageData = pageforstremersname[pageNumber - 1];

                // 페이지 데이터에서 유저 리스트 부분만 추출 (예: "user1, user2, user3")
                int startIndex = pageData.IndexOf("{") + 1;
                int endIndex = pageData.IndexOf("}");

                if (startIndex >= 0 && endIndex >= 0)
                {
                    string userList = pageData.Substring(startIndex, endIndex - startIndex); // "user1, user2, user3"

                    // 유저 리스트에서 유저 이름을 쉼표로 구분하여 배열로 분리
                    string[] users = userList.Split(','); // ["user1", "user2", "user3"]

                    // 유저 번호가 배열 범위 내에 있는지 확인
                    if (userIndex >= 0 && userIndex < users.Length)
                    {
                        // 유저 이름을 반환 (앞뒤 공백 제거)
                        if (users[userIndex].Length >= 10 & !fullname)
                        {
                            return users[userIndex].Trim().Substring(0, 10) + "..";
                        }
                        else if (users[userIndex].Length <= 10 & fullname)
                        {
                            return users[userIndex].Trim();
                        }
                        else
                        {
                            return users[userIndex].Trim();
                        }
                    }
                    else
                    {
                        //printerror($"페이지 {pageNumber}에 유효한 유저가 없습니다.");
                        //printerror(pageData);
                        return null;
                    }
                }
                else
                {
                    //printerror($"페이지 {pageNumber} 데이터 형식에 오류가 있습니다.");
                    //printerror(pageData);
                    return null;
                }
            }
            else
            {
                //printerror($"페이지 {pageNumber}가 존재하지 않습니다.");
                return null;
            }
        }

        private async void update_labe()
        {
            // UI 초기화
            ResetUI();

            // JSON 데이터에서 "users" 항목 추출
            JArray usersArray = (JArray)jsonData["users"];

            // openlivestremername 리스트를 초기화 (중복 방지)
            openlivestremername.Clear();

            // "openlive"가 true인 사용자들의 channelName을 openlivestremername에 추가
            foreach (var user in usersArray)
            {
                if (user["openlive"]?.ToString().Equals("True", StringComparison.OrdinalIgnoreCase) == true)
                {
                    openlivestremername.Add(user["channelName"].ToString());
                }
            }

            // 페이지 나누기
            int pageSize = 3; // 한 페이지에 들어갈 사용자 수
            int totalUsers = openlivestremername.Count;
            int totalPages = (totalUsers + pageSize - 1) / pageSize; // 총 페이지 수 계산
            mexpage = totalPages;

            // 페이지 정보 갱신
            pageforstremersname.Clear();
            for (int page = 0; page < totalPages; page++)
            {
                var pageUsers = openlivestremername.Skip(page * pageSize).Take(pageSize);
                pageforstremersname.Add($"Page {page + 1}: {{{string.Join(", ", pageUsers)}}}");
            }

            // 현재 페이지의 데이터로 UI 갱신
            for (int i = 0; i < pageSize; i++)
            {
                var username = GetUserName(thispage, i, true);
                if (username == null) continue;

                var info = usersArray.FirstOrDefault(u => u["channelName"].ToString() == username);
                if (info != null)
                {
                    await LoadLiveImageAsync(username, info["liveImageUrl"]?.ToString());
                }

                // UI 업데이트 (패널, 레이블, 배경)
                UpdatePanelAndLabels(i, username, (JObject) info);
            }
        }

        private void ResetUI()
        {
            // 모든 UI 요소를 숨김
            channle_image_panel1.Visible = false;
            channle_image_panel2.Visible = false;
            channle_image_panel3.Visible = false;
            no_open_live.Visible = false;
            back_ground_pannel1.Visible = false;
            back_ground_pannel2.Visible = false;
            back_ground_pannel3.Visible = false;

            // 스트리머 이름과 제목 초기화
            stremer_name_label1.Text = "";
            stremer_name_label2.Text = "";
            stremer_name_label3.Text = "";
            streming_title_label1.Text = "";
            streming_title_label2.Text = "";
            streming_title_label3.Text = "";

            // 이미지 초기화
            channle_image_panel1.BackgroundImage = null;
            channle_image_panel2.BackgroundImage = null;
            channle_image_panel3.BackgroundImage = null;
        }

        private async void UpdatePanelAndLabels(int index, string username, JObject info)
        {
            if (index == 0)
            {
                // JSON 데이터 읽기
                string jsonContent = File.Exists(SettingJsonDIrectory) ? File.ReadAllText(SettingJsonDIrectory) : "{}";
                JObject data = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

                // 설정 값 가져오기
                bool showLiveImage = data["setting"]?["showliveimage"]?.Value<bool>() ?? false;
                back_ground_pannel1.Visible = true;
                channle_image_panel1.Visible = true;
                back_ground_pannel1.Visible = true;
                stremer_name_label1.Text = username;
                streming_title_label1.Text = GetdataByName(username, "livetitle", true);
                var imageName = GetdataByName(username, "imagename", false);
                if (imageName != null)
                {
                    channle_image_panel1.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\{imageName}");
                    channle_image_panel1.BackgroundImageLayout = ImageLayout.Stretch;
                }
                if (showLiveImage)
                {
                    if (info != null)
                    {
                        //Print($"{GetUserName(thispage, 0, true)}, {info["liveImageUrl"].ToString()}");
                        await LoadLiveImageAsync(GetUserName(thispage, 0, true), info["liveImageUrl"].ToString());
                    }
                }
                else
                {
                    back_ground_pannel1.BackgroundImage = null;
                }
            }
            else if (index == 1)
            {
                // JSON 데이터 읽기
                string jsonContent = File.Exists(SettingJsonDIrectory) ? File.ReadAllText(SettingJsonDIrectory) : "{}";
                JObject data = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

                // 설정 값 가져오기
                bool showLiveImage = data["setting"]?["showliveimage"]?.Value<bool>() ?? false;
                back_ground_pannel1.Visible = true;
                channle_image_panel2.Visible = true;
                back_ground_pannel2.Visible = true;
                stremer_name_label2.Text = username;
                streming_title_label2.Text = GetdataByName(username, "livetitle", true);
                var imageName = GetdataByName(username, "imagename", false);
                if (imageName != null)
                {
                    channle_image_panel2.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\{imageName}");
                    channle_image_panel2.BackgroundImageLayout = ImageLayout.Stretch;
                }
                if (showLiveImage)
                {
                    if (info != null)
                    {
                        //Print($"{GetUserName(thispage, 0, true)}, {info["liveImageUrl"].ToString()}");
                        await LoadLiveImageAsync(GetUserName(thispage, 1, true), info["liveImageUrl"].ToString());
                    }
                }
                else
                {
                    back_ground_pannel2.BackgroundImage = null;
                }
            }
            else if (index == 2)
            {
                // JSON 데이터 읽기
                string jsonContent = File.Exists(SettingJsonDIrectory) ? File.ReadAllText(SettingJsonDIrectory) : "{}";
                JObject data = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

                // 설정 값 가져오기
                bool showLiveImage = data["setting"]?["showliveimage"]?.Value<bool>() ?? false;
                back_ground_pannel1.Visible = true;
                channle_image_panel3.Visible = true;
                back_ground_pannel3.Visible = true;
                stremer_name_label3.Text = username;
                streming_title_label3.Text = GetdataByName(username, "livetitle", true);
                var imageName = GetdataByName(username, "imagename", false);
                if (imageName != null)
                {
                    channle_image_panel3.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\{imageName}");
                    channle_image_panel3.BackgroundImageLayout = ImageLayout.Stretch;
                }
                if (showLiveImage)
                {
                    if (info != null)
                    {
                        //Print($"{GetUserName(thispage, 0, true)}, {info["liveImageUrl"].ToString()}");
                        await LoadLiveImageAsync(GetUserName(thispage, 2, true), info["liveImageUrl"].ToString());
                    }
                }
                else
                {
                    back_ground_pannel3.BackgroundImage = null;
                }
            }
        }




        private CancellationTokenSource _cts = new CancellationTokenSource();

        public async Task StartApiUpdateLoopAsync()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    api_update_labe(false);
                    await Task.Delay(TimeSpan.FromMinutes(1), _cts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // 타이머가 취소되면 예외가 발생하므로 이를 무시
            }
            catch (Exception ex)
            {
                printerror($"StartApiUpdateLoopAsync에서 오류 발생: {ex}");
            }
        }

        public void StopApiUpdateLoop()
        {
            _cts.Cancel();
        }



        private CancellationTokenSource _cancellationTokenSource;

        public async void AlarmLabelUpdate(int fontsize, Color color, string text, bool waitfor3sec)
        {
            try
            {
                // 이전 대기 작업 취소
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                // Label 업데이트
                label2.Font = new Font("CookieRun Bold", fontsize, FontStyle.Regular);
                label2.ForeColor = color;
                label2.Text = text;

                if (waitfor3sec)
                {
                    try
                    {
                        // 3초 대기 또는 취소 대기
                        await Task.Delay(3000, token);

                        // 대기 중 취소되지 않았다면 텍스트를 지움
                        if (!token.IsCancellationRequested)
                        {
                            label2.Text = "";
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // 취소 시 특별히 처리할 필요 없음
                    }
                }
            }
            catch (Exception ex)
            {
                printerror($"AlarmLabelUpdate에서 예외 발생 \n\n {ex}");
            }
        }

        public async void api_update_labe(bool sudoung)
        {
            try
            {
                await Api_get();
                update_labe();
                stremer_name_label1.Visible = true;
                stremer_name_label2.Visible = true;
                stremer_name_label3.Visible = true;
                streming_title_label1.Visible = true;
                streming_title_label2.Visible = true;
                streming_title_label3.Visible = true;
                if (sudoung)
                {
                    AlarmLabelUpdate(18, System.Drawing.Color.Lime, "스트리머\n 목록을 업데이트\n 하였습니다", true);
                }
                else
                {
                    AlarmLabelUpdate(18, System.Drawing.Color.Lime, "자동으로 스트리머\n 목록을 업데이트\n 하였습니다", true);
                }

            }
            catch (Exception ex)
            {
                printerror($"api_update_labe에서 오류 발생{ex}");
            }
        }

        private void reload_button_click(object sender, EventArgs e)
        {
            api_update_labe(true);
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

                        // JSON 데이터를 JObject 형식으로 파싱
                        JObject data = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

                        // "users" 배열 생성 또는 가져오기
                        JArray usersArray = data["users"] as JArray ?? new JArray();

                        // 가장 큰 stremerid 찾기
                        int maxStremerid = usersArray.Count > 0
                            ? usersArray.Max(user => (int)user["stremerid"])
                            : 0;

                        // 새 사용자 데이터 생성
                        JObject newUser = new JObject
                        {
                            ["stremerid"] = maxStremerid + 1, // 가장 큰 stremerid + 1
                            ["channelName"] = "noname",
                            ["openlive"] = false,
                            ["followerCount"] = 0,
                            ["livetitle"] = "notitle",
                            ["channlid"] = lastPart,
                            ["imageurl"] = "notload",
                            ["imagename"] = "notload",
                            ["imagedownload"] = "notload",
                            ["bangonallrm"] = false,
                            ["liveImageUrl"] = "none"
                        };

                        // "users" 배열에 새 데이터 추가
                        usersArray.Add(newUser);
                        data["users"] = usersArray;

                        // 변경된 JSON 데이터를 파일에 다시 저장
                        File.WriteAllText(JsonfileDirectory, data.ToString());



                        AlarmLabelUpdate(18, System.Drawing.Color.Lime, "추가 되었습니다", true);
                    }
                    catch (UriFormatException uriEx)
                    {

                        AlarmLabelUpdate(18, System.Drawing.Color.Red, "추가가 취소 \n되었습니다", true);
                        printerror("URL 형식이 잘못되었습니다.\n\n" + uriEx.Message);
                    }
                    catch (JsonException jsonEx)
                    {
                        AlarmLabelUpdate(18, System.Drawing.Color.Red, "추가가 취소 \n되었습니다", true);
                        printerror("JSON 처리 중 오류가 발생했습니다.\n\n" + jsonEx.Message);
                    }
                    catch (Exception ex)
                    {
                        AlarmLabelUpdate(18, System.Drawing.Color.Red, "추가가 취소 \n되었습니다", true);
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
                    AlarmLabelUpdate(18, System.Drawing.Color.Red, "추가가 취소 \n되었습니다", true);
                    printerror("url이 정상적이지 않습니다.");
                }
            }
            else
            {
                textBox2.Text = "";
                AlarmLabelUpdate(18, System.Drawing.Color.Red, "추가가 취소 \n되었습니다", true);
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

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            update_labe();
        }

        private void label1_Click(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/h7vWQR9VH4");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (thispage <= mexpage - 1)
            {
                thispage++;
                label3.Text = thispage.ToString();
                //현제 페이지 내 3 중 1스트리머의 이름 넣기 , 그 스트리머의 라이브 이미지 url값 넣기
                //LoadLiveImageAsync();
                update_labe();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (thispage >= 2)
            {
                thispage--;
                label3.Text = thispage.ToString();
                
                update_labe();
            }
        }

        private void no_open_live_MouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("https://chzzk.naver.com/");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dlete_user(int insu)
        {
            /// <summary>
            /// 유저 삭제 함수
            /// </summary>
            /// <param name="insu">현제 페이지의 몆번제 인수 인지 적으셈 0 부터 2까지임</param>
            if (MessageBox.Show("정말 삭제 하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 유저 json
                JArray usersArray = (JArray)jsonData["users"];
                string targetChannelName = GetUserName(thispage, insu, true);  // 삭제할 유저의 channelName
                if (insu == 0)
                {
                    channle_image_panel1.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\ERROR-none-file.png");
                    channle_image_panel1.BackgroundImageLayout = ImageLayout.Stretch;
                }
                if (insu == 1)
                {
                    channle_image_panel2.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\ERROR-none-file.png");
                    channle_image_panel2.BackgroundImageLayout = ImageLayout.Stretch;
                }
                if (insu == 2)
                {
                    channle_image_panel3.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\ERROR-none-file.png");
                    channle_image_panel3.BackgroundImageLayout = ImageLayout.Stretch;
                }

                // 해당 channelName을 가진 유저 삭제
                for (int i = usersArray.Count - 1; i >= 0; i--)
                {
                    if ((string)usersArray[i]["channelName"] == targetChannelName)
                    {
                        // 해당 유저 삭제
                        usersArray[i].Remove();
                        AlarmLabelUpdate(18, System.Drawing.Color.Lime, "삭제 되었습니다", true);
                        break;
                    }
                }

                // JSON 파일을 업데이트
                File.WriteAllText(JsonfileDirectory, jsonData.ToString());

                // API 업데이트
                api_update_labe(false);
            }
        }
        private void channle_image_panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dlete_user(0);
            }
            else if (e.Button == MouseButtons.Left)
            {
                string link = $"https://chzzk.naver.com/{GetdataByName(GetUserName(thispage, 0, true), "channlid", false)}";
                System.Diagnostics.Process.Start(link);
            }
        }

        private void channle_image_panel2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dlete_user(1);
            }
            else if (e.Button == MouseButtons.Left)
            {
                string link = $"https://chzzk.naver.com/{GetdataByName(GetUserName(thispage, 1, true), "channlid", false)}";
                System.Diagnostics.Process.Start(link);
            }
        }

        private void channle_image_panel3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dlete_user(2);
            }else if (e.Button == MouseButtons.Left)
            {
                string link = $"https://chzzk.naver.com/{GetdataByName(GetUserName(thispage, 2, true), "channlid", false)}";
                System.Diagnostics.Process.Start(link);
            }
        }

        private void channle_image_panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void visble_main_page(bool visble)
        {
            label2.Visible = visble;
            label4.Visible = visble;
            textBox2.Visible = visble;
            reload_button.Visible = visble;
            button4.Visible = visble;
            channle_image_panel1.Visible = visble;
            channle_image_panel2.Visible = visble;
            channle_image_panel3.Visible = visble;
            stremer_name_label1.Visible = visble;
            stremer_name_label2.Visible = visble;
            stremer_name_label3.Visible = visble;
            streming_title_label1.Visible = visble;
            streming_title_label2.Visible = visble;
            streming_title_label3.Visible = visble;
            panel2.Visible = visble;
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            visble_main_page(false);

        }

        private void form_panel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void channle_image_panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private bool settingformshow = false;
        private void opensetting_MouseClick(object sender, MouseEventArgs e)
        {
            
            //visble_main_page(false);
            Form2 form2 = new Form2();
            //settingjsonData["setting"]["firstrun"] = false;
            panel4.Size = new Size(477, 643);
            //Print(panel4.Size.ToString());
            //panel4.Location = new Point(8, 46);
            form2.TopLevel = false; // 최상위 윈도우 속성 비활성화
            form2.FormBorderStyle = FormBorderStyle.None; // 테두리 제거
            form2.Dock = DockStyle.Fill; // 패널 크기에 맞게 채우기
            panel4.Controls.Clear();
            panel4.Controls.Add(form2); // 패널에 추가
            form2.Show(); // 폼 표시

            //visble_main_page(settingformshow);
            panel4.Visible = !settingformshow;
            settingformshow = !settingformshow;
            set_label_color();
        }

        private void howtousebtn_MouseClick(object sender, MouseEventArgs e)
        {
            string link = @"https://scratch-maple-882.notion.site/1449098ea1a480ff89d3fdee4e334616";
            System.Diagnostics.Process.Start(link);
        }
    }
}

