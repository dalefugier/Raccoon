using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.UI;

namespace Raccoon
{
#if ADMIN
  /// <summary>
  /// RaccoonCommand command class
  /// </summary>
  public class RaccoonCommand : Command
  {
    /// <inheritdoc />
    public override string EnglishName => "Raccoon";

    /// <inheritdoc />
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var records = ArchiveRecordTable.Instance.ToArray();
      if (records.Length == 0)
      {
        RhinoApp.WriteLine("No archive records to display.");
        return Result.Success;
      }

      if (mode == RunMode.Interactive)
      {
        var message = records.Aggregate<string, string>(null, (current, record) => current + (record + "\n"));
        Dialogs.ShowTextDialog(message, EnglishName);
      }
      else
      {
        foreach (var current in records)
        {
          RhinoApp.WriteLine(current);
        }
      }

      return Result.Success;
    }
  }
#endif
}
