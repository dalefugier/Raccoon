using System;
using System.IO;
using System.Security.Principal;
using System.Text;
using Rhino.FileIO;

namespace Raccoon
{
  /// <summary>
  /// ArchiveRecord class
  /// </summary>
  internal class ArchiveRecord
  {
    #region Private Data Members

    // Major and minor version numbers of our document user data
    private const int MAJOR = 1;
    private const int MINOR = 0;

    #endregion

    #region Construction

    /// <summary>
    /// Public constructor
    /// </summary>
    public ArchiveRecord(bool create)
    {
      if (create)
      {
        MachineName = Environment.MachineName;

        var identity = WindowsIdentity.GetCurrent();
        UserName = identity.Name;
        if (string.IsNullOrEmpty(UserName))
          UserName = Environment.UserDomainName + Path.DirectorySeparatorChar + Environment.UserName;

        Date = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
      }
    }

    #endregion

    #region Public Data Members

    /// <summary>
    /// Gets the machine name
    /// </summary>
    public string MachineName { get; set; }

    /// <summary>
    /// Get the user name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Get the date and time of the archive
    /// </summary>
    public string Date { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// ToString override
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return $"{Date},{MachineName},{UserName}";
    }

    /// <summary>
    /// Write the record to a binary archive
    /// </summary>
    public bool Read(BinaryArchiveReader archive)
    {
      // Read the version chunk
      archive.Read3dmChunkVersion(out var major, out var minor);
      if (major == 1 && minor >= 0)
      {
        // Read the MachineName
        var bytes = archive.ReadCompressedBuffer();
        MachineName = GetString(bytes);

        // Read the UserName
        bytes = archive.ReadCompressedBuffer();
        UserName = GetString(bytes);

        // Read the Date
        bytes = archive.ReadCompressedBuffer();
        Date = GetString(bytes);
      }
      return !archive.ReadErrorOccured;
    }

    /// <summary>
    /// Read the record from a binary archive
    /// </summary>
    public bool Write(BinaryArchiveWriter archive)
    {
      // Write the version chunk
      archive.Write3dmChunkVersion(MAJOR, MINOR);

      // Write the MachineName
      var bytes = GetBytes(MachineName);
      archive.WriteCompressedBuffer(bytes);

      // Write the UserName
      bytes = GetBytes(UserName);
      archive.WriteCompressedBuffer(bytes);

      // Write the Date
      bytes = GetBytes(Date);
      archive.WriteCompressedBuffer(bytes);

      return !archive.WriteErrorOccured;
    }

    /// <summary>
    /// Converts a string to an array of bytes
    /// </summary>
    private static byte[] GetBytes(string src)
    {
      return Encoding.UTF8.GetBytes(src);
    }

    /// <summary>
    /// Converts an array of bytes to a string
    /// </summary>
    private static string GetString(byte[] src)
    {
      return Encoding.UTF8.GetString(src);
    }

    #endregion
  }
}
