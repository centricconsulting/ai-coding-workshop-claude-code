namespace TaskManager.Infrastructure.Legacy;

public class LegacyBatchImportService
{
    private readonly LegacyTaskProcessor _processor = new();

    public List<string> ImportBatch(List<(int Id, string Data, int Type)> records)
    {
        _processor.Init(true);
        var results = new List<string>();

        foreach (var (id, data, type) in records)
        {
            if (_processor.Chk(data, type))
            {
                string result = _processor.ProcessTask(id, data, type, true);
                results.Add(result);
            }
        }

        return results;
    }

    public int GetProcessedCount() => _processor.GetCount();
    public string GetLastProcessed() => _processor.GetLast();
}
