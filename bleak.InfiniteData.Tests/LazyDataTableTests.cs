using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bleak.InfiniteData.Tests
{
    [TestClass]
    public class LazyDataTableTests
    {/*
        [TestMethod]
        public void LazyDataTableTestFileLFTest()
        {
            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");
            var start = DateTime.Now;
            var util = new RegexUtilities();

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "LazyDataTableTestFileLF.csv");
            Assert.IsTrue(File.Exists(filename));
            var rowsInFile = 1000;
            var rowsToCache = 89;
            var ldt = new LazyDataTable(filename, ",", null, true, rowsToCache: rowsToCache);

            // Assert that all the columns match up.
            var fields = new string[] { "id", "first_name", "last_name", "email", "gender", "ip_address" };
            Assert.IsTrue(ldt.Columns.Count() == fields.Count());
            foreach (var field in fields)
            {
                var column = ldt.Columns.Where(x => x.ColumnName == field).FirstOrDefault();
                Assert.IsNotNull(column);
            }

            Assert.IsTrue(ldt.FileName == filename);
            Assert.IsTrue(ldt.Delimiter == ",");
            Assert.IsTrue(ldt.FirstRowHasHeaders == true);
            Assert.IsTrue(ldt.TextQualifer == null);
            Assert.IsTrue(ldt.RowsToCache == rowsToCache);
            Assert.IsTrue(ldt.Count == 1000);

            LineStreamReference previousReference = null;
            int page = 0;
            foreach (var lineReference in ldt.LineReferences)
            {
                if (previousReference == null)
                {
                    Assert.IsTrue(lineReference.BeginPosition == 0);
                    Assert.IsTrue(lineReference.FirstRowIndex == null);
                    Assert.IsTrue(lineReference.LastRowIndex == null);
                    Assert.IsTrue(lineReference.EndPosition > 0);
                }
                else
                {
                    Assert.IsTrue(lineReference.BeginPosition > previousReference.EndPosition);
                    Assert.IsTrue(lineReference.FirstRowIndex == page++ * rowsToCache);

                    // MaxRow is an Index, not an actual count, so -1.
                    var maxRow = (lineReference.FirstRowIndex + rowsToCache > rowsInFile ? rowsInFile : lineReference.FirstRowIndex + rowsToCache) - 1;
                    Assert.IsTrue(lineReference.LastRowIndex == maxRow);
                    Assert.IsTrue(lineReference.EndPosition > 0);
                }
                previousReference = lineReference;
            }

            // Loop through to determine that all rows are individually right.
            int i = 1;
            foreach (var row in ldt)
            {
                Assert.IsTrue(ldt.CurrentLineReference.LastRowIndex <= ldt.Count);

                Assert.IsTrue(row["id"].ToString() == i.ToString());
                var email = row["email"].ToString();
                Assert.IsTrue(util.IsValidEmail(email));

                var gender = row["gender"].ToString();
                Assert.IsTrue(new string[] { "Male", "Female" }.Contains(gender));
                i++;
            }
            var testTime = DateTime.Now - start;
            Debug.WriteLine($"Milliseconds: {testTime.TotalMilliseconds}");
            Debug.WriteLine($"Complete: {DateTime.Now.ToLongTimeString()}");
        }


        [TestMethod]
        public void QuotedLazyDataTableTestFileCRLFTest()
        {
            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");
            var start = DateTime.Now;
            var util = new RegexUtilities();

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "QuotedLazyDataTableTestFileCRLF.csv");
            Assert.IsTrue(File.Exists(filename));
            var rowsInFile = 1000;
            var rowsToCache = 89;
            var ldt = new LazyDataTable(filename: filename,
                delimiter: ",",
                textqualifer: "\"",
                firstRowHasHeaders: true,
                rowsToCache: rowsToCache);

            // Assert that all the columns match up.
            var fields = new string[] { "id", "first_name", "last_name", "email", "gender", "ip_address" };
            Assert.IsTrue(ldt.Columns.Count() == fields.Count());
            foreach (var field in fields)
            {
                var column = ldt.Columns.Where(x => x.ColumnName == field).FirstOrDefault();
                Assert.IsNotNull(column);
            }

            Assert.IsTrue(ldt.FileName == filename);
            Assert.IsTrue(ldt.Delimiter == ",");
            Assert.IsTrue(ldt.FirstRowHasHeaders == true);
            Assert.IsTrue(ldt.TextQualifer == "\"");
            Assert.IsTrue(ldt.RowsToCache == rowsToCache);
            Assert.IsTrue(ldt.Count == 1000);

            LineStreamReference previousReference = null;
            int page = 0;
            foreach (var lineReference in ldt.LineReferences)
            {
                if (previousReference == null)
                {
                    Assert.IsTrue(lineReference.BeginPosition == 0);
                    Assert.IsTrue(lineReference.FirstRowIndex == null);
                    Assert.IsTrue(lineReference.LastRowIndex == null);
                    Assert.IsTrue(lineReference.EndPosition > 0);
                }
                else
                {
                    Assert.IsTrue(lineReference.BeginPosition > previousReference.EndPosition);
                    Assert.IsTrue(lineReference.FirstRowIndex == page++ * rowsToCache);

                    // MaxRow is an Index, not an actual count, so -1.
                    var maxRow = (lineReference.FirstRowIndex + rowsToCache > rowsInFile ? rowsInFile : lineReference.FirstRowIndex + rowsToCache) - 1;
                    Assert.IsTrue(lineReference.LastRowIndex == maxRow);
                    Assert.IsTrue(lineReference.EndPosition > 0);
                }
                previousReference = lineReference;
            }

            // Loop through to determine that all rows are individually right.
            int i = 1;
            foreach (var row in ldt)
            {
                Assert.IsTrue(ldt.CurrentLineReference.LastRowIndex <= ldt.Count);

                Assert.IsTrue(row["id"].ToString() == i++.ToString());
                var email = row["email"].ToString();
                Assert.IsTrue(util.IsValidEmail(email));

                var gender = row["gender"].ToString();
                Assert.IsTrue(new string[] { "Male", "Female" }.Contains(gender));
            }
            var testTime = DateTime.Now - start;
            Debug.WriteLine($"Milliseconds: {testTime.TotalMilliseconds}");
            Debug.WriteLine($"Complete: {DateTime.Now.ToLongTimeString()}");
        }

        [TestMethod]
        public void QuotedLazyDataTableTestFileCRLF_CommasTest()
        {
            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");
            var start = DateTime.Now;
            var util = new RegexUtilities();

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "QuotedLazyDataTableTestFileCRLF_Commas.csv");
            Assert.IsTrue(File.Exists(filename));
            var rowsInFile = 1000;
            var rowsToCache = 797;
            var ldt = new LazyDataTable(filename: filename,
                delimiter: ",",
                textqualifer: "\"",
                firstRowHasHeaders: true,
                rowsToCache: rowsToCache);

            // Assert that all the columns match up.
            var fields = new string[] { "id", "first_name", "last_name", "email", "gender", "ip_address" };
            Assert.IsTrue(ldt.Columns.Count() == fields.Count());
            foreach (var field in fields)
            {
                var column = ldt.Columns.Where(x => x.ColumnName == field).FirstOrDefault();
                Assert.IsNotNull(column);
            }

            Assert.IsTrue(ldt.FileName == filename);
            Assert.IsTrue(ldt.Delimiter == ",");
            Assert.IsTrue(ldt.FirstRowHasHeaders == true);
            Assert.IsTrue(ldt.TextQualifer == "\"");
            Assert.IsTrue(ldt.RowsToCache == rowsToCache);
            Assert.IsTrue(ldt.Count == 1000);

            LineStreamReference previousReference = null;
            int page = 0;
            foreach (var lineReference in ldt.LineReferences)
            {
                if (previousReference == null)
                {
                    Assert.AreEqual(true, lineReference.IsHeader);
                    Assert.IsTrue(lineReference.BeginPosition == 0);
                    Assert.IsTrue(lineReference.FirstRowIndex == null);
                    Assert.IsTrue(lineReference.LastRowIndex == null);
                    Assert.IsTrue(lineReference.EndPosition > 0);
                }
                else
                {
                    Assert.IsTrue(lineReference.BeginPosition > previousReference.EndPosition);
                    Assert.IsTrue(lineReference.FirstRowIndex == page++ * rowsToCache);

                    // MaxRow is an Index, not an actual count, so -1.
                    var maxRow = (lineReference.FirstRowIndex + rowsToCache > rowsInFile ? rowsInFile : lineReference.FirstRowIndex + rowsToCache) - 1;
                    Assert.IsTrue(lineReference.LastRowIndex == maxRow);
                    Assert.IsTrue(lineReference.EndPosition > 0);
                }
                previousReference = lineReference;
            }

            // Loop through to determine that all rows are individually right.
            int i = 1;
            foreach (var row in ldt)
            {
                Assert.IsTrue(ldt.CurrentLineReference.LastRowIndex <= ldt.Count);

                Assert.IsTrue(row["id"].ToString() == i++.ToString());
                var email = row["email"].ToString();
                Assert.IsTrue(util.IsValidEmail(email));

                var gender = row["gender"].ToString();
                Assert.IsTrue(new string[] { "Male", "Female" }.Contains(gender));
            }
            var testTime = DateTime.Now - start;
            Debug.WriteLine($"Milliseconds: {testTime.TotalMilliseconds}");
            Debug.WriteLine($"Complete: {DateTime.Now.ToLongTimeString()}");
        }
        */
        /*
        [TestMethod]
        public void ParallelLazyDataTableTestFileCRLF_CommasTest()
        {
            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");
            var start = DateTime.Now;
            var util = new RegexUtilities();

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "QuotedLazyDataTableTestFileCRLF_Commas.csv");
            var rowsToCache = 100;
            var ldt = new LazyDataTable(filename: filename,
                delimiter: ",",
                textqualifer: "\"",
                firstRowHasHeaders: true,
                rowsToCache: rowsToCache);

            // Assert that all the columns match up.
            var fields = new string[] { "id", "first_name", "last_name", "email", "gender", "ip_address" };

            // This will need changing if ever becomes configurable.
            Assert.IsTrue(ldt.MaxDegreesOfParallelism == 50);

            // Loop through to determine that all rows are individually right.
            //foreach (var row in ldt)
            Parallel.ForEach(
                source: ldt,
                parallelOptions: new ParallelOptions() { MaxDegreeOfParallelism = 20 },
                body: (row) =>
                {
                    Assert.IsTrue(ldt.CurrentLineReference.MaxRow <= ldt.Count);

                    Assert.IsTrue(ldt.CurrentLineReference.MinRow >= 0);
                    Assert.IsTrue(ldt.CurrentLineReference.ExtendedMinRow == ldt.CurrentLineReference.MinRow - ldt.RowsToCache || ldt.CurrentLineReference.ExtendedMinRow == 0);
                    Assert.IsTrue(ldt.CurrentLineReference.ExtendedMaxRow == ldt.CurrentLineReference.MaxRow + ldt.RowsToCache || ldt.CurrentLineReference.ExtendedMaxRow == ldt.Count - 1);
                    Assert.IsTrue(ldt.CurrentLineReference.RelativeRowIndex[ldt.FileIndex] >= 0);
                    Assert.IsTrue(ldt.CurrentLineReference.RelativeRowIndex[ldt.FileIndex] < rowsToCache * 2);

                    //Assert.IsTrue(row["id"].ToString() == i++.ToString());
                    //var email = row["email"].ToString();
                    //Assert.IsTrue(util.IsValidEmail(email));

                    //var gender = row["gender"].ToString();
                    //Assert.IsTrue(new string[] { "Male", "Female" }.Contains(gender));
                });
            var testTime = DateTime.Now - start;
            Debug.WriteLine($"Milliseconds: {testTime.TotalMilliseconds}");
            Debug.WriteLine($"Complete: {DateTime.Now.ToLongTimeString()}");
        }
        */
        /*
        [TestMethod]
        public void ParallelLazyDataTableTestHugeFileCRLF()
        {
            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");
            var start = DateTime.Now;
            var util = new RegexUtilities();

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "hugefile.csv");
            var rowsToCache = 50000;
            var ldt = new LazyDataTable(filename: filename,
                delimiter: ",",
                textqualifer: "\"",
                firstRowHasHeaders: true,
                rowsToCache: rowsToCache);

            // Assert that all the columns match up.
            var fields = new string[] { "id", "first_name", "last_name", "email", "gender", "ip_address" };

            // This will need changing if ever becomes configurable.
            Assert.IsTrue(ldt.MaxDegreesOfParallelism == 50);

            // Loop through to determine that all rows are individually right.
            //foreach (var row in ldt)
            Parallel.ForEach(
                source: ldt,
                parallelOptions: new ParallelOptions() { MaxDegreeOfParallelism = 20 },
                body: (row) =>
                {
                    Assert.IsTrue(ldt.CurrentLineReference.LastRowIndex <= ldt.Count);

                    Assert.IsTrue(ldt.CurrentLineReference.FirstRowIndex >= 0);
                    //Assert.IsTrue(ldt.CurrentLineReference.ExtendedFirstRowIndex >= ldt.CurrentLineReference.FirstRowIndex - ldt.RowsToCache || (ldt.CurrentLineReference.ExtendedFirstRowIndex >= 0);
                    //Assert.IsTrue(ldt.CurrentLineReference.ExtendedLastRowIndex == ldt.CurrentLineReference.LastRowIndex + ldt.RowsToCache || ldt.CurrentLineReference.ExtendedLastRowIndex == ldt.Count - 1);
                    //Assert.IsTrue(ldt.CurrentLineReference.RelativeRowIndex[ldt.FileIndex] >= 0);
                    //Assert.IsTrue(ldt.CurrentLineReference.RelativeRowIndex[ldt.FileIndex] < rowsToCache * 2);

                    //Assert.IsTrue(row["id"].ToString() == i++.ToString());
                    //var email = row["email"].ToString();
                    //Assert.IsTrue(util.IsValidEmail(email));

                    //var gender = row["gender"].ToString();
                    //Assert.IsTrue(new string[] { "Male", "Female" }.Contains(gender));
                });
            var testTime = DateTime.Now - start;
            Debug.WriteLine($"Milliseconds: {testTime.TotalMilliseconds}");
            Debug.WriteLine($"Complete: {DateTime.Now.ToLongTimeString()}");
        }
        */


        public class RegexUtilities
        {
            private bool invalid = false;

            public bool IsValidEmail(string strIn)
            {
                invalid = false;
                if (String.IsNullOrEmpty(strIn))
                {
                    return false;
                }

                // Use IdnMapping class to convert Unicode domain names.
                try
                {
                    strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                          RegexOptions.None, TimeSpan.FromMilliseconds(200));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }

                if (invalid)
                {
                    return false;
                }

                // Return true if strIn is in valid e-mail format.
                try
                {
                    return Regex.IsMatch(strIn,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }

            private string DomainMapper(Match match)
            {
                // IdnMapping class with default property values.
                IdnMapping idn = new IdnMapping();

                string domainName = match.Groups[2].Value;
                try
                {
                    domainName = idn.GetAscii(domainName);
                }
                catch (ArgumentException)
                {
                    invalid = true;
                }
                return match.Groups[1].Value + domainName;
            }
        }
    }
}