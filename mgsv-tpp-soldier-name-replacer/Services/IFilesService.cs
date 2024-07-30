using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MgsvTppSoldierNameReplacer.Services
{
    internal interface IFilesService
    {
        public Task<IStorageFolder?> SelectFolderAsync(string dialogWindowTitle, bool allowMultiple, string? suggestedStartLocation);
        public Task<IStorageFile?> SelectFileAsync(string dialogWindowTitle, bool allowMultiple, string? suggestedStartLocation);
    }
}
