using System.Collections.Generic;
using Rhino;
using Rhino.FileIO;

namespace Raccoon
{
  internal class ArchiveRecordTable
  {
    #region Private Data Members

    // Major and minor version numbers of our document user data
    private const int MAJOR = 1;
    private const int MINOR = 0;

    // The archive record list
    private readonly List<ArchiveRecord> m_records;

    #endregion

    #region Construction

    /// <summary>
    /// Private constructor
    /// </summary>
    private ArchiveRecordTable()
    {
      m_records = new List<ArchiveRecord>();
    }

    /// <summary>
    /// The one and only ArchiveRecordTable object
    /// </summary>
    private static ArchiveRecordTable g_table;

    /// <summary>
    /// Returns the one and only ArchiveRecordTable object
    /// </summary>
    public static ArchiveRecordTable Instance => g_table ?? (g_table = new ArchiveRecordTable());

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns archive records as an array of strings
    /// </summary>
    public string[] ToArray()
    {
      var list = new List<string>();
      // Write records as strings in reverse order
      for (var i = m_records.Count - 1; i >= 0; i--)
      {
        list.Add(m_records[i].ToString());
      }
      return list.ToArray();
    }

    /// <summary>
    /// Called whenever a Rhino is about to save a .3dm file.
    /// </summary>
    public bool ShouldCallWriteDocument(FileWriteOptions options)
    {
      return false == options.WriteGeometryOnly && false == options.WriteSelectedObjectsOnly;
    }

    /// <summary>
    /// Called when Rhino is saving a .3dm file to allow the plug-in to save document user data.
    /// </summary>
    public void WriteDocument(RhinoDoc doc, BinaryArchiveWriter archive, FileWriteOptions options)
    {
      // Before writing, add a new record
      var archive_record = new ArchiveRecord(true);
      m_records.Add(archive_record);

      // Write version chunk
      archive.Write3dmChunkVersion(MAJOR, MINOR);
      
      // Write number of records
      archive.WriteInt(m_records.Count);

      // Write records
      foreach (var record in m_records)
      {
        record.Write(archive);
      }
    }

    /// <summary>
    /// Called whenever a Rhino document is being loaded and plug-in user data was
    /// encountered written by a plug-in with this plug-in's GUID.
    /// </summary>
    public void ReadDocument(RhinoDoc doc, BinaryArchiveReader archive, FileReadOptions options)
    {
      var open_mode = !options.ImportMode && !options.ImportReferenceMode;

      // Read version chunk
      archive.Read3dmChunkVersion(out var major, out var minor);
      if (major == 1 && minor >= 0)
      {
        // Read number of records
        var count = archive.ReadInt();

        // Read records
        for (var i = 0; i < count; i++)
        {
          var record = new ArchiveRecord(false);
          if (record.Read(archive))
          {
            if (open_mode)
              m_records.Add(record);
          }
        }
      }
    }

    /// <summary>
    /// OnCloseDocument event handler
    /// </summary>
    public void OnCloseDocument(object sender, DocumentEventArgs e)
    {
      // When the docment is closed, clear the table.
      m_records.Clear();
    }

    #endregion
  }
}
