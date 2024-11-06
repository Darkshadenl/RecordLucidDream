using System;
using System.Diagnostics;

namespace HouseScraping.Helpers;

public static class CacheHelper
{
    public static void ClearCache()
    {
        try
        {
            var cacheDir = FileSystem.CacheDirectory;
            if (Directory.Exists(cacheDir))
            {
                foreach (var file in Directory.GetFiles(cacheDir))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting file {file}: {ex.Message}");
                    }
                }

                foreach (var dir in Directory.GetDirectories(cacheDir))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting directory {dir}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error clearing cache: {ex.Message}");
        }
    }
}

