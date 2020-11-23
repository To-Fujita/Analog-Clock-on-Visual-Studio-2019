// Analog Clock by T. Fujita on 2020/11/23

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Timers;
using System.IO;

namespace Analog_Clock
{
    public partial class Form1 : Form
    {
        Bitmap Clock_Face = Properties.Resources.Clock_Face_006;        // 時計の文字盤
        Bitmap Clock_Second = Properties.Resources.Clock_Hand_001s;     // 秒針
        Bitmap Clock_Minute = Properties.Resources.Clock_Hand_001m;     // 長針
        Bitmap Clock_Hour = Properties.Resources.Clock_Hand_001h;       // 短針
        public static int Face_No = 5;              // 文字盤の選択番号
        public static int Face_flag = 0;            // 文字盤変更の有無フラグ（0:無、1:有）
        public static int Hands_No = 0;             // 針の選択番号
        public static int Hands_flag = 0;           // 針変更の有無フラグ（0:無、1:有）
        public static int Size_flag = 0;            // サイズ変更の有無フラグ（0:無、1:有）
        public static int Pos_flag = 0;             // 表示位置変更の有無フラグ（0:無、1:有）
        public static int Size_Max;                 // 時計サイズの最大値
        public static int Size_Min = 64;            // 時計サイズの最小値
        public static int Size_Data = 300;          // 時計サイズ
        public static int Scr_X;                    // 横画面サイズ
        public static int Scr_Y;                    // 縦画面サイズ
        public static int Pos_X = 400;              // 表示位置X
        public static int Pos_Y = 300;              // 表示位置Y
        public static int T_Signal_flag = 0;        // 時報の有無フラグ（0:無、1:有）
        public static int Alarm_flag = 0;           // アラームの有無フラグ（0:無、1:有）
        public static int Alarm_No = 0;             // アラームの音色番号
        public static int Alarm_H = 7;              // アラーム設定（時）
        public static int Alarm_M = 0;              // アラーム設定（分）
        public static double Opacity_Data = 0.8;    // 不透明度の指定値
        public static System.Media.SoundPlayer player_T_Signal = null;      // 時報のサウンドプレーヤー
        public static System.Media.SoundPlayer player_Alarm = null;         // アラームのサウンドプレーヤー
        public static string[] ClockFaceData = new string[8];               // 時計の文字盤名称
        public static string[] ClockHandsData = new string[6];              // 時計の針の名称
        public delegate void Delegate();            // Invoke()を使用するため、関数を変数のように扱う為のもの
        public static int Time_Count = 0;

