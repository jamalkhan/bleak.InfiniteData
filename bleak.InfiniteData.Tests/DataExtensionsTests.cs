using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;

namespace bleak.InfiniteData.Tests
{

    [TestClass]
    public class DataExtensionsTests
    {
        [TestMethod]
        public void ToDelimitedValueTests()
        {
            var table = new DataTable();
            table.Columns.Add("Column1");
            table.Columns.Add("Column2");
            table.Columns.Add("Column3");


            var rows = new List<DataRow>();
            var row1 = table.NewRow();
            row1["Column1"] = "Row1[Column1]";
            row1["Column2"] = "Row1[Column2]";
            row1["Column3"] = "Row1[Column3]";
            table.Rows.Add(row1);

            var row2 = table.NewRow();
            row2["Column1"] = "Row2[Column1]";
            row2["Column2"] = "Row2[Column2]";
            row2["Column3"] = "Row2[Column3]";
            table.Rows.Add(row2);

            var test1 = table.ToDelimitedValue(
                        fieldSeparator: ",",
                        withHeaderRow: false,
                        textQualifier: "\"",
                        lineSeparator: "\n");

            Assert.AreEqual(test1, "\"Row1[Column1]\",\"Row1[Column2]\",\"Row1[Column3]\"\n\"Row2[Column1]\",\"Row2[Column2]\",\"Row2[Column3]\"\n");

            var test2 = table.ToDelimitedValue(
                        fieldSeparator: ",",
                        withHeaderRow: false,
                        textQualifier: "",
                        lineSeparator: "\n");

            Assert.AreEqual(test2, "Row1[Column1],Row1[Column2],Row1[Column3]\nRow2[Column1],Row2[Column2],Row2[Column3]\n");

            var test3 = table.ToDelimitedValue(
                        fieldSeparator: ",",
                        withHeaderRow: true,
                        textQualifier: "\"",
                        lineSeparator: "\n");

            Assert.AreEqual(test3, "\"Column1\",\"Column2\",\"Column3\"\n\"Row1[Column1]\",\"Row1[Column2]\",\"Row1[Column3]\"\n\"Row2[Column1]\",\"Row2[Column2]\",\"Row2[Column3]\"\n");

            var test4 = table.ToDelimitedValue(
                        fieldSeparator: ",",
                        withHeaderRow: true,
                        textQualifier: "",
                        lineSeparator: "\n");

            Assert.AreEqual(test4, "Column1,Column2,Column3\nRow1[Column1],Row1[Column2],Row1[Column3]\nRow2[Column1],Row2[Column2],Row2[Column3]\n");
        }
    }
}