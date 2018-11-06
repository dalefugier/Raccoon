using Rhino;
using Rhino.FileIO;
using Rhino.PlugIns;

namespace Raccoon
{
  /// <summary>
  /// RaccoonPlugIn plug-in class
  /// </summary>
  public class RaccoonPlugIn : PlugIn
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    public RaccoonPlugIn()
    {
      Instance = this;
    }

    /// <summary>
    /// Gets the only instance of the RaccoonPlugIn plug-in.
    /// </summary>
    public static RaccoonPlugIn Instance
    {
      get; private set;
    }

    /// <inheritdoc />
    public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

    /// <inheritdoc />
    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
      // Add an event handler so we know when documents are closed.
      RhinoDoc.CloseDocument += OnCloseDocument;
      return LoadReturnCode.Success;
    }

    /// <summary>
    /// OnCloseDocument event handler
    /// </summary>
    private static void OnCloseDocument(object sender, DocumentEventArgs e)
    {
      ArchiveRecordTable.Instance.OnCloseDocument(sender, e);
    }

    /// <inheritdoc />
    protected override bool ShouldCallWriteDocument(FileWriteOptions options)
    {
      return ArchiveRecordTable.Instance.ShouldCallWriteDocument(options);
    }

    /// <inheritdoc />
    protected override void WriteDocument(RhinoDoc doc, BinaryArchiveWriter archive, FileWriteOptions options)
    {
      ArchiveRecordTable.Instance.WriteDocument(doc, archive, options);
    }

    /// <inheritdoc />
    protected override void ReadDocument(RhinoDoc doc, BinaryArchiveReader archive, FileReadOptions options)
    {
      ArchiveRecordTable.Instance.ReadDocument(doc, archive, options);
    }
  }
}