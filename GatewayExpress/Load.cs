using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using MessagingToolkit.Barcode;
using BasselTech_CamCapture;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace GatewayExpress
{
    public partial class Load : Form
    {

        Thread th;  //declaration of thread as th
        string referenceNumber;
        int panelWidth;
        bool Hidden;
        double Balance=0;
        string birthDate, cityAddress, dateAndTime, gender, homeAddress, licenseNumber, middleName, nationality, password, phoneNumber, telephoneNumber, status, accountStatus, purpose;

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "P2k1udVSBnRvxFKNk1Ve5OVZo8A4x90jWAEFXqYH",
            BasePath = "https://lancaster-new-city-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        public Load()
        {
            InitializeComponent();
            panelWidth = PanelSlide.Width;
            Hidden = false;

            timer3.Start();
        }

        void clear()
        {
            txtId.Text = "";
            txtUsername.Text = "";
            txtGivenName.Text = "";
            txtLastName.Text = "";
            txtAmount.Text = "";
            Balance = 0;
        }


        private void btnToll_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();   //command for closing the application
                th = new Thread(openNewEToll); //creating thread
                th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
                th.Start(); //start the thread
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            this.Close();   //command for closing the application
            th = new Thread(openSplashScreen); //creating thread
            th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
            th.Start(); //start the thread
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();   //command for closing the application
            th = new Thread(openNewForm1); //creating thread
            th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
            th.Start(); //start the thread
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (Hidden)
            {
                PanelSlide.Width = PanelSlide.Width + 10;
                if (PanelSlide.Width >= panelWidth)
                {
                    timer1.Stop();
                    Hidden = false;
                    this.Refresh();
                }
            }
            else
            {
                PanelSlide.Width = PanelSlide.Width - 10;
                if (PanelSlide.Width <= 0)
                {
                    timer1.Stop();
                    Hidden = true;
                    this.Refresh();
                }
            }
        }

        private void btnSidebar_Click_1(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void Load_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(ifc);
            }
            catch
            {
                MessageBox.Show("No Internet or Connection Problem");
            }

            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                comboBox1.Items.Add(filterInfo.Name);
            comboBox1.SelectedIndex = 0;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            this.lblDate.Text = time.ToString("MM/dd/yyyy");
            this.lblTime.Text = time.ToString("hh:mm tt");
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                captureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
                captureDevice.NewFrame += CaptureDevice_NewFrame;
                captureDevice.Start();
                timer2.Start();
            }

            catch (Exception ex)
            {
                timer2.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            ptbScanner.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Load_FormClosing(object sender, FormClosingEventArgs e)
        {
                try
                {
                    if (captureDevice.IsRunning)
                        captureDevice.Stop();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ptbScanner.Image != null)
                {
                    BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
                    Result result = barcodeDecoder.Decode((Bitmap)ptbScanner.Image);
                    if (result != null)
                    {
                        txtId.Text = result.ToString();
                        timer2.Stop();
                        if (captureDevice.IsRunning)
                            captureDevice.Stop();
                        try
                        {
                            if (txtId.Text != "")
                            {
                                if (txtAmount.Text != "")
                                {
                                    FirebaseResponse res2 = client.Get(@"USER INFO/" + txtId.Text + "/BALANCE");
                                    string holder1 = res2.ResultAs<String>();
                                    Balance = Double.Parse(holder1);


                                    FirebaseResponse res = client.Get(@"LCOUNTER/LCOUNTER");
                                    int Counter = int.Parse(res.ResultAs<String>());
                                    Counter++;
                                    MyUser std1 = new MyUser()
                                    {
                                        LCOUNTER = Counter.ToString()
                                    };
                                    var setter1 = client.Set("LCOUNTER", std1);


                                    if (Counter < 10) { referenceNumber = "L-0000000" + Counter.ToString(); }
                                    else if (Counter < 99) { referenceNumber = "L-000000" + Counter.ToString(); }
                                    else if (Counter < 999) { referenceNumber = "L-00000" + Counter.ToString(); }
                                    else if (Counter < 9999) { referenceNumber = "L-0000" + Counter.ToString(); }
                                    else if (Counter < 99999) { referenceNumber = "L-000" + Counter.ToString(); }
                                    else if (Counter < 999999) { referenceNumber = "L-00" + Counter.ToString(); }
                                    else if (Counter < 9999999) { referenceNumber = "L-0" + Counter.ToString(); }
                                    else { Counter.ToString(); }


                                    MyUser std2 = new MyUser()
                                    {
                                        OR_NUMBER = referenceNumber.ToString().Trim()
                                    };
                                    var setter2 = client.Set("LOAD_SERIAL_NUMBER/" + Counter.ToString(), std2);


                                    MyUser std3 = new MyUser()
                                    {
                                        REFERENCE_NUMBER = referenceNumber.ToString(),
                                        FROM = "COUNTER",
                                        FROM_USERNAME = "COUNTER",
                                        USERNAME = txtUsername.Text,
                                        GIVEN_NAME = txtGivenName.Text,
                                        LAST_NAME = txtLastName.Text,
                                        DATE = lblDate.Text,
                                        TIME = lblTime.Text,
                                        TYPE = "LOAD",
                                        AMOUNT = txtAmount.Text
                                    };
                                    var setter3 = client.Set("LOAD/" + referenceNumber.ToString(), std3);

                                    Balance = Balance + Double.Parse(txtAmount.Text);
                                    MyUser std4 = new MyUser()
                                    {
                                        USERNAME = txtUsername.Text,
                                        PASSWORD = password,
                                        GIVEN_NAME = txtGivenName.Text,
                                        MIDDLE_NAME = middleName,
                                        LAST_NAME = txtLastName.Text,
                                        PHONE_NUMBER = phoneNumber,
                                        TELEPHONE_NUMBER = telephoneNumber,
                                        BIRTHDATE = birthDate,
                                        GENDER = gender,
                                        NATIONALITY = nationality,
                                        HOME_ADDRESS = homeAddress,
                                        CITY_ADDRESS = cityAddress,
                                        DATE_AND_TIME = dateAndTime,
                                        STATUS = status,
                                        BALANCE = Balance.ToString(),
                                        ACCOUNT_STATUS = accountStatus,
                                        PURPOSE = purpose
                                    };
                                    var setter4 = client.Set("USER INFO/" + txtId.Text, std4);

                                    MessageBox.Show("Data Inserted Successfully!");
                                    clear();
                                }
                                else { MessageBox.Show("Enter Load Amount"); }

                            }

                            else
                            {
                                MessageBox.Show("Scan A Bar Code First");
                            }
                            timer2.Stop();
                            if (captureDevice.IsRunning)
                                captureDevice.Stop();
                        }
                        catch (Exception ex1)
                        {

                        }
                        clear();

                    }
                    else
                    {
                        timer2.Stop();
                        if (captureDevice.IsRunning)
                            captureDevice.Stop();
                        //MessageBox.Show("There is no QR code Scanned");
                    }

                }
            }
            catch (Exception ex)
            {
                timer2.Stop();
                if (captureDevice.IsRunning)
                    captureDevice.Stop();
                //MessageBox.Show("There is no QR code Scanned");
            }
        }

        private void txtId_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (txtId.Text != "")
                {
                    var result2 = client.Get("USER INFO/" + txtId.Text);
                    MyUser std = result2.ResultAs<MyUser>();
                    
                    txtUsername.Text = std.USERNAME;
                    txtGivenName.Text = std.GIVEN_NAME;
                    txtLastName.Text = std.LAST_NAME;
                    birthDate = std.BIRTHDATE;
                    cityAddress = std.CITY_ADDRESS;
                    dateAndTime = std.DATE_AND_TIME;
                    gender = std.GENDER;
                    homeAddress = std.HOME_ADDRESS;
                    licenseNumber = std.LICENSE_NUMBER;
                    middleName = std.MIDDLE_NAME;
                    nationality = std.NATIONALITY;
                    password = std.PASSWORD;
                    phoneNumber = std.PHONE_NUMBER;
                    telephoneNumber = std.TELEPHONE_NUMBER;
                    status = std.STATUS;
                    accountStatus = std.ACCOUNT_STATUS;
                    purpose = std.PURPOSE;
                }
                else
                {
                    //MessageBox.Show("There is no QR code Scanned");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("There is no QR code Scanned");
            }
        }

        private void openNewDashBoad(object obj)
        {
            Application.Run(new Dashboard());   //Run Dashboard Tab
        }
        private void openNewEToll(object obj)
        {
            Application.Run(new EToll());   //Run EToll Tab
        }
        private void openNewForm1(object obj)
        {
            Application.Run(new Form1());   //Run Form1 or Login Form Tab
        }
        private void openSplashScreen(object obj)
        {
            Application.Run(new SplashScreen());   //Run SplashScreen
        }
        private void openNewDrivers(object obj)
        {
            Application.Run(new Drivers());   //Run Drivers or User's Info
        }
        private void openNewAdmins(object obj)
        {
            Application.Run(new Admins());   //Run Admin's Info Tab
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void btnAdmins_Click(object sender, EventArgs e)
        {

            this.Close();   //command for closing the application
            th = new Thread(openNewAdmins); //creating thread
            th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
            th.Start(); //start the thread
        }

        private void btnDrivers_Click(object sender, EventArgs e)
        {
            this.Close();   //command for closing the application
            th = new Thread(openNewDrivers); //creating thread
            th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
            th.Start(); //start the thread
        }

        private void btnPunch_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtId.Text != "")
                {
                    if (txtAmount.Text != "")
                    {
                        FirebaseResponse res2 = client.Get(@"USER INFO/" + txtId.Text + "/BALANCE");
                        string holder1 = res2.ResultAs<String>();
                        Balance = Double.Parse(holder1);


                        FirebaseResponse res = client.Get(@"LCOUNTER/LCOUNTER");
                        int Counter = int.Parse(res.ResultAs<String>());
                        Counter++;
                        MyUser std1 = new MyUser()
                        {
                            LCOUNTER = Counter.ToString()
                        };
                        var setter1 = client.Set("LCOUNTER", std1);


                        if (Counter < 10) { referenceNumber = "L-0000000" + Counter.ToString(); }
                        else if (Counter < 99) { referenceNumber = "L-000000" + Counter.ToString(); }
                        else if (Counter < 999) { referenceNumber = "L-00000" + Counter.ToString(); }
                        else if (Counter < 9999) { referenceNumber = "L-0000" + Counter.ToString(); }
                        else if (Counter < 99999) { referenceNumber = "L-000" + Counter.ToString(); }
                        else if (Counter < 999999) { referenceNumber = "L-00" + Counter.ToString(); }
                        else if (Counter < 9999999) { referenceNumber = "L-0" + Counter.ToString(); }
                        else { referenceNumber = Counter.ToString(); }


                        MyUser std2 = new MyUser()
                        {
                            OR_NUMBER = referenceNumber.ToString().Trim()
                        };
                        var setter2 = client.Set("LOAD_SERIAL_NUMBER/" + Counter.ToString(), std2);


                        MyUser std3 = new MyUser()
                        {
                            REFERENCE_NUMBER = referenceNumber.ToString(),
                            FROM = "COUNTER",
                            FROM_USERNAME = "COUNTER",
                            USERNAME = txtUsername.Text,
                            GIVEN_NAME = txtGivenName.Text,
                            LAST_NAME = txtLastName.Text,
                            DATE = lblDate.Text,
                            TIME = lblTime.Text,
                            TYPE = "LOAD",
                            AMOUNT = txtAmount.Text
                        };
                        var setter3 = client.Set("LOAD/" + referenceNumber.ToString(), std3);

                        Balance = Balance + Double.Parse(txtAmount.Text);
                        MyUser std4 = new MyUser()
                        {
                            USERNAME = txtUsername.Text,
                            PASSWORD = password,
                            GIVEN_NAME = txtGivenName.Text,
                            MIDDLE_NAME = middleName,
                            LAST_NAME = txtLastName.Text,
                            PHONE_NUMBER = phoneNumber,
                            TELEPHONE_NUMBER = telephoneNumber,
                            BIRTHDATE = birthDate,
                            GENDER = gender,
                            NATIONALITY = nationality,
                            HOME_ADDRESS = homeAddress,
                            CITY_ADDRESS = cityAddress,
                            DATE_AND_TIME = dateAndTime,
                            STATUS = status,
                            BALANCE = Balance.ToString(),
                            ACCOUNT_STATUS = accountStatus,
                            PURPOSE = purpose
                        };
                        var setter4 = client.Set("USER INFO/" + txtId.Text, std4);

                        MessageBox.Show("Data Inserted Successfully!");
                        clear();
                    }
                    else { MessageBox.Show("Enter Load Amount"); }

                }

                else
                {
                    MessageBox.Show("Scan A Bar Code First");
                }
                timer2.Stop();

            }
            catch (Exception ex)
            {
                timer2.Stop();
                //MessageBox.Show("There is no QR code Scanned");
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
