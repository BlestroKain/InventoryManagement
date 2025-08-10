using System;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RapiMesa.Utility
{
    public static class ExportHelper
    {
        public static void ExportToFile(DataGridView grid, string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".csv")
            {
                ExportToCsv(grid, path);
            }
            else if (ext == ".pdf")
            {
                ExportToPdf(grid, path);
            }
        }

        private static void ExportToCsv(DataGridView grid, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    if (i > 0) writer.Write(",");
                    writer.Write('"' + grid.Columns[i].HeaderText.Replace("\"", "\"\"") + '"');
                }
                writer.WriteLine();

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        if (i > 0) writer.Write(",");
                        var val = row.Cells[i].Value?.ToString() ?? string.Empty;
                        writer.Write('"' + val.Replace("\"", "\"\"") + '"');
                    }
                    writer.WriteLine();
                }
            }
        }

        private static void ExportToPdf(DataGridView grid, string path)
        {
            var table = new PdfPTable(grid.Columns.Count);
            foreach (DataGridViewColumn col in grid.Columns)
            {
                table.AddCell(new Phrase(col.HeaderText));
            }
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    table.AddCell(new Phrase(cell.Value?.ToString() ?? string.Empty));
                }
            }

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();
                doc.Add(table);
                doc.Close();
            }
        }
    }
}
