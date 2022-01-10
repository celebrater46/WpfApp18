using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly string calcFailureMessage = "Calculation failed.";
        private static readonly int maxLength = 16;
        
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void initEvents()
        {
            foreach (var ctrl in LogicalTreeHelper.GetChildren(rootGrid))
            {
                // ignore not button
                if (!(ctrl is Button))
                {
                    continue;
                }

                (ctrl as Button).Click += (sender, e) =>
                {
                    string inKey = (sender as Button).Content.ToString();
                    switch (inKey)
                    {
                        case "AC":
                            formula.Text = "0";
                            break;
                        case "=":
                            if (formula.Text == calcFailureMessage)
                            {
                                formula.Text = "0";
                            }
                            else
                            {
                                formula.Text = Calculate(formula.Text);
                            }
                            break;
                        default:
                            if (formula.Text.Length >= maxLength)
                            {
                                break;
                            } else if (formula.Text == "0" && inKey != "/" && inKey != "*" && inKey != "-" &&
                                       inKey != "+")
                            {
                                formula.Text = inKey;
                            } else if (formula.Text == calcFailureMessage)
                            {
                                formula.Text = inKey;
                            }
                            else
                            {
                                formula.Text += inKey;
                            }
                            break;
                    }
                };
            }
        }

        private string Calculate(string formula)
        {
            try
            {
                var p = new System.Diagnostics.Process();
                string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string powerShellFullPath = Path.Combine(systemFolder, @"WindowsPowerShell\v1.0\powershell.exe");

                p.StartInfo.FileName = powerShellFullPath;
                p.StartInfo.Arguments = "-Command " + formula;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;

                p.Start();
                string calcResult = p.StandardOutput.ReadLine();
                p.WaitForExit();
                p.Close();

                if (string.IsNullOrEmpty(calcResult))
                {
                    return calcFailureMessage;
                }
                else
                {
                    return calcResult;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}