using log4net.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Log
{
    public class RollingFileAppender : log4net.Appender.RollingFileAppender
    {
        #region 


        private string baseFileName;

        #endregion

        #region 


        public int MaxDaySizeRollBackups
        {
            get;
            set;
        }

        #endregion

        #region 

        public override void ActivateOptions()
        {
            if (base.SecurityContext == null)
            {
                base.SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
            }

            using (base.SecurityContext.Impersonate(this))
            {
                base.File = ConvertToFullPath(base.File.Trim());

                this.baseFileName = base.File;
            }

            base.ActivateOptions();

            this.DeleteOldFiles();
        }


        protected override void AdjustFileBeforeAppend()
        {

            //this.DeleteOldFiles();


            base.AdjustFileBeforeAppend();
        }

        private void DeleteOldFiles()
        {
            if (this.MaxDaySizeRollBackups <= 0)
            {
                return;
            }


            string fullPath = null;

            using (base.SecurityContext.Impersonate(this))
            {
                fullPath = Path.GetFullPath(this.baseFileName);
            }

            ArrayList existingFilesArrayList = this.GetExistingFiles(fullPath);


            string logDirectoryPath = Path.GetDirectoryName(fullPath);



            DateTime targetDate = DateTime.Today.AddDays(-this.MaxDaySizeRollBackups + 1);

            foreach (string existingFileName in existingFilesArrayList)
            {

                // conflict with MaxSizeRollBackups
                // MaxSizeRollBackups will roll log file by filename (fileName_1, fileName_2) based on maximumFileSize.
                //if (!this.AssumeLogFileName(existingFileName))
                //{

                //    continue;
                //}

                string filePath = Path.Combine(logDirectoryPath, existingFileName);

                FileInfo info = new FileInfo(filePath);

                if (info.LastWriteTime < targetDate)
                {
                    base.DeleteFile(filePath);
                }
            }
        }

        /// <summary>
        /// Appender。
        /// </summary>
        /// <example>
        /// <pre>
        ///   File = log\TEST, DatePattern = yyyyMMdd".log", ログ名 = TEST_20140201.log   ==> true
        ///   File = log\,     DatePattern = yyyyMMdd".log", ログ名 = TEST_20140201.log   ==> false
        ///   File = log\,     DatePattern = yyyyMMdd".log", ログ名 = 20140201.log        ==> true
        ///   File = log\,     DatePattern = yyyyMMdd".log", ログ名 = abcdefgh.log        ==> false
        ///   File = log\,     DatePattern = yyyyMMdd".log", ログ名 = 20140201.log.1      ==> false
        /// </pre>
        /// </example>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool AssumeLogFileName(string fileName)
        {
            string expectedFileName = Path.GetFileName(this.baseFileName) + default(DateTime).ToString(base.DatePattern);

            if (fileName.Length < expectedFileName.Length)
            {
                return false;
            }
            else if (fileName.Length > expectedFileName.Length && !Regex.IsMatch(fileName.Substring(expectedFileName.Length), @"^\.\d+$"))
            {
                return false;
            }

            for (int i = 0, length = expectedFileName.Length; i < length; i++)
            {
                char expectedChar = expectedFileName[i];
                char mainChar = fileName[i];

                bool expectedCharIsNumber = ('0' <= expectedChar && expectedChar <= '9');
                bool mainCharIsNumber = ('0' <= mainChar && mainChar <= '9');

                if (expectedCharIsNumber && !mainCharIsNumber)
                {
                    return false;
                }
                else if (!expectedCharIsNumber && expectedChar != mainChar)
                {

                    return false;
                }
            }

            return true;
        }

        private ArrayList GetExistingFiles(string baseFilePath)
        {
            ArrayList result = new ArrayList();

            string directory = null;

            using (base.SecurityContext.Impersonate(this))
            {
                string fullPath = Path.GetFullPath(baseFilePath);

                directory = Path.GetDirectoryName(fullPath);

                if (Directory.Exists(directory))
                {
                    string fileName = Path.GetFileName(fullPath);

                    string[] files = Directory.GetFiles(directory, this.GetWildcardPatternForFile(fileName));

                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            string curFileName = Path.GetFileName(files[i]);
                            if (curFileName.StartsWith(Path.GetFileNameWithoutExtension(fileName)))
                            {
                                result.Add(curFileName);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string GetWildcardPatternForFile(string fileName)
        {
            if (this.PreserveLogFileNameExtension)
            {
                return Path.GetFileNameWithoutExtension(fileName) + ".*" + Path.GetExtension(fileName);
            }
            else
            {
                return fileName + '*';
            }
        }

        #endregion
    }
}