        public Form1()
        {
            InitializeComponent();

            ClockFaceData[0] = "List-1(シンプル)";
            ClockFaceData[1] = "List-2(アラビア数字)";
            ClockFaceData[2] = "List-3(ローマ数字)";
            ClockFaceData[3] = "List-4(アラビア数字)";
            ClockFaceData[4] = "List-5(ローマ数字)";
            ClockFaceData[5] = "List-6(ローマ数字)";
            ClockFaceData[6] = "List-7(アラビア数字)中型";
            ClockFaceData[7] = "List-8(目覚まし時計)小型";

            ClockHandsData[0] = "List-1(長い針)";
            ClockHandsData[1] = "List-2(長い針／赤の秒針)";
            ClockHandsData[2] = "List-3(中型の針)";
            ClockHandsData[3] = "List-4(中型の針／赤の秒針)";
            ClockHandsData[4] = "List-5(短い針)";
            ClockHandsData[5] = "List-6(短い針／赤の秒針)";

            CSV_Read CSV_Load = new CSV_Read();
            try
            {
                CSV_Load.Read_CSV();
            }
            catch (Exception)
            {
                CSV_Save CSV_Write = new CSV_Save();
                CSV_Write.Save_CSV();
            }

            player_T_Signal = new System.Media.SoundPlayer(Properties.Resources.Zihou01_1);
            if (Alarm_No == 0)
            {
                player_Alarm = new System.Media.SoundPlayer(Properties.Resources.Clock_Alarm02_1_Loop_);
            }
            else if (Alarm_No == 1)
            {
                player_Alarm = new System.Media.SoundPlayer(Properties.Resources.Clock_Alarm04_01);
            }
            else
            {
                player_Alarm = new System.Media.SoundPlayer(Properties.Resources.Japanese_School_Bell01_1);
            }

            this.FormBorderStyle = FormBorderStyle.None;        // Windowのタイトルバーと境界線を消す
            this.TopMost = true;                                // 最前面表示
            this.Opacity = Opacity_Data;                        // 透過度の設定

            Scr_X = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            Scr_Y = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            if (Scr_X > Scr_Y)
            {
                Size_Max = Scr_Y;
            }
            else
            {
                Size_Max = Scr_X;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(Size_Data, Size_Data);                 // 時計のサイズ
            this.Location = new Point(Pos_X, Pos_Y);                    // 時計の表示位置
            System.Drawing.Graphics g;

            pictureBox1.Dock = DockStyle.Fill;                          // pictureBoxをForm1に合わせて適切なサイズに調節
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;             // pictureBoxをサイズ比率を維持して拡大・縮小

            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBox3.Dock = DockStyle.Fill;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBox4.Dock = DockStyle.Fill;
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

            Clock_Face.MakeTransparent();
            Clock_Hour.MakeTransparent();
            Clock_Minute.MakeTransparent();
            Clock_Second.MakeTransparent();

            g = pictureBox2.CreateGraphics();
            g.DrawImage(Clock_Hour, new System.Drawing.Point(0, 0));
            g.Dispose();

            g = pictureBox3.CreateGraphics();
            g.DrawImage(Clock_Minute, new System.Drawing.Point(0, 0));
            g.Dispose();

            g = pictureBox4.CreateGraphics();
            g.DrawImage(Clock_Second, new System.Drawing.Point(0, 0));
            g.Dispose();

            pictureBox1.Image = Clock_Face;

            pictureBox2.Parent = pictureBox1;
            pictureBox3.Parent = pictureBox2;
            pictureBox4.Parent = pictureBox3;

            pictureBox2.BackColor = Color.Transparent;              // 画像を重ねるため、背景色を透過にする
            pictureBox3.BackColor = Color.Transparent;
            pictureBox4.BackColor = Color.Transparent;

            System.Timers.Timer timer = new System.Timers.Timer(200);
            timer.Elapsed += (sender_Temp, e_Temp) =>
            {
                Draw_Clock();

                if (Size_flag == 1)
                {
                    this.Invoke(new Delegate(this.Size_Set));
                    Size_flag = 0;
                }
                if (Pos_flag == 1)
                {
                    this.Invoke(new Delegate(this.Position_Set));
                    Pos_flag = 0;
                }
            };
            timer.Start();
        }

        // 時計の表示
        private void Draw_Clock()
        {
            if (Face_flag == 1)
            {
                Clock_Face = Properties.Resources.Clock_Face_001;
                if (Face_No == 1)
                {
                    Clock_Face = Properties.Resources.Clock_Face_002;
                }
                if (Face_No == 2)
                {
                    Clock_Face = Properties.Resources.Clock_Face_003;
                }
                if (Face_No == 3)
                {
                    Clock_Face = Properties.Resources.Clock_Face_004;
                }
                if (Face_No == 4)
                {
                    Clock_Face = Properties.Resources.Clock_Face_005;
                }
                if (Face_No == 5)
                {
                    Clock_Face = Properties.Resources.Clock_Face_006;
                }
                if (Face_No == 6)
                {
                    Clock_Face = Properties.Resources.Clock_Face_007;
                }
                if (Face_No == 7)
                {
                    Clock_Face = Properties.Resources.Clock_Face_008;
                }
                Face_flag = 0;
                pictureBox1.Image = Clock_Face;
            }

            if (Hands_flag == 1)
            {
                System.Drawing.Graphics g;
                Clock_Hour = Properties.Resources.Clock_Hand_001h;
                Clock_Minute = Properties.Resources.Clock_Hand_001m;
                Clock_Second = Properties.Resources.Clock_Hand_001s;
                if (Hands_No == 1)
                {
                    Clock_Hour = Properties.Resources.Clock_Hand_001h;
                    Clock_Minute = Properties.Resources.Clock_Hand_001m;
                    Clock_Second = Properties.Resources.Clock_Hand_001sR;
                }
                if (Hands_No == 2)
                {
                    Clock_Hour = Properties.Resources.Clock_Hand_002h;
                    Clock_Minute = Properties.Resources.Clock_Hand_002m;
                    Clock_Second = Properties.Resources.Clock_Hand_002s;
                }
                if (Hands_No == 3)
                {
                    Clock_Hour = Properties.Resources.Clock_Hand_002h;
                    Clock_Minute = Properties.Resources.Clock_Hand_002m;
                    Clock_Second = Properties.Resources.Clock_Hand_002sR;
                }
                if (Hands_No == 4)
                {
                    Clock_Hour = Properties.Resources.Clock_Hand_003h;
                    Clock_Minute = Properties.Resources.Clock_Hand_003m;
                    Clock_Second = Properties.Resources.Clock_Hand_003s;
                }
                if (Hands_No == 5)
                {
                    Clock_Hour = Properties.Resources.Clock_Hand_003h;
                    Clock_Minute = Properties.Resources.Clock_Hand_003m;
                    Clock_Second = Properties.Resources.Clock_Hand_003sR;
                }
                Hands_flag = 0;
                g = pictureBox2.CreateGraphics();
                g.DrawImage(Clock_Hour, new System.Drawing.Point(0, 0));
                g.Dispose();

                g = pictureBox3.CreateGraphics();
                g.DrawImage(Clock_Minute, new System.Drawing.Point(0, 0));
                g.Dispose();

                g = pictureBox4.CreateGraphics();
                g.DrawImage(Clock_Second, new System.Drawing.Point(0, 0));
                g.Dispose();
            }

            DateTime time = DateTime.Now;
            float SecondAng = (float)(time.Second * 6.0);
            float MinuteAng = (float)((time.Minute + time.Second / 60.0) * 6.0);
            float HourAng = (float)((time.Hour + time.Minute / 60.0) * 30.0);

            if (Time_Count == 0)
            {
                pictureBox4.Image = RotateBitmap(Clock_Second, SecondAng, Clock_Second.Width / 2, Clock_Second.Height / 2);
            }
            if (Time_Count == 1)
            {
                pictureBox3.Image = RotateBitmap(Clock_Minute, MinuteAng, Clock_Minute.Width / 2, Clock_Minute.Height / 2);
            }
            if (Time_Count == 2)
            {
                pictureBox2.Image = RotateBitmap(Clock_Hour, HourAng, Clock_Hour.Width / 2, Clock_Hour.Height / 2);
            }

            Time_Count = Time_Count + 1;
            if (Time_Count > 4)
            {
                Time_Count = 0;
            }

            if (T_Signal_flag == 1)
            {
                if ((int)time.Minute == 59 && (int)time.Second == 57)
                {
                    player_T_Signal.Play();
                }
            }

            if (Alarm_flag == 1)
            {
                if ((int)time.Hour == Alarm_H && (int)time.Minute == Alarm_M && (int)time.Second == 0)
                {
                    player_Alarm.Play();
                }
            }
        }

        // 時計の針画像の回転
        public Bitmap RotateBitmap(Bitmap org_bmp, float angle, int x, int y)
        {
            Bitmap result_bmp = new Bitmap((int)org_bmp.Width, (int)org_bmp.Height);
            Graphics g = Graphics.FromImage(result_bmp);

            g.TranslateTransform(-x, -y);
            g.RotateTransform(angle, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.TranslateTransform(x, y, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(org_bmp, 0, 0);
            g.Dispose();

            return result_bmp;
        }

        // 時計のサイズ設定
        private void Size_Set()
        {
            this.Size = new Size(Size_Data, Size_Data);
        }

        // 時計の位置設定
        private void Position_Set()
        {
            this.Location = new Point(Pos_X, Pos_Y);
        }

        // Context MenuでExitを選択した場合の終了処理
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Dispose();
            pictureBox2.Dispose();
            pictureBox3.Dispose();
            pictureBox4.Dispose();
            Application.Exit();
        }

        // Context MenuでClock_Faceを選択した場合のダイアログボックス表示
        private void clockFaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog_Display dialog1 = new Dialog_Display();
            dialog1.Show();
        }

        private void locationSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog_Pos_Size dialog1 = new Dialog_Pos_Size();
            dialog1.Show();
        }

