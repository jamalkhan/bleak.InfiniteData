using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace bleak.InfiniteData.Tests
{

    [TestClass]
    public class LazyDataTableAccuracyTests
    {
        private string[] upcs = new string[]
        {
            "100000000001",
            "100000000002",
            "100000000003",
            "100000000004",
            "100000000005",
            "100000000006",
            "100000000007",
            "100000000008",
            "100000000009",
            "100000000010",
            "100000000011",
            "100000000012",
            "100000000013",
            "100000000014",
            "100000000015",
            "100000000016",
            "100000000017",
            "100000000018",
            "100000000019",
            "100000000020",
            "100000000021",
            "100000000022",
            "100000000023",
            "100000000024",
            "100000000025",
            "100000000026",
            "100000000027",
            "100000000028",
            "100000000029",
            "100000000030",
        };
        [TestMethod]
        public void SmallProductFeedTestFullPage()
        {
            int rowsToCache = 1000;

            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "SmallProductFeedPaged.csv");
            Assert.IsTrue(File.Exists(filename));

            var ldt = new LazyDataTable(filename: filename, delimiter: ",", textqualifer: "\"", firstRowHasHeaders: true, rowsToCache: rowsToCache);
            int i = 0;

            Assert.AreEqual(30, ldt.Count);
            Assert.AreEqual(0, ldt.LineReferences[0].BeginPosition);
            Assert.AreEqual(null, ldt.LineReferences[0].FirstRowIndex);
            Assert.AreEqual(null, ldt.LineReferences[0].LastRowIndex );
            Assert.AreEqual(475, ldt.LineReferences[0].EndPosition);
            Assert.AreEqual(476, ldt.LineReferences[1].BeginPosition);
            Assert.AreEqual(0 ,ldt.LineReferences[1].ExtendedFirstRowIndex);
            Assert.AreEqual(29, ldt.LineReferences[1].ExtendedLastRowIndex);
            Assert.AreEqual(0, ldt.LineReferences[1].FirstRowIndex);
            Assert.AreEqual(29, ldt.LineReferences[1].LastRowIndex);

            Assert.AreEqual(17834, ldt.LineReferences[1].EndPosition);

            List<string> openedUpcs = new List<string>();
            foreach (var row in ldt)
            {
                openedUpcs.Add(row["upc"].ToString());
                i++;
            }

            foreach (var upc in openedUpcs)
            {
                Assert.IsTrue(upcs.Contains(upc));
            }

            foreach (var upc in upcs)
            {
                Assert.IsTrue(openedUpcs.Contains(upc));
            }

            Assert.AreEqual(30, i);
        }
        [TestMethod]
        public void SmallProductFeedTestPaged()
        {
            int rowsToCache = 10;

            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "SmallProductFeed.csv");
            Assert.IsTrue(File.Exists(filename));

            var ldt = new LazyDataTable(filename: filename, delimiter: ",", textqualifer: "\"", firstRowHasHeaders: true, rowsToCache: rowsToCache);
            int i = 0;

            Assert.IsTrue(ldt.Count == 30);

            List<string> openedUpcs = new List<string>();
            foreach (var row in ldt)
            {
                var upc = row["upc"].ToString();
                openedUpcs.Add(upc);
                i++;
            }

            foreach (var upc in openedUpcs)
            {
                Assert.IsTrue(upcs.Contains(upc));
            }

            foreach (var upc in upcs)
            {
                Assert.IsTrue(openedUpcs.Contains(upc));
            }

            Assert.AreEqual(30, i);

        }

        private string[] alphabetTest = new string[]
        {
            "col1",
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z",
        };
        [TestMethod]
        public void AlphabetCSVTest()
        {
            int rowsToCache = 10;

            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "AlphabetCount.csv");
            Assert.IsTrue(File.Exists(filename));

            var rowsInFile = 0;
            foreach (var line in File.ReadLines(filename))
            {
                Assert.AreEqual(line, alphabetTest[rowsInFile++]);
            }
            // Don't count the header
            var dataRowsInFile = rowsInFile - 1;

            var ldt = new LazyDataTable(
                filename: filename,
                delimiter: ",",
                textqualifer: "\"",
                firstRowHasHeaders: true,
                rowsToCache: rowsToCache);


            Assert.AreEqual(26, ldt.Count);
            Assert.AreEqual(26, ldt.LineReferences[0].NumberOfRowsInDataFile);
            Assert.AreEqual(26, ldt.LineReferences[1].NumberOfRowsInDataFile);
            Assert.AreEqual(26, ldt.LineReferences[2].NumberOfRowsInDataFile);
            Assert.AreEqual(26, ldt.LineReferences[3].NumberOfRowsInDataFile);

            Assert.AreEqual(10, ldt.LineReferences[1].NumberOfRowsInLineReference);
            Assert.AreEqual(10, ldt.LineReferences[2].NumberOfRowsInLineReference);
            Assert.AreEqual(6, ldt.LineReferences[3].NumberOfRowsInLineReference);

            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[0], 0);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[1], 1);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[2], 2);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[3], 3);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[4], 4);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[5], 5);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[6], 6);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[7], 7);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[8], 8);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[9], 9);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[10], 10);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[11], 11);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[12], 12);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[13], 13);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[14], 14);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[15], 15);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[16], 16);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[17], 17);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[18], 18);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[19], 19);

            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[0], 0);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[1], 1);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[2], 2);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[3], 3);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[4], 4);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[5], 5);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[6], 6);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[7], 7);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[8], 8);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[9], 9);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[10], 10);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[11], 11);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[12], 12);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[13], 13);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[14], 14);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[15], 15);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[16], 16);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[17], 17);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[18], 18);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[19], 19);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[20], 20);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[21], 21);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[22], 22);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[23], 23);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[24], 24);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[25], 25);

            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[10], 0);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[11], 1);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[12], 2);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[13], 3);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[14], 4);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[15], 5);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[16], 6);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[17], 7);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[18], 8);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[19], 9);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[20], 10);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[21], 11);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[22], 12);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[23], 13);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[24], 14);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[25], 15);

            List<string> openedUpcs = new List<string>();
            int rowNum = 0;
            foreach (var row in ldt)
            {
                var col1 = row["col1"].ToString();
                Assert.AreEqual("abcdefghijklmnopqrstuvwxyz"[rowNum].ToString(), col1);
                rowNum++;
            }

            Assert.AreEqual(rowNum, dataRowsInFile);
        }

        [TestMethod]
        public void SampleCSVTest()
        {
            int rowsToCache = 10;

            Debug.WriteLine($"Starting: {DateTime.Now.ToLongTimeString()}");

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "Sample.csv");
            Assert.IsTrue(File.Exists(filename));

            var rowsInFile = 0;
            foreach (var line in File.ReadLines(filename))
            {
                rowsInFile++;
            }
            // Don't count the header
            var dataRowsInFile = rowsInFile - 1;

            var ldt = new LazyDataTable(
                filename: filename,
                delimiter: ",",
                textqualifer: "\"",
                firstRowHasHeaders: true,
                rowsToCache: rowsToCache);

            Assert.AreEqual(60, ldt.Count);
            
            Assert.AreEqual(ldt.LineReferences[0].IsHeader, true);
            Assert.AreEqual(ldt.LineReferences[0].FirstRowIndex, null);
            Assert.AreEqual(ldt.LineReferences[0].LastRowIndex, null);
            Assert.AreEqual(ldt.LineReferences[0].ExtendedFirstRowIndex, null);
            Assert.AreEqual(ldt.LineReferences[0].ExtendedLastRowIndex, null);
            Assert.AreEqual(ldt.LineReferences[0].BeginPosition, 0);
            Assert.AreEqual(ldt.LineReferences[0].EndPosition, 15);

            Assert.AreEqual(ldt.LineReferences[1].IsHeader, false);
            Assert.AreEqual(ldt.LineReferences[1].FirstRowIndex, 0);
            Assert.AreEqual(ldt.LineReferences[1].LastRowIndex, 9);
            Assert.AreEqual(ldt.LineReferences[1].ExtendedFirstRowIndex, 0);
            Assert.AreEqual(ldt.LineReferences[1].ExtendedLastRowIndex, 19);
            Assert.AreEqual(ldt.LineReferences[1].BeginPosition, 16);
            Assert.AreEqual(ldt.LineReferences[1].EndPosition, 204);
            Assert.AreEqual(ldt.LineReferences[1].NumberOfRowsInDataFile, 60);

            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[0], 0);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[1], 1);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[2], 2);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[3], 3);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[4], 4);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[5], 5);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[6], 6);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[7], 7);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[8], 8);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[9], 9);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[10], 10);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[11], 11);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[12], 12);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[13], 13);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[14], 14);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[15], 15);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[16], 16);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[17], 17);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[18], 18);
            Assert.AreEqual(ldt.LineReferences[1].RelativeRowIndex[19], 19);

            Assert.AreEqual(ldt.LineReferences[2].IsHeader, false);
            Assert.AreEqual(ldt.LineReferences[2].FirstRowIndex, 10);
            Assert.AreEqual(ldt.LineReferences[2].LastRowIndex, 19);
            Assert.AreEqual(ldt.LineReferences[2].ExtendedFirstRowIndex, 0);
            Assert.AreEqual(ldt.LineReferences[2].ExtendedLastRowIndex, 29);
            Assert.AreEqual(ldt.LineReferences[2].BeginPosition, 206);
            Assert.AreEqual(ldt.LineReferences[2].EndPosition, 394);
            Assert.AreEqual(ldt.LineReferences[2].NumberOfRowsInDataFile, 60);

            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[0], 0);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[1], 1);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[2], 2);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[3], 3);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[4], 4);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[5], 5);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[6], 6);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[7], 7);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[8], 8);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[9], 9);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[10], 10);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[11], 11);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[12], 12);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[13], 13);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[14], 14);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[15], 15);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[16], 16);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[17], 17);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[18], 18);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[19], 19);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[20], 20);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[21], 21);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[22], 22);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[23], 23);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[24], 24);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[25], 25);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[26], 26);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[27], 27);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[28], 28);
            Assert.AreEqual(ldt.LineReferences[2].RelativeRowIndex[29], 29);

            Assert.AreEqual(ldt.LineReferences[3].IsHeader, false);
            Assert.AreEqual(ldt.LineReferences[3].FirstRowIndex, 20);
            Assert.AreEqual(ldt.LineReferences[3].LastRowIndex, 29);
            Assert.AreEqual(ldt.LineReferences[3].ExtendedFirstRowIndex, 9);
            Assert.AreEqual(ldt.LineReferences[3].ExtendedLastRowIndex, 39);
            Assert.AreEqual(ldt.LineReferences[3].BeginPosition, 396);
            Assert.AreEqual(ldt.LineReferences[3].EndPosition, 584);
            Assert.AreEqual(ldt.LineReferences[3].NumberOfRowsInDataFile, 60);

            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[10], 00);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[11], 01);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[12], 02);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[13], 03);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[14], 04);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[15], 05);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[16], 06);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[17], 07);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[18], 08);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[19], 09);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[20], 10);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[21], 11);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[22], 12);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[23], 13);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[24], 14);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[25], 15);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[26], 16);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[27], 17);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[28], 18);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[29], 19);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[30], 20);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[31], 21);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[32], 22);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[33], 23);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[34], 24);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[35], 25);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[36], 26);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[37], 27);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[38], 28);
            Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[39], 29);

            Assert.AreEqual(ldt.LineReferences[4].IsHeader, false);
            Assert.AreEqual(ldt.LineReferences[4].FirstRowIndex, 30);
            Assert.AreEqual(ldt.LineReferences[4].LastRowIndex, 39);
            Assert.AreEqual(ldt.LineReferences[4].ExtendedFirstRowIndex, 19);
            Assert.AreEqual(ldt.LineReferences[4].ExtendedLastRowIndex, 49);
            Assert.AreEqual(ldt.LineReferences[4].BeginPosition, 586);
            Assert.AreEqual(ldt.LineReferences[4].EndPosition, 774);
            Assert.AreEqual(ldt.LineReferences[4].NumberOfRowsInDataFile, 60);

            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[20], 00);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[21], 01);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[22], 02);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[23], 03);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[24], 04);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[25], 05);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[26], 06);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[27], 07);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[28], 08);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[29], 09);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[30], 10);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[31], 11);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[32], 12);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[33], 13);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[34], 14);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[35], 15);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[36], 16);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[37], 17);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[38], 18);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[39], 19);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[40], 20);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[41], 21);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[42], 22);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[43], 23);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[44], 24);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[45], 25);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[46], 26);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[47], 27);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[48], 28);
            Assert.AreEqual(ldt.LineReferences[4].RelativeRowIndex[49], 29);

            Assert.AreEqual(ldt.LineReferences[5].IsHeader, false);
            Assert.AreEqual(ldt.LineReferences[5].FirstRowIndex, 40);
            Assert.AreEqual(ldt.LineReferences[5].LastRowIndex, 49);
            Assert.AreEqual(ldt.LineReferences[5].ExtendedFirstRowIndex, 29);
            Assert.AreEqual(ldt.LineReferences[5].ExtendedLastRowIndex, 59);
            Assert.AreEqual(ldt.LineReferences[5].BeginPosition, 776);
            Assert.AreEqual(ldt.LineReferences[5].EndPosition, 964);
            Assert.AreEqual(ldt.LineReferences[5].NumberOfRowsInDataFile, 60);

            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[30], 00);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[31], 01);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[32], 02);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[33], 03);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[34], 04);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[35], 05);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[36], 06);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[37], 07);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[38], 08);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[39], 09);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[40], 10);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[41], 11);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[42], 12);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[43], 13);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[44], 14);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[45], 15);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[46], 16);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[47], 17);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[48], 18);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[49], 19);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[50], 20);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[51], 21);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[52], 22);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[53], 23);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[54], 24);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[55], 25);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[56], 26);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[57], 27);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[58], 28);
            Assert.AreEqual(ldt.LineReferences[5].RelativeRowIndex[59], 29);

            Assert.AreEqual(ldt.LineReferences[6].IsHeader, false);
            Assert.AreEqual(ldt.LineReferences[6].FirstRowIndex, 50);
            Assert.AreEqual(ldt.LineReferences[6].LastRowIndex, 59);
            Assert.AreEqual(ldt.LineReferences[6].ExtendedFirstRowIndex, 39);
            Assert.AreEqual(ldt.LineReferences[6].ExtendedLastRowIndex, 59);
            Assert.AreEqual(ldt.LineReferences[6].BeginPosition, 966);
            Assert.AreEqual(ldt.LineReferences[6].EndPosition, 1154);
            Assert.AreEqual(ldt.LineReferences[6].NumberOfRowsInDataFile, 60);

            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[40], 00);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[41], 01);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[42], 02);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[43], 03);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[44], 04);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[45], 05);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[46], 06);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[47], 07);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[48], 08);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[49], 09);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[50], 10);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[51], 11);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[52], 12);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[53], 13);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[54], 14);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[55], 15);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[56], 16);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[57], 17);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[58], 18);
            Assert.AreEqual(ldt.LineReferences[6].RelativeRowIndex[59], 19);


            //Assert.AreEqual(ldt.LineReferences[3].RelativeRowIndex[20], 11);

            //Assert.IsTrue(ldt.LineReferences[7].IsHeader == false);
            //Assert.IsTrue(ldt.LineReferences[7].FirstRowIndex == 60);
            //Assert.IsTrue(ldt.LineReferences[7].LastRowIndex == 60);
            //Assert.IsTrue(ldt.LineReferences[7].ExtendedFirstRowIndex == 40);
            //Assert.IsTrue(ldt.LineReferences[7].ExtendedLastRowIndex == 59);
            //Assert.IsTrue(ldt.LineReferences[7].BeginPosition == 948);
            //Assert.IsTrue(ldt.LineReferences[7].EndPosition == 1138);

            List<string> openedUpcs = new List<string>();
            int rowNum = 1;
            string formatString = "00";
            foreach (var row in ldt)
            {
                var col1 = row["col1"].ToString();
                var col2 = row["col2"].ToString();
                var col3 = row["col3"].ToString();
                Assert.AreEqual($"r{rowNum.ToString(formatString)}c1", col1);
                Assert.AreEqual($"r{rowNum.ToString(formatString)}c2", col2);
                Assert.AreEqual($"r{rowNum.ToString(formatString)}c3", col3);
                rowNum++;
            }

            Assert.AreEqual(rowNum - 1, dataRowsInFile);
        }
    }
}