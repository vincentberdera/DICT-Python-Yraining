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
using System.Drawing.Printing;

namespace GatewayExpress
{
    public partial class VisitorsPass : Form
    {
        string visitorPassNumber;

        MyUser c = new MyUser();
        Thread th;  //declaration of thread as th
        PrintPreviewDialog prntprvw = new PrintPreviewDialog();
        PrintDocument pntdoc = new PrintDocument();
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
        public VisitorsPass()
        {
            InitializeComponent();
            panel1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 25, 25));
            btnPrint.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnPrint.Width, btnPrint.Height, 50, 50));
        }

        //public void ab(string a)
        //{
//visitorPassNumber = a.ToString();
        //}

        private void VisitorsPass_Load(object sender, EventArgs e)
        {
            int visitorPassNumber2 = int.Parse(Dashboard.visitPass);
            visitorPassNumber2++;
            visitorPassNumber = visitorPassNumber2.ToString();
            lblVisitorsCount.Text = visitorPassNumber.ToString();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print(this.panel1);
            this.Close();
        }

        public void Print(Panel pnl) 
        {
            PrinterSettings ps = new PrinterSettings();
            panel1 = pnl;
            getprintarea(pnl);
            prntprvw.Document = pntdoc;
            pntdoc.PrintPage += new PrintPageEventHandler(pntdoc_printpage);
            prntprvw.ShowDialog();
        }
        public void pntdoc_printpage(object sender, PrintPageEventArgs e)
        {
            Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(memorying,(pagearea.Width/2)-(this.panel1.Width/2),this.panel1.Location.Y);
        }
        Bitmap memorying;
        public void getprintarea(Panel pnl)
        {
            memorying = new Bitmap(pnl.Width,pnl.Height);
            pnl.DrawToBitmap(memorying, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }


        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {
            
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
        }
    }
}
