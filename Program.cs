/*
 *    Copyright 2021 RePass Cloud Pty Ltd
 *
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 */

using System;
using System.IO;
using System.Net;
using System.Threading;

namespace MakeFTPDir
{
    class Program
    {
        public static string ftpPath;
        public static string subFtpPath;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ftpPath = args[0];
            }
            else
            {
                Console.WriteLine("RePass Cloud Make FTP Directory\n" +
                    "Copyright (c) RePass Cloud Pty Ltd. All rights reserved.\n" +
                    "\n" +
                    "Creates path for upload to FTP server.\n" +
                    "\n" +
                    "\n" +
                    "USAGE:\n" +
                    "\n" +
                    "  mkftpd [ftp://localhost/dir/file.txt]\n");

                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            string ftpDirectory = Path.GetDirectoryName(ftpPath).Replace(@"ftp:\", @"ftp://").Replace(@"\","/");

            bool dirExists = DoesFtpDirectoryExist(dirPath: ftpDirectory);
            if (!dirExists)
            {
                subFtpPath = ftpDirectory.Replace("ftp://localhost/", "") + "/";
                MakeFTPDir(ftpAddress: "localhost", pathToCreate: subFtpPath, login: "ftp_user", password: "ftp_pass");
            }
        }

        public static void MakeFTPDir(string ftpAddress, string pathToCreate, string login, string password) //, byte[] fileContents, string ftpProxy = null)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;

            string[] subDirs = pathToCreate.Split('/');

            string currentDir = string.Format("ftp://{0}", ftpAddress);

            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + "/" + subDir;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(login, password);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                    ftpStream.Close();
                    response.Close();
                }
                catch (Exception e)
                {
                    // directory exists
                    //Console.WriteLine(e);
                }
            }
        }

        public static bool DoesFtpDirectoryExist(string dirPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential("ftp_user", "ftp_pass");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
    }
}
