using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Windows.Media.Animation;
using System.Threading;

using Emgu.CV;
using Emgu.CV.Structure;
using GdiGraphics = System.Drawing.Graphics;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.Structure;

namespace Rehab_MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    using Rehab;
    public partial class MainWindow : Window
    {
		Thread t = null;
		int resultTemp;
		int testTemp = 0;
		string strTemp;
		bool qq = true;
		int temp = 0;
		int tempIn = 0;
		double[] arrTempX = new double[5000];
		double[] arrTempY = new double[5000];
		IntPtr imgTemp = new IntPtr();
		static IntPtr hue;
		IntPtr[] hueArr = { hue };
		private IntPtr image;
		IntPtr hsv = new IntPtr();
		IntPtr mask = new IntPtr();
		IntPtr backproject = new IntPtr();
		IntPtr histimg = new IntPtr();
		IntPtr hist;
		IntPtr must1 = new IntPtr();
		IntPtr must2 = new IntPtr();
		IntPtr must3 = new IntPtr();
		int backproject_mode = 0;
		int select_object = 0;
		int track_object = 0;
		int show_hist = 1;
		Point origin;
		System.Drawing.Rectangle selection;
		System.Drawing.Rectangle track_window;
		MCvBox2D track_box;
		MCvConnectedComp track_comp;
		int[] hdims = new int[1] { 16 };
		float[] hranges_arr = { 0, 180 };
		IntPtr[] hranges = new IntPtr[2] { IntPtr.Zero, new IntPtr(180) };
		int vmin = 90;
		int vmax = 256;
		int smin = 90;

        public static user currentUser;
        public MainWindow()
        {
            InitializeComponent();
            double workHeight = SystemParameters.WorkArea.Height;
            double workWidth = SystemParameters.WorkArea.Width;
            this.Width = workWidth;//设置窗体宽度
            this.Height = workHeight;//设置窗体高度
            this.Top = 0;
            this.Left = 0;
            users.Add("普通用户");
            users.Add("医生");
            currentUser = new user();
            SetPlayer(false);
        }


        public List<string> users = new List<string>();
        public List<string> UserType
        {
            get
            {
                return users;
            }
        }

        private void SetPlayer(bool bVal)
        {
            sTrainButton.IsEnabled = !bVal;
            pTrainButton.IsEnabled = bVal;
        }
        private void PlayerPause(object sender, RoutedEventArgs e)
        {
            if (pTrainButton.Content.ToString() == "          暂停")
            {
                mediaElement.Pause();
                pTrainButton.Content = "       继续播放";
                mediaElement.ToolTip = "Click to Pause";
            }
            else
            {
                mediaElement.Play();
                sTrainButton.Content = "          暂停";
                mediaElement.ToolTip = "Click to Play";
            }
        }
		private void Change(object sender, RoutedEventArgs e)
		{
			qq = false;
		}
		void loadTemplateImage()
		{

			IntPtr tempimage = CvInvoke.cvLoadImage(System.Environment.CurrentDirectory + "/002.jpg", Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_COLOR);//!!!!
			CvInvoke.cvCvtColor(tempimage, hsv, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_BGR2HSV);
			int _vmin = vmin;
			int _vmax = vmax;
			CvInvoke.cvInRangeS(hsv, new MCvScalar(0, smin, Min(_vmin, _vmax), 0),
					new MCvScalar(180, 256, Max(_vmin, _vmax), 0), mask);
			CvInvoke.cvSplit(hsv, hue, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			selection.X = 1;
			selection.Y = 1;
			selection.Width = 640 - 1;
			selection.Height = 480 - 1;
			CvInvoke.cvSetImageROI(hue, selection);
			CvInvoke.cvSetImageROI(mask, selection);
			CvInvoke.cvCalcHist(new IntPtr[1] { hue }, hist, false, mask);

			float max_val = 0.0f;
			int[] a1 = new int[0];
			int[] b1 = new int[0];
			float ax = 0;
			float scale = 0;
			if (max_val != 0)
			{
				scale = 255 / max_val;
			}
			CvInvoke.cvGetMinMaxHistValue(hist, ref ax, ref max_val, a1, b1);


			//CvInvoke.cvConvertScale(hist.MCvHistogram.bins,hist.MCvHistogram.bins,scale, 0 );
			CvInvoke.cvResetImageROI(hue);
			CvInvoke.cvResetImageROI(mask);

			track_window = selection;
			track_object = 1;
			CvInvoke.cvReleaseImage(ref tempimage);
		}
		public int Max(int num1, int num2) { return num1 > num2 ? num1 : num2; }
		public int Min(int num1, int num2) { return num1 < num2 ? num1 : num2; } 
		private void Run()
		{
			int c = 0;
			IntPtr capture = CvInvoke.cvCreateCameraCapture(-1);
			if (capture == null)
			{
				System.Console.WriteLine("Could not initialize capturing...\n");

			}
			//CvInvoke.cvNamedWindow("CamShiftDemo");


			while (qq)
			{

				mediaElement.Unloaded += Change;
				//int i, bin_w, c;
				IntPtr frame = CvInvoke.cvQueryFrame(capture);
				// IntPtr mode = CvInvoke.cvLoadImage("E:/02.jpg", Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_COLOR);
				if (frame.Equals(IntPtr.Zero))
					break;
				if (image.Equals(IntPtr.Zero))
				{
					image = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(frame), IPL_DEPTH.IPL_DEPTH_8U, 3);
					//image = frame.GetType ;

					hsv = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(frame), IPL_DEPTH.IPL_DEPTH_8U, 3);
					hue = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(frame), IPL_DEPTH.IPL_DEPTH_8U, 1);
					mask = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(frame), IPL_DEPTH.IPL_DEPTH_8U, 1);
					backproject = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(frame), IPL_DEPTH.IPL_DEPTH_8U, 1);
					hist = CvInvoke.cvCreateHist(1, hdims, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1);
					histimg = CvInvoke.cvCreateImage(new System.Drawing.Size(320, 200), IPL_DEPTH.IPL_DEPTH_8U, 3);
					CvInvoke.cvZero(histimg);
					loadTemplateImage();
				}
				CvInvoke.cvCopy(frame, image, IntPtr.Zero);
				CvInvoke.cvCvtColor(image, hsv, COLOR_CONVERSION.CV_BGR2HSV);
				if (track_object != 0)
				{
					int _vmin = vmin;
					int _vmax = vmax;
					CvInvoke.cvInRangeS(hsv, new Emgu.CV.Structure.MCvScalar(0, smin, Min(_vmin, _vmax), 0), new Emgu.CV.Structure.MCvScalar(180, 256, Max(_vmin, _vmax), 0), mask);
					CvInvoke.cvSplit(hsv, hue, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
					CvInvoke.cvCalcBackProject(new IntPtr[1] { hue }, backproject, hist);
					CvInvoke.cvAnd(backproject, mask, backproject, IntPtr.Zero);
					CvInvoke.cvCamShift(backproject, track_window, new MCvTermCriteria(10, 1), out track_comp, out track_box);
					track_window = track_comp.rect;
					/*
					 if (backproject_mode!=0)
						 CvInvoke.cvCvtColor(backproject, image, COLOR_CONVERSION.CV_GRAY2BGR);
					 if (false)
						 track_box.angle = -track_box.angle;
					 */
					//CvInvoke.cvEllipseBox(image,track_box,)

				}
				//CvInvoke.cvShowImage("CamShiftDemo", image);
				//System.Console.WriteLine("(x,y)--->>(" + track_box.center.X + "," + track_box.center.Y + ")");

				if (temp % 10 == 0)
				{
					//std::cout<<"(x,y)--->>("<<track_box.center.x<<","<<track_box.center.y<<")"<<"----"<<image->origin<<endl ;
					arrTempX[tempIn] = track_box.center.X;
					arrTempY[tempIn] = track_box.center.Y;
					//cout << "(x,y)--->>(" << arrTempX[tempIn] << "," << arrTempY[tempIn] << ")" << "----" << tempIn << endl;
					tempIn++;
				}
				temp++;
				if (temp == 300)
				{
					qq = false;
				}
				if (temp == 700)
				{
					qq = false;
				}
				//MyPlayer.Unloaded += Change;

				c = CvInvoke.cvWaitKey(10);
				
			}

			double maxX = arrTempX[0];
			double maxY = arrTempY[0];
			double minX = arrTempX[0];
			double minY = arrTempY[0];

			for (int comTemp = 0; comTemp < tempIn - 1; comTemp++)
			{
				if (maxX <= arrTempX[comTemp + 1])
				{
					maxX = arrTempX[comTemp + 1];
				}
				if (maxY <= arrTempY[comTemp + 1])
				{
					maxY = arrTempY[comTemp + 1];
				}
				if (minX >= arrTempX[comTemp + 1])
				{
					minX = arrTempX[comTemp + 1];
				}
				if (minY >= arrTempY[comTemp + 1])
				{
					minY = arrTempY[comTemp + 1];
				}
			}
			//cout << maxX << "," << maxY << "    " << minX << "," << minY << endl;
			double result = maxX - minX + maxY - minY;

			if (result < 300 || result > 700)
			{
				strTemp = "10%";
				testTemp = 1;
			}
			else
			{
				//cout << 100 - abs((result - 500) / 2) << "%" << endl;

				resultTemp = (int)(100 - System.Math.Abs((result - 500) / 2));
				//TrainingPrecent.Text = (resultTemp).ToString() + "%";
			}

			if (testTemp == 0)
			{
				this.Dispatcher.Invoke(new Action(() => { TrainingPrecent.Text = (resultTemp).ToString() + "%"; }));
			}
			else
			{
				this.Dispatcher.Invoke(new Action(() => { TrainingPrecent.Text = ("10%").ToString() + "%"; }));
			}
			if (t.IsAlive)
			{
				resultTemp = 0;
				qq = true;
				//this.Dispatcher.Invoke(new Action(() => { TrainingPrecent.Text = ("").ToString(); }));
				t.Abort();
			}
			CvInvoke.cvReleaseCapture(ref capture);


		}
        private void PlayerPlay(object sender, RoutedEventArgs e)
        {
			mediaElement.Source = new Uri(System.Environment.CurrentDirectory + "/path.MOV", UriKind.Relative);
			SetPlayer(true);
			mediaElement.Play();

			TrainingPrecent.Text = ("").ToString();

			t = new Thread(new ThreadStart(Run));
			t.Start();
			       
/*
            int [] aa = new int[2];
            aa[0] = standard ;
            aa[1] = standard ;
            testtrain ts = new testtrain();
            ts.insert("testtrain1", aa, currentUser.uID);
 */
        }

        private void LoadTrainReButton(object sender, RoutedEventArgs e)
        {
            if (currentUser.uState == 1)
            {
                if (currentUser.uIdentity == 0)
                {
                    TrainReUrname.Text = currentUser.uRname;
                    List<testtrain> myTestTrain = new List<testtrain>();
                    int Thedays = 0;
                    testtrain Newtrain = new testtrain();
                    myTestTrain = Newtrain.QueryNew("testtrain1", currentUser.uID, out Thedays);
                    Thedays += 1;
                    TrainDays.Text = "第" + Thedays.ToString() + "天";
                    if (myTestTrain!=null)
                    {
                        foreach (testtrain temp in myTestTrain)
                        {
                            Newtrain = temp;
                        }
                        TrainactionOneNum.Text = Newtrain.point1.ToString() + "%";
                        TrainactionTwoNum.Text = Newtrain.point2.ToString() + "%";
                        TrainactionThreeNum.Text = Newtrain.point3.ToString() + "%";
                        double TarinOne = (Newtrain.point1 / 100.0) * 340;
                        double TarinTwo = (Newtrain.point2 / 100.0) * 340;
                        double TarinThree = (Newtrain.point3 / 100.0) * 340;
                        DoubleAnimation TrainactionOne = new DoubleAnimation();
                        TrainactionOne.From = 0;
                        TrainactionOne.To = TarinOne;
                        TrainactionOne.Duration = TimeSpan.FromSeconds(0.5);
                        DoubleAnimation TrainactionTwo = new DoubleAnimation();
                        TrainactionTwo.From = 0;
                        TrainactionTwo.To = TarinTwo;
                        TrainactionTwo.Duration = TimeSpan.FromSeconds(0.5);
                        DoubleAnimation TrainactionThree = new DoubleAnimation();
                        TrainactionThree.From = 0;
                        TrainactionThree.To = TarinThree;
                        TrainactionThree.Duration = TimeSpan.FromSeconds(0.5);
                        double TarinTotal = 146 - (Newtrain.point / 100.0) * 146;
                        if (Newtrain.point < 10)
                        {
                            TotalFinished.Text = "0" + Newtrain.point.ToString() + "%";
                        }
                        else
                        {
                            TotalFinished.Text = Newtrain.point.ToString() + "%";
                        }
                        DoubleAnimation TheTrainactionTotal = new DoubleAnimation();
                        TheTrainactionTotal.From = 146;
                        TheTrainactionTotal.To = TarinTotal;
                        TheTrainactionTotal.Duration = TimeSpan.FromSeconds(0.5);
                        List<testtrain> RecentTrain = new List<testtrain>();
                        int RecentDays = 10;
                        testtrain theRecenTemp = new testtrain();
                        RecentTrain = theRecenTemp.QueryDays("testtrain1", currentUser.uID, RecentDays);
                        testtrain[] theRecents = new testtrain[RecentDays];
                        int count = 0;
                        foreach (testtrain temp in RecentTrain)
                        {
                            theRecents[count] = temp;
                            count++;
                            if (count > 9)
                            {
                                break;
                            }
                        }
                        System.Windows.Media.PointCollection Mypoints = new System.Windows.Media.PointCollection();
                        Mypoints.Add(new Point(5, 298));
                        Double[] thePointsTemp = new Double[10] { 44, 84, 123, 163, 202, 242, 281, 321, 360, 400 };
                        for (int i = 0; i < 10; ++i)
                        {
                            if (theRecents[i] != null)
                            {
                                Double temp = 298 - (theRecents[i].point) / 100.0 * 238;
                                Mypoints.Add(new Point(thePointsTemp[i], temp));
                            }
                        }
                        myMove.Points = Mypoints;
                        TrainactionOnePic.BeginAnimation(WidthProperty, TrainactionOne);
                        TrainactionTwoPic.BeginAnimation(WidthProperty, TrainactionTwo);
                        TrainactionThreePic.BeginAnimation(WidthProperty, TrainactionThree);
                        TrainactionTotal.BeginAnimation(HeightProperty, TheTrainactionTotal);
                    }
                    Storyboard story = (Storyboard)this.FindResource("TrainReButtonClick");
                    story.Begin();

                }
                else
                {
                    GetPlistButton(sender, e);
                }
            }
            else
            {
                LogEmailEnsure.Text = "请先登录";
                LogEmailEnsure.Visibility = Visibility.Visible;
                Storyboard story = (Storyboard)this.FindResource("UserButtonClick");
                story.Begin();
            }
        }
        private void LogInButton(object sender, RoutedEventArgs e)
        {    
            
            string Uid = UserLogId.Text;
            string Pas = UserPassWord.Password;
            if (Uid != "" && Pas != "")
            {
                user ss = new user();
                List<user> aa = new List<user>();
                int re;
                //re is uid of table user;
                bool b = ss.isRight(Uid, Pas, out re);
                if (b)
                {
                    //QueryAll can use uid to Query;
                    aa = ss.QueryAll(re);
                    foreach (user tempUser in aa)
                    {
                        UserInfoRName.Text = tempUser.uRname;
                        UserInfoName.Text = tempUser.uName;
                        UserInfoEmail.Text = tempUser.uEmail;
                        UserInfoBdate.Text = tempUser.uBirth.ToString("yyyy年MM月dd日");
                        UserInfoAddress.Text = tempUser.uAddress;
                        currentUser = tempUser;
                        currentUser.uState = 1;
                    }
                    Storyboard story = (Storyboard)this.FindResource("LogInButtonClick");
                    story.Begin();
                }
                else
                {
                    LogEmailEnsure.Text = "邮箱密码不匹配";
                    LogEmailEnsure.Visibility = Visibility.Visible;
                }
            }
            else
            {
                    LogEmailEnsure.Text = "邮箱密码不能为空";
                    LogEmailEnsure.Visibility = Visibility.Visible;
            }
        }

        private void LogOutButton(object sender, RoutedEventArgs e)
        {
            if (currentUser.uState == 1)
            {
                currentUser.uState = 0;
                Storyboard story = (Storyboard)this.FindResource("LogOutButtonClick");
                story.Begin();
                Storyboard MYstory = (Storyboard)this.FindResource("GetBackSt");
                story.Begin();
            }
        }

        private void EnterLogInButton(object sender, RoutedEventArgs e)
        {
            if (RegisterBorder.Width == 318)
            {
                RegisterBorderCloseSt.Begin();
            }
            if (currentUser.uState != 1)
            {
                Storyboard story = (Storyboard)this.FindResource("UserButtonClick");
                story.Begin();
            }
            else
            {
                UserInfoRName.Text = currentUser.uRname;
                UserInfoName.Text = currentUser.uName;
                UserInfoEmail.Text = currentUser.uEmail;
                UserInfoBdate.Text = currentUser.uBirth.ToString("yyyy年MM月dd日");
                UserInfoAddress.Text = currentUser.uAddress;
                Storyboard story = (Storyboard)this.FindResource("LogInTureButtonClick");
                story.Begin();
            }
        }

        private void EnterAlterPassButton(object sender, RoutedEventArgs e)
        {
            if (currentUser.uState == 1)
            {
                AlterPassUname.Text = currentUser.uName;
                AlterPassEmail.Text = currentUser.uEmail;
            }
        }

        private void AlterInfoButton(object sender, RoutedEventArgs e)
        {
            user MyUser = new user();
            bool succeed = true;
            bool real = true;
            if (AlterInfoY.Text != "" || AlterInfoM.Text != "" || AlterInfoD.Text != "")
            {
                if (AlterInfoY.Text != "" && AlterInfoM.Text != "" && AlterInfoD.Text != "")
                {
                    succeed = MyUser.upDate("uBirth", AlterInfoY.Text+"-" + AlterInfoM.Text+"-" + AlterInfoD.Text, currentUser.uID);
                    if (!succeed)
                    {
                        AlterInfoEnsureofDate.Text = "日期格式不正确";
                        real = false;
                    }
                }
                else
                {

                }
            }
            if (AlterInfoUrname.Text != "")
            {
                succeed = MyUser.upDate("uRname", AlterInfoUrname.Text, currentUser.uID);
            }
            if (AlterInfoSm.IsChecked == true || AlterInfoSw.IsChecked == true)
            {
                if (AlterInfoSm.IsChecked == true)
                {
                    succeed = MyUser.upDate("uSex", 1, currentUser.uID);
                }
                else
                {
                    succeed = MyUser.upDate("uSex", 0, currentUser.uID);
                }
            }
            if (AlterInfoAddress.Text != "")
            {
                succeed = MyUser.upDate("uAddress", AlterInfoAddress.Text, currentUser.uID);
            }
            if (real)
            {
                user ss = new user();
                List<user> aa = new List<user>();
                int re;
                //re is uid of table user;
                bool b = ss.isRight(currentUser.uEmail, currentUser.uPassword, out re);
                if (b)
                {
                    //QueryAll can use uid to Query;
                    aa = ss.QueryAll(re);
                    foreach (user tempUser in aa)
                    {
                        UserInfoRName.Text = tempUser.uRname;
                        UserInfoName.Text = tempUser.uName;
                        UserInfoEmail.Text = tempUser.uEmail;
                        UserInfoBdate.Text = tempUser.uBirth.ToString("yyyy年MM月dd日");
                        UserInfoAddress.Text = tempUser.uAddress;
                        currentUser = tempUser;
                        currentUser.uState = 1;
                    }
                    Storyboard story = (Storyboard)this.FindResource("SaveAlterInfoButtonClick");
                    story.Begin();
                }
            }
        }

        private void AlterPassButton(object sender, RoutedEventArgs e)
        {
            if (OldAlterPassEnsure.Text == "" && NewAlterPassEnsure.Text == "" && EnsureAlterPassEnsure.Text == "")
            {
                user MyUser = new user();
                bool succeed = MyUser.upDate("uPassword", NewAlterPass.Password , currentUser.uID );
                if (succeed)
                {
                    Storyboard story = (Storyboard)this.FindResource("AlterButtonClick");
                    story.Begin();
                }
            }
        }

        private void RegisterButton(object sender, RoutedEventArgs e)
        {
            if (RegisterInfoNameEnsure.Text == "" && RegisterInfoEmailEnsure.Text == "" && RegisterInfoPasswordEnsure.Text == "" && EnsurePassEnsure.Text == "" && RegisterUserType.Text != "")
            {
                string Uname = RegisterInfoName.Text;
                string UEmail = RegisterInfoEmail.Text;
                string Upass = RegisterInfoPassword.Password;
                string Utype;
                if (RegisterUserType.Text == "普通用户")
                {
                    Utype = "0";
                }
                else
                {
                    Utype = "1";
                }
                string[] UInfo = new string[4] { Uname, UEmail, Upass, Utype };
                user us = new user();
                bool succeed = us.regist(UInfo);
                if (succeed)
                {
                    user ss = new user();
                    List<user> aa = new List<user>();
                    int re;
                    //re is uid of table user;
                    bool b = ss.isRight(UEmail, Upass, out re);
                    if (b)
                    {
                        //QueryAll can use uid to Query;
                        aa = ss.QueryAll(re);
                        foreach (user tempUser in aa)
                        {
                            UserInfoRName.Text = tempUser.uRname;
                            UserInfoName.Text = tempUser.uName;
                            UserInfoEmail.Text = tempUser.uEmail;
                            UserInfoBdate.Text = tempUser.uBirth.ToString("yyyy年MM月dd日");
                            UserInfoAddress.Text = tempUser.uAddress;
                            currentUser = tempUser;
                            currentUser.uState = 1;
                        }
                        Storyboard story = (Storyboard)this.FindResource("RegisterButtonClick");
                        story.Begin();
                    }

                }
            }
            else
            {
                if (RegisterUserType.Text == "")
                {
                    UserTypeEnsure.Text = "请选择用户类型";
                    UserTypeEnsure.Visibility = Visibility.Visible;
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    RegisterInfoNamePic.Source = imagetemp;
                    RegisterInfoNameEnsure.Text = "请填写完整信息";
                    RegisterInfoNamePic.Visibility = Visibility.Visible;
                    RegisterInfoNameEnsure.Visibility = Visibility.Visible;
                }
            }
        }

        private void CheckInfoName(object sender, RoutedEventArgs e)
        {
            
            string Uname = RegisterInfoName.Text;
            if (Uname != "")
            {
                user us = new user();
                bool Succeed = us.checkUnique("uName", Uname);
                if (Succeed)
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                    RegisterInfoNamePic.Source = imagetemp;
                    RegisterInfoNameEnsure.Text = "";
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    RegisterInfoNamePic.Source = imagetemp;
                    RegisterInfoNameEnsure.Text = "用户名已存在";
                }
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                RegisterInfoNamePic.Source = imagetemp;
                RegisterInfoNameEnsure.Text = "用户名不能为空";
            }
            RegisterInfoNamePic.Visibility = Visibility.Visible;
            RegisterInfoNameEnsure.Visibility = Visibility.Visible;
        }

        private void CheckInfoEmail(object sender, RoutedEventArgs e)
        {
            string UEmail = RegisterInfoEmail.Text;
            if (UEmail != "")
            {
                user us = new user();
                if (us.verifyEmail(UEmail))
                {
                    if (us.checkUnique("uEmail", UEmail))
                    {
                        BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                        RegisterInfoEmailPic.Source = imagetemp;
                        RegisterInfoEmailEnsure.Text = "";
                    }
                    else
                    {
                        BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                        RegisterInfoEmailPic.Source = imagetemp;
                        RegisterInfoEmailEnsure.Text = "邮箱已注册";
                    }
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    RegisterInfoEmailPic.Source = imagetemp;
                    RegisterInfoEmailEnsure.Text = "邮箱格式不正确";
                }
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                RegisterInfoEmailPic.Source = imagetemp;
                RegisterInfoEmailEnsure.Text = "邮箱不能为空";
            }
            RegisterInfoEmailPic.Visibility = Visibility.Visible;
            RegisterInfoEmailEnsure.Visibility = Visibility.Visible;
        }

 /*       private void CheckInfoEmail(object sender, RoutedEventArgs e)
        {
            string UEmail = RegisterInfoEmail.Text;
            if (UEmail != "")
            {
                user us = new user();
                if (us.verifyEmail(UEmail))
                {
                    RegisterInfoEmailEnsure.Text = " ";
                    //lock (RegisterInfoEmail.Text)
                    //{
                        Func<bool> asyncAction = this.checkUniqueE;
                        Action<IAsyncResult> resultHandler = delegate(IAsyncResult asyncResult)
                        {
                            bool theRe = asyncAction.EndInvoke(asyncResult);
                            this.checkUniqueI(theRe);
                        };
                        AsyncCallback asyncActionCallback = delegate(IAsyncResult asyncResult)
                        {
                            this.Dispatcher.BeginInvoke(resultHandler, asyncResult);
                        };
                        asyncAction.BeginInvoke(asyncActionCallback, null);
                    //}
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    RegisterInfoEmailPic.Source = imagetemp;
                    RegisterInfoEmailEnsure.Text = "邮箱格式不正确";
                }
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                RegisterInfoEmailPic.Source = imagetemp;
                RegisterInfoEmailEnsure.Text = "邮箱不能为空";
            }
            RegisterInfoEmailPic.Visibility = Visibility.Visible;
            RegisterInfoEmailEnsure.Visibility = Visibility.Visible;
        }
        public bool checkUniqueE()
        {
            //Thread.Sleep(4000);
            string UEmail = RegisterInfoEmail.Text;
            user us = new user();
            bool my = us.checkUnique("uEmail", UEmail);
            return my;
        }
        public void checkUniqueI(bool re)
        {
            if (re)
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                RegisterInfoEmailPic.Source = imagetemp;
                RegisterInfoEmailEnsure.Text = "";
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                RegisterInfoEmailPic.Source = imagetemp;
                RegisterInfoEmailEnsure.Text = "邮箱已注册";
            }
        }*/

        private void CheckInfoPass(object sender, RoutedEventArgs e)
        {
            string Upass = RegisterInfoPassword.Password;
            if (Upass != "")
            {
                if(Upass.Length >= 6)
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                    RegisterInfoPasswordPic.Source = imagetemp;
                    RegisterInfoPasswordEnsure.Text = "";
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    RegisterInfoPasswordPic.Source = imagetemp;
                    RegisterInfoPasswordEnsure.Text = "密码长度需大于6";
                }
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                RegisterInfoPasswordPic.Source = imagetemp;
                RegisterInfoPasswordEnsure.Text = "密码不能为空";
            }
            RegisterInfoPasswordPic.Visibility = Visibility.Visible;
            RegisterInfoPasswordEnsure.Visibility = Visibility.Visible;
        }

        private void CheckInfoRePass(object sender, RoutedEventArgs e)
        {
            string Upass = RegisterInfoPassword.Password;
            string URepass = EnsurePass.Password;
            if (Upass == URepass)
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                    EnsurePassPic.Source = imagetemp;
                    EnsurePassEnsure.Text = "";
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    EnsurePassPic.Source = imagetemp;
                    EnsurePassEnsure.Text = "输入密码不同";
                }
            EnsurePassPic.Visibility = Visibility.Visible;
            EnsurePassEnsure.Visibility = Visibility.Visible;
        }

        private void CheckAlterPass(object sender, RoutedEventArgs e)
        {
            string Upass = NewAlterPass.Password;
            if (Upass != "")
            {
                if (Upass.Length >= 6)
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                    NewAlterPassPic.Source = imagetemp;
                    NewAlterPassEnsure.Text = "";
                }
                else
                {
                    BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                    NewAlterPassPic.Source = imagetemp;
                    NewAlterPassEnsure.Text = "密码长度需大于6";
                }
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                NewAlterPassPic.Source = imagetemp;
                NewAlterPassEnsure.Text = "密码不能为空";
            }
            NewAlterPassPic.Visibility = Visibility.Visible;
            NewAlterPassEnsure.Visibility = Visibility.Visible;
        }

        private void CheckAlterRePass(object sender, RoutedEventArgs e)
        {
            string Upass = NewAlterPass.Password;
            string URepass = EnsureAlterPass.Password;
            if (Upass == URepass)
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                EnsureAlterPassPic.Source = imagetemp;
                EnsureAlterPassEnsure.Text = "";
            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                EnsureAlterPassPic.Source = imagetemp;
                EnsureAlterPassEnsure.Text = "输入密码不同";
            }
            EnsureAlterPassPic.Visibility = Visibility.Visible;
            EnsureAlterPassEnsure.Visibility = Visibility.Visible;
        }

        private void CheckAlterOldPass(object sender, RoutedEventArgs e)
        {
            string OldPass = OldAlterPass.Password;
            if (OldPass == currentUser.uPassword)
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/true.png", UriKind.Relative));
                OldAlterPassPic.Source = imagetemp;
                OldAlterPassEnsure.Text = "";

            }
            else
            {
                BitmapImage imagetemp = new BitmapImage(new Uri("pic/false.png", UriKind.Relative));
                OldAlterPassPic.Source = imagetemp;
                OldAlterPassEnsure.Text = "输入密码错误";
            }
            OldAlterPassPic.Visibility = Visibility.Visible;
            OldAlterPassEnsure.Visibility = Visibility.Visible;
        }

        private void LostFoucsedMove(object sender, RoutedEventArgs e)
        {
            bool IsMenueOpen = false;
            if (LogBorder.Width == 318)
            {
                LogBorderCloseSt.Begin();
                IsMenueOpen = true;
            }
            if (RegisterBorder.Width == 318)
            {
                RegisterBorderCloseSt.Begin();
                IsMenueOpen = true;
            }
            if (PersonInfoBorder.Width == 318)
            {
                PersonInfoBorderCloseSt.Begin();
                IsMenueOpen = true;
            }
            if (FixCodeBorder.Width == 318)
            {
                FixCodeBorderCloseSt.Begin();
                IsMenueOpen = true;
            }
            if (IsMenueOpen && MenueBorder.Width == 240)
            {
                MenueBorderCloseSt.Begin();
            }
        }

        private void UserCommunityButton(object sender, RoutedEventArgs e)
        {
            if (currentUser.uState != 1)
            {
                LogEmailEnsure.Text = "请先登录";
                LogEmailEnsure.Visibility = Visibility.Visible;
                Storyboard story = (Storyboard)this.FindResource("UserButtonClick");
                story.Begin();
            }
            else if (currentUser.uIdentity == 1)
            {
                if (currentUser.all)
                {
                    Pselectmenue.Visibility = Visibility.Hidden;
                    Dselectmenue.Visibility = Visibility.Visible;
                    DocPinfoButton(sender, e);
                }
                else
                {
                    ModifyDocPinfoButton(sender, e);
                }
            }
            else
            {
                if (currentUser.all)
                {
                    Pselectmenue.Visibility = Visibility.Visible;
                    Dselectmenue.Visibility = Visibility.Hidden;
                    FindMyDoc(sender, e);
                }
                else
                {
                    ModifyPinfoButton(sender, e);
                }
            }
        }

        private void PcasePageButton(object sender, RoutedEventArgs e)
        {
            bool re = false;
            if (currentUser.all)
            {
                if (PcasePagePwork.Text != "")
                {
                    re = patient.Update("pCareer", PcasePagePwork.Text, currentUser.uID);
                }
                if (PcasePagePphone.Text != "")
                {
                    re = patient.Update("pPhone", PcasePagePphone.Text, currentUser.uID);
                }
                if (PcasePagePemail.Text != "")
                {
                    re = patient.Update("pEmail", PcasePagePemail.Text, currentUser.uID);
                }
                if (PcasePagePworkp.Text != "")
                {
                    re = patient.Update("pWorkUnit", PcasePagePworkp.Text, currentUser.uID);
                }
                if (PcasePagePconp.Text != "")
                {
                    re = patient.Update("pConAddress", PcasePagePconp.Text, currentUser.uID);
                }
                if (PcasePagePsurgery.Text != "")
                {
                    re = patient.Update("pOpHistory", PcasePagePsurgery.Text, currentUser.uID);
                }
                if (PcasePagePallergy.Text != "")
                {
                    re = patient.Update("pAllergy", PcasePagePallergy.Text, currentUser.uID);
                }
                if (PcasePagePhurt.Text != "")
                {
                    re = patient.Update("pInjuredPart", PcasePagePhurt.Text, currentUser.uID);
                }
                if (PcasePagePhosp.Text != "")
                {
                    re = patient.Update("pHospital", PcasePagePhosp.Text, currentUser.uID);
                }
                if (PcasePagePre.Text != "")
                {
                    re = patient.Update("pDiagnosis", PcasePagePre.Text, currentUser.uID);
                }
                if (PcasePagePcheck.Text != "")
                {
                    re = patient.updateImage(PcasePagePcheck.Text, currentUser.uID);
                }
            }
            //参数为string数组，依次为puid,psex,page,pcareer,pphone,pemail,pworkunit,
            //pconaddress,pophistory,pallergy,pinjuredpart,phospital,pdiagnosis,pinspectlist,pidcard,prname
            //返回bool值     测试接口...   有错速报
            else
            {
                if (PcasePagePphone.Text != "" && PcasePagePemail.Text != "" && PcasePagePhurt.Text != "" & PcasePagePhosp.Text != "" && PcasePagePre.Text != "" && PcasePagePcheck.Text!="")
                {
                    string Pid = currentUser.uID.ToString();
                    string Psex = currentUser.uSex.ToString();
                    string Page = PcasePagePage.Text;
                    string Pcareer="";
                    if (PcasePagePwork.Text != "")
                    {
                        Pcareer = PcasePagePwork.Text;
                    }
                    string Pphone = PcasePagePphone.Text;
                    string Pemail = PcasePagePemail.Text;
                    string Pwork="";
                    if (PcasePagePworkp.Text != "")
                    {
                        Pwork = PcasePagePworkp.Text;
                    }
                    string Pconad="";
                    if (PcasePagePconp.Text != "")
                    {
                        Pconad = PcasePagePconp.Text;
                    }
                    string Psurgery="";
                    if (PcasePagePsurgery.Text != "")
                    {
                       Psurgery = PcasePagePsurgery.Text;
                    }
                    string Pallergy="";
                    if (PcasePagePallergy.Text != "")
                    {
                        Pallergy = PcasePagePallergy.Text;
                    }
                    string Phurt = PcasePagePhurt.Text;
                    string Phosp = PcasePagePhosp.Text;
                    string Pdre = PcasePagePre.Text;

                    string Prname = currentUser.uRname;

                    string[] Pinfo = new string[] { Pid, Psex, Page, Pcareer, Pphone, Pemail, Pwork, Pconad, Psurgery, Pallergy, Phurt, Phosp, Pdre, Prname };
                    re = patient.insert(Pinfo);
                    re = patient.insertImage(PcasePagePcheck.Text, currentUser.uID);
                }
                else
                {
                    PcaseSaveEnsure.Text = "必要信息未填完";
                }
            }
            if (re)
            {
                QueryPatient(sender, e);
            }
            else
            {
                PcaseSaveEnsure.Text = "填写信息有误";
            }
        }

        private void DinfoPageButton(object sender, RoutedEventArgs e)
        {
            bool succeed = false;
            doctor myDoc = new doctor();
            myDoc.duid = currentUser.uID;
            if (currentUser.all)
            {               
                if (DinfoPageIDcard.Text != "")
                {
                    succeed = myDoc.Update("dIDcard", DinfoPageIDcard.Text);
                }
                if (DinfoPageWorkP.Text != "")
                {
                    succeed = myDoc.Update("dworkunit", DinfoPageWorkP.Text);
                }
                if (DinfoPageDepart.Text != "")
                {
                    succeed = myDoc.Update("dworkpart", DinfoPageDepart.Text);
                }
                if (DinfoPageTitle.Text != "")
                {
                    succeed = myDoc.Update("dlevel", DinfoPageTitle.Text);
                }
                if(DinfoPagePhone.Text != "")
                {
                    succeed = myDoc.Update("dphone", DinfoPagePhone.Text);
                }
                if (DinfoPagePintro.Text != "")
                {
                    succeed = myDoc.Update("dresume", DinfoPagePintro.Text);
                }
                if (DinfoPageDcard.Text != ""&&PicSourceOFd.Text!="")
                {
                    succeed = doctor.updateImage(PicSourceOFd.Text, DinfoPageDcard.Text, currentUser.uID);
                }
            }
            else
            {
                if (DinfoPageIDcard.Text != "" && PicSourceOFd.Text != "" && DinfoPageWorkP.Text != "" && DinfoPageDepart.Text != "" && DinfoPageTitle.Text != "" && DinfoPagePhone.Text != "" && DinfoPageDcard.Text !="")
                {
                    string Did = currentUser.uID.ToString();
                    string Drname = currentUser.uRname;
                    string Dsex = currentUser.uSex.ToString();
                    int age = DateTime.Now.Year - currentUser.uBirth.Year;
                    string Dage = age.ToString();
                    string DidCard = DinfoPageIDcard.Text;
                    string Dworkp = DinfoPageWorkP.Text;
                    string Dworkdpart = DinfoPageDepart.Text;
                    string Dtitle = DinfoPageTitle.Text;
                    string Dphone = DinfoPagePhone.Text;

                    string Dpintro = DinfoPagePintro.Text;

                    string[] Dinfo = new string[] { Did, Drname, Dsex, Dage, DidCard, Dworkp, Dworkdpart, Dtitle, Dphone, Dpintro, PicSourceOFd.Text, DinfoPageDcard.Text };

                    succeed = doctor.insert(Dinfo);
                }
            }
            if (succeed)
            {
                DocPinfoButton(sender, e);
            }
            else
            {
                DinfoPageSaveEnsur.Text = "填写信息不完整或有错";
            }
            
        }

        private void QueryPatient(object sender, RoutedEventArgs e)
        {
            List<patient> PatLi = new List<patient>();
            PatLi = patient.QueryAll(currentUser.uID);
            foreach (patient temp in PatLi)
            {
                ViewPatLrname.Text = currentUser.uRname;
                if (temp.pSex == 0)
                {
                    ViewPatLsex.Text = "女";
                }
                else
                {
                    ViewPatLsex.Text = "男";
                }
                ViewPatLage.Text = temp.pAge.ToString();
                ViewPatLwork.Text = temp.pCareer;
                ViewPatLphone.Text = temp.pPhone;
                ViewPatLemail.Text = temp.pEmail;
                ViewPatLwouni.Text = temp.pWorkUnit;
                ViewPatLcontp.Text = temp.pConAddress;
                ViewPatLsurgery.Text = temp.pOpHistory;
                ViewPatLallergy.Text = temp.pAllergy;
                ViewPatLhurt.Text = temp.pInjuredPart;
                ViewPatLhosp.Text = temp.pHospital;
                ViewPatLre.Text = temp.pDiagnosis;
            }
            Storyboard story = (Storyboard)this.FindResource("QueryPatientClick");
            story.Begin();
        }

        private void DocPinfoButton(object sender, RoutedEventArgs e)
        {
            List<doctor> DocInfo = new List<doctor>();
            doctor myDoc = new doctor();
            DocInfo = myDoc.queryAll("doctor", "duid=" + currentUser.uID.ToString());
            foreach ( doctor temp in DocInfo)
            {
                DocPinfoDname.Text = temp.drname;
                if (temp.dsex == 0)
                {
                    DocPinfoDsex.Text = "女";
                }
                else
                {
                    DocPinfoDsex.Text = "男";
                }
                DocPinfoDbir.Text = (DateTime.Now.Year - currentUser.uBirth.Year).ToString();
                DocPinfoDidc.Text = temp.dIDcard;
                DocPinfoDpho.Text = temp.dphone;
                DocPinfoDwor.Text = temp.dworkunit;
                DocPinfoDdpart.Text = temp.dworkpart;
                DocPinfoDti.Text = temp.dlevel;
                DocPinfoDint.Text = temp.dresume;
                BitmapImage[] the = doctor.getImage(temp.duid);
                DocPinfoPic.Source = the[0];
                DocPinfoPicCardPic.Source = the[1];
            }
            Storyboard story = (Storyboard)this.FindResource("DocPinfoButtonClick");
            story.Begin();
        }

        private void ModifyDocPinfoButton(object sender, RoutedEventArgs e)
        {
            DinfoPageDname.Text = currentUser.uRname;
            if (currentUser.uSex == 0)
            {
                DinfoPageDsex.Text = "女";
            }
            else
            {
                DinfoPageDsex.Text = "男";
            }
            DinfoPageDbrith.Text = currentUser.uBirth.ToString("yyyy年MM月dd日");
            PcasePage.Visibility = Visibility.Hidden;
            DinfoPage.Visibility = Visibility.Visible;
            Storyboard story = (Storyboard)this.FindResource("UsersInfoButtonClick");
            story.Begin();
        }

        private void ModifyPinfoButton(object sender, RoutedEventArgs e)
        {
            PcasePagePname.Text = currentUser.uRname;
            if (currentUser.uSex == 0)
            {
                PcasePagePsex.Text = "女";
            }
            else
            {
                PcasePagePsex.Text = "男";
            }
            int age = DateTime.Now.Year - currentUser.uBirth.Year;
            PcasePagePage.Text = age.ToString();
            PcasePage.Visibility = Visibility.Visible;
            Storyboard story = (Storyboard)this.FindResource("UsersInfoButtonClick");
            story.Begin();
        }

        private void GetReMessage(object sender, RoutedEventArgs e)
        {
            ReMessageCreateBox.Children.Clear();
            List<message> myMessage = new List<message>();
            myMessage = message.queryAll(currentUser.uID);
            foreach (message temp in myMessage)
            {
                if (temp.mreplied == 0 && temp.mdirec == 0)
                {
                    ReMessageCreate(temp.mpname, temp.mtext, temp.mpuid.ToString(), temp.mid.ToString());
                }
            }
            
            Storyboard story = (Storyboard)this.FindResource("PMessageInDocBuClick");
            story.Begin();
        }

        private void ReMessageCreate(string pname,string pmessage,string pid,string mid)
        {
            Border myBorder = new Border();
            Border innerBorder = new Border();
            StackPanel outterPal = new StackPanel();
            StackPanel innerPalone = new StackPanel();
            StackPanel innerPaltwo = new StackPanel();
            TextBlock Prname = new TextBlock();
            TextBlock getMessage = new TextBlock();
            TextBlock reMessage = new TextBlock();
            TextBlock mID = new TextBlock();
            TextBlock pID = new TextBlock();
            TextBlock thepId = new TextBlock();
            Button getPInfo = new Button();
            Button backMessage = new Button();
            Button sendMessage = new Button();
            TextBox writeMessage = new TextBox();

            
            myBorder.Width = 780;
            myBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(13,166,192));
            myBorder.BorderThickness = new Thickness(2);
            myBorder.Margin = new Thickness(-2, -2, -2, 0);
            myBorder.Padding = new Thickness(0, 0, 0, 20);


            innerBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0,131,137));
            innerBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            innerBorder.BorderThickness = new Thickness(1);
            innerBorder.Width = 700;
            innerBorder.Padding = new Thickness(2, 2, 2, 2);


            outterPal.Orientation = Orientation.Vertical;
            outterPal.Margin = new Thickness(40, 15, 0, 0);


            innerPalone.Orientation = Orientation.Horizontal;


            innerPaltwo.Orientation = Orientation.Vertical;


            getPInfo.Content = " 查看信息";
            getPInfo.FontFamily = new FontFamily("微软雅黑");
            getPInfo.FontSize = 16;
            getPInfo.Height = 24;
            getPInfo.Width = 76;
            getPInfo.Cursor = Cursors.Hand;
            getPInfo.Style = (Style)this.FindResource("ButtonStyleFive");
            getPInfo.Margin = new Thickness(80, 0, 0, 0);
            getPInfo.Click += ReMessageQueryPButton;


            backMessage.Content = " 回复";
            backMessage.FontFamily = new FontFamily("微软雅黑");
            backMessage.FontSize = 18;
            backMessage.Height = 24;
            backMessage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            backMessage.Cursor = Cursors.Hand;
            backMessage.Style = (Style)this.FindResource("ButtonStyleFoure");
            backMessage.Margin = new Thickness(20, 0, 0, 0);
            backMessage.Click += ReMessageCreateButton;


            sendMessage.Content = "     发送";
            sendMessage.FontFamily = new FontFamily("微软雅黑");
            sendMessage.FontSize = 16;
            sendMessage.Height = 0;
            sendMessage.Width = 76;
            sendMessage.Cursor = Cursors.Hand;
            sendMessage.Style = (Style)this.FindResource("ButtonStyleFive");
            sendMessage.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            sendMessage.Margin = new Thickness(0, 0, 40, 0);
            sendMessage.Click += ReMessageButtonClick;


            writeMessage.FontFamily = new FontFamily("微软雅黑");
            writeMessage.TextWrapping = TextWrapping.Wrap;
            writeMessage.FontSize = 16;
            writeMessage.Height = 0;
            writeMessage.BorderThickness = new Thickness(0);


            Prname.FontFamily = new FontFamily("微软雅黑");
            Prname.FontSize = 16;
            Prname.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Prname.Foreground = new SolidColorBrush(Color.FromRgb(13,166,192));


            getMessage.TextWrapping = TextWrapping.Wrap;
            getMessage.Width = 700;
            getMessage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            getMessage.FontSize = 18;
            getMessage.FontFamily = new FontFamily("微软雅黑");
            getMessage.Margin = new Thickness(0, 10, 0, 0);



            reMessage.FontFamily = new FontFamily("微软雅黑");
            reMessage.TextWrapping = TextWrapping.Wrap;
            reMessage.FontSize = 18;
            reMessage.Height = 0;
            reMessage.Width = 700;

            mID.Height = 0;
            pID.Height = 0;
            thepId.Height = 0;
            thepId.Width = 0;
            mID.Text = mid;
            pID.Text = pid;
            thepId.Text = pid;

            Prname.Text = pname;
            getMessage.Text = pmessage;


            innerPalone.Children.Add(thepId);
            innerPalone.Children.Add(Prname);
            innerPalone.Children.Add(getPInfo);

            innerPaltwo.Children.Add(backMessage);
            innerPaltwo.Children.Add(writeMessage);
            innerPaltwo.Children.Add(reMessage);
            innerPaltwo.Children.Add(pID);
            innerPaltwo.Children.Add(mID);
            //innerPaltwo.Children.Add(innerBorder);
            innerPaltwo.Children.Add(sendMessage);

           // innerBorder.Child = innerPaltwo;


            outterPal.Children.Add(innerPalone);
            outterPal.Children.Add(getMessage);
            //outterPal.Children.Add(innerPaltwo);
            //outterPal.Children.Add(myBorder);            
            outterPal.Children.Add(innerBorder);


            innerBorder.Child = innerPaltwo;
            myBorder.Child = outterPal;
            

            ReMessageCreateBox.Children.Add(myBorder);
            
           
        }
        /*                            <Border Width="780"  BorderBrush="#ff0da6c0" BorderThickness="2" Margin="-2,-2,-2,0" Padding="0,0,0,20">
                       <StackPanel Orientation="Vertical" Margin="40,15,0,0">
                           <StackPanel Orientation="Horizontal">
                               <TextBlock FontFamily="微软雅黑" FontSize="16" VerticalAlignment="Center" Text="程逸蒙" Foreground="#ff0da6c0" ></TextBlock>
                               <Button Content=" 查看信息" FontFamily="微软雅黑" Height="24" FontSize="16" Width="76" Cursor="Hand" Style="{StaticResource ButtonStyleFive}" Margin="80,0,0,0">
                               </Button>
                           </StackPanel>
                           <TextBlock TextWrapping="Wrap" Width="700" HorizontalAlignment="Left" FontFamily="微软雅黑" FontSize="18" Margin="0,10,0,0" ></TextBlock>
                           <Border BorderBrush="#008389" HorizontalAlignment="Left" BorderThickness="1" Width="700" Padding="2,2,2,2">
                               <StackPanel Orientation="Vertical" >
                                   <Button Content="回复" HorizontalAlignment="Left" FontFamily="微软雅黑" Height="24" FontSize="18" Style="{StaticResource ButtonStyleFoure}" Margin="20,0,0,0" Cursor="Hand" ></Button>
                                   <TextBox FontFamily="微软雅黑" TextWrapping="Wrap" FontSize="16" Height="0" BorderThickness="0"></TextBox>
                                   <TextBlock FontFamily="微软雅黑" TextWrapping="Wrap" FontSize="16" Height="0"></TextBlock>
                               </StackPanel>
             <Button Content="   发送" FontFamily="微软雅黑" Height="24" HorizontalAlignment="Right" FontSize="16" Width="76" Cursor="Hand" Style="{StaticResource ButtonStyleFive}">
                           </Border>
                       </StackPanel>
                   </Border>*/
        private void ReMessageCreateButton(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {                
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBox)
                {
                    (childVisual as TextBox).Height = 100;
                }
                else if (childVisual is Button)
                {
                    (childVisual as Button).Height = 24;
                }
                btn.Height = 0;
            }
        }

        private void ReMessageButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            bool my = false;
            int mid=0;
            int pid=0;
            string text = "";
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBlock)
                {
                    if ((childVisual as TextBlock).Text != "")
                    {
                        if (!my)
                        {
                            pid = Convert.ToInt16((childVisual as TextBlock).Text);
                            my = true;
                        }
                        else
                            mid = Convert.ToInt16((childVisual as TextBlock).Text);
                    }                    
                }
                if(childVisual is TextBox)                
                    text = (childVisual as TextBox).Text;
            }
            my = message.insert(pid, currentUser.uID, text, mid);
            if (my)
            {
                bool IsText = true;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
                {
                    
                    Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                    if (childVisual is TextBox)
                        (childVisual as TextBox).Height = 0;
                    if (childVisual is Button)
                        (childVisual as Button).Height = 0;
                    if (childVisual is TextBlock)
                    {
                        if (IsText)
                        {
                            (childVisual as TextBlock).Height = 100;
                            (childVisual as TextBlock).Text = text;
                            IsText = false;
                        }
                    }
                }
            }
        }

        private void ReMessageQueryPButton(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string pid="0";
            bool re = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBlock)
                {
                    if (!re)
                    {
                        pid = (childVisual as TextBlock).Text;
                        re = true;
                    }
                }
            }
            List<patient> PatLi = new List<patient>();
            PatLi = patient.QueryAll(Convert.ToInt16(pid));
            foreach (patient temp in PatLi)
            {
                ViewPatLrname.Text = temp.pRname;
                if (temp.pSex == 0)
                {
                    ViewPatLsex.Text = "女";
                }
                else
                {
                    ViewPatLsex.Text = "男";
                }
                ViewPatLage.Text = temp.pAge.ToString();
                ViewPatLwork.Text = temp.pCareer;
                ViewPatLphone.Text = temp.pPhone;
                ViewPatLemail.Text = temp.pEmail;
                ViewPatLwouni.Text = temp.pWorkUnit;
                ViewPatLcontp.Text = temp.pConAddress;
                ViewPatLsurgery.Text = temp.pOpHistory;
                ViewPatLallergy.Text = temp.pAllergy;
                ViewPatLhurt.Text = temp.pInjuredPart;
                ViewPatLhosp.Text = temp.pHospital;
                ViewPatLre.Text = temp.pDiagnosis;
            }
            TrainEsetBut.IsEnabled = false;
            Storyboard story = (Storyboard)this.FindResource("QueryPatientClick");
            story.Begin();

        }

        private void DtoPmessageCreate(string pname,string askmes,string remes)
        {
            Border myBorder = new Border();
            Border innerBorder = new Border();
            StackPanel myStapal = new StackPanel();
            TextBlock Prname = new TextBlock();
            TextBlock askmessage = new TextBlock();
            TextBlock remessage = new TextBlock();

            myBorder.Width = 500;
            myBorder.BorderThickness = new Thickness(0);
            myBorder.Padding = new Thickness(0, 0, 0, 20);

            myStapal.Orientation = Orientation.Vertical;
            myStapal.Margin = new Thickness(40, 15, 0, 0);

            Prname.Text = pname;
            Prname.FontFamily = new FontFamily("微软雅黑");
            Prname.FontSize = 16;
            Prname.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Prname.Foreground = new SolidColorBrush(Color.FromRgb(13, 166, 192));

            askmessage.Text = askmes;
            askmessage.TextWrapping = TextWrapping.Wrap;
            askmessage.Width = 420;
            askmessage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            askmessage.FontFamily = new FontFamily("微软雅黑");
            askmessage.FontSize = 16;
            askmessage.Margin = new Thickness(0, 10, 0, 0);

            innerBorder.BorderThickness = new Thickness(1);
            innerBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 131, 137));
            innerBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            innerBorder.Width = 420;
            innerBorder.Padding = new Thickness(2, 2, 2, 2);

            remessage.Text = remes;
            remessage.FontSize = 18;
            remessage.FontFamily = new FontFamily("微软雅黑");
            remessage.Width = 420;
            remessage.TextWrapping = TextWrapping.Wrap;

            innerBorder.Child = remessage;

            myStapal.Children.Add(Prname);
            myStapal.Children.Add(askmessage);
            myStapal.Children.Add(innerBorder);

            myBorder.Child = myStapal;

            DtPmessage.Children.Add(myBorder);
        }

        private void MyDocPageButtonClick(object sender, RoutedEventArgs e)
        {
            DtPmessage.Children.Clear();

            List<doctor> myDoc = new List<doctor>();
            myDoc = patient.queryDoctor(currentUser.uID);
            if (myDoc.Count == 1)
            {
                foreach (doctor temp in myDoc)
                {
                    MycocInfoname.Text = temp.drname;
                    MydocImfowork.Text = temp.dworkunit;
                    MydocImfodpart.Text = temp.dworkpart;
                    MydocImfotitle.Text = temp.dlevel;
                    MydocId.Text = temp.duid.ToString();
                    MydocImfotitlePic.Source = doctor.getImage(temp.duid)[0];
                }
                MydocInfosendB.Content = "    发送";
                MydocInfosendB.IsEnabled = true ;
            }
            else
            {

                MycocInfoname.Text = "您还未添加医生，快去添加吧";
                MydocImfowork.Text = "";
                MydocImfodpart.Text = "";
                MydocImfotitle.Text = "";
                MydocId.Text = "0";
                MydocImfotitlePic.Source = null;
                MydocInfosendB.Content = "    发送";
                MydocInfosendB.IsEnabled = false;
            }

            List<message> myMessage = new List<message>();
            myMessage = message.queryAll(Convert.ToInt16(MydocId.Text), currentUser.uID);
            foreach (message temp1 in myMessage)
            {
                if (temp1.mdirec == 0)
                {
                    if (temp1.mreplied == 1)
                    {
                        foreach (message temp2 in myMessage)
                        {
                            if (temp2.mdirec == temp1.mid)
                                DtoPmessageCreate(currentUser.uRname, temp1.mtext, temp2.mtext);
                        }
                    }
                    else
                    {
                        string my = "";
                        DtoPmessageCreate(currentUser.uRname, temp1.mtext, my);
                    }
                }
            }

            Storyboard story = (Storyboard)this.FindResource("MyDocPageButtonClick");
            story.Begin();
        }

        private void MyDocPageSendClick(object sender, RoutedEventArgs e)
        {
            if (MydocId.Text != "0")
            {
                string text = MydocSendMes.Text;
                bool re = message.insert(currentUser.uID, Convert.ToInt16(MydocId.Text), text);
                if (re)
                {
                    MydocInfosendB.Content = " 发送成功";
                    MydocInfosendB.IsEnabled = false;
                }
                else
                {
                    MydocInfosendB.Content = " 发送失败";
                }
            }
            else
            {
                MydocSendMes.Text = "未添加医生，不能发送";
            }

        }

        private void MyDocPageQueryDoc(object sender, RoutedEventArgs e)
        {
            if (MydocId.Text != "0")
            {
                List<doctor> myDoc = new List<doctor>();
                myDoc = patient.queryDoctor(currentUser.uID);
                foreach (doctor temp in myDoc)
                {
                    DocPinfoDname.Text = temp.drname;
                    if (temp.dsex == 0)
                    {
                        DocPinfoDsex.Text = "女";
                    }
                    else
                    {
                        DocPinfoDsex.Text = "男";
                    }
                    DocPinfoDbir.Text = temp.dage.ToString();
                    DocPinfoDidc.Text = temp.dIDcard;
                    DocPinfoDpho.Text = temp.dphone;
                    DocPinfoDwor.Text = temp.dworkunit;
                    DocPinfoDdpart.Text = temp.dworkpart;
                    DocPinfoDti.Text = temp.dlevel;
                    DocPinfoDint.Text = temp.dresume;
                    DocPinfoPicCardPic.Source = doctor.getImage(temp.duid)[1];
                    DocPinfoPic.Source = doctor.getImage(temp.duid)[0];
                }
                DocInfoPagesetB.IsEnabled = false;
                Storyboard story = (Storyboard)this.FindResource("DocPinfoButtonClick");
                story.Begin();
            }
        }

        private void MyDocPageDeleteDoc(object sender, RoutedEventArgs e)
        {
            if (MydocId.Text != "0")
            {
                bool re = patient.delete(currentUser.uID);
                MydocId.Text = "0";
                MyDocPageButtonClick(sender, e);
            }
        }
        /*                                <Border Width="500"  BorderThickness="0" Margin="-2,-2,-2,0" Padding="0,0,0,20">
                                <StackPanel Orientation="Vertical" Margin="40,15,0,0">
                                    <TextBlock FontFamily="微软雅黑" FontSize="16" VerticalAlignment="Center" Text="程逸蒙" Foreground="#ff0da6c0" ></TextBlock>
                                    <TextBlock TextWrapping="Wrap" Width="420" HorizontalAlignment="Left" FontFamily="微软雅黑" FontSize="18" Margin="0,10,0,0" ></TextBlock>
                                    <Border BorderBrush="#008389" HorizontalAlignment="Left" BorderThickness="1" Width="420" Padding="2,2,2,2">
                                        <TextBlock FontFamily="微软雅黑" Width="420" TextWrapping="Wrap" FontSize="16" Height="0"></TextBlock>
                                    </Border>
                                </StackPanel>
                            </Border>*/


        private void DlistCreate(doctor temp,int coul,int row)
        {
            StackPanel otterPal = new StackPanel();
            StackPanel innerPalone = new StackPanel();
            StackPanel innerPaltwo = new StackPanel();
            StackPanel innerinPal = new StackPanel();
            Border myBorder = new Border();
            TextBlock Drname = new TextBlock();
            TextBlock Dwork = new TextBlock();
            TextBlock Ddpart = new TextBlock();
            TextBlock Dtitle = new TextBlock();
            TextBlock Did = new TextBlock();
            Button Dinfo = new Button();
            Button addDoc = new Button();
            Image image = new Image();

            image.Height = 102;
            image.Width = 72;
            image.Source = doctor.getImage(temp.duid)[0];

            otterPal.Orientation = Orientation.Horizontal;

            myBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            myBorder.BorderThickness = new Thickness(1);
            myBorder.Height = 102;
            myBorder.Width = 72;
            myBorder.Margin = new Thickness(30, 10, 0, 0);

            innerPalone.Orientation = Orientation.Vertical;
            innerPalone.Margin = new Thickness(20, 50, 0, 0);

            Drname.Text = temp.drname;
            Drname.FontFamily = new FontFamily("微软雅黑");
            Drname.FontSize = 20;
            Drname.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            innerinPal.Orientation = Orientation.Horizontal;
            innerinPal.Margin = new Thickness(0, 10, 0, 0);

            Dwork.Text = temp.dworkunit;
            Dwork.FontFamily = new FontFamily("微软雅黑");
            Dwork.FontSize = 18;
            Dwork.Width = 140;
            Dwork.TextWrapping = TextWrapping.Wrap;
            Dwork.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Ddpart.Text = temp.dworkpart;
            Ddpart.FontFamily = new FontFamily("微软雅黑");
            Ddpart.FontSize = 18;
            Ddpart.Width = 70;
            Ddpart.TextWrapping = TextWrapping.Wrap;
            Ddpart.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Ddpart.Margin = new Thickness(5, 0, 0, 0);

            Dtitle.Text = temp.dlevel;
            Dtitle.FontFamily = new FontFamily("微软雅黑");
            Dtitle.FontSize = 18;
            Dtitle.Width = 85;
            Dtitle.TextWrapping = TextWrapping.Wrap;
            Dtitle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Dtitle.Margin = new Thickness(5, 0, 0, 0);

            Dinfo.Content = " 个人信息 ";
            Dinfo.Height = 24;
            Dinfo.FontFamily = new FontFamily("微软雅黑");
            Dinfo.FontSize = 15;
            Dinfo.Style = (Style)this.FindResource("ButtonStyleTwo");
            Dinfo.Margin = new Thickness(0, 52, 0, 0);
            Dinfo.Click += FindDocQueryDoc;

            addDoc.Content = " 加为医生 ";
            addDoc.Height = 24;
            addDoc.FontFamily = new FontFamily("微软雅黑");
            addDoc.FontSize = 15;
            addDoc.Style = (Style)this.FindResource("ButtonStyleTwo");
            addDoc.Margin = new Thickness(0, 12, 0, 0);
            addDoc.Click += FindDocAdd;

            Did.Text = temp.did.ToString();
            Did.Height = 0;
            Did.Width = 0;

            innerinPal.Children.Add(Dwork);
            innerinPal.Children.Add(Ddpart);
            innerinPal.Children.Add(Dtitle);

            innerPalone.Children.Add(Drname);
            innerPalone.Children.Add(innerinPal);

            innerPaltwo.Children.Add(Did);
            innerPaltwo.Children.Add(Dinfo);
            innerPaltwo.Children.Add(addDoc);

            myBorder.Child = image;

            otterPal.Children.Add(myBorder);
            otterPal.Children.Add(innerPalone);
            otterPal.Children.Add(innerPaltwo);

            DocListTP.Children.Add(otterPal);
            Grid.SetColumn(otterPal, coul);
            Grid.SetRow(otterPal, row);
        }

        private void FinMyDocPageDown(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string my = btn.Content.ToString();
            int thenum = Convert.ToInt16(my);
            List<doctor> allDoc = new List<doctor>();
            allDoc = patient.queryAllDoctors(currentUser.uID);
            int num = allDoc.Count;
            int n = (thenum-1)*6;
            BitmapImage myImage = new BitmapImage();
           
            foreach (doctor temp in allDoc)
            { 
                myImage = doctor.getImage(temp.duid)[0];
                if (n < n+6)
                {
                    if (n == 0)
                        DlistCreate(temp, 0, 0);
                    else
                    {
                        DlistCreate(temp, n % 2, n / 2);
                    }
                    ++n;
                }
            }
            
        }

        private void FindMyDoc(object sender, RoutedEventArgs e)
        {
            List<doctor> allDoc = new List<doctor>();
            allDoc = patient.queryAllDoctors(currentUser.uID);
            int num = allDoc.Count;
            int last = 0;
            last = num % 6;
            if (last == 0)
                num = num / 6;
            else
                num = num / 6 + 1;
            int n = 0;
            BitmapImage myImage = new BitmapImage();
            foreach (doctor temp in allDoc)
            {
                myImage = doctor.getImage(temp.duid)[0];
                if (n < 6)
                {
                    if (n == 0)
                        DlistCreate(temp,0,0);
                    else
                    {
                        DlistCreate(temp, n % 2, n / 2);
                    }
                    ++n;
                }
            }
            for (int i = 0; i < num; i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(findAllDocButtons, i);
                if (childVisual is Button)
                {
                    (childVisual as Button).Visibility = Visibility.Visible;
                }
            }
            
            Storyboard story = (Storyboard)this.FindResource("FindMyDocBuClick");
            story.Begin();
            
        }

        private void FindDocQueryDoc(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string theid = "0";
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBlock)
                    theid = (childVisual as TextBlock).Text;
            }
            List<doctor> myDoc = new List<doctor>();
            doctor theDoc =new doctor();
            myDoc = theDoc.queryAll("doctor", "did=" + theid);
            foreach (doctor temp in myDoc)
            {
                DocPinfoDname.Text = temp.drname;
                if (temp.dsex == 0)
                {
                    DocPinfoDsex.Text = "女";
                }
                else
                {
                    DocPinfoDsex.Text = "男";
                }
                DocPinfoDbir.Text = temp.dage.ToString();
                DocPinfoDidc.Text = temp.dIDcard;
                DocPinfoDpho.Text = temp.dphone;
                DocPinfoDwor.Text = temp.dworkunit;
                DocPinfoDdpart.Text = temp.dworkpart;
                DocPinfoDti.Text = temp.dlevel;
                DocPinfoDint.Text = temp.dresume;
                DocPinfoPic.Source = doctor.getImage(temp.duid)[0];
                DocPinfoPicCardPic.Source = doctor.getImage(temp.duid)[1];
            }
            DocInfoPagesetB.IsEnabled = false;
            Storyboard story = (Storyboard)this.FindResource("DocPinfoButtonClick");
            story.Begin();
        }

        private void FindDocAdd(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string theid = "0";
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBlock)
                    theid = (childVisual as TextBlock).Text;
            }
            bool re = patient.addDoctor(currentUser.uID, Convert.ToInt16(theid));
            if (re)
            {
                MyDocPageButtonClick(sender, e);
            }
        }

        /*<StackPanel Grid.Column="0" Grid.Row="0" Orientation="Horizontal">
                    <Border BorderBrush="#999" BorderThickness="1" Height="102" Width="72" Margin="30,10,0,0"></Border>
                    <StackPanel Orientation="Vertical" Margin="20,50,0,0">
                        <TextBlock Text="某某" FontFamily="微软雅黑" FontSize="20" HorizontalAlignment="Left"></TextBlock>
                        <StackPanel Orientation="Horizontal" Margin="0,10,0,0">
                            <TextBlock Text="某某工作单位" FontFamily="微软雅黑" FontSize="18" Width="140" TextWrapping="Wrap" HorizontalAlignment="Left"></TextBlock>
                            <TextBlock Text="某某科" FontFamily="微软雅黑" FontSize="18" Width="70" TextWrapping="Wrap" HorizontalAlignment="Left" Margin="5,0,0,0"></TextBlock>
                            <TextBlock Text="某某职称" FontFamily="微软雅黑" FontSize="18" Width="85" TextWrapping="Wrap" HorizontalAlignment="Left" Margin="5,0,0,0"></TextBlock>

                        </StackPanel>
                    </StackPanel>
                    <StackPanel Orientation="Vertical" Margin="30,0,0,0">
                        <Button Content=" 个人信息 " Height="24" FontFamily="微软雅黑" FontSize="15" Style="{StaticResource ButtonStyleTwo}" Margin="0,52,0,0"></Button>
                        <Button Content=" 加为医生 " Height="24" FontFamily="微软雅黑" FontSize="15" Style="{StaticResource ButtonStyleTwo}" Margin="0,10,0,0"></Button>
                    </StackPanel>
                </StackPanel>*/
        private void GetPlistButton(object sender, RoutedEventArgs e)
        {
            Pcaselist.Children.Clear();
            List<patient> Plist = new List<patient>();
            Plist = doctor.queryPatient(currentUser.uID);
            foreach (patient temp in Plist)
            {
                string caseid = temp.pID.ToString();
                string pname = temp.pRname;
                string psex;
                if (temp.pSex == 0)
                {
                    psex = "女";
                }
                else
                {
                    psex = "男";
                }
                string page = temp.pAge.ToString();
                string theHurt = temp.pInjuredPart;
                PlistCreate(caseid, pname, psex, page, theHurt,temp.puid.ToString());
            }
            
            Storyboard story = (Storyboard)this.FindResource("PListInDocBuClick");
            story.Begin();
        }


        private void PlistCreate(string caseID,string pname,string thesex,string theage,string thehurt,string thepid )
        {
            StackPanel outterPal = new StackPanel();
            TextBlock Pid = new TextBlock();
            TextBlock idcord = new TextBlock();
            TextBlock prname = new TextBlock();
            TextBlock psex = new TextBlock();
            TextBlock page = new TextBlock();
            TextBlock phurt = new TextBlock();
            Button PlistInfo = new Button();
            Button Ptrain = new Button();

            idcord.Text = caseID;
            prname.Text = pname;
            psex.Text = thesex;
            page.Text = theage;
            phurt.Text = thehurt;

            outterPal.Orientation = Orientation.Horizontal;
            outterPal.Height = 30;
            outterPal.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            idcord.Width = 60;
            idcord.Padding = new Thickness(0, 1, 0, 0);
            idcord.FontFamily = new FontFamily("微软雅黑");
            idcord.FontSize = 18;
            idcord.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            idcord.TextAlignment = TextAlignment.Center;
            idcord.Margin = new Thickness(60, 0, 0, 0);

            prname.TextWrapping = TextWrapping.Wrap;
            prname.Width = 95;
            prname.Padding = new Thickness(0, 1, 0, 0);
            prname.FontFamily = new FontFamily("微软雅黑");
            prname.FontSize = 18;
            prname.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            prname.TextAlignment = TextAlignment.Center;
            prname.Margin = new Thickness(70, 0, 0, 0);

            psex.Width = 40;
            psex.Padding = new Thickness(0, 1, 0, 0);
            psex.FontFamily = new FontFamily("微软雅黑");
            psex.FontSize = 18;
            psex.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            psex.TextAlignment = TextAlignment.Center;
            psex.Margin = new Thickness(75, 0, 0, 0);

            page.Width = 60;
            page.Padding = new Thickness(0, 1, 0, 0);
            page.FontFamily = new FontFamily("微软雅黑");
            page.FontSize = 18;
            page.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            page.TextAlignment = TextAlignment.Center;
            page.Margin = new Thickness(50, 0, 0, 0);

            phurt.TextWrapping = TextWrapping.Wrap;
            phurt.Width = 240;
            phurt.Padding = new Thickness(0, 1, 0, 0);
            phurt.FontFamily = new FontFamily("微软雅黑");
            phurt.FontSize = 18;
            phurt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            phurt.TextAlignment = TextAlignment.Center;
            phurt.Margin = new Thickness(30, 0, 0, 0);

            PlistInfo.Content = "查看病例";
            PlistInfo.FontFamily = new FontFamily("微软雅黑");
            PlistInfo.FontSize = 18;
            PlistInfo.Style = (Style)this.FindResource("ButtonStyleFoure");
            PlistInfo.Margin = new Thickness(5, 0, 0, 0);
            PlistInfo.Cursor = Cursors.Hand;
            PlistInfo.Click += PlistQuerycase;

            Ptrain.Content = "训练档案";
            Ptrain.FontFamily = new FontFamily("微软雅黑");
            Ptrain.FontSize = 18;
            Ptrain.Style = (Style)this.FindResource("ButtonStyleFoure");
            Ptrain.Margin = new Thickness(50, 0, 0, 0);
            Ptrain.Cursor = Cursors.Hand;
            Ptrain.Click += PlistQuerytrain;

            Pid.Width = 0;
            Pid.Height = 0;
            Pid.Text = thepid;

            outterPal.Children.Add(Pid);
            outterPal.Children.Add(idcord);
            outterPal.Children.Add(prname);
            outterPal.Children.Add(psex);
            outterPal.Children.Add(page);
            outterPal.Children.Add(phurt);
            outterPal.Children.Add(PlistInfo);
            outterPal.Children.Add(Ptrain);

            Pcaselist.Children.Add(outterPal);
        }

        private void PlistQuerycase(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string pid="0";
            bool re = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBlock)
                {
                    if (childVisual is TextBlock)
                    {
                        if (!re)
                        {
                            pid = (childVisual as TextBlock).Text;
                            re = true;
                        }
                    }
                }
            }
            List<patient> PatLi = new List<patient>();
            PatLi = patient.QueryAll(Convert.ToInt16(pid));
            foreach (patient temp in PatLi)
            {
                ViewPatLrname.Text = temp.pRname;
                if (temp.pSex == 0)
                {
                    ViewPatLsex.Text = "女";
                }
                else
                {
                    ViewPatLsex.Text = "男";
                }
                ViewPatLage.Text = temp.pAge.ToString();
                ViewPatLwork.Text = temp.pCareer;
                ViewPatLphone.Text = temp.pPhone;
                ViewPatLemail.Text = temp.pEmail;
                ViewPatLwouni.Text = temp.pWorkUnit;
                ViewPatLcontp.Text = temp.pConAddress;
                ViewPatLsurgery.Text = temp.pOpHistory;
                ViewPatLallergy.Text = temp.pAllergy;
                ViewPatLhurt.Text = temp.pInjuredPart;
                ViewPatLhosp.Text = temp.pHospital;
                ViewPatLre.Text = temp.pDiagnosis;
            }
            TrainEsetBut.IsEnabled = false;
            Storyboard story = (Storyboard)this.FindResource("QueryPatientClick");
            story.Begin();
        }

        private void PlistQuerytrain(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string pid = "0";
            bool re = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(btn.Parent); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(btn.Parent, i);
                if (childVisual is TextBlock)
                {
                    if (childVisual is TextBlock)
                    {
                        if (!re)
                        {
                            pid = (childVisual as TextBlock).Text;
                            re = true;
                        }
                    }
                }
            }
            TrainReUrname.Text = currentUser.uRname;
            List<testtrain> myTestTrain = new List<testtrain>();
            int Thedays;
            testtrain Newtrain = new testtrain();
            myTestTrain = Newtrain.QueryNew("testtrain1", Convert.ToInt16(pid), out Thedays);
            Thedays += 1;
            TrainDays.Text = "第" + Thedays.ToString() + "天";
            foreach (testtrain temp in myTestTrain)
            {
                Newtrain = temp;
            }
            TrainactionOneNum.Text = Newtrain.point1.ToString() + "%";
            TrainactionTwoNum.Text = Newtrain.point2.ToString() + "%";
            TrainactionThreeNum.Text = Newtrain.point3.ToString() + "%";
            double TarinOne = (Newtrain.point1 / 100.0) * 340;
            double TarinTwo = (Newtrain.point2 / 100.0) * 340;
            double TarinThree = (Newtrain.point3 / 100.0) * 340;
            DoubleAnimation TrainactionOne = new DoubleAnimation();
            TrainactionOne.From = 0;
            TrainactionOne.To = TarinOne;
            TrainactionOne.Duration = TimeSpan.FromSeconds(0.5);
            DoubleAnimation TrainactionTwo = new DoubleAnimation();
            TrainactionTwo.From = 0;
            TrainactionTwo.To = TarinTwo;
            TrainactionTwo.Duration = TimeSpan.FromSeconds(0.5);
            DoubleAnimation TrainactionThree = new DoubleAnimation();
            TrainactionThree.From = 0;
            TrainactionThree.To = TarinThree;
            TrainactionThree.Duration = TimeSpan.FromSeconds(0.5);
            double TarinTotal = 146 - (Newtrain.point / 100.0) * 146;
            if (Newtrain.point < 10)
            {
                TotalFinished.Text = "0" + Newtrain.point.ToString() + "%";
            }
            else
            {
                TotalFinished.Text = Newtrain.point.ToString() + "%";
            }
            DoubleAnimation TheTrainactionTotal = new DoubleAnimation();
            TheTrainactionTotal.From = 146;
            TheTrainactionTotal.To = TarinTotal;
            TheTrainactionTotal.Duration = TimeSpan.FromSeconds(0.5);
            List<testtrain> RecentTrain = new List<testtrain>();
            int RecentDays = 10;
            testtrain theRecenTemp = new testtrain();
            RecentTrain = theRecenTemp.QueryDays("testtrain1", Convert.ToInt16(pid), RecentDays);
            testtrain[] theRecents = new testtrain[RecentDays];
            int count = 0;
            foreach (testtrain temp in RecentTrain)
            {
                theRecents[count] = temp;
                count++;
                if (count > 9)
                {
                    break;
                }
            }
            System.Windows.Media.PointCollection Mypoints = new System.Windows.Media.PointCollection();
            Mypoints.Add(new Point(5, 298));
            Double[] thePointsTemp = new Double[10] { 44, 84, 123, 163, 202, 242, 281, 321, 360, 400 };
            for (int i = 0; i < 10; ++i)
            {
                if (theRecents[i] != null)
                {
                    Double temp = 298 - (theRecents[i].point) / 100.0 * 238;
                    Mypoints.Add(new Point(thePointsTemp[i], temp));
                }
            }
            myMove.Points = Mypoints;
            TrainactionOnePic.BeginAnimation(WidthProperty, TrainactionOne);
            TrainactionTwoPic.BeginAnimation(WidthProperty, TrainactionTwo);
            TrainactionThreePic.BeginAnimation(WidthProperty, TrainactionThree);
            TrainactionTotal.BeginAnimation(HeightProperty, TheTrainactionTotal);
            TrainEsetBut.IsEnabled = false;
            TrainEPage.Visibility = Visibility.Visible;
            TrainRePatList.Visibility = Visibility.Hidden;
            Storyboard story = (Storyboard)this.FindResource("TrainReButtonClick");
            story.Begin();
        }
        /*<StackPanel Orientation="Horizontal" Height="30" VerticalAlignment="Top" >
                        <TextBlock Text="病例号" Width="60" Padding="0,1,0,0" FontFamily="微软雅黑" FontSize="18" HorizontalAlignment="Left" TextAlignment="Center" Margin="60,0,0,0"></TextBlock>
                        <TextBlock Text="姓名" TextWrapping="Wrap" Width="95" Padding="0,1,0,0" FontFamily="微软雅黑" FontSize="18" HorizontalAlignment="Left" TextAlignment="Center" Margin="70,0,0,0"></TextBlock>
                        <TextBlock Text="性别" Width="40" Padding="0,1,0,0" FontFamily="微软雅黑" FontSize="18" HorizontalAlignment="Left" TextAlignment="Center" Margin="75,0,0,0"></TextBlock>
                        <TextBlock Text="年龄" Width="60" Padding="0,1,0,0" FontFamily="微软雅黑" FontSize="18" HorizontalAlignment="Left" TextAlignment="Center" Margin="50,0,0,0"></TextBlock>
                        <TextBlock Text="受伤部位" TextWrapping="Wrap" Width="240" Padding="0,1,0,0" FontFamily="微软雅黑" FontSize="18" HorizontalAlignment="Left" TextAlignment="Center" Margin="30,0,0,0"></TextBlock>
                        <Button Content="查看病例"  FontFamily="微软雅黑" FontSize="18" Style="{StaticResource ButtonStyleFoure}" Margin="5,0,0,0" Cursor="Hand" ></Button>
                        <Button Content="训练档案"  FontFamily="微软雅黑" FontSize="18" Style="{StaticResource ButtonStyleFoure}" Margin="50,0,0,0" Cursor="Hand" ></Button>
     DinfoPageDcard
         * </StackPanel>*/

        private void PcasePagePcheckuP(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //设置文件类型过滤
            dlg.Filter = "图片|*.jpg;*.png;*.gif;*.bmp;*.jpeg";

            // 调用ShowDialog方法显示＂打开文件＂对话框
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //获取所选文件名并在FileNameTextBox中显示完整路径
                string filename = dlg.FileName;
                PcasePagePcheck.Text = filename;
            }
        }
        private void DinfoPageDcardClickup(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //设置文件类型过滤
            dlg.Filter = "图片|*.jpg;*.png;*.gif;*.bmp;*.jpeg";

            // 调用ShowDialog方法显示＂打开文件＂对话框
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //获取所选文件名并在FileNameTextBox中显示完整路径
                string filename = dlg.FileName;
                DinfoPageDcard.Text = filename;
            }
        }

        private void DinfoPagedpicClickup(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //设置文件类型过滤
            dlg.Filter = "图片|*.jpg;*.png;*.gif;*.bmp;*.jpeg";

            // 调用ShowDialog方法显示＂打开文件＂对话框
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //获取所选文件名并在FileNameTextBox中显示完整路径
                string filename = dlg.FileName;
                PicSourceOFd.Text = filename;

                BitmapImage image = new BitmapImage(new Uri(filename));
                image1.Source = image;
                image1.Width = image.Width;
                image1.Height = image.Height;
            }
        }


        private void GetBackStBuClick(object sender, RoutedEventArgs e)
        {
            Storyboard story = (Storyboard)this.FindResource("GetBackSt");
            story.Begin();
        }

        private void StopVoidButton(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            EnterLogInButton(sender, e);
        }

        private void EnterTrainingButtonClick(object sender, RoutedEventArgs e)
        {
            if (currentUser.uState == 1)
            {
                Storyboard story = (Storyboard)this.FindResource("LoadedInTraining");
                story.Begin();
            }
        }
    }

    

    
    
}
