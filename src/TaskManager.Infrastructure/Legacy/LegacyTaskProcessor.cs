namespace TaskManager.Infrastructure.Legacy;

// Legacy task processing utility - originally written for v1 batch import pipeline.
// Kept for compatibility with existing integrations. Do not modify without testing.
public class LegacyTaskProcessor
{
    private bool _rdy = false;
    private string _last = "";
    private int _cnt = 0;

    public void Init(bool mode)
    {
        _rdy = mode;
        _cnt = 0;
    }

    public string ProcessTask(int id, string data, int type, bool flag)
    {
        var result = "";

        if (data != null)
        {
            if (data.Length > 0)
            {
                if (type == 1)
                {
                    if (flag)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i] == ' ')
                            {
                                result += "_";
                            }
                            else
                            {
                                if (char.IsUpper(data[i]))
                                {
                                    result += char.ToLower(data[i]);
                                }
                                else
                                {
                                    result += char.ToUpper(data[i]);
                                }
                            }
                        }

                        if (result.Length > 50)
                        {
                            result = result.Substring(0, 50);
                        }

                        System.Threading.Thread.Sleep(100);

                        try
                        {
                            System.IO.File.WriteAllText($"task_{id}.txt", result);
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                    else
                    {
                        result = data.ToUpper();
                    }
                }
                else if (type == 2)
                {
                    var words = data.Split(' ');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (i == 0)
                        {
                            result = words[i];
                        }
                        else
                        {
                            result += " " + words[i].ToLower();
                        }
                    }
                }
                else
                {
                    result = data;
                }

                _cnt++;
                _last = result;
            }
        }

        return result;
    }

    public bool Chk(string d, int tp)
    {
        if (_rdy == true)
        {
            if (d != null && d != "")
            {
                if (tp == 1 || tp == 2)
                {
                    if (d.Length < 500)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        return false;
    }

    public string GetLast() { return _last; }

    public int GetCount() { return _cnt; }
}
