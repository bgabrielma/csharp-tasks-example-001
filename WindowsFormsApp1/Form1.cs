using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        /**
         * Global variables 
         ***/
        List<string> links = new List<string>();
        string x = string.Empty;

        /**
         * Token to identify an task
         ***/
        CancellationTokenSource cts;

        public Form1()
        {
            InitializeComponent();

            links.Add("https://www.yahoo.com");
            links.Add("https://www.google.com");
            links.Add("https://www.microsoft.com");
            links.Add("https://www.cnn.com");
            links.Add("https://www.stackoverflow.com");
            links.Add("https://www.codeproject.com");
            links.Add("https://www.facebook.com");
            links.Add("https://www.policiarcc.com");
        }

        public void resetConfig()
        {
            progressBar1.Value = 0;
            richTextBox1.Text = "";
        }

        /*
         * ATENÇÃO - QUALQUER EVENTO de botão deverá ser VOID! não TASK! 
         **/

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                /**
                 * Instance cts variable. This is required to "restart" or "recreate" the task
                 ***/
                cts = new CancellationTokenSource();

                /**
                 * Reset progress bar and clear textbox resut
                 ***/
                resetConfig();

                /**
                 * Clock to count this operation. 
                 ***/
                var watch = Stopwatch.StartNew();

                /**
                 * Begin process. Use await do asyn functions. 
                 * Em PT: await runDownload. O programa espera estas funções em background serem resolvidas/processadas
                 ***/
                await runDownloadSync(cts.Token); // Em PT: envio um parâmetro "cts" - já definida para identificar a task.
                watch.Stop();
                MessageBox.Show($"Tempo decorrido: { watch.ElapsedMilliseconds / 1000 } segundos");
            }
            catch(OperationCanceledException ex) /* C# Tasks cancelation event */
            {
                resetConfig();
                richTextBox1.Text = $" - Esta operação foi cancelada!";
                button1.Enabled = true;
            }
        }

        private async Task runDownloadSync(CancellationToken cancellationToken)
        {
            button1.Enabled = false;
            foreach(string url in links)
            {
                // Em PT: Aqui é o segredo do cancelamento. Eu "atiro" ao sistema um aviso para cancelar a task de acordo com o meu parâmetro.
                cancellationToken.ThrowIfCancellationRequested();

                var value = progressBar1.Value;
                /**
                * Método task - corre as funções em background
                ***/
                await Task.Run(() => DownloadWebsite(url));

                richTextBox1.Text += url + "\n";
                progressBar1.Value = (value + 20 > 100) ? progressBar1.Value = 100 : progressBar1.Value += 20;
            }
            button1.Enabled = true;
        }

        private void DownloadWebsite(string url)
        {
            WebClient client = new WebClient();
            client.DownloadString(url);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }
    }
}
