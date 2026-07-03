using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hospital.Shared.Common;

public static class CsvExporter
{
    public static byte[] ExportToCsv<T>(IEnumerable<T> items, List<(string Header, Func<T, object?> Selector)> columns)
    {
        using var memoryStream = new MemoryStream();
        // Excel needs UTF-8 BOM (Byte Order Mark) to display non-ASCII characters correctly
        var preamble = Encoding.UTF8.GetPreamble();
        memoryStream.Write(preamble, 0, preamble.Length);

        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
        {
            // Write Headers
            var headerLine = new List<string>();
            foreach (var col in columns)
            {
                headerLine.Add(EscapeCsv(col.Header));
            }
            writer.WriteLine(string.Join(",", headerLine));

            // Write Data Rows
            foreach (var item in items)
            {
                var rowData = new List<string>();
                foreach (var col in columns)
                {
                    var val = col.Selector(item);
                    var strVal = val?.ToString() ?? string.Empty;
                    rowData.Add(EscapeCsv(strVal));
                }
                writer.WriteLine(string.Join(",", rowData));
            }

            writer.Flush();
        }

        return memoryStream.ToArray();
    }

    private static string EscapeCsv(string val)
    {
        if (string.IsNullOrEmpty(val))
            return string.Empty;

        // If the value contains quotes, commas, or newlines, escape it
        if (val.Contains("\"") || val.Contains(",") || val.Contains("\n") || val.Contains("\r"))
        {
            return "\"" + val.Replace("\"", "\"\"") + "\"";
        }

        return val;
    }
}
