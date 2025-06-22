using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace AplikacjaAndroid
{
    public partial class CommentPopupContext(ICommentService commentService, UserStorage userStorage) : ObservableObject
    {
        [ObservableProperty]
        private CommentBook _comment;

        [ObservableProperty]
        bool _isEditMode = false;

        private TaskCompletionSource<CommentBook?>? _tcs;

        private SnackbarOptions _succesSnackBarOption = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#059669"),
            ActionButtonTextColor = Colors.White,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            TextColor = Colors.White,
        };

        private SnackbarOptions _errorSnackBarOption = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#f43f5e"),
            ActionButtonTextColor = Colors.White,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            TextColor = Colors.White
        };

        public Task<CommentBook?> ShowAsync()
        {
            _tcs = new TaskCompletionSource<CommentBook?>();
            return _tcs.Task;
        }

        public void ReturnResult(CommentBook? result)
        {
            _tcs?.TrySetResult(result);
        }

        [RelayCommand]
        public async Task UpdateComment()
        {
            if (Comment != null)
            {
                IsEditMode = true;

            }

            //await Snackbar.Make("Updated.", visualOptions: _succesSnackBarOption).Show();
            //await Close();
        }

        [RelayCommand]
        public async Task DeleteComment()
        {
            if (Comment != null)
            {
                
            }

            await Snackbar.Make("Deleted.", visualOptions: _succesSnackBarOption).Show();
            await Close();
        }


        [RelayCommand]
        public async Task Close()
        {
            ReturnResult(null);
            await MopupService.Instance.PopAllAsync();
        }
        public void LoadContext(CommentBook comment)
        {
            Comment = comment;
        }

        [RelayCommand]
        public async Task ConfirmEdit()
        {
            // Wywołaj update do API, zamknij popup
            if (!Comment.IsValid())
                return;

            var result = await commentService.UpdateCommentAsync(Comment.MapToUpdateModel(Comment));

            if (result.StatusCode == 200 && result.Data != null)
            {
                await Snackbar.Make("Updated.", visualOptions: _succesSnackBarOption).Show();
                ReturnResult(result.Data);
            }

            IsEditMode = false;
            await Close();
        }

        [RelayCommand]
        public void CancelEdit()
        {
            IsEditMode = false;
        }
    }
}
