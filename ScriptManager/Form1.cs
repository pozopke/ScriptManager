﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ScriptManager.Models;
using Newtonsoft.Json;

namespace Scriptmanager
{
    public partial class Form1 : Form
    {
        string apiSearchChampion = "http://www.bol-tools.com/api/search/champion/";
        string currentPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currentPath = Path.GetDirectoryName(Application.ExecutablePath);
            // Go get all campions

            var championsDataSource = new List<ComboBoxItem>();
            championsDataSource.Add(new ComboBoxItem("Garen", "MonkeyKing"));
            championsDataSource.Add(new ComboBoxItem("Taric", "Taric"));
            championsDataSource.Add(new ComboBoxItem("Wukong", "Garen"));

            
            // set display & value, readonly
            this.cboChampionsList.DataSource = championsDataSource;
            this.cboChampionsList.DisplayMember = "Name";
            this.cboChampionsList.ValueMember = "Value";
        }

        private void cboChampionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedChampionkey = this.cboChampionsList.SelectedValue;

            using( WebClient wc = new WebClient())
            {
                var jsonresult = wc.DownloadString(apiSearchChampion + selectedChampionkey);
                dynamic stuff = JsonConvert.DeserializeObject(jsonresult);
                
                foreach(var script in stuff)
                {
                    // title, author, forum, download
                    Script scriptObject = JsonConvert.DeserializeObject<Script>(script.ToString());
                    grid_champions.Rows.Add(scriptObject.Title, scriptObject.Author, scriptObject.ForumUrl, "Download", scriptObject.UpdateUrl);
                }
            }

            
        }

        private void grid_champions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int cellIndex = e.ColumnIndex;
            // cell[3] is download button
            if (cellIndex == 3)
            {
                // better get good script else drama will cum
                DataGridViewRow row = grid_champions.Rows[e.RowIndex];
                string downloadUrl = row.Cells[4].Value.ToString();
                string scriptTitle = row.Cells[0].Value.ToString();

                using( var client = new WebClient())
                {
                    client.DownloadFile(downloadUrl, scriptTitle+".lua");
                    // downloaded on current folder location
                    // now, we'll move it
                    if(File.Exists(scriptTitle+".lua"))
                    {
                        string bolDllPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../")) + "agent.dll";
                        if(File.Exists(bolDllPath))
                        {
                            string bolScriptsPath = Path.GetFullPath(Path.Combine(bolDllPath, @"/Scripts/"));
                            File.Move(scriptTitle + ".lua", bolScriptsPath);
                        }
                        else
                        {
                            MessageBox.Show("Please, put this application in a folder, which need to be in your BoL folder.", "heeey :(] ");
                        }
                    }
                    else
                    {
                        MessageBox.Show("It seems the downloaded file ran aways :(", "Damn it !!");
                    }
                    
                }

                Console.WriteLine(downloadUrl);
            }

            
        }
    }
}
