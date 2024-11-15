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
            button5.FlatStyle = FlatStyle.Flat;
            button5.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.FlatAppearance.BorderSize = 0;
            no_open_live.Visible = false;
            update_labe();
            
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


        public string GetdataByName(string channelName , string dataname , bool fullname)
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
                Print($"다운로드 시간: {elapsed.TotalSeconds}초");

                Print($"이미지가 {savePath}에 저장되었습니다.");
            }
            catch (Exception e)
            {
                Print($"이미지 다운로드 오류: \nlink : {link}\n \n{e.Message}");
            }
        }


        public bool api_get_runing = false;

        public async Task api_get()
        {
            api_get_runing = true;
            // JSON 데이터를 파일에서 읽음
            string jsonContent = File.ReadAllText(JsonfileDirectory);

            // JSON 데이터를 JObject로 파싱
            JObject data1 = string.IsNullOrWhiteSpace(jsonContent) ? new JObject() : JObject.Parse(jsonContent);

            // users 배열 가져오기
            JArray usersArray = (JArray)data1["users"];

            for (int i = 0; i < usersArray.Count; i++)
            {
                string Targetid = usersArray[i]["channlid"].ToString();
                targetURL = "https://api.chzzk.naver.com/service/v1/channels/" + Targetid;
                api_get_runing = true;
                string result = string.Empty;
                usersArray = (JArray)data1["users"];
                try
                {
                    WebClient client = new WebClient();

                    // Add specific headers
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 13.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6286.210 Safari/537.36");
                    client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                    client.Headers.Add("Accept-Language", "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7,ja;q=0.6");
                    client.Headers.Add("Cache-Control", "max-age=0");
                    client.Headers.Add("Cookie", "NNB=5Z6ILX3ZKMLGO; nid_inf=490044677; NID_AUT=rFmati7co9izzzJzdUlQQurkpqn3DReAtXKyDo8MSPUrfuHeV6eEBP6xri5Mb6FY; NID_JKL=LChhy6zSGaeMi+xf/3QLtuuBUgNo0t7wUo2by5VgLmk=; loungesRecentlyVisited=chzzk; recentKeyword=%5B%5D; recentKeywordInLounge=%5B%5D; BUC=68tGDuP97gonv2Tan7GEzoTZFHI5s5Gxnt6KlEDxh8o=; NID_SES=AAABn8SBvwzaqNRgW7++R1Qy/MoVUI2vJDCdVN4TKXzn4wF7/qupemhbWQoUAw3dGNx+GF4pJeRgAz/4FUewe7a5DZSFkWKjMpkk5Hc0Z1LynDBAXWK/v9fkUNk8ze4JLAnqVHGhHZdoGbO5/Mw12naoe9PvARw96FgyjoM9YwMKC5N/HQ/aViVAzIWx8uJJJJOEoAeDsX7ZNNt4lKtwKKZxKTXyIBiBX7jRN8WqK9Ur+4QG3Oq+QFAlsO5giGJD9P42HCDKUBGGBqYzrLurahQ7Q0W+iLfj0C63UGwiVNT0LCxa9EMzSKYRyNPI//21GRM2DGlbNpxCYFA9zOGVf32PYpJ/WaoKOwvzcw6DKrTwuU8+CCfKxp+vVGrImzFHz9dHCayPnXCB3TZDVzhWjcqq6/mbqXCslobMPoUYBJ+Mx5BDbp2C6a8fjXCpgFkBFris6AnM6DGlgP8IpG0dToZZg+3tptfsu1rOjqusBp7YaYB/HfRfLJsepTlcMI3fvjV9NeDybotgWbdRuOxn4jIzX7kQn5NNl0mX6qi9B8YE8L2X; ba.uuid=0");
                    client.Headers.Add("Priority", "u=0, i");
                    client.Headers.Add("sec-ch-ua", "\"(Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"124\", \"Chromium\";v=\"124\"");
                    client.Headers.Add("sec-ch-ua-full-version-list", "\"(Not(A:Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"124\", \"Chromium\";v=\"124\"");
                    client.Headers.Add("sec-ch-ua-mobile", "?0");
                    client.Headers.Add("sec-ch-ua-platform", "\"macOS\"");
                    client.Headers.Add("sec-fetch-dest", "document");
                    client.Headers.Add("sec-fetch-mode", "navigate");
                    client.Headers.Add("sec-fetch-site", "none");
                    client.Headers.Add("sec-fetch-user", "?1");
                    client.Headers.Add("Upgrade-Insecure-Requests", "1");


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

                    string channelName = jsonResponse.content.channelName;
                    bool openlive = jsonResponse.content.openLive;
                    int followerCount = jsonResponse.content.followerCount;
                    string livetitle = "not streming";
                    string imageurl = "notload";
                    string imagename = "notload";

                    if (openlive)
                    {
                        targetURL = "https://api.chzzk.naver.com/service/v1/channels/" + Targetid + "/live-detail";
                        //https://api.chzzk.naver.com/service/v1/channels/5f44570b82fb82d4814ef9502cf6401a/live-detail
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
                        imageurl = jsonResponse.content.channel.channelImageUrl;

                        string url = imageurl;

                        // URL에서 경로 추출
                        Uri uri = new Uri(url);
                        string path = uri.AbsolutePath; // "/MjAyMzEyMTlfMTU3/MDAxNzAyOTcwMjUyNjMw.Z2tUcFtUCGZQ6au7-J4Y3AdQxZfvUflu4cGHrlmxPjog.Oiu5AzVJUchsNRrQhiZ3ic4n1fq5qlMGKoE7wCAvZDMg.JPEG/KakaoTalk_20201211_220843154.jpg"

                        // 필요한 부분만 추출
                        string[] parts = path.Split('/');
                        string targetPart = parts[2]; // "MjAyMzEyMTlfMTU3/MDAxNzAyOTcwMjUyNjMw.Z2tUcFtUCGZQ6au7-J4Y3AdQxZfvUflu4cGHrlmxPjog.Oiu5AzVJUchsNRrQhiZ3ic4n1fq5qlMGKoE7wCAvZDMg.JPEG"



                        imagename = targetPart;
                        if (usersArray[i]["imageurl"].ToString() == "notload" || usersArray[i]["imagedownload"].ToString() != "Load")
                        {//다운로드
                            try
                            {
                                await DownloadImageAsync(imageurl, imagename);
                                usersArray[i]["imagedownload"] = "Load";

                            }
                            catch (Exception ex)
                            {
                                usersArray[i]["imagedownload"] = "notload";
                                printerror($"이미지 다운로드중 에러 api요청 함수\n\n {ex}");
                            }
                        }
                    }


                    // Update user information in jsonData
                    usersArray[i]["channelName"] = channelName;
                    usersArray[i]["openlive"] = openlive;
                    usersArray[i]["followerCount"] = followerCount;
                    usersArray[i]["livetitle"] = livetitle;
                    usersArray[i]["imageurl"] = imageurl;
                    usersArray[i]["imagename"] = imagename;



                    //Print(usersArray.ToString());
                    //Print($"{usersArray[i]["channelName"]} \n\n {usersArray[i]["openlive"]} \n\n {usersArray[i]["followerCount"]} \n\n {usersArray[i]["livetitle"]}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: 채널 링크의 값을 확인 해주세요 \n\n{ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                finally
                {
                    api_get_runing = false;
                    targetURL = "notset";
                    // Save the updated JSON data back to the file after the loop finishes
                    File.WriteAllText(JsonfileDirectory, data1.ToString());
                    await Task.Delay(500);
                }
            }
            jsonData = data1;

        }


        public string GetUserName(int pageNumber, int userIndex)
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
                        return users[userIndex].Trim();
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

        public List<string> openlivestremername = new List<string>();
        public List<string> pageforstremersname = new List<string>(); // 페이지별 스트리머 이름을 저장할 리스트

        private void update_labe()
        {
            // JSON 데이터에서 "users" 항목을 추출
            JArray usersArray = (JArray)jsonData["users"];

            // openlivestremername 리스트를 초기화 (중복 방지)
            openlivestremername.Clear(); // 이전 데이터 초기화

            // "openlive"가 true인 사용자들의 channelName을 openlivestremername에 추가
            for (int i = 0; i < usersArray.Count; i++)
            {
                if (usersArray[i]["openlive"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    openlivestremername.Add(usersArray[i]["channelName"].ToString());
                }
            }

            // 페이지 나누기
            int pageSize = 3; // 한 페이지에 들어갈 사용자 수
            int totalUsers = openlivestremername.Count;
            int totalPages = (totalUsers + pageSize - 1) / pageSize; // 페이지 수 계산
            mexpage = totalPages;

            // pageforstremersname 리스트 초기화 (새로운 페이지 정보로 갱신)
            pageforstremersname.Clear();

            // 페이지별로 스트리머 이름 나누기
            for (int page = 0; page < totalPages; page++)
            {
                List<string> pageUsers = new List<string>(); // 현재 페이지의 사용자 리스트
                for (int i = page * pageSize; i < (page + 1) * pageSize && i < totalUsers; i++)
                {
                    pageUsers.Add(openlivestremername[i]);
                }

                // 페이지 정보를 string으로 만들어 리스트에 저장
                pageforstremersname.Add($"Page {page + 1}: {{{string.Join(", ", pageUsers)}}}");
            }

            channle_image_panel3.Visible = true;
            channle_image_panel2.Visible = true;
            channle_image_panel1.Visible = true;


            // 온라인 아닌 스트리머 숨기기
            if (GetUserName(thispage, 2) == null)
            {
                channle_image_panel3.Visible = false;
                no_open_live.Visible = false;
            }
            if (GetUserName(thispage, 1) == null)
            {
                channle_image_panel2.Visible = false;
                no_open_live.Visible = false;
            }
            if (GetUserName(thispage, 0) == null)
            {
                channle_image_panel1.Visible = false;
                no_open_live.Visible = true;
            }


                // 페이지 1에서 유저 이름을 올바르게 설정
            stremer_name_label1.Text = GetUserName(thispage, 0); // 첫 번째 유저
            stremer_name_label2.Text = GetUserName(thispage, 1); // 두 번째 유저
            stremer_name_label3.Text = GetUserName(thispage, 2); // 세 번째 유저

            // 제목 업데이트
            streming_title_label1.Text = GetdataByName(GetUserName(thispage, 0) , "livetitle" , true);
            streming_title_label2.Text = GetdataByName(GetUserName(thispage, 1), "livetitle", true);
            streming_title_label3.Text = GetdataByName(GetUserName(thispage, 2), "livetitle", true);

            // 이미지 업데이트
            if (GetdataByName(GetUserName(thispage, 0), "imagename", false) != null)
            {
                channle_image_panel1.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\{GetdataByName(GetUserName(thispage, 0), "imagename", false)}");
                channle_image_panel1.BackgroundImageLayout = ImageLayout.Stretch;
            }

            if (GetdataByName(GetUserName(thispage, 1), "imagename", false) != null)
            {
                channle_image_panel2.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\{GetdataByName(GetUserName(thispage, 1), "imagename", false)}");
                channle_image_panel2.BackgroundImageLayout = ImageLayout.Stretch;
            }
            if (GetdataByName(GetUserName(thispage, 2), "imagename", false) != null)
            {
                channle_image_panel3.BackgroundImage = Image.FromFile($@"{ThisDirectory}\images\{GetdataByName(GetUserName(thispage, 2), "imagename", false)}");
                channle_image_panel3.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }






        private async void reload_button_click(object sender, EventArgs e)
        {
            
            if (!api_get_runing)
            {
                update_labe();
                await api_get();
                update_labe();
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
                            ["imagedownload"] = "notload"
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
                        label2.Text = "추가가 취소 \n되었습니다";
                        label2.ForeColor = System.Drawing.Color.Red;
                        printerror("URL 형식이 잘못되었습니다.\n\n" + uriEx.Message);
                    }
                    catch (JsonException jsonEx)
                    {
                        label2.Text = "추가가 취소 \n되었습니다";
                        label2.ForeColor = System.Drawing.Color.Red;
                        printerror("JSON 처리 중 오류가 발생했습니다.\n\n" + jsonEx.Message);
                    }
                    catch (Exception ex)
                    {
                        label2.Text = "추가가 취소 \n되었습니다";
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
            if (thispage <= mexpage-1)
            {
                thispage++;
                label3.Text = thispage.ToString();
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
    }
}

