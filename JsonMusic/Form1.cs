using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using DevExpress.LookAndFeel;
using System.Drawing.Drawing2D;

namespace JsonMusic
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
            
        
    }
        public static int Pages=1;
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (textBox2.Text != "")
            {
                button1.Text = "搜索中...";
                button1.Enabled = false;
                Mcount = 0;
                dataGridView1.Rows.Clear();//清空上次的数据
                Thread a = new Thread(StartThread);
                a.IsBackground = true;
                toolStripStatusLabel1.Text = "当前关键字搜索："+textBox2.Text+"|";
                toolStripStatusLabel2.Text = "第："+Pages.ToString()+"页|";
                Pages = 1;
                a.Start();
            }
            else
                MessageBox.Show("请输入歌曲名称！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);



        }
        public static int Mcount = 0;//记录搜索个数
        void StartThread()
        {
             string Mn = textBox2.Text;
                    QQmusicGetdata(Mn, Pages); //关键字搜索 page页码
            BeginInvoke(new MethodInvoker(delegate {
             
                button1.Text = "搜索歌曲";
                button1.Enabled = true;
            }));
            

        }
        public void QQmusicGetdata(string Mkey,int page)//Mkey关键字搜索 page页码
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://c.y.qq.com/soso/fcgi-bin/client_search_cp?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=sizer.yqq.song_next&searchid=41304597472518464&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p="+page.ToString()+"&n=20&w="+Mkey+"&g_tk=5381&jsonpCallback=MusicJsonCallback03459405379530622&loginUin=0&hostUin=0&format=jsonp&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0");
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            //Console.WriteLine(retString);
            // string leftstr = retString.Substring(0, retString.IndexOf("("));//取出前缀防止下面json字符串转对象出错
            string right = retString.Substring(retString.IndexOf("{"), retString.Length - retString.Substring(0, retString.IndexOf("{")).Length);
            string temp = right.Substring(0, right.Length - 1);
            //textBox1.Text = temp;//这是已经OK的Json数据
            JObject obj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(temp);//字符串转对象
           for (int n = 0; n < obj["data"]["song"]["list"].Count(); n++)
            {
                QQMusic a = new QQMusic();
                a.s_mid = obj["data"]["song"]["list"][n]["mid"].ToString();//取歌曲ID
                //a.s_mid = obj["data"]["song"]["list"][n]["mid"].ToString();
                a.s_name = obj["data"]["song"]["list"][n]["title"].ToString();  //取歌曲名称
                a.author_name = "";//取作者名称
                if (obj["data"]["song"]["list"][n]["singer"].Count() <= 0)//单个作者
                    a.author_name = obj["data"]["song"]["list"][n]["singer"][0]["name"].ToString();
                else
                {
                    //多个作者
                    a.author_name = obj["data"]["song"]["list"][n]["singer"][0]["name"].ToString();
                    for (int ai = 1; ai < obj["data"]["song"]["list"][n]["singer"].Count(); ai++)
                    {
                        a.author_name += "/" + obj["data"]["song"]["list"][n]["singer"][ai]["name"].ToString();

                    }
                }
                a.album_name = obj["data"]["song"]["list"][n]["album"]["title"].ToString();//取专辑名称
                //神奇的委托
                BeginInvoke(new MethodInvoker(delegate {
                    int index;
                    index = dataGridView1.Rows.Add();
                    
                    dataGridView1.Rows[index].Cells[0].Value = a.s_name;
                    dataGridView1.Rows[index].Cells[1].Value = a.author_name;
                    dataGridView1.Rows[index].Cells[2].Value = a.album_name;
                    dataGridView1.Rows[index].Cells[3].Value = a.s_mid;
                    Mcount++;
                    //textBox1.AppendText("歌曲ID:" + a.s_mid + "    歌曲:" + a.s_name + "    作者:" + a.author_name + "    专辑:" + a.album_name + '\n');
                }));


                //这里是因为歌曲可能有相同的但是专辑不一样，在QQ音乐里是只显示一个其它相同的会不显示但是在他那首歌会有下拉框可以显示
               for (int i = 0; i < obj["data"]["song"]["list"][n]["grp"].Count(); i++)
                {
                    QQMusic a1 = new QQMusic();
                    a1.s_mid = obj["data"]["song"]["list"][n]["grp"][i]["file"]["media_mid"].ToString();//取歌曲ID
                    a1.s_name = obj["data"]["song"]["list"][n]["grp"][i]["title"].ToString();  //取歌曲名称
                    a1.author_name = "";//取作者名称
                    if (obj["data"]["song"]["list"][n]["grp"][i]["singer"].Count() <= 0)//单个作者
                        a1.author_name = obj["data"]["song"]["list"][n]["grp"][i]["singer"][0]["name"].ToString();
                    else
                    {
                        //多个作者
                        a1.author_name = obj["data"]["song"]["list"][n]["grp"][i]["singer"][0]["name"].ToString();
                        for (int ai = 1; ai < obj["data"]["song"]["list"][n]["grp"][i]["singer"].Count(); ai++)
                        {
                            a1.author_name += "/" + obj["data"]["song"]["list"][n]["grp"][i]["singer"][ai]["name"].ToString();

                        }
                    }
                    a1.album_name = obj["data"]["song"]["list"][n]["grp"][i]["album"]["title"].ToString();//取专辑名称
                                                                                                          //神奇的委托
                    BeginInvoke(new MethodInvoker(delegate {
                        int index;
                        index = dataGridView1.Rows.Add();
                       
                        dataGridView1.Rows[index].Cells[0].Value = a1.s_name;
                        dataGridView1.Rows[index].Cells[1].Value = a1.author_name;
                        dataGridView1.Rows[index].Cells[2].Value = a1.album_name;
                        dataGridView1.Rows[index].Cells[3].Value = a1.s_mid;
                        Mcount++;
                        //textBox1.AppendText("歌曲ID:" + a.s_mid + "    歌曲:" + a.s_name + "    作者:" + a.author_name + "    专辑:" + a.album_name + '\n');
                    }));

                }

            } 

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        void GetMusic(string Mid)
        {
            try
            {
                QQMusic sa = new QQMusic();
                JObject obj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(sa.Jtext(Mid));//字符串转对象
                string Music =obj["req_0"]["data"]["midurlinfo"][0]["purl"].ToString();//这里是歌曲的原生链接的尾部
                string host = "http://58.216.106.17/amobile.music.tc.qq.com/";//这里是原生歌曲链接的首部----需抓包分析
                axWindowsMediaPlayer1.URL = host + Music;


            }
            catch
            {
                MessageBox.Show("数据出错！", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(dataGridView1, new Point(e.X, e.Y));
            
        }

      
        private void 播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                GetMusic(dataGridView1.Rows[index].Cells[3].Value.ToString());
                toolStripStatusLabel3.Text = "播放音乐：" + dataGridView1.Rows[index].Cells[1].Value.ToString() + "-" + dataGridView1.Rows[index].Cells[0].Value.ToString()+"|";
                toolStripStatusLabel4.Text = "当前播放状态：正常播放";
            }
            catch {
                MessageBox.Show("请选择您要播放的音乐","系统提示：",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            
        }
        public void GetMusicDownload(string Mid,int index)//选中行数
        {
            
                try
                {

                QQMusic sa = new QQMusic();
                JObject obj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(sa.Jtext(Mid));//字符串转对象
                    string Music = obj["req_0"]["data"]["midurlinfo"][0]["purl"].ToString();//这里是歌曲的原生链接的尾部
                    string host = "http://58.216.106.17/amobile.music.tc.qq.com/" + Music;//这里是原生歌曲链接
                     try
                        {
                            string file = "C:\\" + dataGridView1.Rows[index].Cells[0].Value.ToString() + "-" + dataGridView1.Rows[index].Cells[1].Value.ToString()  + ".mp3";
                           
                            if (sa.DownloadMusic(file,host ))//文件保存地址file 原生链接host
                                MessageBox.Show("下载完成请前往C盘查看！", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            else
                                MessageBox.Show("下载失败", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                         }
                    catch
                        {
                            MessageBox.Show("暂无选中歌曲需要下载！", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }


            }
                catch
                {
                    MessageBox.Show("数据读取出错！", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            
        }

       

        private void 下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread a = new Thread(FileDownload);
            a.IsBackground = true;
            a.Start();
           
            
        }
        public void FileDownload()
        {
            try
            {
                
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                MessageBox.Show("歌曲正在下载....");
                GetMusicDownload(dataGridView1.Rows[index].Cells[3].Value.ToString(),index);

            }
            catch
            {
                MessageBox.Show("请选择您要下载的音乐", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void 分享ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                QQMusic a = new QQMusic();
                JObject obj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(a.Jtext(dataGridView1.Rows[index].Cells[3].Value.ToString()));//字符串转对象
                string Music = obj["req_0"]["data"]["midurlinfo"][0]["purl"].ToString();//这里是歌曲的原生链接的尾部
                string host = "http://58.216.106.17/amobile.music.tc.qq.com/"+Music;//这里是原生歌曲链接
                Clipboard.SetText(host);
                MessageBox.Show("信息已复制到您的剪辑版上赶快分享吧！", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch
            {
                MessageBox.Show("暂无歌曲要分享", "系统提示：", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UserLookAndFeel.Default.SetSkinStyle("Valentine");//加载软件默认皮肤
            Thread a = new Thread(Tim);//取QQ头像
            a.IsBackground = true;
            a.Start();
        }
        public void Tim()
        {
            GetImage();
        }
        public void GetImage()
        {
            BeginInvoke(new MethodInvoker(delegate {
                try
                {
                    //圆角图标
                    pictureBox1.Image = Image.FromStream(WebRequest.Create("http://q2.qlogo.cn/headimg_dl?bs=1647762341&dst_uin=1647762341&dst_uin=1647762341&;dst_uin=1647762341&spec=100&url_enc=0&referer=bu_interface&term_type=PC").GetResponse().GetResponseStream());
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(pictureBox1.ClientRectangle);
                    Region region = new Region(gp);
                    pictureBox1.Region = region;
                    gp.Dispose();
                    region.Dispose();
                }
                catch { }
            }));
           }
        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                button1.Focus();
        }

        private void 取消播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            toolStripStatusLabel4.Text = "  当前播放状态：停止播放";
        }

        private void 下一页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                button1.Text = "搜索中...";
                button1.Enabled = false;
                Mcount = 0;
                dataGridView1.Rows.Clear();//清空上次的数据
                Thread a = new Thread(StartThread);
                a.IsBackground = true;
                Pages++;
                toolStripStatusLabel1.Text = "当前关键字搜索：" + textBox2.Text+"|";
                toolStripStatusLabel2.Text = "第：" + Pages.ToString() + "页|";
                a.Start();
            }
            else
                MessageBox.Show("请输入歌曲名称！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void 上一页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && Pages>1)
            {
                button1.Text = "搜索中...";
                button1.Enabled = false;
                Mcount = 0;
                dataGridView1.Rows.Clear();//清空上次的数据
                Thread a = new Thread(StartThread);
                a.IsBackground = true;
                Pages--;
                toolStripStatusLabel1.Text = "当前关键字搜索：" + textBox2.Text+"|";
                toolStripStatusLabel2.Text = "第：" + Pages.ToString() + "页|";
                a.Start();
            }
            else
                MessageBox.Show("请输入歌曲名称【已是首页】", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void 首页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                button1.Text = "搜索中...";
                button1.Enabled = false;
                Mcount = 0;
                dataGridView1.Rows.Clear();//清空上次的数据
                Thread a = new Thread(StartThread);
                a.IsBackground = true;
                Pages=1;
                toolStripStatusLabel1.Text = "当前关键字搜索：" + textBox2.Text + "|";
                toolStripStatusLabel2.Text = "第：" + Pages.ToString() + "页|";
                a.Start();
            }
            else
                MessageBox.Show("请输入歌曲名称！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