        // Context MenuでAlarm_Etcを選択した場合のダイアログボックス表示
        private void alarmSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog_Sounds dialog1 = new Dialog_Sounds();
            dialog1.Show();
        }

        // Context MenuでAlarm_OFFを選択した場合の処理
        private void alarmOFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player_T_Signal.Stop();
            player_Alarm.Stop();
        }

    }
    // Display用のダイアログボックス
    public class Dialog_Display : Form
    {
        Label label_Face;
        Label label_Hands;
        Label label_Opacity;
        Label label_OpacityInfo;
        ListBox listBox_Face;
        ListBox listBox_Hands;
        public TextBox textBox_Face;
        public TextBox textBox_Hands;
        public TextBox textBox_Opacity;
        public TrackBar trackbar_Opacity;
        public string Selected_Face;
        public string Selected_Hands;
        Button Close_button;

        public Dialog_Display()
        {
            // ダイアログボックスの設定
            this.MaximizeBox = false;                                   // 最大化ボタン
            this.MinimizeBox = false;                                   // 最小化ボタン
            this.ShowInTaskbar = true;                                  // タスクバー上に表示
            this.FormBorderStyle = FormBorderStyle.FixedDialog;         // 境界のスタイル
            this.StartPosition = FormStartPosition.Manual;              // 任意の位置に表示
            this.Size = new Size(300, 400);                             // ダイアログボックスのサイズ指定
            this.Location = new Point(100, 100);                        // ダイアログボックスの表示位置
            this.Text = "Set the Analog Clock";                         // タイトル

            label_Face = new Label()
            {
                Text = "時計の文字盤選択：",
                Location = new Point(10, 10),
            };
            label_Face.Size = new Size(120, 25);
            listBox_Face = new ListBox()
            {
                Location = new Point(50, 35),
            };
            listBox_Face.Size = new Size(200, 60);
            for (int i = 0; i < Form1.ClockFaceData.Length; i++)
            {
                listBox_Face.Items.Add(Form1.ClockFaceData[i]);
            }
            listBox_Face.SelectedIndexChanged += ListBox_Face_SelectedIndexChanged;
            Selected_Face = Form1.ClockFaceData[Form1.Face_No];
            textBox_Face = new TextBox()
            {
                Text = Selected_Face,
                Location = new Point(130, 5),
                ReadOnly = true,
            };
            textBox_Face.Size = new Size(150, 25);
            label_Hands = new Label()
            {
                Text = "時計の針選択：",
                Location = new Point(10, 100),
            };
            label_Hands.Size = new Size(100, 25);
            listBox_Hands = new ListBox()
            {
                Location = new Point(50, 125),
            };
            listBox_Hands.Size = new Size(180, 60);
            for (int i = 0; i < Form1.ClockHandsData.Length; i++)
            {
                listBox_Hands.Items.Add(Form1.ClockHandsData[i]);
            }
            listBox_Hands.SelectedIndexChanged += ListBox_Hands_SelectedIndexChanged;
            Selected_Hands = Form1.ClockHandsData[Form1.Hands_No];
            textBox_Hands = new TextBox()
            {
                Text = Selected_Hands,
                Location = new Point(120, 95),
                ReadOnly = true,
            };
            textBox_Hands.Size = new Size(160, 25);

            label_Opacity = new Label()
            {
                Text = "時計の不透明度（％）：",
                Location = new Point(10, 210),
            };
            label_Opacity.Size = new Size(120, 25);
            label_OpacityInfo = new Label()
            {
                Text = "不透明度は、本ソフト再立ち上げ後有効になります。",
                Location = new Point(10, 235),
            };
            label_OpacityInfo.Size = new Size(280, 25);
            textBox_Opacity = new TextBox()
            {
                Text = (Form1.Opacity_Data * 100).ToString(),
                Location = new Point(150, 205),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
            };
            textBox_Opacity.Size = new Size(50, 25);
            trackbar_Opacity = new TrackBar()
            {
                Location = new Point(20, 260),
                Minimum = 0,
                Maximum = 100,
                Value = (int)(Form1.Opacity_Data * 100),
                TickFrequency = 10,
                SmallChange = 1,
                LargeChange = 10,
                AutoSize = false,
                Size = new Size(240, 40),
            };
            trackbar_Opacity.ValueChanged += Trackbar_Opacity_ValueChanged;

            Close_button = new Button()
            {
                Text = "Close",
                Location = new Point(100, 320),
            };
            Close_button.Click += new EventHandler(Close_button_Clicked);

            this.Controls.AddRange(new Control[]
            {
                label_Face, label_Hands, label_Opacity, label_OpacityInfo,
                listBox_Face, listBox_Hands,
                textBox_Opacity, textBox_Face, textBox_Hands,
                trackbar_Opacity,
                Close_button
            });
        }

        // 時計の文字盤を変更した時の処理
        void ListBox_Face_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1.Face_No = listBox_Face.SelectedIndex;
            Form1.Face_flag = 1;
            textBox_Face.Text = Form1.ClockFaceData[Form1.Face_No];
        }

        // 時計の針を変更した時の処理
        void ListBox_Hands_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1.Hands_No = listBox_Hands.SelectedIndex;
            Form1.Hands_flag = 1;
            textBox_Hands.Text = Form1.ClockHandsData[Form1.Hands_No];
        }

        // 不透明度の値が変更された時の処理
        void Trackbar_Opacity_ValueChanged(object s, EventArgs e)
        {
            textBox_Opacity.Text = trackbar_Opacity.Value.ToString();
            Form1.Opacity_Data = trackbar_Opacity.Value / 100.0;
        }

        // クローズボタンが押された時の処理
        void Close_button_Clicked(object sender, EventArgs e)
        {
            CSV_Save CSV_Write = new CSV_Save();
            CSV_Write.Save_CSV();
            this.Close();
        }
    }

    // アラーム用ダイアログボックス
    public class Dialog_Sounds : Form
    {
        Label label_T_Signal;
        Label label_Alarm;
        Label label_Alarm_S;
        Label label_Alarm_HM;
        public TextBox textBox_T_Signal;
        public TextBox textBox_Alarm;
        public TextBox textBox_Alarm_Info;
        public TextBox textBox_Alarm_H;
        public TextBox textBox_Alarm_M;
        public TrackBar trackbar_Alarm_H;
        public TrackBar trackbar_Alarm_M;
        Button T_Signal_button;
        Button Alarm_button;
        Button Close_button;
        ListBox listBox_Alarm;
        string T_Signal_Text;
        string Alarm_Text;
        string Alarm_Info_Text;

        public Dialog_Sounds()
        {
            // ダイアログボックスの設定
            this.MaximizeBox = false;                                   // 最大化ボタン
            this.MinimizeBox = false;                                   // 最小化ボタン
            this.ShowInTaskbar = true;                                  // タスクバー上に表示
            this.FormBorderStyle = FormBorderStyle.FixedDialog;         // 境界のスタイル
            this.StartPosition = FormStartPosition.Manual;              // 任意の位置に表示
            this.Size = new Size(300, 400);                             // ダイアログボックスのサイズ指定
            this.Location = new Point(100, 100);                        // ダイアログボックスの表示位置
            this.Text = "時報とアラーム";                               // タイトル

            if (Form1.T_Signal_flag == 0)
            {
                T_Signal_Text = "時報OFF";
            }
            else
            {
                T_Signal_Text = "時報ON";
            }
            T_Signal_button = new Button()
            {
                Text = "時報 ON / OFF",
                Location = new Point(10, 10),
            };
            T_Signal_button.Size = new Size(130, 24);
            label_T_Signal = new Label()
            {
                Text = T_Signal_Text,
                Location = new Point(160, 14),
            };
            label_T_Signal.Size = new Size(150, 25);
            T_Signal_button.Click += new EventHandler(T_Signal_button_Clicked);

            if (Form1.Alarm_flag == 0)
            {
                Alarm_Text = "アラームOFF";
            }
            else
            {
                Alarm_Text = "アラームON";
            }
            Alarm_button = new Button()
            {
                Text = "アラーム ON / OFF",
                Location = new Point(10, 50),
            };
            Alarm_button.Size = new Size(130, 25);
            label_Alarm = new Label()
            {
                Text = Alarm_Text,
                Location = new Point(160, 54),
            };
            label_Alarm.Size = new Size(150, 25);
            Alarm_button.Click += new EventHandler(Alarm_button_Clicked);
            label_Alarm_S = new Label()
            {
                Text = "アラーム音の選択：",
                Location = new Point(10, 95),
            };
            label_Alarm_S.Size = new Size(120, 25);
            listBox_Alarm = new ListBox()
            {
                Location = new Point(50, 120),
            };
            listBox_Alarm.Size = new Size(200, 50);
            listBox_Alarm.Items.Add("List-1(電子音)");
            listBox_Alarm.Items.Add("List-2(ベル音)");
            listBox_Alarm.Items.Add("List-3(学校のチャイム)");
            listBox_Alarm.SelectedIndexChanged += ListBox_Alarm_SelectedIndexChanged;

            if (Form1.Alarm_No == 0)
            {
                Alarm_Info_Text = "List-1(電子音)";
            }
            else if (Form1.Alarm_No == 1)
            {
                Alarm_Info_Text = "List-2(ベル音)";
            }
            else
            {
                Alarm_Info_Text = "List-3(学校のチャイム)";
            }
            textBox_Alarm_Info = new TextBox()
            {
                Text = Alarm_Info_Text,
                Location = new Point(130, 90),
                ReadOnly = true,
            };
            textBox_Alarm_Info.Size = new Size(150, 25);

            label_Alarm_HM = new Label()
            {
                Text = "アラーム時刻設定：",
                Location = new Point(10, 180),
            };
            label_Alarm_HM.Size = new Size(120, 25);
            textBox_Alarm_H = new TextBox()
            {
                Text = Form1.Alarm_H.ToString(),
                Location = new Point(130, 180),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
            };
            textBox_Alarm_H.Size = new Size(50, 25);
            trackbar_Alarm_H = new TrackBar()
            {
                Location = new Point(20, 200),
                Minimum = 0,
                Maximum = 23,
                Value = Form1.Alarm_H,
                TickFrequency = 3,
                SmallChange = 1,
                LargeChange = 6,
                AutoSize = false,
                Size = new Size(240, 40),
            };
            trackbar_Alarm_H.ValueChanged += Trackbar_Alarm_H_ValueChanged;

            textBox_Alarm_M = new TextBox()
            {
                Text = Form1.Alarm_M.ToString(),
                Location = new Point(200, 180),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
            };
            textBox_Alarm_M.Size = new Size(50, 25);
            trackbar_Alarm_M = new TrackBar()
            {
                Location = new Point(20, 250),
                Minimum = 0,
                Maximum = 59,
                Value = Form1.Alarm_M,
                TickFrequency = 10,
                SmallChange = 1,
                LargeChange = 10,
                AutoSize = false,
                Size = new Size(240, 40),
            };
            trackbar_Alarm_M.ValueChanged += Trackbar_Alarm_M_ValueChanged;

            Close_button = new Button()
            {
                Text = "Close",
                Location = new Point(100, 300),
            };
            Close_button.Click += new EventHandler(Close_button_CLicked);

            this.Controls.AddRange(new Control[]
            {
                label_T_Signal, label_Alarm, label_Alarm_S, label_Alarm_HM,
                listBox_Alarm,
                T_Signal_button, Alarm_button, Close_button,
                textBox_T_Signal, textBox_Alarm, textBox_Alarm_H, textBox_Alarm_M, textBox_Alarm_Info,
                trackbar_Alarm_H, trackbar_Alarm_M
            });
        }

        // 時報時報 ON / OFF ボタンが押された時の処理
        void T_Signal_button_Clicked(object sender, EventArgs e)
        {
            if (Form1.T_Signal_flag == 0)
            {
                Form1.T_Signal_flag = 1;
                T_Signal_Text = "時報ON";
            }
            else
            {
                Form1.T_Signal_flag = 0;
                T_Signal_Text = "時報OFF";
            }
            label_T_Signal.Text = T_Signal_Text;
        }

        // アラーム ON / OFF ボタンが押された時の処理
        void Alarm_button_Clicked(object sender, EventArgs e)
        {
            if (Form1.Alarm_flag == 0)
            {
                Form1.Alarm_flag = 1;
                Alarm_Text = "アラームON";
            }
            else
            {
                Form1.Alarm_flag = 0;
                Alarm_Text = "アラームOFF";
            }
            label_Alarm.Text = Alarm_Text;
        }

        // アラーム音色が変更された時の処理
        void ListBox_Alarm_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Temp_No = listBox_Alarm.SelectedIndex;
            if (Temp_No == 0)
            {
                Form1.player_Alarm = new System.Media.SoundPlayer(Properties.Resources.Clock_Alarm02_1_Loop_);
                Form1.Alarm_No = 0;
                Alarm_Info_Text = "List-1(電子音)";
            }
            if (Temp_No == 1)
            {
                Form1.player_Alarm = new System.Media.SoundPlayer(Properties.Resources.Clock_Alarm04_01);
                Form1.Alarm_No = 1;
                Alarm_Info_Text = "List-2(ベル音)";
            }
            if (Temp_No == 2)
            {
                Form1.player_Alarm = new System.Media.SoundPlayer(Properties.Resources.Japanese_School_Bell01_1);
                Form1.Alarm_No = 2;
                Alarm_Info_Text = "List-3(学校のチャイム)";
            }
            textBox_Alarm_Info.Text = Alarm_Info_Text;
        }

        // アラームの時が変更された時の処理
        void Trackbar_Alarm_H_ValueChanged(object s, EventArgs e)
        {
            textBox_Alarm_H.Text = trackbar_Alarm_H.Value.ToString();
            Form1.Alarm_H = int.Parse(textBox_Alarm_H.Text);
        }

        // アラームの分が変更された時の処理
        void Trackbar_Alarm_M_ValueChanged(object s, EventArgs e)
        {
            textBox_Alarm_M.Text = trackbar_Alarm_M.Value.ToString();
            Form1.Alarm_M = int.Parse(textBox_Alarm_M.Text);
        }

        // クローズボタンが押された時の処理
        void Close_button_CLicked(object sender, EventArgs e)
        {
            CSV_Save CSV_Write = new CSV_Save();
            CSV_Write.Save_CSV();
            this.Close();
        }
    }

    // 時計の位置とサイズ設定用ダイアログボックス
    public class Dialog_Pos_Size : Form
    {
        Label label_Size;
        Label label_Position;
        Label label_Pos_X;
        Label label_Pos_Y;
        public TextBox textBox_Size;
        public TextBox textBox_Pos_X;
        public TextBox textBox_Pos_Y;
        public TrackBar trackbar_Size;
        public TrackBar trackbar_Pos_X;
        public TrackBar trackbar_Pos_Y;
        Button Close_button;

        public Dialog_Pos_Size()
        {
            // ダイアログボックスの設定
            this.MaximizeBox = false;                                   // 最大化ボタン
            this.MinimizeBox = false;                                   // 最小化ボタン
            this.ShowInTaskbar = true;                                  // タスクバー上に表示
            this.FormBorderStyle = FormBorderStyle.FixedDialog;         // 境界のスタイル
            this.StartPosition = FormStartPosition.Manual;              // 任意の位置に表示
            this.Size = new Size(300, 400);                             // ダイアログボックスのサイズ指定
            this.Location = new Point(100, 100);                        // ダイアログボックスの表示位置
            this.Text = "時計の位置とサイズを設定";                     // タイトル

            label_Position = new Label()
            {
                Text = "時計の位置",
                Location = new Point(10, 10),
            };
            label_Position.Size = new Size(100, 25);
            label_Pos_X = new Label()
            {
                Text = "X = ",
                Location = new Point(50, 50),
            };
            label_Pos_X.Size = new Size(50, 25);
            textBox_Pos_X = new TextBox()
            {
                Text = Form1.Pos_X.ToString(),
                Location = new Point(120, 50),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
            };
            trackbar_Pos_X = new TrackBar()
            {
                Location = new Point(20, 80),
                Minimum = 0,
                Maximum = Form1.Scr_X - Form1.Size_Data,
                Value = Form1.Pos_X,
                TickFrequency = 100,
                SmallChange = 1,
                LargeChange = 50,
                AutoSize = false,
                Size = new Size(240, 40),
            };
            trackbar_Pos_X.ValueChanged += Trackbar_PosX_ValueChanged;

            label_Pos_Y = new Label()
            {
                Text = "Y = ",
                Location = new Point(50, 120),
            };
            label_Pos_Y.Size = new Size(50, 25);
            textBox_Pos_Y = new TextBox()
            {
                Text = Form1.Pos_Y.ToString(),
                Location = new Point(120, 120),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
            };
            trackbar_Pos_Y = new TrackBar()
            {
                Location = new Point(20, 150),
                Minimum = 0,
                Maximum = Form1.Scr_Y - Form1.Size_Data,
                Value = Form1.Pos_Y,
                TickFrequency = 100,
                SmallChange = 1,
                LargeChange = 50,
                AutoSize = false,
                Size = new Size(240, 40),
            };
            trackbar_Pos_Y.ValueChanged += Trackbar_PosY_ValueChanged;

            label_Size = new Label()
            {
                Text = "時計のサイズ：",
                Location = new Point(10, 200),
            };
            label_Size.Size = new Size(100, 25);
            textBox_Size = new TextBox()
            {
                Text = Form1.Size_Data.ToString(),
                Location = new Point(120, 200),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
            };
            trackbar_Size = new TrackBar()
            {
                Location = new Point(20, 230),
                Minimum = Form1.Size_Min,
                Maximum = Form1.Size_Max,
                Value = Form1.Size_Data,
                TickFrequency = 100,
                SmallChange = 1,
                LargeChange = 50,
                AutoSize = false,
                Size = new Size(240, 40),
            };
            trackbar_Size.ValueChanged += Trackbar_Size_ValueChanged;

            Close_button = new Button()
            {
                Text = "Close",
                Location = new Point(120, 300),
            };
            Close_button.Click += new EventHandler(Close_button_CLicked);

            this.Controls.AddRange(new Control[]
            {
                label_Size, label_Position, label_Pos_X, label_Pos_Y,
                Close_button,
                textBox_Size, textBox_Pos_X, textBox_Pos_Y,
                trackbar_Size, trackbar_Pos_X, trackbar_Pos_Y
            });
        }

        // クローズボタンが押された時の処理
        void Close_button_CLicked(object sender, EventArgs e)
        {
            CSV_Save CSV_Write = new CSV_Save();
            CSV_Write.Save_CSV();
            this.Close();
        }

        // 位置Xが変更された時の処理
        void Trackbar_PosX_ValueChanged(object s, EventArgs e)
        {
            textBox_Pos_X.Text = trackbar_Pos_X.Value.ToString();
            Form1.Pos_X = int.Parse(textBox_Pos_X.Text);
            Form1.Pos_flag = 1;
        }

        // 位置Yが変更された時の処理
        void Trackbar_PosY_ValueChanged(object s, EventArgs e)
        {
            textBox_Pos_Y.Text = trackbar_Pos_Y.Value.ToString();
            Form1.Pos_Y = int.Parse(textBox_Pos_Y.Text);
            Form1.Pos_flag = 1;
        }

        // 時計のサイズが変更された時の処理
        void Trackbar_Size_ValueChanged(object s, EventArgs e)
        {
            textBox_Size.Text = trackbar_Size.Value.ToString();
            Form1.Size_Data = int.Parse(textBox_Size.Text);
            Form1.Size_flag = 1;
        }
    }

    public class CSV_Read      // CSV Read
    {
        string[] Temp_Data = new string[10];
        public void Read_CSV()
        {
            StreamReader sr = new StreamReader(@"AnalogClock_Setting.csv", Encoding.GetEncoding("Shift_JIS"));
            try
            {
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        Temp_Data[i] = values[i];
                    }
                    if (Temp_Data[0] == "Face_No")
                    {
                        Form1.Face_No = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Face_flag")
                    {
                        Form1.Face_flag = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Hands_No")
                    {
                        Form1.Hands_No = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Hands_flag")
                    {
                        Form1.Hands_flag = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Size_Data")
                    {
                        Form1.Size_Data = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Pos_X")
                    {
                        Form1.Pos_X = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Pos_Y")
                    {
                        Form1.Pos_Y = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "T_Signal_flag")
                    {
                        Form1.T_Signal_flag = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Alarm_flag")
                    {
                        Form1.Alarm_flag = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Alarm_No")
                    {
                        Form1.Alarm_No = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Alarm_H")
                    {
                        Form1.Alarm_H = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Alarm_M")
                    {
                        Form1.Alarm_M = int.Parse(Temp_Data[1]);
                    }
                    if (Temp_Data[0] == "Opacity_Data")
                    {
                        if (Temp_Data[1] == null)
                        {
                            Form1.Opacity_Data = 0.0;
                        }
                        else
                        {
                            Form1.Opacity_Data = double.Parse(Temp_Data[1]);
                        }
                    }
                }
            }
            finally
            {
                sr.Close();
            }
        }
    }

    public class CSV_Save      // CSV Save
    {
        public void Save_CSV()
        {
            try
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter file = new StreamWriter(@"AnalogClock_Setting.csv", false, sjisEnc);
                file.WriteLine("Face_No" + "," + Form1.Face_No.ToString());
                file.WriteLine("Face_flag" + "," + "1");
                file.WriteLine("Hands_No" + "," + Form1.Hands_No.ToString());
                file.WriteLine("Hands_flag" + "," + "1");
                file.WriteLine("Size_Data" + "," + Form1.Size_Data.ToString());
                file.WriteLine("Pos_X" + "," + Form1.Pos_X.ToString());
                file.WriteLine("Pos_Y" + "," + Form1.Pos_Y.ToString());
                file.WriteLine("T_Signal_flag" + "," + Form1.T_Signal_flag.ToString());
                file.WriteLine("Alarm_flag" + "," + Form1.Alarm_flag.ToString());
                file.WriteLine("Alarm_No" + "," + Form1.Alarm_No.ToString());
                file.WriteLine("Alarm_H" + "," + Form1.Alarm_H.ToString());
                file.WriteLine("Alarm_M" + "," + Form1.Alarm_M.ToString());
                file.WriteLine("Opacity_Data" + "," + Form1.Opacity_Data.ToString("0.00"));

                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Save Error: " + e.Message);
            }
        }
    }
}
