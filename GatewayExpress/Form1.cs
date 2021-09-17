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
using System.Runtime.InteropServices;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace GatewayExpress
{
    public partial class Form1 : Form
    {
        Thread th;  //declaration of thread as th
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );


        public Form1()
        {
            InitializeComponent();
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 25, 25));
            btnLogin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnLogin.Width, btnLogin.Height, 50, 50));
        }

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "P2k1udVSBnRvxFKNk1Ve5OVZo8A4x90jWAEFXqYH",
            BasePath = "https://lancaster-new-city-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(ifc);
            }
            catch 
            {
                MessageBox.Show("No Internet or Connection Problem");
            }

        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = null;
                txtUsername.ForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
            {
                txtUsername.Text = "Username";
                txtUsername.ForeColor = Color.DarkGray;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Password")
            {
                txtPassword.Text = null;
                txtPassword.ForeColor = Color.Black;
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                txtPassword.Text = "Password";
                txtPassword.ForeColor = Color.DarkGray;
                txtPassword.UseSystemPasswordChar = false;
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                #region Condition

                if (txtUsername.Text == "Username")
                {
                    txtUsername.Text = null;
                }
                if (txtPassword.Text == "Password")
                {
                    txtPassword.Text = null;
                }
                if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please Fill All The Fields");

                    txtUsername.Text = "Username";
                    txtPassword.Text = "Password";
                    txtUsername.ForeColor = Color.DarkGray;
                    txtPassword.ForeColor = Color.DarkGray;
                    return;
                }
                #endregion

                
                
                var result1 = client.Get("ADMIN_USER");
                MyUser std = result1.ResultAs<MyUser>();
                int idCounter = int.Parse(std.ADMIN_COUNTER);
                for (int i = 1; i <= idCounter; i++) 
                {
                    FirebaseResponse res = client.Get(@"ADMIN_ACCOUNT/" + i.ToString());
                    MyUser ResUser = res.ResultAs<MyUser>();//database result
                    MyUser CurUser = new MyUser() //USER GIVEN INFO
                    {
                        USERNAME = txtUsername.Text,
                        PASSWORD = txtPassword.Text
                    };
                    //MessageBox.Show(MyUser.CurUser.ToString());
                    if (MyUser.IsEqual(ResUser, CurUser))
                    {
                        this.Close();   //command for closing the application
                        th = new Thread(openSplashScreen); //creating thread
                        th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
                        th.Start(); //start the thread
                    }
                }
                    MyUser.ShowError();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void opennewDashboard(object obj)
        {
            Application.Run(new Dashboard());   //Run Dashboard
        }
        private void openSplashScreen(object obj)
        {
            Application.Run(new SplashScreen());   //Run Dashboard
        }


        private void btnSee_MouseEnter(object sender, EventArgs e)
        {

            txtPassword.UseSystemPasswordChar = false;
        }

        private void btnSee_MouseLeave(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    #region Condition

                    if (txtUsername.Text == "Username")
                    {
                        txtUsername.Text = null;
                    }
                    if (txtPassword.Text == "Password")
                    {
                        txtPassword.Text = null;
                    }
                    if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                        string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Please Fill All The Fields");

                        txtUsername.Text = "";
                        txtPassword.Text = "Password";
                        txtUsername.ForeColor = Color.Black;
                        txtPassword.ForeColor = Color.DarkGray;
                        return;
                    }
                    #endregion

                    FirebaseResponse res = client.Get(@"ADMINISTRATOR/" + txtUsername.Text);
                    MyUser ResUser = res.ResultAs<MyUser>();//database result

                    MyUser CurUser = new MyUser() //USER GIVEN INFO
                    {
                        USERNAME = txtUsername.Text,
                        PASSWORD = txtPassword.Text
                    };
                    //MessageBox.Show(MyUser.USERNAME.ToString());
                    if (MyUser.IsEqual(ResUser, CurUser))
                    {
                        this.Close();   //command for closing the application
                        th = new Thread(openSplashScreen); //creating thread
                        th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
                        th.Start(); //start the thread
                    }
                    else
                    {
                        MyUser.ShowError();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    #region Condition

                    if (txtUsername.Text == "Username")
                    {
                        txtUsername.Text = null;
                    }
                    if (txtPassword.Text == "Password")
                    {
                        txtPassword.Text = null;
                    }
                    if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                        string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Please Fill All The Fields");

                        txtUsername.Text = "Username";
                        txtPassword.Text = "";
                        txtUsername.ForeColor = Color.DarkGray;
                        txtPassword.ForeColor = Color.Black;
                        return;
                    }
                    #endregion

                    FirebaseResponse res = client.Get(@"ADMINISTRATOR/" + txtUsername.Text);
                    MyUser ResUser = res.ResultAs<MyUser>();//database result

                    MyUser CurUser = new MyUser() //USER GIVEN INFO
                    {
                        USERNAME = txtUsername.Text,
                        PASSWORD = txtPassword.Text
                    };
                    //MessageBox.Show(MyUser.USERNAME.ToString());
                    if (MyUser.IsEqual(ResUser, CurUser))
                    {
                        this.Close();   //command for closing the application
                        th = new Thread(openSplashScreen); //creating thread
                        th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
                        th.Start(); //start the thread
                    }
                    else
                    {
                        MyUser.ShowError();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
