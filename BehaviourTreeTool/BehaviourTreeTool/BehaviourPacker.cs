﻿/*
 * 
 * Created by Matt Filer
 * www.mattfiler.co.uk
 * 
 */

using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace BehaviourTreeTool
{
    public partial class BehaviourPacker : Form
    {
        /* ONLOAD */
        public BehaviourPacker()
        {
            InitializeComponent();
            this.Focus();
        }
        private void BehaviourPacker_Load(object sender, EventArgs e)
        {
            HeaderText.Font = FontManager.GetFont(1, 80);
            HeaderText.Parent = HeaderImage;
            Title4.Font = FontManager.GetFont(0, 20);
        }

        /* UNPACK */
        private void unpackButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(SharedData.pathToBehaviourTrees + "alien_all_search_variants.xml")) {
                /* STARTING */
                unpackButton.Enabled = false;
                repackButton.Enabled = false;
                resetTrees.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                /* COPY _BINARY_BEHAVIOUR TO WORKING DIRECTORY */
                File.Copy(SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\_DIRECTORY_CONTENTS.BML", SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.BML");

                /* CONVERT _BINARY_BEHAVIOUR TO XML */
                new AlienConverter(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.BML", SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.xml").Run();

                /* EXTRACT XML TO SEPARATE FILES */
                string directoryContentsXML = File.ReadAllText(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.xml"); //Get contents from newly converted _DIRECTORY_CONTENTS
                string fileHeader = "<?xml version='1.0' encoding='utf-8'?>\n<Behavior>"; //Premade file header
                int count = 0;

                foreach (string currentFile in Regex.Split(directoryContentsXML, "<File name="))
                {
                    count += 1;
                    if (count != 1)
                    {
                        string[] extractedContents = Regex.Split(currentFile, "<Behavior>"); //Split filename and contents
                        string[] extractedContentsMain = Regex.Split(extractedContents[1], "</File>"); //Split contents and footer
                        string[] fileContents = { fileHeader, extractedContentsMain[0] }; //Write preset header and newly grabbed contents
                        string fileName = "";
                        if (File.Exists(SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\gameismodded.txt") || //legacy
                            File.Exists(SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\packagingtool_hasmodded.ayz"))
                        {
                            fileName = extractedContents[0].Substring(1, extractedContents[0].Length - 9); //Grab filename
                        }
                        else
                        {
                            fileName = extractedContents[0].Substring(1, extractedContents[0].Length - 11); //Grab filename UNMODDED FILE
                        }

                        File.WriteAllLines(SharedData.pathToBehaviourTrees + fileName + ".xml", fileContents); //Write new file
                    }
                }

                /* DELETE EXCESS FILES */
                File.Delete(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.BML");
                File.Delete(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.xml");

                /* DONE */
                Cursor.Current = Cursors.Default;
                unpackButton.Enabled = true;
                repackButton.Enabled = true;
                resetTrees.Enabled = true;
            }

            /* OPEN BRAINIAC */
            ProcessStartInfo brainiacProcess = new ProcessStartInfo();
            brainiacProcess.WorkingDirectory = Environment.CurrentDirectory;
            brainiacProcess.FileName = Environment.CurrentDirectory + "/Brainiac Designer.exe";
            Process myProcess = Process.Start(brainiacProcess);
        }

        /* REPACK */
        private void repackButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(SharedData.pathToBehaviourTrees + "alien_all_search_variants.xml"))
            {
                /* STARTING */
                unpackButton.Enabled = false;
                repackButton.Enabled = false;
                resetTrees.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                /* WRITE NEW _DIRECTORY_CONTENTS XML AND DELETE FILES */
                string compiledBinaryBehaviourContents = "<?xml version=\"1.0\" encoding=\"utf-8\"?><DIR>"; //Start file

                DirectoryInfo workingDirectoryInfo = new DirectoryInfo(SharedData.pathToBehaviourTrees); //Get all files in directory
                foreach (FileInfo currentFile in workingDirectoryInfo.GetFiles())
                {
                    string fileContents = File.ReadAllText(currentFile.FullName); //Current file contents
                    string fileName = currentFile.Name; //Current file name
                    string customFileHeader = "<File name=\"" + fileName.Substring(0, fileName.Length - 3) + "bml\">"; //File header
                    string customFileFooter = "</File>"; //File footer

                    compiledBinaryBehaviourContents += customFileHeader + fileContents.Substring(38) + customFileFooter; //Add to file string
                }

                compiledBinaryBehaviourContents += "</DIR>"; //Finish off file string

                string[] compiledContentsAsArray = { compiledBinaryBehaviourContents };
                File.WriteAllLines(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.xml", compiledContentsAsArray); //Write new file

                /* CONVERT _BINARY_BEHAVIOUR TO BML */
                new AlienConverter(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.xml", SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.bml").Run();

                /* COPY _BINARY_BEHAVIOUR TO GAME AND DELETE FILES */
                File.Delete(SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\_DIRECTORY_CONTENTS.BML");
                File.Copy(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.bml", SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\_DIRECTORY_CONTENTS.BML");
                string[] moddedGameText = { "DO NOT DELETE THIS FILE" };
                File.WriteAllLines(SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\packagingtool_hasmodded.ayz", moddedGameText); //Write modded game text
                File.Delete(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.bml");
                File.Delete(SharedData.pathToBehaviourTrees + "_DIRECTORY_CONTENTS.xml");

                /* DONE */
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Modifications have been imported.");
                unpackButton.Enabled = true;
                repackButton.Enabled = true;
                resetTrees.Enabled = true;
            }
            else
            {
                MessageBox.Show("No modifications have been made! Nothing to import.");
            }
        }

        /* RESET */
        private void resetTrees_Click(object sender, EventArgs e)
        {
            /* STARTING */
            unpackButton.Enabled = false;
            repackButton.Enabled = false;
            resetTrees.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            /* RESET FILE */
            File.WriteAllBytes(SharedData.pathToAI + @"\DATA\BINARY_BEHAVIOR\_DIRECTORY_CONTENTS.BML", Properties.Resources._DIRECTORY_CONTENTS);

            /* DONE */
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Changes reset to vanilla.\nIf you have the editor open, close it and re-open through this window.", "Please relaunch the editor if open!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            unpackButton.Enabled = true;
            repackButton.Enabled = true;
            resetTrees.Enabled = true;
        }

        //Close
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
