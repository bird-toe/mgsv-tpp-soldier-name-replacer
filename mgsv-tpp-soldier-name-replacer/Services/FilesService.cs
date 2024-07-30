using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MgsvTppSoldierNameReplacer.Services
{
    internal class FilesService : IFilesService
    {
        private readonly Window _target;

        public FilesService(Window target)
        {
            _target = target;
        }

        public async Task<IStorageFolder?> SelectFolderAsync(string dialogWindowTitle, bool allowMultiple, string? suggestedStartLocation)
        {
            IStorageFolder? startFolder = null;
            if (suggestedStartLocation != null)
            {
                startFolder = await _target.StorageProvider.TryGetFolderFromPathAsync(suggestedStartLocation);
            }
            var result = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = dialogWindowTitle,
                AllowMultiple = allowMultiple,
                SuggestedStartLocation = startFolder
            });
            return result.FirstOrDefault();
        }

        public async Task<IStorageFile?> SelectFileAsync(string dialogWindowTitle, bool allowMultiple, string? suggestedStartLocation)
        {
            IStorageFolder? startFolder = null;
            if (suggestedStartLocation != null)
            {
                startFolder = await _target.StorageProvider.TryGetFolderFromPathAsync(suggestedStartLocation);
            }
            var result = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = dialogWindowTitle,
                AllowMultiple = allowMultiple,
                SuggestedStartLocation = startFolder,
                FileTypeFilter = new [] { new FilePickerFileType("Executables") { Patterns = new[] { "*.exe" } } }
            });
            return result.FirstOrDefault();
        }
    }
}
