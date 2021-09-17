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

namespace GatewayExpress
{
    public partial class SplashScreen : Form
    {
        Thread th;  //declaration of thread as th
        public SplashScreen()
        {
            InitializeComponent();
            th = new Thread(opennewDashboard); //creating thread
            th.SetApartmentState(ApartmentState.STA);   //setting thread as STA
            th.Start(); //start the thread
        }       
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            //command for closing the application
            panel2.Width += 1;
            if (panel2.Width >= 1376)
            {
                timer1.Stop(); 
                this.Close();   //command for closing the application
            }
        }

        private void opennewDashboard(object obj)
        {
            Application.Run(new Dashboard());   //Run Dashboard
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            panel2.Width = 0;
        }
    }
}
