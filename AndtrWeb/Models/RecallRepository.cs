
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AndtrWeb.Models;

public static class RecallRepository
{
    private class Indexed
    {
        public RecallNotice Notice = null!;
        public string Title = "", Group = "", Firm = "", Reason = "", Desc = "", RecNo = "";
    }

    private static List<Indexed> _index = new();
    public static int Count => _index.Count;

    public static IEnumerable<RecallNotice> All => _index.Select(i => i.Notice);
    public static RecallNotice? Find(int id) => _index.Select(i => i.Notice).FirstOrDefault(n => n.Id == id);

    // "Smarter search": a word also matches related words used in the dataset
    private static readonly Dictionary<string, string[]> Synonyms = new()
    {
        ["heart"]     = new[] { "cardiac", "cardio", "cardiopulmonary", "pacemaker", "defibrillator", "cardioplegia" },
        ["breathing"] = new[] { "ventilator", "respiratory", "tracheal", "airway", "oxygen" },
        ["lung"]      = new[] { "respiratory", "ventilator", "pulmonary" },
        ["sugar"]     = new[] { "glucose", "insulin", "diabetes" },
        ["diabetes"]  = new[] { "glucose", "insulin" },
        ["pump"]      = new[] { "infusion" },
        ["scan"]      = new[] { "mri", "imaging", "ct", "x-ray" },
        ["leak"]      = new[] { "seal", "leakage", "leaking" },
        ["fire"]      = new[] { "overheating", "burn", "ignition" },
        ["needle"]    = new[] { "syringe", "cannula" },
        ["battery"]   = new[] { "power", "charging" },
        ["software"]  = new[] { "firmware", "program" },
        ["tube"]      = new[] { "catheter", "tubing" },
    };

    public static void Load(string path)
    {
        var list = new List<Indexed>();
        if (!File.Exists(path)) { _index = list; return; }

        var rows = ParseCsv(File.ReadAllText(path, Encoding.UTF8));
        if (rows.Count < 2) { _index = list; return; }

        var header = rows[0].Select(h => h.Trim().TrimStart('﻿').ToLowerInvariant()).ToList();
        string Get(List<string> row, string col)
        {
            var i = header.IndexOf(col.ToLowerInvariant());
            return i >= 0 && i < row.Count ? row[i].Trim() : "";
        }
        DateTime? Date(string s) =>
            DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : null;

        foreach (var row in rows.Skip(1))
        {
            if (row.All(string.IsNullOrWhiteSpace)) continue;
            int.TryParse(Get(row, "Id"), out var id);
            int.TryParse(Get(row, "RecallClass"), out var cls);

            var n = new RecallNotice
            {
                Id = id,
                ProductCode = Get(row, "ProductCode"),
                ProductCodeName = Get(row, "ProductCodeName"),
                RegulationNumber = Get(row, "RegulationNumber"),
                RecallNumber = Get(row, "RecallNumber"),
                ProductDescription = Get(row, "ProductDescription"),
                TradeName = Get(row, "TradeName"),
                RecallClass = cls,
                ClassifiedDate = Date(Get(row, "ClassifiedDate")),
                PostedDate = Date(Get(row, "PostedDate")),
                TerminationDate = Date(Get(row, "TerminationDate")),
                FirmName = Get(row, "FirmName"),
                RecallReason = Get(row, "RecallReason")
            };

            list.Add(new Indexed
            {
                Notice = n,
                Title = n.DisplayName.ToLowerInvariant(),
                Group = n.ProductCodeName.ToLowerInvariant(),
                Firm = n.FirmName.ToLowerInvariant(),
                Reason = n.RecallReason.ToLowerInvariant(),
                Desc = n.ProductDescription.ToLowerInvariant(),
                RecNo = n.RecallNumber.ToLowerInvariant()
            });
        }
        _index = list;
    }

    public static List<SearchHit> Search(string? query, int? recallClass, string? status)
    {
        IEnumerable<Indexed> pool = _index;
        if (recallClass is >= 1 and <= 3) pool = pool.Where(i => i.Notice.RecallClass == recallClass);
        if (status == "ongoing") pool = pool.Where(i => i.Notice.Ongoing);
        if (status == "terminated") pool = pool.Where(i => !i.Notice.Ongoing);

        var words = Regex.Matches((query ?? "").ToLowerInvariant(), @"[a-z0-9][a-z0-9\-\.]*")
                         .Select(m => m.Value).Where(w => w.Length > 1).Distinct().ToList();

        // No search words: show everything, newest first
        if (words.Count == 0)
            return pool.OrderByDescending(i => i.Notice.ClassifiedDate)
                       .Select(i => new SearchHit(i.Notice, 0)).ToList();

        var hits = new List<SearchHit>();
        foreach (var i in pool)
        {
            var total = 0;
            var allWordsMatched = true;
            foreach (var w in words)
            {
                // each typed word is matched together with its synonyms; every typed word must match something
                var terms = new List<string> { w };
                if (Synonyms.TryGetValue(w, out var syn)) terms.AddRange(syn);

                var best = terms.Max(t => Score(i, t));
                if (best == 0) { allWordsMatched = false; break; }
                total += best;
            }
            if (allWordsMatched) hits.Add(new SearchHit(i.Notice, total));
        }
        return hits.OrderByDescending(h => h.Score).ThenByDescending(h => h.Notice.ClassifiedDate).ToList();
    }

    private static int Score(Indexed i, string t)
    {
        var s = 0;
        if (i.RecNo.Contains(t)) s += 5;
        if (i.Title.Contains(t)) s += 4;
        if (i.Group.Contains(t)) s += 3;
        if (i.Firm.Contains(t)) s += 2;
        if (i.Reason.Contains(t)) s += 2;
        if (i.Desc.Contains(t)) s += 1;
        return s;
    }

    // Small CSV parser that handles quoted fields containing commas and line breaks
    private static List<List<string>> ParseCsv(string text)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (inQuotes)
            {
                if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                else if (c == '"') inQuotes = false;
                else field.Append(c);
            }
            else if (c == '"') inQuotes = true;
            else if (c == ',') { row.Add(field.ToString()); field.Clear(); }
            else if (c == '\n' || c == '\r')
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(field.ToString()); field.Clear();
                rows.Add(row); row = new List<string>();
            }
            else field.Append(c);
        }
        if (field.Length > 0 || row.Count > 0) { row.Add(field.ToString()); rows.Add(row); }
        return rows;
    }
}
