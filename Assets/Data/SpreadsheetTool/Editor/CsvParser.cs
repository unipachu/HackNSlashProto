#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;

public static class CsvParser
{
    public static List<List<string>> Parse(string csv)
    {
        List<List<string>> rows = new();
        List<string> row = new();
        StringBuilder cell = new();

        bool insideQuotes = false;

        for (int i = 0; i < csv.Length; i++)
        {
            char c = csv[i];

            if (insideQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < csv.Length && csv[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = false;
                    }
                }
                else
                {
                    cell.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    insideQuotes = true;
                }
                else if (c == ',')
                {
                    row.Add(cell.ToString());
                    cell.Clear();
                }
                else if (c == '\n')
                {
                    row.Add(cell.ToString());
                    cell.Clear();

                    rows.Add(row);
                    row = new List<string>();
                }
                else if (c == '\r')
                {
                    // Ignore.
                }
                else
                {
                    cell.Append(c);
                }
            }
        }

        row.Add(cell.ToString());

        if (row.Count > 1 || row[0].Length > 0)
            rows.Add(row);

        return rows;
    }
}
#endif